using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class UOACZUndeadDye : Item
	{
        private int m_DyedHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DyedHue
        {
            get { return m_DyedHue; }
            set
            { 
                m_DyedHue = value;
                Hue = m_DyedHue;
            }
        }

        public static int StartingCharges = 10;

        private int m_UsesRemaining;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; }
        }

        private PlayerMobile m_Owner = null;
        [CommandProperty(AccessLevel.Administrator)]
        public PlayerMobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        } 

        [Constructable]
        public UOACZUndeadDye(): base(3622)
        {
            Name = "undead dye";
            Weight = 1;

            m_UsesRemaining = StartingCharges;           
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(Hue: " + DyedHue.ToString() + ")");
            LabelTo(from, "[" + UsesRemaining.ToString() + " Uses Remaining]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("That dye must be in your pack in order to use it.");
                return;
            }

            if (!player.IsUOACZUndead)
            {
                player.SendMessage("Only Undead players may use this.");
                return;
            }

            if (m_Owner != null)
            {
                if (m_Owner.Account != null)
                {                    
                    string ownerAccountName = m_Owner.Account.Username;
                    string playerAccountName = player.Account.Username;

                    if (ownerAccountName != playerAccountName)
                    {
                        player.SendMessage("Only the owner of this item may use this.");
                        return;
                    }
                }                
            }

            if (player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod == DyedHue)
            {
                player.SendMessage("You are already that hue.");
                return;
            }

            player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod = DyedHue;
            player.m_UOACZAccountEntry.UndeadProfile.HueMod = DyedHue;
            player.HueMod = DyedHue;

            from.PlaySound(0x23E);

            UsesRemaining--;           

            if (UsesRemaining <= 0)
            {
                from.SendMessage("You use the last dye charge.");
                Delete();
            }

            else
            {
                from.SendMessage("You change your hue.");
            }           
        }

        public UOACZUndeadDye(Serial serial): base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write((int)0); // version

            writer.Write(m_DyedHue);            
            writer.Write(m_UsesRemaining);
            writer.Write(m_Owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_DyedHue = reader.ReadInt();                
                m_UsesRemaining = reader.ReadInt();
                m_Owner = (PlayerMobile)reader.ReadMobile() as PlayerMobile;
            }
		}	
	}
}