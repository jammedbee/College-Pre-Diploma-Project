using cinemaBeginningUWP.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class FilmsOverviewPage : Page
    {
        private SqlConnection MainConnection;
        private ObservableCollection<Film> Films;

        public FilmsOverviewPage()
        {
            this.InitializeComponent();
            Films = new ObservableCollection<Film>();
        }

        private async void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await GetFilmsFromServerAsync();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(FilmPage), new FilmParameters {Connection = MainConnection, Film = null, Id = -1});
        }

        private void FilmsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(FilmPage),
                new FilmParameters
                {
                    Connection = MainConnection,
                    Id = (e.ClickedItem as Film).Id,
                    Film = e.ClickedItem as Film
                });
        }

        private async Task GetFilmsFromServerAsync()
        {
            if (Films.Count > 0)
            {
                Films = new ObservableCollection<Film>();
            }
            progress.IsActive = true;
            if (MainConnection.State == System.Data.ConnectionState.Closed)
            {
                await MainConnection.OpenAsync();
            }

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = "SELECT * FROM films";
                sqlCommand.Connection = MainConnection;
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            BitmapImage bitmapImage;
                            if (reader.IsDBNull(5))
                            {
                                bitmapImage = new BitmapImage();
                                bitmapImage = null;
                            }
                            else
                                bitmapImage = await new ImageConverter().ByteArrayToBitmapImageAsync((byte[])reader["poster"]);
                            Films.Add(new Film(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2),
                                reader.GetString(3),
                                new DateTime(reader.GetDateTime(4).Year, reader.GetDateTime(4).Month, reader.GetDateTime(4).Day),
                                bitmapImage));
                        }
                    }
                    else
                    {
                        await new Windows.UI.Popups.MessageDialog("Не было найдено ни одной записи", "Ошибка").ShowAsync();
                    }
                }
            }

            progress.IsActive = false;
            MainConnection.Close();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainConnection = e.Parameter as SqlConnection;
            await GetFilmsFromServerAsync();
            base.OnNavigatedTo(e);
        }
    }
}
