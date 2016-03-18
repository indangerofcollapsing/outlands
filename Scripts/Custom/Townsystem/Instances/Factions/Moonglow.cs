using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class MoonglowFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public MoonglowFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					6,
                    true,
					1278, // blue
                    1278, // FLAG HUE: Blue
                    1278, // join stone : blue
                    1278, // broadcast : blue
                    5647, 5648,
					"Moonglow", "Moonglow", "Moonglow",
					"Moonglow",
					"Moonglow faction",
					"<center>Moonglow</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Moonglow.",
					"This sigil has been corrupted by Moonglow",
					"The faction signup stone for Moonglow",
					"The Faction Stone of Moonglow",
					": Moonglow",
					"Members of Moonglow will now be ignored.",
					"Members of Moonglow will now be warned to leave.",
					"Members of Moonglow will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Moonglow"),
                        new RankDefinition(  9, 950, "Champion of Moonglow"),
                        new RankDefinition(  8, 900, "Celestial Herald of Moonglow"),
                        new RankDefinition(  7, 800, "Might of Moonglow"),
                        new RankDefinition(  6, 700, "Moonglow Mystic"),
                        new RankDefinition(  5, 600, "Acolyte of Moonglow"),
                        new RankDefinition(  4, 500, "Knight of Moonglow"),
                        new RankDefinition(  3, 400, "Apprentice of Moonglow"),
                        new RankDefinition(  2, 200, "Novice of Moonglow"),
                        new RankDefinition(  1,   0, "Novice of Moonglow")
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