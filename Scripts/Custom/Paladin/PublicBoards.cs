using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x1E5E, 0x1E5F )]
	public class VengeanceBoard : Item
	{
		[Constructable]
		public VengeanceBoard() : base( 0x1E5E )
		{
            Name = "public vengeance listings board";
            Hue = 754;
            Movable = false;
		}

        public VengeanceBoard(Serial serial)
            : base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            pm_From.CloseGump(typeof(VengeanceEntry.VengeanceGump));
            pm_From.SendGump(new VengeanceEntry.VengeanceGump(VengeanceGumpMode.PublicSelected, pm_From, new VengeanceFilter(), 0));
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            pm_From.CloseGump(typeof(VengeanceEntry.VengeanceGump));
            pm_From.SendGump(new VengeanceEntry.VengeanceGump(VengeanceGumpMode.PublicSelected, pm_From, new VengeanceFilter(), 0));
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

    [Flipable(0x1E5E, 0x1E5F)]
    public class RestitutionBoard : Item
    {
        [Constructable]
        public RestitutionBoard()
            : base(0x1E5E)
        {
            Name = "public restitution fees listing board";
            Hue = 916;
            Movable = false;
        }

        public RestitutionBoard(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            pm_From.CloseGump(typeof(RestitutionEntry.RestitutionGump));
            pm_From.SendGump(new RestitutionEntry.RestitutionGump(pm_From, new RestitutionFilter(), 0));
        }     

        public override void OnDoubleClickDead(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            pm_From.CloseGump(typeof(RestitutionEntry.RestitutionGump));
            pm_From.SendGump(new RestitutionEntry.RestitutionGump(pm_From, new RestitutionFilter(), 0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}