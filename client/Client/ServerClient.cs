using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Server;
using Newtonsoft.Json;


namespace Client
{

    public class JsonResult
    {
       /* [JsonConstructor]
        public JsonResult(string json)
        {
            success = !json.Contains("false");
        }*/

        /*public JsonResult(Dictionary<string, object> data)
        {
            success = true;
            error = null;
            this.data = data;
        }*/

        public bool success { get; set; }
        public string errorReason { get; set; }
        public IEnumerable<Dictionary<string, object>> data { get; set; }
    }

    public class ServerClient
    {
        private Client.Server.ServiceSoapClient serverClient;

        public ServerClient()
        {
            this.serverClient = new ServiceSoapClient();
        }

        public Guid? Authorize(string nickname, string password)
        {
            var json = serverClient.Authorize(nickname, password);
            JsonResult res = JsonConvert.DeserializeObject<JsonResult>(json);
            if (res.success)
            {
                //int index = json.IndexOf("UserId", StringComparison.Ordinal) + 2;
                //string result = json.Substring(index, 36);
                //return result;
                //res.data.First().ContainsKey("userId");
                Dictionary<string, object> dataDictionary = new Dictionary<string, object>(res.data.First());
                string idString = (string) dataDictionary["userId"];
                Guid id = Guid.Parse(idString);
                return id;
            }
            else
            {
                
                return null;
            }
        }

        public bool Register(string nickname, string password, string firstName, string lastName, string info)
        {
            var json = serverClient.Register(nickname, password, firstName, lastName, info);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.success;

        }

        public IEnumerable<Dictionary<string, object>> GetUserContacts(Guid userId)
        {
            var json = serverClient.GetUserContacts(userId);
            JsonResult res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.data;
        }

        public bool AddNewContact(string login)
        {
            var json = serverClient.AddNewContact(login);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.success;
        }
    }
}
