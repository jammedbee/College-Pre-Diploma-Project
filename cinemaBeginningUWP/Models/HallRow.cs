using System.Collections.Generic;

namespace cinemaBeginningUWP
{
    public class HallRow
    {
        public int RowId { get; set; }
        public int RowNumber { get; set; }
        public int SeatCount { get; set; }
        public List<Seat> Seats;

        public HallRow(int rowId, int rowNumber, int seatCount)
        {
            RowId = rowId;
            RowNumber = rowNumber;
            SeatCount = seatCount;
            Seats = new List<Seat>();
        }
    }
}
