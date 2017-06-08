﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.Timing.Drawables;

namespace osu.Game.Rulesets.Mania.Timing.Drawables
{
    internal class GravityScrollingHitObjectCollection : HitObjectCollection
    {
        private readonly TimingSection timingSection;
        private readonly Func<double> timeSpan;

        public GravityScrollingHitObjectCollection(TimingSection timingSection, Func<double> timeSpan)
            : base(Axes.Y)
        {
            this.timingSection = timingSection;
            this.timeSpan = timeSpan;
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            // The gravity-adjusted start position
            float startPos = (float)computeGravityTime(timingSection.Time);
            // The gravity-adjusted end position
            float endPos = (float)computeGravityTime(timingSection.Time + RelativeChildSize.Y);

            Y = startPos;
            Height = endPos - startPos;
        }

        /// <summary>
        /// Applies gravity to a time value based on the current time.
        /// </summary>
        /// <param name="time">The time value gravity should be applied to.</param>
        /// <returns>The time after gravity is applied to <paramref name="time"/>.</returns>
        private double computeGravityTime(double time)
        {
            double relativeTime = relativeTimeAt(time);

            // The sign of the relative time, this is used to apply backwards acceleration leading into startTime
            double sign = relativeTime < 0 ? -1 : 1;

            return timeSpan() - acceleration * relativeTime * relativeTime * sign;
        }

        /// <summary>
        /// The acceleration due to "gravity" of the content of this container.
        /// </summary>
        private double acceleration => 1 / timeSpan();

        /// <summary>
        /// Computes the current time relative to <paramref name="time"/>, accounting for <see cref="timeSpan"/>.
        /// </summary>
        /// <param name="time">The non-offset time.</param>
        /// <returns>The current time relative to <paramref name="time"/> - <see cref="timeSpan"/>. </returns>
        private double relativeTimeAt(double time) => Time.Current - time + timeSpan();
    }
}