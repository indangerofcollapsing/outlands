using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class MinocFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public MinocFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					5,
                    false,
					2123, // blue
                    2123, // FLAG HUE: Blue
                    2123, // join stone : blue
                    2123, // broadcast : blue
                    5657, 5658,
					"Minoc", "Minoc", "Minoc",
					"Minoc",
					"Minoc faction",
					"<center>Minoc</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Minoc.",
					"This sigil has been corrupted by Minoc",
					"The faction signup stone for Minoc",
					"The Faction Stone of Minoc",
					": Minoc",
					"Members of Minoc will now be ignored.",
					"Members of Minoc will now be warned to leave.",
					"Members of Minoc will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Minoc"),
                        new RankDefinition( 9, 950, "Champion of Minoc"),
                        new RankDefinition( 8, 900, "Swordmaster of Minoc"),
                        new RankDefinition( 7, 800, "Avenger of Minoc"),
                        new RankDefinition( 6, 700, "Minoc Zealot"),
                        new RankDefinition( 5, 600, "Warden of Minoc"),
                        new RankDefinition( 4, 500, "Knight of Minoc"),
                        new RankDefinition( 3, 400, "Noble of Minoc"),
                        new RankDefinition( 2, 200, "Militiaman of Minoc"),
                        new RankDefinition( 1, 0, "Militiaman of Minoc")
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