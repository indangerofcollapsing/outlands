using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonCore : Item
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
        public DungeonCore(): base(3985)
		{
            Name = "a dungeon core";

            Stackable = true;
            Amount = 1;
            Weight = 1;
		}

        [Constructable]
        public DungeonCore(int amount): base(3985)
		{
            Name = "a dungeon core";

            Stackable = true;
            Amount = amount;
            Weight = 1;
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, GetDungeonName(Dungeon).ToLower() + " dungeon core : " + Amount.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use a dungeon mould to create or upgrade an existing dungeon weapon.");
        }

        public static DungeonCore GetRandomDungeonCore(int amount)
        {
            DungeonCore item = null;

            int dungeonIndex = Utility.RandomMinMax(1, 9);

            switch (dungeonIndex)
            {                
                case 1: item = new CovetousDungeonCore(amount); break;
                case 2: item = new DespiseDungeonCore(amount); break;
                case 3: item = new DestardDungeonCore(amount); break;
                case 4: item = new DeceitDungeonCore(amount); break;
                case 5: item = new FireDungeonCore(amount); break;
                case 6: item = new HythlothDungeonCore(amount); break;
                case 7: item = new IceDungeonCore(amount); break;
                case 8: item = new ShameDungeonCore(amount); break;
                case 9: item = new WrongDungeonCore(amount); break;             
            }

            return item;
        }

        public DungeonCore(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

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