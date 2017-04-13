// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;
using osu.Game.Modes.Scoring;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiTeam : Screens.Tournament.Play.MultiTeam
    {
        public MultiTeam(bool teamRed, WorkingBeatmap beatmap)
            : base(teamRed, beatmap)
        {
        }

        public override Screens.Tournament.Play.MultiPlayer CreatePlayer(bool teamRed, Score score, WorkingBeatmap beatmap) => new MultiPlayer(teamRed, score, beatmap);
    }
}
