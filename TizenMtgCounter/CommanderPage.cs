using System;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CommanderPage : CounterPage<int>
	{
		public CommanderPage() : base()
		{
			DarkenButton createTaxButton()
			{
				int tax = 0;
				bool clear = false;
				DarkenButton button = new DarkenButton {
					Source = "0_mana.png",
					WidthRequest = 50,
					HeightRequest = 50,
					CornerRadius = 25
				};
				Timer timer = new Timer {
					Interval = 500,
					Enabled = false,
					AutoReset = false
				};
				timer.Elapsed += (sender, e) => clear = true;

				button.Pressed += (sender, e) => timer.Start();
				button.Released += (sender, e) => {
					timer.Stop();
					tax = clear ? 0 : tax + 2;
					clear = false;
					button.Source = tax <= 20 ? $"{tax}_mana.png" : "infinity_mana.png";
				};

				return button;
			}

			DarkenButton left = createTaxButton();
			DarkenButton right = createTaxButton();
			Children.Add(left, (p) => (p.Width - p.GetSize(left).Width)/2, Math.PI);
			Children.Add(right, (p) => (p.Width - p.GetSize(right).Width)/2, 0);
		}
	}
}
