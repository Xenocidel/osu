// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Objects.Drawables
{
    /// <summary>
    /// A <see cref="DrawableHitObject"/> that wraps a <see cref="DrawableHitObject<TObject, TJudgement>"/>
    /// and provides the capability to scroll through the screen.
    /// </summary>
    public abstract class ScrollingWrapper<TObject, TJudgement> : DrawableHitObject<TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
    {
        public readonly BindableDouble ScrollTime = new BindableDouble(6000);

        public new readonly DrawableHitObject<TObject, TJudgement> HitObject;

        private BeatmapDifficulty difficulty;
        private ControlPointInfo controlPoints;

        private double baseVelocity;

        public ScrollingWrapper(DrawableHitObject<TObject, TJudgement> hitObject)
            : base(hitObject.HitObject)
        {
            HitObject = hitObject;

            base.AutoSizeAxes = Axes.Both;
            Anchor = hitObject.Anchor;
            Origin = hitObject.Origin;

            Add(hitObject);
            AddNested(hitObject);
        }

        [BackgroundDependencyLoader]
        private void load(OsuGameBase game)
        {
            difficulty = game.Beatmap.Value.BeatmapInfo.Difficulty;
            controlPoints = game.Beatmap.Value.Beatmap.ControlPointInfo;

            var timingPoint = controlPoints.TimingPointAt(HitObject.HitObject.StartTime);
            var difficultyPoint = controlPoints.DifficultyPointAt(HitObject.HitObject.StartTime);

            // Todo: I changed these calculations a bit, check this!!
            baseVelocity = 1000 / timingPoint.BeatLength * difficultyPoint.SpeedMultiplier * difficulty.SliderMultiplier;
        }

        protected override void Update()
        {
            base.Update();

            UpdateScroll((float)((Time.Current - HitObject.HitObject.StartTime) / ScrollTime.Value * baseVelocity));
        }

        protected abstract void UpdateScroll(float completion);

        protected sealed override TJudgement CreateJudgement() => null;

        protected sealed override void UpdateState(ArmedState state) { }
    }
}
