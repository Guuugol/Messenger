using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Windows;
using Client;
using Client.Server;
using Microsoft.AspNet.SignalR.Client;


namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        public static ServerClient ServerClient = new ServerClient();

        public static Guid CurrentUserId;

        //public static List<User> Contacts; 
        //public static Connection Connection = new HubConnection();
        public static User User;
        //public Client.Server.ServiceSoapClient serverClient;
        //private static ServiceSoapClient cl = new ServiceSoapClient();  
       /* public App() : base()
        {
            Contacts = new List<User>();
        }*/


    }
}
