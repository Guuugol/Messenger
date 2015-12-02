using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Cors;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
[assembly: OwinStartup(typeof(WebService.Startup))]

namespace WebService
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }



    class JsonResult
    {

        public bool success { get; set; }
        public string errorReason { get; set; }
        public IEnumerable<Dictionary<string, object>> data { get; set; }

        public JsonResult(string errorReason)
        {
            this.errorReason = errorReason;
            success = false;
        }
        public JsonResult(IEnumerable<Dictionary<string, object>> data)
        
        {
            success = true;
            this.data = data;
        }

        public JsonResult()
        {
            success = false;
        }
        
    }


    public class Service : System.Web.Services.WebService
    {

        private readonly MessengerEntities _db = new MessengerEntities();


        [WebMethod]
        public string Authorize(string nickname, string password)
        {
            User user = _db.User.FirstOrDefault(u => ((u.Nickname == nickname) && (u.Password == password)));
            JsonResult jsonResult;
            if (user == null)
            {
                jsonResult = new JsonResult("invalid nickname/password");
                return JsonConvert.SerializeObject(jsonResult);
            }
            jsonResult = new JsonResult(new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"userId", user.ID},
                    {"firstName", user.FirstName}, 
                    {"lastName", user.LastName},
                    {"info", user.Info}
                }
            });
            return JsonConvert.SerializeObject(jsonResult);
        }

        [WebMethod]
        public string Register(string nickname, string password, string firstName, string lastName, string info)
        {
            if (_db.User.Any() && _db.User.Any(u => u.Nickname == nickname))
                return JsonConvert.SerializeObject(new JsonResult("another user have the same nickname"));
            User user = new User
            {
                Nickname = nickname,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Info = info,
                ID = Guid.NewGuid()
            };
            JsonResult jsonResult;
            try
            {
                _db.User.Add(user);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                jsonResult = new JsonResult("unknown error occured while adding a new record in data base");
                return JsonConvert.SerializeObject(jsonResult);
            }
            jsonResult = new JsonResult(new List<Dictionary<string, object>>());
            return JsonConvert.SerializeObject(jsonResult);
        }

        [WebMethod]
        public string GetUserContacts(Guid guid)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            var contacts = from u in _db.User
                from c in _db.Contact
                where ((u.ID == c.ContactID) && (c.UserID == guid))
                select u;
            foreach (var contact in contacts)
            {
                string userGuid = contact.ID.ToString();
                data.Add(new Dictionary<string, object>
                {
                    {"userId", contact.ID},
                    {"firstName", contact.FirstName},
                    {"lastName", contact.LastName},
                    {"info", contact.Info},
                    {"nickname", contact.Nickname},
                    {"online",ChatHub.Users.Any(u=>u.Guid==userGuid)}
                });
            }
            return JsonConvert.SerializeObject(new JsonResult(data));
        }
/*
        [WebMethod]
        public string AddNewContact(Guid userGuid, string contactNickname, string name)
        {
            try
            {
                User contact = DB.User.First(u => u.Nickname == contactNickname);
                DB.Contact.Add(new Contact
                {
                    UserID = userGuid,
                    ContactID = contact.ID,
                    Name = name
                });
                return JsonConvert.SerializeObject(new JsonResult(new List<Dictionary<string, object>>()));
            }
            catch (Exception)
            {
                return
                    JsonConvert.SerializeObject(
                        new JsonResult("unknown error occured while adding a new record in data base"));
            }
        }*/

        [WebMethod]
        public string AddNewContact(Guid userId,string contactNickname)
        {
            try
            {
                User contact = _db.User.First(u => u.Nickname == contactNickname);
                _db.Contact.Add(new Contact
                {
                    ContactID = contact.ID,
                    UserID = userId,
                    ID = Guid.NewGuid()
                });
                _db.SaveChanges();
                return JsonConvert.SerializeObject(new JsonResult(new List<Dictionary<string, object>>()));
            }
            catch (Exception)
            {
                return
                    JsonConvert.SerializeObject(
                        new JsonResult("unknown error occured while adding a new record in data base"));
            }
        }

        [WebMethod]
        public string DeleteContact(Guid userGuid, Guid contactGuid)
        {
            try
            {
                _db.Contact.Remove(_db.Contact.First(c => ((c.UserID == userGuid) && (c.ContactID == contactGuid))));
                return JsonConvert.SerializeObject(new JsonResult(new List<Dictionary<string, object>>()));
            }
            catch (Exception)
            {
                return
                    JsonConvert.SerializeObject(
                        new JsonResult("unknown error occured while adding a new record in data base"));
            }

        }

        [WebMethod]
        public string GetMessageHistory(Guid userGuid, Guid contactGuid)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            try
            {
                foreach (
                    var message in
                        _db.Message.Where(
                            m =>
                                (((m.FromID == userGuid) && (m.ToID == contactGuid)) ||
                                 ((m.ToID == userGuid) && (m.FromID == contactGuid)))).OrderBy(m=>m.Date_time))
                {
                    data.Add(new Dictionary<string, object>
                    {
                        {"fromId", message.FromID},
                        {"toId", message.ToID},
                        {"text", message.Text},
                        {"recieved",message.Recieved}
                    });
                }
                return JsonConvert.SerializeObject(new JsonResult(data));
            }
            catch (Exception)
            {
                return
                    JsonConvert.SerializeObject(
                        new JsonResult("unknown error occured while adding a new record in data base"));
            }

        }


        [WebMethod]
        public string GetUserByGuid(Guid guid)
        {
            try
            {
                User user = _db.User.First(u => u.ID == guid);
                List<Dictionary<string,object>> data=new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"userId",user.ID},
                        {"firstName",user.FirstName},
                        {"lastName",user.LastName},
                        {"nickname",user.Nickname},
                        {"info",user.Info}
                    }
                };
                return JsonConvert.SerializeObject(new JsonResult(data));
            }
            catch
            {
                return
                    JsonConvert.SerializeObject(
                        new JsonResult("unknown error occured while adding a new record in data base"));
            }
        }
    }
}

