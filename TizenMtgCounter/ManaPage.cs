using System;
using System.Collections.Immutable;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page for tracking contents of the mana pool.
	/// </summary>
	public class ManaPage : CounterPage<ManaType>
	{
		/// <summary>Size of the button for each mana type.</summary>
		public const int ButtonSize = 60;
		/// <summary>Offset of each button from the edge of the screen.</summary>
		public const int ButtonOffset = 5;

		/// <summary>
		/// Creates a new ManaPage.
		/// The page will be populated with a button for each of the six types of mana to track the amount of that mana in the mana pool
		/// with an initial amount of 0 for each one.
		/// </summary>
		public ManaPage() : base(() => ManaType.Values.ToImmutableDictionary((m) => m, (m) => new CounterData { Value = 0, Minimum = 0 }))
		{
			for (int i = 0; i < ManaType.Values.Count; i++)
			{
				ManaType t = ManaType.Values[i];
				DarkenButton button = new DarkenButton {
					Source = t.ImageSource,
					WidthRequest = ButtonSize,
					HeightRequest = ButtonSize,
					CornerRadius = ButtonSize/2
				};
				AddButton(ManaType.Values[i], button, (p) => (p.Width - ButtonSize)/2 - ButtonOffset, (i - 2)*Math.PI/3);
				Children.Add(
					Labels[t],
					(p) => (p.Width - Math.Max(p.GetSize(Labels[t]).Width, p.GetSize(Labels[t]).Height))/2 - ButtonSize - ButtonOffset,
					(i - 2)*Math.PI/3
				);
			}
		}
	}
}
