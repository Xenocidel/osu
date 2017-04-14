// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Online.API;

namespace osu.Game.Screens.Tournament.Play.Objects
{
    public class GetGameRequest : APIRequest<GetRoomResponse>
    {
        protected override string Target => $@"rooms/{roomId}";

        private readonly int roomId;

        public GetGameRequest(int roomId)
        {
            this.roomId = roomId;
        }
    }
}
