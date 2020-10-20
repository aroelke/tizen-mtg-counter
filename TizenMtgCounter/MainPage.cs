using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage, IRotaryEventReceiver
	{
		public const int TICK_THRESHOLD = 5;

		private int life;
		private int ticks;
		private Label label;
		private Timer resetThreshold;

		public MainPage()
		{
			life = 0;
			ticks = 0;

			label = new Label
			{
				Text = "0",
				FontSize = 32
			};

			resetThreshold = new Timer
			{
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetThreshold.Elapsed += (sender, e) => ticks = 0;

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					label
				}
			};

			RotaryFocusObject = this;
		}

		public void Rotate(RotaryEventArgs args)
		{
			if (args.IsClockwise)
			{
				if (ticks >= 0)
					ticks++;
				else
					ticks = 1;
				if (ticks <= TICK_THRESHOLD)
					life++;
				else
					life += TICK_THRESHOLD;
			}
			else
			{
				if (ticks <= 0)
					ticks--;
				else
					ticks = -1;
				if (ticks >= -TICK_THRESHOLD)
					life--;
				else
					life -= TICK_THRESHOLD;
			}
			label.Text = life.ToString();

			resetThreshold.Stop();
			resetThreshold.Start();
		}
	}
}
