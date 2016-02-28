using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWRestAPI
{
    public class AirWatchUser
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }     
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public int SecurityType { get; set; }
        public int LocationGroupID { get; set; }
        public string MessageType { get; set; }
        public string EmailUserName { get; set; }
    }
}
