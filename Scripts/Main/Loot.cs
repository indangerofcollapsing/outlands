using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

using Server.Custom.Items;

namespace Server
{
	public class Loot
	{
        public enum LootTier
        {
            Zero=0,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
        }

        public static LootTier GetLootTier(BaseCreature creature)
        {
            double difficulty = creature.Difficulty;

            LootTier[] values = (LootTier[])Enum.GetValues(typeof(LootTier));
            for (int i = values.Length - 1; i >= 0; i--)
            {
                if (difficulty >= LootTierThreshold[(LootTier)i])
                    return (LootTier)i;
            }

            return LootTier.Zero;
        }

        private static Dictionary<LootTier, double> LootTierThreshold = new Dictionary<LootTier, double>()
        {
            {LootTier.Zero,  00.00},  // rats, mongbats
            {LootTier.One,   00.50},  // 
            {LootTier.Two,   02.00},  // 
            {LootTier.Three, 05.00},  // 
            {LootTier.Four,  09.00},  // 
            {LootTier.Five,  14.00},  // 
            {LootTier.Six,   21.00},  // 
            {LootTier.Seven, 30.00},  // end game mobs (balrons, AL, AW)
            {LootTier.Eight, 60.00},  // world bosses
        };

        private static void AddSequentialBossLoot(BaseCreature mob)
        {
            mob.AddLoot(LootPack.BossCloth);
            mob.AddLoot(LootPack.BossSandals);
            mob.AddLoot(LootPack.BossMaskDye);
        }

        private static void AddGlobalLootTables(BaseCreature mob)
        {
            int amount = Math.Max(1, (int)mob.LootTier - 5);

            if (mob.LootTier > LootTier.Two)
            {
                mob.AddLoot(LootPack.RareCrafting, amount);
                mob.AddLoot(LootPack.RareCloth, amount);
                mob.AddLoot(LootPack.TownCloth, amount);
            }
            
            mob.AddLoot(LootPack.EnhancedTome, amount);
        }

        private static void AddDungeonArmor(BaseCreature mob)
        {
            if (mob.Region == null) return; 

            double chance = 1.0 / 2500.0;
            int luck = 0;
            if (mob.LastPlayerKiller != null) luck = mob.LastPlayerKiller.Luck;

            chance = chance * (1 + (luck / 100.0));

            bool getArmor = false;

            if (Utility.RandomDouble() <= chance)
            {
                getArmor = true;

                IndexedRegionName regionName = mob.Region.IndexedName;
                Item item = null;

                switch (regionName)
                {
                    /*
                    case IndexedRegionName.Deceit: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Deceit, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.Destard: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Destard, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.Shame: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Shame, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.Hythloth: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Hythloth, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;


                    //Luthius Expansion                    
                    case IndexedRegionName.Covetous: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Covetous, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.Wrong: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Wrong, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.Despise: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Despise, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.IceDungeon: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Ice, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    case IndexedRegionName.FireDungeon: item = DungeonArmor.CreateDungeonArmor(DungeonEnum.Fire, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified); break;
                    */
                }

                if (item != null)
                    mob.PackItem(item);
            }

            //if (getArmor)
                //mob.PackItem(new ArcaneDust());
        }

        public static void AddTieredLoot(BaseCreature mob)
        {
            if (mob.IsBonded || mob.Summoned || mob.NoKillAwards) return;

            AddGlobalLootTables(mob);
            AddDungeonArmor(mob);

            if (mob.Region is NewbieDungeonRegion)
                mob.AddLoot(LootPack.NewbieDungeon);
            
            double uniqueLootChance = .001;

            double craftingChanceComponentScalar = 5;
            double prestigeChanceScalar = 5;
            double researchMaterialsChanceScalar = 2.5;
            double spellHueDeedChanceScalar = 0.5;
            double UOACZUnlockableChanceScalar = 0.3;

            switch (mob.LootTier)
            {
                case Loot.LootTier.One: 
                    mob.PackMagicItems(1, 1, 0.05);

                    uniqueLootChance = .001;

                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);      
                break;

                case Loot.LootTier.Two:
                    mob.PackMagicItems(1, 1, 0.05);

                    uniqueLootChance = .0025;
                            
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);
                break;

                case Loot.LootTier.Three:
                    mob.PackGem();
                    mob.PackMagicItems(1, 2, 0.05);

                    uniqueLootChance = .005;

                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);
                break;

                case Loot.LootTier.Four:
                    mob.AddLoot(LootPack.TownCloth);

                    mob.PackMagicItems(1, 3, 0.05);
                    mob.PackMagicItems(1, 3, 0.05);

                    uniqueLootChance = .0075;
                                
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);

                    mob.PackPetDye(1, 1.0 / 8000.0);
                break;

                case Loot.LootTier.Five:
                    mob.AddLoot(LootPack.TownCloth);
                    mob.AddLoot(LootPack.CommonTitleDye);
                    mob.PackGem(1, 2);
                    mob.PackArmor(1, 4, 0.1);
                    mob.PackWeapon(1, 4, 0.1);

                    uniqueLootChance = .01;
                               
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);

                    mob.PackPetDye(1, 1.0 / 4000.0);
                break;

                case Loot.LootTier.Six:
                    mob.AddLoot(LootPack.TownCloth);
                    mob.AddLoot(LootPack.UncommonTitleDye);
                    mob.PackGem(2, 4);
                    mob.PackArmor(2, 5, 0.2);
                    mob.PackWeapon(2, 5, 0.2);

                    uniqueLootChance = .015;
                 
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);

                    mob.PackPetDye(1, 1.0 / 2000.0);
                break;

                case Loot.LootTier.Seven:
                    mob.AddLoot(LootPack.FilthyRich);
                    mob.AddLoot(LootPack.TownCloth);
                    mob.AddLoot(LootPack.RareTitleDye);
                    mob.PackGem(3, 6);
                    mob.PackArmor(2, 5, 0.2);
                    mob.PackArmor(2, 5, 0.2);
                    mob.PackWeapon(2, 5, 0.2);
                    mob.PackWeapon(2, 5, 0.2);

                    uniqueLootChance = .02;
                
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);

                    mob.PackPetDye(1, 1.0 / 1000.0);
                break;

                case Loot.LootTier.Eight:
                    int max = Math.Max(1, (int)(mob.Difficulty / 100));
                    mob.AddLoot(LootPack.SuperBoss, Utility.RandomMinMax(1, max));
                    mob.AddLoot(LootPack.BossCloth, Utility.RandomMinMax(1, max));
                    mob.AddLoot(LootPack.BossSandals, Utility.RandomMinMax(1, max));
                    mob.AddLoot(LootPack.BossMaskDye, Utility.RandomMinMax(1, max));
                    mob.AddLoot(LootPack.VeryRareTitleDye);
                    mob.PackGem(6, 9);
                    mob.PackArmor(3, 5);
                    mob.PackArmor(2, 5);
                    mob.PackWeapon(3, 5);
                    mob.PackWeapon(2, 5);

                    uniqueLootChance = .04;
                
                    mob.PackCraftingComponent(1, uniqueLootChance * craftingChanceComponentScalar);
                    mob.PackPrestigeScroll(1, uniqueLootChance * prestigeChanceScalar);
                    mob.PackResearchMaterials(1, uniqueLootChance * researchMaterialsChanceScalar);
                    mob.PackSpellHueDeed(1, uniqueLootChance * spellHueDeedChanceScalar);
                    mob.PackUOACZUnlockableDeed(1, uniqueLootChance * UOACZUnlockableChanceScalar);

                    mob.PackPetDye(1, 1.0 / 1000.0);
                break;
            }
        }

        private static void AddHybridLoot(BaseCreature mob)
        {
            if (Utility.RandomBool())
                AddSpellCasterLoot(mob);
        }        

        private static void AddSpellCasterLoot(BaseCreature mob)
        {
            switch (mob.LootTier)
            {
                case LootTier.Zero:
                case LootTier.One:
                case LootTier.Two:
                    mob.PackScroll(1, 4);
                    mob.PackReg(1, 4);
                    break;
                case LootTier.Three:
                case LootTier.Four:
                    mob.PackScroll(1, 6);
                    mob.PackScroll(1, 5);
                    mob.PackReg(4, 12);
                    break;
                case LootTier.Five:
                case LootTier.Six:
                case LootTier.Seven:
                    mob.PackScroll(2, 6);
                    mob.PackScroll(2, 6);
                    mob.PackReg(8, 16);
                    mob.PackReg(8, 16);
                    break;
            }
        }

        public static Type[] WeaponTypes { get { return m_WeaponTypes; } }
        private static Type[] m_WeaponTypes = new Type[]
			{
				typeof( Axe ),
                typeof( ExecutionersAxe ),
                typeof( TwoHandedAxe ),
                typeof( Katana ),
                typeof( Scimitar ),
                typeof( BattleAxe ),
                typeof( Bardiche ),
                typeof( Broadsword ),
                typeof( VikingSword ),
                typeof( DoubleAxe ),
                typeof( Halberd ),
                typeof( Cutlass ),
                typeof( Longsword ),
                typeof( Spear ),
                typeof( WarFork ),
                typeof( ShortSpear ),
                typeof( Kryss ),
                typeof( Pitchfork ),
                typeof( Mace ),
                typeof( WarMace ),
                typeof( QuarterStaff ),
                typeof( HammerPick ),
                typeof( WarAxe ),
                typeof( Maul ),
                typeof( BlackStaff ),
                typeof( Club ),
                typeof( WarHammer ),
                typeof( GnarledStaff ),
                typeof( Bow ),
                typeof( Crossbow ),
                typeof( HeavyCrossbow )
			};

        public static Item RandomSwordWeapon()
        {
            return Construct(m_SwordWeaponTypes);
        }

        public static Type[] SwordWeaponTypes { get { return m_SwordWeaponTypes; } }
        private static Type[] m_SwordWeaponTypes = new Type[]
			{
				typeof( Axe ),
                typeof( ExecutionersAxe ),
                typeof( TwoHandedAxe ),
                typeof( Katana ),
                typeof( Scimitar ),
                typeof( BattleAxe ),
                typeof( Bardiche ),
                typeof( Broadsword ),
                typeof( VikingSword ),
                typeof( DoubleAxe ),
                typeof( Halberd ),
                typeof( Cutlass ),
                typeof( Longsword ),
			};

        public static Item RandomFencingWeapon()
        {
            return Construct(m_FencingWeaponTypes);
        }

        public static Type[] FencingWeaponTypes { get { return m_FencingWeaponTypes; } }
        private static Type[] m_FencingWeaponTypes = new Type[]
			{
				typeof( Spear ),
                typeof( WarFork ),
                typeof( ShortSpear ),
                typeof( Kryss ),
                typeof( Pitchfork )
			};

        public static Item RandomMacingWeapon()
        {
            return Construct(m_MacingWeaponTypes);
        }

        public static Type[] MacingWeaponTypes { get { return m_MacingWeaponTypes; } }
        private static Type[] m_MacingWeaponTypes = new Type[]
			{
				typeof( Mace ),
                typeof( WarMace ),
                typeof( QuarterStaff ),
                typeof( HammerPick ),
                typeof( WarAxe ),
                typeof( Maul ),
                typeof( BlackStaff ),
                typeof( Club ),
                typeof( WarHammer ),
                typeof( GnarledStaff ),
			};

        public static Type[] RangedWeaponTypes { get { return m_RangedWeaponTypes; } }
        private static Type[] m_RangedWeaponTypes = new Type[]
			{
				typeof( Bow ),
                typeof( Crossbow ),
                typeof( HeavyCrossbow )
			};


        public static Type[] ArmorTypes { get { return m_ArmorTypes; } }
        private static Type[] m_ArmorTypes = new Type[]
			{
				typeof( BoneArms ),				typeof( BoneChest ),			typeof( BoneGloves ),
				typeof( BoneLegs ),				typeof( BoneHelm ),				typeof( ChainChest ),
				typeof( ChainLegs ),			typeof( ChainCoif ),			typeof( Bascinet ),
				typeof( CloseHelm ),			typeof( Helmet ),				typeof( NorseHelm ),
				typeof( OrcHelm ),				typeof( FemaleLeatherChest ),	typeof( LeatherArms ),
				typeof( LeatherBustierArms ),	typeof( LeatherChest ),			typeof( LeatherGloves ),
				typeof( LeatherGorget ),		typeof( LeatherLegs ),			typeof( LeatherShorts ),
				typeof( LeatherSkirt ),			typeof( LeatherCap ),			typeof( FemalePlateChest ),
				typeof( PlateArms ),			typeof( PlateChest ),			typeof( PlateGloves ),
				typeof( PlateGorget ),			typeof( PlateHelm ),			typeof( PlateLegs ),
				typeof( RingmailArms ),			typeof( RingmailChest ),		typeof( RingmailGloves ),
				typeof( RingmailLegs ),			typeof( FemaleStuddedChest ),	typeof( StuddedArms ),
				typeof( StuddedBustierArms ),	typeof( StuddedChest ),			typeof( StuddedGloves ),
				typeof( StuddedGorget ),		typeof( StuddedLegs )
			};

        public static BaseShield RandomShield()
        {
            return Construct(m_ShieldTypes) as BaseShield;
        }

        public static Type[] ShieldTypes { get { return m_ShieldTypes; } }
        private static Type[] m_ShieldTypes = new Type[]
			{
				typeof( BronzeShield ),			typeof( Buckler ),				typeof( HeaterShield ),
				typeof( MetalShield ),			typeof( MetalKiteShield ),		typeof( WoodenKiteShield ),
				typeof( WoodenShield )
			};       

        public static BaseInstrument RandomInstrument()
        {
            return Construct(m_InstrumentTypes) as BaseInstrument;
        }

        public static Type[] InstrumentTypes { get { return m_InstrumentTypes; } }
        private static Type[] m_InstrumentTypes = new Type[]
		{
			typeof( Drums ),				typeof( Harp ),					typeof( LapHarp ),
			typeof( Lute ),					typeof( Tambourine ),			typeof( TambourineTassel )
		};

        public static SpellScroll RandomScroll(int minIndex, int maxIndex, SpellbookType type)
        {
            Type[] types;

            switch (type)
            {
                default:
                case SpellbookType.Regular: types = m_RegularScrollTypes; break;
            }

            return Construct(types, Utility.RandomMinMax(minIndex, maxIndex)) as SpellScroll;
        }

        public static Type[] RegularScrollTypes { get { return m_RegularScrollTypes; } }
        private static Type[] m_RegularScrollTypes = new Type[]
			{
				typeof( ClumsyScroll ),			typeof( CreateFoodScroll ),		typeof( FeeblemindScroll ),		typeof( HealScroll ),
				typeof( MagicArrowScroll ),		typeof( NightSightScroll ),		typeof( ReactiveArmorScroll ),	typeof( WeakenScroll ),
				typeof( AgilityScroll ),		typeof( CunningScroll ),		typeof( CureScroll ),			typeof( HarmScroll ),
				typeof( MagicTrapScroll ),		typeof( MagicUnTrapScroll ),	typeof( ProtectionScroll ),		typeof( StrengthScroll ),
				typeof( BlessScroll ),			typeof( FireballScroll ),		typeof( MagicLockScroll ),		typeof( PoisonScroll ),
				typeof( TelekinisisScroll ),	typeof( TeleportScroll ),		typeof( UnlockScroll ),			typeof( WallOfStoneScroll ),
				typeof( ArchCureScroll ),		typeof( ArchProtectionScroll ),	typeof( CurseScroll ),			typeof( FireFieldScroll ),
				typeof( GreaterHealScroll ),	typeof( LightningScroll ),		typeof( ManaDrainScroll ),		typeof( RecallScroll ),
				typeof( BladeSpiritsScroll ),	typeof( DispelFieldScroll ),	typeof( IncognitoScroll ),		typeof( MagicReflectScroll ),
				typeof( MindBlastScroll ),		typeof( ParalyzeScroll ),		typeof( PoisonFieldScroll ),	typeof( SummonCreatureScroll ),
				typeof( DispelScroll ),			typeof( EnergyBoltScroll ),		typeof( ExplosionScroll ),		typeof( InvisibilityScroll ),
				typeof( MarkScroll ),			typeof( MassCurseScroll ),		typeof( ParalyzeFieldScroll ),	typeof( RevealScroll ),
				typeof( ChainLightningScroll ), typeof( EnergyFieldScroll ),	typeof( FlamestrikeScroll ),	typeof( GateTravelScroll ),
				typeof( ManaVampireScroll ),	typeof( MassDispelScroll ),		typeof( MeteorSwarmScroll ),	typeof( PolymorphScroll ),
				typeof( EarthquakeScroll ),		typeof( EnergyVortexScroll ),	typeof( ResurrectionScroll ),	typeof( SummonAirElementalScroll ),
				typeof( SummonDaemonScroll ),	typeof( SummonEarthElementalScroll ),	typeof( SummonFireElementalScroll ),	typeof( SummonWaterElementalScroll )
			};

        public static Item RandomReagent()
        {
            return Construct(m_RegTypes);
        }

        public static Type[] RegTypes { get { return m_RegTypes; } }
        private static Type[] m_RegTypes = new Type[]
			{
				typeof( BlackPearl ),			typeof( Bloodmoss ),			typeof( Garlic ),
				typeof( Ginseng ),				typeof( MandrakeRoot ),			typeof( Nightshade ),
				typeof( SulfurousAsh ),			typeof( SpidersSilk )
			};

        public static Item RandomPotion()
        {
            return Construct(m_PotionTypes);
        }

        public static Type[] PotionTypes { get { return m_PotionTypes; } }
        private static Type[] m_PotionTypes = new Type[]
			{
				typeof( AgilityPotion ),		typeof( StrengthPotion ),		typeof( RefreshPotion ),
				typeof( LesserCurePotion ),		typeof( LesserHealPotion ),		typeof( LesserPoisonPotion )
			};

        public static Item RandomGreaterPotion()
        {
            return Construct(m_GreaterPotionTypes);
        }        

        private static Type[] m_GreaterPotionTypes = new Type[]
			{
				typeof( GreaterAgilityPotion ),		typeof( GreaterStrengthPotion ),		typeof( TotalRefreshPotion ),
				typeof( GreaterCurePotion ),		typeof( GreaterHealPotion )
			};

        public static Item RandomGem()
        {
            return Construct(m_GemTypes);
        }

        public static Type[] GemTypes { get { return m_GemTypes; } }
        private static Type[] m_GemTypes = new Type[]
			{
				typeof( Amber ),				typeof( Amethyst ),				typeof( Citrine ),
				typeof( Diamond ),				typeof( Emerald ),				typeof( Ruby ),
				typeof( Sapphire ),				typeof( StarSapphire ),			typeof( Tourmaline )
			};
        

		private static Type[] m_StatueTypes = new Type[]
		{
			typeof( StatueSouth ),			typeof( StatueSouth2 ),			typeof( StatueNorth ),
			typeof( StatueWest ),			typeof( StatueEast ),			typeof( StatueEast2 ),
			typeof( StatueSouthEast ),		typeof( BustSouth ),			typeof( BustEast )
		};

		public static Type[] StatueTypes{ get{ return m_StatueTypes; } }

		private static Type[] m_ClothingTypes = new Type[]
			{
				typeof( Cloak ),				
				typeof( Bonnet ),               typeof( Cap ),		            typeof( FeatheredHat ),
				typeof( FloppyHat ),            typeof( JesterHat ),			typeof( Surcoat ),
				typeof( SkullCap ),             typeof( StrawHat ),	            typeof( TallStrawHat ),
				typeof( TricorneHat ),			typeof( WideBrimHat ),          typeof( WizardsHat ),
				typeof( BodySash ),             typeof( Doublet ),              typeof( Boots ),
				typeof( FullApron ),            typeof( JesterSuit ),           typeof( Sandals ),
				typeof( Tunic ),				typeof( Shoes ),				typeof( Shirt ),
				typeof( Kilt ),                 typeof( Skirt ),				typeof( FancyShirt ),
				typeof( FancyDress ),			typeof( ThighBoots ),			typeof( LongPants ),
				typeof( PlainDress ),           typeof( Robe ),					typeof( ShortPants ),
				typeof( HalfApron )
			};
		public static Type[] ClothingTypes{ get{ return m_ClothingTypes; } }

		private static Type[] m_HatTypes = new Type[]
			{
				typeof( SkullCap ),			typeof( Bandana ),		typeof( FloppyHat ),
				typeof( Cap ),				typeof( WideBrimHat ),	typeof( StrawHat ),
				typeof( TallStrawHat ),		typeof( WizardsHat ),	typeof( Bonnet ),
				typeof( FeatheredHat ),		typeof( TricorneHat ),	typeof( JesterHat )
			};

		public static Type[] HatTypes{ get{ return m_HatTypes; } }

		#region Accessors


		
		public static BaseClothing RandomClothing()
		{
			return Construct( m_ClothingTypes ) as BaseClothing;
		}		

		public static BaseWeapon RandomRangedWeapon()
		{
			return Construct( m_RangedWeaponTypes ) as BaseWeapon;
		}		

		public static BaseWeapon RandomWeapon()
		{
            // give a 25% chance for each weapon type
            int seed = Utility.Random(100);

            if (seed <= 24)
                return RandomRangedWeapon();
            else if (seed <= 49)
                return RandomMacingWeapon() as BaseWeapon;
            else if (seed <= 74)
                return RandomFencingWeapon() as BaseWeapon;
            else
                return RandomSwordWeapon() as BaseWeapon;

			return Construct( m_WeaponTypes ) as BaseWeapon;
		}

		public static BaseArmor RandomArmor()
		{
			return Construct( m_ArmorTypes ) as BaseArmor;
		}

		public static BaseHat RandomHat()
		{
			return Construct( m_HatTypes ) as BaseHat;
		}	

		public static BaseArmor RandomArmorOrShield()
		{
			return Construct( m_ArmorTypes, m_ShieldTypes ) as BaseArmor;
		}
	

		public static Item RandomPossibleReagent()
		{
			return Construct( m_RegTypes );
		}		

		public static Item RandomStatue()
		{
			return Construct( m_StatueTypes );
		}		
		#endregion

		#region Construction methods
		public static Item Construct( Type type )
		{
			try
			{
				return Activator.CreateInstance( type ) as Item;
			}
			catch
			{
				return null;
			}
		}

		public static Item Construct( Type[] types )
		{
			if ( types.Length > 0 )
				return Construct( types, Utility.Random( types.Length ) );

			return null;
		}

		public static Item Construct( Type[] types, int index )
		{
			if ( index >= 0 && index < types.Length )
				return Construct( types[index] );

			return null;
		}

		public static Item Construct( params Type[][] types )
		{
			int totalLength = 0;

			for ( int i = 0; i < types.Length; ++i )
				totalLength += types[i].Length;

			if ( totalLength > 0 )
			{
				int index = Utility.Random( totalLength );

				for ( int i = 0; i < types.Length; ++i )
				{
					if ( index >= 0 && index < types[i].Length )
						return Construct( types[i][index] );

					index -= types[i].Length;
				}
			}

			return null;
		}
		#endregion
	}
}