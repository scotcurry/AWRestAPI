using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestAPIApp;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestHandlerTests
{

    [TestClass]
    public class RestTests
    {
        // Environment Information
        private string awServer = string.Empty;
        private string awTenantName = "aw-tenant-code";
        private string awTenantCode = string.Empty;
        private string userName = string.Empty;
        private string password = string.Empty;
        private int locationGroupID = 0;

        // Values used for all tests
        private string acceptTypeName = "Accept";
        private string acceptTypeValue = "application/json";
        private static string createUserFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "createUser.txt");

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

        [TestMethod]
        public void TestCreateAirWatchUser()
        {
            if (awServer == string.Empty)
                Assert.Fail("Server Not Set");
            if (locationGroupID == 0)
                Assert.Fail("Location Group Not Set");
            if (awTenantCode == string.Empty)
                Assert.Fail("Tenant Code Not Set");
            if (userName == string.Empty)
                Assert.Fail("Username Not Set");
            if (password == string.Empty)
                Assert.Fail("Password Not Set");

            AirWatchUser newUser = new AirWatchUser();
            newUser.Email = "user@company.com";
            newUser.FirstName = "First";
            newUser.LastName = "Name";
            newUser.LocationGroupID = locationGroupID;
            newUser.MessageType = "Email";
            newUser.Password = "password";
            newUser.SecurityType = 2;
            newUser.Status = true;
            newUser.UserName = "RestUser";
            newUser.EmailUserName = "EmailUserName";

            string userJSON = JsonConvert.SerializeObject(newUser);

            NameValueCollection headers = new NameValueCollection();
            headers.Add(awTenantName, awTenantCode);
            headers.Add(acceptTypeName, acceptTypeValue);

            string awSiteURL = awServer;
            RestHandler restHandler = new RestHandler(awSiteURL);
            IRestResponse returnCode = restHandler.RestPostEndpointBasicAuth("system/users/adduser", userName, password, headers, userJSON);
            if (returnCode.StatusDescription == "OK")
            {
                JObject returnedContent = JObject.Parse(returnCode.Content);
                string createdUserId = (string)returnedContent["Value"];
                using (StreamWriter sw = new StreamWriter(createUserFile))
                {
                    sw.WriteLine(Convert.ToString(createdUserId));
                }
            }

            Assert.AreEqual(returnCode.StatusDescription, "OK");
        }

        [TestMethod]
        public void TestAirWatchCall()
        {
            if (awServer == string.Empty)
                Assert.Fail("AirWatch Server Not Set");
            if (locationGroupID == 0)
                Assert.Fail("Location Group Not Set");
            if (awTenantCode == string.Empty)
                Assert.Fail("Tenant Code Not Set");
            if (userName == string.Empty)
                Assert.Fail("Username Not Set");
            if (password == string.Empty)
                Assert.Fail("Password Not Set");

            NameValueCollection headers = new NameValueCollection();
            headers.Add(awTenantName, awTenantCode);
            headers.Add(acceptTypeName, acceptTypeValue);

            string awSiteURL = awServer;
            RestHandler restHandler = new RestHandler(awSiteURL);
            IRestResponse returnCode = restHandler.RestGetEndpointBasicAuth("system/admins/search", userName, password, headers);

            Assert.AreEqual(returnCode.StatusDescription, "OK");
        }

        [TestMethod]
        public void TestRegisterDevice()
        {
            if (awServer == string.Empty)
                Assert.Fail("AirWatch Server Not Set");
            if (locationGroupID == 0)
                Assert.Fail("Location Group Not Set");
            if (awTenantCode == string.Empty)
                Assert.Fail("Tenant Code Not Set");
            if (userName == string.Empty)
                Assert.Fail("Username Not Set");
            if (password == string.Empty)
                Assert.Fail("Password Not Set");

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
            deviceToRegister.AssetNumber = "Asset #1";
            deviceToRegister.FriendlyName = "Scot's Device";
            deviceToRegister.LocationGroupId = locationGroupID;
            deviceToRegister.MessageType = "Email";
            deviceToRegister.ModelId = 3;
            deviceToRegister.OperatingSystemId = 2;
            deviceToRegister.Ownership = "Corporate";
            deviceToRegister.PlatformId = 2;
            deviceToRegister.SerialNumber = "124";
            deviceToRegister.ToEmailAddress = "user@organization.com";
            deviceToRegister.Udid = "123123123123";

            string jsonToSend = JsonConvert.SerializeObject(deviceToRegister);

            NameValueCollection headers = new NameValueCollection();
            headers.Add(awTenantName, awTenantCode);
            headers.Add(acceptTypeName, acceptTypeValue);

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
