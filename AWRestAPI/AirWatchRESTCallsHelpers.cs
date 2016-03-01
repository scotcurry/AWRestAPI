using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace AWRestAPI
{
    class AirWatchRESTCallsHelpers
    {
        public static string BuildBulkValuesJSON(StringCollection valuesToAdd)
        {
            bool firstUser = true;
            const char quote = '\"';
            string bulkJSONString = "{ \"BulkValues\": { ";
            foreach (string currentUser in valuesToAdd)
            {
                if (firstUser == true)
                {
                    bulkJSONString += quote + "Value:" + quote + ":[" + quote + currentUser + quote;
                    firstUser = false;
                }
                else
                {
                    bulkJSONString += ", " + quote + currentUser + quote + " ";
                }
            }

            bulkJSONString += "] } }";
            return bulkJSONString;
        }
    }
}
