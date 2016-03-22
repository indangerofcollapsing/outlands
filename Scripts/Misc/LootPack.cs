using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.Items;
using Server.Custom.Donations.Items;

namespace Server
{
    public class LootPack
    {
        public static int GetLuckChanceForKiller(Mobile dead)
        {
            BaseCreature killed = dead as BaseCreature;
            if (killed == null)
                return 0;
            else
                return killed.GetLuckChanceForKiller() * 100;
        }

        public static bool CheckLuck(int chance)
        {
            return (chance > Utility.Random(10000));
        }

        private LootPackEntry[] m_Entries;

        public LootPack(LootPackEntry[] entries)
        {
            m_Entries = entries;
        }

        public void Generate(Mobile from, Container cont, bool spawning)
        {
            if (cont == null)
                return;

            int luckChance = 0;

            if (!spawning)
                luckChance = LootPack.GetLuckChanceForKiller(from);

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                LootPackEntry entry = m_Entries[i];

                bool shouldAdd = entry.Chance > Utility.Random(10000);

                if (!shouldAdd)
                {
                    if (LootPack.CheckLuck(luckChance))
                        shouldAdd = entry.Chance > Utility.Random(10000);
                }

                if (!shouldAdd)
                    continue;

                Item item = entry.Construct(from, luckChance, spawning);

                if (item != null)
                {
                    if (!item.Stackable || !cont.TryDropItem(from, item, false))
                        cont.DropItem(item);
                }
            }
        }
        
        public static readonly LootPackItem[] Gold = new LootPackItem[]
			{
				new LootPackItem( typeof( Gold ), 1 )
			};

        public static readonly LootPackItem[] Instruments = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseInstrument ), 1 )
			};


        public static readonly LootPackItem[] LowScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ClumsyScroll ), 1 )
			};

        public static readonly LootPackItem[] MedScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( ArchCureScroll ), 1 )
			};

        public static readonly LootPackItem[] HighScrollItems = new LootPackItem[]
			{
				new LootPackItem( typeof( SummonAirElementalScroll ), 1 )
			};

        public static readonly LootPackItem[] GemItems = new LootPackItem[]
			{
				new LootPackItem( typeof( Amber ), 1 )
			};

        public static readonly LootPackItem[] PotionItems = new LootPackItem[]
			{
				new LootPackItem( typeof( AgilityPotion ), 1 ),
				new LootPackItem( typeof( StrengthPotion ), 1 ),
				new LootPackItem( typeof( RefreshPotion ), 1 ),
				new LootPackItem( typeof( LesserCurePotion ), 1 ),
				new LootPackItem( typeof( LesserHealPotion ), 1 ),
				new LootPackItem( typeof( LesserPoisonPotion ), 1 )
			};

        public static readonly LootPackItem[] MagicItems = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseJewel ), 1 ),
				new LootPackItem( typeof( BaseArmor ), 4 ),
				new LootPackItem( typeof( BaseWeapon ), 3 ),
				new LootPackItem( typeof( BaseShield ), 1 )
			};

        public static readonly LootPackItem[] Reagents = new LootPackItem[]
        {
            new LootPackItem(typeof(BlackPearl), 50),
            new LootPackItem(typeof(SulfurousAsh), 50),
            new LootPackItem(typeof(SpidersSilk), 50),
            new LootPackItem(typeof(MandrakeRoot), 50),
            new LootPackItem(typeof(Bloodmoss), 50),
            new LootPackItem(typeof(Garlic), 50),
            new LootPackItem(typeof(Ginseng), 50),
            new LootPackItem(typeof(Nightshade), 50),
        };

        public static readonly LootPackItem[] Resources = new LootPackItem[] {
            new LootPackItem(typeof(Bandage), 50),
            new LootPackItem(typeof(Arrow), 25),
            new LootPackItem(typeof(Bolt), 25),
        };

        public static readonly LootPack NewbieDungeon = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, Reagents, 10, 3),
            new LootPackEntry(false, Resources, 10, 5),
        });

        public static readonly LootPack VeryPoor = new LootPack(new LootPackEntry[]
            {
                new LootPackEntry(false, MagicItems, 10, 1, 0, 0, 0),
            });


        public static readonly LootPack Poor = new LootPack(new LootPackEntry[]
			{
                new LootPackEntry(false, MagicItems, 20, 1, 1, 1, 1),
			});

        public static readonly LootPack Meager = new LootPack(new LootPackEntry[]
			{
				//new LootPackEntry( false, Instruments,	  0.05, 1 ),
				new LootPackEntry( false, MagicItems,  0.20, 1, 1, 0, 30 ),
				new LootPackEntry( false, MagicItems,  0.10, 1, 1, 0, 40 )
			});

        public static readonly LootPack Average = new LootPack(new LootPackEntry[]
			{
				//new LootPackEntry( false, Instruments,	  0.40, 1 ),
				new LootPackEntry( false, MagicItems,  2.50, 1, 1, 0, 40 ),
				new LootPackEntry( false, MagicItems,  1.00, 1, 1, 20, 40 ),
				new LootPackEntry( false, MagicItems,  0.01, 1, 1, 0, 90 )
			});

        public static readonly LootPack Rich = new LootPack(new LootPackEntry[]
			{
				//new LootPackEntry( false, Instruments,	  1.00, 1 ),
				new LootPackEntry( false, MagicItems, 5.00, 1, 1, 0, 60 ),
				new LootPackEntry( false, MagicItems, 5.00, 1, 1, 20, 80 ),
				new LootPackEntry( false, MagicItems,  0.1, 1, 1, 60, 100 )
			});

        public static readonly LootPack FilthyRich = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItems,  15.0, 1, 2, 30, 80 ),
				new LootPackEntry( false, MagicItems,  10.0, 1, 2, 40, 80 ),
				new LootPackEntry( false, MagicItems,  5.00, 1, 2, 40, 100 ),
				new LootPackEntry( false, MagicItems,  1.00, 1, 2, 60, 100 )
			});

        public static readonly LootPack UltraRich = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItems,	40, 1, 2, 40, 80 ),
				new LootPackEntry( false, MagicItems,	40, 1, 2, 10, 80 ),
				new LootPackEntry( false, MagicItems,	10, 1, 2, 10, 100 ),
				new LootPackEntry( false, MagicItems,	10, 1, 2, 10, 100 ),
				new LootPackEntry( false, MagicItems,	1, 1, 2, 60, 100 ),
				new LootPackEntry( false, MagicItems,	1, 1, 2, 60, 100 )
			});

        public static readonly LootPack SuperBoss = new LootPack(new LootPackEntry[]
		{
			new LootPackEntry( false, MagicItems,	40, 1, 2, 40, 100 ),
			new LootPackEntry( false, MagicItems,	40, 1, 2, 40, 100 ),
			new LootPackEntry( false, MagicItems,	40, 1, 2, 40, 100 ),
			new LootPackEntry( false, MagicItems,	30, 1, 2, 50, 100 ),
			new LootPackEntry( false, MagicItems,	30, 1, 2, 50, 100 ),
			new LootPackEntry( false, MagicItems,	30, 1, 2, 50, 100 ),
			new LootPackEntry( false, MagicItems,	20, 1, 2, 60, 100 ),
			new LootPackEntry( false, MagicItems,	20, 1, 2, 60, 100 ),
			new LootPackEntry( false, MagicItems,	20, 1, 2, 60, 100 ),
			new LootPackEntry( false, MagicItems,	10, 1, 2, 70, 100 ),
            new LootPackEntry( false, HighScrollItems, 10, 1 ),
		});  

        public static readonly LootPackItem[] RareClothHues = new LootPackItem[] {
            new LootPackItem(typeof(RareCloth), 1),
        };

        public static readonly LootPackItem[] TownClothHues = new LootPackItem[] {
            new LootPackItem(typeof(TownCloth), 1),
        };

        public static readonly LootPackItem[] BossClothHues = new LootPackItem[] {
            new LootPackItem(typeof(BossCloth), 1),
        };

        public static readonly LootPackItem[] BossSandalHues = new LootPackItem[] {
            new LootPackItem(typeof(RareSandals), 1),
        };

        public static readonly LootPackItem[] BossMaskDyeHues = new LootPackItem[] {
            new LootPackItem(typeof(MaskDye), 1),
        };

        public static readonly LootPackItem[] RareCraftingIngredients = new LootPackItem[]
        {
            // only one of each entry

            // alchemy
            new LootPackItem(typeof(RareVial), 1),
            new LootPackItem(typeof(RareFlask), 1),
            // bowcraft
            new LootPackItem(typeof(RareArrowBundle), 1),
            new LootPackItem(typeof(RareBoltBundle), 1),
            // blacksmith
            new LootPackItem(typeof(Rope), 1),
            new LootPackItem(typeof(RarePegboard), 1),
            new LootPackItem(typeof(RareChains), 1),
            new LootPackItem(typeof(RareGoldIngotBundle), 1),
            new LootPackItem(typeof(RareBeeswax), 1),
            new LootPackItem(typeof(RareCopperIngotBundle), 1),
            // carpentry
            new LootPackItem(typeof(RareDartboard), 1),
            new LootPackItem(typeof(RareFoldedSheets), 1),
            new LootPackItem(typeof(RareBrokenChair), 1),
            new LootPackItem(typeof(RareLogPile), 1),
            // cooking
            new LootPackItem(typeof(RarePot), 1),
            new LootPackItem(typeof(RareHorseDung), 1),
            new LootPackItem(typeof(RareBerries), 5),
            // inscription
            new LootPackItem(typeof(RareOpenBook), 1),
            new LootPackItem(typeof(RareBook), 1),
            new LootPackItem(typeof(RareNecroScroll), 1),
            // tailoring
            new LootPackItem(typeof(RareFishnetLarge), 1),
            new LootPackItem(typeof(RareFishnetSmall), 1),
            // tinkering
            new LootPackItem(typeof(IronWire), 1),
	    };

        public static readonly LootPackItem[] CommonTitleDyes = new LootPackItem[] 
        {
            new LootPackItem(typeof(CommonTitleDye), 1)
        };
        public static readonly LootPackItem[] UncommonTitleDyes = new LootPackItem[] 
        {
            new LootPackItem(typeof(UncommonTitleDye), 1)
        };
        public static readonly LootPackItem[] RareTitleDyes = new LootPackItem[] 
        {
            new LootPackItem(typeof(RareTitleDye), 1)
        };
        public static readonly LootPackItem[] VeryRareTitleDyes = new LootPackItem[] 
        {
            new LootPackItem(typeof(VeryRareTitleDye), 1)
        };

        public static readonly LootPackItem[] EnhancedTomeItems = new LootPackItem[] 
        {
    		new LootPackItem(typeof(EnhancedSpellbook), 1),
        };

        public static readonly LootPack RareCloth = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, RareClothHues, 0.025, 5),
        });

        public static readonly LootPack TownCloth = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, TownClothHues, 0.075, 5),
        });

        public static readonly LootPack BossCloth = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, BossClothHues, 1, 5),
        });

        public static readonly LootPack BossSandals = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, BossSandalHues, 1, 1),
        });

        public static readonly LootPack BossMaskDye = new LootPack(new LootPackEntry[] {
            new LootPackEntry(false, BossMaskDyeHues, 1, 1),
        });

        public static readonly LootPack CommonTitleDye = new LootPack(new LootPackEntry[] 
        {
            new LootPackEntry(false, CommonTitleDyes, 1.0, 1),
        });
        public static readonly LootPack UncommonTitleDye = new LootPack(new LootPackEntry[] 
        {
            new LootPackEntry(false, UncommonTitleDyes, 1.0, 1),
        });
        public static readonly LootPack RareTitleDye = new LootPack(new LootPackEntry[] 
        {
            new LootPackEntry(false, RareTitleDyes, 0.5, 1),
        });
        public static readonly LootPack VeryRareTitleDye = new LootPack(new LootPackEntry[] 
        {
            new LootPackEntry(false, VeryRareTitleDyes, 0.1, 1),
        });

        public static readonly LootPack RareCrafting = new LootPack(new LootPackEntry[] 
        {
            new LootPackEntry(false, RareCraftingIngredients, 0.25, 1),
        });
        
        public static readonly LootPack EnhancedTome = new LootPack(new LootPackEntry[] 
        {
    	    new LootPackEntry(false, EnhancedTomeItems, 1.0, 1),
  	    });

        public static readonly LootPack LowScrolls = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, LowScrollItems,	100.00, 1 )
			});

        public static readonly LootPack MedScrolls = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, LowScrollItems,	100.00, 1 )
			});

        public static readonly LootPack HighScrolls = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, LowScrollItems,	100.00, 1 )
			});

        public static readonly LootPack Gems = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, GemItems,			100.00, 1 )
			});

        public static readonly LootPack Potions = new LootPack(new LootPackEntry[]
			{
				new LootPackEntry( false, PotionItems,		100.00, 1 )
			});
    }

    public class LootPackEntry
    {
        private int m_Chance;
        private LootPackDice m_Quantity;

        private int m_MaxProps, m_MinIntensity, m_MaxIntensity;

        private bool m_AtSpawnTime;

        private LootPackItem[] m_Items;

        public int Chance
        {
            get { return m_Chance; }
            set { m_Chance = value; }
        }

        public LootPackDice Quantity
        {
            get { return m_Quantity; }
            set { m_Quantity = value; }
        }

        public int MaxProps
        {
            get { return m_MaxProps; }
            set { m_MaxProps = value; }
        }

        public int MinIntensity
        {
            get { return m_MinIntensity; }
            set { m_MinIntensity = value; }
        }

        public int MaxIntensity
        {
            get { return m_MaxIntensity; }
            set { m_MaxIntensity = value; }
        }

        public LootPackItem[] Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }

        public Item Construct(Mobile from, int luckChance, bool spawning)
        {
            if (m_AtSpawnTime != spawning)
                return null;

            int totalChance = 0;

            for (int i = 0; i < m_Items.Length; ++i)
                totalChance += m_Items[i].Chance;

            int rnd = Utility.Random(totalChance);

            for (int i = 0; i < m_Items.Length; ++i)
            {
                LootPackItem item = m_Items[i];

                if (rnd < item.Chance)
                    return Mutate(from, luckChance, item.Construct());

                rnd -= item.Chance;
            }

            return null;
        }

        private int GetRandomBonus(int luck = 0)
        {
            int rnd = Utility.RandomMinMax(m_MinIntensity, m_MaxIntensity);
            rnd += luck / 10000;

            if (50 > rnd)
                return 1;
            else
                rnd -= 50;

            if (25 > rnd)
                return 2;
            else
                rnd -= 25;

            if (14 > rnd)
                return 3;
            else
                rnd -= 14;

            if (8 > rnd)
                return 4;

            return 5;
        }

        public Item Mutate(Mobile from, int luckChance, Item item)
        {
            if (item != null)
            {
                if (item is BaseWeapon && 1 > Utility.Random(100))
                {
                    item.Delete();
                    item = new FireHorn();
                    return item;
                }

                if (item is BaseWeapon || item is BaseArmor)
                {
                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        if (0.80 > Utility.RandomDouble() + (luckChance / 10000))
                            weapon.AccuracyLevel = (WeaponAccuracyLevel)GetRandomBonus(luckChance);

                        if (0.60 > Utility.RandomDouble() + (luckChance / 10000))
                            weapon.DamageLevel = (WeaponDamageLevel)GetRandomBonus(luckChance);

                        if (0.40 > Utility.RandomDouble() + (luckChance / 10000))
                            weapon.DurabilityLevel = (WeaponDurabilityLevel)GetRandomBonus(luckChance);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        if (0.80 > Utility.RandomDouble() + (luckChance / 10000))
                            armor.ProtectionLevel = (ArmorProtectionLevel)GetRandomBonus(luckChance);

                        if (0.40 > Utility.RandomDouble() + (luckChance / 10000))
                            armor.Durability = (ArmorDurabilityLevel)GetRandomBonus(luckChance);
                    }
                }

                else if (item is BaseInstrument)
                {

                    BaseInstrument instr = (BaseInstrument)item;

                    instr.Quality = InstrumentQuality.Regular;
                }

                if (item.Stackable)
                    item.Amount = m_Quantity.Roll();
            }

            return item;
        }

        public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, string quantity)
            : this(atSpawnTime, items, chance, new LootPackDice(quantity), 0, 0, 0)
        {
        }

        public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, int quantity)
            : this(atSpawnTime, items, chance, new LootPackDice(0, 0, quantity), 0, 0, 0)
        {
        }

        public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, string quantity, int maxProps, int minIntensity, int maxIntensity)
            : this(atSpawnTime, items, chance, new LootPackDice(quantity), maxProps, minIntensity, maxIntensity)
        {
        }

        public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity)
            : this(atSpawnTime, items, chance, new LootPackDice(0, 0, quantity), maxProps, minIntensity, maxIntensity)
        {
        }

        public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int maxProps, int minIntensity, int maxIntensity)
        {
            m_AtSpawnTime = atSpawnTime;
            m_Items = items;
            m_Chance = (int)(100 * chance);
            m_Quantity = quantity;
            m_MaxProps = maxProps;
            m_MinIntensity = minIntensity;
            m_MaxIntensity = maxIntensity;
        }
    }

    public class LootPackItem
    {
        private Type m_Type;
        private int m_Chance;

        public Type Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public int Chance
        {
            get { return m_Chance; }
            set { m_Chance = value; }
        }

        private static Type[] m_BlankTypes = new Type[] { typeof(BlankScroll) };
        private static Type[] m_MasterScrolls = new Type[] 
        {
            // 7th
            typeof(ChainLightningScroll), typeof(EnergyFieldScroll), typeof(FlamestrikeScroll), typeof(GateTravelScroll), typeof(MassDispelScroll), typeof(MeteorSwarmScroll), typeof(PolymorphScroll),
            // 8th
            typeof(SummonAirElementalScroll), typeof(SummonEarthElementalScroll), typeof(EarthquakeScroll), typeof(EnergyVortexScroll), typeof(SummonFireElementalScroll), typeof(SummonDaemonScroll), typeof(SummonWaterElementalScroll),
        };

        public static Item RandomEnhancedSpellbook()
        {
            Array types = Enum.GetValues(typeof(EnhancedSpellbookType));
            EnhancedSpellbookType tomeType = EnhancedSpellbookType.Energy;

            if (Utility.RandomDouble() < 0.5)
                tomeType = EnhancedSpellbookType.Slayer;
            else
                tomeType = (EnhancedSpellbookType)types.GetValue(Utility.Random(types.Length - 1));

            return new EnhancedSpellbookTome(tomeType);
        }

        public static Item RandomScroll(int index, int minCircle, int maxCircle)
        {
            --minCircle;
            --maxCircle;

            int scrollCount = ((maxCircle - minCircle) + 1) * 8;

            if (index == 0)
                scrollCount += m_BlankTypes.Length;

            int rnd = Utility.Random(scrollCount);

            if (index == 0 && rnd < m_BlankTypes.Length)
                return Loot.Construct(m_BlankTypes);
            else if (index == 0)
                rnd -= m_BlankTypes.Length;


            return Loot.RandomScroll(minCircle * 8, (maxCircle * 8) + 7, SpellbookType.Regular);
        }

        public Item Construct()
        {
            try
            {
                Item item;

                if (m_Type == typeof(BaseWeapon))
                    item = Loot.RandomWeapon();
                else if (m_Type == typeof(BaseArmor))
                    item = Loot.RandomArmor();
                else if (m_Type == typeof(BaseShield))
                    item = Loot.RandomShield();
                else if (m_Type == typeof(BaseInstrument))
                    item = Loot.RandomInstrument();
                else if (m_Type == typeof(Amber)) // gem
                    item = Loot.RandomGem();
                else if (m_Type == typeof(ClumsyScroll)) // low scroll
                    item = RandomScroll(0, 1, 3);
                else if (m_Type == typeof(ArchCureScroll)) // med scroll
                    item = RandomScroll(1, 4, 6);
                else if (m_Type == typeof(SummonAirElementalScroll)) // master scroll
                    item = RandomMasterScroll();
                else if (m_Type == typeof(EnhancedSpellbook)) // enhanced spellbook
                    item = RandomEnhancedSpellbook();               
                else if (m_Type == typeof(MaskDye))
                    item = MaskDye.RandomDye();
                else
                    item = Activator.CreateInstance(m_Type) as Item;

                return item;
            }
            catch
            {
            }

            return null;
        }

        private Item RandomMasterScroll()
        {
            var scroll = m_MasterScrolls[Utility.Random(m_MasterScrolls.Length)];
            return SpellScroll.MakeMaster(Activator.CreateInstance(scroll) as SpellScroll);
        }

        public LootPackItem(Type type, int chance)
        {
            m_Type = type;
            m_Chance = chance;
        }
    }

    public class LootPackDice
    {
        private int m_Count, m_Sides, m_Bonus;

        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        public int Sides
        {
            get { return m_Sides; }
            set { m_Sides = value; }
        }

        public int Bonus
        {
            get { return m_Bonus; }
            set { m_Bonus = value; }
        }

        public int Roll()
        {
            int v = m_Bonus;

            for (int i = 0; i < m_Count; ++i)
                v += Utility.Random(1, m_Sides);

            return v;
        }

        public LootPackDice(string str)
        {
            int start = 0;
            int index = str.IndexOf('d', start);

            if (index < start)
                return;

            m_Count = Utility.ToInt32(str.Substring(start, index - start));

            bool negative;

            start = index + 1;
            index = str.IndexOf('+', start);

            if (negative = (index < start))
                index = str.IndexOf('-', start);

            if (index < start)
                index = str.Length;

            m_Sides = Utility.ToInt32(str.Substring(start, index - start));

            if (index == str.Length)
                return;

            start = index + 1;
            index = str.Length;

            m_Bonus = Utility.ToInt32(str.Substring(start, index - start));

            if (negative)
                m_Bonus *= -1;
        }

        public LootPackDice(int count, int sides, int bonus)
        {
            m_Count = count;
            m_Sides = sides;
            m_Bonus = bonus;
        }
    }
}