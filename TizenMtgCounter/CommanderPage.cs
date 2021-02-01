using System;
using System.Collections.Immutable;
using System.Linq;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page useful for tracking damage from opposing commanders.
	/// A user can add or remove up to 8 counters with customizable icons. Also includes buttons for tracking "commander taxes."
	/// Note that only increments of 2 are supported for taxes.
	/// </summary>
	public class CommanderPage : CounterPage<int>
	{
		/// <summary>Size of the buttons for adding or removing opponents.</summary>
		public const int OpponentButtonSize = 60;
		/// <summary>Size of the commander tax buttons.</summary>
		public const int TaxButtonSize = 60;
		/// <summary>Size of the commander damage buttons.</summary>
		public const int DamageButtonSize = 45;
		/// <summary>Distance from the edge of the screen to display buttons.</summary>
		public const int ButtonOffset = 5;

		private int leftTax;
		private int rightTax;
		private readonly DarkenButton[] buttons;
		private int opponents;
		private readonly DarkenButton addButton;

		/// <summary>
		/// Create a new <c>CommanderPage</c> with no visible opponents and zero commander tax.
		/// </summary>
		public CommanderPage() : base(() => Enumerable.Range(0, 8).ToImmutableDictionary((i) => i + 1, (i) => new CounterData { Value = 0, Minimum = 0, Thresholds = { (20, Color.Default), (int.MaxValue, Color.Red) }}))
		{
			leftTax = rightTax = 0;

			bool leftClear = false;
			DarkenButton leftButton = new DarkenButton {
				WidthRequest = TaxButtonSize,
				HeightRequest = TaxButtonSize,
				CornerRadius = TaxButtonSize/2
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
			Children.Add(leftButton, (p) => (p.Width - TaxButtonSize)/2 - ButtonOffset, Math.PI);

			bool rightClear = false;
			DarkenButton rightButton = new DarkenButton {
				WidthRequest = TaxButtonSize,
				HeightRequest = TaxButtonSize,
				CornerRadius = TaxButtonSize/2
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
			Children.Add(rightButton, (p) => (p.Width - TaxButtonSize)/2 - ButtonOffset, 0);

			double theta = Math.PI/6;
			buttons = new DarkenButton[counter.Data.Count];
			opponents = 0;
			for (int i = 0; i < counter.Data.Count; i++)
			{
				buttons[i] = new DarkenButton {
					WidthRequest = DamageButtonSize,
					HeightRequest = DamageButtonSize,
					CornerRadius = DamageButtonSize/2,
					IsVisible = false
				};
				AddButton(i + 1, buttons[i], (p) => (p.Width - DamageButtonSize)/2 - ButtonOffset, theta - Math.PI/2);
				Label l = Labels[i + 1];
				l.IsVisible = false;
				Children.Add(
					l,
					(p) => (p.Width - Math.Max(p.GetSize(l).Width, p.GetSize(l).Height))/2 - DamageButtonSize - ButtonOffset,
					theta - Math.PI/2
				);
				if (i % 2 == 0)
					theta += Math.PI/6;
				else
					theta += Math.PI/3;
			}

			AddOpponentPage addPage = new AddOpponentPage { CommanderPage = this };
			addButton = new DarkenButton {
				Source = "add.png", // (part of) icon by Pixel perfect from flaticon.com
				WidthRequest = OpponentButtonSize
			};
			addButton.Clicked += async (sender, e) => {
				if (opponents < counter.Data.Count)
					await Navigation.PushAsync(addPage);
			};
			Children.Add(addButton, (p) => (p.Width - p.GetSize(addButton).Height)/2 - ButtonOffset, 3*Math.PI/2);

			DarkenButton removeButton = new DarkenButton {
				Source = "remove.png", // (part of) icon by Pixel perfect from flaticon.com
				WidthRequest = OpponentButtonSize,
			};
			removeButton.Clicked += (sender, e) => RemoveOpponent();
			Children.Add(removeButton, (p) => (p.Width - p.GetSize(removeButton).Height)/2 - ButtonOffset, Math.PI/2);
		}

		/// <summary>
		/// Gets or sets the tax of the left counter.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the tax of the right counter.
		/// </summary>
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

		/// <summary>
		/// Gets the image file to be used for the left tax button.
		/// </summary>
		public FileImageSource LeftButtonSource => leftTax <= 20 ? $"{leftTax}_mana.png" : "infinity_mana.png";

		/// <summary>
		/// Gets the image file to be used for the right tax button.
		/// </summary>
		public FileImageSource RightButtonSource => rightTax <= 20 ? $"{rightTax}_mana.png" : "infinity_mana.png";

		/// <summary>
		/// Add a new opponent. New opponents are added clockwise starting from the top of the screen.
		/// </summary>
		/// <param name="source">Icon to use to represent the new opponent.</param>
		public void AddOpponent(ImageSource source)
		{
			buttons[opponents].Source = source;
			buttons[opponents].IsVisible = true;
			opponents++;
			Labels[opponents].IsVisible = true;
		}

		/// <summary>
		/// Remove the last opponent that was added.
		/// </summary>
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

		/// <summary>
		/// Remove all opponents and set both taxes to 0.
		/// </summary>
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
