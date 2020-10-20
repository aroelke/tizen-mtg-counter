using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage, IRotaryEventReceiver
	{
		private int life;
		private int ticks;
		private readonly Label counter;
		private readonly Timer resetTicks;

		public MainPage() : base()
		{
			life = 0;
			ticks = 0;

			counter = new Label
			{
				Text = life.ToString(),
				FontSize = 32,
				HorizontalOptions = LayoutOptions.Center
			};
			Label plus = new Label
			{
				Text = "+",
				FontSize = 18,
				HorizontalOptions = LayoutOptions.Center
			};
			Label minus = new Label
			{
				Text = "\u2212",
				FontSize = 18,
				HorizontalOptions = LayoutOptions.Center
			};

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			StackLayout counterLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Spacing = -24,
				Children = {
					plus,
					counter,
					minus
				}
			};
			layout.Children.Add(
				counterLayout,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counterLayout).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counterLayout).Height)/2)
			);
			Content = layout;

			RotaryFocusObject = this;
			resetTicks = new Timer
			{
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetTicks.Elapsed += (sender, e) => ticks = 0;

			Button button = new Button();

			TapGestureRecognizer increment = new TapGestureRecognizer();
			increment.Tapped += (sender, e) => Life++;
			plus.GestureRecognizers.Add(increment);

			TapGestureRecognizer decrement = new TapGestureRecognizer();
			decrement.Tapped += (sender, e) => Life--;
			minus.GestureRecognizers.Add(decrement);
		}
		public int Life
		{
			get { return life; }
			set
			{
				life = value;
				counter.Text = life.ToString();

				var thresholds = ColorThresholds.Keys.ToList();
				thresholds.Sort();
				thresholds.Reverse();

				counter.TextColor = Color.White;
				foreach (int threshold in thresholds)
				{
					if (life <= threshold)
						counter.TextColor = ColorThresholds[threshold];
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
