﻿using osu.Game.Modes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Samples;

namespace osu.Game.Modes.Taiko.Objects
{
    public class TaikoHitObject : HitObject
    {
        public float Scale { get; set; } = 1;
        public float ScrollSpeed { get; set; } 

        public TaikoHitType Type => ((Sample?.Type ?? SampleType.None) & (~SampleType.Finish & ~SampleType.Normal)) == 0 ? TaikoHitType.Don : TaikoHitType.Katsu;
        public bool IsFinisher => ((Sample?.Type ?? SampleType.None) & SampleType.Finish) > 0;

        public override void SetDefaultsFromBeatmap(Beatmap beatmap)
        {
            base.SetDefaultsFromBeatmap(beatmap);

            Scale = 1f - 0.7f * -3f / 5 / 2;
            ScrollSpeed = 640 / (float)beatmap.SliderVelocityAt(StartTime);
        }
    }

    public enum TaikoHitType
    {
        None,
        Don,
        Katsu,
    }
}
