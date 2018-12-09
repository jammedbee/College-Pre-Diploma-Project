namespace cinemaBeginningUWP
{
    public class Seat
    {
        public Hall Hall { get; set; }
        public HallRow Row { get; set; }
        public int Number { get; set; }
        public bool IsTaken { get; set; }
        

        public Seat(int seatNumber, Hall hall, HallRow row, bool isTaken)
        {
            Hall = hall;
            Row = row;
            Number = seatNumber;
            IsTaken = isTaken;
        }

        public void TakeSeat()
        {
            IsTaken = true;
        }

        public void FreeSeat()
        {
            IsTaken = false;
        }
    }
}
