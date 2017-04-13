// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Modes.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiTeam : Container, IHasAccentColour
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

        public readonly FlowContainer<MultiPlayer> Players;
        private readonly ScoreCounter teamScoreCounter;
        private readonly ScoreCounter scoreDiffCounter;

        private readonly Sprite flag;

        private readonly OsuSpriteText name;

        private MultiTeam otherColumn;

        private int playersCompleted;
        private int playersFailed;

        private readonly WorkingBeatmap beatmap;
        private readonly bool teamRed;

        public MultiTeam(bool teamRed, WorkingBeatmap beatmap)
        {
            this.beatmap = beatmap;
            this.teamRed = teamRed;

            RelativeSizeAxes = Axes.Both;
            Width = 0.5f;
            Padding = new MarginPadding { Top = player_container_start };
            Masking = true;

            Children = new Drawable[]
            {
                Players = new FillFlowContainer<MultiPlayer>()
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
            MultiPlayer player = CreatePlayer(teamRed, score, beatmap);
            player.OnCompletion = playerCompleted;
            player.OnFail = playerFailed;

            Players.Add(player);
            Players.Children.ForEach(p => p.Height = 1f / Players.Children.Count() - player_spacing / player_container_height);
        }

        public void BindTeamColumn(MultiTeam other)
        {
            if (otherColumn != null)
                return;

            otherColumn = other ?? throw new ArgumentNullException(nameof(other));
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

        public abstract MultiPlayer CreatePlayer(bool teamRed, Score score, WorkingBeatmap beatmap);
    }
}
