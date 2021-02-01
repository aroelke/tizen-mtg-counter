using System.Collections.Generic;
using Xamarin.Forms;
using Label = Xamarin.Forms.Label;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page for customizing the opponent to add when adding a new opponent to the
	/// <see cref="CommanderPage"/>.
	/// </summary>
	public class AddOpponentPage : ContentPage
	{
		/// <summary>
		/// Create a new <c>AddOpponentPage</c> with a preset list of icons to choose from.
		/// </summary>
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

			// Add an extra label at the top of the stack here so the text doesn't get cut off
			Content = new StackLayout { Children = { new Label(), new Label { Text = "Choose an icon:", HorizontalOptions = LayoutOptions.CenterAndExpand }, carousel }};
		}

		/// <summary>
		/// Gets or sets the <see cref="CommanderPage"/> where the new opponent will be added.
		/// </summary>
		public CommanderPage CommanderPage { get; set; }
	}
}
