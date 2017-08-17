// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input;
using osu.Framework.MathUtils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.UI
{
    /// <summary>
    /// A type of <see cref="Playfield{TObject, TJudgement}"/> specialized towards scrolling <see cref="DrawableHitObject"/>s.
    /// </summary>
    public abstract class ScrollingPlayfield<TObject, TJudgement> : Playfield<TObject, TJudgement>
        where TObject : HitObject
        where TJudgement : Judgement
    {
        /// <summary>
        /// The default amount of time it takes hit objects to scroll through the bounds of the playfield.
        /// This is clamped between <see cref="min_scroll_time"/> and <see cref="max_scroll_time"/>.
        /// </summary>
        private const double default_scroll_time = 1500;
        /// <summary>
        /// The minimum amount of time it can take hit objects to scroll through the bounds of the playfield.
        /// </summary>
        private const double min_scroll_time = 50;
        /// <summary>
        /// The maximum amount of time it can take hit objects to scroll through the bounds of the playfield.
        /// </summary>
        private const double max_scroll_time = 10000;
        /// <summary>
        /// The step increase/decrease for <see cref="VisibleTimeRange"/>.
        /// </summary>
        private const double scroll_time_step = 50;

        private double scrollTime;
        /// <summary>
        /// The amount of time it takes hit objecst to scroll through the bounds of the playfield.
        /// </summary>
        public double ScrollTime
        {
            get { return scrollTime; }
            set { scrollTime = MathHelper.Clamp(value, min_scroll_time, max_scroll_time); }
        }

        public override void Add(DrawableHitObject<TObject, TJudgement> h) => base.Add(CreateScrollingWrapper(h));

        public override void Remove(DrawableHitObject<TObject, TJudgement> h)
        {
            var wrapper = HitObjects.Objects.First(w => w.Contains(h));
            var p = (DrawableHitObject)h.Parent;

            HitObjects.Remove(wrapper);
            p.Remove(h);
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (state.Keyboard.ControlPressed)
            {
                switch (args.Key)
                {
                    case Key.Minus:
                        transformScrollTimeTo(ScrollTime + scroll_time_step, 200, Easing.OutQuint);
                        break;
                    case Key.Plus:
                        transformScrollTimeTo(ScrollTime - scroll_time_step, 200, Easing.OutQuint);
                        break;
                }
            }

            return false;
        }

        private void transformScrollTimeTo(double newScrollTime, double duration = 0, Easing easing = Easing.None)
            => this.TransformTo(nameof(ScrollTime), newScrollTime, duration, easing);

        /// <summary>
        /// Creates a container which wraps a <see cref="DrawableHitObject<TObject, Tjudgement>"/> to handle scrolling the hit object.
        /// </summary>
        /// <param name="hitObject">The <see cref="DrawableHitObject<TObject, TJudgement>"/> to create the wrapper for.</param>
        /// <returns>The <see cref="ScrollingWrapper<TObject, TJudgement>"/> wrapper.</returns>
        protected abstract ScrollingWrapper<TObject, TJudgement> CreateScrollingWrapper(DrawableHitObject<TObject, TJudgement> hitObject);
    }
}
