using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class HistoryPage : BezelInteractionPage
	{
		private const int LabelSpacing = 32;

		private int starting;
		private readonly Label startLabel;
		private readonly List<int> changes;
		private readonly List<Label> historyLabels;
		private readonly StackLayout historyList;

		public HistoryPage() : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			starting = 0;
			changes = new List<int>();
			historyLabels = new List<Label>();
			historyList = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand };

			startLabel = new Label();
			startLabel.SetBinding(Label.TextProperty, "StartingLife");
			startLabel.BindingContext = this;
			Clear();

			Button reset = new Button { Text = "Reset", VerticalOptions = LayoutOptions.End };
			Random r = new Random();
			reset.Clicked += (sender, e) => Reset?.Invoke(this, e);

			CircleScrollView scroll = new CircleScrollView {
				Content = historyList,
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			Content = new StackLayout { Children = { scroll, reset }};
			RotaryFocusObject = scroll;
		}

		public void Clear()
		{
			changes.Clear();
			historyLabels.Clear();
			historyList.Children.Clear();

			Grid startGrid = new Grid {
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = { new RowDefinition() },
				ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
				ColumnSpacing = LabelSpacing
			};
			startGrid.Children.Add(startLabel, 1, 0);
			historyList.Children.Add(startGrid);
		}

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

		public void AddChange(int n)
		{
			changes.Add(n);

			Grid changeGrid = new Grid {
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = { new RowDefinition() },
				ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
				ColumnSpacing = LabelSpacing
			};
			changeGrid.Children.Add(new Label {
				Text = (n > 0 ? "+" : "") + n.ToString(),
				TextColor = n < 0 ? Color.Red : Color.Default,
				HorizontalTextAlignment = TextAlignment.End
			}, 0, 0);
			historyLabels.Add(new Label {
				Text = (StartingLife + changes.Sum()).ToString(),
				TextColor = n < 0 ? Color.Red : Color.Default
			});
			changeGrid.Children.Add(historyLabels.Last(), 1, 0);
			historyList.Children.Add(changeGrid);
		}

		public event EventHandler Reset;
	}
}
