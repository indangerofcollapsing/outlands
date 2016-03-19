using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
    public class UOACZStockpileContainer : WoodenChest
    {
        public override int DefaultDropSound { get { return 0x42; } }

        private string m_AccountName;
        [CommandProperty(AccessLevel.GameMaster)]
        public string AccountName
        {
            get { return m_AccountName; }
            set { m_AccountName = value; }
        }

        private UOACZAccountEntry m_AccountEntry;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZAccountEntry AccountEntry
        {
            get { return m_AccountEntry; }
            set { m_AccountEntry = value; }
        }

        [Constructable]
        public UOACZStockpileContainer(string accountName): base()
        {           
            Name = "";           

            m_AccountName = accountName;
            m_AccountEntry = UOACZPersistance.FindUOACZAccountEntryByAccountName(m_AccountName);

            MaxItems = 15;
           
            Movable = false;
        }
        
        public override bool IsDecoContainer
        {
            get { return false; }
        }        

        public override bool IsAccessibleTo(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            if (m_AccountEntry == null)
                return false;

            if (!player.IsUOACZHuman)
                return false;

            if (player.m_UOACZAccountEntry != m_AccountEntry)    
                return false;

            if (DateTime.UtcNow < player.LastCombatTime + UOACZSystem.CombatDelayBeforeStockpileAccess)
            {
                DateTime cooldown = player.LastCombatTime + UOACZSystem.CombatDelayBeforeStockpileAccess;

                string nextActivationAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, cooldown, false, false, false, true, true);

                player.SendMessage("You have been in combat recently and must wait another " + nextActivationAllowed + " before accessing this.");
                return false;
            }

            return base.IsAccessibleTo(from);
        }
        
        public UOACZStockpileContainer(Serial serial): base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_AccountName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0 
            if (version >= 0)
            {
                m_AccountName = reader.ReadString();
            }

            //--------

            m_AccountEntry = UOACZPersistance.FindUOACZAccountEntryByAccountName(m_AccountName);
        }
    }
}