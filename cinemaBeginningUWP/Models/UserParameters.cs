using cinemaBeginningUWP.Models;
using System.Data.SqlClient;

namespace cinemaBeginningUWP
{
    public class UserParameters : ConnectionParameter
    {
        public User CurrentUser;
    }
}
