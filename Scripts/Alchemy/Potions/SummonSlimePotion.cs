using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;
namespace Server.Items
{
    public class SummonSlimePotion : BasePotion
    {
        public override bool RequireFreeHand { get { return true; } }

        [Constructable]
        public SummonSlimePotion()
            : base(0xF0E, PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = Utility.RandomSlimeHue();
            Name = "a murky potion";
        }
        public SummonSlimePotion(Serial serial)
            : base(serial)
        {
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
                    BaseCreature slime = new PetSlime();
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
