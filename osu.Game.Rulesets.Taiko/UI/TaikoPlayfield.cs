// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.UI;
using OpenTK;
using OpenTK.Graphics;
using osu.Game.Rulesets.Taiko.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Extensions.Color4Extensions;
using System.Linq;
using osu.Game.Rulesets.Taiko.Objects.Drawables;
using System;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.Taiko.Timing.Drawables;
using osu.Framework.Configuration;
using osu.Framework.Input;
using osu.Framework.Graphics.Transforms;
using osu.Framework.MathUtils;
using OpenTK.Input;

namespace osu.Game.Rulesets.Taiko.UI
{
    public class TaikoPlayfield : Playfield<TaikoHitObject, TaikoJudgement>
    {
        /// <summary>
        /// The default play field height.
        /// </summary>
        public const float DEFAULT_PLAYFIELD_HEIGHT = 178f;

        /// <summary>
        /// The offset from <see cref="left_area_size"/> which the center of the hit target lies at.
        /// </summary>
        public const float HIT_TARGET_OFFSET = TaikoHitObject.DEFAULT_STRONG_CIRCLE_DIAMETER / 2f + 40;

        /// <summary>
        /// The size of the left area of the playfield. This area contains the input drum.
        /// </summary>
        private const float left_area_size = 240;

        private const double time_span_default = 6000;
        private const double time_span_min = 50;
        private const double time_span_max = 10000;
        private const double time_span_step = 50;

        private readonly BindableDouble visibleTimeRange = new BindableDouble(time_span_default)
        {
            MinValue = time_span_min,
            MaxValue = time_span_max
        };

        private readonly Container<HitExplosion> hitExplosionContainer;
        private readonly Container<KiaiHitExplosion> kiaiExplosionContainer;
        private readonly SpeedAdjustmentCollection barLineContainer;
        private readonly Container<DrawableTaikoJudgement> judgementContainer;

        private readonly SpeedAdjustmentCollection hitObjectContainer;
        private readonly Container topLevelHitContainer;

        private readonly Container overlayBackgroundContainer;
        private readonly Container backgroundContainer;

        private readonly Box overlayBackground;
        private readonly Box background;

        public TaikoPlayfield()
        {
            Children = new Drawable[]
            {
                new ScaleFixContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = DEFAULT_PLAYFIELD_HEIGHT,
                    Children = new[]
                    {
                        backgroundContainer = new Container
                        {
                            Name = "Transparent playfield background",
                            RelativeSizeAxes = Axes.Both,
                            BorderThickness = 2,
                            Masking = true,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.2f),
                                Radius = 5,
                            },
                            Children = new Drawable[]
                            {
                                background = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Alpha = 0.6f
                                },
                            }
                        },
                        new Container
                        {
                            Name = "Right area",
                            RelativeSizeAxes = Axes.Both,
                            Margin = new MarginPadding { Left = left_area_size },
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    Name = "Hit explosion mask",
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                    Masking = true,
                                    Children = new[]
                                    {
                                        hitExplosionContainer = new Container<HitExplosion>
                                        {
                                            RelativeSizeAxes = Axes.Y,
                                            BlendingMode = BlendingMode.Additive,
                                        },
                                    }
                                },
                                new Container
                                {
                                    Name = "Bar line mask",
                                    RelativeSizeAxes = Axes.Both,
                                    // A "sane" value because we only want to mask along one axis
                                    Y = 4,
                                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                    Masking = true,
                                    Children = new[]
                                    {
                                        new Container
                                        {
                                            Name = "Bar line re-sizer",
                                            RelativeSizeAxes = Axes.Both,
                                            // Inverse of the bar line mask so we get them back to their original size
                                            Y = 0.25f,
                                            Children = new[]
                                            {
                                                barLineContainer = new SpeedAdjustmentCollection
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    VisibleTimeRange = visibleTimeRange
                                                },
                                            }
                                        }

                                    }
                                },
                                new Container
                                {
                                    Name = "Hit objects mask",
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new HitTarget
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.Centre,
                                        },
                                        hitObjectContainer = new SpeedAdjustmentCollection
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            VisibleTimeRange = visibleTimeRange
                                        },
                                    }
                                },
                                kiaiExplosionContainer = new Container<KiaiHitExplosion>
                                {
                                    Name = "Kiai hit explosions",
                                    RelativeSizeAxes = Axes.Y,
                                    Margin = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                    BlendingMode = BlendingMode.Additive
                                },
                                judgementContainer = new Container<DrawableTaikoJudgement>
                                {
                                    Name = "Judgements",
                                    RelativeSizeAxes = Axes.Y,
                                    Margin = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                    BlendingMode = BlendingMode.Additive
                                },
                            }
                        },
                        overlayBackgroundContainer = new Container
                        {
                            Name = "Left overlay",
                            Size = new Vector2(left_area_size, DEFAULT_PLAYFIELD_HEIGHT),
                            BorderThickness = 1,
                            Children = new Drawable[]
                            {
                                overlayBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new InputDrum
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativePositionAxes = Axes.X,
                                    Position = new Vector2(0.10f, 0),
                                    Scale = new Vector2(0.9f)
                                },
                                new Box
                                {
                                    Anchor = Anchor.TopRight,
                                    RelativeSizeAxes = Axes.Y,
                                    Width = 10,
                                    ColourInfo = Framework.Graphics.Colour.ColourInfo.GradientHorizontal(Color4.Black.Opacity(0.6f), Color4.Black.Opacity(0)),
                                },
                            }
                        },
                    }
                },
                topLevelHitContainer = new Container
                {
                    Name = "Top level hit objects",
                    RelativeSizeAxes = Axes.Both,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            overlayBackgroundContainer.BorderColour = colours.Gray0;
            overlayBackground.Colour = colours.Gray1;

            backgroundContainer.BorderColour = colours.Gray1;
            background.Colour = colours.Gray0;
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (state.Keyboard.ControlPressed)
            {
                switch (args.Key)
                {
                    case Key.Minus:
                        transformVisibleTimeRangeTo(visibleTimeRange + time_span_step, 200, EasingTypes.OutQuint);
                        break;
                    case Key.Plus:
                        transformVisibleTimeRangeTo(visibleTimeRange - time_span_step, 200, EasingTypes.OutQuint);
                        break;
                }
            }

            return false;
        }

        public override void Add(DrawableHitObject<TaikoHitObject, TaikoJudgement> h)
        {
            h.Depth = (float)h.HitObject.StartTime;

            hitObjectContainer.Add(h);

            // Swells should be moved at the very top of the playfield when they reach the hit target
            var swell = h as DrawableSwell;
            if (swell != null)
                swell.OnStart += () => topLevelHitContainer.Add(swell.CreateProxy());
        }

        public void Add(DrawableBarLine barLine) => barLineContainer.Add(barLine);

        public void AddBarLineSpeedAdjustment(SpeedAdjustmentContainer speedAdjustment) => barLineContainer.Add(speedAdjustment);
        public void AddHitObjectSpeedAdjustment(SpeedAdjustmentContainer speedAdjustment) => hitObjectContainer.Add(speedAdjustment);

        public override void OnJudgement(DrawableHitObject<TaikoHitObject, TaikoJudgement> judgedObject)
        {
            bool wasHit = judgedObject.Judgement.Result == HitResult.Hit;
            bool secondHit = judgedObject.Judgement.SecondHit;

            judgementContainer.Add(new DrawableTaikoJudgement(judgedObject.Judgement)
            {
                Anchor = wasHit ? Anchor.TopLeft : Anchor.CentreLeft,
                Origin = wasHit ? Anchor.BottomCentre : Anchor.Centre,
                RelativePositionAxes = Axes.X,
                X = wasHit ? judgedObject.Position.X : 0,
            });

            if (!wasHit)
                return;

            bool isRim = judgedObject.HitObject is RimHit;

            if (!secondHit)
            {
                if (judgedObject.X >= -0.05f && (judgedObject is DrawableHit))
                {
                    // If we're far enough away from the left stage, we should bring outselves in front of it
                    topLevelHitContainer.Add(judgedObject.CreateProxy());
                }

                hitExplosionContainer.Add(new HitExplosion(judgedObject.Judgement, isRim));

                if (judgedObject.HitObject.Kiai)
                    kiaiExplosionContainer.Add(new KiaiHitExplosion(judgedObject.Judgement, isRim));
            }
            else
                hitExplosionContainer.Children.FirstOrDefault(e => e.Judgement == judgedObject.Judgement)?.VisualiseSecondHit();
        }

        private void transformVisibleTimeRangeTo(double newTimeRange, double duration = 0, EasingTypes easing = EasingTypes.None)
        {
            TransformTo(() => visibleTimeRange.Value, newTimeRange, duration, easing, new TransformTimeSpan());
        }

        /// <summary>
        /// This is a very special type of container. It serves a similar purpose to <see cref="FillMode.Fit"/>, however unlike <see cref="FillMode.Fit"/>,
        /// this will only adjust the scale relative to the height of its parent and will maintain the original width relative to its parent.
        ///
        /// <para>
        /// By adjusting the scale relative to the height of its parent, the aspect ratio of this container's children is maintained, however this is undesirable
        /// in the case where the hit object container should not have its width adjusted by scale. To counteract this, another container is nested inside this
        /// container which takes care of reversing the width adjustment while appearing transparent to the user.
        /// </para>
        /// </summary>
        private class ScaleFixContainer : Container
        {
            protected override Container<Drawable> Content => widthAdjustmentContainer;
            private readonly WidthAdjustmentContainer widthAdjustmentContainer;

            /// <summary>
            /// We only want to apply DrawScale in the Y-axis to preserve aspect ratio and <see cref="TaikoPlayfield"/> doesn't care about having its width adjusted.
            /// </summary>
            protected override Vector2 DrawScale => Scale * RelativeToAbsoluteFactor.Y / DrawHeight;

            public ScaleFixContainer()
            {
                AddInternal(widthAdjustmentContainer = new WidthAdjustmentContainer { ParentDrawScaleReference = () => DrawScale.X });
            }

            /// <summary>
            /// The container type that reverses the <see cref="Drawable.DrawScale"/> width adjustment.
            /// </summary>
            private class WidthAdjustmentContainer : Container
            {
                /// <summary>
                /// This container needs to know its parent's <see cref="Drawable.DrawScale"/> so it can reverse the width adjustment caused by <see cref="Drawable.DrawScale"/>.
                /// </summary>
                public Func<float> ParentDrawScaleReference;

                public WidthAdjustmentContainer()
                {
                    // This container doesn't care about height, it should always fill its parent
                    RelativeSizeAxes = Axes.Y;
                }

                protected override void Update()
                {
                    base.Update();

                    // Reverse the DrawScale adjustment
                    Width = Parent.DrawSize.X / ParentDrawScaleReference();
                }
            }
        }

        private class TransformTimeSpan : Transform<double>
        {
            public override double CurrentValue
            {
                get
                {
                    double time = Time?.Current ?? 0;
                    if (time < StartTime) return StartValue;
                    if (time >= EndTime) return EndValue;

                    return Interpolation.ValueAt(time, StartValue, EndValue, StartTime, EndTime, Easing);
                }
            }

            public override void Apply(Drawable d)
            {
                base.Apply(d);

                var p = (TaikoPlayfield)d;
                p.visibleTimeRange.Value = (float)CurrentValue;
            }
        }
    }
}