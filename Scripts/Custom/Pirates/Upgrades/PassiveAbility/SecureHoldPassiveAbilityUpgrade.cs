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
    public class SecureHoldUpgrade : BaseBoatPassiveAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Secure Hold"; } }
        public override PassiveAbilityType PassiveAbility { get { return PassiveAbilityType.SecureHold; } } 

        [Constructable]
        public SecureHoldUpgrade(): base()
        {
            Name = "a ship passive ability upgrade: Secure Hold";
        }

        public SecureHoldUpgrade(Serial serial): base(serial)
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