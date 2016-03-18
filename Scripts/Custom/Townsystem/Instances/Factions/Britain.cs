using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class BritainFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public BritainFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					1,
                    true,
					2125, // blue 1325
					2125, // FLAG HUE: Red
					2125, // join stone : blue
					2125, // broadcast : blue
                    5651, 5652, 
					"Britain", "britain", "Britain",
					"Britain",
					"Britain Militia",
					"<center>Britain</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8744, //CRYSTAL ID
					"This city is controlled by Britain.",
					"This sigil has been corrupted by Britain",
					"The faction signup stone for Britain",
					"The Faction Stone of Britain",
					": Britain",
					"Members of Britain will now be ignored.",
					"Members of Britain will now be warned to leave.",
					"Members of Britain will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Britain"),
                        new RankDefinition(  9, 950, "Champion of Britain"),
                        new RankDefinition(  8, 900, "Imperial Knight of Britain"),
                        new RankDefinition(  7, 800, "Iron Templar of Britain"),
                        new RankDefinition(  6, 700, "Vindicator of Britain"),
                        new RankDefinition(  5, 600, "Sentinel of Britain"),
                        new RankDefinition(  4, 500, "Knight of Britain"),
                        new RankDefinition(  3, 400, "Defender of Britain"),
                        new RankDefinition(  2, 200, "Squire of Britain"),
                        new RankDefinition(  1,   0, "Squire of Britain")
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