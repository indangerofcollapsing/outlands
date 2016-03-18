using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class UOACZUnlockableDeed : Item
    {
        private UOACZUnlockableType m_UnlockableType = UOACZUnlockableType.DeerMask;
        [CommandProperty(AccessLevel.Administrator)]
        public UOACZUnlockableType UnlockableType
        {
            get { return m_UnlockableType; }
            set { m_UnlockableType = value; }
        }

        [Constructable]
        public UOACZUnlockableDeed(): base(0x14F0)
        {
            Name = "UOACZ Unlockable deed";

            Hue = 2405;
            Weight = 0.1;

            m_UnlockableType = UOACZUnlockables.GetRandomUnlockableType();
        }

        [Constructable]
        public UOACZUnlockableDeed(UOACZUnlockableType unlockableType): this()
        {
            Name = "UOACZ Unlockable deed";

            Hue = 2405;
            Weight = 0.1;

            m_UnlockableType = unlockableType;
        }

        public UOACZUnlockableDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            UOACZUnlockableDetail details = UOACZUnlockables.GetUnlockableDetail(m_UnlockableType);

            LabelTo(from, "UOACZ Unlockable Deed: " + details.Name);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!player.Alive)
            {
                player.SendMessage("You must be alive to use that.");
                return;
            }

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("that must be in your backpack for you to use it.");
                return;
            }

            Activate(player);
        }

        public virtual void Activate(PlayerMobile player)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(m_UnlockableType);
            UOACZUnlockableDetailEntry unlockableDetailEntry = UOACZUnlockables.GetUnlockableDetailEntry(player, m_UnlockableType);

            if (unlockableDetailEntry != null)
            {
                player.SendMessage("You have already unlocked the UOACZ Unlockable: " + unlockableDetail.Name + ".");
                return;
            }

            else
            {
                player.m_UOACZAccountEntry.m_Unlockables.Add(new UOACZUnlockableDetailEntry(m_UnlockableType, true, false));
                player.SendMessage("You have unlocked the UOACZ Unlockable: " + unlockableDetail.Name + ".");

                player.PlaySound(0x0F7);
                player.FixedParticles(0x373A, 10, 15, 5012, 2587, 0, EffectLayer.Waist);                   
                
                player.CloseGump(typeof(UOACZScoreGump));

                Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_UnlockableType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_UnlockableType = (UOACZUnlockableType)reader.ReadInt();
            }
        }
    }
}