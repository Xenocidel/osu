// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Modes.UI;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiHudOverlay : Screens.Tournament.Play.MultiHudOverlay
    {
        public MultiHudOverlay(bool teamRed)
            : base(teamRed)
        {
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

            if (TeamRed)
            {
                ComboCounter.AccentColour = colours.PinkLighter;
                ScoreCounter.AccentColour = colours.PinkLighter;
                AccuracyCounter.AccentColour = colours.PinkLighter;
                shd.AccentColour = colours.PinkLighter;
                shd.GlowColour = colours.PinkDarker;
            }
        }
    }
}
