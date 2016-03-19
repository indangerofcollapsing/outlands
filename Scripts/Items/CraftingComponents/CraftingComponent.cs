using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class CraftingComponent : Item
    {
        public enum CraftingComponentType
        {
            BluecapMushroom,
            Creepervine,
            CockatriceEgg,
            FireEssence,
            Ghostweed,
            GhoulHide,
            LuniteHeart,
            ObsidianShard,
            Quartzstone,
            ShatteredCrystal,
            Snakeskin,
            TrollFat
        }

        [Constructable]
        public CraftingComponent(): base(3350)
		{
            Name = "a crafting component";

            Stackable = true;
            Amount = 1;
            Weight = 0.1;
		}

        [Constructable]
        public CraftingComponent(int amount): base(3350)
		{
            Name = "a crafting component";

            Stackable = true;
            Amount = amount;
            Weight = 0.1;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(crafting component)");
        }

        public static CraftingComponent GetRandomCraftingComponent(int amount)
        {
            CraftingComponent item = null;
            
            int craftingIndex = Utility.RandomMinMax(1, 12);

            switch (craftingIndex)
            {
                case 1: item = new BluecapMushroom(amount); break;
                case 2: item = new CockatriceEgg(amount); break;
                case 3: item = new Creepervine(amount); break;
                case 4: item = new FireEssence(amount); break;
                case 5: item = new Ghostweed(amount); break;
                case 6: item = new GhoulHide(amount); break;
                case 7: item = new LuniteHeart(amount); break;
                case 8: item = new ObsidianShard(amount); break;
                case 9: item = new Quartzstone(amount); break;
                case 10: item = new ShatteredCrystal(amount); break;
                case 11: item = new Snakeskin(amount); break;
                case 12: item = new TrollFat(amount); break;
            }

            return item;
        }

        public static CraftingComponentDetail GetCraftingComponentDetail(CraftingComponent.CraftingComponentType craftingComponent)
        {
            CraftingComponentDetail detail = new CraftingComponentDetail();

            switch (craftingComponent)
            {
                case CraftingComponentType.BluecapMushroom:
                    detail.m_ItemId = 3350;
                    detail.m_Hue = 2599;
                    detail.m_OffsetX = -2;
                    detail.m_OffsetY = 0;
                break;
                case CraftingComponentType.CockatriceEgg:
                    detail.m_ItemId = 10249;
                    detail.m_Hue = 2589;
                    detail.m_OffsetX = 0;
                    detail.m_OffsetY = 3;
                break;
                case CraftingComponentType.Creepervine:
                    detail.m_ItemId = 22311;
                    detail.m_Hue = 2208;
                    detail.m_OffsetX = -17;
                    detail.m_OffsetY = -10;
                break;
                case CraftingComponentType.FireEssence:
                    detail.m_ItemId = 16395;
                    detail.m_Hue = 2075;
                    detail.m_OffsetX = 0;
                    detail.m_OffsetY = 3;
                break;
                case CraftingComponentType.Ghostweed:
                    detail.m_ItemId = 731;
                    detail.m_Hue = 2498;
                    detail.m_OffsetX = 5;
                    detail.m_OffsetY = -5;
                break;
                case CraftingComponentType.GhoulHide:
                    detail.m_ItemId = 12677;
                    detail.m_Hue = 2610;
                    detail.m_OffsetX = 0;
                    detail.m_OffsetY = 3;                    
                break;
                case CraftingComponentType.LuniteHeart:
                    detail.m_ItemId = 12126;
                    detail.m_Hue = 2605;
                    detail.m_OffsetX = 2;
                    detail.m_OffsetY = 3; 
                break;
                case CraftingComponentType.ObsidianShard:
                    detail.m_ItemId = 11703;
                    detail.m_Hue = 1102;
                    detail.m_OffsetX = 3;
                    detail.m_OffsetY = 0; 
                break;
                case CraftingComponentType.Quartzstone:
                    detail.m_ItemId = 5925;
                    detail.m_Hue = 2507;
                    detail.m_OffsetX = -3;
                    detail.m_OffsetY = 2; 
                break;
                case CraftingComponentType.ShatteredCrystal:
                    detail.m_ItemId = 22328;
                    detail.m_Hue = 84;
                    detail.m_OffsetX = -2;
                    detail.m_OffsetY = 0; 
                break;
                case CraftingComponentType.Snakeskin:
                    detail.m_ItemId = 22340;
                    detail.m_Hue = 2515;
                    detail.m_OffsetX = 10;
                    detail.m_OffsetY = -7; 
                break;
                case CraftingComponentType.TrollFat:
                    detail.m_ItemId = 5163;
                    detail.m_Hue = 2612;
                    detail.m_OffsetX = -3;
                    detail.m_OffsetY = 0; 
                break;
            }

            return detail;
        }

        public CraftingComponent(Serial serial): base(serial)
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

    public class CraftingComponentDetail
    {
        public CraftingComponent.CraftingComponentType m_CraftingComponentType = CraftingComponent.CraftingComponentType.BluecapMushroom;
        public int m_ItemId = 3350;
        public int m_Hue  = 2599;
        public int m_OffsetX = 0;
        public int m_OffsetY = 0;

        public CraftingComponentDetail()
        {
        }
    }
}