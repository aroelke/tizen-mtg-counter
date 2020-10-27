using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CounterData<T> : INotifyPropertyChanged where T : IComparable<T>
	{
		private T val = default;

		public T Value {
			get => val;
			set
			{
				val = value;
				OnPropertyChanged();
				OnPropertyChanged("TextColor");
			}
		}

		public IList<(T, Color)> Thresholds { get; set; } = new List<(T, Color)>();
		public Color TextColor
		{
			get
			{
				foreach ((T threshold, Color color) in Thresholds)
					if (val.CompareTo(threshold) <= 0)
						return color;
				return Color.Default;
			}
		}

		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
