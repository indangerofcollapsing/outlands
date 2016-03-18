using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;

namespace Server.Achievements
{
    public interface AchievementSystemBase // derived from in the Scripts/Custom/AchievementSystem.cs
    {
        void TickProgress(Mobile mobile, AchievementTriggers trigger); // single increment
        void TickProgressMulti(Mobile mobile, AchievementTriggers trigger, int amount); // optimized version for large increments (smelting ingots for example)
        void LoadFromXML();
    }

    public class DummyImpl : AchievementSystemBase
    {
        public void TickProgress(Mobile mobile, AchievementTriggers trigger) { }
        public void TickProgressMulti(Mobile mobile, AchievementTriggers trigger, int amount) { }
        public void LoadFromXML() { }
    }

    public static class AchievementSystem
    {
        public static AchievementSystemBase Instance = new DummyImpl();
    }


    // Triggers are passed from gamecode into the achievement system.
    // The connection between a trigger and its achievement is hard coded and is highly dependent on the number values here.

    //////////////////////////////////////////////////////////////////////////
    // IMPORTANT : The per-player state is serialized using indices, meaning...
    //				YOU CAN NOT REMOVE A TRIGGER! SERVER WILL CRASH ON STARTUP!
    //				DO NOT REMOVE TRIGGERS (!)
    //////////////////////////////////////////////////////////////////////////
    public enum AchievementTriggers
    {
        Trigger_AncientWyrmKilled = 0,//														DONE
        Trigger_50kInBank = 1,	// Bank holds at least 50k                                      DONE
        Trigger_100kInBank = 2,	// 100k in bank                                                 DONE
        Trigger_250kInBank = 3,	// 250k in bank                                                 DONE
        Trigger_500kInBank = 4,	// 500k in bank                                                 DONE
        Trigger_1MillionInBank = 5,	// 1mill in bank                                            DONE
        Trigger_KilledBlueDefenseless = 6,	// Killing defenseless blue player target 
        Trigger_PlayerCorpseCut = 7,	// Cutting a red player corpse as non-red               DONE
        Trigger_ExploreHedgeMaze = 8,//															
        Trigger_ExploreWind = 9,        //														DONE
        Trigger_ExploreBritain = 10,//															DONE
        Trigger_ExploreBuccsDen = 11,//															DONE
        Trigger_ExploreCove = 12,//																DONE
        Trigger_ExploreJhelom = 13,//															DONE
        Trigger_ExploreMagincia = 14,//															DONE
        Trigger_ExploreMinoc = 15,//															DONE
        Trigger_ExploreMoonglow = 16,//															DONE
        Trigger_ExploreNujelm = 17,//															DONE
        Trigger_ExploreOcclo = 18,//															DONE
        Trigger_ExploreSerpentsHold = 19,//														DONE
        Trigger_ExploreSkaraBrae = 20,//														DONE
        Trigger_ExploreTrinsic = 21,//															DONE
        Trigger_ExploreVesper = 22,//															DONE
        Trigger_ExploreYew = 23,//																DONE
        Trigger_ExploreDespise = 24,//															DONE
        Trigger_ExploreDeceit = 25,//															DONE
        Trigger_ExploreDestard = 26,//															DONE
        Trigger_ExploreWrong = 27,//															DONE
        Trigger_ExploreCovetous = 28,//															DONE
        Trigger_ExploreShame = 29,//															DONE
        Trigger_ExploreHythloth = 30,//															DONE
        Trigger_ExploreFireDungeon = 31,//														DONE
        Trigger_ExploreIceDungeon = 32,//														DONE
        Trigger_ExploreTerathanKeep = 33,//														DONE
        Trigger_ExploreEveryRegion = 34,
        Trigger_ExploreEveryCity = 35,
        Trigger_ExploreEveryDungeon = 36,

        Trigger_GetMurderCount = 37, // reported as a murderer									DONE                              
        Trigger_AirElementalKilled = 38, //														DONE
        Trigger_EarthElementalKilled = 39, //													DONE
        Trigger_KillRabbitDogCat = 40, //														DONE
        Trigger_KillTamedRabbDogCatBird = 41,//													DONE
        Trigger_OgreKilled = 42,//																DONE
        Trigger_OrcKilled = 43, //																DONE
        Trigger_PoisonElementalKilled = 44,//													DONE
        Trigger_DaemonKilled = 45,//															DONE
        Trigger_BalronKilled = 46,//															DONE
        Trigger_JWilsonKilled = 47,//															DONE
        Trigger_BloodElementalKilled = 48,//													DONE
        Trigger_GargoyleKilled = 49,//															DONE
        Trigger_LichKilled = 50,//																DONE
        Trigger_AncientLichKilled = 51,//														DONE
        Trigger_DragonKilled = 52,//															DONE
        Trigger_BrigandKilled = 53,//															DONE
        Trigger_WaterElementalKilled = 54,//													DONE
        Trigger_FireElementalKilled = 55,//														DONE
        Trigger_RareRatman = 56,//																DONE
        Trigger_RareOgre = 57,//																DONE
        Trigger_RareOrc = 58,//																	DONE
        Trigger_RareDemon = 59,//																DONE
        Trigger_RareTroll = 60,//																DONE
        Trigger_RareSilverSerpent = 61,//														DONE
        Trigger_RareSnake = 62,//																DONE
        Trigger_RareHarpy = 63,//																DONE
        Trigger_RareWolf = 64,//																DONE
        Trigger_RareSkeleton = 65,//															DONE
        Trigger_RareZombie = 66,//																DONE

        Trigger_CraftArmor = 67, //																DONE
        Trigger_CraftWeapon = 68,//																DONE
        Trigger_MarkCraftItem = 69,//															DONE
        Trigger_SmeltItem = 70,//																DONE
        Trigger_HammerBroke = 71,//																DONE
        Trigger_PickaxeBroke = 72,//															DONE
        Trigger_TongsBroke = 73,//																DONE
        Trigger_SewingKitBroke = 74,//															DONE
        Trigger_MortarPestleBroke = 75,//														DONE
        Trigger_ScribesPenBroke = 76,//															DONE
        Trigger_TinkerToolsBroke = 77,//														DONE

        Trigger_MineDullCopper = 78,//															DONE
        Trigger_MineCopper = 79,//																DONE
        Trigger_MineBronze = 80,//																DONE
        Trigger_MineShadow = 81,//																DONE
        Trigger_MineGold = 82,//																DONE
        Trigger_MineAgapite = 83,//																DONE
        Trigger_MineVerite = 84,//																DONE
        Trigger_MineValorite = 85,//															DONE
        Trigger_MineIron = 86,//																DONE

        Trigger_CreateIngotDullCopper = 87,//													DONE
        Trigger_CreateIngotCopper = 88,//														DONE
        Trigger_CreateIngotBronze = 89,//														DONE
        Trigger_CreateIngotShadow = 90,//														DONE
        Trigger_CreateIngotGold = 91,//															DONE
        Trigger_CreateIngotAgapite = 92,//														DONE
        Trigger_CreateIngotVerite = 93,//														DONE
        Trigger_CreateIngotValorite = 94,//														DONE
        Trigger_FailMiningAttempt = 95,//														DONE	 // NOTE!!! This is actually implemented as "failed any type of harvest"

        Trigger_CatchPrizedFish = 96,//															DONE
        Trigger_CatchWondrousFish = 97,//														DONE
        Trigger_CatchTrulyRareFish = 98,//														DONE
        Trigger_CatchHighlyPeculiarFish = 99,//													DONE
        Trigger_CatchMIB = 100,//																DONE
        Trigger_CatchBoots = 101,

        Trigger_CreateHealPotion = 102,//														DONE
        Trigger_CreateCurePotion = 103,//														DONE
        Trigger_CreatePoisonPotion = 104,//														DONE
        Trigger_CreateRefreshPotion = 105,//													DONE
        Trigger_CreateStrengthPotion = 106,//													DONE
        Trigger_CreateAgilityPotion = 107,//													DONE
        Trigger_CreateDeadlyPoisonPotion = 108,//												DONE

        Trigger_SkinAnimal = 109,               //												DONE
        Trigger_UsePortal = 110,//																DONE
        Trigger_ResurrectAnimal = 111, //														DONE
        Trigger_CureAnimalWithBandages = 112,//													DONE
        Trigger_HealAnimalUsingBandages = 113,//												DONE
        Trigger_KillAnyTamedAnimal = 114,//														DONE

        Trigger_PlayLuteWell = 115,//															DONE
        Trigger_PlayDrumWell = 116,//															DONE
        Trigger_PlayTambourineWell = 117,//														DONE
        Trigger_PlayHarpWell = 118,//															DONE

        Trigger_TameChicken = 119,//															DONE
        Trigger_TameBear = 120,//																DONE
        Trigger_TameDog = 121,//																DONE
        Trigger_TameTimberWolf = 122,//															DONE
        Trigger_TameHind = 123,//																DONE
        Trigger_TameCat = 124,//																DONE
        Trigger_TameGiantToad = 125,//															DONE
        Trigger_TameJackRabbit = 126,//															DONE
        Trigger_TameRabbit = 127,//																DONE
        Trigger_TameRat = 128,//																DONE
        Trigger_TameGorilla = 129,//															DONE
        Trigger_TameWalrus = 130,//																DONE
        Trigger_TamePolarBear = 131,//															DONE

        Trigger_Donate80to250GoldToNPC = 132,//													DONE
        Trigger_Donate250to500GoldToNPC = 133,//												DONE
        Trigger_DonateOver500GoldToNPC = 134,//													DONE

        Trigger_TrainCraftingSkillFromNPC = 135,//												DONE
        Trigger_HealPlayerUnder300Skill = 136,//                                                DONE
        Trigger_CurePlayerUnder300Skill = 137,//                                                DONE
        Trigger_BlessPlayerUnder300Skill = 138,//                                               DONE

        Trigger_CreateAGuild = 139,//                                                           DONE
        Trigger_JoinAGuild = 140,//                                                             DONE

        Trigger_GetDrunk = 141,//                                                               DONE

        Trigger_CompleteAllBlacksmithAchievements = 142,//                                      DONE (MASTERY)

        Trigger_Reach100InAlchemy = 143,//                                                DONE
        Trigger_Reach100InAnatomy = 144,//                                                DONE
        Trigger_Reach100InAnimalLore = 145,//                                                DONE
        Trigger_Reach100InItemID = 146,//                                                DONE
        Trigger_Reach100InArmsLore = 147,//                                                DONE
        Trigger_Reach100InParry = 148,//                                                DONE
        Trigger_Reach100InBegging = 149,//                                                DONE
        Trigger_Reach100InBlacksmith = 150,//                                                DONE
        Trigger_Reach100InFletching = 151,//                                                DONE
        Trigger_Reach100InPeacemaking = 152,//                                                DONE
        Trigger_Reach100InCamping = 153,//                                                DONE
        Trigger_Reach100InCarpentry = 154,//                                                DONE
        Trigger_Reach100InCartography = 155,//                                                DONE
        Trigger_Reach100InCooking = 156,//                                                DONE
        Trigger_Reach100InDetectHidden = 157,//                                                DONE
        Trigger_Reach100InDiscordance = 158,//                                                DONE
        Trigger_Reach100InEvalInt = 159,//                                                DONE
        Trigger_Reach100InHealing = 160,//                                                DONE
        Trigger_Reach100InFishing = 161,//                                                DONE
        Trigger_Reach100InForensics = 162,//                                                DONE
        Trigger_Reach100InHerding = 163,//                                                DONE
        Trigger_Reach100InHiding = 164,//                                                DONE
        Trigger_Reach100InProvocation = 165,//                                                DONE
        Trigger_Reach100InInscribe = 166,//                                                DONE
        Trigger_Reach100InLockpicking = 167,//                                                DONE
        Trigger_Reach100InMagery = 168,//                                                DONE
        Trigger_Reach100InMagicResist = 169,//                                                DONE
        Trigger_Reach100InTactics = 170,//                                                DONE
        Trigger_Reach100InSnooping = 171,//                                                DONE
        Trigger_Reach100InMusicianship = 172,//                                                DONE
        Trigger_Reach100InPoisoning = 173,//                                                DONE
        Trigger_Reach100InArchery = 174,//                                                DONE
        Trigger_Reach100InSpiritSpeak = 175,//                                                DONE
        Trigger_Reach100InStealing = 176,//                                                DONE
        Trigger_Reach100InTailoring = 177,//                                                DONE
        Trigger_Reach100InAnimalTaming = 178,//                                                DONE
        Trigger_Reach100InTasteID = 179,//                                                DONE
        Trigger_Reach100InTinkering = 180,//                                                DONE
        Trigger_Reach100InTracking = 181,//                                                DONE
        Trigger_Reach100InVeterinary = 182,//                                                DONE
        Trigger_Reach100InSwords = 183,//                                                DONE
        Trigger_Reach100InMacing = 184,//                                                DONE
        Trigger_Reach100InFencing = 185,//                                                DONE
        Trigger_Reach100InWrestling = 186,//                                                DONE
        Trigger_Reach100InLumberjacking = 187,//                                                DONE
        Trigger_Reach100InMining = 188,//                                                DONE
        Trigger_Reach100InMeditation = 189,//                                                DONE
        Trigger_Reach100InStealth = 190,//                                                DONE
        Trigger_Reach100InRemoveTrap = 191,//                                                DONE

        Trigger_CompleteAllTame100Achs = 192,//                                                DONE (MASTERY)
        Trigger_TameAnyAnimal = 193,//                                                DONE
        Trigger_TameHellHound = 194,//                                                DONE

        Trigger_EscortNPC = 195,//                                                DONE
        Trigger_KillEscortedNPC = 196,//                                                DONE

        Trigger_PlaceSmallHouse = 197,//                                                DONE
        Trigger_PlaceMediumHouse = 198,//                                                DONE
        Trigger_PlaceTower = 199,//                                                DONE
        Trigger_PlaceKeep = 200,//                                                DONE
        Trigger_PlaceCastle = 201,//                                                DONE

        Trigger_MakeAnyPotion = 202,//                                                DONE
        Trigger_CompleteAllAlchemyAchs = 203,//                                                DONE (MASTERY)

        Trigger_SellItem1to9gold = 204,//                                                DONE
        Trigger_SellItem10to99gold = 205,//                                                DONE
        Trigger_SellItem100to199gold = 206,//                                                DONE
        Trigger_SellItemOver199gold = 207,//                                                DONE

        Trigger_CompleteAllFishingAchs = 208,//												DONE
        Trigger_CatchFish = 209,//												DONE

        Trigger_1dayOldAccount = 210,//												DONE
        Trigger_7dayOldAccount = 211,//												DONE
        Trigger_30dayOldAccount = 212,//												DONE
        Trigger_90dayOldAccount = 213,//												DONE
        Trigger_180dayOldAccount = 214,//												DONE
        Trigger_365dayOldAccount = 215,//												DONE

        Trigger_GargoyleKilled_TIMED = 216,//												DONE
        Trigger_HellhoundKilled_TIMED = 217,//												DONE
        Trigger_ChickenKilled_TIMED = 218,//												DONE
        Trigger_ShearSheep_TIMED = 219,//												DONE
        Trigger_KillMongbat_TIMED = 220,//												DONE
        Trigger_KillSnakeWithClub_TIMED = 221,//												DONE

        Trigger_CompleteAllTimeBaseAchs = 222,//												DONE (MASTERY)

        // Arena
        Trigger_ArenaWin1v1 = 223,//												DONE
        Trigger_ArenaWin2v2 = 224,//												DONE
        Trigger_ArenaWin3v3 = 225,//												DONE
        Trigger_ArenaChampion1v1 = 226,//												DONE
        Trigger_Reach1750ArenaRanking = 227,//												DONE
        Trigger_Reach2000ArenaRanking = 228,//												DONE
        Trigger_ArenaLoose1v1 = 229,//												DONE
        Trigger_ArenaLoose2v2 = 230,//												DONE
        Trigger_ArenaLoose3v3 = 231,//												DONE
        Trigger_ArenaChampion2v2 = 232,//												DONE
        Trigger_ArenaChampion3v3 = 233,//												DONE
        Trigger_CompleteAllArenaAchs = 234,//												DONE (MASTERY)


        // OCB
        Trigger_LightBrazier = 235,//												DONE				
        Trigger_DouseBrazier = 236,//												DONE
        Trigger_StealEnemyTreasury = 237,//												DONE
                                         //Trigger_KillEnemyOfOrder		= 238,//												DONE
                                         //Trigger_KillEnemyOfBalance	= 239,//											    DONE
        Trigger_KillMiltiiaEnemy = 240,//												DONE

        // Casino
        Trigger_Gamble = 241,//                                                

        // Fire Dungeon
        Trigger_BaronVonGeddon = 242,//
        Trigger_Ogres = 243,//
        Trigger_RubyDragon = 244,//
        Trigger_ImmortalFlame = 245,//
        Trigger_FireDungeon = 246,//

        // Slayer Armor
        Trigger_HythlothArmor = 247,//
        Trigger_ShameArmor = 248,//
        Trigger_DestardArmor = 249,//
        Trigger_DeceitArmor = 250,//
        Trigger_SlayerArmor = 251,//

        // Daily
        Trigger_DailyAchievement = 252,//

        // Battlegrounds
        Trigger_CaveLakeBattleground = 253,
        Trigger_TreasuryBattleground = 254,

        // ctf bgs
        Trigger_VolcanoBattleground = 255,
        Trigger_StarryExpanseBattleground = 256,
        Trigger_FungalBattleground = 257,
        Trigger_OasisBattleground = 258,

        Trigger_SanctuaryBattleground = 259,

        //Expansion Dungeon Armors
        Trigger_CovetousArmor = 260,
        Trigger_WrongArmor = 261,
        Trigger_DespiseArmor = 262,
        Trigger_IceArmor = 263,
        Trigger_FireArmor = 264,

        //Lumberjacking stuff
        Trigger_HarvestRegularWood = 265,
        Trigger_HarvestOakWood = 266,//															DONE
        Trigger_HarvestAshWood = 267,//																DONE
        Trigger_HarvestYewWood = 268,//																DONE
        Trigger_HarvestBloodWood = 269,//																DONE
        Trigger_HarvestHeartWood = 270,//																DONE
        Trigger_HarvestFrostwood = 271,//																DONE

        //Pirates
        Trigger_SinkNpcBoats = 272,
        Trigger_SinkPlayerBoats = 273,
        Trigger_SinkOrcShips = 274,
        Trigger_SinkOrghereimShips = 275,
        Trigger_SinkPirateShips = 276,
        Trigger_SinkUndeadShips = 277,
        Trigger_SinkBritainNavyShips = 278,
        Trigger_SinkFishingShips = 279,
        Trigger_SinkSmallShips = 280,
        Trigger_SinkMediumShips = 281,
        Trigger_SinkLargeShips = 282,
        Trigger_SinkCarracks = 283,
        Trigger_SinkGalleon = 284,
        Trigger_KillTheDeepOne = 285,

        //UOACZ (Must Also Be Added into UOACZAchievements.cs)
        Trigger_UOACZKillZombies = 286,
        Trigger_UOACZCorruptWildlife = 287,
        Trigger_UOACZScoreFromKillingUndead = 288,
        Trigger_UOACZScoreFromKillingHumans = 289,
        Trigger_UOACZCorruptionAbilities = 290,
        Trigger_UOACZKillUndeadChampion = 291,
        Trigger_UOACZKillUndeadBoss = 292,
        Trigger_UOACZKillHumanChampion = 293,
        Trigger_UOACZKillHumanBoss = 294,
        Trigger_UOACZUseTunnels = 295,
        Trigger_UOACZMurderAnotherHuman = 296,
        Trigger_UOACZKillUndeadPlayer = 297,
        Trigger_UOACZKillHumanPlayer = 298,
        Trigger_UOACZRepairObjects = 299,
        Trigger_UOACZHumanVictory = 300,
        Trigger_UOACZUndeadVictory = 301,
        Trigger_UOACZNightwalker = 302,
        Trigger_UOACZMonsterTierLevel5 = 303,
        Trigger_UOACZConsumeBrains = 304,
        Trigger_UOACZPurchaseUpgradesWithoutShuffles = 305,
        Trigger_UOACZPurchaseDifferentHumanUpgrades = 306,
        Trigger_UOACZHighestSessionTotalScore = 307,
        Trigger_UOACZDamageObjects = 308,
        Trigger_UOACZCraftingSkill = 309,
        Trigger_UOACZHarvesting = 310,
        Trigger_UOACZScavenging = 311,
        Trigger_UOACZStarve = 312,
        Trigger_UOACZDieOfThirst = 313,
        Trigger_UOACZKillCreaturesWithFirefields = 314,
        Trigger_UOACZCauseUndeadTransformation = 315,
        Trigger_UOACZBuildAndPlaceFortifications = 316,
        Trigger_UOACZHumanSurvival1 = 317,
        Trigger_UOACZHumanSurvival2 = 318,
        Trigger_UOACZHumanSurvival3 = 319,
        Trigger_UOACZUndeadSwarmSize15 = 320,
        Trigger_UOACZUndeadAddToSwarm = 321,
        Trigger_UOACZSnaresTrapsNets = 322,
        Trigger_UOACZKillWithDifferentForms1 = 323,
        Trigger_UOACZKillWithDifferentForms2 = 324,
        Trigger_UOACZKillWithDifferentForms3 = 325,

        Trigger_NumTriggers,				// Always last
    };
}
