using cinemaBeginningUWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class HallsOverviewPage : Page
    {
        SqlConnection MainConnection;
        ObservableCollection<Hall> Halls;

        public HallsOverviewPage()
        {
            this.InitializeComponent();
            Halls = new ObservableCollection<Hall>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(HallPage), new HallParameters { Connection = MainConnection, });
        }

        private async Task GetHallsAsync()
        {
            if (Halls.Count > 0)
            {
                Halls = new ObservableCollection<Hall>();
            }
            if (MainConnection.State == System.Data.ConnectionState.Closed)
            {
                await MainConnection.OpenAsync();
            }

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = "SELECT * FROM halls";
                sqlCommand.Connection = MainConnection;
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Halls.Add(new Hall(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)
                                ));
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

        private async Task<Hall> GetFullHallInformation(int hallId)
        {
            Hall hall = new Hall(0, "none", 0);

            if (MainConnection.State == System.Data.ConnectionState.Closed)
            {
                await MainConnection.OpenAsync();
            }

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = MainConnection;
                sqlCommand.CommandText = $"SELECT * FROM halls WHERE halls.id = {hallId}";
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync()) 
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            hall = new Hall(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                        }
                    }
                }
            }

            hall.HallRows = await GetHallRowsAsync(hall.Id);

            for (int i = 0; i < hall.RowCount; i++)
            {
                hall.HallRows[i].Seats = await GetSeatsAsync(hall, hall.HallRows[i]);
            }

            return hall;
        }

        private async Task<List<HallRow>> GetHallRowsAsync(int hallId)
        {
            List<HallRow> rows = new List<HallRow>();
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = MainConnection;
                sqlCommand.CommandText = $"SELECT * FROM hallRows WHERE hallRows.hallId = {hallId}";
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new HallRow(reader.GetInt32(0), reader.GetInt32(2), reader.GetInt32(3)));
                        }
                    }
                }
            }

            return rows;
        }

        private async Task<List<Seat>> GetSeatsAsync(Hall hall, HallRow row)
        {
            List<Seat> seats = new List<Seat>();
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = MainConnection;
                sqlCommand.CommandText = $"SELECT * FROM hallSeats WHERE hallSeats.hallId = {hall.Id} AND hallSeats.rowId = {row.RowId}";
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            seats.Add(new Seat(reader.GetInt32(2), hall, row, reader.GetBoolean(3)));
                        }
                    }
                }
            }

            return seats;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainConnection = e.Parameter as SqlConnection;
            await GetHallsAsync();
            base.OnNavigatedTo(e);
        }

        private async void HallsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {

            (Parent as Frame).Navigate(typeof(HallPage),
                new HallParameters
                {
                    Connection = MainConnection,
                    Id = (e.ClickedItem as Hall).Id,
                    Hall = await GetFullHallInformation((e.ClickedItem as Hall).Id)
                });
        }
    }
}
