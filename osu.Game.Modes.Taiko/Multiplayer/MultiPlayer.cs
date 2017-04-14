// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;
using osu.Game.Modes.UI;
using osu.Game.Modes.Taiko.UI;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiPlayer : Screens.Tournament.Play.MultiPlayer
    {
        public MultiPlayer(bool teamRed, int userId, WorkingBeatmap beatmap)
            : base(teamRed, userId, beatmap)
        {
        }

        protected override HitRenderer CreateHitRenderer(WorkingBeatmap beatmap) => new TaikoHitRenderer(beatmap)
        {
            Origin = Anchor.BottomLeft,
            Anchor = Anchor.BottomLeft,
            Height = 0.65f,
            Margin = new MarginPadding { Bottom = 5 },
            AspectAdjust = false,
        };

        protected override Screens.Tournament.Play.MultiHudOverlay CreateHudOverlay(bool teamRed) => new MultiHudOverlay(teamRed);
    }
}
