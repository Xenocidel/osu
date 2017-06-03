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
using osu.Game.Rulesets.Timing.Drawables;
using osu.Framework.Lists;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Timing;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Taiko.Timing.Drawables;

namespace osu.Game.Rulesets.Taiko.UI
{
    public class TaikoHitRenderer : HitRenderer<TaikoHitObject, TaikoJudgement>
    {
        public List<TimingChange> TimingChanges;

        public TaikoHitRenderer(WorkingBeatmap beatmap, bool isForCurrentRuleset)
            : base(beatmap, isForCurrentRuleset)
        {
            generateDefaultTimingChanges();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            loadBarLines();
        }

        private void generateDefaultTimingChanges()
        {
            if (TimingChanges != null)
                return;

            TimingChanges = new List<TimingChange>();

            double lastSpeedMultiplier = 1;
            double lastBeatLength = 500;

            // Merge timing + difficulty points
            var allPoints = new SortedList<ControlPoint>(Comparer<ControlPoint>.Default);
            allPoints.AddRange(Beatmap.ControlPointInfo.TimingPoints);
            allPoints.AddRange(Beatmap.ControlPointInfo.DifficultyPoints);

            // Generate the timing points, making non-timing changes use the previous timing change
            var timingChanges = allPoints.Select(c =>
            {
                var timingPoint = c as TimingControlPoint;
                var difficultyPoint = c as DifficultyControlPoint;

                if (timingPoint != null)
                    lastBeatLength = timingPoint.BeatLength;

                if (difficultyPoint != null)
                    lastSpeedMultiplier = difficultyPoint.SpeedMultiplier;

                return new TimingChange
                {
                    Time = c.Time,
                    BeatLength = lastBeatLength,
                    SpeedMultiplier = lastSpeedMultiplier
                };
            });

            double lastObjectTime = (Objects.LastOrDefault() as IHasEndTime)?.EndTime ?? Objects.LastOrDefault()?.StartTime ?? double.MaxValue;

            // Perform some post processing of the timing changes
            timingChanges = timingChanges
                // Collapse sections after the last hit object
                .Where(s => s.Time <= lastObjectTime)
                // Collapse sections with the same start time
                .GroupBy(s => s.Time).Select(g => g.Last()).OrderBy(s => s.Time)
                // Collapse sections with the same beat length
                .GroupBy(s => s.BeatLength * s.SpeedMultiplier).Select(g => g.First());

            TimingChanges = timingChanges.ToList();
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

        protected override Playfield<TaikoHitObject, TaikoJudgement> CreatePlayfield()
        {
            var playfield = new TaikoPlayfield
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            };

            foreach (var change in TimingChanges)
                playfield.Add(change);

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
