using Xamarin.Forms;

namespace TizenMtgCounter
{
	class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			LoadApplication(new Application { MainPage = new NavigationPage(new MainPage(new HistoryPage { StartingLife = 20 })) });
		}

		protected override void OnTerminate()
		{
			base.OnTerminate();
			DevicePower.ReleaseLock(DevicePower.DISPLAY);
		}

		protected override void OnResume()
		{
			base.OnResume();
			DevicePower.RequestLock(DevicePower.DISPLAY, 0);
		}

		protected override void OnPause()
		{
			base.OnPause();
			DevicePower.ReleaseLock(DevicePower.DISPLAY);
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
