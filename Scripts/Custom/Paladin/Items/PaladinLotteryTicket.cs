using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class PaladinLotteryTicket : Item
	{   
        [Constructable]
        public PaladinLotteryTicket(): base(0x0FBD)
        {
            Name = "Paladin Lottery Ticket";
            
            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;            

            LootType = Server.LootType.Blessed;
        }

        public PaladinLotteryTicket(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();            
        }
    }
}