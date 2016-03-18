using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Items
{

    public class FjordjinGorget : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }
        //Changed to IPY values
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 45; } }
        public override int OldStrReq { get { return 30; } }

        public override int OldDexBonus { get { return -1; } }
        //Changed to IPY values
        public override int ArmorBase { get { return 30; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

        [Constructable]
        public FjordjinGorget()
            : base(0x1413)
        {
            Weight = 2.0;
            Name = "Fjordjin, the wrath collar";
            Hue = 2707;
            ProtectionLevel = ArmorProtectionLevel.Fortification;
            Durability = ArmorDurabilityLevel.Indestructible;

        }

        public override bool OnEquip(Mobile from)
        {
            from.PlaySound(Utility.RandomList(0x166, 0x170));
            return base.OnEquip(from);

        }

        public FjordjinGorget(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x2645, 0x2646)]
    public class YigalrothHelm : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 55; } }
        public override int InitMaxHits { get { return 75; } }

        public override int AosStrReq { get { return 75; } }
        public override int OldStrReq { get { return 40; } }

        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 40; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Dragon; } }
        public override CraftResource DefaultResource { get { return CraftResource.RedScales; } }

        [Constructable]
        public YigalrothHelm()
            : base(0x2645)
        {
            Weight = 5.0;
            Name = "Yigalroth, the pestilence shroud";
            Hue = 2707;
            ProtectionLevel = ArmorProtectionLevel.Fortification;
            Durability = ArmorDurabilityLevel.Indestructible;

        }

        public override bool OnEquip(Mobile from)
        {
            from.PlaySound(Utility.RandomList(0x166, 0x170));
            return base.OnEquip(from);
           
        }

        public YigalrothHelm(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Weight == 1.0)
                Weight = 5.0;
        }
    }
   
        [FlipableAttribute(0x13f0, 0x13f1)]
        public class VashreLegs : BaseArmor
        {
            public override int BasePhysicalResistance { get { return 3; } }
            public override int BaseFireResistance { get { return 3; } }
            public override int BaseColdResistance { get { return 1; } }
            public override int BasePoisonResistance { get { return 5; } }
            public override int BaseEnergyResistance { get { return 3; } }

            public override int InitMinHits { get { return 40; } }
            public override int InitMaxHits { get { return 50; } }

            public override int AosStrReq { get { return 40; } }
            public override int OldStrReq { get { return 20; } }

            //Changed to IPY values
            public override int OldDexBonus { get { return 0; } }
            //Changed to IPY values
            public override int ArmorBase { get { return 20; } }
            //Added by IPY
            public override int RevertArmorBase { get { return 3; } }

            public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

            [Constructable]
            public VashreLegs()
                : base(0x13F0)
            {
                Weight = 15.0;
                Name = "Vash're, the doom greaves";
                Hue = 2707;
                ProtectionLevel = ArmorProtectionLevel.Fortification;
                Durability = ArmorDurabilityLevel.Indestructible;

            }

            public override bool OnEquip(Mobile from)
            {
                from.PlaySound(Utility.RandomList(0x166, 0x170));
                return base.OnEquip(from);
                
            }

            public VashreLegs(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    


        [FlipableAttribute(0x13eb, 0x13f2)]
         public class ApoxisGloves : BaseArmor
        {
        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 1; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }

        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }
        //Changed to IPY values
        public override int OldDexBonus { get { return 0; } }
        //Changed to IPY values
        public override int ArmorBase { get { return 20; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

        [Constructable]
        public ApoxisGloves()
            : base(0x13EB)
        {
            Weight = 2.0;
            Name = "Apoxis, the death grips";
            Hue = 2707;
            ProtectionLevel = ArmorProtectionLevel.Fortification;
            Durability = ArmorDurabilityLevel.Indestructible;

        }

        public override bool OnEquip(Mobile from)
        {

            from.PlaySound(Utility.RandomList(0x166, 0x170));
            return base.OnEquip(from);
           
        }

        public ApoxisGloves(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Weight == 1.0)
                Weight = 2.0;
        }
    }

        [FlipableAttribute(0x13ec, 0x13ed)]
        public class OrghereimChest : BaseArmor
        {
            public override int BasePhysicalResistance { get { return 3; } }
            public override int BaseFireResistance { get { return 3; } }
            public override int BaseColdResistance { get { return 1; } }
            public override int BasePoisonResistance { get { return 5; } }
            public override int BaseEnergyResistance { get { return 3; } }

            public override int InitMinHits { get { return 40; } }
            public override int InitMaxHits { get { return 50; } }

            public override int AosStrReq { get { return 40; } }
            public override int OldStrReq { get { return 20; } }
            //Changed to IPY values
            public override int OldDexBonus { get { return 0; } }
            //Changed to IPY values
            public override int ArmorBase { get { return 20; } }
            //Added by IPY
            public override int RevertArmorBase { get { return 9; } }

            public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

            [Constructable]
            public OrghereimChest()
                : base(0x13EC)
            {
                Weight = 15.0;
                Hue = 2707;
                Name = "Orghereim, the grim chest";
                ProtectionLevel = ArmorProtectionLevel.Fortification;
                Durability = ArmorDurabilityLevel.Indestructible;

            }

            public override bool OnEquip(Mobile from)
            {

                from.PlaySound(Utility.RandomList(0x166, 0x170));
                return base.OnEquip(from);
               
            }

            public OrghereimChest(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                if (Weight == 1.0)
                    Weight = 15.0;
            }
        }
        
        [FlipableAttribute(0x13ee, 0x13ef)]
        public class HergamnonArms : BaseArmor
        {
            public override int BasePhysicalResistance { get { return 3; } }
            public override int BaseFireResistance { get { return 3; } }
            public override int BaseColdResistance { get { return 1; } }
            public override int BasePoisonResistance { get { return 5; } }
            public override int BaseEnergyResistance { get { return 3; } }

            public override int InitMinHits { get { return 40; } }
            public override int InitMaxHits { get { return 50; } }

            public override int AosStrReq { get { return 40; } }
            public override int OldStrReq { get { return 20; } }
            //Changed to IPY values
            public override int OldDexBonus { get { return 0; } }
            //Changed to IPY values
            public override int ArmorBase { get { return 20; } }
            //Added by IPY
            public override int RevertArmorBase { get { return 2; } }

            public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

            [Constructable]
            public HergamnonArms()
                : base(0x13EE)
            {
                Weight = 15.0;
                Hue = 2707;
                Name = "Hergamnon, the plague bracers";
                ProtectionLevel = ArmorProtectionLevel.Fortification;
                Durability = ArmorDurabilityLevel.Indestructible;
            }

            public override bool OnEquip(Mobile from)
            {

                from.PlaySound(Utility.RandomList(0x166, 0x170));
                return base.OnEquip(from);
                

            }

            public HergamnonArms(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                if (Weight == 1.0)
                    Weight = 15.0;

            }
        }

        

    }
