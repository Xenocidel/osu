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

namespace osu.Desktop.VisualTests.Tests
{
    internal class TestCaseMultiReplayPlayer : TestCase
    {
        public override void Reset()
        {
            base.Reset();

            Add(new MultiReplayPlayer());

            Add(new StarCounter
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                MaxCount = 10,
                Count = 5
            });
        }

        internal class MultiReplayPlayer : OsuScreen
        {
            protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

            private readonly Sprite backgroundSprite;

            public MultiReplayPlayer()
            {
                Children = new[]
                {
                    backgroundSprite = new Sprite
                    {
                        FillMode = FillMode.Fill
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                backgroundSprite.Texture = textures.Get(@"Backgrounds/Tournament/background");
            }
        }

        internal class StarCounter : Container, IHasAccentColour
        {
            private readonly Container<Star> starContainer;
            private readonly Container<ProxyDrawable> topMostStars;

            private Dictionary<Star, ProxyDrawable> proxyMap = new Dictionary<Star, ProxyDrawable>();

            public StarCounter()
            {
                AutoSizeAxes = Axes.Both;

                Children = new Drawable[]
                {
                    starContainer = new Container<Star>
                    {
                        AutoSizeAxes = Axes.Both
                    },
                    topMostStars = new Container<ProxyDrawable>
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                };
            }

            private Color4 accentColour;
            public Color4 AccentColour
            {
                get { return accentColour; }
                set
                {
                    if (accentColour == value)
                        return;
                    accentColour = value;

                    starContainer.Children.ForEach(s => s.AccentColour = accentColour);
                }
            }

            private int maxCount;
            public int MaxCount
            {
                get { return maxCount; }
                set
                {
                    if (maxCount == value)
                        return;

                    if (value > maxCount)
                    {
                        // MaxCount has increased, add more stars
                        for (int i = maxCount; i < value; i++)
                        {
                            var newStar = new Star
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                AccentColour = AccentColour,
                                X = Star.STAR_SIZE * i,
                            };

                            newStar.AnimationStart = () =>
                            {
                                // Remove any existing proxy
                                ProxyDrawable existing;
                                if (proxyMap.TryGetValue(newStar, out existing))
                                    topMostStars.Remove(existing);
                                else
                                    existing = proxyMap[newStar] = newStar.CreateProxy();

                                topMostStars.Add(existing);
                            };

                            starContainer.Add(newStar);
                        }
                    }
                    else
                    {
                        // MaxCount has decreased, remove some stars
                        for (int i = maxCount; i > value; i--)
                            starContainer.Remove(starContainer.Children.Last());
                    }

                    maxCount = value;
                }
            }

            private int count;
            public int Count
            {
                get { return count; }
                set
                {
                    if (count == value)
                        return;

                    value = MathHelper.Clamp(value, 0, MaxCount);

                    if (value > count)
                    {
                        // Count has increased, fill stars
                        starContainer.Children.Take(value).ForEach(s => s.IsFilled = true);
                    }
                    else
                    {
                        // Count has decreased, unfill stars
                        starContainer.Children.Skip(value).Take(count - value).ForEach(s => s.IsFilled = false);
                    }

                    count = value;
                }
            }

            protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
            {
                if (args.Button == MouseButton.Left)
                {
                    Count++;
                    return true;
                }

                if (args.Button == MouseButton.Right)
                {
                    Count--;
                    return true;
                }

                return false;
            }

            private class Star : Container, IHasAccentColour
            {
                public const float STAR_SIZE = 40f;
                private const float glow_sigma = 10f;

                public Action AnimationStart;
                public Action AnimationEnd;

                private readonly Container filledStarContainer;
                private readonly TextAwesome baseStar;
                private readonly BufferedContainer filledStarGlow;

                public Star()
                {
                    AutoSizeAxes = Axes.Both;

                    Children = new Drawable[]
                    {
                        baseStar = new TextAwesome
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = FontAwesome.fa_star_o,
                            TextSize = STAR_SIZE
                        },
                        filledStarContainer = new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.Both,
                            Scale = new Vector2(0.001f), // Todo: Fix broken size (NaN?)
                            Children = new Drawable[]
                            {
                                filledStarGlow = new BufferedContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    AutoSizeAxes = Axes.Both,
                                    Scale = new Vector2(1.3f),
                                    BlurSigma = new Vector2(glow_sigma),
                                    CacheDrawnFrameBuffer = true,
                                    BlendingMode = BlendingMode.Additive,
                                    Alpha = 0,
                                    Children = new []
                                    {
                                        new TextAwesome
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.fa_star,
                                            TextSize = STAR_SIZE,
                                            Shadow = false
                                        },
                                    }
                                },
                                new TextAwesome
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = FontAwesome.fa_star,
                                    TextSize = STAR_SIZE,
                                }
                            }
                        }
                    };
                }

                private Color4 accentColour;
                public Color4 AccentColour
                {
                    get { return accentColour; }
                    set
                    {
                        if (accentColour == value)
                            return;
                        accentColour = value;

                        baseStar.Colour = accentColour;
                    }
                }

                private bool isFilled;
                public bool IsFilled
                {
                    get { return isFilled; }
                    set
                    {
                        if (isFilled == value)
                            return;
                        isFilled = value;

                        DelayReset();
                        Flush(true);

                        if (isFilled)
                        {
                            AnimationStart?.Invoke();

                            filledStarContainer.ScaleTo(2f, 300, EasingTypes.OutBack);

                            Delay(200, true);
                            filledStarGlow.FadeIn(350, EasingTypes.OutSine);
                            filledStarGlow.RotateTo(filledStarGlow.Rotation + 1080, 12000);

                            Delay(3000, true);
                            filledStarGlow.FadeOut(400, EasingTypes.OutCubic);
                            filledStarContainer.ScaleTo(1, 300, EasingTypes.InBack);
                        }
                        else
                            filledStarContainer.ScaleTo(0, 200);
                    }
                }
            }
        }
    }
}
