using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;


namespace WebService
{

    public class UserMap
    {
        public string Guid { get; set; }
        public string ConnectionId { get; set; }
    }

    public class ChatHub : Hub
    {
        public static readonly List<UserMap> Users = new List<UserMap>();

        public void Send(string senderGuid,string recieverGuid, string message)
        {
            var client=Clients.Client(Users.First(u=>u.Guid==recieverGuid).ConnectionId);
            bool recieved;
            if (client != null)
            {
                client.addMessage(senderGuid, message);
                recieved = true;
            }
            else
                recieved = false;
            using (var db=new MessengerEntities())
            {
                db.Message.Add(new Message
                {
                    FromID = Guid.Parse(senderGuid),
                    ToID = Guid.Parse(recieverGuid),
                    Text = message,
                    ID = Guid.NewGuid(),
                    Recieved = (byte)(recieved?1:0)

                });
                db.SaveChanges();

            }


    }

        public void Connect(string guid)
        {
            var id = Context.ConnectionId;


            if (Users.All(x => x.ConnectionId != id))
            {
                Users.Add(new UserMap { ConnectionId = id, Guid = guid });
            }
            Clients.Caller.onConnected(id, guid, Users);
            Guid userGuid = Guid.Parse(guid);
            using (var db = new MessengerEntities())
            {
                foreach (var reciever in (from u in db.User
                                          from c in db.Contact
                                          where ((u.ID == c.UserID) && (c.ContactID == userGuid))
                                          select u))
                {
                    string recieverGuid = reciever.ID.ToString();
                    var recieverMapRecord = Users.FirstOrDefault(u => u.Guid == recieverGuid);
                    if (recieverMapRecord != null)
                    {
                        string recieverConnectionId = recieverMapRecord.ConnectionId;
                        var recieverHubDescriptor = Clients.Client(recieverConnectionId);
                        if (recieverHubDescriptor != null)
                        {
                            recieverHubDescriptor.onContactConnected(guid);
                        }
                    }
                }
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                
                Guid userGuid=Guid.Parse(item.Guid);
                using (var db = new MessengerEntities())
                {
                    foreach (var reciever in (from u in db.User
                        from c in db.Contact
                        where ((u.ID == c.UserID) && (c.ContactID == userGuid))
                        select u))
                    {
                        string recieverGuid = reciever.ID.ToString();
                        var recieverMapRecord = Users.FirstOrDefault(u => u.Guid == recieverGuid);
                        if (recieverMapRecord != null)
                        {
                            string recieverConnectionId = recieverMapRecord.ConnectionId;
                            var recieverHubDescriptor= Clients.Client(recieverConnectionId);
                            if (recieverHubDescriptor != null)
                            {
                                recieverHubDescriptor.onContactDisconnected(item.Guid);
                            }
                        }
                    }
                }
                Users.Remove(item);
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}