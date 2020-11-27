using Xamarin.Forms;

namespace TizenMtgCounter
{
	class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			LoadApplication(new Application { MainPage = new NavigationPage(new MainPage(
				new CommanderPage(),
				new ManaPage(),
				new AdditionalPage(),
				new HistoryPage { StartingLife = 20 }
			)) });
		}

		static void Main(string[] args)
		{
			var app = new Program();
			Forms.Init(app);
			global::Tizen.Wearable.CircularUI.Forms.FormsCircularUI.Init();
			app.Run(args);
		}
	}
}
