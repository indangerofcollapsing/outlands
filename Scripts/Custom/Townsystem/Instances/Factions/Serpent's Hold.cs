using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class SerpentsHoldFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public SerpentsHoldFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					9,
                    false,
                    1157, // dark red
                    1157, // FLAG HUE: dark red
                    1157, // join stone : dark red
                    1157, // broadcast : dark red
                    5651, 5652,
					"Serpent's Hold", "Serpent's Hold", "Serpent's Hold",
					"Serpent's Hold",
					"Serpent's Hold faction",
					"<center>Serpent's Hold</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Serpent's Hold.",
					"This sigil has been corrupted by Serpent's Hold",
					"The faction signup stone for Serpent's Hold",
					"The Faction Stone of Serpent's Hold",
					": Serpent's Hold",
					"Members of Serpent's Hold will now be ignored.",
					"Members of Serpent's Hold will now be warned to leave.",
					"Members of Serpent's Hold will now be beaten with a stick.",
					
					new RankDefinition[]
					{
                        new RankDefinition( 10, 991, "Hero of Serpent's Hold"),
                        new RankDefinition(  9, 950, "Wyrm Knight of Serpent's Hold"),
                        new RankDefinition(  8, 900, "Paragon of Serpent's Hold"),
                        new RankDefinition(  7, 800, "Commander of Serpent's Hold"),
                        new RankDefinition(  6, 700, "Serpent Guard"),
                        new RankDefinition(  5, 600, "Dragon of Serpent's Hold"),
                        new RankDefinition(  4, 500, "Knight of Serpent's Hold"),
                        new RankDefinition(  3, 400, "Defender of Serpent's Hold"),
                        new RankDefinition(  2, 200, "Squire of Serpent's Hold"),
                        new RankDefinition(  1,   0, "Pawn of Serpent's Hold")
					},
					new GuardDefinition[]
					{
						new GuardDefinition( typeof( FactionHenchman ),		0x1403, 1500, 150, 10,		"HENCHMAN", "Hire Henchman"),
						new GuardDefinition( typeof( FactionMercenary ),	0x0F62, 2000, 200, 10,		new TextDefinition( 1011527, "MERCENARY" ),		new TextDefinition( 1011511, "Hire Mercenary" ) ),
						new GuardDefinition( typeof( FactionSorceress ),	0x0E89, 2500, 250, 10,		new TextDefinition( 1011507, "SORCERESS" ),		new TextDefinition( 1011501, "Hire Sorceress" ) ),
					    new GuardDefinition( typeof( FactionWizard ),		0x13F8, 3000, 300, 10,		new TextDefinition( 1011508, "ELDER WIZARD" ),	new TextDefinition( 1011502, "Hire Elder Wizard" ) ),
					}
				);
		}
	}
}