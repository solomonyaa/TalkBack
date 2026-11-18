using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Talkback.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var config = File.ReadAllText("config.json");
            var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(config);

            MainWindow window = new MainWindow(cfg.clientUrl);
            window.Show();


        }
    }
}
