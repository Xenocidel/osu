// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Rulesets.Timing;

namespace osu.Game.Rulesets.Taiko.Timing.Drawables
{
    public class DrawableScrollingTimingChange : DrawableTaikoTimingChange
    {
        public DrawableScrollingTimingChange(TimingChange timingChange)
            : base(timingChange)
        {
        }

        protected override void Update()
        {
            base.Update();

            Content.X = (float)(TimingChange.Time - Time.Current);
        }
    }
}