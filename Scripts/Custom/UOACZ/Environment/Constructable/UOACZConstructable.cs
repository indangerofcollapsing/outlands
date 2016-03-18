using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Custom
{
    public class UOACZConstructable : UOACZBreakableStatic
    {
        private UOACZConstructionTile m_ConstructionTile;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZConstructionTile ConstructionTile
        {
            get { return m_ConstructionTile; }
            set { m_ConstructionTile = value; }
        }

        [Constructable]
        public UOACZConstructable(): base()
        {
            Name = "constructable object";

            InteractionRange = 1;
            
            DamageState = DamageStateType.Broken;
            HitPoints = 0;
        }

        public UOACZConstructable(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_ConstructionTile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ConstructionTile = (UOACZConstructionTile)reader.ReadItem();
            }
        }
    }
}
