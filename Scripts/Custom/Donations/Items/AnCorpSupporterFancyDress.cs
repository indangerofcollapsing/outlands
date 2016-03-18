using System;

namespace Server.Items
{
    [FlipableAttribute( 0x1eff, 0x1f00 )]
    public class AnCorpSupporterFancyDress : BaseOuterTorso
    {
        [Constructable]
		public AnCorpSupporterFancyDress() : this( 0 )
		{
		}

		[Constructable]
		public AnCorpSupporterFancyDress( int hue ) : base( 0x1EFF, hue )
		{
            Name = "an An Corp Supporter fancy dress";

            Hue = 1159;
            Weight = 3.0;

            LootType = Server.LootType.Blessed;            
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "an An Corp Supporter fancy shirt");
            LabelTo(from, "(blessed)"); 
        }

        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            Cloth cloth = new Cloth();
            cloth.Amount = 1;
            cloth.Hue = Hue;

            Delete();

            from.AddToBackpack(cloth);
            from.SendMessage("You cut the item into cloth.");

            return false;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public AnCorpSupporterFancyDress(Serial serial): base(serial)
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

            //Override
            Hue = 1159;
		}
    }
}