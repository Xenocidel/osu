// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps.Timing;

namespace osu.Game.Beatmaps
{
    public class TimingInfo
    {
        public List<ControlPoint> ControlPoints = new List<ControlPoint>();

        public double BPMMaximum => 60000 / (ControlPoints?.Where(c => c.BeatLength != 0).OrderBy(c => c.BeatLength).FirstOrDefault() ?? ControlPoint.Default).BeatLength;
        public double BPMMinimum => 60000 / (ControlPoints?.Where(c => c.BeatLength != 0).OrderByDescending(c => c.BeatLength).FirstOrDefault() ?? ControlPoint.Default).BeatLength;
        public double BPMMode => BPMAt(ControlPoints.Where(c => c.BeatLength != 0).GroupBy(c => c.BeatLength).OrderByDescending(grp => grp.Count()).First().First().Time);

        public double BPMAt(double time)
        {
            return 60000 / BeatLengthAt(time);
        }

        /// <summary>
        /// Finds the BPM multiplier at a time.
        /// </summary>
        /// <param name="time">The time to find the BPM multiplier at.</param>
        /// <returns>The BPM multiplier.</returns>
        public double BPMMultiplierAt(double time)
        {
            ControlPoint overridePoint;
            TimingPointAt(time, out overridePoint);

            return overridePoint?.VelocityAdjustment ?? 1;
        }

        /// <summary>
        /// Finds the beat length (in milliseconds) at a time.
        /// </summary>
        /// <param name="time">The time to find the beat length at.</param>
        /// <returns>The beat length.</returns>
        public double BeatLengthAt(double time)
        {
            ControlPoint overridePoint;
            ControlPoint timingPoint = TimingPointAt(time, out overridePoint);

            return timingPoint.BeatLength;
        }

        /// <summary>
        /// Finds the beat velocity at a time.
        /// </summary>
        /// <param name="time">The time to find the velocity at.</param>
        /// <returns>The velocity.</returns>
        public double BeatVelocityAt(double time)
        {
            ControlPoint overridePoint;
            TimingPointAt(time, out overridePoint);

            return overridePoint?.VelocityAdjustment ?? 1;
        }

        /// <summary>
        /// Finds the beat length (in distance) at a time.
        /// </summary>
        /// <param name="time">The time to find the beat length at.</param>
        /// <returns>The beat length.</returns>
        public double BeatDistanceAt(double time)
        {
            ControlPoint overridePoint;
            ControlPoint timingPoint = TimingPointAt(time, out overridePoint);

            return timingPoint.BeatLength * (overridePoint?.VelocityAdjustment ?? 1);
        }

        /// <summary>
        /// Finds the timing point at a time.
        /// </summary>
        /// <param name="time">The time to find the timing point at.</param>
        /// <param name="overridePoint">The timing point containing the velocity change of the returned timing point.</param>
        /// <returns>The timing point.</returns>
        public ControlPoint TimingPointAt(double time, out ControlPoint overridePoint)
        {
            overridePoint = null;

            ControlPoint timingPoint = null;
            foreach (var controlPoint in ControlPoints)
            {
                // Some beatmaps have the first timingPoint (accidentally) start after the first HitObject(s).
                // This null check makes it so that the first ControlPoint that makes a timing change is used as
                // the timingPoint for those HitObject(s).
                if (controlPoint.Time <= time || timingPoint == null)
                {
                    if (controlPoint.TimingChange)
                    {
                        timingPoint = controlPoint;
                        overridePoint = null;
                    }
                    else
                        overridePoint = controlPoint;
                }
                else break;
            }

            return timingPoint ?? ControlPoint.Default;
        }

    }
}