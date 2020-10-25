using System.Collections.Generic;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	class Counter<K>
	{
		public static Counter<K> operator ++(Counter<K> counter)
		{
			if (EqualityComparer<K>.Default.Equals(counter.Focus, default))
				counter[counter.Focus]++;
			return counter;
		}

		public IDictionary<K, CounterData<int>> Data { get; set; } = new Dictionary<K, CounterData<int>>();

		public K Focus { get; set; } = default;

		public int this[K key]
		{
			get => Data[key].Value;
			set => Data[key].Value = value;
		}

		public Color GetTextColor()
		{
			if (EqualityComparer<K>.Default.Equals(Focus, default))
				return Color.Transparent;
			else
				return GetTextColor(Focus);
		}

		public Color GetTextColor(K key)
		{
			return Data[key].GetTextColor();
		}
	}
}
