﻿using osu.Game.Modes.Objects.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Modes.Taiko.Objects
{
    public class TaikoDrumRollJudgementInfo : TaikoJudgementInfo
    {
        protected override int ScoreToInt(TaikoScoreResult result)
        {
            switch (result)
            {
                default:
                case TaikoScoreResult.Miss:
                    return 0;
                case TaikoScoreResult.Great:
                    return 200;
            }
        }
    }
}
