using cinemaBeginningUWP.Models;
using System;
using System.Data.SqlClient;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class LoginPage : Page
    {
        private SqlConnection mainConnection;

        public LoginPage()
        {
            this.InitializeComponent();

            mainConnection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=cinema; Integrated Security=SSPI");
        }

        private async void LogIn_Click(object sender, RoutedEventArgs e)
        {
            Progress.IsActive = true;
            Login.IsEnabled = false;
            LogIn.IsEnabled = false;
            Password.IsEnabled = false;
            Cancel.IsEnabled = false;
            using (SqlCommand sqlCommand = new SqlCommand($"sp_autorization", mainConnection))
            {
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@login", Login.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@password", Password.Password));
                sqlCommand.Parameters.Add(new SqlParameter("@status", 10)).Direction = System.Data.ParameterDirection.Output;
                sqlCommand.Parameters.Add(new SqlParameter("@fullName", System.Data.SqlDbType.NVarChar, 255)).Direction = System.Data.ParameterDirection.Output;
                sqlCommand.Parameters.Add(new SqlParameter("@postId", -1)).Direction = System.Data.ParameterDirection.Output;
                await mainConnection.OpenAsync();
                int res = await sqlCommand.ExecuteNonQueryAsync();

                if ((int)sqlCommand.Parameters["@status"].Value == 1)
                {
                    User user = new User((string)sqlCommand.Parameters["@fullName"].Value, (int)sqlCommand.Parameters["@postId"].Value);
                    (Parent as Frame).Navigate(typeof(MainLayoutPage), new UserParameters { CurrentUser = user, Connection = mainConnection });
                }
                else
                {
                    Progress.IsActive = !Progress.IsActive;
                    await new MessageDialog("Неверный логин или пароль", "Ошибка").ShowAsync();
                }
            }
            Progress.IsActive = false;
            mainConnection.Close();
            Login.IsEnabled = !Login.IsEnabled;
            LogIn.IsEnabled = !LogIn.IsEnabled;
            Password.IsEnabled = !Password.IsEnabled;
            Cancel.IsEnabled = !Cancel.IsEnabled;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
