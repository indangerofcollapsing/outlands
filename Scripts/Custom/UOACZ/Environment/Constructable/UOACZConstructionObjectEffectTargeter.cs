using System;
using Server;
using Server.Mobiles;
using System.Collections;
using Server.Custom;

namespace Server.Items
{
    public class UOACZConstructionObjectEffectTargeter : Item
    {
        private UOACZConstructionTile m_ConstructionTile;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZConstructionTile ConstructionTile
        {
            get { return m_ConstructionTile; }
            set { m_ConstructionTile = value; }
        }

        private int m_Radius = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }        

        [Constructable]
        public UOACZConstructionObjectEffectTargeter(): base(6126)		
		{
            Name = "construction object effect targeter";

            Hue = 2500;

            Movable = false;           
		}

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public UOACZConstructionObjectEffectTargeter(Serial serial): base(serial)
        {   
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
           
            writer.Write(m_Radius);
            writer.Write(m_ConstructionTile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
           
            m_Radius = reader.ReadInt();
            m_ConstructionTile = (UOACZConstructionTile)reader.ReadItem();
        }
    }
}