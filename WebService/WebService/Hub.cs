using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using WebService;


namespace WebService
{

    public class UserMap
    {
        public string Guid { get; set; }
        public string ConnectionId { get; set; }
    }

    public class ChatHub : Hub
    {
        static List<UserMap> Users = new List<UserMap>();

        public void Send(string senderGuid,string recieverGuid, string message)
        {
            var client=Clients.Client(Users.First(u=>u.Guid==recieverGuid).ConnectionId);
            if(client!=null)
                client.addMessage(senderGuid, message);
            using (var DB=new MessengerEntities())
            {
                DB.Message.Add(new Message
                {
                    FromID = Guid.Parse(senderGuid),
                    ToID = Guid.Parse(recieverGuid),
                    Text = message,
                    ID = Guid.NewGuid()
                });
                DB.SaveChanges();

            }


    }

        public void Connect(string Guid)
        {
            var id = Context.ConnectionId;


            if (Users.All(x => x.ConnectionId != id))
            {
                Users.Add(new UserMap { ConnectionId = id, Guid = Guid });
            }
            Clients.Caller.onConnected(id, Guid, Users);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}