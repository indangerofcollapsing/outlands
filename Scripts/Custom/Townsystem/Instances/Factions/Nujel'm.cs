using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class NujelmFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public NujelmFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					7,
                    false,
					2118, // blue
                    2118, // FLAG HUE: Blue
                    2118, // join stone : blue
                    2118, // broadcast : blue
                    5678, 5671,
					"Nujel'm", "Nujel'm", "Nujel'm",
					"Nujel'm",
					"Nujel'm faction",
					"<center>Nujel'm</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Nujel'm.",
					"This sigil has been corrupted by Nujel'm",
					"The faction signup stone for Nujel'm",
					"The Faction Stone of Nujel'm",
					": Nujel'm",
					"Members of Nujel'm will now be ignored.",
					"Members of Nujel'm will now be warned to leave.",
					"Members of Nujel'm will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Nujel'm"),
                        new RankDefinition(  9, 950, "Champion of Nujel'm"),
                        new RankDefinition(  8, 900, "Archblade of Nujel’m"),
                        new RankDefinition(  7, 800, "Sacred Sword of Nujel'm"),
                        new RankDefinition(  6, 700, "Mystic Blade of Nujel’m"),
                        new RankDefinition(  5, 600, "Nujel'm Sentinel"),
                        new RankDefinition(  4, 500, "Knight of Nujel'm"),
                        new RankDefinition(  3, 400, "Defender of Nujel'm"),
                        new RankDefinition(  2, 200, "Peasant Warrior of Nujel'm"),
                        new RankDefinition(  1,   0, "Peasant Warrior of Nujel'm")
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