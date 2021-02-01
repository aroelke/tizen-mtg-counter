using System;
using System.Collections.Immutable;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page for tracking miscellaneous game state.
	/// </summary>
	public class AdditionalPage : CounterPage<AdditionalState>
	{
		/// <summary>Size of the buttons.</summary>
		public const int ButtonSize = 60;
		/// <summary>Offset from the edge of the screen to display buttons.</summary>
		public const int ButtonOffset = 5;

		/// <summary>
		/// Create a new <c>AdditionalStatePage</c> to display the count for each <see cref="AdditionalState"/>.
		/// This page will be populated with a button for each quantity and a count that defaults to 0.
		/// </summary>
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
