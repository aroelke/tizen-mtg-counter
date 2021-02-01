using Tizen.Wearable.CircularUI.Forms;
using Tizen.Wearable.CircularUI.Forms.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;

[assembly: ExportRenderer(typeof(TizenMtgCounter.CounterPopupEntry), typeof(TizenMtgCounter.CounterPopupEntryRenderer))]
namespace TizenMtgCounter
{
	/// <summary>
	/// Special <see cref="PopupEntry"/> for rendering with large text. When tapped,
	/// the font size reduces to fit the window with the keyboard.
	/// </summary>
	public class CounterPopupEntry : PopupEntry
	{
		/// <summary>
		/// Font size of the entry text while the keyboard is active.
		/// </summary>
		public const int FocusFontSize = 16;

		public CounterPopupEntry() : base() {}
	}

	/// <summary>
	/// Renderer for <see cref="CounterPopupEntry"/> that handles the text size changing while
	/// the keyboard is up.
	/// </summary>
	internal class CounterPopupEntryRenderer : PopupEntryRenderer
	{
		public CounterPopupEntryRenderer() : base() {}

		/// <summary>
		/// When the <see cref="CounterPopupEntry"/> is tapped and the keyboard is brought up,
		/// change the font size so it fits. Change it back if the keyboard is dismissed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				double size = e.NewElement.FontSize;
				Control.Clicked += (sender, v) => e.NewElement.FontSize = CounterPopupEntry.FocusFontSize;
				e.NewElement.Completed += (sender, v) => e.NewElement.FontSize = size;
			}
		}
	}
}
