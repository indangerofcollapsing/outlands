using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class CoveFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public CoveFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					2,
                    false,
					1109, // blue
                    1109, // FLAG HUE: Blue
                    1109, // join stone : blue
                    1109, // broadcast : blue
                    5659, 5660,
					"Cove", "Cove", "Cove",
					"Cove",
					"Cove faction",
					"<center>Cove</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Cove.",
					"This sigil has been corrupted by Cove",
					"The faction signup stone for Cove",
					"The Faction Stone of Cove",
					": Cove",
					"Members of Cove will now be ignored.",
					"Members of Cove will now be warned to leave.",
					"Members of Cove will now be beaten with a stick.",

					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Cove"),
                        new RankDefinition(  9, 950, "Champion of Cove"),
                        new RankDefinition(  8, 900, "Black Knight of Cove"),
                        new RankDefinition(  7, 800, "Reaver of Cove"),
                        new RankDefinition(  6, 700, "Commander of Cove"),
                        new RankDefinition(  5, 600, "Dragon of Cove"),
                        new RankDefinition(  4, 500, "Knight-Captain of Cove"),
                        new RankDefinition(  3, 400, "Knight of Cove"),
                        new RankDefinition(  2, 200, "Defender of Cove"),
                        new RankDefinition(  1,   0, "Defender of Cove")
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