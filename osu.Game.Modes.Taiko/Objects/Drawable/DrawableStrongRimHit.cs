﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK.Input;
using osu.Framework.Allocation;
using osu.Game.Graphics;
using osu.Game.Modes.Taiko.Objects.Drawable.Pieces;

namespace osu.Game.Modes.Taiko.Objects.Drawable
{
    public class DrawableStrongRimHit : DrawableStrongHit
    {
        protected override Key[] HitKeys { get; } = { Key.D, Key.K };

        public DrawableStrongRimHit(Hit hit)
            : base(hit)
        {
            Circle.Add(new RimHitSymbolPiece());
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Circle.AccentColour = colours.BlueDarker;
        }

        protected override CirclePiece CreateCirclePiece() => new StrongCirclePiece();
    }
}
