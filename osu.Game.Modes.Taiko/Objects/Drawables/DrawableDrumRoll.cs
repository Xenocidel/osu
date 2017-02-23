﻿using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Modes.Taiko.Objects.Drawables.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Modes.Taiko.Objects.Drawables
{
    public class DrawableDrumRollFinisher : DrawableDrumRoll
    {
        public DrawableDrumRollFinisher(DrumRoll drumRoll)
            : base(drumRoll)
        {
            Size *= new Vector2(1, 1.5f);
        }

        protected override DrumRollBodyPiece CreateBody(float length) => new DrumRollFinisherBodyPiece(length);
    }

    public class DrawableDrumRoll : DrawableTaikoHitObject
    {
        private DrumRoll drumRoll;

        private DrumRollBodyPiece body;
        private FlowContainer<DrawableDrumRollTick> ticks;

        public DrawableDrumRoll(DrumRoll drumRoll)
            : base(drumRoll)
        {
            this.drumRoll = drumRoll;

            Size = new Vector2((float)drumRoll.Length * drumRoll.RepeatCount, 128);

            Children = new Drawable[]
            {
                body = CreateBody(Size.X),
                ticks = new FlowContainer<DrawableDrumRollTick>
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FlowDirections.Horizontal
                }
           };

            foreach (var tick in drumRoll.Ticks)
            {
                var newTick = new DrawableDrumRollTick(tick);

                ticks.Add(newTick);
                ticks.Spacing = new Vector2((float)drumRoll.TickDistance - newTick.Size.X * tick.Scale, 0);
            }

        }

        protected virtual DrumRollBodyPiece CreateBody(float length) => new DrumRollBodyPiece(length);
    }
}