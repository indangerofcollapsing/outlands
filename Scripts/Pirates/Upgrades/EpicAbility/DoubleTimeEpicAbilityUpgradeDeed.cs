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
    public class DoubleTimeEpicAbilityUpgradeDeed : BaseBoatEpicAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Double Time"; } }

        public override EpicAbilityType EpicAbility { get { return EpicAbilityType.DoubleTime; } }

        [Constructable]
        public DoubleTimeEpicAbilityUpgradeDeed(): base()
        {
            Name = "a ship epic ability upgrade: Double Time";
        }

        public DoubleTimeEpicAbilityUpgradeDeed(Serial serial): base(serial)
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