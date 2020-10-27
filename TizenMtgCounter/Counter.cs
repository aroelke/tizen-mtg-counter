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
					Text = value[p.Key].Value.ToString(),
					FontSize = 8,
					TextColor = GetTextColor(p.Key)
				})).ToDictionary((p) => p.Key, (p) => p.Value);
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

		public CounterReference<K> this[K key]
		{
			get => new CounterReference<K>(Data[key].Value, key, this);
			set
			{
				Data[key].Value = value;
				Labels[key].Text = value.ToString();
				Labels[key].TextColor = GetTextColor(key);
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

	public readonly struct CounterReference<K>
	{
		public static CounterReference<K> operator +(CounterReference<K> o, int i) => new CounterReference<K>(o.n + i, o.key, o.counter);
		public static CounterReference<K> operator ++(CounterReference<K> o) => o + 1;
		public static CounterReference<K> operator -(CounterReference<K> o, int i) => o + -i;
		public static CounterReference<K> operator --(CounterReference<K> o) => o - 1;

		public static implicit operator int(CounterReference<K> r) => r.n;
		public static implicit operator CounterReference<K>(int i) => new CounterReference<K>(i, default, default);

		private readonly int n;
		private readonly K key;
		private readonly Counter<K> counter;

		public CounterReference(int i, K k, Counter<K> c)
		{
			n = i;
			key = k;
			counter = c;

			if (!EqualityComparer<Counter<K>>.Default.Equals(c, default))
				c[k] = this;
		}

		public override string ToString() => n.ToString();
	}
}
