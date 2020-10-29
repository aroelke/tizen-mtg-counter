using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class ManaPage : BezelInteractionPage
	{
		private const int ButtonSize = 60;

		public ManaPage() : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			Counter<ManaType> counter = new Counter<ManaType> { Data = ManaType.Values.ToDictionary((m) => m, (m) => new CounterData { Value = 0, Minimum = 0 }) };

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
				Timer timer = new Timer {
					Interval = 500,
					Enabled = false,
					AutoReset = false
				};
				timer.Elapsed += (sender, e) => Device.BeginInvokeOnMainThread(() => counter[t] = 0);

				buttons[t].Pressed += (sender, e) => {
					buttons[t].Opacity = 1.0/3.0;
					timer.Start();
				};

				buttons[t].Released += (sender, e) => {
					buttons[t].Opacity = 1;
					if (timer.Enabled)
					{
						timer.Stop();

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
					}
				};
			}
		}
	}
}
