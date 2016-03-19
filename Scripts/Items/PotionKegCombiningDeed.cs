﻿using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class PotionKegCombiningDeed : Item
    {
        [Constructable]
        public PotionKegCombiningDeed()
            : base(0x14F0)
        {
            Name = "a potion keg combining deed";

            Hue = 2500;
            Weight = .1;
        }

        public PotionKegCombiningDeed(Serial serial)
            : base(serial)
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

            from.SendMessage("Target an empty potion keg you wish to combine with another.");
            from.Target = new FirstPotionKegTarget(this);
        }

        public class FirstPotionKegTarget : Target
        {
            private PotionKegCombiningDeed m_PotionKegCombineDeed;

            public FirstPotionKegTarget(PotionKegCombiningDeed PotionKegCombineDeed)
                : base(2, false, TargetFlags.None)
            {
                m_PotionKegCombineDeed = PotionKegCombineDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_PotionKegCombineDeed.Deleted || m_PotionKegCombineDeed.RootParent != from) return;
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target an empty potion keg in your backpack.");
                        return;
                    }

                    if (!(item is PotionKeg))
                    {
                        from.SendMessage("That item is not an empty potion keg.");
                        return;
                    }

                    PotionKeg firstPotionKeg = item as PotionKeg;

                    if (firstPotionKeg.Held > 0)
                    {
                        from.SendMessage("You may only combine empty potion kegs.");
                        return;
                    }

                    from.SendMessage("Target an empty potion keg you wish to combine the previous with. The newly combined potion keg will maintain the hue of the second targeted potion keg.");
                    from.Target = new SecondPotionKegTarget(m_PotionKegCombineDeed, firstPotionKeg);
                }

                else
                {
                    from.SendMessage("That is not a valid item to combine.");
                    return;
                }
            }
        }

        public class SecondPotionKegTarget : Target
        {
            private PotionKegCombiningDeed m_PotionKegCombineDeed;
            private PotionKeg m_FirstPotionKeg;

            public SecondPotionKegTarget(PotionKegCombiningDeed PotionKegCombineDeed, PotionKeg firstPotionKeg)
                : base(2, false, TargetFlags.None)
            {
                m_PotionKegCombineDeed = PotionKegCombineDeed;
                m_FirstPotionKeg = firstPotionKeg;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_PotionKegCombineDeed.Deleted || m_PotionKegCombineDeed.RootParent != from) return;
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target an empty potion keg in your backpack.");
                        return;
                    }

                    if (!(item is PotionKeg))
                    {
                        from.SendMessage("That item is not an empty potion keg.");
                        return;
                    }

                    PotionKeg secondPotionKeg = item as PotionKeg;

                    if (secondPotionKeg == m_FirstPotionKeg)
                    {
                        from.SendMessage("You can cannot combine the same keg into itself.");
                        return;
                    }

                    if (secondPotionKeg.Held > 0)
                    {
                        from.SendMessage("You may only combine empty potion kegs.");
                        return;
                    }

                    m_PotionKegCombineDeed.CombinePotionKegs(from, m_FirstPotionKeg, secondPotionKeg);
                }

                else
                {
                    from.SendMessage("That is not a valid item to combine.");
                    return;
                }
            }
        }

        public void CombinePotionKegs(Mobile from, PotionKeg firstPotionKeg, PotionKeg secondPotionKeg)
        {
            bool combineFail = false;

            if (from == null) combineFail = true;
            if (from.Deleted || !from.Alive) combineFail = true;

            if (Deleted) combineFail = true;
            if (!IsChildOf(from.Backpack)) combineFail = true;

            if (firstPotionKeg.Deleted) combineFail = true;
            if (!firstPotionKeg.IsChildOf(from.Backpack)) combineFail = true;
            if (firstPotionKeg.Held > 0) combineFail = true;

            if (secondPotionKeg.Deleted) combineFail = true;
            if (!secondPotionKeg.IsChildOf(from.Backpack)) combineFail = true;
            if (secondPotionKeg.Held > 0) combineFail = true;

            if (combineFail)
            {
                from.SendMessage("At least one of the potion kegs intended to be combined is no longer accessable or is no longer empty.");
                return;
            }
            
            if (firstPotionKeg.IsPotionBarrel)
                secondPotionKeg.IsPotionBarrel = true;

            secondPotionKeg.MaxHeld += firstPotionKeg.MaxHeld;
            firstPotionKeg.Delete();

            from.PlaySound(0x23D);
            from.SendMessage("You combine the capacity of the two kegs.");

            Delete();
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