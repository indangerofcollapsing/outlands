using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Accounting;

namespace Server
{
    public class UOACZSurvivalStone : Item
    {
        private PlayerMobile m_Player;
        [CommandProperty(AccessLevel.Administrator)]
        public PlayerMobile Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }

        [Constructable]
        public UOACZSurvivalStone(): base(571)
        {
            Name = "a survival stone";
            Hue = 2655;
        }

        [Constructable]
        public UOACZSurvivalStone(PlayerMobile player): base(571)
        {
            Name = "a survival stone";
            Hue = 2655;

            m_Player = player;           
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (m_Player == null)
                return true;

            Account ownerAccount = m_Player.Account as Account;
            Account playerAccount = player.Account as Account;

            if (ownerAccount != null && playerAccount != null && ownerAccount != playerAccount)
                return false;

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Player == null)
                return;

            else if (m_Player.Deleted)
                return;

            LabelTo(from, "(usable by " + m_Player.Name + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZPersistance.Enabled)
            {
                from.SendMessage("UOACZ system currently disabled.");
                return;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (m_Player != null)
            {
                Account ownerAccount = m_Player.Account as Account;
                Account playerAccount = player.Account as Account;

                if (ownerAccount != null && playerAccount != null && ownerAccount != playerAccount)
                {
                    from.SendMessage("That item may only be used by " + m_Player.Name + ".");
                    return;
                }
            }

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.SurvivalPoints, 1, true);
            player.SendSound(UOACZSystem.earnSurvivalSound);

            UOACZSystem.RefreshAllGumps(player);

            Delete();
        }

        public override bool CheckLift(Mobile from, Item item, ref Server.Network.LRReason reject)
        {
            bool baseCheck = base.CheckLift(from, item, ref reject);
            bool liftResult = true;

            PlayerMobile player = from as PlayerMobile;

            if (item.RootParentEntity is Corpse)
            {
                if (m_Player != null && player != null && from.AccessLevel == AccessLevel.Player)
                {
                    Account ownerAccount = m_Player.Account as Account;
                    Account playerAccount = player.Account as Account;

                    if (ownerAccount != null && playerAccount != null && ownerAccount != playerAccount)
                    {
                        liftResult = false;
                        from.SendMessage("That item is only accessable by " + m_Player.Name + ".");
                    }
                }
            }

            return baseCheck && liftResult;
        }

        public UOACZSurvivalStone(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_Player);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;
            }

            if (m_Player == null)
                Delete();

            else if (m_Player.Deleted)
                Delete();
        }
    }
}