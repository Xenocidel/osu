using System.Collections.Generic;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.Taiko.Timing.Drawables;
using osu.Game.Rulesets.Taiko.UI;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModGravity : Mod, IApplicableMod<TaikoHitObject>
    {
        public override string Name => "Gravity";

        public override double ScoreMultiplier => 0;

        public override FontAwesome Icon => FontAwesome.fa_sort_desc;

        public void ApplyToHitRenderer(HitRenderer<TaikoHitObject> hitRenderer)
        {
            var taikoHitRenderer = (TaikoHitRenderer)hitRenderer;

            taikoHitRenderer.TimingChanges = new List<TimingChange>();

            foreach (HitObject obj in taikoHitRenderer.Objects)
            {
                var taikoObject = obj as TaikoHitObject;
                if (taikoObject == null)
                    continue;

                taikoHitRenderer.TimingChanges.Add(new TimingChange
                {
                    Time = obj.StartTime,
                    BeatLength = 1000
                });
            }
        }
    }
}