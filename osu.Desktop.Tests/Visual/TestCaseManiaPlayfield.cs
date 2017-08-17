﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Linq;
using osu.Framework.Configuration;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Timing;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.Objects.Drawables;
using osu.Game.Rulesets.Mania.UI;
using OpenTK;
using OpenTK.Input;

namespace osu.Desktop.Tests.Visual
{
    internal class TestCaseManiaPlayfield : OsuTestCase
    {
        public override string Description => @"Mania playfield";

        protected override double TimePerAction => 200;

        public TestCaseManiaPlayfield()
        {
            Action<int, SpecialColumnPosition> createPlayfield = (cols, pos) =>
            {
                Clear();
                Add(new ManiaPlayfield(cols)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    SpecialColumnPosition = pos,
                    Scale = new Vector2(1, -1)
                });
            };

            const double start_time = 500;
            const double duration = 500;

            Action<bool> createPlayfieldWithNotes = gravity =>
            {
                Clear();

                var rateAdjustClock = new StopwatchClock(true) { Rate = 1 };

                ManiaPlayfield playField;
                Add(playField = new ManiaPlayfield(4)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(1, -1),
                    Clock = new FramedClock(rateAdjustClock)
                });

                for (double t = start_time; t <= start_time + duration; t += 100)
                {
                    playField.Add(new DrawableNote(new Note
                    {
                        StartTime = t,
                        Column = 0
                    }, new Bindable<Key>(Key.D)));

                    playField.Add(new DrawableNote(new Note
                    {
                        StartTime = t,
                        Column = 3
                    }, new Bindable<Key>(Key.K)));
                }

                playField.Add(new DrawableHoldNote(new HoldNote
                {
                    StartTime = start_time,
                    Duration = duration,
                    Column = 1
                }, new Bindable<Key>(Key.F)));

                playField.Add(new DrawableHoldNote(new HoldNote
                {
                    StartTime = start_time,
                    Duration = duration,
                    Column = 2
                }, new Bindable<Key>(Key.J)));
            };

            AddStep("1 column", () => createPlayfield(1, SpecialColumnPosition.Normal));
            AddStep("4 columns", () => createPlayfield(4, SpecialColumnPosition.Normal));
            AddStep("Left special style", () => createPlayfield(4, SpecialColumnPosition.Left));
            AddStep("Right special style", () => createPlayfield(4, SpecialColumnPosition.Right));
            AddStep("5 columns", () => createPlayfield(5, SpecialColumnPosition.Normal));
            AddStep("8 columns", () => createPlayfield(8, SpecialColumnPosition.Normal));
            AddStep("Left special style", () => createPlayfield(8, SpecialColumnPosition.Left));
            AddStep("Right special style", () => createPlayfield(8, SpecialColumnPosition.Right));

            AddStep("Notes with input", () => createPlayfieldWithNotes(false));
            AddWaitStep((int)Math.Ceiling((start_time + duration) / TimePerAction));

            AddStep("Notes with gravity", () => createPlayfieldWithNotes(true));
            AddWaitStep((int)Math.Ceiling((start_time + duration) / TimePerAction));
        }

        private void triggerKeyDown(Column column)
        {
            column.TriggerOnKeyDown(new InputState(), new KeyDownEventArgs
            {
                Key = column.Key,
                Repeat = false
            });
        }

        private void triggerKeyUp(Column column)
        {
            column.TriggerOnKeyUp(new InputState(), new KeyUpEventArgs
            {
                Key = column.Key
            });
        }
    }
}
