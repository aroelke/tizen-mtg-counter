using System.Collections.Generic;
using System.Linq;
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
		private readonly Label label;
		private readonly Timer resetTicks;
		private readonly IDictionary<int, Color> colors;

		public MainPage(int start, IDictionary<int, Color> thresholds) : base()
		{
			life = start;
			ticks = 0;
			colors = thresholds;

			label = new Label
			{
				Text = start.ToString(),
				FontSize = 32
			};
			setLabelColor();

			resetTicks = new Timer
			{
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetTicks.Elapsed += (sender, e) => ticks = 0;

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

		public MainPage(int start) : this(start, new Dictionary<int, Color>()) {}

		private void setLabelColor()
		{
			var thresholds = colors.Keys.ToList();
			thresholds.Sort();
			thresholds.Reverse();

			label.TextColor = Color.White;
			foreach (int threshold in thresholds)
			{
				if (life <= threshold)
					label.TextColor = colors[threshold];
			}
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
			setLabelColor();

			resetTicks.Stop();
			resetTicks.Start();
		}
	}
}
