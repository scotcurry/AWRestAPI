using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Use this so the function returns the REST Response
using RestSharp;
using Newtonsoft.Json;

using RestAPIApp;

namespace AWRestAPI
{
    public class AirWatchRESTCalls
    {
        private NameValueCollection headers;
        private string userName;
        private string password;
        private RestHandler restHandler;

        public AirWatchRESTCalls(NameValueCollection Headers, string UserName, string Password, string baseURL)
        {
            headers = Headers;
            userName = UserName;
            password = Password;

            restHandler = new RestHandler(baseURL);
        }

        public enum UserSearchType { Username, EmailAddress };

        /// <summary>
        /// Returns an AirWatchUser class populated with the returned user if found.  Returns null if not found
        /// </summary>
        /// <param name="searchString">AirWatch Username or Email address of user to search</param>
        /// <param name="searchType"></param>
        /// <returns>An AirWatchUser or null</returns>
        public AirWatchUser SearchForUser(string searchString, UserSearchType searchType)
        {
            string apiEndpoint = string.Empty;
            if (searchType == UserSearchType.Username)
            {
                apiEndpoint = "/API/v1/system/users/search?username=" + searchString;
            }
            else if (searchType == UserSearchType.EmailAddress)
            {
                apiEndpoint = "/API/v1/system/users/search?email=" + searchString;
            }

            AirWatchUser userToReturn = new AirWatchUser();
            IRestResponse response = restHandler.RestGetEndpointBasicAuth(apiEndpoint, userName, password, headers);
            AirWatchUsers awUser = new AirWatchUsers();
            if (response.StatusDescription == "OK")
            {
                AirWatchUsers usersReturned = new AirWatchUsers();
                JsonConvert.PopulateObject(response.Content, usersReturned);
                userToReturn = usersReturned.Users[0];
            }
            else
            {
                userToReturn = null;
            }
            return userToReturn;
        }
    }
}
