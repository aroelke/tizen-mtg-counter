using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// Main program class.
	/// Loads the application and handles screen darkening.
	/// </summary>
	class Program : Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		/// <summary>
		/// Handle program creation, generating the app pages and load the main one.
		/// </summary>
		protected override void OnCreate()
		{
			base.OnCreate();
			LoadApplication(new Application { MainPage = new NavigationPage(new MainPage(new HistoryPage { StartingLife = 20 })) });
		}

		/// <summary>
		/// Handle termination, releasing the display lock so the watch can go dark to conserve battery.
		/// </summary>
		protected override void OnTerminate()
		{
			base.OnTerminate();
			DevicePower.ReleaseLock(DevicePower.DISPLAY);
		}

		/// <summary>
		/// Handle resume, requesting the display lock so the watch doesn't exit the app if not updated frequently enough.
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();
			DevicePower.RequestLock(DevicePower.DISPLAY, 0);
		}

		/// <summary>
		/// Handle pause, releasing the display lock if the app is manually paused (i.e. with the home button).
		/// This allows battery conservation while the app is closed.
		/// </summary>
		protected override void OnPause()
		{
			base.OnPause();
			DevicePower.ReleaseLock(DevicePower.DISPLAY);
		}

		static void Main(string[] args)
		{
			var app = new Program();
			Forms.Init(app);
			Tizen.Wearable.CircularUI.Forms.FormsCircularUI.Init();
			app.Run(args);
		}
	}
}
