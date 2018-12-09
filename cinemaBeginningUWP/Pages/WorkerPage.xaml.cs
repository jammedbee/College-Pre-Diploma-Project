using cinemaBeginningUWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class WorkerPage : Page
    {
        private Worker Worker;
        private SqlConnection MainConnection;
        private String Password { get; set; }
        private String Login { get; set; }
        private Byte[] ByteImage { get; set; }
        private ObservableCollection<Post> Posts;

        public WorkerPage()
        {
            this.InitializeComponent();
            Posts = new ObservableCollection<Post>();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Worker == null)
                await SaveNewWorker();
            else
                await SaveChanges();
        }

        private async Task SaveNewWorker()
        {
            if (MainConnection.State == System.Data.ConnectionState.Closed)
                await MainConnection.OpenAsync();
            using (SqlCommand command = new SqlCommand("sp_newLogin", MainConnection))
            {
                string birthDate = DatePicker.Date.Year.ToString() + '-' +
                    DatePicker.Date.Month.ToString() + '-' +
                    DatePicker.Date.Day.ToString();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@login", Login);
                command.Parameters.AddWithValue("@password", Password);
                command.Parameters.AddWithValue("@fullName", FullNameTextBox.Text);
                command.Parameters.AddWithValue("@phoneNumber", Phone.Text);
                command.Parameters.AddWithValue("@paymentCardNumber", PaymentCardNumberTextBox.Text);
                command.Parameters.AddWithValue("@passportNumber", PassportTextBox.Text);
                command.Parameters.AddWithValue("@TIN", TINTextBox.Text);
                command.Parameters.AddWithValue("@birthDate", birthDate);
                command.Parameters.AddWithValue("@postId", (PostComboBox.SelectedItem as Post).Id);
                command.Parameters.AddWithValue("@photo", ByteImage);

                await command.ExecuteNonQueryAsync();
            }

            MainConnection.Close();
            (Parent as Frame).Navigate(typeof(WorkersOverviewPage), MainConnection);
        }

        private async Task SaveChanges()
        {
            if (MainConnection.State == System.Data.ConnectionState.Closed)
                await MainConnection.OpenAsync();
            using (SqlCommand command = new SqlCommand("sp_editWorker", MainConnection))
            {
                string birthDate = DatePicker.Date.Year.ToString() + '-' +
                    DatePicker.Date.Month.ToString() + '-' +
                    DatePicker.Date.Day.ToString();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", Worker.Id);
                command.Parameters.AddWithValue("@newFullName", FullNameTextBox.Text);
                command.Parameters.AddWithValue("@newPhoneNumber", Phone.Text);
                command.Parameters.AddWithValue("@newPaymentCardNumber", PaymentCardNumberTextBox.Text);
                command.Parameters.AddWithValue("@newPassportNumber", PassportTextBox.Text);
                command.Parameters.AddWithValue("@newTIN", TINTextBox.Text);
                command.Parameters.AddWithValue("@newBirthDate", birthDate);
                command.Parameters.AddWithValue("@newPostId", (PostComboBox.SelectedItem as Post).Id);
                command.Parameters.AddWithValue("@newPhoto", ByteImage);

                await command.ExecuteNonQueryAsync();
            }

            MainConnection.Close();
            (Parent as Frame).Navigate(typeof(WorkersOverviewPage), MainConnection);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => (Parent as Frame).GoBack();

        private async void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
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
                Photo.Source = writeableBitmap;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            WorkerParameters parameters = e.Parameter as WorkerParameters;
            Worker = parameters.Worker;
            EditButton.IsEnabled = (Worker == null) ? false : true;
            OptionsButton.IsEnabled = (Worker == null) ? false : true;
            SaveButton.IsEnabled = (Worker == null) ? true : false;
            MainConnection = parameters.Connection;
            Password = parameters.Password;
            Login = parameters.Login;
            if (Worker != null)
            {
                DeactivateAll();
                await GetImage();
            }
            await GetPostsAsync();
            LoadWorker();
        }

        private async Task GetPostsAsync()
        {
            if (MainConnection.State == System.Data.ConnectionState.Closed)
                await MainConnection.OpenAsync();
            using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM posts", MainConnection))
            {
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            Posts.Add(new Post(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            MainConnection.Close();
        }

        private void LoadWorker()
        {
            if (Worker != null)
            {
                IEnumerable<Post> thisPost = from post in Posts
                                             where post.Id == Worker.PostId
                                             select post;
                FullNameTextBox.Text = Worker.FullName;
                PostComboBox.SelectedItem = thisPost.First();
                Phone.Text = Worker.PhoneNumber;
                DatePicker.Date = Worker.BirthDate;
                AgeTextBlock.Text = Worker.Age.ToString();
                PassportTextBox.Text = Worker.PassportNumber;
                PaymentCardNumberTextBox.Text = Worker.PaymentCardNumber;
                TINTextBox.Text = Worker.TIN;
                Photo.Source = Worker.Photo;
                //WriteableBitmap writeableBitmap = new WriteableBitmap(Worker.Photo.PixelWidth, Worker.Photo.PixelHeight);
            }
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            AgeTextBlock.Text = (DateTime.Now.Year - DatePicker.Date.Year).ToString();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = true;
            EditButton.IsEnabled = false;
            ActivateAll();
        }

        private void ActivateAll()
        {
            UploadPhotoButton.IsEnabled = true;
            FullNameTextBox.IsEnabled = true;
            Phone.IsEnabled = true;
            PaymentCardNumberTextBox.IsEnabled = true;
            PassportTextBox.IsEnabled = true;
            TINTextBox.IsEnabled = true;
            DatePicker.IsEnabled = true;
            PostComboBox.IsEnabled = true;
        }

        private void DeactivateAll()
        {
            UploadPhotoButton.IsEnabled = false;
            FullNameTextBox.IsEnabled = false;
            Phone.IsEnabled = false;
            PaymentCardNumberTextBox.IsEnabled = false;
            PassportTextBox.IsEnabled = false;
            TINTextBox.IsEnabled = false;
            DatePicker.IsEnabled = false;
            PostComboBox.IsEnabled = false;
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog();
            confirmDialog.Content = "Вы уверены, что хотите удалить данного сотрудника? \nЭто действие не может быть отменено";
            confirmDialog.PrimaryButtonText = "Да";
            confirmDialog.SecondaryButtonText = "Нет";
            confirmDialog.PrimaryButtonClick += async (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                using (SqlCommand sqlCommand = new SqlCommand("sp_deleteWorker", MainConnection))
                {
                    if (MainConnection.State == System.Data.ConnectionState.Closed)
                        await MainConnection.OpenAsync();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@id", Worker.Id);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                confirmDialog.Hide();
                (Parent as Frame).Navigate(typeof(WorkersOverviewPage), MainConnection);
            };
            confirmDialog.SecondaryButtonClick += (ContentDialog dialog, ContentDialogButtonClickEventArgs args) =>
            {
                dialog.Hide();
            };
            await confirmDialog.ShowAsync();
        }

        private async Task GetImage()
        {
            using (SqlConnection connection = new SqlConnection(MainConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand($"SELECT photo FROM workers WHERE workers.id = {Worker.Id}", connection))
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        if (reader["photo"] != DBNull.Value)
                        {
                            ByteImage = (byte[])reader["photo"];
                        }
                    }
                    connection.Close();
                }
            }
        }
    }
}
