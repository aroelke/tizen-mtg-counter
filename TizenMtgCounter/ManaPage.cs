using System;
using System.Collections.Immutable;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class ManaPage : CounterPage<ManaType>
	{
		private const int ButtonSize = 60;

		public ManaPage() : base(ManaType.Values.ToImmutableDictionary((m) => m, (m) => new CounterData { Value = 0, Minimum = 0 }))
		{
			for (int i = 0; i < ManaType.Values.Count; i++)
			{
				ManaType t = ManaType.Values[i];
				ImageButton button = new ImageButton {
					Source = t.ImageSource,
					WidthRequest = ButtonSize,
					HeightRequest = ButtonSize,
					CornerRadius = ButtonSize/2
				};
				AddButton(ManaType.Values[i], button, (i - 2)*Math.PI/3);
				Children.Add(
					counter.Labels[t],
					(p) => (p.Width - Math.Max(p.GetSize(counter.Labels[t]).Width, p.GetSize(counter.Labels[t]).Height))/2 - ButtonSize,
					(i - 2)*Math.PI/3
				);
			}
		}
	}
}
