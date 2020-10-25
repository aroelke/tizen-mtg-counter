using System.Collections.Generic;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;

namespace TizenMtgCounter
{
	class Counter<K> : IRotaryEventReceiver
	{
		private IDictionary<K, CounterData<int>> data;
		private K selected;
		private int ticks;
		private readonly StackLayout content;
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

			content = new StackLayout {
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
					entry.TextColor = GetTextColor();
				}
				else
					entry.TextColor = Color.Transparent;
			}
		}

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
					entry.TextColor = GetTextColor();
				}
			}
		}

		public View Content { get => content; }

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
					entry.TextColor = GetTextColor();
				}
			}
		}

		public Color GetTextColor()
		{
			if (EqualityComparer<K>.Default.Equals(selected, default))
				return Color.Transparent;
			else
				return GetTextColor(selected);
		}

		public Color GetTextColor(K key)
		{
			return Data[key].GetTextColor();
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
