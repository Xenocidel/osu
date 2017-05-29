using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using osu.Framework.Configuration;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Mania.Replays
{
    public class ManiaFramedReplayInputHandler : FramedReplayInputHandler
    {
        private readonly List<Bindable<Key>> keys;

        public ManiaFramedReplayInputHandler(IEnumerable<Bindable<Key>> keys, Replay replay)
            : base(replay)
        {
            this.keys = keys.ToList();
        }

        public override List<InputState> GetPendingStates()
        {
            var keys = new List<Key>();

            int x = (int)CurrentFrame.Position.X;

            for (int i = 0; i < this.keys.Count; i++)
                if ((x & (int)Math.Pow(2, i)) > 0)
                    keys.Add(this.keys[i]);

            return new List<InputState> { new InputState { Keyboard = new ReplayKeyboardState(keys) } };
        }
    }
}