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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Enter.xaml
    /// </summary>
    public partial class Enter : Window
    {
        public new string Name = "EnterPage";

        private Guid _userId;

        public Enter()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text;
            string password = PbPassword.Password;
            Guid? result =  MainWindow.ServerClient.Authorize(login, password);
            if (result == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TbLogin.Clear();
                PbPassword.Clear();
            }
            else
            {
                _userId = (Guid) result;
                this.Hide();
                var contatcs = new MainWindow(_userId, login);
                contatcs.Activate();
                contatcs.Show();
                this.Close();
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            var register = new ContactInformation();
            this.Hide();
            register.Activate();
            register.ShowDialog();
            this.Show();
        }


    }
}
