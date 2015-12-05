using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        public Guid ContactId;
        public Guid UserId;
        public String ContactNick;
        private List<Dictionary<string, object>> MessageHistory;

        public HubConnection hubConnection;
        public IHubProxy hubProxy;
        
        public Chat(Guid userId ,Guid contactId)
        {
            InitializeComponent();
            ContactId = contactId;
            UserId = userId;
            MessageHistory = new List<Dictionary<string, object>>();
            MessageHistory = MainWindow.ServerClient.GetMessageHistory(UserId, ContactId);
            refresh();
            startConnection();

        }

        public void refresh()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                foreach (var message in MessageHistory)
                {
                    Guid from = Guid.Parse((string) message["fromId"]);
                    Guid to = Guid.Parse((string) message["toId"]);
                    string text = (string) message["text"];
                    var recieved = message["recieved"];
                    var fullText = from == ContactId ? ContactNick + ": " : "Я: ";
                    fullText += text;
                    ChatBlock.AppendText(fullText + "\n");

                }
            }));

        }

        public async void startConnection()
        {
            hubConnection = new HubConnection("http://localhost:5661/signalr");
            hubProxy = hubConnection.CreateHubProxy("ChatHub");

            await hubConnection.Start();
            await hubProxy.Invoke("Connect", UserId.ToString());
            hubProxy.On<string, string>("addMessage", (senderGuid, message) =>
            {
                var fullText = "Я: ";
                fullText += message;
                this.Dispatcher.Invoke(()=> ChatBlock.AppendText(fullText + "\n") );
            });
        }

        async private void Send_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageBlock.Text;
            ChatBlock.AppendText(messageText);

            /*Connection connection = new HubConnection("http://localhost:5661/signalr");*/

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            data.Add(new Dictionary<string, object>
            {
                {"fromId", UserId},
                {"toId", ContactId},
                {"text", messageText}
            });

            await hubProxy.Invoke("Send", new String[] {UserId.ToString(), ContactId.ToString(), messageText});

            MessageBlock.Clear();


        }



    }
}

