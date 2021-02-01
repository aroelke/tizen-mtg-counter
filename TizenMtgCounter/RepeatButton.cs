using System;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	/// <summary>
	/// <see cref="Button"/> that repeats its action while held down.
	/// </summary>
	public class RepeatButton : Button
	{
		/// <summary>
		/// Create a new <c>RepeatButton</c> with default repeat delay and time.
		/// </summary>
		public RepeatButton() : base()
		{
			Timer timer = new Timer
			{
				Enabled = false,
				AutoReset = true
			};
			timer.Elapsed += (sender, e) => {
				timer.Interval = Interval;
				Held?.Invoke(timer, e);
			};

			Pressed += (sender, e) => {
				timer.Interval = Delay;
				timer.Start();
			};
			Released += (sender, e) => timer.Stop();
		}

		/// <value>Gets or sets delay in ms after the button is pressed before repetition starts.</value>
		public double Delay { get; set; } = 100;

		/// <value>Gets or sets delay in ms between repetitions while the button is held.</value>
		public double Interval { get; set; } = 100;

		/// <summary>
		/// Occurs every <see cref="Delay"/> ms while the button is held after <see cref="Interval"/> ms of initial press.
		/// </summary>
		public event EventHandler Held;
	}
}
