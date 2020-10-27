using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CounterData<T> where T : IComparable<T>
	{
		public T Value { get; set; }
		public IList<(T, Color)> Thresholds { get; set; } = new List<(T, Color)>();
	}
}
