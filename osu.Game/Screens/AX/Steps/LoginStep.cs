﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;

namespace osu.Game.Screens.AX.Steps
{
    public class LoginStep : Step
    {
        private OsuTextBox username;
        private OsuTextBox password;
        private OsuButton loginButton;

        private APIAccess api;
        private UserInputManager inputManager;

        private bool loginRequested;

        public LoginStep()
        {
            Add(new Drawable[]
            {
                new OsuSpriteText
                {
                    Text = "ACCOUNT",
                    Font = "Exo2.0-Bold"
                },
                username = new OsuTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "Username",
                    TabbableContentContainer = this
                },
                password = new OsuPasswordTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "Password",
                    TabbableContentContainer = this,
                    OnCommit = (s, n) => performLogin()
                },
                loginButton = new OsuButton
                {
                    RelativeSizeAxes = Axes.X,
                    Text = "Login",
                    Action = performLogin
                }
            });
        }

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load(APIAccess api, UserInputManager inputManager)
        {
            this.api = api;
            this.inputManager = inputManager;

            inputManager?.ChangeFocus(username);
        }

        private void performLogin()
        {
            if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text))
                return;

            if (loginRequested)
                return;

            api?.Login(username.Text, password.Text);
            loginRequested = true;
        }
    }
}
