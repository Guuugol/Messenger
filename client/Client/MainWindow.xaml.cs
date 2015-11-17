using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    public partial class MainWindow : Window
    {

        public static List<User> Contacts;
        public Guid CurrentUserId;

        public void RefreshContacts()
        {
            var data = App.ServerClient.GetUserContacts(CurrentUserId);
            Contacts.Clear();
           
            foreach (var user in data.Select(dict => new User
            {
                ID = Guid.Parse((string)dict["userId"]),
                Nickname = (string)dict["nickname"],
                FirstName = (string)dict["firstName"],
                LastName = (string)dict["lastName"],
                Info = (string)dict["info"]
            }))
            {
                Contacts.Add(user);
            }

            foreach (var user in Contacts)
            {
                
            }
            
        }

        
        public MainWindow(Guid userId)
        {
            CurrentUserId = userId;
            InitializeComponent();
            Contacts = new List<User>();
            RefreshContacts();
            //ContactGrid.DataContext = App.Contacts;
            ContactGrid.ItemsSource = Contacts;
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var addNewContact = new AddNewContact(this, CurrentUserId);
            addNewContact.ShowDialog();
            RefreshContacts();

        }

    }
}
