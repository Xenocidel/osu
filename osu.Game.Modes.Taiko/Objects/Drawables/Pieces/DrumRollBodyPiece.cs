﻿using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Modes.Taiko.Objects.Drawables.Pieces
{
    public class DrumRollFinisherBodyPiece : DrumRollBodyPiece
    {
        public override float CornerRadius => base.CornerRadius * 1.5f;

        public DrumRollFinisherBodyPiece(float baseLength)
            : base(baseLength)
        {
        }
    }

    public class DrumRollBodyPiece : Container
    {
        private static Color4 yellow_colour = new Color4(238, 170, 0, 255);

        public override float CornerRadius => 32;

        public DrumRollBodyPiece(float baseLength)
        {
            Size = new Vector2(baseLength + CornerRadius * 2, CornerRadius * 2);

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            Masking = true;
            BorderColour = Color4.White;
            BorderThickness = 4;

            EdgeEffect = new EdgeEffect()
            {
                Colour = new Color4(yellow_colour.R, yellow_colour.G, yellow_colour.B, 0.75f),
                Radius = 50,
                Type = EdgeEffectType.Glow,
            };

            Children = new Drawable[]
            {
                new Box()
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = yellow_colour
                },
                new DrumRollTrianglesPiece()
                {
                    RelativeSizeAxes = Axes.Both,

                    Colour = Color4.Black,
                    Alpha = 0.05f,
                }
            };
        }
    }
}