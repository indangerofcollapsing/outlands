using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonDistillation : Item
    {
        private BaseDungeonArmor.DungeonEnum m_Dungeon = BaseDungeonArmor.DungeonEnum.Shame;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDungeonArmor.DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set
            {
                m_Dungeon = value;

                BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(m_Dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }        

        [Constructable]
        public DungeonDistillation(): base(6198)
        {
            Name = "dungeon distillation";

            int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;

            Dungeon = (BaseDungeonArmor.DungeonEnum)Utility.RandomMinMax(1, dungeonCount - 1);
        }

        [Constructable]
        public DungeonDistillation(BaseDungeonArmor.DungeonEnum dungeonType): base(6198)
        {
            Name = "dungeon distillation";

            Dungeon = dungeonType;
        }

        public DungeonDistillation(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, BaseDungeonArmor.GetDungeonName(Dungeon).ToLower() + " dungeon distillation");
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
                Dungeon = (BaseDungeonArmor.DungeonEnum)reader.ReadInt();
            }
        }
    }
}