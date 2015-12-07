using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Client
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class UserContact
    {

        public UserContact(UserContact selectedItem)
        {
            Id = selectedItem.Id;
            Nickname = selectedItem.Nickname;
            FirstName = selectedItem.FirstName;
            LastName = selectedItem.LastName;
            Online = selectedItem.Online;

        }

        public UserContact(Guid id, string nickname, string firstName, string lastName, bool online)
        {
            Id = id;
            Nickname = nickname;
            FirstName = firstName;
            LastName = lastName;
            Online = online ? "online" : "offline";

        }

        public UserContact()
        {
            throw new NotImplementedException();
        }

        public System.Guid Id { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Online { get; set; }
        
    }
    
    public partial class MainWindow : Window
    {
        public static ServerClient ServerClient = new ServerClient();
        
        
        public static List<User> Contacts;
        public Guid CurrentUserId;
        public  ObservableCollection<UserContact> NicknameList;
        public List<Guid> ChatStartedGuids;
        public List<Chat> ChatWindows; 

       //   
        public HubConnection HubConnection;
        public IHubProxy HubProxy;

        public async void StartConnection()
        {
            HubConnection = new HubConnection("http://localhost:5661/signalr");
            HubProxy = HubConnection.CreateHubProxy("ChatHub");

            await HubConnection.Start();
            await HubProxy.Invoke("Connect", CurrentUserId.ToString());
            HubProxy.On<string, string>("addMessage", (senderGuid, message) =>
            {
                Guid sender = Guid.Parse(senderGuid);
                string nickName = "somebody";
                if (ChatStartedGuids.Exists(x => x == sender))
                {
                    foreach (UserContact user in NicknameList)
                    {
                        if (user.Id == sender)
                        {
                            nickName = user.Nickname;
                        }
                    }
                    bool sended = false;
                    foreach (var chat in ChatWindows)
                    {
                        if (chat.ContactId == Guid.Parse(senderGuid))
                        {
                            var fullText = nickName + ": ";
                            fullText += message;
                            var chat1 = chat;
                            this.Dispatcher.Invoke(() => chat1.AppendText(fullText + "\n"));
                            sended = true;
                        }
                    }
                    /*if (!sended)
                    {
                        string text = "Вы получили новое сообщение от " + nickName;
                        MessageBox.Show(text, "Новое сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    }*/
                    
                }
                else
                {
                    foreach (UserContact user in NicknameList)
                    {
                        if (user.Id == sender)
                        {
                            nickName = user.Nickname;
                        }
                    }
                    string text = "Вы получили новое сообщение от " + nickName;
                    MessageBox.Show(text, "Новое сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            });
            HubProxy.On<string>("onContactConnected", (guid) =>
            {
                Dispatcher.Invoke(RefreshContacts);
            });
            HubProxy.On<string>("onContactDisconnected", (guid) =>
            {
                Dispatcher.Invoke(RefreshContacts);
            });
        }

        public void RefreshContacts()
        {
            var data = ServerClient.GetUserContacts(CurrentUserId);
            Contacts.Clear();
            NicknameList.Clear();
            foreach (var user in data.Select(dict => new User
            {
                Id = Guid.Parse((string)dict["userId"]),
                Nickname = (string)dict["nickname"],
                FirstName = (string)dict["firstName"],
                LastName = (string)dict["lastName"],
                Info = (string)dict["info"],
                Online = (bool)dict["online"]
            }))
            {
                Contacts.Add(user);
            }

            foreach (var user in Contacts)
            {
                NicknameList.Add(new UserContact(user.Id, user.Nickname, user.FirstName, user.LastName, user.Online));
            }
            ((ArrayList)ContactListView.Resources["UserContacts"]).AddRange(NicknameList);
            ContactListView.ItemsSource = NicknameList;
        }

        
        public MainWindow(Guid userId)
        {
            CurrentUserId = userId;
            InitializeComponent();
            Contacts = new List<User>();
            NicknameList = new ObservableCollection<UserContact>();
            ChatStartedGuids = new List<Guid>();
            ChatWindows = new List<Chat>();

            RefreshContacts();
            StartConnection();

        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var addNewContact = new AddNewContact(this, CurrentUserId);
            addNewContact.ShowDialog();
            RefreshContacts();

        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ContactListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ContactListView.SelectedItem == null)
                return;
            UserContact contact = new UserContact((UserContact)ContactListView.SelectedItem);
            string login = contact.Nickname;
            foreach (var cont in Contacts)
            {
                if (cont.Nickname == login)
                {
                    var chat = new Chat(CurrentUserId, cont.Id, cont.Nickname, this);
                    ChatStartedGuids.Add(cont.Id);
                    chat.Uid = cont.Id.ToString();
                    ChatWindows.Add(chat);
                    chat.Show();
                    ContactListView.SelectedItem = null;
                    break;
                }
            }
        }

        private void ContactList_Closed(object sender, EventArgs e)
        {
            HubConnection.Stop();
        } 
    }
}
