using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{

    public class TreasuryWall : Item
    {
        //IRON: North: 2083
        //       East: 2081

        public Town m_Town;
        private TreasuryWallTypes m_Type;
        private Direction m_Direction;
        private int m_MaxHits = 1;
        private int m_Hits;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHits { get { return m_MaxHits; } set { m_MaxHits = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; } }

        public TreasuryWallTypes TreasuryWallType
        {
            get { return m_Type; }
            set 
            {
                if (value == TreasuryWallTypes.Iron)
                {
                    ItemID = 2083;
                    MaxHits = 1;
                    Hits = MaxHits;
                    Name = "a treasury wall";

                }
                else
                {
                    ItemID = 14662;
                    MaxHits = 3;
                    Hits = MaxHits;
                    Name = "a magical treasury wall";
                }
            }
        }

        public Direction Direction
        {
            get { return m_Direction; }
            set 
            {
                if (value == m_Direction)
                    return;

                if (value == Direction.North)
                {
                    ItemID = ItemID == 2081 ? 2083 : 14662;
                }
                else
                {
                    ItemID = ItemID == 2083 ? 2081 : 14678;
                }
            }
        }


        public TreasuryWall(Town town, TreasuryWallTypes type, Direction dir) : base(2083)
        {
            m_Town = town;
            TreasuryWallType = type;
            m_Direction = Direction.North;
            Direction = dir;
            Hits = MaxHits;
            Movable = false;
            Name = "a treasury wall";
        }

        public TreasuryWall(Serial serial) : base(serial)
        {
        }

        public void Damage(int value)
        {
            Hits -= value;
            if (Hits <= 0)
            {
                this.Delete();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, String.Format("[Durability: {0}/{1}]", m_Hits, m_MaxHits));
        }

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.TreasuryWalls != null && m_Town.TreasuryWalls.Contains(this))
                m_Town.TreasuryWalls.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)3);

            writer.Write((byte)m_Type);

            //version 2
            writer.Write(m_Hits);
            writer.Write(m_MaxHits);

            //version 1
            Town.WriteReference(writer, m_Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        m_Type = (TreasuryWallTypes)reader.ReadByte();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Hits = reader.ReadInt();
                        m_MaxHits = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Town = Town.ReadReference(reader);
                        break;
                    }

            }

            if (version < 2 && m_Town != null)
            {
                if (m_Town.TreasuryWallType == TreasuryWallTypes.Iron)
                {
                    m_Hits = m_MaxHits = 1;
                }
                else if (m_Town.TreasuryWallType == TreasuryWallTypes.Magical)
                {
                    m_Hits = m_MaxHits = 3;
                }
            }

            this.Delete();
            
        }
    }
}
