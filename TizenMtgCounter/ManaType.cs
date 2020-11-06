using System.Collections.Generic;
using System.Collections.Immutable;

namespace TizenMtgCounter
{
	public class ManaType
	{
		public static readonly ManaType Colorless = new ManaType { Name = "colorless" };
		public static readonly ManaType White = new ManaType { Name = "white" };
		public static readonly ManaType Blue = new ManaType { Name = "blue" };
		public static readonly ManaType Black = new ManaType { Name = "black" };
		public static readonly ManaType Red = new ManaType { Name = "red" };
		public static readonly ManaType Green = new ManaType { Name = "green" };

		public static IImmutableList<ManaType> Values => new List<ManaType> { Colorless, White, Blue, Black, Red, Green }.ToImmutableList();

		private ManaType() {}

		private string Name { get; set; }

		public string ImageSource { get => Name + "_mana.png"; }
	}
}
