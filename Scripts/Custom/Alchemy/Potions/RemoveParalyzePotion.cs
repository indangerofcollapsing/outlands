using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class RemoveParalyzePotion : BasePotion
	{
		public override bool RequireFreeHand{ get{ return true; } }

        [Constructable]
		public RemoveParalyzePotion() 
            : base( 0xF0D, PotionEffect.Custom )
		{
            Weight = 1.0;
            Movable = true;
            Hue = 1642;
            Name = "a bottle of red powder";
		}

		public RemoveParalyzePotion( Serial serial ) : base( serial )
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

		public override void Drink( Mobile from )
		{
            if (this != null && this.ParentEntity != from.Backpack)
            {
                from.SendMessage("The potion must be in your pack to drink it.");
            }
            else
            {
                if (!RequireFreeHand || HasFreeHand(from))
                {
                    from.RevealingAction();
                    if (from.Paralyzed)
                    {
                        Effects.PlaySound(from.Location, from.Map, 0x1EF);
                        from.Animate(34, 5, 1, true, false, 0);
                        BasePotion.PlayDrinkEffect(from);
                        from.Paralyzed = false;
                        this.Consume();
                    }
                    else
                    {
                        from.SendMessage("You are not paralyzed.");
                    } 
                }
                else
                {
                    from.SendLocalizedMessage(502172);
                }
            }
		}
	}
}