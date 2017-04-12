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

namespace osu.Desktop.VisualTests.Tests
{
    internal class TestCaseMultiReplayPlayer : TestCase
    {
        public override void Reset()
        {
            base.Reset();

            Add(new MultiReplayPlayer()
            {
                Clock = new FramedClock()
            });
        }

        internal class MultiReplayPlayer : OsuScreen
        {
            protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

            private StarCounter blueStarCounter;
            private StarCounter redStarCounter;
            private Sprite backgroundSprite;

            [BackgroundDependencyLoader]
            private void load(TextureStore textures, OsuColour colours)
            {
                Children = new Drawable[]
                {
                    backgroundSprite = new Sprite
                    {
                        FillMode = FillMode.Fill
                    },
                    blueStarCounter = new StarCounter
                    {
                        Origin = Anchor.CentreLeft,
                        Position = new Vector2(28, 40),
                        MaxCount = 3
                    },
                    redStarCounter = new StarCounter
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.CentreRight,
                        Position = new Vector2(-28, 40),
                        MaxCount = 3,
                        Direction = StarCounterDirection.RightToLeft
                    },
                    new PlayerContainer(4)
                    {
                        Position = new Vector2(0, 135)
                    }
                };

                backgroundSprite.Texture = textures.Get(@"Backgrounds/Tournament/background");

                blueStarCounter.AccentColour = colours.Blue;
                redStarCounter.AccentColour = colours.Red;
            }
        }

        internal class PlayerContainer : FillFlowContainer
        {
            private FlowContainer<Player> bluePlayers;
            private FlowContainer<Player> redPlayers;

            public PlayerContainer(int playersPerTeam)
            {
                RelativeSizeAxes = Axes.X;

                Children = new[]
                {
                    bluePlayers = new FillFlowContainer<Player>()
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y
                    },
                    redPlayers = new FillFlowContainer<Player>()
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y
                    }
                };

                if (playersPerTeam > 2)
                {
                    // Tile teams horizontally
                    bluePlayers.Width = 0.5f;
                    redPlayers.Width = 0.5f;
                }

                for (int i = 0; i < playersPerTeam; i++)
                {
                    bluePlayers.Add(new Player(false));
                    redPlayers.Add(new Player(true));
                }
            }
        }

        internal class Player : Container
        {
            private readonly HitRenderer hitRenderer;
            private readonly HudOverlay hudOverlay;
            private readonly ScoreProcessor scoreProcessor;

            public Player(bool teamRed)
            {
                RelativeSizeAxes = Axes.X;
                Height = 140;

                Ruleset ruleset = Ruleset.GetRuleset(PlayMode.Taiko);

                List<HitObject> objects = new List<HitObject>();

                int time = 500;
                for (int i = 0; i < 100; i++)
                {
                    objects.Add(new HitCircle
                    {
                        StartTime = time,
                        Position = new Vector2(RNG.Next(0, 512), RNG.Next(0, 384)),
                        Scale = RNG.NextSingle(0.5f, 1.0f),
                    });

                    time += RNG.Next(50, 500);
                }

                WorkingBeatmap beatmap = new TestWorkingBeatmap(new Beatmap
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
                });

                Children = new Drawable[]
                {
                    hitRenderer = ruleset.CreateHitRendererWith(beatmap),
                    hudOverlay = new HudOverlay(teamRed)
                };

                scoreProcessor = hitRenderer.CreateScoreProcessor();

                hitRenderer.Origin = Anchor.BottomLeft;
                hitRenderer.Anchor = Anchor.BottomLeft;
                hitRenderer.RelativeSizeAxes = Axes.X;
                hitRenderer.Height = 97;
                hitRenderer.Margin = new MarginPadding { Bottom = 5 };
                hitRenderer.AspectAdjust = false;

                hudOverlay.BindProcessor(scoreProcessor);
            }
        }

        internal class HudOverlay : StandardHudOverlay
        {
            private readonly bool teamRed;

            public HudOverlay(bool teamRed)
            {
                this.teamRed = teamRed;
            }

            protected override HealthDisplay CreateHealthDisplay() => new StandardHealthDisplay
            {
                Origin = Anchor.BottomLeft,
                Anchor = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Size = new Vector2(1, 5)
            };

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                var hd = HealthDisplay as StandardHealthDisplay;

                if (teamRed)
                {
                    hd.AccentColour = colours.PinkLighter;
                    hd.GlowColour = colours.PinkDarker;
                }
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
                public const float STAR_SIZE = 40f;
                private const float glow_sigma = 10f;

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
