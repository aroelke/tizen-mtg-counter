using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Label = Xamarin.Forms.Label;

namespace TizenMtgCounter
{
	public class Counter<K> : IRotaryEventReceiver, INotifyPropertyChanged
	{
		private IDictionary<K, CounterData> data;
		private K selected;
		private int ticks;
		private readonly RepeatButton plusButton;
		private readonly RepeatButton minusButton;
		private readonly Timer resetTicks;
		private int changeInterval;
		private IDictionary<K, Timer> changeTimers;
		private IDictionary<K, int> oldValues;

		public Counter()
		{
			data = new Dictionary<K, CounterData>();
			selected = default;
			ticks = 0;
			changeInterval = 600;
			changeTimers = new Dictionary<K, Timer>();
			oldValues = new Dictionary<K, int>();

			plusButton = new RepeatButton {
				Text = "+",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			plusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);
			minusButton = new RepeatButton {
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

			plusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => Value++);
			plusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => Value++);
			minusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => Value--);
			minusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => Value--);
		}

		public IImmutableDictionary<K, CounterData> Data
		{
			get => data.ToImmutableDictionary();
			set
			{
				data = value.ToDictionary((e) => e.Key, (e) => e.Value);

				plusButton.IsVisible = minusButton.IsVisible = SelectedValid();

				changeTimers = value.ToDictionary((p) => p.Key, (p) => new Timer {
					Interval = changeInterval,
					Enabled = false,
					AutoReset = false
				});
				oldValues = value.ToDictionary((p) => p.Key, (p) => p.Value.Value);
				foreach (K key in changeTimers.Keys)
				{
					changeTimers[key].Elapsed += (sender, e) => {
						if (oldValues[key] != Data[key].Value)
							ValueChanged?.Invoke(this, new CounterChangedEventArgs<K> { Key = key, OldValue = oldValues[key], NewValue = Data[key].Value });
					};
				}

				if (SelectedValid())
				{
					OnPropertyChanged("Value");
					OnPropertyChanged("TextColor");
				}
			}
		}

		public K Selected
		{
			get => selected;
			set
			{
				if (!EqualityComparer<K>.Default.Equals(selected, value))
				{
					selected = value;
					plusButton.IsVisible = minusButton.IsVisible = !EqualityComparer<K>.Default.Equals(value, default);
					OnPropertyChanged("Value");
					OnPropertyChanged("TextColor");
					OnPropertyChanged("SelectedValid");
				}
			}
		}

		public int Value
		{
			get => SelectedValid() ? data[selected].Value : default;
			set
			{
				if (SelectedValid())
					this[selected] = value;
			}
		}

		public Color TextColor { get => SelectedValid() ? data[selected].TextColor : Color.Transparent; }

		public View Content { get; private set; }

		public int TickThreshold { get; set; } = 10;

		public int FastTickStep { get; set; } = 5;

		public int ChangeInterval
		{
			get => changeInterval;
			set
			{
				changeInterval = value;
				foreach (Timer t in changeTimers.Values)
					t.Interval = value;
			}
		}

		public int this[K key]
		{
			get => Data[key].Value;
			set
			{
				if (Data[key].Value != value)
				{
					if (!changeTimers[key].Enabled)
						oldValues[key] = Data[key].Value;
					else
						changeTimers[key].Stop();
					Data[key].Value = value;
					if (EqualityComparer<K>.Default.Equals(key, selected))
					{
						OnPropertyChanged("Value");
						OnPropertyChanged("TextColor");
					}
					changeTimers[key].Start();
				}
			}
		}

		public bool SelectedValid() => !EqualityComparer<K>.Default.Equals(selected, default) && data.ContainsKey(selected);

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
						Value++;
					else
						Value += FastTickStep;
				}
				else
				{
					if (ticks <= 0)
						ticks--;
					else
						ticks = -1;
					if (ticks >= -TickThreshold)
						Value--;
					else
						Value -= FastTickStep;
				}

				resetTicks.Stop();
				resetTicks.Start();
			}
		}

		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<CounterChangedEventArgs<K>> ValueChanged;
	}

	public class CounterChangedEventArgs<K> : EventArgs
	{
		public CounterChangedEventArgs() : base() {}

		public K Key { get; set; }
		public int OldValue { get; set; }
		public int NewValue { get; set; }
	}
}
