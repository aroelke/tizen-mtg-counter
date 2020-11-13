using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

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

		public CounterPage() : this(() => ImmutableDictionary<K, CounterData>.Empty) {}

		public CounterPage(Func<IImmutableDictionary<K, CounterData>> initial, K focus = default) : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			RotaryFocusObject = counter = new Counter<K>();
			initialValues = initial;
			initialFocus = focus;
			labels = new Dictionary<K, Label>();
			Clear();
			entry = new CounterPopupEntry {
				Text = counter.Value.ToString(),
				FontSize = 32,
				Keyboard = Keyboard.Numeric,
				BackgroundColor = Color.Transparent,
				HorizontalTextAlignment = TextAlignment.Center,
				IsVisible = counter.SelectedValid()
			};
			entry.Completed += (sender, e) => {
				if (int.TryParse(entry.Text, out int result))
					counter.Value = result;
			};

			counter.PropertyChanged += (sender, e) => {
				switch (e.PropertyName)
				{
				case "Value":
					entry.Text = counter.Value.ToString();
					break;
				case "TextColor":
					entry.TextColor = counter.TextColor;
					break;
				case "SelectedValid":
					entry.IsVisible = counter.SelectedValid();
					break;
				}
			};

			Content = layout = new RelativeLayout();
			layout.Children.Add(
				entry,
				Constraint.RelativeToParent((p) => (p.Width - p.GetSize(entry).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - p.GetSize(entry).Height)/2)
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

			button.Pressed += (sender, e) => {
				button.Opacity = 1.0/3.0;
				timer.Start();
			};

			button.Released += (sender, e) => {
				button.Opacity = 1;
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

		public void AddButton(K key, ImageButton button, double theta = 0) => AddButton(
			key, button,
			Constraint.RelativeToParent((p) => (p.Width - p.GetSize(button).Width)/2*(1 + Math.Cos(theta))),
			Constraint.RelativeToParent((p) => (p.Height - p.GetSize(button).Height)/2*(1 + Math.Sin(theta)))
		);

		public virtual void Clear()
		{
			var initial = initialValues();
			counter.Data = initial;
			labels = labels.Where((e) => initial.ContainsKey(e.Key)).ToDictionary((e) => e.Key, (e) => e.Value);
			foreach (K key in initial.Keys)
			{
				if (!labels.ContainsKey(key))
				{
					labels[key] = new Label { FontSize = 8 };
					labels[key].SetBinding(Label.TextProperty, "Value");
					labels[key].SetBinding(Label.TextColorProperty, "TextColor");
				}
				labels[key].BindingContext = initial[key];
				labels[key].IsVisible = true;
			}
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