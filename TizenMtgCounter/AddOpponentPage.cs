using System.Collections.Generic;
using Xamarin.Forms;
using Label = Xamarin.Forms.Label;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TizenMtgCounter
{
	public class AddOpponentPage : ContentPage
	{
		public AddOpponentPage() : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			var items = new List<FileImageSource> {
				"colorless_mana.png",
				"white_mana.png",
				"blue_mana.png",
				"black_mana.png",
				"red_mana.png",
				"green_mana.png",
				"white_blue_mana.png",
				"blue_black_mana.png",
				"black_red_mana.png",
				"red_green_mana.png",
				"green_white_mana.png",
				"white_black_mana.png",
				"black_green_mana.png",
				"green_blue_mana.png",
				"blue_red_mana.png",
				"red_white_mana.png",
				"snow_mana.png"
			};
			ListView carousel = new ListView {
				ItemsSource = items,
				ItemTemplate = new DataTemplate(() => {
					DarkenButton img = new DarkenButton();
					img.SetBinding(ImageButton.SourceProperty, "File");
					img.Clicked += (sender, e) => {
						CommanderPage.AddOpponent(img.Source);
						Navigation.PopAsync(true);
					};
					return new ViewCell { View = new StackLayout { Children = { img }, Padding = new Thickness(10), HeightRequest = 100 }};
				}),
				HasUnevenRows = true,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			Content = new StackLayout { Children = { new Label(), new Label { Text = "Choose an icon:", HorizontalOptions = LayoutOptions.CenterAndExpand }, carousel }};
		}

		public CommanderPage CommanderPage { get; set; }
	}
}
