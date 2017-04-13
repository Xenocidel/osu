// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;
using osu.Framework.Graphics;

namespace osu.Game.Modes.Taiko.Multiplayer
{
    public class MultiTeamContainer : Screens.Tournament.Play.MultiTeamContainer
    {
        public MultiTeamContainer(WorkingBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override Screens.Tournament.Play.MultiTeam CreateTeam(bool teamRed, WorkingBeatmap beatmap) => new MultiTeam(teamRed, beatmap)
        {
            RelativePositionAxes = Axes.X,
            X = teamRed ? 0.5f : 0
        };
    }
}
