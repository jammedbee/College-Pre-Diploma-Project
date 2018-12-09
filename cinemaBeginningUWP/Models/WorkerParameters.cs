using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cinemaBeginningUWP.Models
{
    public class WorkerParameters : ConnectionParameter
    {
        public Worker Worker;
        public string Login;
        public string Password;
        public int Id;
    }
}
