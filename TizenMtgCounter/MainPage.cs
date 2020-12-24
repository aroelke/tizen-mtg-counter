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

		public MainPage(HistoryPage h) : base(() => new Dictionary<int, CounterData>() {
			[LIFE] = new CounterData { Value = h.StartingLife, Thresholds = { (5, Color.Red), (10, Color.Orange) } },
			[POISON] = new CounterData { Value = 0, Minimum = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) } }
		}.ToImmutableDictionary(), LIFE)
		{
			history = h;
			CommanderPage c = new CommanderPage();
			ManaPage m = new ManaPage();
			AdditionalPage a = new AdditionalPage();

			DarkenButton poisonButton = new DarkenButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 70,
				HeightRequest = 70,
				CornerRadius = 35
			};
			DarkenButton commanderPageButton = new DarkenButton {
				Source = "commander.png",
				WidthRequest = 45,
				HeightRequest = 45,
				CornerRadius = 0
			};
			DarkenButton manaPageButton = new DarkenButton {
				Source = "mana.png",
				WidthRequest = ManaPage.ButtonSize,
				HeightRequest = ManaPage.ButtonSize,
				CornerRadius = ManaPage.ButtonSize/2
			};
			DarkenButton additionalPageButton = new DarkenButton {
				Source = "additional.png",
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			DarkenButton historyPageButton = new DarkenButton {
				Source = "history.png", // Icon made by Google from www.flaticon.com
				WidthRequest = 45,
				HeightRequest = 45
			};

			Children.Add(historyPageButton, (p) => (p.Width - p.GetSize(historyPageButton).Width)/2 - 5, Math.PI/2);
			Children.Add(additionalPageButton, (p) => (p.Width - p.GetSize(additionalPageButton).Width)/2, Math.PI/2 + 2*Math.PI/5);
			Children.Add(poisonButton, (p) => (p.Width - p.GetSize(poisonButton).Width)/2, Math.PI/2 + 4*Math.PI/5);
			Children.Add(
				Labels[POISON],
				(p) => (p.Width - Math.Max(p.GetSize(Labels[POISON]).Width, p.GetSize(Labels[POISON]).Height))/2 - 55,
				Math.PI/2 + 4*Math.PI/5
			);
			Children.Add(commanderPageButton, (p) => (p.Width - p.GetSize(commanderPageButton).Width*Math.Sqrt(2))/2, Math.PI/2 + 6*Math.PI/5);
			Children.Add(manaPageButton, (p) => (p.Width - ManaPage.ButtonSize)/2 - ManaPage.ButtonOffset, Math.PI/2 + 8*Math.PI/5);

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
				if (counter.Selected == LIFE)
					maximizeTimer.Start();
			};
			poisonButton.Released += (sender, e) => {
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
						c.Clear();
						m.Clear();
						a.Clear();
						history.Clear(); // Must come after resetting life counter so it doesn't record it

						restart.Dismiss();
						Navigation.PopAsync(true);
					})
				};
				restart.BackButtonPressed += (s2, e2) => restart.Dismiss();
				restart.Show();
			};

			commanderPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(c, true);
			manaPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(m, true);
			additionalPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(a, true);
			historyPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(history, true);
		}
	}
}
