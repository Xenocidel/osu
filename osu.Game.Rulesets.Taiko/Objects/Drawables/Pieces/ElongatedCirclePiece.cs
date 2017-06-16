// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Taiko.Objects.Drawables.Pieces
{
    public class ElongatedCirclePiece : CirclePiece
    {
        public ElongatedCirclePiece(bool isStrong = false)
            : base(isStrong)
        {
        }

        protected override void Update()
        {
            base.Update();

            var padding = Content.DrawHeight * Content.Width / 2;

            Content.Padding = new MarginPadding
            {
                Left = padding,
                Right = padding,
            };

            // The circle piece ends are rounded with radius = DrawHeight / 2, so the width needs to be increased by the diameter
            // otherwise the hit object start/end times of the hit object will lie on the border of the circle piece
            Width = Parent.DrawSize.X + Content.DrawHeight;
        }
    }
}
