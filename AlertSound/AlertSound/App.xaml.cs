using AlertSound.Database;
using AlertSound.Services;
using System;
using System.IO;
using System.Timers;
using Xamarin.Forms;

namespace AlertSound
{
    public partial class App : Application
    {
        private static DataStore data;
        public static DataStore Data
        {
            get
            {
                if (data == null)
                {
                    var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var pathDatabase = Path.Combine(documentsFolder, "events_db.db3");

                    data = new DataStore(pathDatabase);
                }
                return data;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        private void onExceutorAlert(bool enable)
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += delegate
            {
                AlertExecutor.Excecutor();
            };

            aTimer.Interval = 60000;
            aTimer.Enabled = enable;
            aTimer.AutoReset = enable;
        }

        protected override void OnStart()
        {
             onExceutorAlert(true);
        }

        protected override void OnSleep()
        {
            onExceutorAlert(false);
        }

        protected override void OnResume()
        {
            onExceutorAlert(true);
        }
    }
}
