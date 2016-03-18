using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    [FlipableAttribute(0x0EDC, 0x0EDB)]
    public class PaladinTrophyStone : Item
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }
                
        public override bool Decays { get { return false; } }

        public string m_MurdererName = "";
        public string m_PaladinName = "";

        [Constructable]
        public PaladinTrophyStone(): base(0x0EDC)
        {
            Name = "a paladin trophy stone";
            Weight = 10.0;

            Hue = 2500;

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
        }

        public PaladinTrophyStone(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            if (m_MurdererName == "" || m_PaladinName == "")
            {
                LabelTo(from, "a paladin trophy stone");

                PlayerClassPersistance.PlayerClassSingleClick(this, from);
            }

            else
                LabelTo(from, "Here lies the murderous " + m_MurdererName + ". Slain by the honorable " + m_PaladinName + ".");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (m_MurdererName == "" || m_PaladinName == "")
            {
                if (!from.Alive)
                {
                    from.SendMessage("You must be alive to use that.");
                    return;
                }

                PlayerMobile pm_From = from as PlayerMobile;

                if (!pm_From.Paladin || this.PlayerClassOwner != pm_From)
                {
                    from.SendMessage("Only the Paladin owner of this item may use it.");
                    return;
                }

                else if (from.InRange(this.GetWorldLocation(), 1))
                {
                    from.SendMessage("Target the head of the Murderer you wish to engrave onto this stone.");
                    from.Target = new InternalTarget(this);
                }

                else
                {
                    from.SendMessage("That is too far away to use.");
                    return;
                }
            }
        }

        private class InternalTarget : Target
        {
            private PaladinTrophyStone m_PaladinTrophyStone;

            public InternalTarget(PaladinTrophyStone paladinTrophyStone): base(1, false, TargetFlags.None)
            {
                m_PaladinTrophyStone = paladinTrophyStone;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_PaladinTrophyStone == null) return;
                if (m_PaladinTrophyStone.Deleted) return;

                Head head = targeted as Head;

                if (head == null)
                {
                    from.SendMessage("That is not the head of a person.");
                    return;
                }

                if (!head.Movable || head.IsLockedDown)
                {
                    from.SendMessage("You do not have access to that.");
                    return;
                }

                if (head.PlayerType != PlayerType.Murderer)
                {
                    from.SendMessage("That individual was not a Murderer.");
                    return;
                }

                if (head.Killer != from)
                {
                    from.SendMessage("You were not responsible for that Murderer's demise.");
                    return;
                }

                from.PlaySound(0x5CA);
                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 1150, 0, 5029, 0);

                from.SendMessage("You engrave the details into the stone.");

                m_PaladinTrophyStone.m_PaladinName = head.KillerName;
                m_PaladinTrophyStone.m_MurdererName = head.PlayerName;        

                head.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            writer.Write(m_MurdererName);
            writer.Write(m_PaladinName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            { 
                m_MurdererName = reader.ReadString();
                m_PaladinName = reader.ReadString();
            }
        }
    }
}