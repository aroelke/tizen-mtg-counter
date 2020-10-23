using System;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			LoadApplication(new Application
			{
				MainPage = new MainPage
				{
					Life = 20,
					LifeThresholds = { (5, Color.Red), (10, Color.Orange) }
				}
			});
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
