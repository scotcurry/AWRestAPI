﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestAPIApp;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AWRestAPI;

namespace RestHandlerTests
{

    [TestClass]
    public class RestTests
    {
        /*
          You can fill in the fields you want to test here.  This also supports a settings.json
          file that you will need to modify the file information in the TestInitialize section.

          This test also works only with JSON, so if you want to use XML you will need to make 
          changes to refactor a great deal of the code.
        */
        private string awServer = "https://demo.awmdm.com";
        private string awTenantCode = "6+uNI3w/kFgA78hSRYpzuIleq4MY6A7WPmo9K9AbM6A=";
        private string userName = @"airwatchdemo\scurry";
        private string password = "AirWatch2@";
        private int locationGroupID = 0;

        /*
          Fields for user creation / lookup /delte functions
        */
        private string awUserName = "Vincent";
        private string awUserPassword = "AirWatch1";
        private string awEmailAddress = "scotcurry@air-watch.com";

        // Values used for all tests
        private string acceptTypeName = "Accept";
        private string acceptTypeValue = "application/json";
        private string awTenantName = "aw-tenant-code";
        private static string createUserFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "createUser.txt");
        private NameValueCollection headers = new NameValueCollection();

        // Build the AirWatchRESTCalls and set the Authentication, Accept, and Content-Type in the TestInitialize
        private AirWatchRESTCalls awRestCalls;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            if (File.Exists(createUserFile))
                File.Delete(createUserFile);
        }

        [TestInitialize]
        public void TestInit()
        {
            string documentFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string settingsFile = Path.Combine(documentFolder, "settings.json");
            if (File.Exists(settingsFile))
            { 
                string settingsJSON = string.Empty;
                using (StreamReader sr = new StreamReader(settingsFile))
                {
                    settingsJSON = sr.ReadToEnd();
                }

                JObject jsonParse = JObject.Parse(settingsJSON);
                awServer = (string)jsonParse["AirWatchServer"];
                awTenantCode = (string)jsonParse["Tenant_Code"];
                userName = (string)jsonParse["UserName"];
                password = (string)jsonParse["Password"];
                locationGroupID = (int)jsonParse["LocationGroupID"];
            }

            headers.Add(awTenantName, awTenantCode);
            headers.Add(acceptTypeName, acceptTypeValue);
            awRestCalls = new AirWatchRESTCalls(headers, userName, password, awServer);
        }

        bool userForCreateDeleteExists = false;
        [TestMethod]
        public void CheckIfUserExists()
        {
            AirWatchUser userRecord = awRestCalls.SearchForUser("VincentCurry", AirWatchRESTCalls.UserSearchType.Username);
            Assert.IsNotNull(userRecord.UserName);
        }

        [TestMethod]
        public void DeleteUsers()
        {
            StringCollection usersToDelete = new StringCollection();
            usersToDelete.Add("OtisCurry");
            usersToDelete.Add("Scurry");
            int userWasDeleted = awRestCalls.DeleteUsers(usersToDelete);
        }

        [TestMethod]
        public void TestCreateBasicAirWatchUser()
        {
            // Add basic user
            /*
            AirWatchUser newUser = new AirWatchUser();
            newUser.Email = "scotcurry@air-watch.com";
            newUser.FirstName = "Vincent";
            newUser.LastName = "Curry";
            newUser.LocationGroupID = locationGroupID;
            newUser.MessageType = "Email";
            newUser.Password = "AirWatch2@";
            newUser.SecurityType = 2;
            newUser.Status = true;
            newUser.UserName = "VincentCurry";
            newUser.EmailUserName = "vcurry";
            */

            // Add Directory user
            AirWatchUser newUser = new AirWatchUser();
            newUser.UserName = "vincent";
            newUser.Status = true;
            newUser.SecurityType = 1;
            string userJSON = JsonConvert.SerializeObject(newUser);

            string awSiteURL = awServer;
            RestHandler restHandler = new RestHandler(awSiteURL);
            IRestResponse returnCode = restHandler.RestPostEndpointBasicAuth("/API/v1/system/users/adduser", userName, password, headers, userJSON);
            if (returnCode.StatusDescription == "OK")
            {
                JObject returnedContent = JObject.Parse(returnCode.Content);
                string createdUserId = (string)returnedContent["Value"];
                using (StreamWriter sw = new StreamWriter(createUserFile))
                {
                    sw.WriteLine(Convert.ToString(createdUserId));
                }
            }
            Assert.AreEqual("OK", returnCode.StatusDescription);
        }

        [TestMethod]
        public void TestAirWatchCall()
        {
            string awSiteURL = awServer;
            RestHandler restHandler = new RestHandler(awSiteURL);
            IRestResponse returnCode = restHandler.RestGetEndpointBasicAuth("/API/v1/system/admins/search", userName, password, headers);

            Assert.AreEqual("OK", returnCode.StatusDescription);
        }

        [TestMethod]
        public void TestRegisterDevice()
        {
            string createdUserID = string.Empty;
            if (File.Exists(createUserFile))
            {
                using (StreamReader sr = new StreamReader(createUserFile))
                {
                    createdUserID = sr.ReadLine();
                }
            }
            else
                Assert.Fail("Not Created User File");

            AirWatchDevices deviceToRegister = new AirWatchDevices();
            // deviceToRegister.AssetNumber = "Asset #1";
            deviceToRegister.FriendlyName = "Scot's Device";
            deviceToRegister.LocationGroupId = locationGroupID;
            deviceToRegister.MessageType = "Email";
            deviceToRegister.ModelId = 2;
            // deviceToRegister.OperatingSystemId = 2;
            deviceToRegister.Ownership = "Corporate";
            deviceToRegister.PlatformId = 2;
            // deviceToRegister.SerialNumber = "DLXNV3RZFLMJ";
            // deviceToRegister.ToEmailAddress = "scotcurry@air-watch.com";
            // deviceToRegister.Udid = "7BBE62B953C21847AA447919304027C98CCB148C";

            string jsonToSend = JsonConvert.SerializeObject(deviceToRegister);

            string awSiteURL = awServer;
            RestHandler restHandler = new RestHandler(awSiteURL);
            string endpoint = "system/users/" + Convert.ToString(createdUserID) + "/registerdevice";
            IRestResponse returnCode = restHandler.RestPostEndpointBasicAuth(endpoint, userName, password, headers, jsonToSend);
            if (returnCode.StatusDescription == "OK")
                createdUserID = "debug";

            Assert.AreEqual(returnCode.StatusDescription, "OK");
        }
    }
}
