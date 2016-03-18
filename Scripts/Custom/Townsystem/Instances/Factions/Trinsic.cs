using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class TrinsicFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public TrinsicFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					11,
                    false,
                    1151, // blue
                    1151, // FLAG HUE: Blue
                    1151, // join stone : blue
                    1151, // broadcast : blue
                    5655, 5656,
					"Trinsic", "Trinsic", "Trinsic",
					"Trinsic",
					"Trinsic faction",
					"<center>Trinsic</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Trinsic.",
					"This sigil has been corrupted by Trinsic",
					"The faction signup stone for Trinsic",
					"The Faction Stone of Trinsic",
					": Trinsic",
					"Members of Trinsic will now be ignored.",
					"Members of Trinsic will now be warned to leave.",
					"Members of Trinsic will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Trinsic"),
                        new RankDefinition( 9, 950, "Grand Crusader of Trinsic"),
                        new RankDefinition( 8, 900, "Sacred Sword of Trinsic"),
                        new RankDefinition( 7, 800, "Crusader of Trinsic"),
                        new RankDefinition( 6, 700, "Trinsic's Royal Guard"),
                        new RankDefinition( 5, 600, "Keeper of Trinsic"),
                        new RankDefinition( 4, 500, "Silver Knight of Trinsic"),
                        new RankDefinition( 3, 400, "Knight of Trinsic"),
                        new RankDefinition( 2, 200, "Defender of Trinsic"),
                        new RankDefinition( 1, 0, "Squire of Trinsic")
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