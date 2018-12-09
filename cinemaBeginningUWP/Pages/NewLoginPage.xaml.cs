using cinemaBeginningUWP.Models;
using System;
using System.Data.SqlClient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class NewLoginPage : Page
    {
        public SqlConnection MainConnection;

        public NewLoginPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainConnection = e.Parameter as SqlConnection;
        }

        private async void Login_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckingLoginProgress.IsActive = true;
            if (String.IsNullOrWhiteSpace(Login.Text))
            {

            }
            else
            {
                using (SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM logins WHERE (logins.login LIKE '{Login.Text}')", MainConnection))
                {
                    if (MainConnection.State == System.Data.ConnectionState.Closed)
                        await MainConnection.OpenAsync();
                    int result = (int)await command.ExecuteScalarAsync();
                    if (result != 0)
                    {
                        Continue.IsEnabled = false;
                        LoginExistsFlyout.ShowAt(Login);
                    }
                    else
                    {
                        CheckInputs();
                        LoginExistsFlyout.Hide();
                    }
                }
            }
            CheckingLoginProgress.IsActive = false;
        }

        private void CheckInputs()
        {
            if ((Password.Password == ConfirmedPassword.Password) &&
                (!String.IsNullOrWhiteSpace(Password.Password)) &&
                (!String.IsNullOrWhiteSpace(Login.Text)))
            {
                Continue.IsEnabled = true;
            }
            else
            {
                Continue.IsEnabled = false;
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(WorkerPage), new WorkerParameters
            {
                Connection = MainConnection,
                Login = Login.Text,
                Password = Password.Password,
                Worker = null
            });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Login.Text = string.Empty;
            Password.Password = string.Empty;
            ConfirmedPassword.Password = string.Empty;
            (Parent as Frame).GoBack();
        }

        private void ConfirmedPassword_PasswordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            CheckInputs();
        }

        private async void Login_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            CheckingLoginProgress.IsActive = true;
            if (String.IsNullOrWhiteSpace(Login.Text))
            {

            }
            else
            {
                using (SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM logins WHERE (logins.login LIKE '{Login.Text}')", MainConnection))
                {
                    if (MainConnection.State == System.Data.ConnectionState.Closed)
                        await MainConnection.OpenAsync();
                    int result = (int)await command.ExecuteScalarAsync();
                    if (result != 0)
                        Continue.IsEnabled = false;

                }
            }
            CheckingLoginProgress.IsActive = false;
        }
    }
}
