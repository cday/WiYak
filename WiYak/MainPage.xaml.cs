using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace WiYak
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage ()
        {
            InitializeComponent();
        }

        #region Lobby

        private void LobbyBox_Key (object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                App.State.SendMessage(LobbyBox.Text);
                LobbyBox.Text = "";
                this.Focus();
            }
        }

        private void LobbyList_Selection(object sender, SelectionChangedEventArgs e)
        {
            LobbyList.SelectedItem = null;
        }

        private void LobbyList_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            item.Width = LobbyList.ActualWidth;
            LobbyList.ScrollIntoView(App.State.Lobby.Messages.Last());
        }

        private void LobbyList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LobbyList.ItemsSource = null;

            if (App.State != null)
            {
                LobbyList.ItemsSource = App.State.Lobby.Messages;
            }
        }

        private void LobbyList_ItemLoaded(object sender, RoutedEventArgs e)
        {
            if (LobbyList.ActualWidth > 100)
            {
                FrameworkElement item = sender as FrameworkElement;
                item.Width = LobbyList.ActualWidth - 100;
                LobbyList.ScrollIntoView(App.State.Lobby.Messages.Last());
            }
        }

        #endregion

        #region Page Wide

        private void Page_OrientationChanged (object sender, OrientationChangedEventArgs e)
        {
            //App.State.Lobby.Messages.Add(new Message(new User("bob", null), "you flipped", false));
            //LobbyList.ScrollIntoView(App.State.Lobby.Messages.Last());
        }

        private void Page_Loaded (object sender, RoutedEventArgs e)
        {
            LobbyList.ItemsSource = App.State.Lobby.Messages;
            ThreadList.ItemsSource = App.State.Threads;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string threads = string.Empty;

            if (NavigationContext.QueryString.TryGetValue("threads", out threads))
            {
                Pivot.SelectedIndex = 1;
                NavigationService.RemoveBackEntry();
            }

            App.CurrentThread = App.State.Lobby;

            base.OnNavigatedTo(e);
        }

        #endregion

        #region Threads

        private void ThreadList_Delete(object sender, RoutedEventArgs e)
        {
            //find which ListBoxItem contains the MenuItem that was clicked
            ListBoxItem list_item = ThreadList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;

            //get the Thread object contained by the ListBoxItem
            Thread thread_item = list_item.Content as Thread;

            //remove Thread item from Threads list
            App.State.DeleteThread(thread_item);

            //remove the BackEntry for the ContextMenu associated with the now deleted Thread
            NavigationService.RemoveBackEntry();
        }

        private void ThreadList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //see above comments for gist of what is happening here
            ListBoxItem list_item = ThreadList.ItemContainerGenerator.ContainerFromItem((sender as Grid).DataContext) as ListBoxItem;

            Thread thread_item = list_item.Content as Thread;

            int item_index = App.State.Threads.IndexOf(thread_item);

            NavigationService.Navigate(new Uri("/ThreadPage.xaml?index=" + item_index, UriKind.Relative));
        }

        private void ThreadList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThreadList.SelectedItem = null;
        }

        private void ThreadList_Enter(object sender, MouseEventArgs e)
        {
            Grid item = sender as Grid;
            PlaneProjection clicked = new PlaneProjection();

            clicked.LocalOffsetX = 2;
            clicked.LocalOffsetY = -3;

            item.Projection = clicked;
        }

        private void ThreadList_Leave(object sender, MouseEventArgs e)
        {
            Grid item = sender as Grid;
            item.Projection = null;
        }

        private void Pivot_Changed(object sender, SelectionChangedEventArgs e)
        {
            ThreadList.ItemsSource = null;
            ThreadList.ItemsSource = App.State.Threads;
        }

        #endregion
    }
}