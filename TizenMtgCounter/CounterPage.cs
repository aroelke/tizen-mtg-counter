using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Entry = Xamarin.Forms.Entry;
using Label = Xamarin.Forms.Label;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TizenMtgCounter
{
	/// <summary>
	/// Application page containing counters for tracking a player's game state.
	/// Counters are maximized by tapping their corresponding buttons on the edge of the screen, followed
	/// by either tapping the up/down arrow buttons or rotating the bezel. Counters can also be configured
	/// to change color as they increase or decrease. Holding the button for a counter (or pressing the one
	/// for another counter) minimizes it and holding the button longer resets it.
	/// </summary>
	/// <typeparam name="K">Type of data being counted.</typeparam>
	public class CounterPage<K> : BezelInteractionPage
	{
		protected readonly Counter<K> counter;
		private readonly Func<IImmutableDictionary<K, CounterData>> initialValues;
		private readonly K initialFocus;
		private readonly RelativeLayout layout;
		private IDictionary<K, Label> labels;
		private readonly CounterPopupEntry entry;
		private readonly RepeatButton plusButton;
		private readonly RepeatButton minusButton;

		/// <summary>
		/// Create a CounterPage for counting various values.
		/// </summary>
		/// <param name="initial">Mapping of data to be counted onto its initial/reset value.</param>
		/// <param name="focus">Which counter, if any, should start maximized. Leave as <c>default</c> to
		/// start with no counter maximized.</param>
		public CounterPage(Func<IImmutableDictionary<K, CounterData>> initial, K focus = default) : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			RotaryFocusObject = counter = new Counter<K>();
			counter.Data = (initialValues = initial)();
			counter.Selected = initialFocus = focus;
			labels = counter.Data.ToDictionary((e) => e.Key, (e) => {
				Label l = new Label { FontSize = 8 };
				l.SetBinding(Label.TextProperty, "Value");
				l.SetBinding(Label.TextColorProperty, "TextColor");
				l.BindingContext = e.Value;
				l.IsVisible = true;
				return l;
			});

			entry = new CounterPopupEntry {
				Text = counter.Value.ToString(),
				FontSize = 32,
				Keyboard = Keyboard.Numeric,
				BackgroundColor = Color.Transparent,
				HorizontalTextAlignment = TextAlignment.Center,
				IsVisible = counter.SelectedValid()
			};
			plusButton = new RepeatButton {
				Text = "\u25b2",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60,
				IsVisible = counter.SelectedValid()
			};
			plusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);
			minusButton = new RepeatButton {
				Text = "\u25bc",
				Delay = 500,
				Interval = 100,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 60,
				IsVisible = counter.SelectedValid()
			};
			minusButton.On<Xamarin.Forms.PlatformConfiguration.Tizen>().SetStyle(ButtonStyle.Text);

			entry.SetBinding(Entry.TextProperty, "Value", BindingMode.OneWay);
			entry.SetBinding(Entry.TextColorProperty, "TextColor", BindingMode.OneWay);
			entry.BindingContext = counter;

			counter.PropertyChanged += (sender, e) => {
				if (e.PropertyName == "SelectedValid")
					minusButton.IsVisible = plusButton.IsVisible = entry.IsVisible = counter.SelectedValid();
			};
			entry.Completed += (sender, e) => {
				if (int.TryParse(entry.Text, out int result))
					counter.Value = result;
			};
			plusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => counter.Value++);
			plusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => counter.Value++);
			minusButton.Pressed += (sender, e) => Device.BeginInvokeOnMainThread(() => counter.Value--);
			minusButton.Held += (sender, e) => Device.BeginInvokeOnMainThread(() => counter.Value--);

			Content = layout = new RelativeLayout();
			layout.Children.Add(
				entry,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(entry).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(entry).Height)/2)
			);
			layout.Children.Add(
				plusButton,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(plusButton).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(entry).Height - p.GetSize(plusButton).Height)/2)
			);
			layout.Children.Add(
				minusButton,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(minusButton).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height + p.GetSize(entry).Height - p.GetSize(minusButton).Height)/2)
			);
		}

		/// <summary>
		/// Gets the children of the page's layout.
		/// </summary>
		public RelativeLayout.IRelativeList<View> Children => layout.Children;

		/// <summary>
		/// Gets the labels showing the state of each counter.
		/// </summary>
		public IImmutableDictionary<K, Label> Labels => labels.ToImmutableDictionary();

		/// <summary>
		/// Add a new button for maximizing a particular counter.
		/// </summary>
		/// <param name="key">Counter value for the button to control.</param>
		/// <param name="button">Button to add.</param>
		/// <param name="x">x-coordinate within the screen.</param>
		/// <param name="y">y-coordinate within the screen.</param>
		/// <param name="w">Width of the button.</param>
		/// <param name="h">Height of the button.</param>
		public void AddButton(K key, ImageButton button, Constraint x = null, Constraint y = null, Constraint w = null, Constraint h = null)
		{
			bool clear = false;
			bool maximized = false;

			Timer timer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = true
			};
			timer.Elapsed += (sender, e) => Device.BeginInvokeOnMainThread(() => {
				if (clear)
				{
					counter[key] = 0;
					timer.AutoReset = false;
				}
				else if (!EqualityComparer<K>.Default.Equals(counter.Selected, key))
				{
					if (counter.SelectedValid())
						labels[counter.Selected].IsVisible = true;
					counter.Selected = key;
					labels[key].IsVisible = false;
					entry.IsVisible = true;
				}
				maximized = clear = true;
			});

			button.Pressed += (sender, e) => {
				clear = EqualityComparer<K>.Default.Equals(counter.Selected, key);
				maximized = false;
				timer.AutoReset = true;
				timer.Start();
			};

			button.Released += (sender, e) => {
				timer.Stop();
				if (!maximized)
				{
					if (EqualityComparer<K>.Default.Equals(counter.Selected, key))
					{
						counter.Selected = default;
						labels[key].IsVisible = true;
						entry.IsVisible = false;
					}
					else if (!clear)
						counter[key]++;
				}
				maximized = clear = false;
			};

			layout.Children.Add(button, x, y, w, h);
		}

		/// <summary>
		/// Add a button for maximizing a particular counter using radial coordinates centered at the center of the screen
		/// and center of the button.
		/// </summary>
		/// <param name="key">Counter value for the button to control.</param>
		/// <param name="button">Button to add.</param>
		/// <param name="r">Radius to add the button at.</param>
		/// <param name="theta">Angle to add the button at.</param>
		public void AddButton(K key, ImageButton button, Func<RelativeLayout, double> r, double theta = 0) => AddButton(
			key, button,
			Constraint.RelativeToParent((p) => (p.Width - p.GetSize(button).Width)/2 + r(p)*Math.Cos(theta)),
			Constraint.RelativeToParent((p) => (p.Height - p.GetSize(button).Height)/2 + r(p)*Math.Sin(theta))
		);

		public virtual void Clear()
		{
			counter.Data = initialValues();
			counter.Selected = initialFocus;
		}
	}

	/// <summary>
	/// Extensions class for adding things to <see cref="RelativeLayout"/>s.
	/// </summary>
	public static class CounterPageLayoutExtensions
	{
		/// <summary>
		/// Get the size of an item within a layout.
		/// </summary>
		/// <param name="v"><see cref="RelativeLayout"/> containing the item to measure.</param>
		/// <param name="view">Item to measure.</param>
		/// <returns>The size of the item.</returns>
		public static Size GetSize(this View v, View view) => new Size(view.Measure(v.Width, v.Height).Request.Width, view.Measure(v.Width, v.Height).Request.Height);

		/// <summary>
		/// Add an item to the layout using radial coordinates centered at the center of the layout and center of the item.
		/// </summary>
		/// <param name="children"><see cref="RelativeLayout.IRelativeList{T}"/> to add the item to.</param>
		/// <param name="view">Item to add.</param>
		/// <param name="r">Radius to add the item at.</param>
		/// <param name="theta">Angle to add the item at.</param>
		/// <param name="w">Width of the item.</param>
		/// <param name="h">Height of the item.</param>
		public static void Add(this RelativeLayout.IRelativeList<View> children, View view, double r, double theta, Constraint w = null, Constraint h = null)
		{
			children.Add(view,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(view).Width)/2 + r*Math.Cos(theta)),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(view).Height)/2 + r*Math.Sin(theta)),
				w, h
			);
		}

		/// <summary>
		/// Add an item to the layout using radial coordinates centered at the center of the layout and center of the item.
		/// </summary>
		/// <param name="children"><see cref="RelativeLayout.IRelativeList{T}"/> to add the item to.</param>
		/// <param name="view">Item to add.</param>
		/// <param name="r">Function providing the radius to add the item at.</param>
		/// <param name="theta">Angle to add the item at.</param>
		/// <param name="w">Width of the item.</param>
		/// <param name="h">Height of the item.</param>
		public static void Add(this RelativeLayout.IRelativeList<View> children, View view, Func<RelativeLayout, double> r, double theta, Constraint w = null, Constraint h = null)
		{
			children.Add(view,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(view).Width)/2 + r(p)*Math.Cos(theta)),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(view).Height)/2 + r(p)*Math.Sin(theta)),
				w, h
			);
		}
	}
}