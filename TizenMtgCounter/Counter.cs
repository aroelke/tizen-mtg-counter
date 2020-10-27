using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Label = Xamarin.Forms.Label;

namespace TizenMtgCounter
{
	public class Counter<K> : IRotaryEventReceiver
	{
		private IDictionary<K, CounterData<int>> data;
		private K selected;
		private int ticks;
		private readonly CounterPopupEntry entry;
		private readonly Timer resetTicks;

		public Counter() : base()
		{
			data = new Dictionary<K, CounterData<int>>();
			selected = default;
			ticks = 0;

			entry = new CounterPopupEntry {
				Text = "",
				FontSize = 32,
				TextColor = Color.Transparent,
				Keyboard = Keyboard.Numeric,
				BackgroundColor = Color.Transparent,
				HorizontalTextAlignment = TextAlignment.Center
			};
			RepeatButton plusButton = new RepeatButton {
				Text = "+",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			plusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);
			RepeatButton minusButton = new RepeatButton {
				Text = "\u2212",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			minusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);

			resetTicks = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetTicks.Elapsed += (sender, e) => ticks = 0;

			entry.Completed += (sender, e) => {
				if (!EqualityComparer<K>.Default.Equals(selected, default) && int.TryParse(entry.Text, out int result))
					this[selected] = result;
			};

			plusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => { if (!EqualityComparer<K>.Default.Equals(selected, default)) this[Selected]++; });
			plusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => { if (!EqualityComparer<K>.Default.Equals(selected, default)) this[Selected]++; });
			minusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => { if (!EqualityComparer<K>.Default.Equals(selected, default)) this[Selected]--; });
			minusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => { if (!EqualityComparer<K>.Default.Equals(selected, default)) this[Selected]--; });

			Content = new StackLayout {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Spacing = -24,
				Children = { plusButton, entry, minusButton }
			};
		}

		public IDictionary<K, CounterData<int>> Data
		{
			get => data;
			set
			{
				data = value;

				if (data.ContainsKey(selected))
				{
					entry.Text = this[selected].ToString();
					entry.TextColor = GetTextColor(selected);
				}
				else
					entry.TextColor = Color.Transparent;

				Labels = value.Select((p) => new KeyValuePair<K, Label>(p.Key, new Label {
					FontSize = 8
				})).ToDictionary((p) => p.Key, (p) => p.Value);
				foreach (K key in data.Keys)
				{
					Labels[key].SetBinding(Label.TextProperty, "Value");
					Labels[key].SetBinding(Label.TextColorProperty, "TextColor");
					Labels[key].BindingContext = value[key];
				}
			}
		}

		public IDictionary<K, Label> Labels { get; private set; } = new Dictionary<K, Label>();

		public K Selected
		{
			get => selected;
			set
			{
				selected = value;
				if (EqualityComparer<K>.Default.Equals(value, default))
					entry.TextColor = Color.Transparent;
				else
				{
					entry.Text = this[value].ToString();
					entry.TextColor = GetTextColor(value);
				}
			}
		}

		public View Content { get; private set; }

		public int TickThreshold { get; set; } = 10;

		public int FastTickStep { get; set; } = 5;

		public int this[K key]
		{
			get => Data[key].Value;
			set
			{
				Data[key].Value = value;
				if (EqualityComparer<K>.Default.Equals(selected, key))
				{
					entry.Text = value.ToString();
					entry.TextColor = GetTextColor(selected);
				}
			}
		}

		public Color GetTextColor(K key)
		{
			foreach ((int threshold, Color color) in data[key].Thresholds)
				if (data[key].Value <= threshold)
					return color;
			return Color.Default;
		}

		public void Rotate(RotaryEventArgs args)
		{
			if (!EqualityComparer<K>.Default.Equals(selected, default))
			{
				if (args.IsClockwise)
				{
					if (ticks >= 0)
						ticks++;
					else
						ticks = 1;
					if (ticks <= TickThreshold)
						this[selected]++;
					else
						this[selected] += FastTickStep;
				}
				else
				{
					if (ticks <= 0)
						ticks--;
					else
						ticks = -1;
					if (ticks >= -TickThreshold)
						this[selected]--;
					else
						this[selected] -= FastTickStep;
				}

				resetTicks.Stop();
				resetTicks.Start();
			}
		}
	}
}
