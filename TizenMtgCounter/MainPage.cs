using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : CounterPage<int>
	{
		private const int LIFE = 1;
		private const int POISON = 2;

		private readonly HistoryPage history;

		public MainPage(CommanderPage c, ManaPage m, AdditionalPage a, HistoryPage h) : base(() => new Dictionary<int, CounterData>() {
			[LIFE] = new CounterData { Value = h.StartingLife, Thresholds = { (5, Color.Red), (10, Color.Orange) } },
			[POISON] = new CounterData { Value = 0, Minimum = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) } }
		}.ToImmutableDictionary(), LIFE)
		{
			history = h;

			ImageButton poisonButton = new ImageButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 70,
				HeightRequest = 70,
				CornerRadius = 35
			};
			ImageButton commanderPageButton = new ImageButton {
				Source = "commander.png",
				WidthRequest = 45,
				HeightRequest = 45,
				CornerRadius = 0
			};
			ImageButton manaPageButton = new ImageButton {
				Source = "mana.png",
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			ImageButton additionalPageButton = new ImageButton {
				Source = "additional.png",
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			ImageButton historyPageButton = new ImageButton {
				Source = "history.png", // Icon made by Google from www.flaticon.com
				WidthRequest = 45,
				HeightRequest = 45
			};

			Children.Add(poisonButton, (p) => (p.Width - p.GetSize(poisonButton).Width)/2, 5*Math.PI/4);
			Children.Add(
				Labels[POISON],
				(p) => (p.Width - Math.Max(p.GetSize(Labels[POISON]).Width, p.GetSize(Labels[POISON]).Height))/2 - 55,
				5*Math.PI/4
			);
			Children.Add(commanderPageButton, (p) => (p.Width - p.GetSize(commanderPageButton).Width*Math.Sqrt(2))/2, -Math.PI/4);
			Children.Add(manaPageButton, (p) => (p.Width - p.GetSize(manaPageButton).Width)/2, 0);
			Children.Add(additionalPageButton, (p) => (p.Width - p.GetSize(additionalPageButton).Width)/2, Math.PI);
			Children.Add(historyPageButton, (p) => (p.Width - p.GetSize(historyPageButton).Width)/2, Math.PI/2);

			counter.ValueChanged += (sender, e) => {
				if (e.Key == LIFE)
					history.AddChange(e.NewValue - e.OldValue);
			};

			bool maximize = false;
			Timer maximizeTimer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			maximizeTimer.Elapsed += (sender, e) => {
				maximize = true;
				Device.BeginInvokeOnMainThread(() => {
					counter.Selected = POISON;
					Labels[POISON].IsVisible = false;
				});
			};
			poisonButton.Pressed += (sender, e) => {
				poisonButton.Opacity = 1.0/3.0;
				if (counter.Selected == LIFE)
					maximizeTimer.Start();
			};
			poisonButton.Released += (sender, e) => {
				poisonButton.Opacity = 1;
				maximizeTimer.Stop();
				if (!maximize)
				{
					if (counter.Selected == POISON)
					{
						counter.Selected = LIFE;
						Labels[POISON].IsVisible = true;
					}
					else
						counter[POISON]++;
				}
				maximize = false;
			};

			history.Reset += (sender, e) => {
				CircleStepper initial = new CircleStepper {
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.Start,
					Minimum = 0,
					Maximum = 999,
					Value = history.StartingLife,
					IsWrapEnabled = false
				};
				TwoButtonPopup restart = new TwoButtonPopup {
					Title = "Start a new game?",
					Content = new StackLayout {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Orientation = StackOrientation.Vertical,
						Children = { new Label { Text = "Starting life total:", FontSize = 8, HorizontalOptions = LayoutOptions.CenterAndExpand }, initial }
					}
				};
				restart.FirstButton = new MenuItem {
					IconImageSource = "cancel.png", // Icon made by Google from www.flaticon.com
					Command = new Command(restart.Dismiss)
				};
				restart.SecondButton = new MenuItem {
					IconImageSource = "confirm.png", // Icon made by Google from www.flaticon.com
					Command = new Command(() => {
						history.StartingLife = (int)initial.Value;

						Clear();
						m.Clear();
						a.Clear();
						history.Clear(); // Must come after resetting life counter so it doesn't record it

						restart.Dismiss();
					})
				};
				restart.BackButtonPressed += (s2, e2) => restart.Dismiss();
				restart.Show();
			};

			commanderPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(c, true);
			commanderPageButton.Pressed += (sender, e) => commanderPageButton.Opacity = 1.0/3.0;
			commanderPageButton.Released += (sender, e) => commanderPageButton.Opacity = 1;

			manaPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(m, true);
			manaPageButton.Pressed += (sender, e) => manaPageButton.Opacity = 1.0/3.0;
			manaPageButton.Released += (sender, e) => manaPageButton.Opacity = 1;

			additionalPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(a, true);
			additionalPageButton.Pressed += (sender, e) => additionalPageButton.Opacity = 1.0/3.0;
			additionalPageButton.Released += (sender, e) => additionalPageButton.Opacity = 1;

			historyPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(history, true);
			historyPageButton.Pressed += (sender, e) => historyPageButton.Opacity = 1.0/3.0;
			historyPageButton.Released += (sender, e) => historyPageButton.Opacity = 1;
		}
	}
}
