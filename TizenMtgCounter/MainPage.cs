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
	public class MainPage : BezelInteractionPage, IRotaryEventReceiver
	{
		private readonly CounterData<int> life;
		private readonly CounterData<int> poison;
		private int ticks;
		private bool maximized;
		private readonly CounterPopupEntry lifeCounter;
		private readonly CounterPopupEntry poisonEntry;
		private readonly Label poisonCounter;
		private readonly Timer resetTicks;

		public MainPage() : base()
		{
			life = new CounterData<int> { Value = 0 };
			poison = new CounterData<int> { Value = 0, Thresholds = { (9, Color.Default), (int.MaxValue, Color.Red) }};
			ticks = 0;
			maximized = false;

			lifeCounter = new CounterPopupEntry
			{
				Text = life.Value.ToString(),
				FontSize = 32,
				TextColor = life.GetTextColor(),
				Keyboard = Keyboard.Numeric,
				BackgroundColor = Color.Transparent,
				HorizontalTextAlignment = TextAlignment.Center
			};
			RepeatButton plusButton = new RepeatButton
			{
				Text = "+",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			plusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);
			RepeatButton minusButton = new RepeatButton
			{
				Text = "\u2212",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60
			};
			minusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);

			ImageButton poisonButton = new ImageButton
			{
				Source = "poison.png", // Image by reptiletc
				WidthRequest = 70,
				HeightRequest = 70,
				CornerRadius = 35
			};
			poisonCounter = new Label {
				Text = poison.Value.ToString(),
				FontSize = 8,
				TextColor = poison.GetTextColor()
			};
			poisonEntry = new CounterPopupEntry {
				Text = life.Value.ToString(),
				FontSize = 32,
				TextColor = life.GetTextColor(),
				Keyboard = Keyboard.Numeric,
				BackgroundColor = Color.Transparent,
				HorizontalTextAlignment = TextAlignment.Center,
				IsVisible = false
			};

			Size getSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);
			RelativeLayout layout = new RelativeLayout();
			StackLayout counterLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Spacing = -24,
				Children = {
					plusButton,
					lifeCounter,
					poisonEntry,
					minusButton
				}
			};
			layout.Children.Add(
				counterLayout,
				Constraint.RelativeToParent((p) => (p.Width - getSize(counterLayout).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - getSize(counterLayout).Height)/2)
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

			RotaryFocusObject = this;
			resetTicks = new Timer
			{
				Interval = 500,
				Enabled = false,
				AutoReset = false,
			};
			resetTicks.Elapsed += (sender, e) => ticks = 0;

			lifeCounter.Completed += (sender, e) => {
				if (int.TryParse(lifeCounter.Text, out int result))
					Life = result;
				else
					Life = Life;
			};
			plusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => Increment(1));
			plusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => Increment(1));
			minusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => Increment(-1));
			minusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => Increment(-1));

			bool maximize = false;
			Timer maximizeTimer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			maximizeTimer.Elapsed += (sender, e) => {
				maximize = maximized = true;
				Device.BeginInvokeOnMainThread(() => {
					lifeCounter.IsVisible = poisonCounter.IsVisible = false;
					poisonEntry.IsVisible = true;
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
						lifeCounter.IsVisible = poisonCounter.IsVisible = true;
						poisonEntry.IsVisible = false;
					}
					else
						Poison++;
				}
				maximize = false;
			};
		}
		public int Life
		{
			get => life.Value;
			set
			{
				life.Value = value;
				lifeCounter.Text = life.Value.ToString();
				lifeCounter.TextColor = life.GetTextColor();
			}
		}

		public int TickThreshold { get; set; } = 10;

		public int FastTickStep { get; set; } = 5;

		public IList<(int, Color)> LifeThresholds
		{
			get => life.Thresholds;
			set => life.Thresholds = value;
		}

		public int Poison
		{
			get => poison.Value;
			set
			{
				poison.Value = value;
				poisonCounter.Text = poisonEntry.Text = poison.Value.ToString();
				poisonCounter.TextColor = poisonEntry.TextColor = poison.GetTextColor();
			}
		}

		public (int Threshold, Color Color) PoisonThreshold
		{
			get => poison.Thresholds[0];
			set => poison.Thresholds[0] = value;
		}

		private void Increment(int value)
		{
			if (maximized)
				Poison += value;
			else
				Life += value;
		}

		public void Rotate(RotaryEventArgs args)
		{
			if (args.IsClockwise)
			{
				if (ticks >= 0)
					ticks++;
				else
					ticks = 1;
				if (ticks <= TickThreshold)
					Increment(1);
				else
					Increment(FastTickStep);
			}
			else
			{
				if (ticks <= 0)
					ticks--;
				else
					ticks = -1;
				if (ticks >= -TickThreshold)
					Increment(-1);
				else
					Increment(-FastTickStep);
			}

			resetTicks.Stop();
			resetTicks.Start();
		}
	}
}