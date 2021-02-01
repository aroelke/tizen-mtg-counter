using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Label = Xamarin.Forms.Label;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page for tracking history of life changes and resetting all counters.
	/// </summary>
	public class HistoryPage : BezelInteractionPage
	{
		private const int LabelSpacing = 32;

		private int starting;
		private readonly Label startLabel;
		private readonly List<int> changes;
		private readonly List<Label> historyLabels;
		private readonly StackLayout historyList;

		/// <summary>
		/// Create a new <c>HistoryPage</c> with a default <see cref="StartingLife"/> of 0.
		/// </summary>
		public HistoryPage() : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			starting = 0;
			changes = new List<int>();
			historyLabels = new List<Label>();
			historyList = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand };

			startLabel = new Label { HorizontalTextAlignment = TextAlignment.Center };
			startLabel.SetBinding(Label.TextProperty, "StartingLife");
			startLabel.BindingContext = this;
			Clear();

			Button reset = new Button { Text = "New Game", VerticalOptions = LayoutOptions.End };
			reset.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Bottom);
			reset.Clicked += (sender, e) => Reset?.Invoke(this, e);

			CircleScrollView scroll = new CircleScrollView {
				Content = historyList,
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			Content = new StackLayout { Children = { scroll, reset }};
			RotaryFocusObject = scroll;
		}

		/// <summary>
		/// Clear the life total history.
		/// </summary>
		public void Clear()
		{
			changes.Clear();
			historyLabels.Clear();
			historyList.Children.Clear();
			historyList.Children.Add(startLabel);
		}

		/// <summary>
		/// Get or set the starting life total when resetting the <see cref="MainPage"/> counters.
		/// </summary>
		public int StartingLife
		{
			get => starting;
			set
			{
				starting = value;
				OnPropertyChanged();
				for (int i = 0; i < changes.Count; i++)
					historyLabels[i].Text = (value + changes.Take(i + 1).Sum()).ToString();
			}
		}

		/// <summary>
		/// Add a life total change to the history.
		/// </summary>
		/// <param name="n">Amount of life that was gained or lost.</param>
		public void AddChange(int n)
		{
			changes.Add(n);

			historyLabels.Add(new Label {
				Text = (StartingLife + changes.Sum()).ToString(),
				TextColor = n < 0 ? Color.Red : Color.Default
			});
			StackLayout row = new StackLayout {
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Orientation = StackOrientation.Horizontal,
				Spacing = LabelSpacing,
				Children = {
					new Label {
						Text = (n > 0 ? "+" : "") + n.ToString(),
						TextColor = n < 0 ? Color.Red : Color.Default,
						HorizontalTextAlignment = TextAlignment.End
					},
					historyLabels.Last()
				}
			};
			historyList.Children.Add(row);
		}

		/// <summary>
		/// Occurs whenever the reset button is pressed.
		/// </summary>
		public event EventHandler Reset;
	}
}
