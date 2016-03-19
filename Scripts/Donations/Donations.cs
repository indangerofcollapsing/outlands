using System;
using System.Collections.Generic;
using Server;
using Server.Items;

using Server.Gumps;
using System.Collections;
using System.Collections.Specialized;
using Server.Commands;
using Server.Items.MusicBox;
using Server.Mobiles;
using System.IO;
using Server.Items.Misc;
using Server.Custom.Donations.Items;

namespace Server.Custom.Donations
{
    public static class Donations
    {
        // these items will be blessed and non transferrable
        public static Type[] DonationItemTypes = new Type[]
        { 
            // lottery
            typeof(DragonLotteryTicket),
            // masks
            typeof(MaskDye),
            // deeds
            typeof(GenderChangeDeed), typeof(NameChangeDeed), typeof(HairRestylingDeed), typeof(BeardRestylingDeed), typeof(LockDownExpansionDeed), typeof(SecureExpansionDeed),
            typeof(ItemRenameDeed),
            // pets
            typeof(PetBattleBurrowBeetleToken),
        };


        public static OrderedDictionary DragonTable = new OrderedDictionary()
        {
            { "Masks",
            new List<DonationItem>()
            {
                new DonationItem("Bear Mask", "Does not provide any armor. <i>Nonblessed</i>", 5445, 0, 300, false, typeof(BearMask)),
                new DonationItem("Deer Mask", "Does not provide any armor. <i>Nonblessed</i>", 5447, 0, 300, false, typeof(DeerMask)),
                new DonationItem("Tribal Mask", "Does not provide any armor. <i>Nonblessed</i>", 5451, 0, 300, false, typeof(TribalMask)),
                new DonationItem("Horned Tribal Mask", "Does not provide any armor. <i>Nonblessed</i>", 5449, 0, 300, false, typeof(HornedTribalMask)),
                new DonationItem("Orc Mask", "Does not provide any armor. <i>Nonblessed</i>", 0x141B, 0, 300, false, typeof(OrcMask)),

                new DonationItem("Britain Mask Dye", "Love Britain? Show it off with this mask dye! Hue 2125 <i>Nonblessed</i>", 0xE26, 2125, 2000, false, typeof(MaskDye)),
                new DonationItem("Cove Mask Dye", "Love Cove? Show it off with this mask dye! Hue 1109 <i>Nonblessed</i>", 0xE26, 1109, 2000, false, typeof(MaskDye)),
                new DonationItem("Jhelom Mask Dye", "Love Jhelom? Show it off with this mask dye! Hue 1761 <i>Nonblessed</i>", 0xE26, 1761, 2000, false, typeof(MaskDye)),
                new DonationItem("Magincia Mask Dye", "Love Magincia? Show it off with this mask dye! Hue 1158 <i>Nonblessed</i>", 0xE26, 1158, 2000, false, typeof(MaskDye)),
                new DonationItem("Minoc Mask Dye", "Love Minoc? Show it off with this mask dye! Hue 2123 <i>Nonblessed</i>", 0xE26, 2123, 2000, false, typeof(MaskDye)),
                new DonationItem("Moonglow Mask Dye", "Love Moonglow? Show it off with this mask dye! Hue 1278 <i>Nonblessed</i>", 0xE26, 1278, 2000, false, typeof(MaskDye)),
                new DonationItem("Nujel'm Mask Dye", "Love Nujel'm? Show it off with this mask dye! Hue 2118 <i>Nonblessed</i>", 0xE26, 2118, 2000, false, typeof(MaskDye)),
                new DonationItem("Ocllo Mask Dye", "Love Ocllo? Show it off with this mask dye! Hue 1156 <i>Nonblessed</i>", 0xE26, 1156, 2000, false, typeof(MaskDye)),
                new DonationItem("Serpent's Hold Mask Dye", "Love Serpent's Hold? Show it off with this mask dye! Hue 1157 <i>Nonblessed</i>", 0xE26, 1157, 2000, false, typeof(MaskDye)),
                new DonationItem("Skara Brae Mask Dye", "Love Skara Brae? Show it off with this mask dye! Hue 1196 <i>Nonblessed</i>", 0xE26, 1196, 2000, false, typeof(MaskDye)),
                new DonationItem("Trinsic Mask Dye", "Love Trinsic? Show it off with this mask dye! Hue 1151 <i>Nonblessed</i>", 0xE26, 1151, 2000, false, typeof(MaskDye)),
                new DonationItem("Vesper Mask Dye", "Love Vesper? Show it off with this mask dye! Hue 1366 <i>Nonblessed</i>", 0xE26, 1366, 2000, false, typeof(MaskDye)),
                new DonationItem("Yew Mask Dye", "Love Yew? Show it off with this mask dye! Hue 2212 <i>Nonblessed</i>", 0xE26, 2212, 2000, false, typeof(MaskDye)),
            }},

            { "Sandals",
                new List<DonationItem>()
                {
                    new DonationItem("Britain Sandals", "Love Britain? Show it off with these fashionable sandals. Hue 2125 <i>Nonblessed</i>", 0x170D, 2125, 250, false, typeof(Sandals)),
                    new DonationItem("Cove Sandals", "Love Cove? Show it off with these fashionable sandals. Hue 1109 <i>Nonblessed</i>", 0x170D, 1109, 250, false, typeof(Sandals)),
                    new DonationItem("Jhelom Sandals", "Love Jhelom? Show it off with these fashionable sandals. Hue 1761 <i>Nonblessed</i>", 0x170D, 1761, 250, false, typeof(Sandals)),
                    new DonationItem("Magincia Sandals", "Love Magincia? Show it off with these fashionable sandals. Hue 1158 <i>Nonblessed</i>", 0x170D, 1158, 250, false, typeof(Sandals)),
                    new DonationItem("Minoc Sandals", "Love Minoc? Show it off with these fashionable sandals. Hue 2123 <i>Nonblessed</i>", 0x170D, 2123, 250, false, typeof(Sandals)),
                    new DonationItem("Moonglow Sandals", "Love Moonglow? Show it off with these fashionable sandals. Hue 1278 <i>Nonblessed</i>", 0x170D, 1278, 250, false, typeof(Sandals)),
                    new DonationItem("Nujel'm Sandals", "Love Nujel'm? Show it off with these fashionable sandals. Hue 2118 <i>Nonblessed</i>", 0x170D, 2118, 250, false, typeof(Sandals)),
                    new DonationItem("Ocllo Sandals", "Love Ocllo? Show it off with these fashionable sandals. Hue 1156 <i>Nonblessed</i>", 0x170D, 1156, 250, false, typeof(Sandals)),
                    new DonationItem("Serpent's Hold Sandals", "Love Serpent's Hold? Show it off with these fashionable sandals. Hue 1157 <i>Nonblessed</i>", 0x170D, 1157, 250, false, typeof(Sandals)),
                    new DonationItem("Skara Brae Sandals", "Love Skara Brae? Show it off with these fashionable sandals. Hue 1196 <i>Nonblessed</i>", 0x170D, 1196, 250, false, typeof(Sandals)),
                    new DonationItem("Trinsic Sandals", "Love Trinsic? Show it off with these fashionable sandals. Hue 1151 <i>Nonblessed</i>", 0x170D, 1151, 250, false, typeof(Sandals)),
                    new DonationItem("Vesper Sandals", "Love Vesper? Show it off with these fashionable sandals. Hue 1366 <i>Nonblessed</i>", 0x170D, 1366, 250, false, typeof(Sandals)),
                    new DonationItem("Yew Sandals", "Love Yew? Show it off with these fashionable sandals. Hue 2212 <i>Nonblessed</i>", 0x170D, 2212, 250, false, typeof(Sandals)),

                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 8 <i>Nonblessed</i>", 0x170D, 8, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 3 <i>Nonblessed</i>", 0x170D, 3, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 93 <i>Nonblessed</i>", 0x170D, 93, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 88 <i>Nonblessed</i>", 0x170D, 88, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 73 <i>Nonblessed</i>", 0x170D, 73, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 58 <i>Nonblessed</i>", 0x170D, 58, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 53 <i>Nonblessed</i>", 0x170D, 53, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 43 <i>Nonblessed</i>", 0x170D, 43, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 38 <i>Nonblessed</i>", 0x170D, 38, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 33 <i>Nonblessed</i>", 0x170D, 33, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 23 <i>Nonblessed</i>", 0x170D, 23, 500, false, typeof(Sandals)),
                    new DonationItem("Sandals", "Upgrade your look with these snazzy sandals. Hue 13 <i>Nonblessed</i>", 0x170D, 13, 500, false, typeof(Sandals)),

                    new DonationItem("Black Sandals", "Upgrade your look with these snazzy black sandals. Hue 1 <i>Nonblessed</i>", 0x170D, 1, 1000, false, typeof(Sandals)),

                   new DonationItem("An Corp Supporter Sandals", "Show your support for An Corp with this commemorative sandals! <i>Blessed</i>", 0x170D, 1159, 1450, false, typeof(AnCorpSupporterSandals)),
                }
            },

            { "Paints / Dyes",
            new List<DonationItem>()
            {
                new DonationItem("Dull Copper Pet Dye", "Tired of the same old Ice Skitter? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2419, 300, false, typeof(PetDye)),
                new DonationItem("Bronze Pet Dye", "Tired of the same old Caribou? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2418, 300, false, typeof(PetDye)),
                new DonationItem("Copper Pet Dye", "Tired of the same old Dragon Whelp? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2413, 300, false, typeof(PetDye)),
                new DonationItem("Shadow Pet Dye", "Tired of the same old Giant Bat? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2406, 300, false, typeof(PetDye)),

                new DonationItem("Gold Pet Dye", "Tired of the same old Ferret? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2215, 600, false, typeof(PetDye)),
                new DonationItem("Agapite Pet Dye", "Tired of the same old Arcane Dragon? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2425, 600, false, typeof(PetDye)),
                new DonationItem("Verite Pet Dye", "Tired of the same old Armored Crab? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2207, 600, false, typeof(PetDye)),
                new DonationItem("Valorite Pet Dye", "Tired of the same old Imp? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2219, 600, false, typeof(PetDye)),

                new DonationItem("Blood Red Pet Dye", "Tired of the same old Rock Spider? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 1157, 1000, false, typeof(PetDye)),
                new DonationItem("Black Pet Dye", "Tired of the same old Sphinx? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 1175, 1000, false, typeof(PetDye)),
                new DonationItem("White Pet Dye", "Tired of the same old Swamp Crawler? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 1154, 1000, false, typeof(PetDye)),
                new DonationItem("Dagger Island White Pet Dye", "Tired of the same old Scorpion? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 1150, 1000, false, typeof(PetDye)),
                new DonationItem("Dark Green Pet Dye", "Tired of the same old White Wyrm? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 1267, 1000, false, typeof(PetDye)),
                new DonationItem("Light Pink Pet Dye", "Tired of the same old Hellhound? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 2633, 1000, false, typeof(PetDye)),
                new DonationItem("Bright Pink Pet Dye", "Tired of the same old Drake? Change it up! 5 Charges. <i>Nonblessed</i>", 4033, 329, 1000, false, typeof(PetDye)),

                new DonationItem("Black Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 2019, 500, false, typeof(BlackPaintUpgrade)),
                new DonationItem("Blue Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 399, 500, false, typeof(BluePaintUpgrade)),
                new DonationItem("Bone Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 2107, 500, false, typeof(BonePaintUpgrade)),
                new DonationItem("Brown Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1035, 500, false, typeof(BrownPaintUpgrade)),
                new DonationItem("Dark Grey Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1890, 500, false, typeof(DarkGreyPaintUpgrade)),
                new DonationItem("Gold Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 2213, 500, false, typeof(GoldPaintUpgrade)),
                new DonationItem("Green Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1164, 500, false, typeof(GreenPaintUpgrade)),
                new DonationItem("Grey Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 905, 500, false, typeof(GreyPaintUpgrade)),
                new DonationItem("Light Red Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1654, 500, false, typeof(LightRedPaintUpgrade)),
                new DonationItem("Lime Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1191, 500, false, typeof(LimePaintUpgrade)),
                new DonationItem("Orange Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1355, 500, false, typeof(OrangePaintUpgrade)),
                new DonationItem("Pink Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 2662, 500, false, typeof(PinkPaintUpgrade)),
                new DonationItem("Purple Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1760, 500, false, typeof(PurplePaintUpgrade)),
                new DonationItem("Red Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1775, 500, false, typeof(RedPaintUpgrade)),
                new DonationItem("Sea Green Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1353, 500, false, typeof(SeaGreenPaintUpgrade)),
                new DonationItem("Tan Ship Paint", "Looking to sail the seas in style? Paint that boat! <i>Nonblessed</i>", 5363, 1031, 500, false, typeof(TanPaintUpgrade)),

                new DonationItem("Special Hair Dye", "Looking for a makeover? Dye your hair something fierce with this special hair dye. <i>Newbied.</i>", 3622, 0, 875, false, typeof(SpecialHairDye)),
                new DonationItem("Special Beard Dye", "Can't have new hair without a sweet beard to match! <i>Newbied.</i>", 3622, 0, 875, false, typeof(SpecialBeardDye)),

                new DonationItem("Leather Dye Tub", "Upgrade your armor with this unlimited leather dye tub! <i>blessed.</i>", 0xFAB, 0, 1500, false, typeof(LeatherDyeTub)),
                new DonationItem("Runebook Dye Tub", "Dye your runebooks with this dye tub! 3 Charges. <i>Nonblessed</i>", 0xFAB, 0, 150, false, typeof(RunebookDyeTub)),
                new DonationItem("Bulk Order Book Dye Tub", "Dye your bulk order books with this dye tub! 3 Charges. <i>Nonblessed</i>", 0xFAB, 0, 150, false, typeof(BulkOrderBookDyeTub)),
            }},

            { "Deeds",
            new List<DonationItem>()
            {
                new DonationItem("Clothing Bless Deed", "Blesses one article of clothing, or one armored hat. <i>blessed.</i>", 5360, 0, 450, false, typeof(ClothingBlessDeed)),
                new DonationItem("Gender Change Deed", "Fancy a gender change? <i>Nontransferrable.</i>", 5360, 0, 200, false, typeof(GenderChangeDeed)),
                new DonationItem("Weapon/Armor Rename Deed", "Rename your weapon to something intimidating with this rename deed. <i>Nontransferrable.</i>", 5360, 0, 200, false, typeof(ItemRenameDeed)),
                new DonationItem("Name Change Deed", "Need to change your name? <i>Nontransferrable.</i>", 5360, 0, 500, false, typeof(NameChangeDeed)),
                new DonationItem("Hair Restyling Deed", "Fancy a haircut? This deed will allow you to restyle your hair. <i>Nontransferrable.</i>", 5360, 0, 475, false, typeof(HairRestylingDeed)),
                new DonationItem("Beard Restyling Deed", "Need some facial manscaping? This deed will allow you to restyle your beard. <i>Nontransferrable.</i>", 5360, 0, 475, false, typeof(BeardRestylingDeed)),
                new DonationItem("Lock Down Expansion Deed", "Running out of room at home? This deed will expand your house's lockdowns by 250. <i>Nontransferrable.</i>", 5360, 0, 175, false, typeof(LockDownExpansionDeed)),
                new DonationItem("Secure Expansion Deed", "Running out of safe storage at home? This deed will expand your house's secures by 1. <i>Nontransferrable.</i>", 5360, 0, 300, false, typeof(SecureExpansionDeed)),
                new DonationItem("Sash Layering Deed", "Have an awesome sash you want everyone to see? Use this to wear it over everything. <i>blessed.</i>", 5360, 0, 1000, false, typeof(SashLayerDeed)),
                new DonationItem("Potion Keg Combining Deed", "Combine the capacities of two potion kegs!", 6464, 0, 100, false, typeof(PotionKegCombiningDeed)),
                new DonationItem("Potion Barrel Creation Deed", "Convert a potion keg with 500 capacity or more into a potion barrel!", 3703, 0, 200, false, typeof(PotionBarrelConversionDeed)),

                new DonationItem("Icy Spell Hue Deeds: Pack 1", "Icy Spells: Air Elem, Energy Bolt, Meteor Swarm, Energy Field, Curse and Magic Arrow", 5360, 2579, 2000, false, typeof(IcySpellHuePack1)),
                new DonationItem("Icy Spell Hue Deeds: Pack 2", "Icy Spells: Water Elem, Explosion, Blade Spirits, Arch Cure, Dispel, Feeblemind and Para Field", 5360, 2579, 2000, false, typeof(IcySpellHuePack2)),
                new DonationItem("Icy Spell Hue Deeds: Pack 3", "Icy Spells: Fire Elem, Flamestrike, Harm, Summon Creature, Mass Curse, Clumsy and Protection", 5360, 2579, 2000, false, typeof(IcySpellHuePack3)),
                new DonationItem("Icy Spell Hue Deeds: Pack 4", "Icy Spells: Summon Daemon, Teleport, Curse, Bless, Mass Dispel, Arch Protection and Reactive Armor", 5360, 2579, 2000, false, typeof(IcySpellHuePack4)),
                new DonationItem("Icy Spell Hue Deeds: Pack 5", "Icy Spells: Energy Vortex, Earth Elem, Wall of Stone, Magic Reflect, Fireball, Mindblast and Weaken", 5360, 2579, 2000, false, typeof(IcySpellHuePack5)),

                new DonationItem("Earthy Spell Hue Deeds: Pack 1", "Earthy Spells: Air Elem, Energy Bolt, Meteor Swarm, Energy Field, Curse and Magic Arrow", 5360, 2553, 2000, false, typeof(EarthySpellHuePack1)),
                new DonationItem("Earthy Spell Hue Deeds: Pack 2", "Earthy Spells: Water Elem, Explosion, Blade Spirits, Arch Cure, Dispel, Feeblemind and Para Field", 5360, 2553, 2000, false, typeof(EarthySpellHuePack2)),
                new DonationItem("Earthy Spell Hue Deeds: Pack 3", "Earthy Spells: Fire Elem, Flamestrike, Harm, Summon Creature, Mass Curse, Clumsy and Protection", 5360, 2553, 2000, false, typeof(EarthySpellHuePack3)),
                new DonationItem("Earthy Spell Hue Deeds: Pack 4", "Earthy Spells: Summon Daemon, Teleport, Curse, Bless, Mass Dispel, Arch Protection and Reactive Armor", 5360, 2553, 2000, false, typeof(EarthySpellHuePack4)),
                new DonationItem("Earthy Spell Hue Deeds: Pack 5", "Earthy Spells: Energy Vortex, Earth Elem, Wall of Stone, Magic Reflect, Fireball, Mindblast and Weaken", 5360, 2553, 2000, false, typeof(EarthySpellHuePack5)),

                new DonationItem("Fiery Spell Hue Deeds: Pack 1", "Fiery Spells: Air Elem, Energy Bolt, Meteor Swarm, Energy Field, Curse and Magic Arrow", 5360, 2117, 2000, false, typeof(FierySpellHuePack1)),
                new DonationItem("Fiery Spell Hue Deeds: Pack 2", "Fiery Spells: Water Elem, Explosion, Blade Spirits, Arch Cure, Dispel, Feeblemind and Para Field", 5360, 2117, 2000, false, typeof(FierySpellHuePack2)),
                new DonationItem("Fiery Spell Hue Deeds: Pack 3", "Fiery Spells: Fire Elem, Flamestrike, Harm, Summon Creature, Mass Curse, Clumsy and Protection", 5360, 2117, 2000, false, typeof(FierySpellHuePack3)),
                new DonationItem("Fiery Spell Hue Deeds: Pack 4", "Fiery Spells: Summon Daemon, Teleport, Curse, Bless, Mass Dispel, Arch Protection and Reactive Armor", 5360, 2117, 2000, false, typeof(FierySpellHuePack4)),
                new DonationItem("Fiery Spell Hue Deeds: Pack 5", "Fiery Spells: Energy Vortex, Earth Elem, Wall of Stone, Magic Reflect, Fireball, Mindblast and Weaken", 5360, 2117, 2000, false, typeof(FierySpellHuePack5)),
            }},

            { "Decorations",
            new List<DonationItem>()
            {
                new DonationItem("Pirate House Deco Package", "Yarr! Contains an anchor, spittoon, cards, and a couple other goodies for you and your wench.", 5367, 0, 700, false, typeof(PirateDecoBag)),
                new DonationItem("Dragon Slayer Package", "Show off your dragon slaying skills with these dragon slaying items.", 8756, 0, 700, false, typeof(DragonSlayerPack)),
                new DonationItem("Dollhouse Furniture Package", "Outfit your pack with this set of miniature furniture - look for future addon packs!", 0x3F1F, 0, 450, false, typeof(DollHouseFurniture1)),
                new DonationItem("Ogre House Deco Package", "Contains a cauldron, skull pole, and some other garbage for your fearsome home.", 1064, 0, 600, false, typeof(OgreDecoBag)),
                new DonationItem("Grandmaster Smith Deco Package", "A house decoration for the greatest Blacksmiths in Britannia!", 4017, 0, 1000, false, typeof(BlacksmithDecoBag)),
                new DonationItem("Hunter's Deco Package", "Decorate your fine woodland home with authentic hunter's gear!", 7776, 0, 1000, false, typeof(HuntersPack)),
                new DonationItem("Monster Slayer Deco Package", "Nothing adds ambience like severed monster heads!", 7782, 0, 1000, false, typeof(MonsterSlayerPack)),
                new DonationItem("Spooky Deco Package", "Keep pesky kids off your lawn with these spooky offerings!", 7732, 0, 1000, false, typeof(SpookyPack)),
                new DonationItem("Chef's Deco Package", "Be a good neighbor and feed your guests!", 2450, 0, 1000, false, typeof(ChefsPack)),
                new DonationItem("Ruined Home Deco Package", "Slide into squalor and disrepair... in style!", 634, 0, 1000, false, typeof(RuinedHomePack)),
                new DonationItem("Vineyard Deco Package", "Add a bit of refinement to your estate!", 3354, 0, 1000, false, typeof(VineyardPack)),
                new DonationItem("Animal Handler Deco Package", "Both beast and human alike will view this collection of animal handling items with envy!", 3896, 0, 1000, false, typeof(AnimalHandlerPack)),
                new DonationItem("Lifesize Chess Set Deco Package", "Turn the rooftop of your home into a lifesize chessboard!", 13704, 0, 1500, false, typeof(LifesizeChessSetPack)),
                new DonationItem("Music Box", "Play any Ultima Online song from the comfort of your own home!", 11005, 0, 650, false, typeof(MusicBoxSouthDeed)),
                new DonationItem("Ultima Online Cloth Map", "Hang an oversized cloth map of Britannia in your house!", 15287, 0, 650, false, typeof(ClothMapDecoration)),
                new DonationItem("Sosarian Tapestry", "Hang the quintessential piece of UO artwork right in your house!", 0x234F, 0, 650, false, typeof(SosarianTapestry)),
            }},

            { "Misc",
            new List<DonationItem>()
            {
                new DonationItem("Dragon Lottery Ticket", "Special rare artifacts and in-game items. Gives a small chance of a random prize. <i>Nontransferrable.</i>", 0x14ED, 0, 30, false, typeof(DragonLotteryTicket)),
                new DonationItem("Pouch With Many Pockets", "Blessed trapped pouch with 10 charges.", 0xE79, 14, 225, false, typeof(PouchWithManyPockets)),
                new DonationItem("Journeyman Skill Scroll Spyglass", "15 Charges.  Use on a monster to double its chance of dropping a skill scroll. (Up to journeyman level only).", 5365, 0, 35, false, typeof(JourneymanSkillScrollSpyglass)),
                new DonationItem("Expert Skill Scroll Spyglass", "15 Charges.  Use on a monster to double its chance of dropping a skill scroll. (Up to expert level only).", 5365, 0, 50, false, typeof(ExpertSkillScrollSpyglass)),
                new DonationItem("Adept Skill Scroll Spyglass", "10 Charges.  Use on a monster to double its chance of dropping a skill scroll. (Up to adept level only).", 5365, 0, 75, false, typeof(AdeptSkillScrollSpyglass)),
                new DonationItem("An Corp Supporter Fancy Dress", "Show your support for An Corp with this commemorative fancy dress! <i>Blessed</i>", 0x1EFF, 1159, 750, false, typeof(AnCorpSupporterFancyDress)),
                new DonationItem("An Corp Supporter Fancy Shirt", "Show your support for An Corp with this commemorative fancy shirt! <i>Blessed</i>", 0x1EFD, 1159, 750, false, typeof(AnCorpSupporterFancyShirt)),
                new DonationItem("An Corp Supporter Shirt", "Show your support for An Corp with this commemorative shirt! <i>Blessed</i>", 0x1517, 1159, 750, false, typeof(AnCorpSupporterShirt)),
                new DonationItem("An Corp Supporter Tunic", "Show your support for An Corp with this commemorative tunic! <i>Blessed</i>", 0x1fa1, 1159, 750, false, typeof(AnCorpSupporterTunic)),
                new DonationItem("Incognito Potion", "Change your appearance and go undercover for a brief time with this incognito potion! (duration 5 minutes)", 0xF0B, 2515, 50, false, typeof(IncognitoPotion)),
                new DonationItem("Monstrous Polymorph Potion", "Change your appearance to that of a random monster! (duration 5 minutes)", 0xF0B, 2600, 50, false, typeof(PolymorphPotion)),
            }},

            { "Pet Battles",
            new List<DonationItem>()
            {
                new DonationItem("Burrow Beetle", "A Burrow Beetle to decimate your opponents in the Pet Battle Arena.<i>Nontransferrable.</i>", 9743, 0, 325, false, typeof(PetBattleBurrowBeetleToken)),
            }},

            { "Addons",
            new List<DonationItem>()
            {
                new DonationItem("Hearth", "A warm, cozy fireplace for your home.", 5360, 0, 485, false, typeof(HearthOfHomeFireDeed)),
                new DonationItem("Banner", "Display a proud banner in your home.", 5552, 0, 125, false, typeof(BannerDeed)),
                new DonationItem("Archery Butte", "An archery butte for your home.", 5360, 0, 125, false, typeof(ArcheryButteDeed)),
                new DonationItem("Sandstone Fountain", "A sandstone fountain addon for your home.", 5360, 0, 350, false, typeof(SandstoneFountainDeed)),
                new DonationItem("Mining Cart", "A mining cart addon deed.", 6787, 0, 600, false, typeof(MiningCartDeed)),
                new DonationItem("Tree Stump", "A tree stump addon deed.", 3672, 0, 600, false, typeof(TreeStumpDeed)),
                new DonationItem("Aquarium Deed (east)", "Show off your collections from the sea with your very own aquarium!", 17159, 0, 750, false, typeof(AquariumEastDeed)),
                new DonationItem("Aquarium Deed (north)", "Show off your collections from the sea with your very own aquarium!", 17159, 0, 750, false, typeof(AquariumNorthDeed)),
                new DonationItem("House Bulletin Board (south)", "Leave messages for your friends and guildmates in your home!", 8977, 0, 500, false, typeof(PlayerBBSouth)),
                new DonationItem("House Bulletin Board (east)", "Leave messages for your friends and guildmates in your home!", 8978, 0, 500, false, typeof(PlayerBBEast)),

            }},
        };

        public static void Initialize()
        {
            CommandSystem.Register("DonationShop", AccessLevel.Player, new CommandEventHandler(DonationShop_OnCommand));
        }

        [Usage("DonationShop")]
        [Aliases("DonationStore", "Store", "Shop", "Donation")]
        [Description("Views the donation shop for UOAC.")]
        public static void DonationShop_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.IsInUOACZ)
                {
                    player.SendMessage("The donation shop cannot be accessed while in this region.");
                    return;
                }
            }

            if (!e.Mobile.HasGump(typeof(DonationShop)))
                e.Mobile.SendGump(new DonationShop());
        }

        public static void LogPurchase(PlayerMobile player, String type, int amount)
        {
            if (type == null || player == null)
                return;

            using (StreamWriter op = new StreamWriter("doantion_shop.log", true))
            {
                op.Write("{0}\t : {1} bought {2} for {3}", DateTime.UtcNow.ToShortDateString(), player.Name.ToUpper(), type.ToString(), amount);
                op.WriteLine();
            }
        }

        public static void PurchaseItem(Mobile from, DonationItem di, DonationShop gump)
        {
            int cost = di.Cost;
            int supply = 0;

            Item[] stacks;

            stacks = from.Backpack.FindItemsByType(typeof(DragonCoin));

            for (int i = 0; i < stacks.Length; i++)
                supply += stacks[i].Amount;

            if (supply < cost)
            {
                from.SendMessage("You do not have enough donation currency to purchase that item.");
                from.SendGump(gump);
                return;
            }

            object newObj = Activator.CreateInstance(di.ItemType);
            
            if (newObj != null)
            {
                Item newItem = newObj as Item;
                newItem.Hue = di.ItemHue;

                if (newItem == null)
                    return;

                if (newItem is ValentinesGift)
                {
                    var necklace = new GoldNecklace();
                    necklace.Name = String.Format("Happy Valentine's Day{0}", from == null ? "" : " from " + from.RawName);
                    necklace.LootType = LootType.Blessed;
                    ((ValentinesGift)newItem).AddItem(necklace);
                }

                if (newItem != null && Array.IndexOf(DonationItemTypes, di.ItemType) != -1)
                    newItem.DonationItem = true;

                if (from.AddToBackpack(newItem))
                {
                    from.Backpack.ConsumeTotal(typeof(DragonCoin), cost);

                    if (from.AccessLevel == AccessLevel.Player)
                        LogPurchase(from as PlayerMobile, di.ItemType.ToString(), cost);

                    from.SendMessage(String.Format("Thank you for your purchasing {0}.  Your donation is very much appreciated.", di.Name));
                }
                else
                {
                    from.SendMessage("Your backpack is full. Please make room and try again.");

                    if (newItem != null)
                        newItem.Delete();
                }

            }

            from.SendGump(gump);
        }
    }

    public class DonationItem
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Type ItemType { get; private set; }
        public int ItemID { get; private set; }
        public int ItemHue { get; private set; }
        public int Cost { get; private set; }
        public bool Limited { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }

        public DonationItem(string name, string description, int itemID, int itemHue, int cost, bool limited, Type itemType, int xOffset = 0, int yOffset = 0)
        {
            Name = name;
            Description = description;
            ItemID = itemID;
            ItemHue = itemHue;
            Cost = cost;
            Limited = limited;
            ItemType = itemType;
            XOffset = xOffset;
            YOffset = yOffset;
        }
    }
}
