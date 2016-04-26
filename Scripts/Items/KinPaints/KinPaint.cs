using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class KinPaint : Item
	{
        public int PaintHue;

		[Constructable]
		public KinPaint() : base( 0x9EC )
		{
            Name = "kin paint";
			Weight = 1.0;

            PaintHue = 0;
		}

        public KinPaint(Serial serial): base(serial)
		{           
		}

        public static bool IsWearingKinPaint(Mobile mobile)
        {
            PlayerMobile player = mobile as PlayerMobile;

            if (player != null)
            {
                if (player.KinPaintHue != -1)
                    return true;
            }

            return false;
        }        

		public override void OnDoubleClick( Mobile from )
		{
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

			if ( IsChildOf( from.Backpack ) )
			{
                if (!from.CanBeginAction(typeof(Spells.Fifth.IncognitoSpell)))
                    from.SendLocalizedMessage(501698); // You cannot disguise yourself while incognitoed.				

                else if (!from.CanBeginAction(typeof(Spells.Seventh.PolymorphSpell)))
                    from.SendLocalizedMessage(501699); // You cannot disguise yourself while polymorphed.

                else if (TransformationSpellHelper.UnderTransformation(from))
                    from.SendLocalizedMessage(501699); // You cannot disguise yourself while polymorphed.

                else if (KinPaint.IsWearingKinPaint(from))
                    from.SendMessage("You already have a kin paint applied.");

                else
                {
                    player.KinPaintHue = PaintHue;
                    player.KinPaintExpiration = DateTime.UtcNow + TimeSpan.FromDays(14);

                    player.HueMod = PaintHue;
                    
                    from.PlaySound(0x5AC);
                    from.SendMessage("You apply the kin paint to yourself. The paint will last for two weeks or may be removed at any time with an oil cloth.");
                    
                    Consume();
                }
			}

			else			
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.			
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

            writer.Write(PaintHue);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            PaintHue = reader.ReadInt();
		}
	}
}