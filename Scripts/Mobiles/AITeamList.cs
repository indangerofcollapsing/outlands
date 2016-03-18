using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Factions;
using Server.Items;

namespace Server
{
    public class AITeamList
    {
        private Type[] m_Types;

        public Type[] Types
        {
            get { return m_Types; }
            set { m_Types = value; }
        }

        public AITeamList(Type[] types)
        {
            m_Types = types;
        }

        public static bool CheckTeam(object objFrom, object objTarget)
        {
            Type fromType = objFrom.GetType();
            Type targetType = objTarget.GetType();

            //Both the Same Type: Automatically Teammates
            if (fromType == targetType)
                return true;

            bool foundFrom = false;
            bool foundTarget = false;

            //For Each Team in List: Except Last Team (Which is Independents)
            for (int i = 0; i < (m_Teams.Length - 1); ++i)
            {
                AITeamList info = m_Teams[i];
                Type[] types = info.Types;

                foundFrom = false;
                foundTarget = false;

                //Check If From is In Current Team
                for (int j = 0; j < types.Length; ++j)
                {
                    if (fromType == types[j])
                    {
                        foundFrom = true;
                        break;
                    }
                }

                //Check If Target Is Also in Current Team
                if (foundFrom)
                {
                    for (int j = 0; j < types.Length; ++j)
                    {
                        if (targetType == types[j])
                        {
                            foundTarget = true;
                            break;
                        }
                    }
                }

                //Both From and Target in Same List (They are Teammates)
                if (foundFrom && foundTarget)
                    return true;
            }

            return false;
        }

        private static AITeamList[] m_Teams = new AITeamList[]
		{  
            //Undead
			new AITeamList(new Type[]
			{
				typeof( Zombie ),
                typeof( rZombie ),     
                typeof( ZombieMagi ),
                typeof( Mummy ),
                typeof( RottingCorpse ),                
                typeof( InterredGrizzle),
                typeof( Server.Engines.Quests.Paladin.PaladinQuestSkeleton ),
                typeof( Server.Engines.Quests.Paladin.PaladinQuestBoneKnight ),
                typeof( DeathKnight ),
                typeof( DeathKnightLord ),
                typeof( DeathKnightKing ),
                typeof( UndeadKnight ), 
                typeof( BoneKnight ),	
                typeof( Ghoul ),
                typeof( Skeleton ),
                typeof( rSkeleton ),   
	            typeof( RestlessSoul ),
                typeof( KhaldunRevenant ), 
                typeof( SpectralArmour ),	
                typeof( SkeletalKnight ),
				typeof( Cursed ),
                typeof( SpectralArmour ),
				typeof( Revenant ),
                typeof( PatchworkSkeleton ),
                typeof( EtherealWarrior ), 
                typeof( ShadowFiend ),
                typeof( Shade ),
                typeof( Wraith ),
                typeof( Spectre ),
                typeof( Server.Engines.Quests.Paladin.PaladinQuestBoneMagi ),
                typeof( BoneMagi ),	
                typeof( SkeletalMage ),
                typeof( SkeletalDragon ),
                typeof( ShadowWyrm ),
                typeof( BoneDemon ),
                typeof( Lich ),
                typeof( Server.Engines.Quests.Paladin.PaladinQuestLich ),  
                typeof( RevenantLion ),
                typeof( DemonKnight ),
                typeof( LichLord ),	
                typeof( AncientLich ),
	            typeof( Custom.Pirates.SkeletalCaptain ),
                typeof( Custom.Pirates.SkeletalCrewman ),
                typeof( Custom.Pirates.GhostShipNecromancer ),
                typeof( SkeletalDrake ),
                typeof( AncientNecromancer ),
                typeof( BloodyZombie ),
                typeof( CorruptReaver ),
                typeof( CorruptRunecaster ),
                typeof( CorruptWarmage ),
                typeof( Creeper ),
                typeof( DecayedZombie ),
                typeof( FlamingZombie ),
                typeof( HollowOne ),
                typeof( UndeadOgreLord ),
                typeof( RisenKnight ),
                typeof( RisenNoble ),
                typeof( RisenHonorGuard ),
                typeof( RisenRoyal ),
			} ),

            //Ogres, Trolls, Ettins
			new AITeamList(new Type[]
			{
                typeof( Ogre ),
				typeof( rOgre ),
		        typeof( SuperOgreLord ),		
                typeof( OgreLord ),	
                typeof( ArcticOgreLord ),
                typeof( Ettin ),
	            typeof( FrostTroll ),
                typeof( Troll ),
                typeof( BloodTroll ),
                typeof( rTroll ),
                typeof( SlimeTroll ),
                typeof( GrayTroll ),
                typeof( OgreMage ),
            } ),

            //Orcs
			new AITeamList(new Type[]
			{
                typeof( SpawnedOrcishLord ),
                typeof( rOrcCaptain ),
                typeof( BlackOrc ),
                typeof( Orc ),
			    typeof( OrcBomber ),		
                typeof( OrcBrute ),		
                typeof( OrcCaptain ),
			    typeof( OrcishLord ),		
                typeof( OrcishMage ),
                typeof( DespiseOrc ),
                typeof( ElderMojoka ),
                typeof( OrcishExecutioner ),
                typeof( OrcishGrunt ),
                typeof( OrcishMaurk ),
                typeof( OrcishPeon ),
                typeof( OrcishScout ),
                typeof( OrcishSurjin ),
                typeof( OrcMojoka ),
                typeof( FrostOrc ),
                typeof( FrostOrcLord ),
                typeof( FrostOrcMage ), 
                typeof( OrcWarboss ),
                typeof( ElderWardr ), 
            } ),

            //Gazers
			new AITeamList(new Type[]
			{               
               typeof( GazerLarva ),
               typeof( Gazer ),
               typeof( ElderGazer ),
               typeof( MercuryGazer ),               
            } ),

            //Slimes
			new AITeamList(new Type[]
			{
               typeof( GreaterSlime ),
               typeof( PoisonousSlime ),
               typeof( AncientSlime ),
               typeof( CorrosiveSlime ),
               typeof( FrostOoze ),
               typeof( Jwilson ),	
               typeof( Slime ),
               typeof( Slime2 ),
               typeof( Slime3 ),
               typeof( Slime4 ),
               typeof( Slime5 ),
               typeof( Slime6 ),
               typeof( Slime7 ),
               typeof( Slime8 ),
            } ),

            //Cold Elementals
			new AITeamList(new Type[]
			{
                typeof( IceElemental ),
                typeof( SnowElemental ),
                typeof( ElderSnowElemental ), 
                typeof( ElderIceElemental ), 
            } ),

            //Golem
            new AITeamList(new Type[]
			{
               typeof( GolemController ),
               typeof( Golem )               
            } ),

            //Earth Elementals
			new AITeamList(new Type[]
			{       
               typeof( EarthElemental ),               
               typeof( ValoriteElemental ),
               typeof( VeriteElemental ), 
               typeof( AgapiteElemental ),
               typeof( BronzeElemental ),
               typeof( CopperElemental ),
               typeof( DullCopperElemental ),
               typeof( GoldenElemental ),
               typeof( ShadowIronElemental ),
            } ),

            //Evil Mage
			new AITeamList(new Type[]
			{                
                typeof( EvilMage ),			
                typeof( EvilMageLord ),
            } ),

            //Black Solen
			new AITeamList(new Type[]
			{
                typeof( BlackSolenInfiltratorQueen ),
                typeof( BlackSolenInfiltratorWarrior ),
				typeof( BlackSolenQueen ),	
                typeof( BlackSolenWarrior ), 
                typeof( BlackSolenWorker ),
            } ),

            //Red Solen
			new AITeamList(new Type[]
			{
                typeof( RedSolenInfiltratorQueen ), 
                typeof( RedSolenInfiltratorWarrior ), 
                typeof( RedSolenQueen ),
				typeof( RedSolenWarrior ),	
                typeof( RedSolenWorker ),
            } ),

            //Plants
			new AITeamList(new Type[]
			{
                typeof( BogThing ),				
                typeof( PlagueBeast ),
                typeof( Quagmire ),	
                typeof( PlagueSpawn ),
                typeof( SwampTentacle ),
	            typeof( WhippingVine ),
                typeof( Corpser ),
	            typeof( Reaper ),
                typeof( EvilOak ),
            } ),

            //Beetles
            new AITeamList(new Type[]
			{
                typeof( AntLion ), 
                typeof( Beetle ),
                typeof( RuneBeetle ),
                typeof( DeathwatchBeetle ),
                typeof( DeathwatchBeetleHatchling ),
                typeof( FireBeetle ),
                typeof( EtherealBeetle),
            } ),

            //SummonedDrones
            new AITeamList(new Type[]
			{
                typeof( EnergyVortex ),
                typeof( BladeSpirits )
            } ),

            //Spiders
            new AITeamList(new Type[]
			{
                typeof( FrostSpider ),	
                typeof( GiantSpider ),
		        typeof( GiantBlackWidow ),                    	
		        typeof( DreadSpider ),
                typeof( rGiantSpider ),               
            } ),

            //Gargoyles
            new AITeamList(new Type[]
			{
               	typeof( FireGargoyle ),
                typeof( Gargoyle ),
	            typeof( EnslavedGargoyle ),
                typeof( StoneGargoyle ),
                typeof( GargoyleDestroyer ),
                typeof( GargoyleEnforcer ),
                typeof( rGargoyle ),
            } ),

            //Snakes
            new AITeamList(new Type[]
			{
               	typeof( IceSerpent ),
	            typeof( GiantSerpent ),
                typeof( IceSnake ),
	            typeof( LavaSerpent ),
                typeof( LavaSnake ),
                typeof( Snake ),
                typeof( rSnake ),
                typeof( rSilverSerpent ),
                typeof( SilverSerpent ), 
                typeof( SeaSerpent ),
                typeof( DeepSeaSerpent ),
            } ),

            //Frogs
            new AITeamList(new Type[]
			{
               typeof( BullFrog ),               
               typeof( GiantToad ),
            } ),

            //Rats
            new AITeamList(new Type[]
			{
                typeof( Ratman ),
			    typeof( rRatman ),
                typeof( RatmanArcher ),		
                typeof( RatmanMage ),
                typeof( Rat ),
	            typeof( Sewerrat ),
	            typeof( GiantRat ),	
            } ),

            //Giants
            new AITeamList(new Type[]
			{
                typeof( Titan ), 
	            typeof( Cyclops ), 	
                typeof( CyclopsShaman ),
 	            typeof( GreaterCyclops ), 	
            } ),

            //Daemons
            new AITeamList(new Type[]
			{
                typeof( ChaosDaemon ),
                typeof( Daemon ),
                typeof( IceFiend ),
                typeof( IceFiendLord ),
	            typeof( Balron ),
                typeof( ArcaneDaemon ),
                typeof( Moloch ), 
                typeof( rDaemon ), 
                typeof( SilverDaemon ),
                typeof( SilverDaemonLord ), 
                typeof( GoldenBalron ),
            } ),

            //Dragons
            new AITeamList(new Type[]
			{
                typeof( Dragon ),			
                typeof( Drake ),
                typeof( ShadowWyrm ),
	            typeof( SerpentineDragon ),   
                typeof( SkeletalDragon ),	
                typeof( SwampDragon ),
				typeof( ScaledSwampDragon ),								
                typeof( Wyvern ),
                typeof( WhiteWyrm ),
                typeof( AncientWinterWyrm ),
                typeof( AncientWyrm ), 
                typeof( AbyssDragon ),
                typeof( ChaosDragon ),
                typeof( DragonHandler ),                
                typeof( GreaterDragon ),
                typeof( OrderDragon ),	
                typeof( RubyDragon ),	
                typeof( TankDragon ),	
                typeof( EtherealSwampDragon ),
                typeof( RaidDrake ),
            } ),

            //Terathans
            new AITeamList(new Type[]
			{
                typeof( TerathanAvenger ),
				typeof( TerathanDrone ),	
                typeof( TerathanMatriarch ), 
                typeof( TerathanWarrior ),               
            } ),

            //Ophidians
            new AITeamList(new Type[]
			{
                typeof( OphidianArchmage ),
				typeof( OphidianMage ),		
                typeof( OphidianWarrior ),	
                typeof( OphidianMatriarch ),
				typeof( OphidianKnight ),
            } ),

            //Imps & Minions
            new AITeamList(new Type[]
			{
                typeof( Imp ),	
                typeof( HordeMinion ),
                typeof( Mongbat ),
				typeof( StrongMongbat ),
	            typeof( GreaterMongbat ),
                typeof( CapturedHordeMinion ),
                typeof( MongbatLord ),
                typeof( Mobiles.Belial.ImpAdds ),
                typeof( Belial ),
            } ),

            //Air Elemental
            new AITeamList(new Type[]
			{
                typeof( AirElemental ),	
                typeof( RAirElemental ),	
            } ),

            //Fire Elemental
            new AITeamList(new Type[]
			{
                typeof( FireElemental ),
                typeof( RaidFireElemental ),
            } ),

            //Water Elemental
            new AITeamList(new Type[]
			{
                typeof( WaterElemental ),
            } ),

            //Sea Monsters
            new AITeamList(new Type[]
			{
                typeof( DeepSeaSerpent ),  
                typeof( Kraken ),	
                typeof( SeaSerpent ),
                typeof( Leviathan ),
            } ),

            //Juka
            new AITeamList(new Type[]
			{
                typeof( JukaLord ),			
                typeof( JukaMage ),
				typeof( JukaWarrior ),	
            } ),

            //Meer
            new AITeamList(new Type[]
			{
                typeof( MeerWarrior ),	
                typeof( MeerEternal ),		
                typeof( MeerMage ),
				typeof( MeerCaptain ),	
            } ),

            //Savage
            new AITeamList(new Type[]
			{
              	typeof( SavageShaman ),
                typeof( Savage ),
                typeof( SavageRider ),
                typeof( SavageRidgeback ),
            } ),

            //Harpy
            new AITeamList(new Type[]
			{
              	typeof( Harpy ),
	            typeof( rHarpy ),
                typeof( StoneHarpy ),
            } ),

            //Wisp
            new AITeamList(new Type[]
			{
              	typeof( ShadowWisp ),
                typeof( DarkWisp ),
                typeof( Wisp )
            } ),

            //Khaldun
            new AITeamList(new Type[]
			{
              	typeof( KhaldunZealot ),	
                typeof( KhaldunSummoner ),
                typeof( KhaldunRevenant ),
            } ),

            //Hiryu
            new AITeamList(new Type[]
			{
              	typeof( LesserHiryu ),		
                typeof( Hiryu )
            } ),  
            
            //Faction
            new AITeamList(new Type[]
			{
              	typeof( FactionKnight ),    
                typeof( FactionHenchman ),	
                typeof( FactionBerserker ),
                typeof( FactionMercenary ),	
                typeof( FactionSorceress ),	
                typeof( FactionWizard ), 
                typeof( FactionPaladin ),
            } ),

            //Guard
            new AITeamList(new Type[]
			{
                //typeof( DungeonGuardMelee ),
                //typeof( DungeonGuardRanged ),
            } ),

            //Sanguin
            new AITeamList(new Type[]
			{                
                typeof( SanguinAssassin ),
				typeof( SanguinConscript ),
				typeof( SanguinDefender ),
				typeof( SanguinHealer ),
                typeof( SanguinKnight ),
                typeof( SanguinMage),
                typeof( SanguinMedic ),
                typeof( SanguinMender ),
                typeof( SanguinProtector ),
                typeof( SanguinScout ),
                typeof( SanguinWizard ),
                typeof( SanguinHunter ),
                typeof( SanguinAlchemist ),                
            } ),

            //Svirfneblin
            new AITeamList(new Type[]
			{       
                typeof(SvirfneblinIllusionist),
                typeof(SvirfneblinRogue)
            } ),

            //Drow and Drider
            new AITeamList(new Type[]
			{       
                typeof(DrowBlackguard),
                typeof(DrowBlademaster),
                typeof(DrowSpellsinger),
                typeof(DriderWarrior),
                typeof(DriderSentinel),
                typeof(DriderHarbinger)
            } ),

            //Kuo-Toa, Salamander, and Giant Toad
            new AITeamList(new Type[]
			{       
                typeof(KuoToa),
                typeof(Salamander),
                typeof(GiantToad),
            } ),

            //Orghereim
            new AITeamList(new Type[]
			{       
                typeof(OrghereimBeastmaster),
                typeof(OrghereimCrone),
                typeof(OrghereimBoneMender),
                typeof(OrghereimIceCarl),
                typeof(OrghereimSage),
                typeof(OrghereimShieldMaiden),
                typeof(OrghereimShieldMother),
                typeof(OrghereimSwordThane),
                typeof(OrghereimTracker),
                typeof(OrghereimBowMaiden),
                typeof(Valkyrie),
            } ),

             //Atlantian
             new AITeamList(new Type[]
			{       
                 typeof(AtlanteanBattleMage),
                 typeof(AtlanteanWarden),
                 typeof(ElementalSeer),
                 typeof(EnergyElementalist),
                 typeof(FlameElementalist),
            } ),

            //Britain Naval Forces
            new AITeamList(new Type[]
			{   
                typeof(Custom.Pirates.BritainShipCaptain),
                typeof(Custom.Pirates.BritainSailor),
                typeof(Custom.Pirates.BritainMarine),
                typeof(Custom.Pirates.BritainShipCarpenter),
                typeof(Custom.Pirates.BritainShipSurgeon),
            } ),

            //Pirate
            new AITeamList(new Type[]
			{   
                 typeof(Custom.Pirates.OceanPirateCaptain),
                 typeof(Custom.Pirates.OceanPirate),
                 typeof(Custom.Pirates.PirateSawbones),
                 typeof(Custom.Pirates.PirateShipCarpenter),
            } ),

            //Criminal Humans
            new AITeamList(new Type[]
			{   
                 typeof(TombRaider),
                 typeof(GraveRobber),
                 typeof(Poacher),
                 typeof(Thug),
                 typeof(Bootlegger),
                 typeof(Smuggler),
            } ),

            //Arboreal / Summer Festival
            new AITeamList(new Type[]
			{
                 typeof(Myconid),
                 typeof(MyconidTallstalk),
                 typeof(WoodlandSprite),
                 typeof(ElderWoodlandSprite),
                 typeof(ArborealMyconid),
                 typeof(EarthlyTendril),
                 typeof(TreeStalker),
                 typeof(Ent),
                 typeof(WildOne),
                 typeof(TreeOfLife),
                 typeof(WoodlandDruid),
            } ),            

            //UOACZ Undead
            new AITeamList(new Type[]
			{
                 typeof(UOACZBloodyZombie),
                 typeof(UOACZCorpseBride),
                 typeof(UOACZCorpseEater),
                 typeof(UOACZDarkWisp),
                 typeof(UOACZDecayedZombie),
                 typeof(UOACZEnvelopingDarkness),
                 typeof(UOACZFailedExperiment),
                 typeof(UOACZFlamingZombie),
                 typeof(UOACZFleshGolem),
                 typeof(UOACZFountainOfEvil),
                 typeof(UOACZGhost),
                 typeof(UOACZGhoul),
                 typeof(UOACZGiantBat),
                 typeof(UOACZGiantSkeleton),
                 typeof(UOACZLich),
                 typeof(UOACZLichLord),
                 typeof(UOACZMummy),
                 typeof(UOACZNecromancer),
                 typeof(UOACZNightmare),
                 typeof(UOACZPatchworkSkeleton),
                 typeof(UOACZPhantom),
                 typeof(UOACZPitTentacle),
                 typeof(UOACZPlagueRat),
                 typeof(UOACZRagWitch),
                 typeof(UOACZRottingCorpse),
                 typeof(UOACZShadowDragon),
                 typeof(UOACZShadowFiend),
                 typeof(UOACZShadowKnight),
                 typeof(UOACZSkeletalCritter),
                 typeof(UOACZSkeletalDrake),
                 typeof(UOACZSkeletalHorse),
                 typeof(UOACZSkeletalKnight),
                 typeof(UOACZSkeletalMage),
                 typeof(UOACZSkeleton),
                 typeof(UOACZSpectre),
                 typeof(UOACZVampireBat),
                 typeof(UOACZVampireCountess),
                 typeof(UOACZVampireThrall),
                 typeof(UOACZVoidSlime),
                 typeof(UOACZZombie),
                 typeof(UOACZZombieMagi),
            } ),                        

            //INDEPENDENTS: NO TEAM - This Must be the Last Table in List
            //Table Not Looked At: Only Here For Reference
            new AITeamList(new Type[]
			{
            } )		
		};
    }

    public class AIKinTeamList
    {
        public Type[] Types { get; set; }
        public int[] HueMod { get; set; }

        public AIKinTeamList(Type[] types, int[] hue)
        {
            Types = types;
            HueMod = hue;
        }

        public static bool CheckKinTeam(Mobile from, Mobile target)
        {
            PlayerMobile pm_Target = target as PlayerMobile;

            if (pm_Target == null)
                return false;           

            bool orcMaskEquipped = false;

            Item headItem = pm_Target.FindItemOnLayer(Layer.Helm);
            {
                if (headItem is OrcishKinMask || headItem is Custom.Items.ArmoredOrcishKinMask)                
                    orcMaskEquipped = true;                
            }

            if (!(KinPaint.IsWearingKinPaint(pm_Target) || orcMaskEquipped))
                return false;

            for (int a = 0; a < m_KinTeams.Length; a++)
            {
                AIKinTeamList typeList = m_KinTeams[a];

                Type[] types = typeList.Types;
                int[] kinHues = typeList.HueMod;

                for (int b = 0; b < types.Length; b++)
                {
                    if (from.GetType() == types[b])
                    {
                        foreach (int hue in kinHues)
                        {
                            //Orc Mask Override
                            if ((hue == 2416 || hue == 1439) && orcMaskEquipped)
                                return true;

                            if (hue == pm_Target.HueMod)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static AIKinTeamList[] m_KinTeams = new AIKinTeamList[]
        {  
            //Orcs
            new AIKinTeamList(new Type[]
            {
                typeof( Orc ),			    
                typeof( OrcCaptain ),
			    typeof( OrcishLord ),
		        typeof( rOrcCaptain ),		
                typeof( OrcishMage ),
                typeof( FrostOrc ),
			    typeof( FrostOrcLord ),		
                typeof( FrostOrcMage ),

            }, new int[]{2416, 1439}),

            //Drow
            new AIKinTeamList(new Type[]
            {
                typeof(SvirfneblinIllusionist),
                typeof(SvirfneblinRogue),
                typeof(Gauth),
                typeof(Beholder),                

            }, new int[]{1107}),

            //Undead
            new AIKinTeamList(new Type[]
            {
                typeof(Skeleton),                
                typeof(Zombie),
                typeof(Ghoul),
                typeof(Spectre),
                typeof(Shade),
                typeof(Wraith)

            }, new int[]{0}),

            //Tribal
            new AIKinTeamList(new Type[]
            {
                typeof(Savage),               
                typeof(SavageShaman),
                typeof(SavageRider)

            }, new int[]{1619}),     
        };
    }
}