// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
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
    public abstract class DrawableScrollingHitObject<TObject, TJudgement> : DrawableHitObject<TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
    {
        public readonly BindableDouble ScrollTime = new BindableDouble(6000);

        private BeatmapDifficulty difficulty;
        private ControlPointInfo controlPoints;

        private double baseVelocity;

        private readonly DrawableHitObject<TObject, TJudgement> baseHitObject;

        protected DrawableScrollingHitObject(DrawableHitObject<TObject, TJudgement> hitObject)
            : base(hitObject.HitObject)
        {
            baseHitObject = hitObject;

            base.AutoSizeAxes = Axes.Both;

            Add(hitObject);
            AddNested(hitObject);
        }

        [BackgroundDependencyLoader]
        private void load(OsuGameBase game)
        {
            difficulty = game.Beatmap.Value.BeatmapInfo.Difficulty;
            controlPoints = game.Beatmap.Value.Beatmap.ControlPointInfo;

            var timingPoint = controlPoints.TimingPointAt(HitObject.StartTime);
            var difficultyPoint = controlPoints.DifficultyPointAt(HitObject.StartTime);

            // Todo: I changed these calculations a bit, check this!!
            baseVelocity = 1000 / timingPoint.BeatLength * difficultyPoint.SpeedMultiplier * difficulty.SliderMultiplier;
        }

        protected override void Update()
        {
            base.Update();

            UpdateScroll((float)((HitObject.StartTime - Time.Current) / ScrollTime.Value * baseVelocity));
        }

        protected virtual void UpdateScroll(float completion) { }

        public new Axes AutoSizeAxes
        {
            get { return Axes.Both; }
            set { throw new InvalidOperationException($"{nameof(DrawableScrollingHitObject<TObject, TJudgement>)} must always be auto-sized."); }
        }

        public new Axes RelativeSizeAxes
        {
            get { return Axes.None; }
            set { throw new InvalidOperationException($"{nameof(DrawableScrollingHitObject<TObject, TJudgement>)} must always be auto-sized."); }
        }

        protected sealed override TJudgement CreateJudgement() => null;

        protected sealed override void UpdateState(ArmedState state) { }
    }
}
