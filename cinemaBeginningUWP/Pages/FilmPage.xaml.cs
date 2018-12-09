using cinemaBeginningUWP.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class FilmPage : Page
    {
        private Film Film;
        private SqlConnection MainConnection;
        private Byte[] ByteImage { get; set; }

        public FilmPage()
        {
            this.InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Film == null)
                await SaveNewFilm();
            else
                await SaveChanges();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = true;
            EditButton.IsEnabled = false;
            ActivateAll();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => (Parent as Frame).GoBack();

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = "Вы уверены, что хотите удалить данный фильм? \nЭто действие не может быть отменено";
            confirmDialog.PrimaryButtonText = "Да";
            confirmDialog.SecondaryButtonText = "Нет";
            confirmDialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                using (SqlCommand sqlCommand = new SqlCommand("sp_deleteFilm", MainConnection))
                {
                    if (MainConnection.State == System.Data.ConnectionState.Closed)
                        await MainConnection.OpenAsync();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@id", Film.Id);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                confirmDialog.Hide();
                (Parent as Frame).Navigate(typeof(FilmsOverviewPage), MainConnection);
            };
            confirmDialog.SecondaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                dialog.Hide();
            };
            await confirmDialog.ShowAsync();
        }

        private void LoadFilm()
        {
            if (Film != null)
            {
                NameTextBox.Text = Film.Name;
                DurationTextBox.Text = Film.Duration.ToString();
                DescriptionTextBox.Text = Film.Description;
                DatePicker.Date = Film.ReleaseDate;
                Poster.Source = Film.Poster;
            }
        }

        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            fileOpenPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile selectedFile = await fileOpenPicker.PickSingleFileAsync();
            if (selectedFile != null)
            {
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await selectedFile.OpenAsync(Windows.Storage.FileAccessMode.Read));
                WriteableBitmap writeableBitmap = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                await writeableBitmap.SetSourceAsync(await selectedFile.OpenAsync(Windows.Storage.FileAccessMode.Read));
                ByteImage = await new ImageConverter().ConvertRandomAccessStreamToByteArray(await selectedFile.OpenAsync(Windows.Storage.FileAccessMode.Read));
                Poster.Source = writeableBitmap;
            }
        }

        private async Task SaveNewFilm()
        {
            if (MainConnection.State == System.Data.ConnectionState.Closed)
                await MainConnection.OpenAsync();
            using (SqlCommand command = new SqlCommand("sp_addFilm", MainConnection))
            {
                string releaseDate = DatePicker.Date.Year.ToString() + '-' +
                    DatePicker.Date.Month.ToString() + '-' +
                    DatePicker.Date.Day.ToString();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@name", NameTextBox.Text);
                command.Parameters.AddWithValue("@duration", Convert.ToInt32(DurationTextBox.Text));
                command.Parameters.AddWithValue("@description", DescriptionTextBox.Text);
                command.Parameters.AddWithValue("@releaseDate", releaseDate);
                command.Parameters.AddWithValue("@poster", ByteImage);

                await command.ExecuteNonQueryAsync();
            }

            MainConnection.Close();
            (Parent as Frame).Navigate(typeof(FilmsOverviewPage), MainConnection);
        }

        private async Task SaveChanges()
        {
            if (MainConnection.State == System.Data.ConnectionState.Closed)
                await MainConnection.OpenAsync();
            using (SqlCommand command = new SqlCommand("sp_editFilm", MainConnection))
            {
                string releaseDate = DatePicker.Date.Year.ToString() + '-' +
                    DatePicker.Date.Month.ToString() + '-' +
                    DatePicker.Date.Day.ToString();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", Film.Id);
                command.Parameters.AddWithValue("@newName", NameTextBox.Text);
                command.Parameters.AddWithValue("@newDuration", Convert.ToInt32(DurationTextBox.Text));
                command.Parameters.AddWithValue("@newDescription", DescriptionTextBox.Text);
                command.Parameters.AddWithValue("@newReleaseDate", releaseDate);
                command.Parameters.AddWithValue("@newPoster", ByteImage);

                await command.ExecuteNonQueryAsync();
            }

            MainConnection.Close();
            (Parent as Frame).Navigate(typeof(FilmsOverviewPage), MainConnection);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            FilmParameters parameters = e.Parameter as FilmParameters;
            Film = parameters.Film;
            EditButton.IsEnabled = (Film == null) ? false : true;
            OptionsButton.IsEnabled = (Film == null) ? false : true;
            SaveButton.IsEnabled = (Film == null) ? true : false;
            MainConnection = parameters.Connection;
            if (Film != null)
            {
                DeactivateAll();
                await GetImage();
            }
            LoadFilm();
            base.OnNavigatedTo(e);
        }

        private void ActivateAll()
        {
            UploadImageButton.IsEnabled = true;
            NameTextBox.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;
            DatePicker.IsEnabled = true;
            DurationTextBox.IsEnabled = true;
        }

        private void DeactivateAll()
        {
            UploadImageButton.IsEnabled = false;
            NameTextBox.IsEnabled = false;
            DescriptionTextBox.IsEnabled = false;
            DatePicker.IsEnabled = false;
            DurationTextBox.IsEnabled = false;
        }

        private async Task GetImage()
        {
            using (SqlConnection connection = new SqlConnection(MainConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand($"SELECT poster FROM films WHERE films.id = {Film.Id}", connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader["poster"] != DBNull.Value)
                        {
                            ByteImage = (byte[])reader["poster"];
                        }
                    }
                    connection.Close();
                }
            }
        }
    }
}
