using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Realms.Sync;
using Realms;

namespace RealmDotnetTutorial
{
    public partial class App : Application
    {
        private const string appId = "tasktracker-gfend";
        public static Realms.Sync.App RealmApp;
        public static Realms.Sync.User gsky_RealmUser { get; set; }
        public static Realms.Sync.SyncConfiguration gsky_RealmSyncConfig { get; set; }
        public static Realm gsky_Realm { get; set; }
        public static string gsky_pk { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            RealmApp = Realms.Sync.App.Create(appId);
            if (App.RealmApp.CurrentUser == null)
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new ProjectPage());
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
