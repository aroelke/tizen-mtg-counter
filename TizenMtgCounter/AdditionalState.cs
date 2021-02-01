using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TizenMtgCounter
{
	/// <summary>
	/// Additional miscellaneeous quantities of game state that can be tracked.
	/// </summary>
	public class AdditionalState
	{
		/// <summary>Experience counters.</summary>
		public static readonly AdditionalState Experience = new AdditionalState { Name = "experience" };
		/// <summary>Energy counters.</summary>
		public static readonly AdditionalState Energy = new AdditionalState { Name = "energy" };
		/// <summary>Lands played this turn.</summary>
		public static readonly AdditionalState Lands = new AdditionalState { Name = "lands" };
		/// <summary>Number of spells played this turn ("storm count").</summary>
		public static readonly AdditionalState Storm = new AdditionalState { Name = "storm" };

		/// <summary>
		/// List of miscellaneous quantities to track.
		/// </summary>
		public static IImmutableList<AdditionalState> Values => new List<AdditionalState> { Experience, Energy, Lands, Storm }.ToImmutableList();

		/// <summary>
		/// There are currently only four permitted quantities, so the constructor is private to prevent external instantiation.
		/// </summary>
		private AdditionalState() {}

		/// <summary>
		/// Gets the name of the quantity.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the file name of the quantity that corresponds to the image to display for it.
		/// </summary>
		public string ImageSource => Name + ".png";
	}
}
