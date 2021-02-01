using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// Backing data containing counters for tracking one part of a game state.
	/// </summary>
	public class CounterData : INotifyPropertyChanged
	{
		private int val = 0;

		/// <summary>
		/// Create a new backing data set.
		/// </summary>
		public CounterData() {}

		/// <summary>
		/// Get or set the value of the quantity being tracked.
		/// </summary>
		public int Value {
			get => val;
			set
			{
				val = Math.Clamp(value, Minimum, Maximum);
				OnPropertyChanged();
				OnPropertyChanged("TextColor");
			}
		}

		/// <summary>
		/// Get or set the minimum value of the quantity being tracked.
		/// </summary>
		public int Minimum { get; set; } = int.MinValue;

		/// <summary>
		/// Get or set the maximum value of the quantity being tracked.
		/// </summary>
		public int Maximum { get; set; } = int.MaxValue;

		/// <summary>
		/// Get or set the thresholds that control the color of the counter displaying the quantity
		/// being tracked.
		/// </summary>
		public IList<(int, Color)> Thresholds { get; set; } = new List<(int, Color)>();
		public Color TextColor
		{
			get
			{
				foreach ((int threshold, Color color) in Thresholds)
					if (val <= threshold)
						return color;
				return Color.Default;
			}
		}

		/// <summary>
		/// Fire all of the property change listeners.
		/// </summary>
		/// <param name="name">Name of the property being changed.</param>
		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// <summary>
		/// Occurs whenever the value of the quantity being tracked changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
