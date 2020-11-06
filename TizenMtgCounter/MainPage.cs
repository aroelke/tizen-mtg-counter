using System;
using System.Collections.Generic;
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

			Children.Add(
				counter.Content,
				Constraint.RelativeToParent((p) => (p.Width - GetSize(counter.Content).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - GetSize(counter.Content).Height)/2)
			);
			Children.Add(
				counter.Labels[POISON],
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - GetSize(poisonButton).Width*(Math.Sqrt(2) - 3)/2 - GetSize(counter.Labels[POISON]).Width/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - GetSize(poisonButton).Height*(Math.Sqrt(2) - 3)/2 - GetSize(counter.Labels[POISON]).Height/2)
			);
			Children.Add(
				poisonButton,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - GetSize(poisonButton).Width*(Math.Sqrt(2) - 1)/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - GetSize(poisonButton).Height*(Math.Sqrt(2) - 1)/2)
			);
			Children.Add(
				manaPageButton,
				Constraint.RelativeToParent((p) => p.Width - GetSize(manaPageButton).Width),
				Constraint.RelativeToParent((p) => (p.Height - GetSize(manaPageButton).Height)/2)
			);
			Children.Add(
				historyPageButton,
				Constraint.RelativeToParent((p) => (p.Width - GetSize(historyPageButton).Width)/2),
				Constraint.Constant(0)
			);

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
				};
				counter.Selected = LIFE;
			}
		}
	}
}
