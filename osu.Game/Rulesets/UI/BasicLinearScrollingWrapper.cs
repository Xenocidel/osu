using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.UI
{
    public class BasicLinearScrollingWrapper<TObject, TJudgement> : DrawableScrollingHitObject<TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
    {
        private BeatmapDifficulty difficulty;
        private ControlPointInfo controlPoints;

        protected BasicLinearScrollingWrapper(DrawableHitObject<TObject, TJudgement> hitObject, ScrollDirection scrollDirection)
            : base(hitObject)
        {
        }

        public enum ScrollDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
