// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE


using System;
using osu.Game.Beatmaps;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiReplayPlayer : Screens.Tournament.Play.MultiReplayPlayer
    {
        protected override Screens.Tournament.Play.MultiPlayer CreatePlayer(bool teamRed, int userId, WorkingBeatmap beatmap) => new MultiPlayer(teamRed, userId, beatmap);
    }
}
