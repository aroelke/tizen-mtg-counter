using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.WebView;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Label = Xamarin.Forms.Label;

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
			RepeatButton plusButton = new RepeatButton
			{
				Text = "+",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			plusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);
			RepeatButton minusButton = new RepeatButton
			{
				Text = "\u2212",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			minusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);

			ImageButton poisonButton = new ImageButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			Label poisonCounter = new Label
			{
				Text = "0",
				FontSize = 10,
				TextColor = Color.Gray
			};

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			StackLayout counterLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Spacing = -24,
				Children = {
					plusButton,
					counter,
					minusButton
				}
			};
			layout.Children.Add(
				counterLayout,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counterLayout).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counterLayout).Height)/2)
			);
			layout.Children.Add(
				poisonButton,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 1)/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 1)/2)
			);
			layout.Children.Add(
				poisonCounter,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 3)/2 - getSize(poisonCounter).Width/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 3)/2 - getSize(poisonCounter).Height/2)
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

			plusButton.Pressed += (sender, e) => Life++;
			plusButton.Held += (sender, e) => Life++;
			minusButton.Pressed += (sender, e) => Life--;
			minusButton.Held += (sender, e) => Life--;
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
					if (life <= threshold)
						counter.TextColor = ColorThresholds[threshold];
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
