// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiReplayPlayer : Game.Screens.Tournament.Play.MultiReplayPlayer
    {
        public MultiReplayPlayer(WorkingBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override Screens.Tournament.Play.MultiTeamContainer CreateTeamContainer(WorkingBeatmap beatmap) => new MultiTeamContainer(beatmap);
    }
}
