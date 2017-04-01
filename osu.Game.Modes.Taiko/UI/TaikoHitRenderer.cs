﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.MathUtils;
using osu.Game.Beatmaps;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.Timing;
using osu.Game.Modes.Objects.Drawables;
using osu.Game.Modes.Objects.Types;
using osu.Game.Modes.Replays;
using osu.Game.Modes.Scoring;
using osu.Game.Modes.Taiko.Beatmaps;
using osu.Game.Modes.Taiko.Judgements;
using osu.Game.Modes.Taiko.Objects;
using osu.Game.Modes.Taiko.Objects.Drawable;
using osu.Game.Modes.Taiko.Replays;
using osu.Game.Modes.Taiko.Objects.Drawable;
using osu.Game.Modes.Taiko.Scoring;
using osu.Game.Modes.UI;
using osu.Game.Screens.Play;

namespace osu.Game.Modes.Taiko.UI
{
    public class TaikoHitRenderer : HitRenderer<TaikoHitObject, TaikoJudgement>
    {
        public TaikoHitRenderer(WorkingBeatmap beatmap)
            : base(beatmap)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            loadBarLines();
        }

        private void loadBarLines()
        {
            var taikoPlayfield = Playfield as TaikoPlayfield;

            if (taikoPlayfield == null)
                return;

            TaikoHitObject lastObject = Beatmap.HitObjects[Beatmap.HitObjects.Count - 1];
            // ReSharper disable once SuspiciousTypeConversion.Global (will be fixed with hitobjects)
            double lastHitTime = 1 + (lastObject as IHasEndTime)?.EndTime ?? lastObject.StartTime;

            var timingPoints = Beatmap.TimingInfo.ControlPoints.FindAll(cp => cp.TimingChange);

            if (timingPoints == null || timingPoints.Count == 0)
                return;

            int currentIndex = 0;

            while (currentIndex < timingPoints.Count && Precision.AlmostEquals(timingPoints[currentIndex].BeatLength, 0))
                currentIndex++;

            double time = timingPoints[currentIndex].Time;
            double measureLength = timingPoints[currentIndex].BeatLength * (int)timingPoints[currentIndex].TimeSignature;

            // Find the bar line time closest to 0
            time -= measureLength * (int)(time / measureLength);

            // Always start barlines from a positive time
            while (time < 0)
                time += measureLength;

            int currentBeat = 0;
            while (time <= lastHitTime)
            {
                ControlPoint current = timingPoints[currentIndex];

                if (time > current.Time || current.OmitFirstBarLine)
                {
                    bool isMajor = currentBeat % (int)current.TimeSignature == 0;

                    var barLine = new BarLine
                    {
                        StartTime = time,
                    };

                    barLine.ApplyDefaults(Beatmap.TimingInfo, Beatmap.BeatmapInfo.Difficulty);

                    taikoPlayfield.AddBarLine(isMajor ? new DrawableMajorBarLine(barLine) : new DrawableBarLine(barLine));

                    currentBeat++;
                }

                double bl = current.BeatLength;

                if (bl < 800)
                    bl *= (int)current.TimeSignature;

                time += bl;

                if (currentIndex + 1 >= timingPoints.Count || time < timingPoints[currentIndex + 1].Time)
                    continue;

                currentBeat = 0;
                currentIndex++;
                time = timingPoints[currentIndex].Time;
            }
        }

        public override ScoreProcessor CreateScoreProcessor() => new TaikoScoreProcessor(this);

        protected override IBeatmapConverter<TaikoHitObject> CreateBeatmapConverter() => new TaikoBeatmapConverter();

        protected override IBeatmapProcessor<TaikoHitObject> CreateBeatmapProcessor() => new TaikoBeatmapProcessor();

        protected override Playfield<TaikoHitObject, TaikoJudgement> CreatePlayfield() => new TaikoPlayfield
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };

        protected override DrawableHitObject<TaikoHitObject, TaikoJudgement> GetVisualRepresentation(TaikoHitObject h)
        {
            var hit = h as Hit;
            if (hit != null)
            {
                if (hit is CentreHit)
                {
                    if (h.IsStrong)
                        return new DrawableStrongCentreHit(hit);
                    return new DrawableCentreHit(hit);
                }

                if (h is RimHit)
                {
                    if (h.IsStrong)
                        return new DrawableStrongRimHit(hit);
                    return new DrawableRimHit(hit);
                }
            }

            var drumRoll = h as DrumRoll;
            if (drumRoll != null)
            {
                if (h.IsStrong)
                    return new DrawableStrongDrumRoll(drumRoll);
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
