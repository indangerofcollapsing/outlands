using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server.Items
{
    public class BaseArmoredHat : BaseArmor, IDyable
    {
        #region IDyable
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack to dye.");
                return false;
            }
            else if (!CanBeDyed)
            {
                from.SendMessage("This hat cannot be dyed.");
                return false;
            }

            Hue = sender.DyedHue;

            return true;
        }
        #endregion

        public virtual bool CanBeDyed { get { return true; } }
        public virtual bool CanBeBlessed { get { return true; } }

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 40; } }

        public override int AosStrReq { get { return 20; } }
        public override int OldStrReq { get { return 15; } }

        public override int ArmorBase { get { return 13; } }
        //Added by IPY
        public override int RevertArmorBase { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        public BaseArmoredHat(int ItemID)
            : base(ItemID)
        {
            Quality = ArmorQuality.Exceptional;
        }

        public BaseArmoredHat(Serial serial)
            : base(serial)
        {
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            int result = base.OnCraft(quality, makersMark, from, craftSystem, typeRes, tool, craftItem, resHue);
            Hue = resHue;
            return result;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version == 0)
                Quality = ArmorQuality.Exceptional;
        }
    }
}
