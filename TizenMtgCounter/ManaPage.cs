using System;
using System.Collections.Immutable;

namespace TizenMtgCounter
{
	public class ManaPage : CounterPage<ManaType>
	{
		public const int ButtonSize = 60;
		public const int ButtonOffset = 5;

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
