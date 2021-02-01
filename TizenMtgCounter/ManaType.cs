using System.Collections.Generic;
using System.Collections.Immutable;

namespace TizenMtgCounter
{
	/// <summary>
	/// Type of mana that can be generated in Magic: The Gathering.
	/// </summary>
	public class ManaType
	{
		/// <summary>Colorless mana.</summary>
		public static readonly ManaType Colorless = new ManaType { Name = "colorless" };
		/// <summary>White mana.</summary>
		public static readonly ManaType White = new ManaType { Name = "white" };
		/// <summary>Blue mana.</summary>
		public static readonly ManaType Blue = new ManaType { Name = "blue" };
		/// <summary>Black mana.</summary>
		public static readonly ManaType Black = new ManaType { Name = "black" };
		/// <summary>Red mana.</summary>
		public static readonly ManaType Red = new ManaType { Name = "red" };
		/// <summary>Green mana.</summary>
		public static readonly ManaType Green = new ManaType { Name = "green" };

		/// <summary>
		/// List of all types of mana in color order.
		/// The order is:
		/// <list type="number">
		/// <item><see cref="Colorless"/></item>
		/// <item><see cref="White"/></item>
		/// <item><see cref="Blue"/></item>
		/// <item><see cref="Black"/></item>
		/// <item><see cref="Red"/></item>
		/// <item><see cref="Green"/></item>
		/// </list>
		/// </summary>
		public static IImmutableList<ManaType> Values => new List<ManaType> { Colorless, White, Blue, Black, Red, Green }.ToImmutableList();

		/// <summary>
		/// There are only six permitted types of mana, so the constructor is private to prevent external instantiation.
		/// </summary>
		private ManaType() {}

		/// <summary>
		/// Gets the name of the type of mana.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the file name of the type of mana that corresponds to the image to display for it.
		/// </summary>
		public string ImageSource => Name + "_mana.png";
	}
}
