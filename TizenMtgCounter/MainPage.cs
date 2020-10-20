using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage, IRotaryEventReceiver
	{
		private int life;
		private int ticks;
		private readonly Label label;
		private readonly Timer resetTicks;

		public MainPage() : base()
		{
			life = 0;
			ticks = 0;

			label = new Label
			{
				Text = life.ToString(),
				FontSize = 32
			};

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
		public int Life
		{
			get { return life; }
			set
			{
				life = value;
				label.Text = life.ToString();

				var thresholds = ColorThresholds.Keys.ToList();
				thresholds.Sort();
				thresholds.Reverse();

				label.TextColor = Color.White;
				foreach (int threshold in thresholds)
				{
					if (life <= threshold)
						label.TextColor = ColorThresholds[threshold];
				}
			}
		}

		public int TickThreshold { get; set; } = 10;

		public int FastTickStep { get; set; } = 5;

		public IDictionary<int, Color> ColorThresholds { get; set; } = new Dictionary<int, Color>();

		public void Rotate(RotaryEventArgs args)
		{
			if (args.IsClockwise)
			{
				if (ticks >= 0)
					ticks++;
				else
					ticks = 1;
				if (ticks <= TickThreshold)
					Life++;
				else
					Life += FastTickStep;
			}
			else
			{
				if (ticks <= 0)
					ticks--;
				else
					ticks = -1;
				if (ticks >= -TickThreshold)
					Life--;
				else
					Life -= FastTickStep;
			}

			resetTicks.Stop();
			resetTicks.Start();
		}
	}
}
