using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class MaginciaFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public MaginciaFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					4,
                    false,
					1158, // blue
                    1158, // FLAG HUE: Blue
                    1158, // join stone : blue
                    1158, // broadcast : blue
                    5649, 5650,
					"Magincia", "Magincia", "Magincia",
					"Magincia",
					"Magincia faction",
					"<center>Magincia</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Magincia.",
					"This sigil has been corrupted by Magincia",
					"The faction signup stone for Magincia",
					"The Faction Stone of Magincia",
					": Magincia",
					"Members of Magincia will now be ignored.",
					"Members of Magincia will now be warned to leave.",
					"Members of Magincia will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Magincia"),
                        new RankDefinition(  9, 950, "Magistrate of Magincia"),
                        new RankDefinition(  8, 900, "Magincian Knight-Lord"),
                        new RankDefinition(  7, 800, "Justicar of Magincia"),
                        new RankDefinition(  6, 700, "Magincian Dragoon"),
                        new RankDefinition(  5, 600, "Ambassador of Magincia"),
                        new RankDefinition(  4, 500, "Arcane Sword of Magincia"),
                        new RankDefinition(  3, 400, "Defender of Magincia"),
                        new RankDefinition(  2, 200, "Squire of Magincia"),
                        new RankDefinition(  1,   0, "Squire of Magincia")
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