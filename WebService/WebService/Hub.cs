using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebService
{
    [HubName("msg")]
    public class Messenger : Hub
    {
        
        public Task Send(dynamic message)
        {
            return Clients.All.SendMessage(message);
        }

        public void Register(Guid userId)
        {
            
            Groups.Add(Context.ConnectionId, userId.ToString());
        }

        public Task ToGroup(dynamic id, string message)
        {
            return Clients.Group(id.ToString()).SendMessage(message);
        }
    }

}
