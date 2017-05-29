using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Replays;
using osu.Game.Users;

namespace osu.Game.Rulesets.Mania.Replays
{
    public class ManiaAutoGenerator : AutoGenerator<ManiaHitObject>
    {
        protected Replay Replay;
        protected List<ReplayFrame> Frames => Replay.Frames;

        public ManiaAutoGenerator(Beatmap<ManiaHitObject> beatmap)
            : base(beatmap)
        {
            Replay = new Replay
            {
                User = new User
                {
                    Username = "Autoplay"
                }
            };
        }

        public override Replay Generate()
        {
            // We generate frames by rows of hit objects, starting with normal notes
            foreach (var objectGroup in Beatmap.HitObjects.OfType<Note>().GroupBy(h => h.StartTime))
            {
                int activeColumns = 0;
                foreach (Note note in objectGroup)
                    activeColumns |= columnValue(note.Column);

                Replay.Frames.Add(new ReplayFrame(objectGroup.First().StartTime, activeColumns, null, ReplayButtonState.None));
                Replay.Frames.Add(new ReplayFrame(objectGroup.First().StartTime + 1, 0, null, ReplayButtonState.None));
            }

            return Replay;
        }

        private int columnValue(int column) => (int)Math.Pow(2, column);
    }
}