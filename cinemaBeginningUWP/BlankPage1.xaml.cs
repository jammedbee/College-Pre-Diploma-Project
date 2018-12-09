using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace cinemaBeginningUWP
{
    public sealed partial class BlankPage1 : Page
    {
        public ObservableCollection<Film> Films;

        public BlankPage1()
        {
            this.InitializeComponent();
            Films = new ObservableCollection<Film>();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var film = (Film)e.ClickedItem;
        }

        private void Refresh()
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    Films.Add(new Film("Film #" + i.ToString(), "Description"));
            //}
        }

        private void ref_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
