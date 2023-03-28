using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ExcelDataReader;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Keycloak_User_Import
{
    class Program
    {
        public static string host = "https://auth.mykeycloak.com.tr/auth/";
        static void Main(string[] args)
        {
            // Excel dosyası bilgileri
            var excelFile = @"C:\users.xlsx";
            var excelSheetName = "Sheet1";

            string adminPassword = "admin";
            string apiUrl = host + "auth/admin/realms/MYREALM/users";

            // Excel dosyasından kullanıcı bilgilerini oku
            using (var excelStream = System.IO.File.Open(excelFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                int i = 0;
                using (var reader = ExcelReaderFactory.CreateReader(excelStream))
                {
                    while (reader.Read())
                    {
                        if (i++ == 0)
                            continue;
                        var email = reader.GetValue(0).ToString();
                        var password = reader.GetValue(1).ToString();
                        var firstName = reader.GetValue(2).ToString();
                        var lastName = reader.GetValue(3).ToString();


                        // Build the JSON data for the new user
                        JObject userData = new JObject();
                        userData.Add("firstName", firstName);
                        userData.Add("lastName", lastName);
                        userData.Add("email", email);
                        userData.Add("emailVerified", true);
                        userData.Add("enabled", true);
                        //userData.Add("username", username);

                        userData.Add("credentials", new JArray(
                            new JObject {
                        { "type", "password" },
                        { "value", password },
                        { "temporary", false }
                        }
                        ));

                        JObject attributes = new JObject(
                            new JProperty("attr1", new JArray(excelReadColumn(reader, 4))),
                            new JProperty("attr2", new JArray(excelReadColumn(reader, 5))),
                            new JProperty("attr3", new JArray(excelReadColumn(reader, 6))),
                            new JProperty("attr4", new JArray(excelReadColumn(reader, 7))),
                            new JProperty("attr5", new JArray(excelReadColumn(reader, 8)))
                        );
                        userData.Add("attributes", attributes);


                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        // Call the Keycloak API to create the user
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                        request.Method = "POST";
                        request.ContentType = "application/json";
                        request.Headers.Add("Authorization", "Bearer " + GetAccessToken(adminPassword));
                        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                        {
                            writer.Write(userData.ToString());
                        }

                        try
                        {
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            Console.WriteLine("User Created : " + email);
                        }
                        catch (WebException ex)
                        {
                            string responseContent = "";

                            Console.WriteLine("Error creating user: " + responseContent);
                        }
                    }
                }
            }


        }

        private static string excelReadColumn(IExcelDataReader reader, int index)
        {
            if (reader[index] != null)
                return reader.GetValue(index).ToString();
            return "";
        }

        static string GetAccessToken(string adminPassword)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string tokenUrl = host + "realms/master/protocol/openid-connect/token";
            string clientId = "admin-cli";
            string username = "admin";
            string password = adminPassword;

            // Build the request for an access token
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] postData = Encoding.UTF8.GetBytes(String.Format(
                "grant_type=password&client_id={0}&username={1}&password={2}", clientId, username, password
            ));
            request.ContentLength = postData.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            // Get the access token from the response
            try
            {

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseContent = "";
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();
                }
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                return jsonResponse.access_token;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
