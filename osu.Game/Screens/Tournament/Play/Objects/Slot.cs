// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using Newtonsoft.Json;

namespace osu.Game.Screens.Tournament.Play.Objects
{
    public class Slot
    {
        [JsonProperty(PropertyName = "user_id")]
        public int UserId;

        [JsonProperty(PropertyName = "status")]
        public string Status;

        [JsonProperty(PropertyName = "team")]
        public string Team;

        [JsonProperty(PropertyName = @"mods")]
        public string[] Mods;
    }
}
