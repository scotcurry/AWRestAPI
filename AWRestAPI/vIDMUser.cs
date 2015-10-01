using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPIApp
{
    public class VIDMUser
    {
        public List<string> schemas { get; set; }
        public string userName { get; set; }
        public name name { get; set; }
        public List<emails> emails { get; set; }
        public string password { get; set; }
    }

    public class name
    {
        public string givenName { get; set; }
        public string familyName { get; set; }
    }

    public class emails
    {
        public string value { get; set; }
    }
}
