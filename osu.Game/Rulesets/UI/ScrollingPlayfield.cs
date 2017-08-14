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
using osu.Game.Rulesets.Timing;

namespace osu.Game.Rulesets.UI
{
    /// <summary>
    /// A type of <see cref="Playfield{TObject, TJudgement}"/> specialized towards scrolling <see cref="DrawableHitObject"/>s.
    /// </summary>
    public class ScrollingPlayfield<TObject, TJudgement> : Playfield<TObject, TJudgement>
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

        private List<ScrollingPlayfield<TObject, TJudgement>> nestedPlayfields;
        /// <summary>
        /// All the <see cref="ScrollingPlayfield{TObject, TJudgement}"/>s nested inside this playfield.
        /// </summary>
        public IEnumerable<ScrollingPlayfield<TObject, TJudgement>> NestedPlayfields => nestedPlayfields;

        /// <summary>
        /// Adds a <see cref="ScrollingPlayfield{TObject, TJudgement}"/> to this playfield. The nested <see cref="ScrollingPlayfield{TObject, TJudgement}"/>
        /// will be given all of the same speed adjustments as this playfield.
        /// </summary>
        /// <param name="otherPlayfield">The <see cref="ScrollingPlayfield{TObject, TJudgement}"/> to add.</param>
        protected void AddNested(ScrollingPlayfield<TObject, TJudgement> otherPlayfield)
        {
            if (nestedPlayfields == null)
                nestedPlayfields = new List<ScrollingPlayfield<TObject, TJudgement>>();

            nestedPlayfields.Add(otherPlayfield);
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
    }
}