using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonDistillation : Item
    {
        private DungeonEnum m_Dungeon = DungeonEnum.Shame;
        [CommandProperty(AccessLevel.GameMaster)]
        public DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set
            {
                m_Dungeon = value;

                DungeonArmor.DungeonArmorDetail detail = new DungeonArmor.DungeonArmorDetail(m_Dungeon, 1);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }        

        [Constructable]
        public DungeonDistillation(): base(6198)
        {
            Name = "dungeon distillation";

            int dungeonCount = Enum.GetNames(typeof(DungeonEnum)).Length;

            Dungeon = (DungeonEnum)Utility.RandomMinMax(1, dungeonCount - 1);
        }

        [Constructable]
        public DungeonDistillation(DungeonEnum dungeonType): base(6198)
        {
            Name = "dungeon distillation";

            Dungeon = dungeonType;
        }

        public DungeonDistillation(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, GetDungeonName(Dungeon).ToLower() + " dungeon distillation");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use this with a dungeon mould and a dungeon cores to create or upgrade an existing dungeon weapon.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
    
            //Version 0
            writer.Write((int)m_Dungeon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                Dungeon = (DungeonEnum)reader.ReadInt();
            }
        }
    }
}