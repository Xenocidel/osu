// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko.Beatmaps;
using osu.Game.Rulesets.Taiko.Judgements;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.Taiko.Objects.Drawables;
using osu.Game.Rulesets.Taiko.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Taiko.Replays;
using OpenTK;
using osu.Game.Rulesets.Beatmaps;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Lists;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Timing;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Taiko.Timing.Drawables;
using System;
using osu.Framework.MathUtils;
using osu.Game.Rulesets.Taiko.Mods;
using osu.Game.Rulesets.Taiko.Timing;

namespace osu.Game.Rulesets.Taiko.UI
{
    public class TaikoHitRenderer : SpeedAdjustedHitRenderer<TaikoHitObject, TaikoJudgement>
    {
        public readonly IEnumerable<DrawableBarLine> BarLines;

        private readonly List<SpeedAdjustmentContainer> hitObjectSpeedAdjustments = new List<SpeedAdjustmentContainer>();
        private readonly List<SpeedAdjustmentContainer> barLineSpeedAdjustments = new List<SpeedAdjustmentContainer>();

        public TaikoHitRenderer(WorkingBeatmap beatmap, bool isForCurrentRuleset)
            : base(beatmap, isForCurrentRuleset)
        {
            // Generate the bar lines
            double lastObjectTime = (Objects.LastOrDefault() as IHasEndTime)?.EndTime ?? Objects.LastOrDefault()?.StartTime ?? double.MaxValue;

            SortedList<TimingControlPoint> timingPoints = Beatmap.ControlPointInfo.TimingPoints;
            var barLines = new List<DrawableBarLine>();

            for (int i = 0; i < timingPoints.Count; i++)
            {
                TimingControlPoint point = timingPoints[i];

                // Stop on the beat before the next timing point, or if there is no next timing point stop slightly past the last object
                double endTime = i < timingPoints.Count - 1 ? timingPoints[i + 1].Time - point.BeatLength : lastObjectTime + point.BeatLength * (int)point.TimeSignature;

                int index = 0;
                for (double t = timingPoints[i].Time; Precision.DefinitelyBigger(endTime, t); t += point.BeatLength, index++)
                    barLines.Add(new DrawableBarLine(new BarLine { StartTime = t }));
            }

            BarLines = barLines;

            // Generate speed adjustments from mods first
            bool useDefaultSpeedAdjustments = true;

            if (Mods != null)
            {
                foreach (var speedAdjustmentMod in Mods.OfType<IGenerateSpeedAdjustments>())
                {
                    useDefaultSpeedAdjustments = false;
                    speedAdjustmentMod.ApplyToHitRenderer(this, ref hitObjectSpeedAdjustments, ref barLineSpeedAdjustments);
                }
            }

            // Generate the default speed adjustments
            if (useDefaultSpeedAdjustments)
                generateDefaultSpeedAdjustments();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            loadBarLines();
        }

        protected override void ApplySpeedAdjustments()
        {
            var taikoPlayfield = (TaikoPlayfield)Playfield;

            foreach (var speedAdjustment in hitObjectSpeedAdjustments)
                taikoPlayfield.AddHitObjectSpeedAdjustment(speedAdjustment);

            foreach (var speedAdjustment in barLineSpeedAdjustments)
                taikoPlayfield.AddBarLineSpeedAdjustment(speedAdjustment);
        }

        private void generateDefaultSpeedAdjustments()
        {
            DefaultControlPoints.ForEach(c =>
            {
                hitObjectSpeedAdjustments.Add(new TaikoSpeedAdjustmentContainer(c, ScrollingAlgorithm.Basic));
                barLineSpeedAdjustments.Add(new TaikoSpeedAdjustmentContainer(c, ScrollingAlgorithm.Basic));
            });
        }

        private void loadBarLines()
        {
            var taikoPlayfield = Playfield as TaikoPlayfield;

            if (taikoPlayfield == null)
                return;

            TaikoHitObject lastObject = Beatmap.HitObjects[Beatmap.HitObjects.Count - 1];
            double lastHitTime = 1 + (lastObject as IHasEndTime)?.EndTime ?? lastObject.StartTime;

            var timingPoints = Beatmap.ControlPointInfo.TimingPoints.ToList();

            if (timingPoints.Count == 0)
                return;

            int currentIndex = 0;
            int currentBeat = 0;
            double time = timingPoints[currentIndex].Time;
            while (time <= lastHitTime)
            {
                int nextIndex = currentIndex + 1;
                if (nextIndex < timingPoints.Count && time > timingPoints[nextIndex].Time)
                {
                    currentIndex = nextIndex;
                    time = timingPoints[currentIndex].Time;
                    currentBeat = 0;
                }

                var currentPoint = timingPoints[currentIndex];

                var barLine = new BarLine
                {
                    StartTime = time,
                };

                barLine.ApplyDefaults(Beatmap.ControlPointInfo, Beatmap.BeatmapInfo.Difficulty);

                bool isMajor = currentBeat % (int)currentPoint.TimeSignature == 0;
                taikoPlayfield.Add(isMajor ? new DrawableBarLineMajor(barLine) : new DrawableBarLine(barLine));

                double bl = currentPoint.BeatLength;
                if (bl < 800)
                    bl *= (int)currentPoint.TimeSignature;

                time += bl;
                currentBeat++;
            }
        }

        protected override Vector2 GetPlayfieldAspectAdjust()
        {
            const float default_relative_height = TaikoPlayfield.DEFAULT_PLAYFIELD_HEIGHT / 768;
            const float default_aspect = 16f / 9f;

            float aspectAdjust = MathHelper.Clamp(DrawWidth / DrawHeight, 0.4f, 4) / default_aspect;

            return new Vector2(1, default_relative_height * aspectAdjust);
        }

        public override ScoreProcessor CreateScoreProcessor() => new TaikoScoreProcessor(this);

        protected override BeatmapConverter<TaikoHitObject> CreateBeatmapConverter() => new TaikoBeatmapConverter();

        protected sealed override Playfield<TaikoHitObject, TaikoJudgement> CreatePlayfield()
        {
            var playfield = new TaikoPlayfield
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            };



            return playfield;
        }

        protected override DrawableHitObject<TaikoHitObject, TaikoJudgement> GetVisualRepresentation(TaikoHitObject h)
        {
            var centreHit = h as CentreHit;
            if (centreHit != null)
            {
                if (h.IsStrong)
                    return new DrawableCentreHitStrong(centreHit);
                return new DrawableCentreHit(centreHit);
            }

            var rimHit = h as RimHit;
            if (rimHit != null)
            {
                if (h.IsStrong)
                    return new DrawableRimHitStrong(rimHit);
                return new DrawableRimHit(rimHit);
            }

            var drumRoll = h as DrumRoll;
            if (drumRoll != null)
            {
                return new DrawableDrumRoll(drumRoll);
            }

            var swell = h as Swell;
            if (swell != null)
                return new DrawableSwell(swell);

            return null;
        }

        protected override FramedReplayInputHandler CreateReplayInputHandler(Replay replay) => new TaikoFramedReplayInputHandler(replay);
    }
}
