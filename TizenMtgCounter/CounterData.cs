using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CounterData : INotifyPropertyChanged
	{
		private int val = 0;

		public int Value {
			get => val;
			set
			{
				val = value;
				OnPropertyChanged();
				OnPropertyChanged("TextColor");
			}
		}

		public int Minimum { get; set; } = int.MinValue;
		public int Maximum { get; set; } = int.MaxValue;

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

		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
