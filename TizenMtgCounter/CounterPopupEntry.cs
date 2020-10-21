using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.Wearable.CircularUI.Forms.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;

[assembly: ExportRenderer(typeof(TizenMtgCounter.CounterPopupEntry), typeof(TizenMtgCounter.CounterPopupEntryRenderer))]
namespace TizenMtgCounter
{
	public class CounterPopupEntry : PopupEntry {}

	internal class CounterPopupEntryRenderer : PopupEntryRenderer
	{
		public CounterPopupEntryRenderer() : base() { }

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				double size = e.NewElement.FontSize;
				Control.Clicked += (sender, v) => e.NewElement.FontSize = 16;
				e.NewElement.Completed += (sender, v) => e.NewElement.FontSize = size;
			}
		}
	}
}
