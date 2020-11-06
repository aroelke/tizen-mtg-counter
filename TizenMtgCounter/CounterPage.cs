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

		public Size GetSize(View view) => new Size(view.Measure(Width, Height).Request.Width, view.Measure(Width, Height).Request.Height);

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

		public RelativeLayout.IRelativeList<View> Children { get => layout.Children; }

		public virtual void Clear()
		{
			counter.Data = initialValues.ToImmutableDictionary((e) => e.Key, (e) => new CounterData(e.Value));
			counter.Selected = initialFocus;
			foreach (Label l in counter.Labels.Values)
				l.IsVisible = true;
		}
	}
}
