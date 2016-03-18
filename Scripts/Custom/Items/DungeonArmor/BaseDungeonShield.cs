using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells.Fourth;
using Server.Spells;
using Server.SkillHandlers;
using Server.Achievements;
using Server.Gumps;

namespace Server.Items
{   
    public class DungeonShield : BaseDungeonShield
    {
        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 1; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 300; } }
        public override int InitMaxHits { get { return 300; } }

        public override int AosStrReq { get { return 90; } }

        public override int ArmorBase { get { return 24; } }
        public override int OldDexBonus { get { return -7; } }

        [Constructable]
        public DungeonShield(BaseDungeonArmor.DungeonEnum dungeon, BaseDungeonArmor.ArmorTierEnum armorTier): base(0x2B01, dungeon, armorTier)
        {          
            Weight = 8.0;
        }

        public DungeonShield(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public abstract class BaseDungeonShield : BaseShield
    { 
        private int m_BlessedCharges = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BlessedCharges
        {
            get { return m_BlessedCharges; }
            set
            { 
                m_BlessedCharges = value;

                if (m_BlessedCharges > m_MaxBlessedCharges)
                    m_BlessedCharges = m_MaxBlessedCharges;
            }
        }

        private int m_MaxBlessedCharges = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxBlessedCharges
        {
            get { return m_MaxBlessedCharges; }
            set 
            { 
                m_MaxBlessedCharges = value;

                if (m_BlessedCharges > m_MaxBlessedCharges)
                    m_BlessedCharges = m_MaxBlessedCharges;
            }
        }

        private BaseDungeonArmor.DungeonEnum m_Dungeon;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDungeonArmor.DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set 
            {                
                m_Dungeon = value;

                BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(m_Dungeon, m_Tier);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }

        private BaseDungeonArmor.ArmorTierEnum m_Tier;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDungeonArmor.ArmorTierEnum Tier
        {
            get { return m_Tier; }
            set { m_Tier = value; }
        }

        [Constructable]
        public BaseDungeonShield(int itemId, BaseDungeonArmor.DungeonEnum dungeon, BaseDungeonArmor.ArmorTierEnum armorTier) : base(itemId)
        {           
            Dungeon = dungeon;
            Tier = armorTier;
        }

        public static BaseDungeonShield CreateDungeonShield(BaseDungeonArmor.DungeonEnum dungeon, BaseDungeonArmor.ArmorTierEnum armorTier)
        {
            BaseDungeonShield shield = null;

            if (dungeon == BaseDungeonArmor.DungeonEnum.Unspecified)
            {
                int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;
                
                dungeon = (BaseDungeonArmor.DungeonEnum)Utility.RandomMinMax(1, dungeonCount - 1);
            }

            if (armorTier == BaseDungeonArmor.ArmorTierEnum.Unspecified)
            {
                int armorTierCount = Enum.GetNames(typeof(BaseDungeonArmor.ArmorTierEnum)).Length;
                armorTier = (BaseDungeonArmor.ArmorTierEnum)Utility.RandomMinMax(1, armorTierCount - 1);
            }

            shield = new DungeonShield(dungeon, armorTier);

            BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(dungeon, armorTier);

            if (armorTier != null && detail != null)
                shield.Hue = detail.Hue;

            return shield;
        }

        public BaseDungeonShield(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            string sName = m_Dungeon.ToString().ToLower() + " dungeon ";

            sName += "shield";
            sName += ": tier " + ((int)Tier).ToString();

            LabelTo(from, sName);
            LabelTo(from, String.Format("[blessed charges: {0}/{1}]", BlessedCharges, MaxBlessedCharges));
        }

        public override void OnDoubleClick(Mobile from)
        {
            //from.CloseGump(typeof(BaseDungeonArmor.DungeonArmorGump));
            //from.SendGump(new BaseDungeonArmor.DungeonArmorGump(this, from));
        }

        public override bool CheckBlessed(Mobile mobile, bool isOnDeath = false)
        {
            PlayerMobile player = mobile as PlayerMobile;

            if (player == null)
                return false;

            if (BlessedCharges > 0)
            {
                if (isOnDeath)
                    BlessedCharges--;

                return true;
            }

            else
                return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); //Version

            writer.Write((int)m_Dungeon);
            writer.Write((int)m_Tier);  
        
            //Version 1
            writer.Write((int)m_BlessedCharges);

            //Version 2
            writer.Write((int)m_MaxBlessedCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_Dungeon = (BaseDungeonArmor.DungeonEnum)reader.ReadInt();
                m_Tier = (BaseDungeonArmor.ArmorTierEnum)reader.ReadInt();
                m_BlessedCharges = reader.ReadInt();
            }

            if (version >= 2)
            {
                m_MaxBlessedCharges = reader.ReadInt();
            }           
        }             
    }    
}