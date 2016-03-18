using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class SlayerGroup
    {
        private static SlayerEntry[] m_TotalEntries;
        private static SlayerGroup[] m_Groups;

        public static SlayerEntry[] TotalEntries
        {
            get { return m_TotalEntries; }
        }

        public static SlayerGroup[] Groups
        {
            get { return m_Groups; }
        }

        public static SlayerEntry GetEntryByName(SlayerName name)
        {
            int v = (int)name;

            if (v >= 0 && v < m_TotalEntries.Length)
                return m_TotalEntries[v];

            return null;
        }

        public static SlayerName GetLootSlayerType(Type type)
        {
            for (int i = 0; i < m_Groups.Length; ++i)
            {
                SlayerGroup group = m_Groups[i];
                Type[] foundOn = group.FoundOn;

                bool inGroup = false;

                for (int j = 0; foundOn != null && !inGroup && j < foundOn.Length; ++j)
                    inGroup = (foundOn[j] == type);

                if (inGroup)
                {
                    int index = Utility.Random(1 + group.Entries.Length);

                    if (index == 0)
                        return group.m_Super.Name;

                    return group.Entries[index - 1].Name;
                }
            }

            return SlayerName.Silver;
        }

        public static SlayerName? GetSlayerForMobType(Type type)
        {
            for (int i = 0; i < m_Groups.Length; ++i)
            {
                SlayerGroup group = m_Groups[i];
                Type[] foundOn = group.Super.Types;

                bool inGroup = false;

                for (int j = 0; foundOn != null && !inGroup && j < foundOn.Length; ++j)
                    inGroup = (foundOn[j] == type);

                if (inGroup)
                {
                    int index = Utility.Random(1 + group.Entries.Length);

                    if (index == 0)
                        return group.m_Super.Name;

                    return group.Entries[index - 1].Name;
                }
            }

            return null;
        }

        static SlayerGroup()
        {
            SlayerGroup humanoid = new SlayerGroup();
            SlayerGroup undead = new SlayerGroup();
            SlayerGroup elemental = new SlayerGroup();
            SlayerGroup abyss = new SlayerGroup();
            SlayerGroup arachnid = new SlayerGroup();
            SlayerGroup reptilian = new SlayerGroup();
            SlayerGroup fey = new SlayerGroup();

            humanoid.Opposition = new SlayerGroup[] { undead };
            humanoid.FoundOn = new Type[] { typeof(BoneKnight), typeof(Lich), typeof(LichLord) };
            humanoid.Super = new SlayerEntry(SlayerName.Repond, typeof(ArcticOgreLord), typeof(Cyclops), typeof(Ettin), typeof(EvilMage), typeof(EvilMageLord), typeof(FrostTroll), typeof(MeerCaptain),
                typeof(MeerEternal), typeof(MeerMage), typeof(MeerWarrior), typeof(Ogre), typeof(OgreLord), typeof(OgreMage), typeof(Orc), typeof(OrcBomber), typeof(OrcBrute), typeof(OrcCaptain),
                typeof(OrcishLord), typeof(OrcishMage), typeof(Ratman), typeof(RatmanArcher), typeof(RatmanMage), typeof(SavageRider), typeof(SavageShaman), typeof(Savage), typeof(Titan), typeof(Troll),
                // despise mobs
                typeof(DespiseOrc), typeof(ElderMojoka), typeof(OrcishExecutioner), typeof(OrcishGrunt), typeof(OrcishMaurk), typeof(OrcishPeon), typeof(OrcishScout), typeof(OrcishSurjin), typeof(OrcMojoka),
                typeof(ArmoredTitan), typeof(CyclopsShaman),
                // shame mobs
                typeof(AtlanteanBattleMage), typeof(AtlanteanWarden),
                // ice mobs
                typeof(OrghereimBeastmaster), typeof(OrghereimCrone), typeof(OrghereimBoneMender), typeof(OrghereimIceCarl), typeof(OrghereimSage), typeof(OrghereimShieldMaiden), typeof(OrghereimShieldMother),
                typeof(OrghereimSwordThane), typeof(OrghereimTracker), typeof(OrghereimBowMaiden), typeof(Valkyrie),
                // sanguin
                typeof(SanguinAlchemist), typeof(SanguinAssassin), typeof(SanguinConscript), typeof(SanguinDefender), typeof(SanguinHealer), typeof(SanguinHunter), typeof(SanguinKnight),
                typeof(SanguinMage), typeof(SanguinMedic), typeof(SanguinMender), typeof(SanguinProtector), typeof(SanguinScout), typeof(SanguinWizard), typeof(SanguinArchblade),
                // covetous
                typeof(DrowBlackguard), typeof(DrowBlademaster), typeof(DrowSpellsinger),
                // misc
                typeof(Brigand), typeof(Smuggler), typeof(TombRaider), typeof(Thug), typeof(Poacher),
                typeof(Bootlegger), typeof(GraveRobber),
                typeof(Custom.Pirates.BritainShipCaptain), typeof(Custom.Pirates.BritainMarine), typeof(Custom.Pirates.BritainSailor), typeof(Custom.Pirates.BritainShipCarpenter), typeof(Custom.Pirates.BritainShipSurgeon),
                typeof(Custom.Pirates.OceanFisherman), typeof(Custom.Pirates.OceanPirate), typeof(Custom.Pirates.OceanPirateCaptain), typeof(Custom.Pirates.PirateCaptain), typeof(Custom.Pirates.Pirate), typeof(Custom.Pirates.PirateSawbones),
                typeof(Custom.Pirates.PirateShipCarpenter), typeof(HeadlessOne), typeof(SvirfneblinRogue), typeof(SvirfneblinIllusionist), typeof(SpawnedOrcishLord), typeof(OgreMage), typeof(KuoToa), typeof(MinotaurScout), typeof(Minotaur),
                typeof(TormentedMinotaur), typeof(MinotaurCaptain), typeof(GreaterCyclops), typeof(Executioner), typeof(EvilHealer), typeof(WanderingHealer), typeof(OrcWarboss), typeof(DrowKeeper), typeof(Atraxis), typeof(Maggot), typeof(DiseasedViscera), typeof(Entrail),
                typeof(SuperOgreLord), typeof(DragonHandler)
                );
            humanoid.Entries = new SlayerEntry[]
                {
                    new SlayerEntry( SlayerName.OgreTrashing, typeof( Ogre ), typeof( OgreLord ), typeof( ArcticOgreLord ), typeof(UndeadOgreLord) , typeof(OgreMage), typeof(Maggot), typeof(SuperOgreLord)),
                    new SlayerEntry( SlayerName.OrcSlaying, typeof( Orc ), typeof( OrcBomber ), typeof( OrcBrute ), typeof( OrcCaptain ), typeof( OrcishLord ), typeof( OrcishMage ),
                        typeof(DespiseOrc), typeof(ElderMojoka), typeof(OrcishExecutioner), typeof(OrcishGrunt), typeof(OrcishMaurk), typeof(OrcishPeon), typeof(OrcishScout),
                        typeof(OrcishSurjin), typeof(OrcMojoka), typeof(FrostOrc), typeof(FrostOrcLord), typeof(FrostOrcMage), typeof(SpawnedOrcishLord), typeof(OrcWarboss)), 
                    new SlayerEntry( SlayerName.TrollSlaughter, typeof( Troll ), typeof( FrostTroll ) )
                };

            undead.Opposition = new SlayerGroup[] { humanoid };
            undead.Super = new SlayerEntry(SlayerName.Silver, typeof(AncientLich), typeof(Bogle), typeof(BoneKnight), typeof(BoneMagi),
                typeof(DarknightCreeper), typeof(FleshGolem), typeof(Ghoul),
                typeof(GoreFiend), typeof(HellSteed), typeof(LadyOfTheSnow), typeof(Lich), typeof(LichLord),
                typeof(Mummy), typeof(PestilentBandage), typeof(Revenant), typeof(RevenantLion),
                typeof(RottingCorpse), typeof(Shade), typeof(ShadowKnight), typeof(SkeletalKnight),
                typeof(SkeletalMage), typeof(Zombie), typeof(Spectre), typeof(Skeleton),
                typeof(BloodyZombie), typeof(CorruptReaver), typeof(CorruptRunecaster), typeof(CorruptWarmage),
                typeof(Creeper), typeof(DecayedZombie), typeof(FlamingZombie), typeof(HollowOne), typeof(UndeadOgreLord), typeof(RisenKnight), typeof(RisenNoble),
                typeof(RisenHonorGuard), typeof(RisenRoyal), typeof(SkeletalDragon), typeof(SkeletalDrake),
                typeof(HalloweenFlamingSkull), typeof(HalloweenMummy), typeof(HalloweenPoltergeist), typeof(HalloweenVampire),
                typeof(Custom.Pirates.GhostShipNecromancer), typeof(Custom.Pirates.SkeletalCaptain), typeof(Custom.Pirates.SkeletalCrewman),
                typeof(Wraith), typeof(PatchworkSkeleton), typeof(AncientNecromancer)
                );

            undead.Entries = new SlayerEntry[0];

            fey.Opposition = new SlayerGroup[] { abyss };
            fey.Super = new SlayerEntry(SlayerName.Fey, typeof(Centaur), typeof(EtherealWarrior), typeof(Kirin), typeof(LordOaks), typeof(Pixie), typeof(Silvani), typeof(Treefellow), typeof(Unicorn), typeof(Wisp), typeof(DarkWisp),
                typeof(MLDryad), typeof(Satyr), typeof(CharredProtector), typeof(WoodlandSprite), typeof(ElderWoodlandSprite),
                typeof(HalloweenPossessedPumpkin), typeof(HalloweenWerewolf), typeof(ShadowWisp), typeof(TreeOfLife), typeof(TreeStalker), typeof(EarthlyTendril), typeof(Ent), typeof(WildOne)
                );
            fey.Entries = new SlayerEntry[0];

            elemental.Opposition = new SlayerGroup[] { abyss };
            elemental.FoundOn = new Type[] { typeof(Balron), typeof(Daemon) };
            elemental.Super = new SlayerEntry(SlayerName.ElementalBan, typeof(AgapiteElemental), typeof(AtlanteanBattleMage), typeof(AtlanteanWarden), typeof(FlameElementalist), typeof(EnergyElementalist), typeof(ElementalSeer), typeof(AirElemental), typeof(SummonedAirElemental), typeof(BloodElemental), typeof(BronzeElemental),
                typeof(CopperElemental), typeof(CrystalElemental), typeof(DullCopperElemental), typeof(EarthElemental), typeof(SummonedEarthElemental), typeof(Efreet), typeof(FireElemental), typeof(SummonedFireElemental),
                typeof(GoldenElemental), typeof(IceElemental), typeof(KazeKemono), typeof(PoisonElemental), typeof(RaiJu), typeof(SandVortex), typeof(ShadowIronElemental), typeof(SnowElemental), typeof(ElderSnowElemental), typeof(ValoriteElemental),
                typeof(VeriteElemental), typeof(WaterElemental), typeof(SummonedWaterElemental), typeof(chaosElemental), typeof(orderElemental), typeof(AncientFlame), typeof(ElderFireElemental), typeof(ElderWaterElemental),
                typeof(FountainOfEvil), typeof(Gust), typeof(Puddle), typeof(EarthOreling), typeof(RockElemental), typeof(RockOreling), typeof(AcidElemental), typeof(ToxicElemental), typeof(ElderBloodElemental), typeof(ElderPoisonElemental), typeof(ElderToxicElemental), typeof(ElderAcidElemental),
                typeof(ElderIceElemental), typeof(ShadowFiend), typeof(TheDeepOne), typeof(DeepCrab), typeof(DeepTentacle), typeof(DeepWater), typeof(ImmortalFlameBoss)
                );

            elemental.Entries = new SlayerEntry[]
                {
                    new SlayerEntry( SlayerName.BloodDrinking, typeof( BloodElemental ), typeof( ElderBloodElemental ) ),
                    new SlayerEntry( SlayerName.EarthShatter, typeof( AgapiteElemental ), typeof( BronzeElemental ), typeof( CopperElemental ), typeof( DullCopperElemental ), typeof( EarthElemental ), typeof( SummonedEarthElemental ),
                        typeof( GoldenElemental ), typeof( ShadowIronElemental ), typeof( ValoriteElemental ), typeof( VeriteElemental ), typeof(EarthOreling), typeof(RockOreling), typeof(RockElemental) ),
                    new SlayerEntry( SlayerName.ElementalHealth, typeof( PoisonElemental ), typeof(AcidElemental), typeof(ToxicElemental), typeof(ElderPoisonElemental), typeof(ElderAcidElemental), typeof(ElderToxicElemental) ),
                    new SlayerEntry( SlayerName.FlameDousing, typeof( FireElemental ), typeof(FlameElementalist), typeof( SummonedFireElemental ), typeof(ElderFireElemental), typeof(AncientFlame), typeof(ImmortalFlameBoss) ),
                    new SlayerEntry( SlayerName.SummerWind, typeof( SnowElemental ), typeof( IceElemental ), typeof( ElderIceElemental ), typeof(ElderSnowElemental) ),
                    new SlayerEntry( SlayerName.Vacuum, typeof( AirElemental ), typeof(EnergyElementalist), typeof( SummonedAirElemental ), typeof(Gust), typeof(SandVortex), typeof(Efreet) ),
                    new SlayerEntry( SlayerName.WaterDissipation, typeof( WaterElemental ), typeof(ElementalSeer), typeof( SummonedWaterElemental ), typeof(Puddle), typeof(ElderWaterElemental), typeof(FountainOfEvil), typeof(TheDeepOne), typeof(DeepCrab), typeof(DeepTentacle), typeof(DeepWater) )
                };

            abyss.Opposition = new SlayerGroup[] { elemental, fey };
            abyss.FoundOn = new Type[] { typeof(BloodElemental) };


            abyss.Super = new SlayerEntry(SlayerName.Exorcism, typeof(AbysmalHorror), typeof(Balron), typeof(BoneDemon), typeof(ChaosDaemon), typeof(Daemon), typeof(SummonedDaemon), typeof(DemonKnight),
                typeof(Devourer), typeof(Gargoyle), typeof(FireGargoyle), typeof(Gibberling), typeof(HordeMinion), typeof(IceFiend), typeof(IceFiendLord), typeof(Imp), typeof(Impaler), typeof(Ravager), typeof(StoneGargoyle), typeof(ArcaneDaemon), typeof(EnslavedGargoyle), typeof(GargoyleDestroyer), typeof(GargoyleEnforcer), typeof(Moloch), typeof(Succubus), typeof(Balron),
                typeof(GoldenBalron), typeof(SilverDaemon), typeof(SilverDaemonLord));

            abyss.Entries = new SlayerEntry[]
                {
                    new SlayerEntry( SlayerName.DaemonDismissal, typeof( AbysmalHorror ), typeof( Balron ), typeof( BoneDemon ), typeof( ChaosDaemon ), typeof( Daemon ), typeof( SummonedDaemon ), typeof( DemonKnight ),
                        typeof( Devourer ), typeof( Gibberling ), typeof( HordeMinion ), typeof( IceFiend ), typeof( Imp ), typeof( Impaler ), typeof( Ravager ), typeof( ArcaneDaemon ), typeof( Moloch ), typeof( SilverDaemon),typeof( Belial ), typeof(SilverDaemonLord)),

                    new SlayerEntry( SlayerName.GargoylesFoe, typeof( FireGargoyle ), typeof( Gargoyle ), typeof( StoneGargoyle ), typeof( EnslavedGargoyle ), typeof( GargoyleDestroyer ), typeof( GargoyleEnforcer )),

                    new SlayerEntry( SlayerName.BalronDamnation, typeof( Balron ), typeof(GoldenBalron), typeof(IceFiendLord)),
                };

            arachnid.Opposition = new SlayerGroup[] { reptilian };
            arachnid.FoundOn = new Type[] { typeof(AncientWyrm), typeof(GreaterDragon), typeof(Dragon), typeof(OphidianMatriarch), typeof(ShadowWyrm) };
            arachnid.Super = new SlayerEntry(SlayerName.ArachnidDoom, typeof(DreadSpider), typeof(FrostSpider), typeof(GiantBlackWidow), typeof(GiantSpider), typeof(Mephitis), typeof(Scorpion), typeof(ChromaticCrawler),
                typeof(TerathanAvenger), typeof(TerathanDrone), typeof(TerathanMatriarch), typeof(TerathanWarrior), typeof(ForestSpider), typeof(RockSpider), typeof(ScorpionHatchling),
                typeof(DriderHarbinger), typeof(DriderSentinel), typeof(DriderWarrior), typeof(DemonwebQueen), typeof(CorruptSpiderling), typeof(Deathspinner));
            arachnid.Entries = new SlayerEntry[]
                {
                    new SlayerEntry( SlayerName.ScorpionsBane, typeof( Scorpion ), typeof( ChromaticCrawler), typeof(ScorpionHatchling) ),
                    new SlayerEntry( SlayerName.SpidersDeath, typeof( DreadSpider ), typeof( FrostSpider ), typeof( GiantBlackWidow ), typeof( GiantSpider ), typeof( Mephitis ), typeof(ForestSpider), typeof(RockSpider), typeof(DemonwebQueen), typeof(CorruptSpiderling), typeof(Deathspinner) ),
                    new SlayerEntry( SlayerName.Terathan, typeof( TerathanAvenger ), typeof( TerathanDrone ), typeof( TerathanMatriarch ), typeof( TerathanWarrior ), typeof(DriderHarbinger), typeof(DriderSentinel), typeof(DriderWarrior))
                };

            reptilian.Opposition = new SlayerGroup[] { arachnid };
            reptilian.FoundOn = new Type[] { typeof(TerathanAvenger), typeof(TerathanMatriarch) };
            reptilian.Super = new SlayerEntry(SlayerName.ReptilianDeath, typeof(AncientWyrm), typeof(DeepSeaSerpent), typeof(GreaterDragon), typeof(Dragon), typeof(ChromaticDragon), typeof(Basilisk), typeof(Cockatrice), typeof(Drake), typeof(ArcaneDrake), typeof(ShadowDrake), typeof(GiantIceWorm),
                typeof(IceSerpent), typeof(GiantSerpent), typeof(Hiryu), typeof(IceSnake), typeof(JukaLord), typeof(JukaMage), typeof(JukaWarrior), typeof(LavaSerpent), typeof(LavaSnake),
                typeof(LesserHiryu), typeof(Lizardman), typeof(OphidianArchmage), typeof(OphidianKnight), typeof(OphidianMage), typeof(OphidianMatriarch), typeof(OphidianWarrior),
                typeof(SeaSerpent), typeof(Serado), typeof(SerpentineDragon), typeof(ShadowWyrm), typeof(SilverSerpent), typeof(SkeletalDragon), typeof(Snake), typeof(SwampDragon),
                typeof(WhiteWyrm), typeof(AncientWinterWyrm), typeof(Wyvern), typeof(Yamandon), typeof(ModAncientWyrm), typeof(RubyDragon), typeof(SkeletalDrake), typeof(DrakeWhelp), typeof(DragonWhelp), typeof(ChaosDragon),
                typeof(CoralSnake), typeof(GiantCoralSnake), typeof(GreaterLizard), typeof(IceSkitter), typeof(LavaLizard), typeof(WyvernHatchling), typeof(WhiteDrake), typeof(Deepstalker), typeof(OrderDragon),
                typeof(AncientRedWyrm), typeof(EternalFlameWyrm), typeof(ShadowDragon), typeof(ArcaneDragon), typeof(EmperorDragon), typeof(BaronVonGeddon)
                );
            reptilian.Entries = new SlayerEntry[]
                {
                    new SlayerEntry( SlayerName.DragonSlaying, typeof( AncientWyrm ), typeof( GreaterDragon ), typeof( Dragon ), typeof( ChromaticDragon ),  typeof(Basilisk), typeof( Drake ),
                    typeof( Hiryu ), typeof( LesserHiryu ), typeof( SerpentineDragon ), typeof( ShadowWyrm ), typeof( SkeletalDragon ), typeof( SwampDragon ), typeof( WhiteWyrm ), typeof( Wyvern ), typeof(ArcaneDragon), typeof(ShadowDragon), typeof(ChaosDragon), typeof(OrderDragon), typeof(WhiteDrake), typeof(ArcaneDrake), typeof(ShadowDrake), typeof(RubyDragon), 
                    typeof(SkeletalDrake), typeof(DrakeWhelp), typeof(DragonWhelp), typeof(WyvernHatchling), typeof(AncientRedWyrm), typeof(EternalFlameWyrm), typeof(EmperorDragon), typeof(BaronVonGeddon)),
                    new SlayerEntry( SlayerName.LizardmanSlaughter, typeof( Lizardman ) ),
                    new SlayerEntry( SlayerName.Ophidian, typeof( OphidianArchmage ), typeof( OphidianKnight ), typeof( OphidianMage ), typeof( OphidianMatriarch ), typeof( OphidianWarrior ) ),
                    new SlayerEntry( SlayerName.SnakesBane, typeof( DeepSeaSerpent ), typeof( GiantIceWorm ), typeof( GiantSerpent ), typeof( IceSerpent ), typeof( IceSnake ), typeof( LavaSerpent ),
                    typeof( LavaSnake ), typeof( SeaSerpent ), typeof( Serado ), typeof( SilverSerpent ), typeof( Snake ), typeof( Yamandon ), typeof(Coral), typeof(GiantCoralSnake) )
                };

            m_Groups = new SlayerGroup[]
                {
                    humanoid,
                    undead,
                    elemental,
                    abyss,
                    arachnid,
                    reptilian,
                    fey
                };

            m_TotalEntries = CompileEntries(m_Groups);
        }

        private static SlayerEntry[] CompileEntries(SlayerGroup[] groups)
        {
            SlayerEntry[] entries = new SlayerEntry[28];

            for (int i = 0; i < groups.Length; ++i)
            {
                SlayerGroup g = groups[i];

                g.Super.Group = g;

                entries[(int)g.Super.Name] = g.Super;

                for (int j = 0; j < g.Entries.Length; ++j)
                {
                    g.Entries[j].Group = g;
                    entries[(int)g.Entries[j].Name] = g.Entries[j];
                }
            }

            return entries;
        }

        private SlayerGroup[] m_Opposition;
        private SlayerEntry m_Super;
        private SlayerEntry[] m_Entries;
        private Type[] m_FoundOn;

        public SlayerGroup[] Opposition { get { return m_Opposition; } set { m_Opposition = value; } }
        public SlayerEntry Super { get { return m_Super; } set { m_Super = value; } }
        public SlayerEntry[] Entries { get { return m_Entries; } set { m_Entries = value; } }
        public Type[] FoundOn { get { return m_FoundOn; } set { m_FoundOn = value; } }

        public bool OppositionSuperSlays(Mobile m)
        {
            for (int i = 0; i < Opposition.Length; i++)
            {
                if (Opposition[i].Super.Slays(m))
                    return true;
            }

            return false;
        }

        public SlayerGroup()
        {
        }
    }
}