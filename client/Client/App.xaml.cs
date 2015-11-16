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


namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 
    
    public partial class App : Application
    {
        public static ServerClient serverClient = new ServerClient();

        public static Guid currentUserId;
        //public Client.Server.ServiceSoapClient serverClient;
        //private static ServiceSoapClient cl = new ServiceSoapClient();  



    }
}
