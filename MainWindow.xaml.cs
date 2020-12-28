using hu.hunluxlauncher.libraries.auth.microsoft;
using System;
using System.Text.Json;
using System.Windows;

namespace Libraries.Auth.Microsoft.Sample
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Title = App.AuthSettings.UserAgent.Replace("/", " - ");
        }

        /// <summary>
        /// Call AcquireToken - to acquire a token requiring user to sign-in
        /// </summary>
        private async void CallGraphButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultText.Text = "Authentication process started...";
                TokenInfoText.Text = "";
                var msacc = await App.MSAuth.SignIn();

                var xbaut = new XboxAuthentication(App.AuthSettings, msacc);
                var mcaut = new MinecraftAuthentication(App.AuthSettings, xbaut);
                ResultText.Text = "Authentication finished!";
                TokenInfoText.Text += $"XBL Token:\r\n{xbaut.XboxLive.Token}\r\n\r\n";
                TokenInfoText.Text += $"XSTS token:\r\n{xbaut.XboxSecurityTokenService.Token}\r\n\r\n";
                TokenInfoText.Text += $"Minecraft:\r\n{JsonSerializer.Serialize(mcaut.Minecraft, new JsonSerializerOptions { WriteIndented = true })}\r\n\r\n";
                CallGraphButton.IsEnabled = false;
                SignOutButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Authentication falied!";
                TokenInfoText.Text = $"{ex}";
            }
        }
        /// <summary>
        /// Sign out the current user
        /// </summary>
        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(await App.MSAuth.SignOut())
                {
                    ResultText.Text = "User has signed-out";
                    TokenInfoText.Text = $"";
                    CallGraphButton.IsEnabled = true;
                    SignOutButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                TokenInfoText.Text = $"{ex}";
            }
        }
    }
}
