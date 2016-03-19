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
    public class BoardingHooksUpgrade : BaseBoatActiveAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Boarding Hooks"; } }
        public override ActiveAbilityType ActiveAbility { get { return ActiveAbilityType.BoardingHooks; } } 

        [Constructable]
        public BoardingHooksUpgrade(): base()
        {
            Name = "a ship active ability upgrade: Boarding Hooks";
        }

        public BoardingHooksUpgrade(Serial serial): base(serial)
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