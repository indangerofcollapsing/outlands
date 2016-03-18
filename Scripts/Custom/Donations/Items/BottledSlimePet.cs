using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class BottledSlimePet : SummonSlimePotion
	{

		[Constructable]
        public BottledSlimePet()
		{
            ItemID = 0xF0E;
            Name = "bottled pet slime";
			Hue = 0;
            LootType = Server.LootType.Blessed;
            DonationItem = true;
		}

		public BottledSlimePet( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

        public override void Drink(Mobile from)
        {
            if (this != null && this.ParentEntity != from.Backpack)
            {
                from.SendMessage("The potion must be in your pack to drink it.");
            }
            else
            {
                if (!RequireFreeHand || HasFreeHand(from))
                {
                    BaseCreature slime = new DonationSlime();
                    if (from.Followers + (slime.ControlSlots) <= from.FollowersMax)
                    {
                        from.Animate(34, 5, 1, true, false, 0);
                        BasePotion.PlayDrinkEffect(from);
                        Point3D loc = from.Location;
                        slime.Hue = this.Hue;
                        slime.MoveToWorld(loc, this.Map);
                        slime.Controlled = true;
                        slime.ControlMaster = from;
                        slime.ControlOrder = OrderType.Come;
                        this.Consume();
                    }
                    else
                    {
                        from.SendMessage("You have too many followers right now.");
                        slime.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
                }
            }
        }
	}
}
