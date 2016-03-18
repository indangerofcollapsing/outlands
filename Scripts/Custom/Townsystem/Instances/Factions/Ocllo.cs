using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class OclloFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public OclloFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					8,
                    false,
					1156, // blue
                    1156, // FLAG HUE: Blue
					1156, // join stone : blue
                    1156, // broadcast : blue
                    5667,  5669,
					"Ocllo", "Ocllo", "Ocllo",
					"Ocllo",
					"Ocllo faction",
					"<center>Ocllo</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Ocllo.",
					"This sigil has been corrupted by Ocllo",
					"The faction signup stone for Ocllo",
					"The Faction Stone of Ocllo",
					": Ocllo",
					"Members of Ocllo will now be ignored.",
					"Members of Ocllo will now be warned to leave.",
					"Members of Ocllo will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Ocllo"),
                        new RankDefinition(  9, 950, "Champion of Ocllo"),
                        new RankDefinition(  8, 900, "Deadly Blade of Ocllo"),
                        new RankDefinition(  7, 800, "Battlemaster of Ocllo"),
                        new RankDefinition(  6, 700, "Assassin of Ocllo"),
                        new RankDefinition(  5, 600, "Ocllo Sentinel"),
                        new RankDefinition(  4, 500, "Knight of Ocllo"),
                        new RankDefinition(  3, 400, "Defender of Ocllo"),
                        new RankDefinition(  2, 200, "Peasant-Warrior of Ocllo"),
                        new RankDefinition(  1,   0, "Peasant-Warrior of Ocllo")
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