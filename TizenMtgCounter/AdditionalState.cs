using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TizenMtgCounter
{
	public class AdditionalState
	{
		public static readonly AdditionalState Experience = new AdditionalState { Name = "experience" };
		public static readonly AdditionalState Energy = new AdditionalState { Name = "energy" };
		public static readonly AdditionalState Lands = new AdditionalState { Name = "lands" };
		public static readonly AdditionalState Storm = new AdditionalState { Name = "storm" };

		public static IImmutableList<AdditionalState> Values => new List<AdditionalState> { Experience, Energy, Lands, Storm }.ToImmutableList();

		private AdditionalState() {}

		public string Name { get; private set; }

		public string ImageSource => Name + ".png";
	}
}
