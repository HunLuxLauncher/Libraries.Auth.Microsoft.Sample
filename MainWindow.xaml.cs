using hu.hunluxlauncher.libraries.auth.microsoft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace Libraries.Auth.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AuthenticationSettings AuthSettings { get; }

        readonly hu.hunluxlauncher.libraries.auth.microsoft.Auth AuthInfo;
        public MainWindow()
        {
            InitializeComponent();
            AuthSettings = new AuthenticationSettings
            {
                UserAgent = Title.Replace(" - ", "/")
            };
            AuthInfo = new hu.hunluxlauncher.libraries.auth.microsoft.Auth(AuthSettings);
            Uri uri;
            bool alreadyLoggedIn = false;
            if (!alreadyLoggedIn) uri = AuthInfo.LoginInit();
            else uri = AuthInfo.LogoutInit();

            viewer.Navigate(uri);
            viewer.Navigating += Viewer_Navigating;
            BackgroundWorker backgroundWorker = new(); Thread thread = null;
            List<string> errors = new();
            backgroundWorker.DoWork += (sender, e) =>
            {
                thread = new Thread(() =>
                {
                    while (AuthorizationToken == null) ; // waiting for authorization to finish
                    this.Dispatcher.Invoke(() => {
                        viewer.Visibility = Visibility.Collapsed;
                        preview.Visibility = Visibility.Visible;
                    });
                    XboxLiveAuthenticate = AuthInfo.XboxLiveAuthenticate(AuthorizationToken.AccessToken);
                    XSTSAuthenticate = AuthInfo.XSTSAuthenticate(XboxLiveAuthenticate.Token);
                    MinecraftAuthenticate = AuthInfo.MinecraftAuthenticate(XboxLiveAuthenticate.DisplayClaims.XuiClaims[0].UserHash, XSTSAuthenticate.Token);
                    GameOwnership = AuthInfo.CheckingGameOwnership(MinecraftAuthenticate.AccessToken);
                    try
                    {
                        ProfileInfo = AuthInfo.GetProfile(MinecraftAuthenticate.AccessToken);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        errors.Add("The Microsoft account, that you're trying to login with, does not own the game.");
                    }

                    this.Dispatcher.Invoke(() => {
                        // authentication process finished, do something after it...
                        if(errors.Count == 0)
                        {
                            if(ProfileInfo != null)
                            {
                                preview.Text = $"Profile info:\r\n{JsonSerializer.Serialize(ProfileInfo)}";
                            }
                            else
                            {
                                errors.Add("The Microsoft account, that you're trying to login with, does not own the game.");
                            }
                        }
                        preview.Text = string.Join("\r\n", errors);
                        viewer.Navigate(AuthInfo.LogoutInit());
                    });
                });
                if (!thread.IsAlive) thread.Start();
            };
            backgroundWorker.RunWorkerAsync();
        }

        public hu.hunluxlauncher.libraries.auth.microsoft.account.TokenResponse AuthorizationToken { get; private set; } = null;
        public hu.hunluxlauncher.libraries.auth.microsoft.xbox.TokenResponse XboxLiveAuthenticate { get; private set; } = null;
        public hu.hunluxlauncher.libraries.auth.microsoft.xbox.TokenResponse XSTSAuthenticate { get; private set; } = null;
        public hu.hunluxlauncher.libraries.auth.microsoft.minecraft.Authenticate MinecraftAuthenticate { get; private set; } = null;
        public hu.hunluxlauncher.libraries.auth.microsoft.minecraft.GameOwnership GameOwnership { get; private set; } = null;
        public hu.hunluxlauncher.libraries.auth.microsoft.minecraft.ProfileInfo ProfileInfo { get; private set; } = null;

        private void Viewer_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var data = AuthInfo.CatchAuthorizationToken(e.Uri);
            if (data != null)
            {
                AuthorizationToken = data;
            }
        }
    }
}
