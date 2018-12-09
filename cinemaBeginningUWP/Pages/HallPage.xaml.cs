using cinemaBeginningUWP.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace cinemaBeginningUWP.Pages
{
    public sealed partial class HallPage : Page
    {
        private SqlConnection MainConnection;
        private Hall Hall;

        public HallPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HallParameters parameters = e.Parameter as HallParameters;
            Hall = parameters.Hall;
            MainConnection = parameters.Connection;
            await LayoutDesign();
            base.OnNavigatedTo(e);
        }

        private async Task LayoutDesign()
        {
            for (int i = 0; i < Hall.RowCount; i++)
            {
                HallGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            }
            for (int j = 0; j < Hall.HallRows[0].SeatCount; j++)
            {
                HallGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            }
            for (int i = 0; i < HallGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < HallGrid.ColumnDefinitions.Count; j++)
                {
                    Button button = new Button();
                    button.Content = (j + 1).ToString();
                    button.Height = 50;
                    button.Width = 50;
                    button.IsEnabled = Hall.HallRows[i].Seats[j].IsTaken ? false : true;
                    button.Click += async (object sender, RoutedEventArgs e) =>
                    {
                        Hall.HallRows[Grid.GetRow(button)].Seats[Grid.GetColumn(button)].TakeSeat();
                        (sender as Button).IsEnabled = false;
                        await UpdateSeatStatus(Hall.HallRows[Grid.GetRow(button)].Seats[Grid.GetColumn(button)]);
                    };
                    HallGrid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
            }
        }

        private async void FreeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            progress.IsActive = !progress.IsActive;
            foreach (HallRow row in Hall.HallRows)
            {
                foreach (var seat in row.Seats)
                {
                    seat.FreeSeat();
                    await UpdateSeatStatus(seat);
                }
            }
            foreach (var item in HallGrid.Children)
            {
                if (item is Button)
                    (item as Button).IsEnabled = true;
            }
            progress.IsActive = !progress.IsActive;
        }

        private async Task UpdateSeatStatus(Seat seat)
        {
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = $"UPDATE hallSeats " +
                    $"SET status = {Convert.ToByte(seat.IsTaken)} " +
                    $"WHERE (hallId = {seat.Hall.Id} AND rowId = {seat.Row.RowId} AND seatNumber = {seat.Number})";
                sqlCommand.Connection = MainConnection;

                if (MainConnection.State == System.Data.ConnectionState.Closed)
                {
                    await MainConnection.OpenAsync();
                }

                await sqlCommand.ExecuteNonQueryAsync();
            }

            MainConnection.Close();
        }
    }
}
