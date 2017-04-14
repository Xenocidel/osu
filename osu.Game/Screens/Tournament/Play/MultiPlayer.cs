// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Modes.Scoring;
using osu.Game.Modes.UI;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Users;
using System;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiPlayer : Container
    {
        public Action OnCompletion;
        public Action OnFail;

        public readonly Bindable<double> Score = new Bindable<double>();
        public readonly int UserId;

        private HitRenderer hitRenderer;
        private MultiHudOverlay hudOverlay;
        private ScoreProcessor scoreProcessor;

        public MultiPlayer(bool teamRed, int userId, WorkingBeatmap beatmap)
        {
            UserId = userId;

            RelativeSizeAxes = Axes.Both;

            Children = new Drawable[]
            {
                hitRenderer = CreateHitRenderer(beatmap),
                hudOverlay = CreateHudOverlay(teamRed)
            };

            scoreProcessor = hitRenderer.CreateScoreProcessor();

            hitRenderer.OnAllJudged += () => OnCompletion?.Invoke();
            scoreProcessor.Failed += () => OnFail?.Invoke();

            Score.BindTo(scoreProcessor.TotalScore);
        }

        [BackgroundDependencyLoader]
        private void load(APIAccess api)
        {
            if (api == null)
                return;

            var req = new GetUserRequest(UserId);
            req.Success += userReceived;
            api.Queue(req);
        }

        private void userReceived(User content)
        {
            hudOverlay.PlayerName = content.Username;
        }

        protected abstract HitRenderer CreateHitRenderer(WorkingBeatmap beatmap);
        protected abstract MultiHudOverlay CreateHudOverlay(bool teamRed);
    }
}
