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
    /// Логика взаимодействия для Enter.xaml
    /// </summary>
    public partial class Enter : Window
    {
        public new string Name = "EnterPage";

        public Enter()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = pbPassword.Password;
            Guid? result =  App.ServerClient.Authorize(login, password);
            if (result == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                tbLogin.Clear();
                pbPassword.Clear();
            }
            else
            {
                App.CurrentUserId = (Guid) result;
                this.Hide();
                var contatcs = new MainWindow();
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
