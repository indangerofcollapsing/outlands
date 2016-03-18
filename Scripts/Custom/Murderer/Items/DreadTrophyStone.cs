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
    [FlipableAttribute(0x0ED8, 0x0ED7)]
    public class DreadTrophyStone : Item
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(60);
        
        public override bool Decays { get { return false; } }

        public string m_MurdererName = "";
        public string m_PaladinName = "";

        [Constructable]
        public DreadTrophyStone(): base(0x0ED8)
        {
            Name = "a dread trophy stone";
            Weight = 10.0;            

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
        }

        public DreadTrophyStone(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            if (m_MurdererName == "" || m_PaladinName == "")
            {
                LabelTo(from, "a dread trophy stone");

                PlayerClassPersistance.PlayerClassSingleClick(this, from);
            }

            else            
                LabelTo(from, "Here lies the foolish paladin " + m_PaladinName + ". Slain by the villanous " + m_MurdererName + ".");            
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

                if (!pm_From.Murderer || this.PlayerClassOwner != pm_From)
                {
                    from.SendMessage("Only the Murderous owner of this item may use it.");
                    return;
                }

                else if (from.InRange(this.GetWorldLocation(), 1))
                {
                    from.SendMessage("Target the head of the Paladin you wish to engrave onto this stone.");
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
            private DreadTrophyStone m_DreadTrophyStone;

            public InternalTarget(DreadTrophyStone dreadTrophyStone): base(1, false, TargetFlags.None)
            {
                m_DreadTrophyStone = dreadTrophyStone;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_DreadTrophyStone == null) return;
                if (m_DreadTrophyStone.Deleted) return;

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

                if (head.PlayerType != PlayerType.Paladin)
                {
                    from.SendMessage("That individual was not a Paladin.");
                    return;
                }

                if (head.Killer != from)
                {
                    from.SendMessage("You were not responsible for that Paladin's demise.");
                    return;
                }

                from.PlaySound(0x17F);
                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2199, 0, 5029, 0);

                from.SendMessage("You engrave the details into the stone.");

                m_DreadTrophyStone.m_MurdererName = head.KillerName;
                m_DreadTrophyStone.m_PaladinName = head.PlayerName;

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