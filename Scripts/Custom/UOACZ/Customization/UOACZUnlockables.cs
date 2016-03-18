using System;
using Server;
using Server.Regions;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{   
    public enum UOACZUnlockableType
    {
        //Humans
        DeerMask, 
        BearMask, 
        HornedTribalMask,
        TribalMask, 
        OrcMask,
        OrcHelm,  

        FloppyHat,
        WideBrimHat,
        Cap,
        SkullCap,
        Bandana,
        TallStrawHat,
        StrawHat,
        WizardsHat,
        Bonnet,
        FeatheredHat,
        TricorneHat,
        JesterHat,

        Cloak,

        Shirt,
        FancyShirt,

        Robe,
        PlainDress,
        FancyDress,       

        BodySash,
        FullApron,
        Surcoat,
        Doublet,
        Tunic,
        JesterSuit,

        Skirt,
        Kilt,

        HalfApron,

        ShortPants,
        LongPants,

        Shoes,
        Boots,
        ThighBoots,
        Sandals,

        NormalDyeTub,          
        CovetousDyeTub, //2212
        DeceitDyeTub, //1908
        DespiseDyeTub, //2516
        DestardDyeTub, //1778
        HythlothDyeTub, //1769
        IceDyeTub, //2579
        ShameDyeTub, //1763
        WrongDyeTub, //2675
        FireDyeTub, //2635
        MilitiaDyeTub, //2405 
        PinkDyeTub, //2622
        TwilightDyeTub, //2587
        EarthlyDyeTub, //2688
        GoldDyeTub, //2125
        LuniteDyeTub, //2603
        FallonDyeTub, //1266  
        WhiteDyeTub, //2498
        SeaFoamDyeTub, //2655
        LimeDyeTub, //2527
        TrueRedDyeTub, //2117

        //Undead
        CovetousUndeadDye, //2212
        DeceitUndeadDye, //1908
        DespiseUndeadDye, //2516
        DestardUndeadDye, //1778
        HythlothUndeadDye, //1769
        IceUndeadDye, //2579
        ShameUndeadDye, //1763
        WrongUndeadDye, //2675
        FireUndeadDye, //2635
        MilitiaUndeadDye, //2405 
        PinkUndeadDye, //2622
        TwilightUndeadDye, //2587
        EarthlyUndeadDye, //2688
        GoldUndeadDye, //2125
        LuniteUndeadDye, //2603
        FallonUndeadDye, //1266  
        WhiteUndeadDye, //2498
        SeaFoamUndeadDye, //2655
        LimeUndeadDye, //2527
        TrueRedUndeadDye, //2117
    }

    public enum UOACZUnlockableCategory
    {
        //Human
        LayerHelm,
        LayerCloak,
        LayerShirt,
        LayerOuterTorso,       
        LayerMiddleTorso,
        LayerOuterLegs,
        LayerWaist,
        LayerPants,
        LayerShoes,
        DyeTub,

        //Undead
        UndeadDye
    }

    public static class UOACZUnlockables
    {
        public static UOACZUnlockableType GetRandomUnlockableType()
        {
            UOACZUnlockableType unlockable = UOACZUnlockableType.DeerMask;

            double randomResult = Utility.RandomDouble();
            
            if (randomResult <= .20)
            {
                switch (Utility.RandomMinMax(1, 14))
                {
                    case 1: unlockable = UOACZUnlockableType.FloppyHat; break;
                    case 2: unlockable = UOACZUnlockableType.WideBrimHat; break;
                    case 3: unlockable = UOACZUnlockableType.Cap; break;
                    case 4: unlockable = UOACZUnlockableType.SkullCap; break;
                    case 5: unlockable = UOACZUnlockableType.Bandana; break;
                    case 6: unlockable = UOACZUnlockableType.TallStrawHat; break;
                    case 7: unlockable = UOACZUnlockableType.StrawHat; break;                    
                    case 8: unlockable = UOACZUnlockableType.Bonnet; break;
                    case 9: unlockable = UOACZUnlockableType.FeatheredHat; break;
                    case 10: unlockable = UOACZUnlockableType.TricorneHat; break;
                    case 11: unlockable = UOACZUnlockableType.JesterHat; break;

                    case 12: unlockable = UOACZUnlockableType.WizardsHat; break;
                    case 13: unlockable = UOACZUnlockableType.WizardsHat; break;
                    case 14: unlockable = UOACZUnlockableType.WizardsHat; break;
                }
            }

            else if (randomResult <= .40)
            {
                switch (Utility.RandomMinMax(1, 6))
                {
                    case 1: unlockable = UOACZUnlockableType.Cloak; break;
                    case 2: unlockable = UOACZUnlockableType.HalfApron; break;
                    case 3: unlockable = UOACZUnlockableType.Skirt; break;
                    case 4: unlockable = UOACZUnlockableType.Kilt; break;
                    case 5: unlockable = UOACZUnlockableType.BodySash; break;
                    case 6: unlockable = UOACZUnlockableType.FullApron; break;
                }
            }            

            else if (randomResult <= .55)
            {
                switch (Utility.RandomMinMax(1, 7))
                {
                    case 1: unlockable = UOACZUnlockableType.Shirt; break;
                    case 2: unlockable = UOACZUnlockableType.FancyShirt; break;
                    case 3: unlockable = UOACZUnlockableType.ShortPants; break;
                    case 4: unlockable = UOACZUnlockableType.LongPants; break;
                    case 5: unlockable = UOACZUnlockableType.Doublet; break;
                    case 6: unlockable = UOACZUnlockableType.Tunic; break;
                    case 7: unlockable = UOACZUnlockableType.JesterSuit; break;
                }                  
            }

            else if (randomResult <= .65)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: unlockable = UOACZUnlockableType.Robe; break;
                    case 2: unlockable = UOACZUnlockableType.PlainDress; break;
                    case 3: unlockable = UOACZUnlockableType.FancyDress; break;
                }
            }

            else if (randomResult <= .75)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: unlockable = UOACZUnlockableType.Shoes; break;
                    case 2: unlockable = UOACZUnlockableType.Boots; break;
                    case 3: unlockable = UOACZUnlockableType.ThighBoots; break;
                    case 4: unlockable = UOACZUnlockableType.Sandals; break;
                }
            }              

            else if (randomResult <= .90)
            {
                if (Utility.RandomDouble() <= .5)                
                    unlockable = UOACZUnlockableType.NormalDyeTub;                

                else
                {
                    double dyeResult = Utility.RandomDouble();

                    if (dyeResult <= .60)
                    {
                        switch (Utility.RandomMinMax(1, 7))
                        {
                            case 1: unlockable = UOACZUnlockableType.MilitiaUndeadDye; break;
                            case 2: unlockable = UOACZUnlockableType.PinkUndeadDye; break;
                            case 3: unlockable = UOACZUnlockableType.LimeUndeadDye; break;
                            case 4: unlockable = UOACZUnlockableType.SeaFoamUndeadDye; break;
                            case 5: unlockable = UOACZUnlockableType.EarthlyUndeadDye; break;
                            case 6: unlockable = UOACZUnlockableType.GoldUndeadDye; break;
                            case 7: unlockable = UOACZUnlockableType.TrueRedUndeadDye; break;
                        }
                    }

                    else if (dyeResult <= .90)
                    {
                        switch (Utility.RandomMinMax(1, 9))
                        {
                            case 1: unlockable = UOACZUnlockableType.CovetousUndeadDye; break;
                            case 2: unlockable = UOACZUnlockableType.DeceitUndeadDye; break;
                            case 3: unlockable = UOACZUnlockableType.DespiseUndeadDye; break;
                            case 4: unlockable = UOACZUnlockableType.DestardUndeadDye; break;
                            case 5: unlockable = UOACZUnlockableType.HythlothUndeadDye; break;
                            case 6: unlockable = UOACZUnlockableType.IceUndeadDye; break;
                            case 7: unlockable = UOACZUnlockableType.ShameUndeadDye; break;
                            case 8: unlockable = UOACZUnlockableType.WrongUndeadDye; break;
                            case 9: unlockable = UOACZUnlockableType.FireUndeadDye; break;
                        }
                    }

                    else
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1: unlockable = UOACZUnlockableType.TwilightUndeadDye; break;
                            case 2: unlockable = UOACZUnlockableType.LuniteUndeadDye; break;
                            case 3: unlockable = UOACZUnlockableType.FallonUndeadDye; break;
                            case 4: unlockable = UOACZUnlockableType.WhiteUndeadDye; break;
                        }
                    }
                }
            }

            else if (randomResult <= .95)
            {
                switch (Utility.RandomMinMax(1, 6))
                {
                    case 1: unlockable = UOACZUnlockableType.DeerMask; break;
                    case 2: unlockable = UOACZUnlockableType.BearMask; break;
                    case 3: unlockable = UOACZUnlockableType.HornedTribalMask; break;
                    case 4: unlockable = UOACZUnlockableType.TribalMask; break;
                    case 5: unlockable = UOACZUnlockableType.OrcMask; break;
                    case 6: unlockable = UOACZUnlockableType.OrcHelm; break;
                }
            }            

            else
            {
                double dyeResult = Utility.RandomDouble();
                
                if (dyeResult <= .60)
                {
                    switch (Utility.RandomMinMax(1, 7))
                    {
                        case 1: unlockable = UOACZUnlockableType.MilitiaDyeTub; break;
                        case 2: unlockable = UOACZUnlockableType.PinkDyeTub; break;
                        case 3: unlockable = UOACZUnlockableType.LimeDyeTub; break;
                        case 4: unlockable = UOACZUnlockableType.SeaFoamDyeTub; break;
                        case 5: unlockable = UOACZUnlockableType.EarthlyDyeTub; break;
                        case 6: unlockable = UOACZUnlockableType.GoldDyeTub; break;
                        case 7: unlockable = UOACZUnlockableType.TrueRedDyeTub; break;
                    }
                }

                else if (dyeResult <= .90)
                {
                    switch (Utility.RandomMinMax(1, 9))
                    {
                        case 1: unlockable = UOACZUnlockableType.CovetousDyeTub; break;
                        case 2: unlockable = UOACZUnlockableType.DeceitDyeTub; break;
                        case 3: unlockable = UOACZUnlockableType.DespiseDyeTub; break;
                        case 4: unlockable = UOACZUnlockableType.DestardDyeTub; break;
                        case 5: unlockable = UOACZUnlockableType.HythlothDyeTub; break;
                        case 6: unlockable = UOACZUnlockableType.IceDyeTub; break;
                        case 7: unlockable = UOACZUnlockableType.ShameDyeTub; break;
                        case 8: unlockable = UOACZUnlockableType.WrongDyeTub; break;
                        case 9: unlockable = UOACZUnlockableType.FireDyeTub; break;
                    }
                }               

                else
                {
                    switch (Utility.RandomMinMax(1, 4))
                    {
                        case 1: unlockable = UOACZUnlockableType.TwilightDyeTub; break;
                        case 2: unlockable = UOACZUnlockableType.LuniteDyeTub; break;
                        case 3: unlockable = UOACZUnlockableType.FallonDyeTub; break;
                        case 4: unlockable = UOACZUnlockableType.WhiteDyeTub; break;
                    }
                }
            }

            return unlockable;
        }

        public static string GetCategoryName(UOACZUnlockableCategory category)
        {
            string categoryName = "";

            switch (category)
            {
                case UOACZUnlockableCategory.LayerHelm: categoryName = "Helm"; break;
                case UOACZUnlockableCategory.LayerCloak: categoryName = "Cloak"; break;
                case UOACZUnlockableCategory.LayerShirt: categoryName = "Shirt"; break;
                case UOACZUnlockableCategory.LayerOuterTorso: categoryName = "Outer Torso"; break;
                case UOACZUnlockableCategory.LayerMiddleTorso: categoryName = "Middle Torso"; break;
                case UOACZUnlockableCategory.LayerOuterLegs: categoryName = "Outer Legs"; break;
                case UOACZUnlockableCategory.LayerWaist: categoryName = "Waist"; break;
                case UOACZUnlockableCategory.LayerPants: categoryName = "Pants"; break;
                case UOACZUnlockableCategory.LayerShoes: categoryName = "Shoes"; break;
                case UOACZUnlockableCategory.DyeTub: categoryName = "Dye Tub"; break;

                case UOACZUnlockableCategory.UndeadDye: categoryName = "Undead Form Dye"; break;
            }

            return categoryName;
        }

        public static UOACZUnlockableDetail GetUnlockableDetail(UOACZUnlockableType unlockableType)
        {
            UOACZUnlockableDetail unlockableDetail = new UOACZUnlockableDetail();

            switch (unlockableType)
            {
                #region Human Unlockables

                //Layer Helm
                case UOACZUnlockableType.DeerMask:
                    unlockableDetail.Name = "Blessed Deer Mask";
                    unlockableDetail.m_Items.Add(typeof(DeerMask));
                    unlockableDetail.Description = new string[] { "Blessed Deer Mask" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5447;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0; 
                break;

                case UOACZUnlockableType.BearMask:
                    unlockableDetail.Name = "Blessed Bear Mask";
                    unlockableDetail.m_Items.Add(typeof(BearMask));
                    unlockableDetail.Description = new string[] { "Blessed Bear Mask" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5445;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0; 
                break;

                case UOACZUnlockableType.HornedTribalMask:
                    unlockableDetail.Name = "Blessed Horned Tribal Mask";
                    unlockableDetail.m_Items.Add(typeof(HornedTribalMask));
                    unlockableDetail.Description = new string[] { "Blessed Horned Tribal Mask" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5449;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.TribalMask:
                    unlockableDetail.Name = "Blessed Tribal Mask";
                    unlockableDetail.m_Items.Add(typeof(TribalMask));
                    unlockableDetail.Description = new string[] { "Blessed Tribal Mask" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5451;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 5; 
                break;

                case UOACZUnlockableType.OrcMask:
                    unlockableDetail.Name = "Blessed Orc Mask";
                    unlockableDetail.m_Items.Add(typeof(OrcMask));
                    unlockableDetail.Description = new string[] { "Blessed Orc Mask" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5147;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 5; 
                break;

                case UOACZUnlockableType.OrcHelm:
                    unlockableDetail.Name = "Blessed Orc Helm";
                    unlockableDetail.m_Items.Add(typeof(OrcHelm));
                    unlockableDetail.Description = new string[] { "Blessed Orc Helm" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 7947;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 5; 
                break;

                case UOACZUnlockableType.FloppyHat:
                    unlockableDetail.Name = "Blessed Floppy Hat";
                    unlockableDetail.m_Items.Add(typeof(FloppyHat));
                    unlockableDetail.Description = new string[] { "Blessed Floppy Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5907;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.WideBrimHat:
                    unlockableDetail.Name = "Blessed Wide Brim Hat";
                    unlockableDetail.m_Items.Add(typeof(WideBrimHat));
                    unlockableDetail.Description = new string[] { "Blessed Wide Brim Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5908;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.Cap:
                    unlockableDetail.Name = "Blessed Cap";
                    unlockableDetail.m_Items.Add(typeof(Cap));
                    unlockableDetail.Description = new string[] { "Blessed Cap" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5909;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.SkullCap:
                    unlockableDetail.Name = "Blessed Skull Cap";
                    unlockableDetail.m_Items.Add(typeof(SkullCap));
                    unlockableDetail.Description = new string[] { "Blessed Skull Cap" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5443;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 4;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.Bandana:
                    unlockableDetail.Name = "Blessed Bandana";
                    unlockableDetail.m_Items.Add(typeof(Bandana));
                    unlockableDetail.Description = new string[] { "Blessed Bandana" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5440;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.TallStrawHat:
                    unlockableDetail.Name = "Blessed Tall Straw Hat";
                    unlockableDetail.m_Items.Add(typeof(TallStrawHat));
                    unlockableDetail.Description = new string[] { "Blessed Tall Straw Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5910;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 3;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.StrawHat:
                    unlockableDetail.Name = "Blessed Straw Hat";
                    unlockableDetail.m_Items.Add(typeof(StrawHat));
                    unlockableDetail.Description = new string[] { "Blessed Straw Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5911;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 3;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.WizardsHat:
                    unlockableDetail.Name = "Blessed Wizard's Hat";
                    unlockableDetail.m_Items.Add(typeof(WizardsHat));
                    unlockableDetail.Description = new string[] { "Blessed Wizard's Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5912;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.Bonnet:
                    unlockableDetail.Name = "Blessed Bonnet";
                    unlockableDetail.m_Items.Add(typeof(Bonnet));
                    unlockableDetail.Description = new string[] { "Blessed Bonnet" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5913;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 3;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.FeatheredHat:
                    unlockableDetail.Name = "Blessed Feathered Hat";
                    unlockableDetail.m_Items.Add(typeof(FeatheredHat));
                    unlockableDetail.Description = new string[] { "Blessed Feathered Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5914;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 3;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.TricorneHat:
                    unlockableDetail.Name = "Blessed Tricorne Hat";
                    unlockableDetail.m_Items.Add(typeof(FeatheredHat));
                    unlockableDetail.Description = new string[] { "Blessed Tricorne Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5915;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.JesterHat:
                    unlockableDetail.Name = "Blessed Jester Hat";
                    unlockableDetail.m_Items.Add(typeof(JesterHat));
                    unlockableDetail.Description = new string[] { "Blessed Jester Hat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerHelm;
                    unlockableDetail.ItemId = 5916;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 5;
                break;

                //Layer Cloak
                case UOACZUnlockableType.Cloak:
                    unlockableDetail.Name = "Blessed Cloak";
                    unlockableDetail.m_Items.Add(typeof(Cloak));
                    unlockableDetail.Description = new string[] { "Blessed Cloak" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerCloak;
                    unlockableDetail.ItemId = 5397;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -10;
                    unlockableDetail.OffsetY = -3;
                break;

                //Layer Shirt
                case UOACZUnlockableType.Shirt:
                    unlockableDetail.Name = "Blessed Shirt";
                    unlockableDetail.m_Items.Add(typeof(Shirt));
                    unlockableDetail.Description = new string[] { "Blessed Shirt" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShirt;
                    unlockableDetail.ItemId = 5399;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -1;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.FancyShirt:
                    unlockableDetail.Name = "Blessed Fancy Shirt";
                    unlockableDetail.m_Items.Add(typeof(FancyShirt));
                    unlockableDetail.Description = new string[] { "Blessed Fancy Shirt" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShirt;
                    unlockableDetail.ItemId = 7933;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 3;
                break;

                //Layer Outer Torso
                case UOACZUnlockableType.Robe:
                    unlockableDetail.Name = "Blessed Robe";
                    unlockableDetail.m_Items.Add(typeof(Robe));
                    unlockableDetail.Description = new string[] { "Blessed Robe" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerOuterTorso;
                    unlockableDetail.ItemId = 7939;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -8;
                    unlockableDetail.OffsetY = -2;
                break;

                case UOACZUnlockableType.PlainDress:
                    unlockableDetail.Name = "Blessed Plain Dress";
                    unlockableDetail.m_Items.Add(typeof(PlainDress));
                    unlockableDetail.Description = new string[] { "Blessed Plain Dress" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerOuterTorso;
                    unlockableDetail.ItemId = 7937;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -3;
                    unlockableDetail.OffsetY = -2;
                break;

                case UOACZUnlockableType.FancyDress:
                    unlockableDetail.Name = "Blessed Fancy Dress";
                    unlockableDetail.m_Items.Add(typeof(PlainDress));
                    unlockableDetail.Description = new string[] { "Blessed Fancy Dress" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerOuterTorso;
                    unlockableDetail.ItemId = 7936;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -10;
                    unlockableDetail.OffsetY = -7;
                break;

                //Layer Middle Torso
                case UOACZUnlockableType.BodySash:
                    unlockableDetail.Name = "Blessed Body Sash";
                    unlockableDetail.m_Items.Add(typeof(BodySash));
                    unlockableDetail.Description = new string[] { "Blessed Body Sash" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 5441;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 4;
                    unlockableDetail.OffsetY = 5;
                break;

                case UOACZUnlockableType.FullApron:
                    unlockableDetail.Name = "Blessed Full Apron";
                    unlockableDetail.m_Items.Add(typeof(FullApron));
                    unlockableDetail.Description = new string[] { "Blessed Full Apron" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 5437;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 3;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.Surcoat:
                    unlockableDetail.Name = "Blessed Surcoat";
                    unlockableDetail.m_Items.Add(typeof(Surcoat));
                    unlockableDetail.Description = new string[] { "Blessed Surcoat" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 8189;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -8;
                    unlockableDetail.OffsetY = -3;
                break;

                case UOACZUnlockableType.Doublet:
                    unlockableDetail.Name = "Blessed Doublet";
                    unlockableDetail.m_Items.Add(typeof(Doublet));
                    unlockableDetail.Description = new string[] { "Blessed Doublet" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 8059;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -13;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.Tunic:
                    unlockableDetail.Name = "Blessed Tunic";
                    unlockableDetail.m_Items.Add(typeof(Tunic));
                    unlockableDetail.Description = new string[] { "Blessed Tunic" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 8097;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -10;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.JesterSuit:
                    unlockableDetail.Name = "Blessed Jester Suit";
                    unlockableDetail.m_Items.Add(typeof(JesterSuit));
                    unlockableDetail.Description = new string[] { "Blessed Jester Suit" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerMiddleTorso;
                    unlockableDetail.ItemId = 8095;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = -10;
                    unlockableDetail.OffsetY = 0;
                break;

                //Layer Outer Legs
                case UOACZUnlockableType.Skirt:
                    unlockableDetail.Name = "Blessed Skirt";
                    unlockableDetail.m_Items.Add(typeof(Skirt));
                    unlockableDetail.Description = new string[] { "Blessed Skirt" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerOuterLegs;
                    unlockableDetail.ItemId = 5398;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.Kilt:
                    unlockableDetail.Name = "Blessed Kilt";
                    unlockableDetail.m_Items.Add(typeof(Kilt));
                    unlockableDetail.Description = new string[] { "Blessed Kilt" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerOuterLegs;
                    unlockableDetail.ItemId = 5431;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 0;
                break;

                //Layer Waist
                case UOACZUnlockableType.HalfApron:
                    unlockableDetail.Name = "Blessed Half Apron";
                    unlockableDetail.m_Items.Add(typeof(HalfApron));
                    unlockableDetail.Description = new string[] { "Blessed Half Apron" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerWaist;
                    unlockableDetail.ItemId = 5435;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 0;
                break;

                //Layer Pants
                case UOACZUnlockableType.ShortPants:
                    unlockableDetail.Name = "Blessed Short Pants";
                    unlockableDetail.m_Items.Add(typeof(ShortPants));
                    unlockableDetail.Description = new string[] { "Blessed Short Pants" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerPants;
                    unlockableDetail.ItemId = 5422;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.LongPants:
                    unlockableDetail.Name = "Blessed Long Pants";
                    unlockableDetail.m_Items.Add(typeof(LongPants));
                    unlockableDetail.Description = new string[] { "Blessed Long Pants" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerPants;
                    unlockableDetail.ItemId = 5433;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 2;
                    unlockableDetail.OffsetY = 0;
                break;

                //Layer Shoes
                case UOACZUnlockableType.Shoes:
                    unlockableDetail.Name = "Blessed Shoes";
                    unlockableDetail.m_Items.Add(typeof(Shoes));
                    unlockableDetail.Description = new string[] { "Blessed Shoes" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShoes;
                    unlockableDetail.ItemId = 5903;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.Boots:
                    unlockableDetail.Name = "Blessed Boots";
                    unlockableDetail.m_Items.Add(typeof(Boots));
                    unlockableDetail.Description = new string[] { "Blessed Boots" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShoes;
                    unlockableDetail.ItemId = 5899;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.ThighBoots:
                    unlockableDetail.Name = "Blessed Thigh Boots";
                    unlockableDetail.m_Items.Add(typeof(ThighBoots));
                    unlockableDetail.Description = new string[] { "Blessed Thigh Boots" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShoes;
                    unlockableDetail.ItemId = 5905;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.Sandals:
                    unlockableDetail.Name = "Blessed Sandals";
                    unlockableDetail.m_Items.Add(typeof(Sandals));
                    unlockableDetail.Description = new string[] { "Blessed Sandals" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.LayerShoes;
                    unlockableDetail.ItemId = 5901;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 5;
                    unlockableDetail.OffsetY = 0;
                break;

                //DyeTub
                case UOACZUnlockableType.NormalDyeTub:
                    unlockableDetail.Name = "Normal Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZDyes));
                    unlockableDetail.m_Items.Add(typeof(UOACZDyeTub));
                    unlockableDetail.Description = new string[] { "Normal Dye Tub with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 0;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0; 
                break;

                case UOACZUnlockableType.CovetousDyeTub:
                    unlockableDetail.Name = "Covetous Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZCovetousDyeTub));
                    unlockableDetail.Description = new string[] { "Covetous Dye Tub (Hue 2212) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2122;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.DeceitDyeTub:
                    unlockableDetail.Name = "Deceit Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZDeceitDyeTub));
                    unlockableDetail.Description = new string[] { "Deceit Dye Tub (Hue 1908) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 1908;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.DespiseDyeTub:
                    unlockableDetail.Name = "Despise Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZDespiseDyeTub));
                    unlockableDetail.Description = new string[] { "Despise Dye Tub (Hue 2516) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2516;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.DestardDyeTub:
                    unlockableDetail.Name = "Destard Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZDestardDyeTub));
                    unlockableDetail.Description = new string[] { "Destard Dye Tub (Hue 1778) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 1778;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.HythlothDyeTub:
                    unlockableDetail.Name = "Hythloth Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZHythlothDyeTub));
                    unlockableDetail.Description = new string[] { "Hythloth Dye Tub (Hue 1769) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 1769;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.IceDyeTub:
                    unlockableDetail.Name = "Ice Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZIceDyeTub));
                    unlockableDetail.Description = new string[] { "Ice Dye Tub (Hue 2579) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2579;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.ShameDyeTub:
                    unlockableDetail.Name = "Shame Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZShameDyeTub));
                    unlockableDetail.Description = new string[] { "Shame Dye Tub (Hue 1763) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 1763;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.WrongDyeTub:
                    unlockableDetail.Name = "Wrong Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZWrongDyeTub));
                    unlockableDetail.Description = new string[] { "Wrong Dye Tub (Hue 2675) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2675;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.FireDyeTub:
                    unlockableDetail.Name = "Fire Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZFireDyeTub));
                    unlockableDetail.Description = new string[] { "Fire Dye Tub (Hue 2635) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2635;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.MilitiaDyeTub:
                    unlockableDetail.Name = "Militia Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZMilitiaDyeTub));
                    unlockableDetail.Description = new string[] { "Militia Dye Tub (Hue 2405) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2405;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.PinkDyeTub:
                    unlockableDetail.Name = "Pink Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZPinkDyeTub));
                    unlockableDetail.Description = new string[] { "Pink Dye Tub (Hue 2622) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2622;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.TwilightDyeTub:
                    unlockableDetail.Name = "Twilight Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZTwilightDyeTub));
                    unlockableDetail.Description = new string[] { "Twilight Dye Tub (Hue 2587) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2587;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.EarthlyDyeTub:
                    unlockableDetail.Name = "Earthly Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZEarthlyDyeTub));
                    unlockableDetail.Description = new string[] { "Earthly Dye Tub (Hue 2688) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2688;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.GoldDyeTub:
                    unlockableDetail.Name = "Gold Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZGoldDyeTub));
                    unlockableDetail.Description = new string[] { "Gold Dye Tub (Hue 2125) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2125;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.LuniteDyeTub:
                    unlockableDetail.Name = "Lunite Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZLuniteDyeTub));
                    unlockableDetail.Description = new string[] { "Lunite Dye Tub (Hue 2603) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2603;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.FallonDyeTub:
                    unlockableDetail.Name = "Fallon Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZFallonDyeTub));
                    unlockableDetail.Description = new string[] { "Fallon Dye Tub (Hue 1266) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 1266;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.WhiteDyeTub:
                    unlockableDetail.Name = "White Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZWhiteDyeTub));
                    unlockableDetail.Description = new string[] { "White Dye Tub (Hue 2498) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2498;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.SeaFoamDyeTub:
                    unlockableDetail.Name = "Seafoam Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZSeafoamDyeTub));
                    unlockableDetail.Description = new string[] { "Seafoam Dye Tub (Hue 2655) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2655;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.LimeDyeTub:
                    unlockableDetail.Name = "Lime Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZLimeDyeTub));
                    unlockableDetail.Description = new string[] { "Lime Dye Tub (Hue 2527) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2527;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                case UOACZUnlockableType.TrueRedDyeTub:
                    unlockableDetail.Name = "True Red Dye Tub";
                    unlockableDetail.m_Items.Add(typeof(UOACZTrueRedDyeTub));
                    unlockableDetail.Description = new string[] { "True Red Dye Tub (Hue 2117) with " + UOACZDyeTub.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.DyeTub;
                    unlockableDetail.ItemId = 4011;
                    unlockableDetail.ItemHue = 2117;
                    unlockableDetail.OffsetX = 0;
                    unlockableDetail.OffsetY = 0;
                break;

                #endregion

                #region Undead Unlockables
              
                case UOACZUnlockableType.CovetousUndeadDye:
                    unlockableDetail.Name = "Covetous Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZCovetousUndeadDye));
                    unlockableDetail.Description = new string[] { "Covetous Undead Dye (Hue 2212) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2122;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.DeceitUndeadDye:
                    unlockableDetail.Name = "Deceit Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZDeceitUndeadDye));
                    unlockableDetail.Description = new string[] { "Deceit Undead Dye (Hue 1908) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 1908;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.DespiseUndeadDye:
                    unlockableDetail.Name = "Despise Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZDespiseUndeadDye));
                    unlockableDetail.Description = new string[] { "Despise Undead Dye (Hue 2516) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2516;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.DestardUndeadDye:
                    unlockableDetail.Name = "Destard Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZDestardUndeadDye));
                    unlockableDetail.Description = new string[] { "Destard Undead Dye (Hue 1778) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 1778;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.HythlothUndeadDye:
                    unlockableDetail.Name = "Hythloth Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZHythlothUndeadDye));
                    unlockableDetail.Description = new string[] { "Hythloth Undead Dye (Hue 1769) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 1769;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.IceUndeadDye:
                    unlockableDetail.Name = "Ice Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZIceUndeadDye));
                    unlockableDetail.Description = new string[] { "Ice Undead Dye (Hue 2579) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2579;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.ShameUndeadDye:
                    unlockableDetail.Name = "Shame Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZShameUndeadDye));
                    unlockableDetail.Description = new string[] { "Shame Undead Dye (Hue 1763) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 1763;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.WrongUndeadDye:
                    unlockableDetail.Name = "Wrong Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZWrongUndeadDye));
                    unlockableDetail.Description = new string[] { "Wrong Undead Dye (Hue 2675) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2675;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.FireUndeadDye:
                    unlockableDetail.Name = "Fire Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZFireUndeadDye));
                    unlockableDetail.Description = new string[] { "Fire Undead Dye (Hue 2635) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2635;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.MilitiaUndeadDye:
                    unlockableDetail.Name = "Militia Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZMilitiaUndeadDye));
                    unlockableDetail.Description = new string[] { "Militia Undead Dye (Hue 2405) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2405;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.PinkUndeadDye:
                    unlockableDetail.Name = "Pink Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZPinkUndeadDye));
                    unlockableDetail.Description = new string[] { "Pink Undead Dye (Hue 2622) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2622;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.TwilightUndeadDye:
                    unlockableDetail.Name = "Twilight Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZTwilightUndeadDye));
                    unlockableDetail.Description = new string[] { "Twilight Undead Dye (Hue 2587) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2587;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.EarthlyUndeadDye:
                    unlockableDetail.Name = "Earthly Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZEarthlyUndeadDye));
                    unlockableDetail.Description = new string[] { "Earthly Undead Dye (Hue 2688) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2688;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.GoldUndeadDye:
                    unlockableDetail.Name = "Gold Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZGoldUndeadDye));
                    unlockableDetail.Description = new string[] { "Gold Undead Dye (Hue 2125) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2125;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.LuniteUndeadDye:
                    unlockableDetail.Name = "Lunite Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZLuniteUndeadDye));
                    unlockableDetail.Description = new string[] { "Lunite Undead Dye (Hue 2603) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2603;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.FallonUndeadDye:
                    unlockableDetail.Name = "Fallon Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZFallonUndeadDye));
                    unlockableDetail.Description = new string[] { "Fallon Undead Dye (Hue 1266) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 1266;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.WhiteUndeadDye:
                    unlockableDetail.Name = "White Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZWhiteUndeadDye));
                    unlockableDetail.Description = new string[] { "White Undead Dye (Hue 2498) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2498;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.SeaFoamUndeadDye:
                    unlockableDetail.Name = "Seafoam Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZSeafoamUndeadDye));
                    unlockableDetail.Description = new string[] { "Seafoam Undead Dye (Hue 2655) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2655;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.LimeUndeadDye:
                    unlockableDetail.Name = "Lime Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZLimeUndeadDye));
                    unlockableDetail.Description = new string[] { "Lime Undead Dye (Hue 2527) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2527;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                case UOACZUnlockableType.TrueRedUndeadDye:
                    unlockableDetail.Name = "True Red Undead Dye";
                    unlockableDetail.m_Items.Add(typeof(UOACZTrueRedUndeadDye));
                    unlockableDetail.Description = new string[] { "True Red Undead Dye (Hue 2117) with " + UOACZUndeadDye.StartingCharges.ToString() + " Charges" };
                    unlockableDetail.UnlockableCategory = UOACZUnlockableCategory.UndeadDye;
                    unlockableDetail.ItemId = 3622;
                    unlockableDetail.ItemHue = 2117;
                    unlockableDetail.OffsetX = 8;
                    unlockableDetail.OffsetY = 3;
                break;

                #endregion
            }

            return unlockableDetail;
        }

        public static UOACZUnlockableDetailEntry GetUnlockableDetailEntry(PlayerMobile player, UOACZUnlockableType unlockableType)
        {
            if (player == null)
                return null;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            foreach (UOACZUnlockableDetailEntry detailEntry in player.m_UOACZAccountEntry.m_Unlockables)
            {
                if (detailEntry.m_UnlockableType == unlockableType)
                    return detailEntry;
            }

            return null;
        }
    }

    public class UOACZUnlockableDetail
    {
        public string Name = "Unlockable Detail";
        public string[] Description = new string[] { };
        public List<Type> m_Items = new List<Type>();
        public UOACZUnlockableCategory UnlockableCategory = UOACZUnlockableCategory.LayerHelm;      
        public int ItemId = 4011;
        public int ItemHue = 0;
        public int OffsetX = 0;
        public int OffsetY = 0;          
    }

    public class UOACZUnlockableDetailEntry
    {
        public UOACZUnlockableType m_UnlockableType = UOACZUnlockableType.DeerMask;
        public bool m_Unlocked = false;
        public bool m_Active = false;

        public UOACZUnlockableDetailEntry(UOACZUnlockableType unlockableType, bool unlocked, bool active)
        {
            m_UnlockableType = unlockableType;
            m_Unlocked = unlocked;
            m_Active = active;
        }
    }    
}