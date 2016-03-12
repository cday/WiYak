using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;

namespace WiYak
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static Thread CurrentThread { get; set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Storage.LoadSettings();
            App.State.Me.Color_ = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Storage.LoadState();

            Deployment.Current.Dispatcher.BeginInvoke(
            () =>
            {
                App.State.Reopen();

                if (App.State.Settings.Joined_)
                    App.State.Join();
            });
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            App.State.DisconnectClean();
            Storage.SaveState();
            App.State.Connection.Close();
            App.State.Connection = null;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            App.State.DisconnectClean();
            Storage.SaveSettings();
            App.State.Connection.Close();
            App.State.Connection = null;
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        private static Chat _state = null;
        public static Chat State
        {
            get
            {
                if (_state == null)
                    _state = new Chat();

                return _state;
            }
            set
            {
                _state = value;
            }
        }

        #region App Bar

        public void Join_Click(object sender, EventArgs e)
        {
            App.State.Join();
        }

        public void Disconnect_Click(object sender, EventArgs e)
        {
            App.State.Disconnect();
        }

        public void Add_Click(object sender, EventArgs e)
        {
            if (App.State.Settings.PrivateMessages_)
            {
                RootFrame.Navigate(new Uri("/CreatePage.xaml", UriKind.Relative));
            }
            else
            {
                Display.MessageBoxShow("threads are disabled. you can enable them in settings.", "Thread", false);
            }
        }

        private void ClearMsg_Click(object sender, EventArgs e)
        {
            string current_page = RootFrame.CurrentSource.OriginalString;

            if (current_page.Contains("MainPage.xaml"))
            {
                App.State.Lobby.Messages.Clear();
            }
            else
            {
                Regex r1 = new Regex("index=([0-9]+)");
                Match match = r1.Match(current_page);

                if (match.Success)
                {
                    int index;

                    if (int.TryParse(match.Groups[1].Value, out index))
                    {
                        App.State.Threads[index].Messages.Clear();
                    }
                }
            }

        }

        private void ShareMsg_Click(object sender, EventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Body = CurrentThread.ToPlainText();
            email.Show();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            RootFrame.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        #endregion
    }
}