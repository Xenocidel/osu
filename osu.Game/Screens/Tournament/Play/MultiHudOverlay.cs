// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Modes.UI;

namespace osu.Game.Screens.Tournament.Play
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

        protected readonly bool TeamRed;

        public MultiHudOverlay(bool teamRed)
        {
            TeamRed = teamRed;

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
                        Text = "..."
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            if (TeamRed)
                nameText.Colour = colours.PinkLighter;
        }
    }
}
