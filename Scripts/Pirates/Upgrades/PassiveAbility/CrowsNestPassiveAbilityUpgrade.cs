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
    public class CrowsNestUpgrade : BaseBoatPassiveAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Crow's Nest"; } }
        public override PassiveAbilityType PassiveAbility { get { return PassiveAbilityType.CrowsNest; } } 

        [Constructable]
        public CrowsNestUpgrade(): base()
        {
            Name = "a ship passive ability upgrade: Crow's Nest";
        }

        public CrowsNestUpgrade(Serial serial): base(serial)
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