using System;
using System.Collections.Generic;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage
	{
		private const int LIFE = 1;
		private const int POISON = 2;

		public MainPage(ManaPage mana, HistoryPage history) : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			Counter<int> counter = new Counter<int> {
				Data = new Dictionary<int, CounterData>() {
					[LIFE] = new CounterData { Value = history.StartingLife, Thresholds = { (5, Color.Red), (10, Color.Orange) }},
					[POISON] = new CounterData { Value = 0, Minimum = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) }}
				},
				Selected = LIFE
			};

			bool maximized = false;

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

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			layout.Children.Add(
				counter.Content,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counter.Content).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counter.Content).Height)/2)
			);
			layout.Children.Add(
				counter.Labels[POISON],
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 3)/2 - getSize(counter.Labels[POISON]).Width/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 3)/2 - getSize(counter.Labels[POISON]).Height/2)
			);
			layout.Children.Add(
				poisonButton,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 1)/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 1)/2)
			);
			layout.Children.Add(
				manaPageButton,
				Constraint.RelativeToParent((p) => p.Width - getSize(manaPageButton).Width),
				Constraint.RelativeToParent((p) => (p.Height - getSize(manaPageButton).Height)/2)
			);
			layout.Children.Add(
				historyPageButton,
				Constraint.RelativeToParent((p) => (p.Width - getSize(historyPageButton).Width)/2),
				Constraint.Constant(0)
			);
			Content = layout;
			RotaryFocusObject = counter;

			counter.ValueChanged += (sender, e) => {
				if (e is CounterChangedEventArgs<int> args && args.Key == LIFE)
					history.AddChange(args.NewValue - args.OldValue);
			};

			bool maximize = false;
			Timer maximizeTimer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			maximizeTimer.Elapsed += (sender, e) => {
				maximize = maximized = true;
				Device.BeginInvokeOnMainThread(() => {
					counter.Selected = POISON;
					counter.Labels[POISON].IsVisible = false;
				});
			};
			poisonButton.Pressed += (sender, e) => {
				poisonButton.Opacity = 1.0/3.0;
				if (!maximized)
					maximizeTimer.Start();
			};
			poisonButton.Released += (sender, e) => {
				poisonButton.Opacity = 1;
				maximizeTimer.Stop();
				if (!maximize)
				{
					if (maximized)
					{
						maximized = false;
						counter.Selected = LIFE;
						counter.Labels[POISON].IsVisible = true;
					}
					else
						counter[POISON]++;
				}
				maximize = false;
			};

			history.Reset += (sender, e) => {
				counter[LIFE] = history.StartingLife;
				counter[POISON] = 0;
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
	}
}
