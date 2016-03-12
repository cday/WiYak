using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace WiYak
{
    public partial class CreatePage : PhoneApplicationPage
    {
        public CreatePage()
        {
            InitializeComponent();
        }

        #region Create Button

        private void CreateButton_Enter(object sender, MouseEventArgs e)
        {
            PlaneProjection clicked = new PlaneProjection();

            clicked.LocalOffsetX = 2;
            clicked.LocalOffsetY = -3;

            CreateButton.Projection = clicked; 
        }

        private void CreateButton_Leave(object sender, MouseEventArgs e)
        {
            CreateButton.Projection = null;
        }

        private void CreateButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            System.Collections.IList temp = UserList.SelectedItems;
            ObservableCollection<User> selected_users = new ObservableCollection<User>();

            foreach (User user in temp)
            {
                selected_users.Add(user);
            }

            App.State.AddThread(selected_users);

            NavigationService.Navigate(new Uri("/MainPage.xaml?threads=true", UriKind.Relative));
        }

        #endregion

        #region Page Wide

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UserList.ItemsSource = App.State.Users;
        }

        #endregion
    }
}