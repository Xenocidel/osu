// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
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

            Width = Parent.DrawSize.X + Content.DrawHeight;
        }
    }
}
