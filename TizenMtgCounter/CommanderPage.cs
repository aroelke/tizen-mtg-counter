using System;
using System.Collections.Immutable;
using System.Linq;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CommanderPage : CounterPage<int>
	{
		private class CommanderTax
		{
			public int Tax { get; set; }
			public DarkenButton Button { get; set; }
		}

		private readonly CommanderTax left;
		private readonly CommanderTax right;
		private readonly DarkenButton[] buttons;
		private int opponents;
		private readonly Button addButton;

		public CommanderPage() : base(() => Enumerable.Range(0, 8).ToImmutableDictionary((i) => i + 1, (i) => new CounterData { Value = 0, Minimum = 0, Thresholds = { (20, Color.Default), (int.MaxValue, Color.Red) }}))
		{
			CommanderTax createTaxButton()
			{
				bool clear = false;
				CommanderTax t = new CommanderTax {
					Tax = 0,
					Button = new DarkenButton {
						Source = "0_mana.png",
						WidthRequest = 60,
						HeightRequest = 60,
						CornerRadius = 30
					}
				};
				Timer timer = new Timer {
					Interval = 500,
					Enabled = false,
					AutoReset = false
				};
				timer.Elapsed += (sender, e) => clear = true;

				t.Button.Pressed += (sender, e) => timer.Start();
				t.Button.Released += (sender, e) => {
					timer.Stop();
					t.Tax = clear ? 0 : t.Tax + 2;
					clear = false;
					t.Button.Source = t.Tax <= 20 ? $"{t.Tax}_mana.png" : "infinity_mana.png";
				};

				return t;
			}

			left = createTaxButton();
			right = createTaxButton();
			Children.Add(left.Button, (p) => (p.Width - p.GetSize(left.Button).Width)/2, Math.PI);
			Children.Add(right.Button, (p) => (p.Width - p.GetSize(right.Button).Width)/2, 0);

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
			if (left != null && right != null)
			{
				left.Tax = right.Tax = 0;
				left.Button.Source = right.Button.Source = "0_mana.png";
			}
			foreach (Label l in Labels.Values)
				l.IsVisible = false;
			if (buttons != null)
				foreach (DarkenButton b in buttons)
					b.IsVisible = false;
			opponents = 0;
		}
	}
}
