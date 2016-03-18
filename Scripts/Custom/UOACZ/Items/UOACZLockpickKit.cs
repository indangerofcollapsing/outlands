using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Custom;

namespace Server.Items
{
    public class UOACZLockpickKit : Item
    {
        private int m_Charges = 50;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public static int MaxCharges = 50;

        [Constructable]
        public UOACZLockpickKit() : base(5373)
        {
            Name = "a lockpick kit";
            Hue = 0;

            Weight = 1;
        }

        public UOACZLockpickKit(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "uses remaining: " + m_Charges.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the item to lockpick.");
            from.Target = new UOACZLockpickTarget(this);
        }

        public class UOACZLockpickTarget : Target
        {
            private UOACZLockpickKit m_UOACZLockpickKit;

            public UOACZLockpickTarget(UOACZLockpickKit lockpickKit): base(1, false, TargetFlags.None)
            {
                m_UOACZLockpickKit = lockpickKit;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_UOACZLockpickKit.Deleted || m_UOACZLockpickKit.RootParent != from) return;     
                if (from == null) return;
                if (from.Deleted || !from.Alive) return;

                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (target is UOACZBaseScavengeObject)
                {
                    UOACZBaseScavengeObject scavengeObject = target as UOACZBaseScavengeObject;

                    if (Utility.GetDistance(player.Location, scavengeObject.Location) > scavengeObject.InteractionRange)
                    {
                        from.SendMessage("You are too far away to use that.");
                        return;
                    }

                    int interactionCount = scavengeObject.GetInteractions(player);

                    if (scavengeObject.YieldsRemaining == 0 || interactionCount >= scavengeObject.MaxPlayerInteractions)
                    {
                        player.SendMessage(scavengeObject.NoYieldRemainingText);
                        return;     
                    }

                    if (!scavengeObject.Locked)
                    {
                        from.SendMessage("That is not locked.");
                        return;
                    }

                    scavengeObject.LockpickInteract(player, m_UOACZLockpickKit);
                }

                else
                {
                    from.SendMessage("That cannot be lockpicked.");
                    return;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version    
      
            //Version 0
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
    }
}