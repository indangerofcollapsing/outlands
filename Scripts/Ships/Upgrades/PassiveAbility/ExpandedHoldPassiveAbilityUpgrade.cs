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
    public class ExpandedHoldUpgrade : BaseBoatPassiveAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Expanded Hold"; } }
        public override PassiveAbilityType PassiveAbility { get { return PassiveAbilityType.ExpandedHold; } } 

        [Constructable]
        public ExpandedHoldUpgrade(): base()
        {
            Name = "a ship passive ability upgrade: Expanded Hold";
        }

        public ExpandedHoldUpgrade(Serial serial): base(serial)
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