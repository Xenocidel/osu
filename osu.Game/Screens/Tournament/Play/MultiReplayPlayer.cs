// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Tournament.Play.Components;
using System.Linq;
using osu.Game.Online.API;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Tournament.Play.Objects;
using osu.Game.Database;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace osu.Game.Screens.Tournament.Play
{
    public abstract class MultiReplayPlayer : OsuScreen
    {
        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;

                if (!IsLoaded)
                    return;

                headerDescription.Text = header;
            }
        }

        public int RoomId;

        private BeatmapDatabase beatmaps;
        private APIAccess api;
        private StarCounter blueStarCounter;
        private StarCounter redStarCounter;
        private Sprite backgroundSprite;

        private OsuSpriteText headerDescription;
        private OsuSpriteText headerBeatmap;

        public Container<MultiTeam> Teams;

        private MultiTeam blueTeam;
        private MultiTeam redTeam;

        private WorkingBeatmap currentWorkingBeatmap;
        private int teamsCompleted;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, OsuColour colours, APIAccess api, BeatmapDatabase beatmaps)
        {
            this.api = api;
            this.beatmaps = beatmaps;

            Children = new Drawable[]
            {
                backgroundSprite = new Sprite
                {
                    FillMode = FillMode.Fill,
                    Texture = textures.Get(@"Backgrounds/Tournament/background")
                },
                new FillFlowContainer
                {
                    Name = "Header info",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Y = 50,
                    Direction = FillDirection.Vertical,
                    Children = new[]
                    {
                        headerDescription = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = Header,
                            TextSize = 24,
                            Font = "Exo2.0-LightItalic"
                        },
                        headerBeatmap = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            TextSize = 14,
                            Font = "Exo2.0-RegularItalic"
                        }
                    }
                },
                blueStarCounter = new StarCounter
                {
                    Origin = Anchor.CentreLeft,
                    Position = new Vector2(30, 50),
                    MaxCount = 3,
                    AccentColour = colours.BlueDarker
                },
                redStarCounter = new StarCounter
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.CentreRight,
                    Position = new Vector2(-30, 50),
                    MaxCount = 3,
                    Direction = StarCounterDirection.RightToLeft,
                    AccentColour = colours.PinkDarker
                },
                Teams = new Container<MultiTeam>
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new[]
                    {
                        blueTeam = new MultiTeam(false),
                        redTeam = new MultiTeam(true)
                        {
                            RelativePositionAxes = Axes.X,
                            X = 0.5f
                        }
                    }
                },
            };

            blueTeam.BindTeam(redTeam);
            redTeam.OnTeamCompletion = onTeamComplete;
            blueTeam.OnTeamCompletion = onTeamComplete;

            // Begin polling
            poll();
            Scheduler.AddDelayed(poll, 1000, true);
        }

        public void poll()
        {
            // Todo: Re-enable
            var req = new GetGameRequest(RoomId);
            req.Success += updateRoom;
            api.Queue(req);

            // Todo: Disable
            //updateRoom(new GetRoomResponse
            //{
            //    Slots = new[]
            //    {
            //        new Slot
            //        {
            //            UserId = 1040328,
            //            Team = "Blue"
            //        }
            //    },
            //    Name = "Some room",
            //    PlayMode = "Taiko",
            //    InProgress = true,
            //});
        }

        private void updateRoom(GetRoomResponse response)
        {
            if (currentWorkingBeatmap == null || currentWorkingBeatmap.BeatmapInfo.Hash != response.BeatmapChecksum)
            {
                BeatmapInfo beatmap = beatmaps.Query<BeatmapInfo>().FirstOrDefault(b => b.Hash == response.BeatmapChecksum);
                currentWorkingBeatmap = beatmap == null ? null : beatmaps.GetWorkingBeatmap(beatmap);
            }

            if (currentWorkingBeatmap == null)
            {
                headerBeatmap.FadeOut(100);
                return;
            }

            // Set the header text
            BeatmapMetadata metadata = currentWorkingBeatmap.BeatmapInfo.Metadata;
            headerBeatmap.Text = $@"{metadata.Artist} - {metadata.Title} [{currentWorkingBeatmap.BeatmapInfo.Version}]";
            headerBeatmap.FadeIn(100);

            // Remove the old players that aren't in the room anymore
            Teams.Children.ForEach(t => t.Players.Children.ForEach(p =>
            {
                string team = t == redTeam ? @"Red" : @"Blue";

                if (!response.Slots.Any(slot => slot.UserId == p.UserId && slot.Team == team))
                {
                    p.FadeOut(500);
                    p.Expire();
                }
            }));

            // Add new players
            response.Slots.ForEach(Slot =>
            {
                // Skip "empty" slots
                if (Slot.UserId == -1)
                    return;

                // Skip slots where the player already exists
                if (Teams.Children.Any(t => t.Players.Children.Any(p => p.UserId == Slot.UserId)))
                    return;

                switch (Slot.Team)
                {
                    case @"Blue":
                        blueTeam.AddPlayer(CreatePlayer(false, Slot.UserId, currentWorkingBeatmap));
                        break;
                    case @"Red":
                        redTeam.AddPlayer(CreatePlayer(true, Slot.UserId, currentWorkingBeatmap));
                        break;
                }
            });
        }

        private void onTeamComplete()
        {
            teamsCompleted++;

            if (teamsCompleted == Teams.Children.Count())
            {
                // Completion sequence
                OnCompleted(redTeam.Score > blueTeam.Score);
            }
        }

        protected virtual void OnCompleted(bool winningTeamWasRed) { }

        protected abstract MultiPlayer CreatePlayer(bool teamRed, int userId, WorkingBeatmap beatmap);
    }
}
