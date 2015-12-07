using System.Windows;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для ContactInformation.xaml
    /// </summary>
    public partial class ContactInformation : Window
    {
        public ContactInformation()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string firstName = FirstName.Text;
            string lastName = LastName.Text;
            string password = Password.Password;
            string confirmPassword = ConfirmPassword.Password;
            string info = Information.Text;

            if (login == null || firstName == null || lastName == null || password == null || confirmPassword == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
          
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Password.Clear();
                ConfirmPassword.Clear();
                return;
            }

            bool res =  MainWindow.ServerClient.Register(login, password, firstName, lastName, info);

            if (!res)
            {
                MessageBox.Show("Ошибка регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Password.Clear();
                ConfirmPassword.Clear();
                FirstName.Clear();
                Login.Clear();
                LastName.Clear();
                Information.Clear();
            }
            else
            {
                MessageBox.Show("Успешная регистрация", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
