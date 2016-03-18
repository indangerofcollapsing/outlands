using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;
namespace Server.Items
{
    public class HungerPotion : BasePotion
    {
        [Constructable]
        public HungerPotion()
            : base(0xF0B, PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 1642;
            Name = "a yummy smelling potion";
        }
        public HungerPotion(Serial serial)
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
            else if (FillHunger(from, 5))
            {
                BasePotion.PlayDrinkEffect(from);
                this.Consume();
                if (from.Body.IsHuman && !from.Mounted)
                {
                    from.Animate(34, 5, 1, true, false, 0);
                }         
            }
        }

        static public bool FillHunger(Mobile from, int fillFactor)
        {
            if (from.Hunger >= 20)
            {
                from.SendMessage("You are simply too full to drink any more of these potions!");
                return false;
            }

            int iHunger = from.Hunger + fillFactor;
            if (from.Stam < from.StamMax)
            {
                from.Stam += Utility.Random(6, 3) + fillFactor / 5;//restore some stamina
            }
            if (iHunger >= 20)
            {
                from.Hunger = 20;
                from.SendMessage("You manage to drink the potion, but you are stuffed!");
            }
            else
            {
                from.Hunger = iHunger;

                if (iHunger < 6)
                    from.SendMessage("You drink the potion, but are still extremely hungry");
                else if (iHunger < 10)
                    from.SendMessage("You drink the potion, and begin to feel more satiated.");
                else if (iHunger < 15)
                    from.SendMessage("After drinking the potion, you feel much less hungry.");
                else
                    from.SendMessage("You feel quite full after drinking the potion.");
            }
            return true;
        }
    }
}
