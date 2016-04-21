using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.Items
{
    public static class ShipDeckItems
    {
        public static void GenerateShipDeckItems(BaseBoat boat)
        {
            if (boat == null) return;
            if (boat.Deleted) return;

            int shipLevel = 1;

            if (boat is MediumBoat || boat is MediumDragonBoat) shipLevel = 2;
            if (boat is LargeBoat || boat is LargeDragonBoat) shipLevel = 3;
            if (boat is CarrackBoat) shipLevel = 4;
            if (boat is GalleonBoat) shipLevel = 5;

            int deckItems = 5 + (shipLevel * 3);            
        }

        public static Item GetRandomTreasureShipItem()
        {
            List<Item> m_Items = new List<Item>();

            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2634 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2635 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2636 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2634 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2635 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2636 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2634 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2635 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2636 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2634 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2635 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2636 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2634 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2635 });
            m_Items.Add(new Static(3823) { Name = "gold", Hue = 2636 });

            m_Items.Add(new Static(6976) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6977) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6987) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6991) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6992) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6997) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(7007) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(7014) { Name = "treasure", Hue = 0 });

            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6986) { Name = "treasure", Hue = 0 });
            m_Items.Add(new Static(6996) { Name = "treasure", Hue = 0 });

            m_Items.Add(new Static(7146) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7147) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7149) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7150) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7146) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7147) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7149) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7150) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7146) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7147) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7149) { Name = "gold bars", Hue = 0 });
            m_Items.Add(new Static(7150) { Name = "gold bars", Hue = 0 });

            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2600 });
            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2576 });
            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2117 });

            m_Items.Add(new Static(2466) { Name = "exotic liquor", Hue = 2577 });
            m_Items.Add(new Static(2462) { Name = "exotic liquor", Hue = 2626 });
            m_Items.Add(new Static(2500) { Name = "exotic liquor", Hue = 2500 });

            m_Items.Add(new Static(2458) { Name = "goblet", Hue = 2515 });
            m_Items.Add(new Static(2458) { Name = "goblet", Hue = 2515 });

            m_Items.Add(new Static(3663) { Name = "gold jars", Hue = 2515 });
            m_Items.Add(new Static(3654) { Name = "gold jars", Hue = 2515 });

            /*
            m_Items.Add(new Static(2445) { Name = "small urns", Hue = 2500 });
            m_Items.Add(new Static(2446) { Name = "small urns", Hue = 2500 });

            m_Items.Add(new Static(2451) { Name = "gilded fruit", Hue = 2515 });

            m_Items.Add(new Static(2466) { Name = "exotic liquor", Hue = 2577 });
            m_Items.Add(new Static(2462) { Name = "exotic liquor", Hue = 2626 });
            m_Items.Add(new Static(2500) { Name = "exotic liquor", Hue = 2500 });

            m_Items.Add(new Static(2491) { Name = "roast pig", Hue = 0 });

            m_Items.Add(new Static(3633) { Name = "brazier", Hue = 2658 });

            m_Items.Add(new Static(3653) { Name = "gold jars", Hue = 2515 });
            m_Items.Add(new Static(3654) { Name = "gold jars", Hue = 2515 });
            m_Items.Add(new Static(3659) { Name = "gold jars", Hue = 2515 });
            m_Items.Add(new Static(3662) { Name = "gold jars", Hue = 2515 });

            m_Items.Add(new Static(3629) { Name = "gold orb", Hue = 2635 });

            m_Items.Add(new Static(3764) { Name = "gold lute", Hue = 2635 });           

            m_Items.Add(new Static(3835) { Name = "exotic perfume", Hue = 0 });
            m_Items.Add(new Static(3836) { Name = "exotic perfume", Hue = 0 });
            m_Items.Add(new Static(3841) { Name = "exotic perfume", Hue = 0 });
            m_Items.Add(new Static(3844) { Name = "exotic perfume", Hue = 0 });

            m_Items.Add(new Static(4091) { Name = "mug", Hue = 0 });
            m_Items.Add(new Static(4098) { Name = "mug", Hue = 0 });

            m_Items.Add(new Static(4810) { Name = "statue", Hue = 2500 });
            m_Items.Add(new Static(5018) { Name = "statue", Hue = 2500 });
            m_Items.Add(new Static(5019) { Name = "statue", Hue = 2500 });
            m_Items.Add(new Static(5020) { Name = "statue", Hue = 2500 });
            m_Items.Add(new Static(5021) { Name = "statue", Hue = 2500 });

            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2600 });
            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2576 });
            m_Items.Add(new Static(5033) { Name = "lavish silk", Hue = 2117 });

            m_Items.Add(new Static(5030) { Name = "lavish pillow", Hue = 2591 });
            m_Items.Add(new Static(5036) { Name = "lavish pillow", Hue = 2576 });

            m_Items.Add(new Static(5159) { Name = "jade tablets", Hue = 2601 });
            m_Items.Add(new Static(5157) { Name = "jade tablets", Hue = 2601 });
            */

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }

        public static Item GetRandomDerelictShipItem()
        {
            List<Item> m_Items = new List<Item>();
            
            m_Items.Add(new Static(4650) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4651) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4653) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4654) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4655) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4650) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4651) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4653) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4654) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4655) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4650) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4651) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4653) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4654) { Name = "water", Hue = 2119 });
            m_Items.Add(new Static(4655) { Name = "water", Hue = 2119 });

            m_Items.Add(new Static(2321) { Name = "dirt", Hue = 0 });
            m_Items.Add(new Static(2322) { Name = "dirt", Hue = 0 });

            m_Items.Add(new Static(3118) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3119) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3120) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3118) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3119) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3120) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3118) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3119) { Name = "debris", Hue = 0 });
            m_Items.Add(new Static(3120) { Name = "debris", Hue = 0 });

            m_Items.Add(new Static(3157) { Name = "weeds", Hue = 2208 });
            m_Items.Add(new Static(3158) { Name = "weeds", Hue = 2208 });
            m_Items.Add(new Static(3211) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3212) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3219) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3244) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3245) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3246) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3247) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3248) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3249) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3250) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3251) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3252) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3253) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3254) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3255) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3256) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3257) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3258) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3270) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3378) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(3379) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(6809) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(6810) { Name = "weeds", Hue = 0 });
            m_Items.Add(new Static(6811) { Name = "weeds", Hue = 0 });

            m_Items.Add(new Static(3340) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3341) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3342) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3343) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3344) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3345) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3346) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3347) { Name = "mushrooms", Hue = 0 });
            m_Items.Add(new Static(3348) { Name = "mushrooms", Hue = 0 });

            m_Items.Add(new Static(3387) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(3388) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(3389) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7041) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7044) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7051) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7053) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7068) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7604) { Name = "branches", Hue = 0 });
            m_Items.Add(new Static(7605) { Name = "branches", Hue = 0 });

            m_Items.Add(new Static(7607) { Name = "barrel hoops", Hue = 0 });

            m_Items.Add(new Static(3391) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(3392) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(12320) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(12321) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(12322) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(12323) { Name = "brambles", Hue = 2207 });
            m_Items.Add(new Static(12324) { Name = "brambles", Hue = 2207 });

            m_Items.Add(new Static(3514) { Name = "seaweed", Hue = 2208 });
            m_Items.Add(new Static(3515) { Name = "seaweed", Hue = 2208 });
            m_Items.Add(new Static(3514) { Name = "seaweed", Hue = 2208 });
            m_Items.Add(new Static(3515) { Name = "seaweed", Hue = 2208 });

            m_Items.Add(new Static(3337) { Name = "lilypad", Hue = 0 });
            m_Items.Add(new Static(3338) { Name = "lilypad", Hue = 0 });
            m_Items.Add(new Static(3339) { Name = "lilypads", Hue = 0 });
            m_Items.Add(new Static(3516) { Name = "lilypad", Hue = 0 });
            m_Items.Add(new Static(3517) { Name = "lilypad", Hue = 0 });
            m_Items.Add(new Static(3518) { Name = "lilypads", Hue = 0 });

            m_Items.Add(new Static(12585) { Name = "sapling", Hue = 0 });
            m_Items.Add(new Static(12586) { Name = "sapling", Hue = 0 });
            m_Items.Add(new Static(6914) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6915) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6916) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6917) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6918) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6919) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6920) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6913) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6914) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6915) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6916) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6917) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6918) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6919) { Name = "cracks", Hue = 0 });
            m_Items.Add(new Static(6920) { Name = "cracks", Hue = 0 });

            m_Items.Add(new Static(6943) { Name = "leaves", Hue = 2208 });
            m_Items.Add(new Static(6944) { Name = "leaves", Hue = 2208 });
            m_Items.Add(new Static(6946) { Name = "leaves", Hue = 2208 });

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }

        public static Item GetRandomFishermanShipItem()
        {
            List<Item> m_Items = new List<Item>();

            m_Items.Add(new Static(3519)); //Fishing Pole
            m_Items.Add(new Static(3520)); //Fishing Pole
            m_Items.Add(new Static(3520)); //Fishing Pole
            m_Items.Add(new Static(2648)); //Bedroll
            m_Items.Add(new Static(2649)); //Bedroll
            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(5370)); //Rope
            m_Items.Add(new Static(7719)); //Oar
            m_Items.Add(new Static(7721)); //Oar            
            m_Items.Add(new Static(2508)); //Large Fish
            m_Items.Add(new Static(2511)); //Large Fish
            m_Items.Add(new Static(3530)); //Fishing Net
            m_Items.Add(new Static(3530)); //Fishing Net
            m_Items.Add(new Static(2448)); //Basket           
            m_Items.Add(new Static(2594)); //Lantern Lit          
            m_Items.Add(new Static(3514)); //Seaweed
            m_Items.Add(new Static(3515)); //Seaweed
            m_Items.Add(new Static(4039)); //Nautilus
            m_Items.Add(new Static(4042)); //Shells
            m_Items.Add(new Static(4587)); //Straw Pillow
            m_Items.Add(new Static(5344)); //Bucket  
            m_Items.Add(new Static(7701)); //Raw Fish
            m_Items.Add(new Static(7704)); //Raw Fish
            m_Items.Add(new Static(7707)); //Fish Heads
            m_Items.Add(new Static(3542)); //Small Fish
            m_Items.Add(new Static(3543)); //Small Fish            
            m_Items.Add(new Static(3545)); //Small Fish            
            m_Items.Add(new Static(3715)); //Empty Tub

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }

        public static Item GetRandomPirateShipItem()
        {
            List<Item> m_Items = new List<Item>();

            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(5370)); //Rope
            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(2648)); //Bedroll
            m_Items.Add(new Static(2649)); //Bedroll
            m_Items.Add(new Static(7719)); //Oar
            m_Items.Add(new Static(7721)); //Oar
            m_Items.Add(new Static(525)); //Acid Proof Rope
            m_Items.Add(new Static(3514)); //Seaweed
            m_Items.Add(new Static(3515)); //Seaweed
            m_Items.Add(new Static(2459)); //Liquor 1
            m_Items.Add(new Static(2460)); //Liquor 2
            m_Items.Add(new Static(2461)); //Liquor 3
            m_Items.Add(new Static(2462)); //Liquor 4
            m_Items.Add(new Static(2463)); //Liquor 1
            m_Items.Add(new Static(2464)); //Liquor 2
            m_Items.Add(new Static(2465)); //Liquor 3
            m_Items.Add(new Static(2466)); //Liquor 4
            m_Items.Add(new Static(2500)); //Liquor 1
            m_Items.Add(new Static(2501)); //Liquor 2
            m_Items.Add(new Static(2502)); //Liquor 3
            m_Items.Add(new Static(2503)); //Liquor 4
            m_Items.Add(new Static(2594)); //Lantern Lit
            m_Items.Add(new Static(2594)); //Lantern Lit
            m_Items.Add(new Static(3619)); //Bloody Water
            m_Items.Add(new Static(4002)); //Playing Cards
            m_Items.Add(new Static(4321)); //Barrel Hoops 
            m_Items.Add(new Static(5344)); //Bucket
            m_Items.Add(new Static(2451)); //Fruit Basket
            m_Items.Add(new Static(5922)); //Bananas
            m_Items.Add(new Static(5925)); //Coconuts
            m_Items.Add(new Static(5929)); //Lemons
            m_Items.Add(new Static(5931)); //Limes
            m_Items.Add(new Static(6262)); //Iron Wire
            m_Items.Add(new Static(4153)); //Flour
            m_Items.Add(new Static(4166)); //Flour
            m_Items.Add(new Static(2536)); //Dirty Pan
            m_Items.Add(new Static(3118)); //Debris
            m_Items.Add(new Static(3119)); //Debris
            m_Items.Add(new Static(3120)); //Debris
            m_Items.Add(new Static(3118)); //Debris
            m_Items.Add(new Static(3119)); //Debris
            m_Items.Add(new Static(3120)); //Debris
            m_Items.Add(new Static(4334)); //Garbage
            m_Items.Add(new Static(4336)); //Garbage
            m_Items.Add(new Static(5357)); //Garbage
            m_Items.Add(new Static(7681)); //Mud
            m_Items.Add(new Static(7683)); //Muddy Footprints
            m_Items.Add(new Static(7684)); //Muddy Footprints
            m_Items.Add(new Static(7819)); //Plucked Chicken
            m_Items.Add(new Static(3616)); //Bloody Bandage
            m_Items.Add(new Static(3700)); //Cannon Balls
            m_Items.Add(new Static(3700)); //Cannon Balls
            m_Items.Add(new Static(3715)); //Empty Tub
            m_Items.Add(new Static(5742)); //Whip
            m_Items.Add(new Static(5743)); //Whip
            m_Items.Add(new Static(8070)); //Glass of Liquor
            m_Items.Add(new Static(8072)); //Glass of Liquor
            m_Items.Add(new Static(8072)); //Glass of Liquor

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }

        public static Item GetRandomBritainNavalForcesShipItem()
        {
            List<Item> m_Items = new List<Item>();

            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(5370)); //Rope
            m_Items.Add(new Static(7719)); //Oar
            m_Items.Add(new Static(7721)); //Oar
            m_Items.Add(new Static(2594)); //Lantern Lit
            m_Items.Add(new Static(2594)); //Lantern Lit
            m_Items.Add(new Static(525)); //Acid Proof Rope
            m_Items.Add(new Static(7681)); //Mud
            m_Items.Add(new Static(6262)); //Iron Wire
            m_Items.Add(new Static(4153)); //Flour
            m_Items.Add(new Static(4166)); //Flour
            m_Items.Add(new Static(4321)); //Barrel Hoops 
            m_Items.Add(new Static(5344)); //Bucket
            m_Items.Add(new Static(2547)); //Pan
            m_Items.Add(new Static(3514)); //Seaweed
            m_Items.Add(new Static(3515)); //Seaweed
            m_Items.Add(new Static(3616)); //Bloody Bandage
            m_Items.Add(new Static(3715)); //Empty Tub
            m_Items.Add(new Static(4152)); //Wood Curls
            m_Items.Add(new Static(4148)); //Saw
            m_Items.Add(new Static(4142)); //Nails
            m_Items.Add(new Static(4139)); //Hammer
            m_Items.Add(new Static(5900)); //Boots
            m_Items.Add(new Static(7035)); //Metal Shield
            m_Items.Add(new Static(7034)); //Wooden Shield
            m_Items.Add(new Static(7125)); //Shafts
            m_Items.Add(new Static(7866)); //Tool Kit
            m_Items.Add(new Static(3904)); //Arrows
            m_Items.Add(new Static(3700)); //Cannon Balls
            m_Items.Add(new Static(3700)); //Cannon Balls

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }

        public static Item GetRandomUndeadShipItem()
        {
            List<Item> m_Items = new List<Item>();

            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))); //Blood
            m_Items.Add(new Static(16142)); //Coffin
            m_Items.Add(new Static(5743)); //Whip
            m_Items.Add(new Static(6657)); //Skeleton Shackled
            m_Items.Add(new Static(6659)); //Skeleton Shackled
            m_Items.Add(new Static(6661)); //Skeleton Shackled
            m_Items.Add(new Static(6665)); //Skeleton Shackled
            m_Items.Add(new Static(8700)); //Huge Skull Pile
            m_Items.Add(new Static(8707)); //Huge Skull
            m_Items.Add(new Static(6884)); //Small Skull
            m_Items.Add(new Static(6883)); //Small Skull
            m_Items.Add(new Static(6882)); //Small Skull
            m_Items.Add(new Static(6940)); //Spine
            m_Items.Add(new Static(6936)); //Ribcage
            m_Items.Add(new Static(6933)); //Pelvis
            m_Items.Add(new Static(3794)); //Bones
            m_Items.Add(new Static(3792)); //Skeleton
            m_Items.Add(new Static(3790)); //Skeleton
            m_Items.Add(new Static(3786)); //Skeleton
            m_Items.Add(new Static(6937)); //Bone Shards
            m_Items.Add(new Static(6927)); //Bone Shards
            m_Items.Add(new Static(6925)); //Bone Pile
            m_Items.Add(new Static(6922)); //Bone Pile
            m_Items.Add(new Static(6921)); //Bone Pile
            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(5370)); //Rope
            m_Items.Add(new Static(5368)); //Rope
            m_Items.Add(new Static(7719)); //Oar
            m_Items.Add(new Static(7721)); //Oar

            Item item = m_Items[Utility.RandomMinMax(0, m_Items.Count - 1)];

            item.Movable = false;

            return item;
        }
    }
}
