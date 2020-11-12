using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : CounterPage<int>
	{
		private const int LIFE = 1;
		private const int POISON = 2;

		private readonly HistoryPage history;

		public MainPage(ManaPage mana, HistoryPage h) : base()
		{
			history = h;
			Clear();

			ImageButton poisonButton = new ImageButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 70,
				HeightRequest = 70,
				CornerRadius = 35
			};

			ImageButton manaPageButton = new ImageButton {
				Source = "mana.png",
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};

			ImageButton historyPageButton = new ImageButton {
				Source = "history.png", // Icon made by Google from www.flaticon.com
				WidthRequest = 45,
				HeightRequest = 45
			};

			Children.Add(poisonButton, (p) => (p.Width - p.GetSize(poisonButton).Width)/2 + 5, 5*Math.PI/4);
			Children.Add(
				counter.Labels[POISON],
				(p) => (p.Width - Math.Max(p.GetSize(counter.Labels[POISON]).Width, p.GetSize(counter.Labels[POISON]).Height))/2 - 55,
				5*Math.PI/4
			);
			Children.Add(manaPageButton, (p) => (p.Width - 60)/2, 0);
			Children.Add(historyPageButton, (p) => (p.Width - 45)/2, 3*Math.PI/2);

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
					counter.Labels[POISON].IsVisible = false;
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
						counter.Labels[POISON].IsVisible = true;
					}
					else
						counter[POISON]++;
				}
				maximize = false;
			};

			history.Reset += (sender, e) => {
				Clear();
				mana.Clear();
				history.Clear(); // Must come after resetting life counter so it doesn't record it
			};

			manaPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(mana, true);
			manaPageButton.Pressed += (sender, e) => manaPageButton.Opacity = 1.0/3.0;
			manaPageButton.Released += (sender, e) => manaPageButton.Opacity = 1;

			historyPageButton.Clicked += async (sender, e) => await Navigation.PushAsync(history, true);
			historyPageButton.Pressed += (sender, e) => historyPageButton.Opacity = 1.0/3.0;
			historyPageButton.Released += (sender, e) => historyPageButton.Opacity = 1;
		}

		public override void Clear()
		{
			// Make sure super constructor doesn't run this code before history is assigned
			if (history != null)
			{
				counter.Data = new Dictionary<int, CounterData>() {
					[LIFE] = new CounterData { Value = history.StartingLife, Thresholds = { (5, Color.Red), (10, Color.Orange) } },
					[POISON] = new CounterData { Value = 0, Minimum = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) } }
				}.ToImmutableDictionary();
				counter.Selected = LIFE;
			}
		}
	}
}
