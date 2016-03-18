/***************************************************************************
 *                              YewJailDoor.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server.YewJail;

namespace Server.Items
{
    public class YewJailDoor : BaseDoor, ILockpickable
    {
        private int m_LockLevel = 94;
        private int m_RequiredSkill = 0;
        private int m_MaxLockLevel;
        private int m_JailNo;
        private Mobile m_Picker;
        private YewJailItem m_YewJailItem;
        private bool spoken = false;

        [CommandProperty(AccessLevel.GameMaster)]
        public int LockLevel
        {
            set { m_LockLevel = value; }
            get { return m_LockLevel; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredSkill
        {
            set { m_RequiredSkill = value; }
            get { return m_RequiredSkill; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLockLevel
        {
            set { m_MaxLockLevel = value; }
            get { return m_MaxLockLevel; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Picker
        {
            set { m_Picker = value; }
            get { return m_Picker; }
        }

        public YewJailItem YewJailItem
        {
            set { m_YewJailItem = value; }
            get { return m_YewJailItem; }
        }
        
        public YewJailDoor(DoorFacing facing, int jailNo)
            : base(0x685 + (2 * (int)facing), 0x686 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing))
        {
            KeyValue = 123456789;
            m_JailNo = jailNo;
            Name = String.Format("Jail Cell {0} Door", jailNo);
            TileData.ItemTable[ItemID].Flags = TileFlag.Window & TileFlag.NoShoot;
        }

        public YewJailDoor(Serial serial)
            : base(serial)
        {
        }

        public void LockPick(Mobile from)
        {
            Locked = false;
            if (m_YewJailItem != null)
                m_YewJailItem.DoorPicked();
        }

        public void RegisterJailDoor()
        {
            YewJailControl.RegisterJailDoor(this, m_JailNo);
            YewJailControl.m_Doors = true;
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.WriteItem(m_YewJailItem);
            writer.Write((int)m_JailNo);
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_YewJailItem = reader.ReadItem() as YewJailItem;
            m_JailNo = reader.ReadInt();

            RegisterJailDoor();
        }
    }
}