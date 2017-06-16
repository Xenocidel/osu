// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Rulesets.Timing;

namespace osu.Game.Rulesets.Taiko.Timing
{
    public class TaikoSpeedAdjustmentContainer : SpeedAdjustmentContainer
    {
        private readonly ScrollingAlgorithm scrollingAlgorithm;

        public TaikoSpeedAdjustmentContainer(MultiplierControlPoint controlPoint, ScrollingAlgorithm scrollingAlgorithm)
            : base(controlPoint)
        {
            this.scrollingAlgorithm = scrollingAlgorithm;
        }

        protected override DrawableTimingSection CreateTimingSection()
        {
            switch (scrollingAlgorithm)
            {
                default:
                case ScrollingAlgorithm.Basic:
                    return new BasicScrollingDrawableTimingSection(ControlPoint);
                case ScrollingAlgorithm.Gravity:
                    return new GravityScrollingDrawableTimingSection(ControlPoint);
            }
        }
    }
}