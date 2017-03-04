﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Modes.Objects;
using osu.Game.Modes.Objects.Drawables;
using osu.Game.Modes.Taiko.Objects.Drawables;
using osu.Game.Modes.Taiko.Objects;
using osu.Game.Modes.UI;
using osu.Framework.Graphics;
using OpenTK;
using osu.Game.Beatmaps;

namespace osu.Game.Modes.Taiko.UI
{
    public class TaikoHitRenderer : HitRenderer<TaikoHitObject>
    {
        protected override HitObjectConverter<TaikoHitObject> Converter => new TaikoConverter();

        private Beatmap beatmap;

        public TaikoHitRenderer(Beatmap beatmap)
            : base(beatmap)
        {
            this.beatmap = beatmap;
        }

        protected override Playfield CreatePlayfield() => new TaikoPlayfield()
        {
            RelativePositionAxes = Axes.Y,
            Position = new Vector2(0, 0.4f)
        };

        protected override DrawableHitObject GetVisualRepresentation(TaikoHitObject h)
        {
            if (h is HitCircle)
            {
                switch (h.Type)
                {
                    case TaikoHitType.Don:
                        if (h.IsFinisher)
                            return new DrawableHitCircleDonFinisher(h as HitCircle);
                        return new DrawableHitCircleDon(h as HitCircle);
                    case TaikoHitType.Katsu:
                        if (h.IsFinisher)
                            return new DrawableHitCircleKatsuFinisher(h as HitCircle);
                        return new DrawableHitCircleKatsu(h as HitCircle);
                }
            }

            if (h is DrumRoll)
            {
                if (h.IsFinisher)
                    return new DrawableDrumRollFinisher(h as DrumRoll);
                return new DrawableDrumRoll(h as DrumRoll);
            }

            if (h is Bash)
                return new DrawableBash(h as Bash);

            return null;
        }
    }
}
