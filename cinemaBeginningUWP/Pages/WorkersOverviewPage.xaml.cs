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
    public sealed partial class WorkersOverviewPage : Page
    {
        private SqlConnection MainConnection;
        private ObservableCollection<Worker> Workers;

        public WorkersOverviewPage()
        {
            this.InitializeComponent();
            Workers = new ObservableCollection<Worker>();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainConnection = e.Parameter as SqlConnection;
            await GetWorkersFromServerAsync();  
        }

        private async Task GetWorkersFromServerAsync()
        {
            if (Workers.Count > 0)
            {
                Workers = new ObservableCollection<Worker>();
            }
            progress.IsActive = true;
            if (MainConnection.State == System.Data.ConnectionState.Closed)
            {
                await MainConnection.OpenAsync();
            }

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = "SELECT * FROM workers";
                sqlCommand.Connection = MainConnection;
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            BitmapImage bitmapImage;
                            if (reader.IsDBNull(10))
                            {
                                bitmapImage = new BitmapImage();
                                bitmapImage = null;
                            }
                            else
                                bitmapImage = await new ImageConverter().ByteArrayToBitmapImageAsync((byte[])reader["photo"]);
                            Workers.Add(new Worker(
                                reader.GetInt32(0),
                                reader.GetString(2),
                                reader.GetString(3),
                                new DateTime(reader.GetDateTime(4).Year, reader.GetDateTime(4).Month, reader.GetDateTime(4).Day),
                                reader.GetInt32(5),
                                reader.GetString(6),
                                reader.GetString(7),
                                reader.GetString(8),
                                reader.GetInt32(9),
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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(NewLoginPage), MainConnection);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(WorkerPage),
                new WorkerParameters
                {
                    Connection = MainConnection,
                    Id = (e.ClickedItem as Worker).Id,
                    Password = "",
                    Worker = e.ClickedItem as Worker
                });
        }

        private async void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await GetWorkersFromServerAsync();
        }
    }
}