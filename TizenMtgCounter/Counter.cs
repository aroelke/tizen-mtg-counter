using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// Backing data behind a <see cref="CounterPage{K}"/> containing the counts of the parts of the
	/// game state being tracked.
	/// </summary>
	/// <typeparam name="K">Type of data being tracked.</typeparam>
	public class Counter<K> : IRotaryEventReceiver, INotifyPropertyChanged
	{
		private IDictionary<K, CounterData> data;
		private K selected;
		private int ticks;
		private readonly Timer resetTicks;
		private int changeInterval;
		private IDictionary<K, Timer> changeTimers;
		private IDictionary<K, int> oldValues;

		/// <summary>
		/// Create a new counter with no tracking data.
		/// </summary>
		public Counter()
		{
			data = new Dictionary<K, CounterData>();
			selected = default;
			ticks = 0;
			changeInterval = 600;
			changeTimers = new Dictionary<K, Timer>();
			oldValues = new Dictionary<K, int>();

			resetTicks = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetTicks.Elapsed += (sender, e) => ticks = 0;
		}

		/// <summary>
		/// Get and set the tracking data for the counter. If any of the quantities are already present and their values are
		/// different after the update, their property change listeners will be fired.
		/// </summary>
		public IImmutableDictionary<K, CounterData> Data
		{
			get => data.ToImmutableDictionary();
			set
			{
				data = value.ToDictionary((e) => e.Key, (e) => e.Value);

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

		/// <summary>
		/// Gets or sets the selected quantity. This can cause property change listeners to fire.
		/// </summary>
		public K Selected
		{
			get => selected;
			set
			{
				if (!EqualityComparer<K>.Default.Equals(selected, value))
				{
					selected = value;
					OnPropertyChanged("Value");
					OnPropertyChanged("TextColor");
					OnPropertyChanged("SelectedValid");
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the selected quantity.
		/// </summary>
		public int Value
		{
			get => SelectedValid() ? data[selected].Value : default;
			set
			{
				if (SelectedValid())
					this[selected] = value;
			}
		}

		/// <summary>
		/// Gets the text color of the selected quantity.
		/// </summary>
		public Color TextColor { get => SelectedValid() ? data[selected].TextColor : Color.Transparent; }

		/// <summary>
		/// Gets or sets the number of bezel rotations where further rotations use higher increments.
		/// </summary>
		public int TickThreshold { get; set; } = 10;

		/// <summary>
		/// Gets or sets the value of the increment to use after <see cref="TickThreshold"/> bezel rotations
		/// have occured.
		/// </summary>
		public int FastTickStep { get; set; } = 5;

		/// <summary>
		/// Gets or sets the amount of time, in ms, to wait before notifying property change listeners that a value
		/// has changed. This allows for coarser granularity of updates rather than updating every time the bezel is
		/// rotated.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the value of a particular quantity.
		/// </summary>
		/// <param name="key">Quantity whose value is to be examined.</param>
		/// <returns>The value of the desired quantity.</returns>
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

		/// <returns>True if there is a valid quantity selected, and false otherwise.</returns>
		public bool SelectedValid() => !EqualityComparer<K>.Default.Equals(selected, default) && data.ContainsKey(selected);

		/// <summary>
		/// Handle a bezel rotation. This increments or decrements the selected value (if there is one) by 1 each rotation
		/// until <see cref="TickThreshold"/> rotations occur, then increments or decrements by <see cref="FastTickStep"/>.
		/// Once the bezel stops rotating or starts rotating in the opposite direction, the increments revert to 1.
		/// </summary>
		/// <param name="args">Information about the bezel rotation.</param>
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

		/// <summary>
		/// Fire all of the property change listeners.
		/// </summary>
		/// <param name="name"></param>
		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// <summary>
		/// Occurs whenever a counter value has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs whenever the selected value has changed.
		/// </summary>
		public event EventHandler<CounterChangedEventArgs<K>> ValueChanged;
	}

	/// <summary>
	/// Class containing information about a counter value change.
	/// </summary>
	/// <typeparam name="K">Type of data whose count was changed.</typeparam>
	public class CounterChangedEventArgs<K> : EventArgs
	{
		public CounterChangedEventArgs() : base() {}

		/// <summary>
		/// Gets or sets the quantity whose value changed.
		/// </summary>
		public K Key { get; set; }

		/// <summary>
		/// Gets or sets the old value before the change.
		/// </summary>
		public int OldValue { get; set; }

		/// <summary>
		/// Gets or sets the new value after the change.
		/// </summary>
		public int NewValue { get; set; }
	}
}
