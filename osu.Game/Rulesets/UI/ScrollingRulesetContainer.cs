// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

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
using osu.Game.Rulesets.Timing;

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

        protected override void Add(DrawableHitObject<TObject, TJudgement> hitObject) => base.Add(CreateScrollingWrapper(hitObject));

        /// <summary>
        /// Creates a container which wraps a <see cref="DrawableHitObject<TObject, Tjudgement>"/> to handle scrolling the hit object.
        /// </summary>
        /// <param name="hitObject">The <see cref="DrawableHitObject<TObject, TJudgement>"/> to create the wrapper for.</param>
        /// <returns>The <see cref="DrawableScrollingHitObject<TObject, TJudgement>"/> wrapper.</returns>
        protected abstract DrawableScrollingHitObject<TObject, TJudgement> CreateScrollingWrapper(DrawableHitObject<TObject, TJudgement> hitObject);
    }
}
