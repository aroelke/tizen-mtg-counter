using System;
using System.Linq;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class ManaPage : CounterPage<ManaType>
	{
		private const int ButtonSize = 60;

		public ManaPage() : base(ManaType.Values.ToDictionary((m) => m, (m) => new CounterData { Value = 0, Minimum = 0 }))
		{
			for (int i = 0; i < ManaType.Values.Count; i++)
			{
				double x = Math.Cos((i + 1)*Math.PI/3);
				double y = Math.Sin((i + 1)*Math.PI/3);
				ManaType t = ManaType.Values[i];

				ImageButton button = new ImageButton {
					Source = t.ImageSource,
					WidthRequest = ButtonSize,
					HeightRequest = ButtonSize,
					CornerRadius = ButtonSize/2
				};
				AddButton(
					ManaType.Values[i],
					button,
					Constraint.RelativeToParent((p) => (p.Width - GetSize(button).Width)/2*(1 - x)),
					Constraint.RelativeToParent((p) => (p.Height - GetSize(button).Height)/2*(1 - y))
				);
				Children.Add(
					counter.Labels[ManaType.Values[i]],
					Constraint.RelativeToParent((p) => p.Width/2*(1 - x) + GetSize(button).Width*x - GetSize(counter.Labels[t]).Width/2 + GetSize(counter.Labels[t]).Height*x/2),
					Constraint.RelativeToParent((p) => p.Height/2*(1 - y) + GetSize(button).Height*y - GetSize(counter.Labels[t]).Height/2 + GetSize(counter.Labels[t]).Height*y/2)
				);
			}
		}
	}
}
