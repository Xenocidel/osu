// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Modes.Scoring;
using osu.Game.Modes.UI;
using System;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiPlayer : Container
    {
        public Action OnCompletion;
        public Action OnFail;

        public readonly Bindable<double> Score = new Bindable<double>();

        private HitRenderer hitRenderer;
        private HudOverlay hudOverlay;
        private ScoreProcessor scoreProcessor;

        public MultiPlayer(bool teamRed, Score score, WorkingBeatmap beatmap)
        {
            RelativeSizeAxes = Axes.Both;

            Children = new Drawable[]
            {
                hitRenderer = CreateHitRenderer(beatmap),
                hudOverlay = CreateHudOverlay(teamRed, score)
            };

            scoreProcessor = hitRenderer.CreateScoreProcessor();

            hitRenderer.OnAllJudged += () => OnCompletion?.Invoke();
            scoreProcessor.Failed += () => OnFail?.Invoke();

            Score.BindTo(scoreProcessor.TotalScore);
        }

        protected abstract HitRenderer CreateHitRenderer(WorkingBeatmap beatmap);
        protected abstract HudOverlay CreateHudOverlay(bool teamRed, Score score);
    }
}
