// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Tournament.Play.Components;
using System.Linq;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiReplayPlayer : OsuScreen
    {
        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;

                if (!IsLoaded)
                    return;

                headerDescription.Text = header;
            }
        }

        private StarCounter blueStarCounter;
        private StarCounter redStarCounter;
        private Sprite backgroundSprite;

        private OsuSpriteText headerDescription;
        private OsuSpriteText headerBeatmap;

        public MultiTeamContainer Teams;

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
                new FillFlowContainer
                {
                    Name = "Header info",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Y = 50,
                    Direction = FillDirection.Vertical,
                    Children = new[]
                    {
                        headerDescription = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = Header,
                            TextSize = 24,
                            Font = "Exo2.0-LightItalic"
                        },
                        headerBeatmap = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            TextSize = 14,
                            Font = "Exo2.0-RegularItalic"
                        }
                    }
                },
                blueStarCounter = new StarCounter
                {
                    Origin = Anchor.CentreLeft,
                    Position = new Vector2(30, 50),
                    MaxCount = 3,
                    AccentColour = colours.BlueDarker
                },
                redStarCounter = new StarCounter
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.CentreRight,
                    Position = new Vector2(-30, 50),
                    MaxCount = 3,
                    Direction = StarCounterDirection.RightToLeft,
                    AccentColour = colours.PinkDarker
                },
                Teams = CreateTeamContainer(beatmap)
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
                OnCompleted(Teams.RedTeam.Score > Teams.BlueTeam.Score);
            }
        }

        protected virtual void OnCompleted(bool winningTeamWasRed) { }

        protected abstract MultiTeamContainer CreateTeamContainer(WorkingBeatmap beatmap);
    }
}
