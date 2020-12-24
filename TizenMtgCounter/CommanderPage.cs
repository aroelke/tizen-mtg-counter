using System;
using System.Collections.Immutable;
using System.Linq;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CommanderPage : CounterPage<int>
	{
		private int leftTax;
		private int rightTax;
		private readonly DarkenButton[] buttons;
		private int opponents;
		private readonly Button addButton;

		public CommanderPage() : base(() => Enumerable.Range(0, 8).ToImmutableDictionary((i) => i + 1, (i) => new CounterData { Value = 0, Minimum = 0, Thresholds = { (20, Color.Default), (int.MaxValue, Color.Red) }}))
		{
			leftTax = rightTax = 0;

			bool leftClear = false;
			DarkenButton leftButton = new DarkenButton {
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			leftButton.SetBinding(ImageButton.SourceProperty, "LeftButtonSource", BindingMode.OneWay);
			leftButton.BindingContext = this;
			Timer leftTimer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			leftTimer.Elapsed += (sender, e) => leftClear = true;
			leftButton.Pressed += (sender, e) => leftTimer.Start();
			leftButton.Released += (sender, e) => {
				leftTimer.Stop();
				LeftTax = leftClear ? 0 : LeftTax + 2;
				leftClear = false;
			};
			Children.Add(leftButton, (p) => (p.Width - p.GetSize(leftButton).Width)/2, Math.PI);

			bool rightClear = false;
			DarkenButton rightButton = new DarkenButton {
				WidthRequest = 60,
				HeightRequest = 60,
				CornerRadius = 30
			};
			rightButton.SetBinding(ImageButton.SourceProperty, "RightButtonSource", BindingMode.OneWay);
			rightButton.BindingContext = this;
			Timer rightTimer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			rightTimer.Elapsed += (sender, e) => rightClear = true;
			rightButton.Pressed += (sender, e) => leftTimer.Start();
			rightButton.Released += (sender, e) => {
				rightTimer.Stop();
				RightTax = rightClear ? 0 : RightTax + 2;
				rightClear = false;
			};
			Children.Add(rightButton, (p) => (p.Width - p.GetSize(rightButton).Width)/2, 0);

			double theta = Math.PI/6;
			buttons = new DarkenButton[counter.Data.Count];
			opponents = 0;
			for (int i = 0; i < counter.Data.Count; i++)
			{
				buttons[i] = new DarkenButton {
					WidthRequest = 45,
					HeightRequest = 45,
					CornerRadius = 22,
					IsVisible = false
				};
				AddButton(i + 1, buttons[i], theta - Math.PI/2);
				Label l = Labels[i + 1];
				l.IsVisible = false;
				Children.Add(
					l,
					(p) => (p.Width - Math.Max(p.GetSize(l).Width, p.GetSize(l).Height))/2 - 45,
					theta - Math.PI/2
				);
				if (i % 2 == 0)
					theta += Math.PI/6;
				else
					theta += Math.PI/3;
			}

			AddOpponentPage addPage = new AddOpponentPage { CommanderPage = this };
			addButton = new Button { Text = "+" };
			addButton.Clicked += async (sender, e) => {
				if (opponents < counter.Data.Count)
					await Navigation.PushAsync(addPage);
			};
			Children.Add(addButton, (p) => (p.Width - p.GetSize(addButton).Height)/2, 3*Math.PI/2);

			Button removeButton = new Button { Text = "-" };
			removeButton.Clicked += (sender, e) => RemoveOpponent();
			Children.Add(removeButton, (p) => (p.Width - p.GetSize(removeButton).Height)/2, Math.PI/2);
		}

		public int LeftTax
		{
			get => leftTax;
			set
			{
				if (value != leftTax)
				{
					leftTax = value;
					OnPropertyChanged("LeftButtonSource");
				}
			}
		}

		public int RightTax
		{
			get => rightTax;
			set
			{
				if (value != rightTax)
				{
					rightTax = value;
					OnPropertyChanged("RightButtonSource");
				}
			}
		}

		public FileImageSource LeftButtonSource => leftTax <= 20 ? $"{leftTax}_mana.png" : "infinity_mana.png";

		public FileImageSource RightButtonSource => rightTax <= 20 ? $"{rightTax}_mana.png" : "infinity_mana.png";

		public void AddOpponent(ImageSource source)
		{
			buttons[opponents].Source = source;
			buttons[opponents].IsVisible = true;
			opponents++;
			Labels[opponents].IsVisible = true;
		}

		private void RemoveOpponent()
		{
			if (opponents > 0)
			{
				if (counter.Selected == opponents)
					counter.Selected = default;
				Labels[opponents].IsVisible = false;
				counter[opponents] = 0;
				opponents--;
				buttons[opponents].IsVisible = false;
			}
		}

		public override void Clear()
		{
			base.Clear();
			LeftTax = RightTax = 0;
			foreach (Label l in Labels.Values)
				l.IsVisible = false;
			foreach (DarkenButton b in buttons)
				b.IsVisible = false;
			opponents = 0;
		}
	}
}
