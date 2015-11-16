using System;
using System.Collections.Generic;
using System.Linq;
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

        // Отправка сообщений
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        // Подключение нового пользователя
        public void Connect(string Guid)
        {
            var id = Context.ConnectionId;


            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new UserMap { ConnectionId = id, Guid = Guid });

                // Посылаем сообщение текущему пользователю
                Clients.Caller.onConnected(id, Guid, Users);

                // Посылаем сообщение всем пользователям, кроме текущего
            }
        }

        // Отключение пользователя
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Guid);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}