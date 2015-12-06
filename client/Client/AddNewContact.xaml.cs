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

        private readonly Guid _currentUserId;

        public AddNewContact(MainWindow mainWindow, Guid userId)
        {
            MainWindow = mainWindow;
            _currentUserId = userId;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var login = TbLoginBox.Text;
            var nickname = TbNicknameBox.Text;

            if (login == null || nickname == null)
            {
                MessageBox.Show("Заполните оба поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (MainWindow.Contacts.Any())
            {
                if (MainWindow.Contacts.Any(user => user.Nickname == login))
                {
                    MessageBox.Show("Данный пользователь уже добавлен", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    TbLoginBox.Clear();
                }
                 if (MainWindow.NicknameList.Any(user => user.Nickname == login))
                {
                    MessageBox.Show("Данный никнейм  уже занят", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    TbLoginBox.Clear();
                }
            }
            var result = MainWindow.ServerClient.AddNewContact(login, _currentUserId, nickname);
            if (!result)
            {
                MessageBox.Show("Невозможно добавить пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TbLoginBox.Clear();
            }
            else
            {
                MessageBox.Show("Контакт успешно добавлен", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void AddNewContact1_Closed(object sender, EventArgs e)
        {
            MainWindow.RefreshContacts();
        }


    }
}

