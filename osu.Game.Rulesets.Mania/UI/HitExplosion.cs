// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Rulesets.Mania.UI
{
    public class HitExplosion : Container
    {
        private const float diameter = 90;

        private readonly Box fill;

        public HitExplosion()
        {
            Size = new Vector2(diameter);

            Children = new[]
            {
                new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    BorderThickness = 1,
                    BorderColour = Color4.White,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Glow,
                        Radius = 8,
                        Colour = Color4.White
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ScaleTo(0);
            ScaleTo(2f, 200, EasingTypes.OutQuint);

            using (BeginDelayedSequence(100))
                FadeTo(0f, 100, EasingTypes.InQuint);
        }
    }
}