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

		public CounterPage() : this(() => ImmutableDictionary<K, CounterData>.Empty) {}

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

		public RelativeLayout.IRelativeList<View> Children => layout.Children;

		public IImmutableDictionary<K, Label> Labels => labels.ToImmutableDictionary();

		public void AddButton(K key, ImageButton button, Constraint x = null, Constraint y = null, Constraint w = null, Constraint h = null)
		{
			Timer timer = new Timer {
				Interval = 500,
				Enabled = false,
				AutoReset = false
			};
			timer.Elapsed += (sender, e) => Device.BeginInvokeOnMainThread(() => counter[key] = 0);

			button.Pressed += (sender, e) => timer.Start();

			button.Released += (sender, e) => {
				if (timer.Enabled)
				{
					timer.Stop();

					if (EqualityComparer<K>.Default.Equals(counter.Selected, key))
					{
						counter.Selected = default;
						labels[key].IsVisible = true;
						entry.IsVisible = false;
					}
					else
					{
						if (counter.SelectedValid())
							labels[counter.Selected].IsVisible = true;
						counter.Selected = key;
						labels[key].IsVisible = false;
						entry.IsVisible = true;
					}
				}
			};

			layout.Children.Add(button, x, y, w, h);
		}

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

	public static class CounterPageLayoutExtensions
	{
		public static Size GetSize(this View v, View view) => new Size(view.Measure(v.Width, v.Height).Request.Width, view.Measure(v.Width, v.Height).Request.Height);

		public static void Add(this RelativeLayout.IRelativeList<View> children, View view, double r, double theta, Constraint w = null, Constraint h = null)
		{
			children.Add(view,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(view).Width)/2 + r*Math.Cos(theta)),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(view).Height)/2 + r*Math.Sin(theta)),
				w, h
			);
		}

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