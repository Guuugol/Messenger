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

        MessengerEntities DB = new MessengerEntities();

        
        [WebMethod]
        public string Authorize(string nickname, string password)
        {
            User user=DB.User.FirstOrDefault(u=>((u.Nickname==nickname)&&(u.Password==password)));
            JsonResult jsonResult;
            if (user==null)
            {
                 jsonResult=new JsonResult("invalid nickname/password");
                return JsonConvert.SerializeObject(jsonResult);
            }
            jsonResult = new JsonResult(new  List<Dictionary<string, object>>{new Dictionary<string, object>{{"UserId",user.ID}}});
            return JsonConvert.SerializeObject(jsonResult);
        }

        [WebMethod]
        public string Register(string nickname, string password, string firstName, string lastName, string info)
        {
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
                DB.User.Add(user);
                DB.SaveChanges();
            }
            catch (Exception)
            {
                jsonResult = new JsonResult("unknown error occured while adding a new record in data base");
                return JsonConvert.SerializeObject(jsonResult);
            }
            jsonResult = new JsonResult(new  List<Dictionary<string, object>>());
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}
