﻿using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Modes.Objects.Drawables;
using osu.Game.Modes.Taiko.Objects.Drawables.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Container<DrawableDrumRollTick> ticks;

        private List<DrawableDrumRollTick> allTicks = new List<DrawableDrumRollTick>();

        public DrawableDrumRoll(DrumRoll drumRoll)
            : base(drumRoll)
        {
            this.drumRoll = drumRoll;

            Origin = Anchor.CentreLeft;

            Size = new Vector2((float)drumRoll.Length * drumRoll.RepeatCount, 64);

            Children = new Drawable[]
            {
                body = CreateBody(Size.X),
                ticks = new Container<DrawableDrumRollTick>()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    RelativeSizeAxes = Axes.Both
                }
           };

            int tickIndex = 0;
            foreach (var tick in drumRoll.Ticks)
            {
                var newTick = new DrawableDrumRollTick(drumRoll, tick)
                {
                    Depth = tickIndex,
                    Position = new Vector2(tickIndex * (float)drumRoll.TickDistance, 0)
                };

                ticks.Add(newTick);
                allTicks.Add(newTick);

                tickIndex++;
            }
        }

        protected virtual DrumRollBodyPiece CreateBody(float length) => new DrumRollBodyPiece(length);

        protected override void CheckJudgement(bool userTriggered)
        {
            if (userTriggered)
                return;

            if (Judgement.TimeOffset < 0)
                return;

            TaikoJudgementInfo taikoJudgement = Judgement as TaikoJudgementInfo;

            int countHit = allTicks.Count(t => t.Judgement.Result.HasValue);

            if (countHit < drumRoll.RequiredGoodHits)
            {
                Judgement.Result = HitResult.Hit;

                if (countHit < drumRoll.RequiredGreatHits)
                    taikoJudgement.Score = TaikoScoreResult.Great;
                else
                    taikoJudgement.Score = TaikoScoreResult.Good;

            }
            else
                Judgement.Result = HitResult.Miss;
        }
    }
}