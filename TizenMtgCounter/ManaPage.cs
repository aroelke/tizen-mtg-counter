using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	class ManaPage : BezelInteractionPage
	{
		private const int ButtonSize = 60;

		public ManaPage() : base()
		{
			Counter<ManaType> counter = new Counter<ManaType> { Data = new Dictionary<ManaType, CounterData> {
				[ManaType.Colorless] = new CounterData { Value = 0 },
				[ManaType.White] = new CounterData { Value = 0 },
				[ManaType.Blue] = new CounterData { Value = 0 },
				[ManaType.Black] = new CounterData { Value = 0 },
				[ManaType.Red] = new CounterData { Value = 0 },
				[ManaType.Green] = new CounterData { Value = 0 }
			}};

			IDictionary<ManaType, ImageButton> buttons = ManaType.Values.ToDictionary(
				(m) => m,
				(m) => new ImageButton {
					Source = m.ImageSource,
					WidthRequest = ButtonSize,
					HeightRequest = ButtonSize,
					CornerRadius = ButtonSize/2
				}
			);

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			layout.Children.Add(
				counter.Content,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counter.Content).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counter.Content).Height)/2)
			);
			for (int i = 0; i < ManaType.Values.Count; i++)
			{
				double x = Math.Cos((i + 1)*Math.PI/3);
				double y = Math.Sin((i + 1)*Math.PI/3);
				ManaType t = ManaType.Values[i];
				layout.Children.Add(
					buttons[ManaType.Values[i]],
					Constraint.RelativeToParent((p) => (p.Width - getSize(buttons[t]).Width)/2*(1 - x)),
					Constraint.RelativeToParent((p) => (p.Height - getSize(buttons[t]).Height)/2*(1 - y))
				);
				layout.Children.Add(
					counter.Labels[ManaType.Values[i]],
					Constraint.RelativeToParent((p) => p.Width/2*(1 - x) + getSize(buttons[t]).Width*x - getSize(counter.Labels[t]).Width/2 + getSize(counter.Labels[t]).Height*x/2),
					Constraint.RelativeToParent((p) => p.Height/2*(1 - y) + getSize(buttons[t]).Height*y - getSize(counter.Labels[t]).Height/2 + getSize(counter.Labels[t]).Height*y/2)
				);
			}
			Content = layout;

			RotaryFocusObject = counter;

			foreach (ManaType t in ManaType.Values)
			{
				buttons[t].Clicked += (sender, e) => {
					if (counter.Selected == t)
					{
						counter.Selected = null;
						counter.Labels[t].IsVisible = true;
					}
					else
					{
						if (counter.SelectedValid())
							counter.Labels[counter.Selected].IsVisible = true;
						counter.Selected = t;
						counter.Labels[t].IsVisible = false;
					}
				};
			}
		}
	}
}
