using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// <see cref="ImageButton"/> that becomes transleucent while held down.
	/// Against a black background, this creates a darkening effect.
	/// </summary>
	class DarkenButton : ImageButton
	{
		/// <summary>
		/// Create a new <c>DarkenButton</c>.
		/// </summary>
		public DarkenButton() : base()
		{
			Opacity = ReleasedOpacity;
			Pressed += (sender, e) => Opacity = HeldOpacity;
			Released += (sender, e) => Opacity = ReleasedOpacity;
		}

		/// <summary>
		/// Get or set the opacity of the button while it's held down.
		/// </summary>
		public double HeldOpacity { get; set; } = 1.0/3.0;

		/// <summary>
		/// Get or set the opacity while the button is not held down.
		/// </summary>
		public double ReleasedOpacity { get; set; } = 1;
	}
}
