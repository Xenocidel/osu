﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osu.Framework.Graphics;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using OpenTK.Graphics;
using System;
using OpenTK;
using osu.Framework.Input;
using osu.Framework.Extensions.IEnumerableExtensions;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using osu.Game.Modes.UI;
using osu.Game.Modes;
using osu.Game.Modes.Objects;
using osu.Game.Modes.Osu.Objects;
using osu.Framework.MathUtils;
using osu.Game.Beatmaps;
using osu.Desktop.VisualTests.Beatmaps;
using osu.Game.Database;
using osu.Framework.Timing;
using osu.Framework.Graphics.Primitives;
using osu.Game.Modes.Scoring;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Beatmaps.Timing;
using osu.Framework.Configuration;
using osu.Framework.Threading;
using osu.Game.Users;

namespace osu.Desktop.VisualTests.Tests
{
    internal class TestCaseMultiReplayPlayer : TestCase
    {
        public override void Reset()
        {
            base.Reset();

            MultiReplayPlayer player;
            Add(player = new MultiReplayPlayer(createTestBeatmap())
            {
                Clock = new FramedClock()
            });

            const int max_players = 3;
            for (int i = 0; i < max_players; i++)
            {
                var score = new Score
                {
                    User = new User { Username = $@"Test player" }
                };

                player.Teams.BlueTeam.AddPlayer(score);
                player.Teams.RedTeam.AddPlayer(score);
            }
        }

        private WorkingBeatmap createTestBeatmap()
        {
            Ruleset ruleset = Ruleset.GetRuleset(PlayMode.Taiko);

            List<HitObject> objects = new List<HitObject>();

            int time = 500;
            for (int i = 0; i < 20; i++)
            {
                objects.Add(new HitCircle
                {
                    StartTime = time,
                    Position = new Vector2(RNG.Next(0, 512), RNG.Next(0, 384)),
                    Scale = RNG.NextSingle(0.5f, 1.0f),
                });

                time += RNG.Next(50, 500);
            }

            Beatmap b = new Beatmap
            {
                HitObjects = objects,
                BeatmapInfo = new BeatmapInfo
                {
                    Difficulty = new BeatmapDifficulty(),
                    Metadata = new BeatmapMetadata
                    {
                        Artist = @"Unknown",
                        Title = @"Sample Beatmap",
                        Author = @"peppy",
                    }
                }
            };

            b.TimingInfo.ControlPoints.Add(new ControlPoint
            {
                Time = 0,
                TimeSignature = TimeSignatures.SimpleQuadruple,
                BeatLength = 1000
            });

            return new TestWorkingBeatmap(b);
        }

        internal class MultiReplayPlayer : OsuScreen
        {
            protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

            private StarCounter blueStarCounter;
            private StarCounter redStarCounter;
            private Sprite backgroundSprite;

            public TeamContainer Teams;

            private int teamsCompleted;

            private readonly WorkingBeatmap beatmap;
            
            public MultiReplayPlayer(WorkingBeatmap beatmap)
            {
                this.beatmap = beatmap;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures, OsuColour colours)
            {
                Children = new Drawable[]
                {
                    backgroundSprite = new Sprite
                    {
                        FillMode = FillMode.Fill,
                        Texture = textures.Get(@"Backgrounds/Tournament/background")
                    },
                    blueStarCounter = new StarCounter
                    {
                        Origin = Anchor.CentreLeft,
                        Position = new Vector2(30, 30),
                        MaxCount = 3,
                        AccentColour = colours.BlueDarker
                    },
                    redStarCounter = new StarCounter
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.CentreRight,
                        Position = new Vector2(-30, 30),
                        MaxCount = 3,
                        Direction = StarCounterDirection.RightToLeft,
                        AccentColour = colours.PinkDarker
                    },
                    Teams = new TeamContainer(4, beatmap),
                };

                Teams.RedTeam.OnTeamCompletion = onTeamComplete;
                Teams.BlueTeam.OnTeamCompletion = onTeamComplete;
            }

            private void onTeamComplete()
            {
                teamsCompleted++;

                if (teamsCompleted == Teams.Children.Count())
                {
                    // Completion sequence
                    onTeamWon(Teams.RedTeam.Score > Teams.BlueTeam.Score);    
                }
            }

            private void onTeamWon(bool teamRed)
            {
                
            }
        }

        internal class TeamContainer : Container<Team>
        {
            public Team BlueTeam;
            public Team RedTeam;

            private readonly WorkingBeatmap beatmap;

            public TeamContainer(int playersPerTeam, WorkingBeatmap beatmap)
            {
                this.beatmap = beatmap;

                RelativeSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(BeatmapDatabase database)
            {
                // To achieve proper masking of the taiko playfield, we use two vertically-relative columns
                // and apply the offset of the play fields to the contents of the columns
                Children = new[]
                {
                    BlueTeam = new Team(false, beatmap)
                    {
                        Name = "Blue team column",
                    },
                    RedTeam = new Team(true, beatmap)
                    {
                        Name = "Red team column",
                        RelativePositionAxes = Axes.X,
                        X = 0.5f
                    }
                };

                BlueTeam.BindTeamColumn(RedTeam);
            }
        }

        internal class Team : Container, IHasAccentColour
        {
            private const float player_container_start = 95;
            private const float player_container_height = 480;
            private const float player_spacing = 14;

            public Action OnTeamFail;
            public Action OnTeamCompletion;

            public Color4 AccentColour
            {
                get { return teamScoreCounter.AccentColour; }
                set
                {
                    teamScoreCounter.AccentColour = value;
                    scoreDiffCounter.AccentColour = value;
                }
            }

            public readonly BindableInt Score = new BindableInt();

            public readonly FlowContainer<Player> Players;
            private readonly ScoreCounter teamScoreCounter;
            private readonly ScoreCounter scoreDiffCounter;

            private readonly Sprite flag;

            private readonly OsuSpriteText name;

            private Team otherColumn;

            private int playersCompleted;
            private int playersFailed;

            private readonly WorkingBeatmap beatmap;
            private readonly bool teamRed;

            public Team(bool teamRed, WorkingBeatmap beatmap)
            {
                this.beatmap = beatmap;
                this.teamRed = teamRed;

                RelativeSizeAxes = Axes.Both;
                Width = 0.5f;
                Padding = new MarginPadding { Top = player_container_start };
                Masking = true;

                Children = new Drawable[]
                {
                    Players = new FillFlowContainer<Player>()
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = player_container_height,
                        Spacing = new Vector2(0, player_spacing)
                    },
                    new FillFlowContainer
                    {
                        Name = "Score container",
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.Centre,
                        Y = -150,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            teamScoreCounter = new ScoreCounter(6)
                            {
                                Name = "Combined score",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                TextSize = 36
                            },
                            scoreDiffCounter = new ScoreCounter
                            {
                                Name = "Score diff",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                TextSize = 14,
                                Alpha = 0
                            }
                        }
                    }
                };

                Score.ValueChanged += v => teamScoreCounter.Current.Value = v;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                if (teamRed)
                {
                    teamScoreCounter.AccentColour = colours.PinkLighter;
                    scoreDiffCounter.AccentColour = colours.PinkLighter;
                }
                else
                {
                    teamScoreCounter.AccentColour = colours.BlueLighter;
                    scoreDiffCounter.AccentColour = colours.BlueLighter;
                }
            }

            public void AddPlayer(Score score)
            {
                Players.Add(new Player(teamRed, score, beatmap)
                {
                    OnCompletion = playerCompleted,
                    OnFail = playerFailed,
                });
                
                Players.Children.ForEach(p => p.Height = 1f / Players.Children.Count() - player_spacing / player_container_height);
            }

            public void BindTeamColumn(Team other)
            {
                if (other == null)
                    throw new ArgumentNullException(nameof(other));

                if (otherColumn != null)
                    return;

                otherColumn = other;
                other.BindTeamColumn(this);
            }

            private void playerCompleted()
            {
                playersCompleted++;

                // Don't perform OnTeamCompletion if the team has failed
                // This is needed because taiko performs OnFail at the end of the map
                if (playersFailed == Players.Children.Count())
                    return;

                if (playersCompleted == Players.Children.Count())
                    OnTeamCompletion?.Invoke();
            }

            private void playerFailed()
            {
                playersFailed++;

                if (playersFailed == Players.Children.Count())
                    OnTeamFail?.Invoke();
            }

            protected override void Update()
            {
                base.Update();

                Score.Value = (int)Players.Children.Sum(p => p.Score);

                if (otherColumn != null)
                    scoreDiffCounter.Current.Value = Math.Min(0, Score.Value - otherColumn.Score.Value);
                scoreDiffCounter.FadeTo(scoreDiffCounter.Current == 0 ? 0 : 1, 200);
            }
        }

        internal class Player : Container
        {
            public Action OnCompletion;
            public Action OnFail;

            public readonly Bindable<double> Score = new Bindable<double>();

            private HitRenderer hitRenderer;
            private HudOverlay hudOverlay;
            private ScoreProcessor scoreProcessor;

            public Player(bool teamRed, Score score, WorkingBeatmap beatmap)
            {
                RelativeSizeAxes = Axes.Both;

                Ruleset ruleset = Ruleset.GetRuleset(PlayMode.Taiko);

                Children = new Drawable[]
                {
                    hitRenderer = ruleset.CreateHitRendererWith(beatmap),
                    hudOverlay = new HudOverlay(teamRed)
                    {
                        PlayerName = score.User.Username
                    }
                };

                scoreProcessor = hitRenderer.CreateScoreProcessor();

                hitRenderer.Origin = Anchor.BottomLeft;
                hitRenderer.Anchor = Anchor.BottomLeft;
                hitRenderer.RelativeSizeAxes = Axes.Both;
                hitRenderer.Height = 0.65f;
                hitRenderer.Margin = new MarginPadding { Bottom = 5 };
                hitRenderer.AspectAdjust = false;

                hitRenderer.OnAllJudged += () => OnCompletion?.Invoke();
                scoreProcessor.Failed += () => OnFail?.Invoke();

                Score.BindTo(scoreProcessor.TotalScore);
                hudOverlay.BindProcessor(scoreProcessor);
            }
        }

        internal class HudOverlay : StandardHudOverlay
        {
            private string playerName;
            public string PlayerName
            {
                get { return playerName; }
                set
                {
                    playerName = value;
                    nameText.Text = value;
                }
            }

            private readonly OsuSpriteText nameText;

            private readonly bool teamRed;

            public HudOverlay(bool teamRed)
            {
                this.teamRed = teamRed;

                Add(new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Position = new Vector2(35, 0),
                    Children = new Drawable[]
                    {
                        nameText = new OsuSpriteText
                        {
                            TextSize = 14,
                            Font = "Exo2.0-RegularItalic",
                            Text = "Name here"
                        }
                    }
                });
            }

            protected override HealthDisplay CreateHealthDisplay() => new StandardHealthDisplay
            {
                Origin = Anchor.BottomLeft,
                Anchor = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Size = new Vector2(1, 5)
            };

            protected override ScoreCounter CreateScoreCounter() => new ScoreCounter(6)
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                TextSize = 21,
                Position = new Vector2(-30, 0)
            };

            protected override RollingCounter<int> CreateComboCounter() => new SimpleComboCounter
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                TextSize = 14,
                Position = new Vector2(-200, 0)
            };

            protected override RollingCounter<double> CreateAccuracyCounter() => new PercentageCounter
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                TextSize = 10,
                Position = new Vector2(-260, 0)
            };

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                var shd = HealthDisplay as StandardHealthDisplay;

                if (teamRed)
                {
                    ComboCounter.AccentColour = colours.PinkLighter;
                    ScoreCounter.AccentColour = colours.PinkLighter;
                    AccuracyCounter.AccentColour = colours.PinkLighter;
                    shd.AccentColour = colours.PinkLighter;
                    shd.GlowColour = colours.PinkDarker;

                    nameText.Colour = colours.PinkLighter;
                }
                else
                    nameText.Colour = colours.BlueLighter;
            }
        }

        internal class StarCounter : Container, IHasAccentColour
        {
            private readonly FlowContainer<Star> starContainer;
            private readonly Container<ProxyDrawable> overlayLayer;

            private Dictionary<Star, ProxyDrawable> proxyMap = new Dictionary<Star, ProxyDrawable>();

            public StarCounter()
            {
                AutoSizeAxes = Axes.Both;

                Children = new Drawable[]
                {
                    starContainer = new FillFlowContainer<Star>
                    {
                        AutoSizeAxes = Axes.Both
                    },
                    overlayLayer = new Container<ProxyDrawable>
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                };
            }

            private Color4 accentColour;
            public Color4 AccentColour
            {
                get { return accentColour; }
                set
                {
                    if (accentColour == value)
                        return;
                    accentColour = value;

                    starContainer.Children.ForEach(s => s.AccentColour = accentColour);
                }
            }

            private StarCounterDirection direction;
            public StarCounterDirection Direction
            {
                get { return direction; }
                set
                {
                    direction = value;

                    starContainer.Children.ForEach(c =>
                    {
                        c.Anchor = direction == StarCounterDirection.LeftToRight ? Anchor.TopLeft : Anchor.TopRight;
                        c.Origin = direction == StarCounterDirection.LeftToRight ? Anchor.TopLeft : Anchor.TopRight;
                    });
                }
            }

            private int maxCount;
            public int MaxCount
            {
                get { return maxCount; }
                set
                {
                    if (maxCount == value)
                        return;

                    if (value > maxCount)
                    {
                        // MaxCount has increased, add more stars
                        for (int i = maxCount; i < value; i++)
                        {
                            var newStar = new Star
                            {
                                Anchor = direction == StarCounterDirection.LeftToRight ? Anchor.TopLeft : Anchor.TopRight,
                                Origin = direction == StarCounterDirection.LeftToRight ? Anchor.TopLeft : Anchor.TopRight,
                                AccentColour = AccentColour
                            };

                            starContainer.Add(newStar);
                            overlayLayer.Add(newStar.CreateOverlayProxy());
                        }
                    }
                    else
                    {
                        // MaxCount has decreased, remove some stars
                        for (int i = maxCount; i > value; i--)
                            starContainer.Remove(starContainer.Children.Last());
                    }

                    maxCount = value;
                }
            }

            private int count;
            public int Count
            {
                get { return count; }
                set
                {
                    if (count == value)
                        return;

                    value = MathHelper.Clamp(value, 0, MaxCount);

                    if (value > count)
                    {
                        // Count has increased, fill stars
                        starContainer.Children.Take(value).ForEach(s => s.IsFilled = true);
                    }
                    else
                    {
                        // Count has decreased, unfill stars
                        starContainer.Children.Skip(value).Take(count - value).ForEach(s => s.IsFilled = false);
                    }

                    count = value;
                }
            }

            protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
            {
                if (args.Button == MouseButton.Left)
                {
                    Count++;
                    return true;
                }

                if (args.Button == MouseButton.Right)
                {
                    Count--;
                    return true;
                }

                return false;
            }

            private class Star : Container, IHasAccentColour
            {
                public const float STAR_SIZE = 28f;
                private const float glow_sigma = 12f;

                private readonly Container filledStarContainer;
                private readonly TextAwesome baseStar;
                private readonly BufferedContainer filledStarGlow;

                public Star()
                {
                    AutoSizeAxes = Axes.Both;

                    Children = new Drawable[]
                    {
                        baseStar = new TextAwesome
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = FontAwesome.fa_star_o,
                            TextSize = STAR_SIZE,
                            BlendingMode = BlendingMode.Additive
                        },
                        filledStarContainer = new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.Both,
                            Scale = new Vector2(0.001f), // Todo: Fix broken size (NaN?)
                            BypassAutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                filledStarGlow = new BufferedContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    AutoSizeAxes = Axes.Both,
                                    Scale = new Vector2(1.3f),
                                    BlurSigma = new Vector2(glow_sigma),
                                    CacheDrawnFrameBuffer = true,
                                    BlendingMode = BlendingMode.Additive,
                                    Alpha = 0,
                                    Children = new []
                                    {
                                        new TextAwesome
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.fa_star,
                                            TextSize = STAR_SIZE,
                                            Shadow = false
                                        },
                                    }
                                },
                                new TextAwesome
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = FontAwesome.fa_star,
                                    TextSize = STAR_SIZE
                                }
                            }
                        }
                    };
                }

                private Color4 accentColour;
                public Color4 AccentColour
                {
                    get { return accentColour; }
                    set
                    {
                        if (accentColour == value)
                            return;
                        accentColour = value;

                        baseStar.Colour = accentColour;
                        filledStarGlow.Colour = accentColour;
                    }
                }

                private bool isFilled;
                public bool IsFilled
                {
                    get { return isFilled; }
                    set
                    {
                        if (isFilled == value)
                            return;
                        isFilled = value;

                        DelayReset();
                        Flush(true);

                        if (isFilled)
                        {
                            filledStarContainer.ScaleTo(2f, 300, EasingTypes.OutBack);

                            Delay(200, true);
                            filledStarGlow.FadeIn(350, EasingTypes.OutSine);
                            filledStarGlow.RotateTo(filledStarGlow.Rotation + 1080, 12000);

                            Delay(3000, true);
                            filledStarGlow.FadeOut(400, EasingTypes.OutCubic);

                            Delay(100, true);
                            filledStarContainer.ScaleTo(1, 300, EasingTypes.InBack);
                        }
                        else
                            filledStarContainer.ScaleTo(0, 200);
                    }
                }

                public ProxyDrawable CreateOverlayProxy() => filledStarContainer.CreateProxy();
            }
        }

        internal enum StarCounterDirection
        {
            LeftToRight,
            RightToLeft
        }
    }
}
