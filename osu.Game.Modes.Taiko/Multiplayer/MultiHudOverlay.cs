// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Modes.UI;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiHudOverlay : StandardHudOverlay
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

        public MultiHudOverlay(bool teamRed)
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
}
