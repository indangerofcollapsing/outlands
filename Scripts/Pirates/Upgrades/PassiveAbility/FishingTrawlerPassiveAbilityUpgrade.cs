using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class FishingTrawlerUpgrade : BaseBoatPassiveAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Fishing Trawler"; } }
        public override PassiveAbilityType PassiveAbility { get { return PassiveAbilityType.FishingTrawler; } } 

        [Constructable]
        public FishingTrawlerUpgrade(): base()
        {
            Name = "a ship passive ability upgrade: Fishing Trawler";
        }

        public FishingTrawlerUpgrade(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
    }
}