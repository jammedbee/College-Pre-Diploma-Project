using System.Data.SqlClient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class MainLayoutPage : Page
    {
        private SqlConnection MainConnection;
        private User CurrentUser;

        public MainLayoutPage()
        {
            this.InitializeComponent();
            rootFrame.Navigated += ChangeBackButtonState;
        }

        private void ChangeBackButtonState(object sender, NavigationEventArgs e)
        {
            BackButton.IsEnabled = rootFrame.CanGoBack ? true : false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as UserParameters;
            MainConnection = parameters.Connection;
            CurrentUser = parameters.CurrentUser;
            if (CurrentUser.PostId != 1)
            {
                WorkersItem.Visibility = Visibility.Collapsed;
            }
            UserPhoto.Source = CurrentUser.Photo;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as ListBox).SelectedIndex)
            {
                case 1:
                    rootFrame.Navigate(typeof(FilmsOverviewPage), MainConnection);
                    break;
                case 2:
                    rootFrame.Navigate(typeof(HallsOverviewPage), MainConnection);
                    break;
                case 3:
                    rootFrame.Navigate(typeof(WorkersOverviewPage), MainConnection);
                    break;
                default:
                    break;
            }
        }

        private void MenuItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Menu.IsPaneOpen = !Menu.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void ListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}