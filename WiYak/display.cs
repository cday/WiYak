using System;
using System.Windows;

namespace WiYak
{
    public class Display
    {
        public static bool MessageBoxShow(string text, string caption, bool type)
        {
            bool result = true;
            MessageBoxResult temp_result = MessageBoxResult.None;

            MessageBoxButton button = type ? MessageBoxButton.OKCancel : MessageBoxButton.OK;

            // Am I already on the UI thread?
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                temp_result = MessageBox.Show(text, caption, button);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    temp_result = MessageBox.Show(text, caption, button);
                });
            }

            if (temp_result == MessageBoxResult.Cancel)
                result = false;

            return result;
        }
    }
}
