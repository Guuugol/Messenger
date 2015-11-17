using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WpfApplication1;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для AddNewContact.xaml
    /// </summary>
    public partial class AddNewContact : Window
    {

        public MainWindow MainWindow;

        public AddNewContact(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var login = TbLoginBox.Text;
            if (login == null)
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (MainWindow.Contacts.Any())
            {
                if (MainWindow.Contacts.Any(user => user.Nickname == login))
                {
                    MessageBox.Show("Данный пользователь уже добавлен", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    TbLoginBox.Clear();
                }
            }
            var result = App.ServerClient.AddNewContact(login);
            if (!result)
            {
                MessageBox.Show("Невозможно добавиьт пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TbLoginBox.Clear();
            }
            else
            {
                MessageBox.Show("Контакт успешно добавлен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }


    }
}

