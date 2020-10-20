using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace TizenMtgCounter
{
	class RepeatButton : Button
	{
		private readonly Timer timer;

		public RepeatButton() : base()
		{
			timer = new Timer
			{
				Enabled = false,
				AutoReset = true
			};
			timer.Elapsed += (sender, e) => {
				timer.Interval = Interval;
				Held.Invoke(timer, e);
			};

			Pressed += (sender, e) => {
				timer.Interval = Delay;
				timer.Start();
			};
			Released += (sender, e) => timer.Stop();
		}

		public double Delay { get; set; } = 100;

		public double Interval { get; set; } = 100;

		public event EventHandler Held;
	}
}
