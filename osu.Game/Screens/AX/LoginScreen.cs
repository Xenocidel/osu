﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Screens.AX.Steps;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Menu;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Input;
using osu.Game.Graphics.Sprites;
using OpenTK.Input;

namespace osu.Game.Screens.AX
{
    public class LoginScreen : OsuScreen
    {
        protected override BackgroundScreen CreateBackground() => new AxBackgroundScreen();

        internal override bool ShowOverlays => false;

        public LoginScreen()
        {
            Add(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.75f
                },
                new OsuLogo
                {
                    Scale = new Vector2(0.60f),
                    Interactive = false,
                    Y = -150
                },
                new OsuSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = "Sign up for our mailing list!",
                    TextSize = 48,
                    Y = 30
                },
                new StepContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(1.0f),
                    Y = 120,
                }
            });
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (args.Key == Key.Escape)
                return true;
            return base.OnKeyDown(state, args);
        }

        private class AxBackgroundScreen : BackgroundScreenCustom
        {
            public AxBackgroundScreen()
                : base("AX/background")
            {
                AddInternal(new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1
                });
            }
        }
    }
}
