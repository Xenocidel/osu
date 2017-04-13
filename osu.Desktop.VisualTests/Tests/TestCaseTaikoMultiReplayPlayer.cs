// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osu.Framework.Graphics;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;
using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using OpenTK.Graphics;
using System;
using OpenTK;
using osu.Framework.Input;
using osu.Framework.Extensions.IEnumerableExtensions;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using osu.Game.Modes.UI;
using osu.Game.Modes;
using osu.Game.Modes.Objects;
using osu.Game.Modes.Osu.Objects;
using osu.Framework.MathUtils;
using osu.Game.Beatmaps;
using osu.Desktop.VisualTests.Beatmaps;
using osu.Game.Database;
using osu.Framework.Timing;
using osu.Framework.Graphics.Primitives;
using osu.Game.Modes.Scoring;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Beatmaps.Timing;
using osu.Framework.Configuration;
using osu.Framework.Threading;
using osu.Game.Users;
using osu.Game.Modes.Taiko.Multiplayer;

namespace osu.Desktop.VisualTests.Tests
{
    internal class TestCaseTaikoMultiReplayPlayer : TestCase
    {
        public override void Reset()
        {
            base.Reset();

            MultiReplayPlayer player;
            Add(player = new MultiReplayPlayer(createTestBeatmap())
            {
                Clock = new FramedClock()
            });

            const int max_players = 3;
            for (int i = 0; i < max_players; i++)
            {
                var score = new Score
                {
                    User = new User { Username = $@"Test player" }
                };

                player.Teams.BlueTeam.AddPlayer(score);
                player.Teams.RedTeam.AddPlayer(score);
            }
        }

        private WorkingBeatmap createTestBeatmap()
        {
            Ruleset ruleset = Ruleset.GetRuleset(PlayMode.Taiko);

            List<HitObject> objects = new List<HitObject>();

            int time = 500;
            for (int i = 0; i < 20; i++)
            {
                objects.Add(new HitCircle
                {
                    StartTime = time,
                    Position = new Vector2(RNG.Next(0, 512), RNG.Next(0, 384)),
                    Scale = RNG.NextSingle(0.5f, 1.0f),
                });

                time += RNG.Next(50, 500);
            }

            Beatmap b = new Beatmap
            {
                HitObjects = objects,
                BeatmapInfo = new BeatmapInfo
                {
                    Difficulty = new BeatmapDifficulty(),
                    Metadata = new BeatmapMetadata
                    {
                        Artist = @"Unknown",
                        Title = @"Sample Beatmap",
                        Author = @"peppy",
                    }
                }
            };

            b.TimingInfo.ControlPoints.Add(new ControlPoint
            {
                Time = 0,
                TimeSignature = TimeSignatures.SimpleQuadruple,
                BeatLength = 1000
            });

            return new TestWorkingBeatmap(b);
        }
    }
}
