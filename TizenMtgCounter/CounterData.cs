using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CounterData<T> where T : IComparable<T>
	{
		public T Value { get; set; }
		public IList<(T, Color)> Thresholds { get; set; } = new List<(T, Color)>();
		public Color TextColor
		{
			get
			{
				foreach ((T threshold, Color color) in Thresholds)
					if (threshold.CompareTo(threshold) <= 0)
						return color;
				return Color.Default;
			}
		}
	}
}
