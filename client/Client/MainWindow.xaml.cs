using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client;

namespace WpfApplication1
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

       // 

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
            RefreshContacts();
           // ContactGrid.DataContext = Contacts;
            //ContactGrid.ItemsSource = NicknameList;
            
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
            UserContact contact = new UserContact((UserContact)ContactListView.SelectedItem);
            string login = contact.Nickname;
            foreach (var cont in Contacts)
            {
                if (cont.Nickname == login)
                {
                    Chat chat = new Chat(CurrentUserId, cont.Id);
                    chat.Show();
                    break;
                }
            }
        } 
    }
}
