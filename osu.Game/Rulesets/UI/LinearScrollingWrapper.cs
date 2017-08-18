// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
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
            RelativePositionAxes |= Axes.Both;
        }

        protected override void UpdateScroll(float completion)
        {
            switch (scrollDirection)
            {
                case ScrollDirection.Up:
                    Position = new Vector2(HitObject.Position.X, -completion);
                    break;
                case ScrollDirection.Down:
                    Position = new Vector2(HitObject.Position.X, completion);
                    break;
                case ScrollDirection.Left:
                    Position = new Vector2(-completion, HitObject.Position.Y);
                    break;
                case ScrollDirection.Right:
                    Position = new Vector2(completion, HitObject.Position.Y);
                    break;
            }
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
