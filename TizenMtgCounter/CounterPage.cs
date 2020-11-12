using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Timers;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	public class CounterPage<K> : BezelInteractionPage
	{
		protected readonly Counter<K> counter;
		private readonly IImmutableDictionary<K, CounterData> initialValues;
		private readonly K initialFocus;
		private readonly RelativeLayout layout;

		public CounterPage() : this(ImmutableDictionary<K, CounterData>.Empty) {}

		public CounterPage(IImmutableDictionary<K, CounterData> initial, K focus = default) : base()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			RotaryFocusObject = counter = new Counter<K>();
			initialValues = initial;
			initialFocus = focus;
			Clear();

			Content = layout = new RelativeLayout();
			layout.Children.Add(
				counter.Content,
				Constraint.RelativeToParent((p) => (p.Width - GetSize(counter.Content).Width)/2),
				Constraint.RelativeToParent((p) => (p.Height - GetSize(counter.Content).Height)/2)
			);
		}

		public Size GetSize(View view) => Content.GetSize(view);

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
						counter.Labels[key].IsVisible = true;
					}
					else
					{
						if (counter.SelectedValid())
							counter.Labels[counter.Selected].IsVisible = true;
						counter.Selected = key;
						counter.Labels[key].IsVisible = false;
					}
				}
			};

			layout.Children.Add(button, x, y, w, h);
		}

		public void AddButton(K key, ImageButton button, double theta = 0) => AddButton(
			key, button,
			Constraint.RelativeToParent((p) => (p.Width - GetSize(button).Width)/2*(1 + Math.Cos(theta))),
			Constraint.RelativeToParent((p) => (p.Height - GetSize(button).Height)/2*(1 + Math.Sin(theta)))
		);

		public RelativeLayout.IRelativeList<View> Children => layout.Children;

		public virtual void Clear()
		{
			counter.Data = initialValues.ToImmutableDictionary((e) => e.Key, (e) => new CounterData(e.Value));
			counter.Selected = initialFocus;
			foreach (Label l in counter.Labels.Values)
				l.IsVisible = true;
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