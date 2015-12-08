using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Server;
//using Client.Service_References.Server;
using Newtonsoft.Json;


namespace Client
{

    public class JsonResult
    {
        public JsonResult(IEnumerable<Dictionary<string, object>> data)
        {
            Success = true;
            this.Data = data;
        }

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

        public bool Success { get; set; }
        public string ErrorReason { get; set; }
        public IEnumerable<Dictionary<string, object>> Data { get; set; }
    }

    public class ServerClient
    {
        private ServiceSoapClient _serverClient;

        public ServerClient()
        {
            this._serverClient = new ServiceSoapClient();
        }

        public Guid? Authorize(string nickname, string password)
        {
            var json = _serverClient.Authorize(nickname, password);
            JsonResult res = JsonConvert.DeserializeObject<JsonResult>(json);
            if (res.Success)
            {
                //int index = json.IndexOf("UserId", StringComparison.Ordinal) + 2;
                //string result = json.Substring(index, 36);
                //return result;
                //res.data.First().ContainsKey("userId");
                Dictionary<string, object> dataDictionary = new Dictionary<string, object>(res.Data.First());
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
            var json = _serverClient.Register(nickname, password, firstName, lastName, info);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.Success;

        }

        public IEnumerable<Dictionary<string, object>> GetUserContacts(Guid userId)
        {
            var json = _serverClient.GetUserContacts(userId);
            JsonResult res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.Data;
        }

        public bool AddNewContact(string login, Guid userId, string nickname)
        {
            var json = _serverClient.AddNewContact(userId, login, nickname);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.Success;
        }


        public List<Dictionary<string, object>> GetMessageHistory(Guid userGuid, Guid contactGuid)
        {
            var json = _serverClient.GetMessageHistory(userGuid, contactGuid);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return (List<Dictionary<string, object>>) res.Data;
        }

        public bool DeleteContact(Guid userId, Guid contactId)
        {
            var json = _serverClient.DeleteContact(userId, contactId);
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.Success;
        }

        public bool DeleteMessageHistory(Guid userId, Guid contactId)
        {
            var json = _serverClient.DeleteMessageHistory(userId.ToString(), contactId.ToString());
            var res = JsonConvert.DeserializeObject<JsonResult>(json);
            return res.Success;
        }
    }
}
