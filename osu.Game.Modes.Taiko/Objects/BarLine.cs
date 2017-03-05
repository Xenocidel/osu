﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Modes.Taiko.Objects
{
    public class BarLine
    {
        public double StartTime;
        public double PreEmpt;

        public void SetDefaultsFromBeatmap(Beatmap beatmap)
        {
            // Don't ask... Old osu! had a random multiplier here, that we now have to multiply everywhere
            float fudgeFactor = 1.4f;

            PreEmpt = 600 / (beatmap.SliderVelocityAt(StartTime) * fudgeFactor) * 1000;
        }
    }
}