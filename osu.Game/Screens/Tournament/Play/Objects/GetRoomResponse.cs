// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using Newtonsoft.Json;

namespace osu.Game.Screens.Tournament.Play.Objects
{
    public class GetRoomResponse
    {
        [JsonProperty(PropertyName = "slots")]
        public Slot[] Slots;

        [JsonProperty(PropertyName = "room_name")]
        public string Name;

        [JsonProperty(PropertyName = "beatmap_checksum")]
        public string BeatmapChecksum;

        [JsonProperty(PropertyName = "play_mode")]
        public string PlayMode;

        [JsonProperty(PropertyName = "seed")]
        public int Seed;

        [JsonProperty(PropertyName = "in_progress")]
        public bool InProgress;
    }
}
