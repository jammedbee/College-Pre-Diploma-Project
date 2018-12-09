using System.Collections.Generic;

namespace cinemaBeginningUWP
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RowCount { get; set; }
        public List<HallRow> HallRows { get; set; }

        public Hall(int id, string hallName, int rowCount)
        {
            Id = id;
            Name = hallName;
            RowCount = rowCount;
            HallRows = new List<HallRow>();
        }
    }
}
