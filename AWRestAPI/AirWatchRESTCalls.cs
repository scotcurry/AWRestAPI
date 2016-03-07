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

        #region Public Methods
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

        /// <summary>
        /// Deletes a basic User, when using this method you should check if the user exists and is 
        /// a basic user.  There is no way to delete AD users.  Currently only supports Username deletion.
        /// </summary>
        /// <param name="usersToDelete">A StringCollection of user names to be deleted.
        ///                             If just one user is to be deleted just one value
        ///                             in the collection.</param>
        /// <returns>Returns the REST API return value.  200 means it worked.</returns>
        public int DeleteUsers(StringCollection usersToDelete)
        {
            string jsonToPost = AirWatchRESTCallsHelpers.BuildBulkValuesJSON(usersToDelete);
            string apiEndpoint = "/API/v1/system/users/delete";
            IRestResponse response = restHandler.RestPostEndpointBasicAuth(apiEndpoint, userName, password, headers, jsonToPost);

            return 0;
        }

        /// <summary>
        /// There are many overloads of this method.  Creates an account based on the fields input.
        /// </summary>
        /// <param name="userName">Username of the account</param>
        /// <param name="password"></param>
        /// <param name="emailAddress"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public int CreateBasicUser(string userName, string password, string emailAddress, string firstName, string lastName)
        {
            return 0;
        }

        public int CreateBasicUser(string userName, string password, string emailAddress, string firstName, string lastName, string roleName)
        {
            return 0;
        }
        #endregion

        #region Private Methods

        private int createUserAllOverLoads(string userName, string password, string emailAddress, string firstName, string lastName, string roleName)
        {
            AirWatchUser userToCreate = new AirWatchUser();
            userToCreate.UserName = userName;
            userToCreate.Password = password;
            userToCreate.Email = emailAddress;
            userToCreate.FirstName = firstName;
            userToCreate.LastName = lastName;

            if (roleName != null)
                userToCreate.Role = roleName;

            return 0;
        }
        #endregion

    }
}
