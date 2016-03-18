using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0x1410, 0x1417)]
    public class DungeonArmorPlateArms : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 80; } }
        public override int OldStrReq { get { return 40; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return -1; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateArms(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1410, dungeon, armorTier)
        {          
            Weight = 5.0;
        }

        public DungeonArmorPlateArms(Serial serial): base(serial)
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

    [FlipableAttribute(0x1415, 0x1416)]
    public class DungeonArmorPlateChest : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 95; } }
        public override int OldStrReq { get { return 60; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return -5; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateChest(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1415, dungeon, armorTier)
        {   
            Weight = 10.0;
        }

        public DungeonArmorPlateChest(Serial serial): base(serial)
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

    [FlipableAttribute(0x1414, 0x1418)]
    public class DungeonArmorPlateGloves : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 70; } }
        public override int OldStrReq { get { return 30; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateGloves(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1414, dungeon, armorTier)
        {
            Weight = 2.0;
        }

        public DungeonArmorPlateGloves(Serial serial): base(serial)
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

    public class DungeonArmorPlateGorget : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 45; } }
        public override int OldStrReq { get { return 30; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateGorget(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1413, dungeon, armorTier)
        {           
            Weight = 2.0;
        }

        public DungeonArmorPlateGorget(Serial serial): base(serial)
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

    public class DungeonArmorPlateHelm : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 80; } }
        public override int OldStrReq { get { return 40; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return -1; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateHelm(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1412, dungeon, armorTier)
        {         
            Weight = 5.0;
        }

        public DungeonArmorPlateHelm(Serial serial): base(serial)
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

    [FlipableAttribute(0x1411, 0x141a)]
    public class DungeonArmorPlateLegs : BaseDungeonArmor
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 90; } }

        public override int OldStrReq { get { return 60; } }

        public override int RevertArmorBase { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 0; } }
        public override int OldDexBonus { get { return -3; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public DungeonArmorPlateLegs(DungeonEnum dungeon, ArmorTierEnum armorTier): base(0x1411, dungeon, armorTier)
        {            
            Weight = 7.0;
        }

        public DungeonArmorPlateLegs(Serial serial): base(serial)
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

    #region Dungeon Armor Bags

    public class BagOfDungeonArmor : Bag
    {
        [Constructable]
        public BagOfDungeonArmor(BaseDungeonArmor.DungeonEnum dungeon, BaseDungeonArmor.ArmorTierEnum tier)
        {
            BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(dungeon, tier);

            if (detail == null)
                return;

            Name = "dungeon armor set: " + detail.DungeonName + " dungeon";
            Hue = detail.Hue;           

            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Helmet));
            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Gorget));
            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Chest));
            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Arms));
            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Gloves));
            DropItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, tier, BaseDungeonArmor.ArmorLocation.Legs));
        }

        public BagOfDungeonArmor(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfDestardTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfDestardTier1(): base(BaseDungeonArmor.DungeonEnum.Destard, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfDestardTier1(Serial serial): base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfDeceitTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfDeceitTier1(): base(BaseDungeonArmor.DungeonEnum.Deceit, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfDeceitTier1(Serial serial): base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfHythlothTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfHythlothTier1(): base(BaseDungeonArmor.DungeonEnum.Hythloth, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfHythlothTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfShameTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfShameTier1(): base(BaseDungeonArmor.DungeonEnum.Shame, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfShameTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfCovetousTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfCovetousTier1(): base(BaseDungeonArmor.DungeonEnum.Covetous, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfCovetousTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfWrongTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfWrongTier1(): base(BaseDungeonArmor.DungeonEnum.Wrong, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfWrongTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfDespiseTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfDespiseTier1(): base(BaseDungeonArmor.DungeonEnum.Despise, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfDespiseTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfIceTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfIceTier1(): base(BaseDungeonArmor.DungeonEnum.Ice, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfIceTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BagOfFireTier1 : BagOfDungeonArmor
    {
        [Constructable]
        public BagOfFireTier1(): base(BaseDungeonArmor.DungeonEnum.Fire, BaseDungeonArmor.ArmorTierEnum.Tier1)
        {
        }

        public BagOfFireTier1(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion
}