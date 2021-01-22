using System;
using System.Collections.Immutable;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class AdditionalPage : CounterPage<AdditionalState>
	{
		private const int ButtonSize = 60;
		private const int ButtonOffset = 5;

		public AdditionalPage() : base(() => AdditionalState.Values.ToImmutableDictionary((s) => s, (s) => new CounterData { Value = 0, Minimum = 0 }))
		{
			for (int i = 0; i < AdditionalState.Values.Count; i++)
			{
				AdditionalState s = AdditionalState.Values[i];
				ImageButton button = new ImageButton {
					Source = s.ImageSource,
					WidthRequest = ButtonSize,
					HeightRequest = ButtonSize,
					CornerRadius = 0
				};
				AddButton(AdditionalState.Values[i], button, (p) => (p.Width - ButtonSize)/2 - ButtonOffset, (i - 2)*Math.PI/2 + Math.PI/4);
				Children.Add(
					Labels[s],
					(p) => (p.Width - Math.Max(p.GetSize(Labels[s]).Width, p.GetSize(Labels[s]).Height))/2 - ButtonSize - ButtonOffset,
					(i - 2)*Math.PI/2 + Math.PI/4
				);
			}
		}
	}
}
