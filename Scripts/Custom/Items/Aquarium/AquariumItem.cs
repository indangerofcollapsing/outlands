using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items
{
    public class AquariumItem : Item
    {
        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            UltraRare
        }

        public enum Type
        {
            Fish,
            Decoration
        }

        public virtual string DescriptionA { get { return ""; } }
        public virtual string DescriptionB { get { return ""; } }
        public virtual Rarity ItemRarity { get { return Rarity.Common; } }

        public virtual Type ItemType { get { return Type.Decoration; } }
        public virtual int ItemId { get { return 15112; } }
        public virtual int ItemHue { get { return 0; } }

        public virtual int OffsetX { get { return 0; } }
        public virtual int OffsetY { get { return 0; } }

        public virtual int MinWeight { get { return 5; } }
        public virtual int MaxWeight { get { return 25; } }

        [Constructable]
        public AquariumItem(): base(0x3B12)
        {
            Name = "an aquarium item";
            ItemID = ItemId;

            if (ItemType == Type.Fish)
                Weight = Utility.RandomMinMax(MinWeight, MaxWeight);
            else
                Weight = 1;

            Hue = ItemHue;
        }

        public static AquariumItem GetRandomFish(Rarity rarity)
        {
            AquariumItem item = null;

            switch (rarity)
            {
                case Rarity.Common:
                    switch(Utility.RandomMinMax(1, 6))
                    {
                        case 1: item = new BlueDasyllus(); break;
                        case 2: item = new BlueTriggerfish(); break;
                        case 3: item = new RedscaleSnapper(); break;
                        case 4: item = new Sandalfish(); break;
                        case 5: item = new Squirrelfish(); break;
                        case 6: item = new Suntail(); break;
                    }
                break;

                case Rarity.Uncommon:
                    switch (Utility.RandomMinMax(1, 11))
                    {
                        case 1: item = new Angelfish(); break;
                        case 2: item = new CoastalWaterfrog(); break;
                        case 3: item = new ColossusShrimp(); break;
                        case 4: item = new KingCrab(); break;
                        case 5: item = new LongnoseButterfly(); break;
                        case 6: item = new Pufferfish(); break;
                        case 7: item = new Shinepike(); break;
                        case 8: item = new StubnoseSwordfish(); break;
                        case 9: item = new TaperedLeopardfin(); break;
                        case 10: item = new Tigerfish(); break;
                        case 11: item = new Tropicola(); break;
                    }
                break;

                case Rarity.Rare:
                    switch (Utility.RandomMinMax(1, 7))
                    {
                        case 1: item = new Bluefin(); break;
                        case 2: item = new EmperorSeahorse(); break;
                        case 3: item = new GreyCoris(); break;
                        case 4: item = new RoyalGoldfish(); break;
                        case 5: item = new StripedIdol(); break;
                        case 6: item = new VioletZebrafish(); break;
                        case 7: item = new WhitetailedRay(); break;
                    }
                break;

                case Rarity.UltraRare:
                    switch (Utility.RandomMinMax(1,4))
                    {
                        case 1: item = new Ghostfin(); break;
                        case 2: item = new JuvenileReefShark(); break;
                        case 3: item = new JuvenileSeaSerpent(); break;
                        case 4: item = new LesserWaterElemental(); break;
                    }
                break;
            }            

            return item;
        }

        public static AquariumItem GetRandomDecoration(Rarity rarity)
        {
            AquariumItem item = null;

            switch (rarity)
            {
                case Rarity.Common:
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: item = new ConchShell(); break;
                        case 2: item = new DrownedFishermansBoots(); break;
                        case 3: item = new DrownedFishermansHat(); break;
                        case 4: item = new SeaVines(); break;
                        case 5: item = new SunkenBarrelStaves(); break;
                        case 6: item = new SeaCactus(); break;
                        case 7: item = new LargeSeaCactus(); break;
                        case 8: item = new SeaGrass(); break;
                    }
                break;

                case Rarity.Uncommon:
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: item = new GiantClam(); break;
                        case 2: item = new GiantConchShell(); break;
                        case 3: item = new SeaCoral(); break;
                        case 4: item = new SubmergedCorpse(); break;
                        case 5: item = new SunkenBarrel(); break;
                        case 6: item = new SunkenChest(); break;
                        case 7: item = new SunkenColumn(); break;
                        case 8: item = new SunkenVase(); break; 
                    }
                break;

                case Rarity.Rare:
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: item = new DrownedPrisoner(); break;
                        case 2: item = new SunkenAnchor(); break;
                        case 3: item = new SunkenColumn(); break;
                        case 4: item = new SunkenNetting(); break;
                        case 5: item = new SunkenUrn(); break;
                    }
                break;

                case Rarity.UltraRare:
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: item = new GiantSeaMushroom(); break;
                        case 2: item = new StrangeSunkenStatue(); break;
                        case 3: item = new SunkenArmor(); break;
                        case 4: item = new SunkenOrnamentalVase(); break;
                        case 5: item = new SunkenPedestal(); break;
                    }
                break;
            }

            return item;
        }

        public static int GetRarityTextColor(Rarity rarity)
        {
            int textcolor = 2036;

            switch (rarity)
            {
                case Rarity.Common: textcolor = 2036; break; //White
                case Rarity.Uncommon: textcolor = 2208; break; //Green
                case Rarity.Rare: textcolor = 2119; break; //Blue
                case Rarity.UltraRare: textcolor = 1258; break; //Orange
            }

            return textcolor;
        }

        public static string GetRarityTextName(Rarity rarity)
        {
           string text = "Common";

            switch (rarity)
            {
                case Rarity.Common: text = "Common"; break;
                case Rarity.Uncommon: text = "Uncommon"; break;
                case Rarity.Rare: text = "Rare"; break;
                case Rarity.UltraRare: text = "Ultra Rare"; break;
            }

            return text;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(an aquarium item)");
        }

        public AquariumItem(Serial serial): base(serial)
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
}
