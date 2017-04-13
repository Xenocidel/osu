// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Database;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiTeamContainer : Container<MultiTeam>
    {
        public MultiTeam BlueTeam;
        public MultiTeam RedTeam;

        private readonly WorkingBeatmap beatmap;

        public MultiTeamContainer(WorkingBeatmap beatmap)
        {
            this.beatmap = beatmap;

            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapDatabase database)
        {
            // To achieve proper masking of playfields, we use two vertically-relative columns
            // and apply the offset of the play fields to the contents of the columns
            Children = new[]
            {
                BlueTeam = CreateTeam(false, beatmap),
                RedTeam = CreateTeam(true, beatmap)
            };

            BlueTeam.BindTeamColumn(RedTeam);
        }

        protected abstract MultiTeam CreateTeam(bool teamRed, WorkingBeatmap beatmap);
    }
}
