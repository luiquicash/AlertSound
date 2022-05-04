using AlertSound.Database;
using System;
using System.IO;
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

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
