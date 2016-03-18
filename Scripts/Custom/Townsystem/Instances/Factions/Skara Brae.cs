using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class SkaraBraeFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public SkaraBraeFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					10,
                    false,
                    1196, // blue
                    1196, // FLAG HUE: Blue
                    1196, // join stone : blue
                    1196, // broadcast : blue
                    5675,
                    5677,
					"Skara Brae", "Skara Brae", "Skara Brae",
					"Skara Brae",
					"Skara Brae faction",
					"<center>Skara Brae</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Skara Brae.",
					"This sigil has been corrupted by Skara Brae",
					"The faction signup stone for Skara Brae",
					"The Faction Stone of Skara Brae",
					": Skara Brae",
					"Members of Skara Brae will now be ignored.",
					"Members of Skara Brae will now be warned to leave.",
					"Members of Skara Brae will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Skara Brae"),
                        new RankDefinition(  9, 950, "Ranger-Lord of Skara Brae"),
                        new RankDefinition(  8, 900, "Bladesinger of Skara Brae"),
                        new RankDefinition(  7, 800, "Ranger of Skara Brae"),
                        new RankDefinition(  6, 700, "Titan Blade of Skara Brae"),
                        new RankDefinition(  5, 600, "Knight-Captain of Skara Brae"),
                        new RankDefinition(  4, 500, "Knight of Skara Brae"),
                        new RankDefinition(  3, 400, "Nobleman of Skara Brae"),
                        new RankDefinition(  2, 200, "Defender of Skara Brae"),
                        new RankDefinition(  1,   0, "Defender of Skara Brae")
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