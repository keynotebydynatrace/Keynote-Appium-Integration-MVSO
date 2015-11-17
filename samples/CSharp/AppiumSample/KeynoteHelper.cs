using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AppiumSample.helpers;
using Newtonsoft.Json;


namespace AppiumSample
{
    public static class KeynoteHelper
    {
        private static String sessionId = "";
        private static KeynoteConfig keynoteConfig = null;

        static KeynoteHelper()
        {
                    //string configdata = File.ReadAllText(Path.Combine(GetInstalledDirectory(), "KeynoteConfig.json"));

                    // if (configdata != null)
                    // {
                    //     keynoteConfig = JsonConvert.DeserializeObject<KeynoteConfig>(configdata);
                    // }

            keynoteConfig = new KeynoteConfig();
            keynoteConfig.AccessServerUrl = Environment.GetEnvironmentVariable("KEYNOTE_APPIUM_HUB_URL");//, EnvironmentVariableTarget.Machine);
            keynoteConfig.Email = Environment.GetEnvironmentVariable("KEYNOTE_USER_NAME");//, EnvironmentVariableTarget.Machine);
            keynoteConfig.Password = Environment.GetEnvironmentVariable("KEYNOTE_PASSWORD");//, EnvironmentVariableTarget.Machine);

            System.Console.WriteLine("KEYNOTE_APPIUM_HUB_URL -->" + Environment.GetEnvironmentVariable("KEYNOTE_APPIUM_HUB_URL"));
            System.Console.WriteLine("KEYNOTE_USERNAME -->" + Environment.GetEnvironmentVariable("KEYNOTE_USERNAME"));
            System.Console.WriteLine("KEYNOTE_PASSWORD -->" + Environment.GetEnvironmentVariable("KEYNOTE_PASSWORD"));

            System.Console.WriteLine("Initializing keynote API");
        }
        

        /// <summary>
        /// Gets the appium url after authentication
        /// </summary>
        /// <param name="mcd"></param>
        /// <returns></returns>
        public static Uri GetAppiumUrl(int mcd)
        {
            try
            {
                string appiumURL = "";
                //sessionId = "";

                if(string.IsNullOrEmpty(sessionId)) 
                    sessionId = AuthenticateUser(keynoteConfig.Email, keynoteConfig.Password);
  
                if (!string.IsNullOrEmpty(sessionId))
                {
                    if (LockDevice(sessionId, mcd))
                    {
                        appiumURL = StartAppium(sessionId, mcd);
                    }
                }

                if (!string.IsNullOrEmpty(appiumURL))
                {
                    return new Uri(appiumURL);
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp);
                return null;
            }
        }

        /// <summary>
        /// Authenticate User with valid user info
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>User session id</returns>
        private static string AuthenticateUser(string userName, string password)
        {
            System.Console.WriteLine("Authenticating...");
            
            var requestJson = new UserInfo();
            requestJson.Email = userName;
            requestJson.Password = password;
            
            RESTClient client = new RESTClient();
            string result = client.PostUrl(keynoteConfig.AccessServerUrl + "portal/establish-api-session", JsonConvert.SerializeObject(requestJson));
  
            if (result != null)
            {
                var resultJson = JsonConvert.DeserializeObject<dynamic>(result);

                if (resultJson.status == "SUCCESS") {

                    System.Console.WriteLine("Authenticated Successfully");
                    return resultJson.sessionID;
                }
                    
                else
                    return null;
            }
            return null;

        }

        /// <summary>
        /// Acquire the device for manual or automated test
        /// </summary>
        /// <param name="sessionId">user session id</param>
        /// <param name="mcd">Device uniq id</param>
        /// <returns>lock result</returns>
        private static bool LockDevice(string sessionId, int mcd)
        {
            System.Console.WriteLine("Lock device...");
            
            var requestJson = new SessionInfo();
            requestJson.SessionId = sessionId;

            RESTClient client = new RESTClient();
            string result = client.PostUrl(keynoteConfig.AccessServerUrl + "device/lock-device/" + mcd, JsonConvert.SerializeObject(requestJson));
     
            if (result != null)
            {
                var resultJson = JsonConvert.DeserializeObject<LockDeviceResponse>(result);
                System.Console.WriteLine("Lock device successfully...");
                return resultJson.Success;
            }

            return false;

        }

        /// <summary>
        /// Start appium server for the device
        /// </summary>
        /// <param name="sessionId">User valid session</param>
        /// <param name="mcd">Device uniq id</param>
        /// <returns>Appium url to apply in client appium script for automation</returns>
        private static string StartAppium(string sessionId, int mcd)
        {
            var requestJson = new SessionInfo();
            requestJson.SessionId = sessionId;

            System.Console.WriteLine("Starting Appium...");

            RESTClient client = new RESTClient();
            string result = client.Get(keynoteConfig.AccessServerUrl + "device/" + sessionId + "/start-appium/" + mcd);
      
            if (result != null)
            {
                System.Console.WriteLine("Started Appium session --> " + result);
                
                return result;
            }
            return null;

        }

        /// <summary>
        /// Logoout use session
        /// This will Stop Appium, Release Device and Log out user
        /// </summary>
        /// <returns></returns>
        public static string LogoutSession()
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                System.Console.WriteLine("Session logout --> " + sessionId);
                
                var requestJson = new SessionInfo();
                requestJson.SessionId = sessionId;

                RESTClient client = new RESTClient();
                string result = client.PostUrl(keynoteConfig.AccessServerUrl + "portal/logout-api-session", JsonConvert.SerializeObject(requestJson));

                sessionId = null;

                if (result != null)
                {
                    return result;
                }

                return null;
            }
            else
            {
                return null;
            }

        }

        private static string GetInstalledDirectory()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            string[] nodes = executingAssembly.CodeBase.Split(
                new string[] { "///" }, StringSplitOptions.None);

            string directory = Path.GetDirectoryName(nodes[1].Replace('/', '\\'));

            DirectoryInfo info = Directory.GetParent(directory);

            if (info.Name == "bin")
                info = Directory.GetParent(info.FullName);

            return info.FullName;
        }

        public static string GetApplicationURL(string appName, string appVersion, string filePath)
        {
            if (File.Exists(filePath))
            {
                System.Console.WriteLine("Add application --> " + filePath);
                
                AddApplicationRequest addAppRequest = new AddApplicationRequest();
                addAppRequest.AppName = appName;
                addAppRequest.FileName = appName;
                addAppRequest.AppType = Path.GetExtension(filePath).Replace(".", "");
                addAppRequest.AppVersion = appVersion;
                addAppRequest.IsSignApp = false;
                addAppRequest.IsEnableApp = false;
                addAppRequest.FileContent =  Convert.ToBase64String( File.ReadAllBytes(filePath));
                addAppRequest.IsGetDownloadUrl = true;

                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = AuthenticateUser(keynoteConfig.Email, keynoteConfig.Password);
                }

                RESTClient client = new RESTClient();
                string result = client.PostUrl(keynoteConfig.AccessServerUrl + "device/" + sessionId + "/add-application", JsonConvert.SerializeObject(addAppRequest));

                if (!string.IsNullOrEmpty(result))
                {
                    System.Console.WriteLine("Application Added successfully --> " + result);
                    result = "http://dademo111.deviceanywhere.com/app/534050.apk";
                    //result = keynoteConfig.AccessServerUrl + "applications/" + sessionId + "/download-data/" + result;
                }

                return result;
            }
            else
                return null;
        }
    }

    public class UserInfo
    {
        string email;

        [JsonProperty("email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        string password;

        [JsonProperty("password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }

    public class SessionInfo
    {
        string sessionId;

        [JsonProperty("sessionID")]
        public string SessionId
        {
            get { return sessionId; }
            set { sessionId = value; }
        }
    }

    public class LockDeviceResponse
    {
        bool success;

        [JsonProperty("success")]
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        string ensembleServerURL;

        [JsonProperty("ensembleServerURL")]
        public string EnsembleServerURL
        {
            get { return ensembleServerURL; }
            set { ensembleServerURL = value; }
        }
    }

    public class KeynoteConfig
    {
        private string accessServerUrl;

        [JsonProperty("access_server_url")]
        public string AccessServerUrl
        {
            get { return accessServerUrl; }
            set { accessServerUrl = value; }
        }

        string email;

        [JsonProperty("email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        string password;

        [JsonProperty("password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }

    
    public class AddApplicationRequest
    {

        private String appType;

        [JsonProperty("appType")]
        public String AppType
        {
            get { return appType; }
            set { appType = value; }
        }

        private String appName;

        [JsonProperty("appName")]
        public String AppName
        {
            get { return appName; }
            set { appName = value; }
        }

        private String fileName;

        [JsonProperty("fileName")]
        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private String appVersion;

        [JsonProperty("appVersion")]
        public String  AppVersion
        {
            get { return appVersion; }
            set { appVersion = value; }
        }

        private String fileContent;

         [JsonProperty("fileContent")]
        public String FileContent
        {
            get { return fileContent; }
            set { fileContent = value; }
        }

        private bool isSignApp;

        [JsonProperty("isSignApp")]
        public bool IsSignApp
        {
            get { return isSignApp; }
            set { isSignApp = value; }
        }

        private bool isEnableApp;

        [JsonProperty("isEnableApp")]
        public bool IsEnableApp
        {
            get { return isEnableApp; }
            set { isEnableApp = value; }
        }

        private bool isGetDownloadUrl;

        [JsonProperty("isGetDownloadUrl")]
        public bool IsGetDownloadUrl
        {
            get { return isGetDownloadUrl; }
            set { isGetDownloadUrl = value; }
        }

    }


}
