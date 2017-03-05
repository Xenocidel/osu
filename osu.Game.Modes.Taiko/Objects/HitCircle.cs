﻿using osu.Game.Beatmaps.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Modes.Taiko.Objects
{
    public class HitCircle : TaikoHitObject
    {
        public override TaikoHitType Type
        {
            get
            {
                SampleType st = Sample?.Type ?? SampleType.None;

                return
                    // Don/Katsu
                    ((st & ~(SampleType.Finish | SampleType.Normal)) == 0 ? TaikoHitType.Don : TaikoHitType.Katsu)
                    // Finisher
                    | ((st & SampleType.Finish) > 0 ? TaikoHitType.Finisher : TaikoHitType.None);
            }
        }
    }
}
