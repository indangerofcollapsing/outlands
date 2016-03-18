using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class PotionBarrelConversionDeed : Item
    {
        [Constructable]
        public PotionBarrelConversionDeed(): base(0x14F0)
        {
            Name = "a potion barrel conversion deed";

            Hue = 2515;
            Weight = .1;
        }

        public PotionBarrelConversionDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);           
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target a potion keg with a capacity of 500 or more that you wish to convert into a potion barrel.");
            from.Target = new PotionBarrelTarget(this);
        }

        public class PotionBarrelTarget : Target
        {
            private PotionBarrelConversionDeed m_PotionBarrelCreationDeed;

            public PotionBarrelTarget(PotionBarrelConversionDeed potionBarrelCreationDeed): base(2, false, TargetFlags.None)
            {
                m_PotionBarrelCreationDeed = potionBarrelCreationDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_PotionBarrelCreationDeed.Deleted || m_PotionBarrelCreationDeed.RootParent != from) return;
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target a potion keg in your backpack.");
                        return;
                    }

                    if (!(item is PotionKeg))
                    {
                        from.SendMessage("That item is not a potion keg.");
                        return;
                    }

                    PotionKeg potionKeg = item as PotionKeg;

                    if (potionKeg.MaxHeld < 500)
                    {
                        from.SendMessage("You must target a potion keg with a capacity of 500 or more.");
                        return;
                    }

                    if (potionKeg.IsPotionBarrel)
                    {
                        from.SendMessage("That keg has already been converted into a potion barrel.");
                        return;
                    }

                    potionKeg.IsPotionBarrel = true;

                    from.PlaySound(0x23D);
                    from.SendMessage("You convert the potion keg into a potion barrel.");

                    m_PotionBarrelCreationDeed.Delete();                   
                }                

                else
                {
                    from.SendMessage("That is not a valid item to combine.");
                    return;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {               
            }
        }
    }
}