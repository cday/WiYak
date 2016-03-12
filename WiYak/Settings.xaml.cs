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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace WiYak
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        #region Back Button

        private void BackButton_Enter(object sender, MouseEventArgs e)
        {
            PlaneProjection clicked = new PlaneProjection();

            clicked.LocalOffsetX = 2;
            clicked.LocalOffsetY = -3;

            BackButton.Projection = clicked;
        }

        private void BackButton_Leave(object sender, MouseEventArgs e)
        {
            BackButton.Projection = null;
        }

        private void BackButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.State.ChangeName(NameBox.Text);
            NavigationService.GoBack();
        }

        #endregion

        #region Page Wide

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadToggle.IsChecked = App.State.Settings.PrivateMessages_;
            NotificationToggle.IsChecked = App.State.Settings.LobbyMessages_;
            NameBox.Text = App.State.Me.Name;
        }

        #endregion

        #region Thread Toggle

        private void Threads_Checked(object sender, RoutedEventArgs e)
        {
            App.State.Settings.PrivateMessages_ = true;
        }

        private void Threads_Unchecked(object sender, RoutedEventArgs e)
        {
            App.State.Settings.PrivateMessages_ = false;
        }

        #endregion

        #region Notification Toggle

        private void Notification_Checked(object sender, RoutedEventArgs e)
        {
            App.State.Settings.LobbyMessages_ = true;
        }

        private void Notification_Unchecked(object sender, RoutedEventArgs e)
        {
            App.State.Settings.LobbyMessages_ = false;
        }

        #endregion

        #region Name Box

        private void NameBox_Key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                App.State.ChangeName(NameBox.Text);
                NavigationService.GoBack();
            }
        }

        #endregion
    }
}