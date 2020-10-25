﻿using System;
using System.Collections.Generic;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Label = Xamarin.Forms.Label;

namespace TizenMtgCounter
{
	public class MainPage : BezelInteractionPage
	{
		private const int LIFE = 1;
		private const int POISON = 2;
		private readonly Counter<int> counter;

		private bool maximized;
		private readonly Label poisonCounter;

		public MainPage() : base()
		{
			counter = new Counter<int> {
				Data = {
					{ LIFE, new CounterData<int> { Value = 20, Thresholds = { (5, Color.Red), (10, Color.Orange) }}},
					{ POISON, new CounterData<int> { Value = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) }}}
				},
				Selected = LIFE
			};

			maximized = false;

			ImageButton poisonButton = new ImageButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 70,
				HeightRequest = 70,
				CornerRadius = 35
			};
			poisonCounter = new Label {
				Text = counter[POISON].ToString(),
				FontSize = 8,
				TextColor = counter.GetTextColor(POISON)
			};

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			layout.Children.Add(
				counter.Content,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counter.Content).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counter.Content).Height)/2)
			);
			layout.Children.Add(
				poisonCounter,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 3)/2 - getSize(poisonCounter).Width/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 3)/2 - getSize(poisonCounter).Height/2)
			);
			layout.Children.Add(
				poisonButton,
				Constraint.RelativeToParent((p) => p.Width/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Width*(Math.Sqrt(2) - 1)/2),
				Constraint.RelativeToParent((p) => p.Height/2*(1 - 1/Math.Sqrt(2)) - getSize(poisonButton).Height*(Math.Sqrt(2) - 1)/2)
			);
			Content = layout;

			RotaryFocusObject = counter;

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
					poisonCounter.IsVisible = false;
				});
			};
			poisonButton.Pressed += (sender, e) => {
				if (!maximized)
					maximizeTimer.Start();
			};
			poisonButton.Released += (sender, e) => {
				maximizeTimer.Stop();
				if (!maximize)
				{
					if (maximized)
					{
						maximized = false;
						counter.Selected = LIFE;
						poisonCounter.Text = counter[POISON].ToString();
						poisonCounter.TextColor = counter.GetTextColor(POISON);
						poisonCounter.IsVisible = true;
					}
					else
					{
						counter[POISON]++;
						poisonCounter.Text = counter[POISON].ToString();
						poisonCounter.TextColor = counter.GetTextColor(POISON);
					}
				}
				maximize = false;
			};
		}
	}
}
