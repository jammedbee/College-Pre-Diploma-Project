using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cinemaBeginningUWP.Models
{
    class HallParameters : ConnectionParameter
    {
        public int Id { get; set; }
        public Hall Hall { get; set; }
    }
}
