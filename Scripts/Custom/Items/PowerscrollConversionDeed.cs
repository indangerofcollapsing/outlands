using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class PowerscrollConversionDeed : Item
    {
        public static int InfluenceTicketsGiven = 3;

        [Constructable]
        public PowerscrollConversionDeed(): base(0x14F0)
        {
            Name = "powerscroll to influence lottery tickets conversion deed";
            Hue = 2515;
        }

        public PowerscrollConversionDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target a powerscroll to convert into " + InfluenceTicketsGiven.ToString() + " influence lottery tickets.");
            from.Target = new ContainerTarget(this);
        }

        public class ContainerTarget : Target
        {
            private IEntity targetLocation;
            private PowerscrollConversionDeed m_PowerscrollConversionDeed;

            public ContainerTarget(PowerscrollConversionDeed PowerscrollConversionDeed): base(25, false, TargetFlags.None)
            {
                m_PowerscrollConversionDeed = PowerscrollConversionDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Deleted || !player.Alive) return;

                if (player.Backpack == null)
                    return;

                if (m_PowerscrollConversionDeed == null) return;
                if (m_PowerscrollConversionDeed.Deleted) return;

                if (!m_PowerscrollConversionDeed.IsChildOf(from.Backpack))
                {
                    from.SendMessage("That deed must be in your pack in order to use it.");
                    return;
                }

                PowerScroll powerScroll = target as PowerScroll;

                if (powerScroll == null)
                {
                    player.SendMessage("That is not a powerscroll.");
                    return;
                }

                if (!powerScroll.IsChildOf(from.Backpack))
                {
                    from.SendMessage("You must target a powerscroll in your backpack.");
                    return;
                }

                if (player.Backpack.Items.Count + InfluenceTicketsGiven - 1 > player.Backpack.MaxItems)
                {
                    from.SendMessage("Your backpack currently has too many items to receive all items that would be created. Please make some space and try again.");
                    return;
                }

                from.SendSound(0x5B5);
                from.SendMessage("You convert the powerscroll into " + InfluenceTicketsGiven + " influence lottery tickets.");

                powerScroll.Delete();
                m_PowerscrollConversionDeed.Delete();

                for (int a = 0; a < InfluenceTicketsGiven; a++)
                {
                    player.Backpack.DropItem(new InfluenceLotteryTicket());
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
        }
    }
}