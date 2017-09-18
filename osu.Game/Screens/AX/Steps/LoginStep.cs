// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;


namespace osu.Game.Screens.AX.Steps
{
    public class LoginStep : Step
    {
        private readonly OsuTextBox fullname;
        private readonly OsuTextBox email;
        private readonly OsuButton submitButton;
        private readonly Container progressContainer;
        private readonly LoadingAnimation loadingAnimation;
        private readonly OsuSpriteText errorText;

        private APIAccess api;
        private UserInputManager inputManager;

        private bool submitQueued;

        private Drawable[] successDialogue;

        public LoginStep()
        {
            Add(new Drawable[]
            {
                new OsuSpriteText
                {
                    Text = "SIGNUP",
                    Font = "Exo2.0-Bold"
                },
                fullname = new OsuTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "Full Name",
                    TabbableContentContainer = this,
                    ReleaseFocusOnCommit = false,
                    OnCommit = (t, n) =>
                    {
                        if (!string.IsNullOrEmpty(t.Text))
                            inputManager?.ChangeFocus(email);
                    }
                },
                email = new OsuTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "Email",
                    TabbableContentContainer = this,
                    OnCommit = (t, n) => submit()
                    // TODO: optional, check format includes @ and .
                    // [Ken] Checked email format validity in submit section below

                },
                progressContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 20,
                    Children = new Drawable[]
                    {
                        loadingAnimation = new LoadingAnimation
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Alpha = 0
                        },
                        errorText = new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Alpha = 0,
                            Colour = OsuColour.FromHex("ed1121")
                        }
                    }
                },
                submitButton = new OsuButton
                {
                    RelativeSizeAxes = Axes.X,
                    Text = "Submit",
                    Action = submit
                }
            });
        }


        //[BackgroundDependencyLoader(permitNulls: true)]
        //private void load(APIAccess api, UserInputManager inputManager)
        //{
        //    this.api = api;
        //    this.inputManager = inputManager;

        //    api?.Register(this);
        //}

        //protected override void LoadComplete()
        //{
        //    base.LoadComplete();

        //    inputManager?.ChangeFocus(fullname);
        //}

        //private ScheduledDelegate errorTextDelegate;

        //public void APIStateChanged(APIAccess api, APIState state)
        //{
        //    switch (state)
        //    {
        //        case APIState.Connecting:
        //            loadingAnimation.Show();
        //            break;
        //        default:
        //            loadingAnimation.Hide();
        //            break;
        //    }

        //    switch (state)
        //    {
        //        case APIState.Offline:
        //            if (api?.username == null)
        //                break;

        //            errorTextDelegate?.Cancel();
        //            errorText.Text = "Incorrect username or password.";
        //            errorText.Show();

        //            email.Text = string.Empty;

        //            errorTextDelegate = Scheduler.AddDelayed(() => errorText.Hide(), 5000);

        //            inputManager?.ChangeFocus(email);

        //            break;
        //        case APIState.Failing:
        //            errorTextDelegate?.Cancel();
        //            errorText.Text = "An API error occurred, please poke on-site staff.";
        //            errorText.Show();

        //            errorTextDelegate = Scheduler.AddDelayed(() => errorText.Hide(), 5000);
        //            break;
        //        default:
        //            errorText.Hide();
        //            break;
        //    }
        //}

        //private void performLogin()
        //{
        //    if (string.IsNullOrEmpty(fullname.Text) || string.IsNullOrEmpty(email.Text))
        //        return;

        //    api?.Login(fullname.Text, email.Text);
        //}

        private void submit()
        {
            if (string.IsNullOrEmpty(fullname.Text) || string.IsNullOrEmpty(email.Text))
                return;

            //if (submitQueued)
            //    return;
            //submitQueued = true;

            // TODO: send to csv or something
            // TODO: make it do whatever SubmissionResultStep does, they use arrows in StepContainer and idk what it means
            // [Ken] Just create a txt where submit constantly appends. We can just use Spreadsheat delimiter to make easy columns

            if(validateEmail(email.Text))
            {
                string path = Environment.CurrentDirectory+"/subscribers.csv";
                // Make file if it doesn't exist
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(fullname.Text + "," + email.Text);
                    }
                }
                else
                {
                    // Append to file
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(fullname.Text + "," + email.Text);
                    }
                }

                fullname.Text = "";
                email.Text = "";
                email.PlaceholderText = "Email";

                successSprite();
                System.Threading.Thread success =
                    new System.Threading.Thread(() =>
                    {
                        System.Threading.Thread.Sleep(3000);
                        Remove(successDialogue);
                    });
                success.Start();
            }
            else
            {
                email.Text = "";
                email.PlaceholderText = "Please enter valid email";
                successSprite("Invalid email, please try again");
                System.Threading.Thread invalid =
                    new System.Threading.Thread(() =>
                    {
                        System.Threading.Thread.Sleep(1500);
                        Remove(successDialogue);
                    });
                invalid.Start();
            }

        }

        public override bool Contains(Vector2 screenSpacePos) => true;

        public override bool AcceptsFocus => true;

        protected override bool OnClick(InputState state) => true;

        protected override void OnFocus(InputState state) => Schedule(() => inputManager?.ChangeFocus(string.IsNullOrEmpty(fullname.Text) ? fullname : email));

        // Helper functions
        bool validateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        void successSprite(String text="Success, thank you for subscribing!")
        {
            // try adding timer? idk
            successDialogue = new Drawable[] {
                new OsuSpriteText
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Text = text,
                    TextSize = 36,
                    Y = 30
                }};
            Add(successDialogue);
        }

        void disposeSuccessDialogue()
        {
            Remove(successDialogue);
        }
    }
}
