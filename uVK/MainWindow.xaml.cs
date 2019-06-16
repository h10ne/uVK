using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using VkNet.Model.RequestParams;
using WMPLib;
using VkNet.Model;
using VkNet.Enums.Filters;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.Windows.Threading;
using System.Net;
namespace uVK
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
        }

        

        //private void DownloadFriendList()
        //{
        //    var friends = api.Friends.Get(new FriendsGetParams
        //    {
        //        Fields = ProfileFields.All
        //    });
        //    foreach (var friend in friends)
        //        this.FrendList.Items.Add($"{friend.FirstName} {friend.LastName}");
        //}




        private void AppWindow_Deactivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            (DataContext as WindowViewModel).DimmableOverlayVisible = true;
        }

        private void AppWindow_Activated(object sender, EventArgs e)
        {
            // Hide overlay if we are focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = false;
        }

        //private void OfflineCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (OnlineCheckBox.IsChecked == true)
        //    {
        //        api.Account.SetOnline(false);
        //        OnlineTimer.Start();
        //        OnlineSwitch.Text = "Вы будете в сети, \nпока запущено приложение.";
        //    }
        //    else
        //    {
        //        OnlineTimer.Stop();
        //        api.Account.SetOffline();
        //        OnlineSwitch.Text = "Вы не будете отображаться в сети.";
        //    }
        //}
    }
}
