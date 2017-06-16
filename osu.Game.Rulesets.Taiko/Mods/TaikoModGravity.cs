using System;
using System.Collections.Generic;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.Taiko.Objects.Drawables;
using osu.Game.Rulesets.Taiko.Timing;
using osu.Game.Rulesets.Taiko.Timing.Drawables;
using osu.Game.Rulesets.Taiko.UI;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModGravity : Mod, IGenerateSpeedAdjustments
    {
        public override string Name => "Gravity";

        public override double ScoreMultiplier => 0;

        public override FontAwesome Icon => FontAwesome.fa_sort_desc;

        public void ApplyToHitRenderer(TaikoHitRenderer hitRenderer, ref List<SpeedAdjustmentContainer> hitObjectTimingChanges, ref List<SpeedAdjustmentContainer> barlineTimingChanges)
        {
            // We have to generate one speed adjustment per hit object for gravity
            foreach (TaikoHitObject obj in hitRenderer.Objects)
            {
                MultiplierControlPoint controlPoint = hitRenderer.CreateControlPointAt(obj.StartTime);
                // Beat length has too large of an effect for gravity, so we'll force it to a constant value for now
                controlPoint.TimingPoint.BeatLength = 1000;

                hitObjectTimingChanges.Add(new TaikoSpeedAdjustmentContainer(controlPoint, ScrollingAlgorithm.Gravity));
            }

            // Like with hit objects, we need to generate one speed adjustment per bar line
            foreach (DrawableBarLine barLine in hitRenderer.BarLines)
            {
                var controlPoint = hitRenderer.CreateControlPointAt(barLine.HitObject.StartTime);
                // Beat length has too large of an effect for gravity, so we'll force it to a constant value for now
                controlPoint.TimingPoint.BeatLength = 1000;

                barlineTimingChanges.Add(new TaikoSpeedAdjustmentContainer(controlPoint, ScrollingAlgorithm.Gravity));
            }
        }
    }
}