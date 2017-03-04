﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Modes.Objects;
using osu.Game.Modes.Objects.Drawables;
using osu.Game.Beatmaps;

namespace osu.Game.Modes.UI
{
    public abstract class HitRenderer : Container
    {
        public event Action<JudgementInfo> OnJudgement;

        public event Action OnAllJudged;

        protected void TriggerOnJudgement(JudgementInfo j)
        {
            OnJudgement?.Invoke(j);
            if (AllObjectsJudged)
                OnAllJudged?.Invoke();
        }

        protected Playfield Playfield;

        public bool AllObjectsJudged => Playfield.HitObjects.Children.First()?.Judgement.Result != null; //reverse depth sort means First() instead of Last().

        public IEnumerable<DrawableHitObject> DrawableObjects => Playfield.HitObjects.Children;
    }

    public abstract class HitRenderer<T> : HitRenderer
        where T : HitObject
    {
        protected abstract HitObjectConverter<T> Converter { get; }

        protected virtual List<T> Convert(Beatmap beatmap) => Converter.Convert(beatmap);

        private Beatmap beatmap;

        public HitRenderer(Beatmap beatmap)
        {
            this.beatmap = beatmap;

            RelativeSizeAxes = Axes.Both;
        }

        protected abstract Playfield CreatePlayfield();

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                Playfield = CreatePlayfield()
            };

            loadObjects();
        }

        private void loadObjects()
        {
            foreach (T h in Convert(beatmap))
            {
                var drawableObject = GetVisualRepresentation(h);

                if (drawableObject == null)
                    continue;

                drawableObject.OnJudgement += onJudgement;

                Playfield.Add(drawableObject);
            }

            Playfield.PostProcess();
        }

        private void onJudgement(DrawableHitObject o, JudgementInfo j) => TriggerOnJudgement(j);

        protected abstract DrawableHitObject GetVisualRepresentation(T h);
    }
}
