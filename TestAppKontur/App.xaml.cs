using Prism;
using Prism.Ioc;
using System;
using System.Diagnostics;
using TestAppKontur.Dependency;
using TestAppKontur.Models.Dao;
using TestAppKontur.ViewModels;
using TestAppKontur.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAppKontur
{
    public partial class App
    {
        public const string DBFILENAME = "test_db_kontur.db";
        /*
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor.
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            string dbPath = DependencyService.Get<IPath>().GetDatabasePath(DBFILENAME);
            Dao.GetInstance(dbPath);

            InitializeComponent();

            var result = await NavigationService.NavigateAsync("NavigationPage/MainPage");            

            if (!result.Success)
            {
                Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ContactPage, ContactPageViewModel>();
        }
    }
}
