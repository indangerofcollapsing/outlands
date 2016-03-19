using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Custom.Donations;

namespace Server.Custom.Mobiles
{
    class CustomizableDonationNPC : CustomizableNPC
    {
        [Constructable]
        public CustomizableDonationNPC(): base()
        {
        }

        public override bool DropsGold { get { return false; } }
        
        public override bool HandlesOnSpeech(Mobile from) { return true; }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Handled || !from.Alive || from.GetDistanceToSqrt(this) > 3)
                return;

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171))) // vendor buy, *buy*
            {
                if (!from.HasGump(typeof(DonationShop)))
                    from.SendGump(new DonationShop());
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(DonationShop)))
                from.SendGump(new DonationShop());
        }

        public CustomizableDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    class CustomizableNPC : BaseCreature
    {
        [Constructable]
        public CustomizableNPC(): base( AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2 )
        {
        }

        public CustomizableNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
