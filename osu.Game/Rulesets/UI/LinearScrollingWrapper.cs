// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.UI
{
    public class LinearScrollingWrapper<TObject, TJudgement> : ScrollingWrapper<TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
    {
        private readonly ScrollDirection scrollDirection;

        public LinearScrollingWrapper(DrawableHitObject<TObject, TJudgement> hitObject, ScrollDirection scrollDirection)
            : base(hitObject)
        {
            this.scrollDirection = scrollDirection;
        }

        protected override void UpdateScroll(float completion)
        {
            Vector2 direction = HitObject.Position;
            switch (scrollDirection)
            {
                case ScrollDirection.Up:
                    direction.Y = -1;
                    break;
                case ScrollDirection.Down:
                    direction.Y = 1;
                    break;
                case ScrollDirection.Left:
                    direction.X = -1;
                    break;
                case ScrollDirection.Right:
                    direction.Y = 1;
                    break;
            }

            direction *= completion;
            Position = direction * completion;
        }
    }

    public enum ScrollDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
