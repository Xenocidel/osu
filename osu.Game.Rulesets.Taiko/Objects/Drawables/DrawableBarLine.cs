// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using OpenTK;
using osu.Game.Rulesets.Objects.Drawables;
using System;
using osu.Game.Rulesets.Taiko.Objects.Drawables.Pieces;

namespace osu.Game.Rulesets.Taiko.Objects.Drawables
{
    /// <summary>
    /// A line that scrolls alongside hit objects in the playfield and visualises control points.
    /// </summary>
    public class DrawableBarLine : DrawableHitObject<BarLine>
    {
        /// <summary>
        /// The width of the line tracker.
        /// </summary>
        private const float tracker_width = 2f;

        /// <summary>
        /// Fade out time calibrated to a pre-empt of 1000ms.
        /// </summary>
        private const float base_fadeout_time = 100f;

        /// <summary>
        /// The visual line tracker.
        /// </summary>
        protected Box Tracker;

        public DrawableBarLine(BarLine barLine)
            : base(barLine)
        {
            RelativePositionAxes = Axes.X;
            X = (float)barLine.StartTime;

            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;

            Add(Tracker = new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Y,
                Width = tracker_width,
                EdgeSmoothness = new Vector2(0.5f, 0),
                Alpha = 0.75f
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Delay(HitObject.StartTime - Time.Current);
        }
    }
}