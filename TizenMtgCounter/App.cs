using System.Collections.Generic;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class App : Application
	{
		public App()
		{
			// The root page of your application
			MainPage = new MainPage
			{
				Life = 20,
				LifeThresholds = { (5, Color.Red), (10, Color.Orange) }
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
