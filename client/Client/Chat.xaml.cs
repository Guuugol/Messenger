using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace Client
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
        private MainWindow parent;

        public HubConnection HubConnection;
        public IHubProxy HubProxy;
        
        public Chat(Guid userId ,Guid contactId, string contactNickname, MainWindow parentMainWindow)
        {
            InitializeComponent();
            ContactId = contactId;
            UserId = userId;
            ContactNick = contactNickname;
            NameLabel.Content = contactNickname;
            parent = parentMainWindow;
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
                    ChatBlock.ScrollToEnd();
                }
            }));

        }

        public async void StartConnection()
        {
            HubConnection = new HubConnection("http://localhost:5661/signalr");
            HubProxy = HubConnection.CreateHubProxy("ChatHub");

            await HubConnection.Start();
            await HubProxy.Invoke("Connect", UserId.ToString());
           /* HubProxy.On<string, string>("addMessage", (senderGuid, message) =>
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
                
            });*/
        }

        async private void Send_Click(object sender, RoutedEventArgs e)
        {
            string messageText = MessageBlock.Text;
            string fullText = "Я: " + messageText;
            ChatBlock.AppendText(fullText + "\n");
            ChatBlock.ScrollToEnd();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            parent.ChatStartedGuids.Remove(ContactId);
            parent.ChatWindows.Remove(this);
        }

        public void AppendText(string text)
        {
            this.ChatBlock.AppendText(text);
            this.ChatBlock.ScrollToEnd();
        }


    }
}

