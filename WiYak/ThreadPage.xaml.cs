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
    public partial class ThreadPage : PhoneApplicationPage
    {
        private Thread CurrentThread;

        public ThreadPage()
        {
            InitializeComponent();
        }

        #region Message

        private void MessageBox_Key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                App.State.SendMessageTo (this.CurrentThread, MessageBox.Text);
                MessageBox.Text = "";
                this.Focus();
            }
        }

        private void MessageList_Selection(object sender, SelectionChangedEventArgs e)
        {
            MessageList.SelectedItem = null;
        }

        private void MessageList_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            item.Width = MessageList.ActualWidth;
            MessageList.ScrollIntoView(CurrentThread.Messages.Last());

            CurrentThread.UnreadCount = 0;
        }

        private void MessageList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MessageList != null)
            {
                MessageList.ItemsSource = null;

                if (CurrentThread != null)
                {
                    MessageList.ItemsSource = CurrentThread.Messages;
                }
            }
        }

        private void MessageList_ItemLoaded(object sender, RoutedEventArgs e)
        {
            if (MessageList.ActualWidth > 100)
            {
                FrameworkElement item = sender as FrameworkElement;
                item.Width = MessageList.ActualWidth - 100;
                MessageList.ScrollIntoView(CurrentThread.Messages.Last());
            }
        }

        #endregion

        #region Page Wide

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MessageList.ItemsSource = CurrentThread.Messages;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string temp = string.Empty;

            if (NavigationContext.QueryString.TryGetValue("index", out temp))
            {
                int index;

                if (int.TryParse(temp, out index))
                {
                    CurrentThread = App.State.Threads[index];
                    App.CurrentThread = App.State.Threads[index];
                    TitleBlock.Text = CurrentThread.ToString();
                }
                else
                {
                    //TODO error find a better way to handle
                    NavigationService.GoBack();
                }
            }
            else
            {
                //error find a better way to handle
                NavigationService.GoBack();
            }

            base.OnNavigatedTo(e);
        }

        private void Page_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //CurrentThread.Messages.Add(new Message(new User("world", null), "you flipped", false));
            //MessageList.ScrollIntoView(CurrentThread.Messages.Last());
        }

        #endregion
    }
}