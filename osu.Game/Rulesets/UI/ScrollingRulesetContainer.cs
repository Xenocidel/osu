// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Lists;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.IO.Serialization;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.UI
{
    /// <summary>
    /// A type of <see cref="RulesetContainer{TPlayfield,TObject,TJudgement}"/> which contains scrolling hit objects.
    /// </summary>
    public abstract class ScrollingRulesetContainer<TPlayfield, TObject, TJudgement> : RulesetContainer<TPlayfield, TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
        where TPlayfield : ScrollingPlayfield<TObject, TJudgement>
    {
        protected ScrollingRulesetContainer(Ruleset ruleset, WorkingBeatmap beatmap, bool isForCurrentRuleset)
            : base(ruleset, beatmap, isForCurrentRuleset)
        {
        }
    }
}
