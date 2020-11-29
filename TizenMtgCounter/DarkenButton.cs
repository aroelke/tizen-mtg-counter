using Xamarin.Forms;

namespace TizenMtgCounter
{
	class DarkenButton : ImageButton
	{
		public DarkenButton() : base()
		{
			Opacity = ReleasedOpacity;
			Pressed += (sender, e) => Opacity = HeldOpacity;
			Released += (sender, e) => Opacity = ReleasedOpacity;
		}

		public double HeldOpacity { get; set; } = 1.0/3.0;
		public double ReleasedOpacity { get; set; } = 1;
	}
}
