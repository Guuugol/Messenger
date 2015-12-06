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
        private readonly List<Dictionary<string, object>> _messageHistory;

        public HubConnection HubConnection;
        public IHubProxy HubProxy;
        
        public Chat(Guid userId ,Guid contactId)
        {
            InitializeComponent();
            ContactId = contactId;
            UserId = userId;
            _messageHistory = new List<Dictionary<string, object>>();
            _messageHistory = MainWindow.ServerClient.GetMessageHistory(UserId, ContactId);
            Refresh();
            StartConnection();

        }

        public void Refresh()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                foreach (var message in _messageHistory)
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

        public async void StartConnection()
        {
            HubConnection = new HubConnection("http://localhost:5661/signalr");
            HubProxy = HubConnection.CreateHubProxy("ChatHub");

            await HubConnection.Start();
            await HubProxy.Invoke("Connect", UserId.ToString());
            HubProxy.On<string, string>("addMessage", (senderGuid, message) =>
            {
                if (Guid.Parse(senderGuid) == ContactId)
                {
                    var fullText = ContactNick + ": ";
                    fullText += message;
                    this.Dispatcher.Invoke(() => ChatBlock.AppendText(fullText + "\n"));
                }
                else
                {
                    return;
                }
                
            });
        }

        async private void Send_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageBlock.Text;
            string fullText = "Я: " + messageText;
            ChatBlock.AppendText(fullText);

            /*Connection connection = new HubConnection("http://localhost:5661/signalr");*/

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            data.Add(new Dictionary<string, object>
            {
                {"fromId", UserId},
                {"toId", ContactId},
                {"text", messageText}
            });

            await HubProxy.Invoke("Send", new String[] {UserId.ToString(), ContactId.ToString(), messageText});

            MessageBlock.Clear();


        }



    }
}

