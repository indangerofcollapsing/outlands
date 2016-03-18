using System;
using Server;

namespace Server.Custom.Townsystem
{
	public class YewFac : Faction
	{
		private static Faction m_Instance;

		public static Faction Instance{ get{ return m_Instance; } }

        public YewFac()
		{
			m_Instance = this;

			Definition =
				new FactionDefinition(
					13,
                    false,
					2212, // blue
					2212, // FLAG HUE: Blue
					2212, // join stone : blue
					2212, // broadcast : blue
                    5664, 5666,
					"Yew", "Yew", "Yew",
					"Yew",
					"Yew faction",
					"<center>Yew</center>",
					    "The council of Mages have their roots in the city of Moonglow, where " +
						"they once convened. They began as a small movement, dedicated to " +
						"calling forth the Stranger, who saved the lands once before.  A " +
						"series of war and murders and misbegotten trials by those loyal to " +
						"Lord British has caused the group to take up the banner of war.",
                    8741, //CRYSTAL ID
					"This city is controlled by Yew.",
					"This sigil has been corrupted by Yew",
					"The faction signup stone for Yew",
					"The Faction Stone of Yew",
					": Yew",
					"Members of Yew will now be ignored.",
					"Members of Yew will now be warned to leave.",
					"Members of Yew will now be beaten with a stick.",
					
					new RankDefinition[]
					{
						new RankDefinition( 10, 991, "Hero of Yew"),
                        new RankDefinition( 9, 950, "Ranger-Lord of Yew"),
                        new RankDefinition( 8, 900, "Champion of the Forest"),
                        new RankDefinition( 7, 800, "Loremaster of Yew"),
                        new RankDefinition( 6, 700, "Knight-Captain of Yew"),
                        new RankDefinition( 5, 600, "Warden of Yew"),
                        new RankDefinition( 4, 500, "Knight of Yew"),
                        new RankDefinition( 3, 400, "Defender of Yew"),
                        new RankDefinition( 2, 200, "Squire of Yew"),
                        new RankDefinition( 1, 0, "Militiaman of Yew")
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