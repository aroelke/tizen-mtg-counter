using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage, IRotaryEventReceiver
	{
		private int x;
		private Label label;

		public MainPage()
		{
			label = new Label
			{
				Text = "0",
				FontSize = 32
			};

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					label
				}
			};

			RotaryFocusObject = this;
		}

		public void Rotate(RotaryEventArgs args)
		{
			if (args.IsClockwise)
				x++;
			else
				x--;
			label.Text = x.ToString();
		}
	}
}
