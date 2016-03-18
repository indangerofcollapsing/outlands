using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class VesperFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public VesperFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					12,
                    true,
                    1366, // blue
                    1366, // FLAG HUE: Blue
                    1366, // join stone : blue
                    1366, // broadcast : blue
                    5678,
                    5674,
					"Vesper", "Vesper", "Vesper",
					"Vesper",
					"Vesper faction",
					"<center>Vesper</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Vesper.",
					"This sigil has been corrupted by Vesper",
					"The faction signup stone for Vesper",
					"The Faction Stone of Vesper",
					": Vesper",
					"Members of Vesper will now be ignored.",
					"Members of Vesper will now be warned to leave.",
					"Members of Vesper will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Vesper"),
                        new RankDefinition( 9, 950, "Champion of Vesper"),
                        new RankDefinition( 8, 900, "Warder of Vesper"),
                        new RankDefinition( 7, 800, "Avenger of Vesper"),
                        new RankDefinition( 6, 700, "Vesper Zealot"),
                        new RankDefinition( 5, 600, "Vesper Dragoon"),
                        new RankDefinition( 4, 500, "Vesper Knight"),
                        new RankDefinition( 3, 400, "Squire of Vesper"),
                        new RankDefinition( 2, 200, "Servant of Vesper"),
                        new RankDefinition( 1, 0, "Servant of Vesper")
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