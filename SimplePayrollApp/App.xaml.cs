namespace SimplePayrollApp
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new NavigationPage(serviceProvider.GetService<Views.MainPage>());
        }

        protected override void OnStart() { }
        protected override void OnSleep() { }
        protected override void OnResume() { }
    }
}
