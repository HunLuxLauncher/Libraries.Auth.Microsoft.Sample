using System;
using System.Windows;
using hu.hunluxlauncher.libraries.auth.microsoft;

namespace Libraries.Auth.Microsoft.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MSAuth = new MicrosoftAuthentication(AuthSettings);
        }

        public static MicrosoftAuthentication MSAuth { get; internal set; }
        public static AuthenticationSettings AuthSettings { get; } = new AuthenticationSettings()
        {
            UserAgent = "HunLux Launcher Authentication Test Environment/20w53a",
            ClientId = "00000000-0000-0000-0000-000000000000",
            BrowserRedirectSuccess = new Uri("https://auth.hunluxlauncher.hu/microsoft/success"),
            BrowserRedirectError = new Uri("https://auth.hunluxlauncher.hu/microsoft/error")
        };
    }
}
