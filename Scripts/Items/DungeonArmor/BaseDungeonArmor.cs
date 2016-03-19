using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells.Fourth;
using Server.Spells;
using Server.SkillHandlers;
using Server.Achievements;
using Server.Gumps;

namespace Server.Items
{
    public abstract class BaseDungeonArmor : BaseArmor
    {        
        public enum DungeonEnum
        {
            Unspecified,

            Shame,
            Deceit,
            Destard,
            Hythloth,  
            Covetous,
            Wrong,
            Despise,
            Ice,
            Fire
        }

        public enum ArmorTierEnum
        {
            Unspecified,
            Tier1,
            Tier2,
            Tier3,
            Tier4
        }

        public enum ArmorLocation
        {
            Unspecified,
            Helmet,
            Gorget,
            Chest,
            Arms,
            Gloves,
            Legs
        }

        private int m_BlessedCharges = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BlessedCharges
        {
            get { return m_BlessedCharges; }
            set
            { 
                m_BlessedCharges = value;

                if (m_BlessedCharges > m_MaxBlessedCharges)
                    m_BlessedCharges = m_MaxBlessedCharges;
            }
        }

        private int m_MaxBlessedCharges = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxBlessedCharges
        {
            get { return m_MaxBlessedCharges; }
            set 
            { 
                m_MaxBlessedCharges = value;

                if (m_BlessedCharges > m_MaxBlessedCharges)
                    m_BlessedCharges = m_MaxBlessedCharges;
            }
        }

        private DungeonEnum m_Dungeon;
        [CommandProperty(AccessLevel.GameMaster)]
        public DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set 
            {                
                m_Dungeon = value;

                DungeonArmorDetail detail = new DungeonArmorDetail(m_Dungeon, m_Tier);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }

        private ArmorTierEnum m_Tier;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArmorTierEnum Tier
        {
            get { return m_Tier; }
            set { m_Tier = value; }
        }

        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public BaseDungeonArmor(int itemId, DungeonEnum dungeon, ArmorTierEnum armorTier) : base(itemId)
        {           
            Dungeon = dungeon;
            Tier = armorTier;
        }

        public static BaseDungeonArmor CreateDungeonArmor(DungeonEnum dungeon, ArmorTierEnum armorTier, ArmorLocation armorLocation)
        {
            BaseDungeonArmor armorPiece = null;

            if (dungeon == DungeonEnum.Unspecified)
            {
                int dungeonCount = Enum.GetNames(typeof(DungeonEnum)).Length;
                
                dungeon = (DungeonEnum)Utility.RandomMinMax(1, dungeonCount - 1);
            }

            if (armorTier == ArmorTierEnum.Unspecified)
            {
                int armorTierCount = Enum.GetNames(typeof(ArmorTierEnum)).Length;
                armorTier = (ArmorTierEnum)Utility.RandomMinMax(1, armorTierCount - 1);
            }

            if (armorLocation == ArmorLocation.Unspecified)
            {
                int armorLocationCount = Enum.GetNames(typeof(ArmorLocation)).Length;
                armorLocation = (ArmorLocation)Utility.RandomMinMax(1, armorLocationCount - 1);
            }

            switch (armorLocation)
            {
                case ArmorLocation.Helmet: armorPiece = new DungeonArmorPlateHelm(dungeon, armorTier); break;
                case ArmorLocation.Gorget: armorPiece = new DungeonArmorPlateGorget(dungeon, armorTier); break;
                case ArmorLocation.Chest: armorPiece = new DungeonArmorPlateChest(dungeon, armorTier); break;
                case ArmorLocation.Arms: armorPiece = new DungeonArmorPlateArms(dungeon, armorTier); break;
                case ArmorLocation.Gloves: armorPiece = new DungeonArmorPlateGloves(dungeon, armorTier); break;
                case ArmorLocation.Legs: armorPiece = new DungeonArmorPlateLegs(dungeon, armorTier); break;
            }

            DungeonArmorDetail detail = new DungeonArmorDetail(dungeon, armorTier);

            if (armorTier != null && detail != null)
                armorPiece.Hue = detail.Hue;

            return armorPiece;
        }

        public BaseDungeonArmor(Serial serial): base(serial)
        {
        }

        public static string GetDungeonName(DungeonEnum dungeon)
        {
            switch (dungeon)
            {
                case DungeonEnum.Shame: return "Shame";
                case DungeonEnum.Deceit: return "Deceit";
                case DungeonEnum.Destard: return "Destard";
                case DungeonEnum.Hythloth: return "Hythloth";
                case DungeonEnum.Covetous: return "Covetous";
                case DungeonEnum.Wrong: return "Wrong";
                case DungeonEnum.Despise: return "Despise";
                case DungeonEnum.Ice: return "Ice";
                case DungeonEnum.Fire: return "Fire";
            }

            return "";
        }

        public override void OnSingleClick(Mobile from)
        {
            string sName = m_Dungeon.ToString().ToLower() + " dungeon ";
                        
            switch (Layer)
            {
                case Layer.Helm: sName += "helm"; break;
                case Layer.Neck: sName += "gorget"; break;
                case Layer.InnerTorso: sName += "chest"; break;
                case Layer.Arms: sName += "arms"; break;
                case Layer.Gloves: sName += "gloves"; break;
                case Layer.Pants: sName += "leggings"; break;
            }

            sName += ": tier " + ((int)Tier).ToString();

            LabelTo(from, sName);
            LabelTo(from, String.Format("[blessed charges: {0}/{1}]", BlessedCharges, MaxBlessedCharges));
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.CloseGump(typeof(DungeonArmorGump));
            from.SendGump(new DungeonArmorGump(this, from));
        }

        public static void CheckForAndUpdateDungeonArmorProperties(PlayerMobile player)
        {
            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            BaseDungeonArmor dungeonArmor = null;

            for (int a = 0; a < m_Layers.Count; a++)
            {
                dungeonArmor = player.FindItemOnLayer(m_Layers[a]) as BaseDungeonArmor;
                                
                if (dungeonArmor != null)
                    break;
            }

            //Only Need to Initiate The Update With Single Piece Player is Wearing
            if (dungeonArmor != null)
                dungeonArmor.UpdateProperties(player);
        }

        public void UpdateProperties(Mobile from)
        {
            if (from == null)
                return;

            PlayerDungeonArmorProfile dungeonArmor = new PlayerDungeonArmorProfile(from, null);
                
            bool matchingSet = false;

            if (dungeonArmor.MatchingSet)
                matchingSet = true;

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            for (int a = 0; a < m_Layers.Count; a++)
            {
                BaseArmor armor = from.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                if (armor != null)
                {
                    string modName = armor.Serial.ToString();

                    from.RemoveStatMod(modName + "Dex");
                                        
                    armor.BaseArmorRating = armor.ArmorBase;
                    armor.MeditationAllowance = armor.DefMedAllowance;

                    if (matchingSet && !dungeonArmor.InPlayerCombat)
                    {
                        armor.BaseArmorRating = dungeonArmor.DungeonArmorDetail.TieredArmorRating;

                        //Assign All of the Suit's Dex Penalty to The Chest Piece (Dex Will Be Automatically Recalculated if Any Piece is Removed)
                        if (armor.Layer == Layer.InnerTorso)
                            from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dungeonArmor.DungeonArmorDetail.DexPenalty, TimeSpan.Zero));

                        armor.MeditationAllowance = dungeonArmor.DungeonArmorDetail.MeditationAllowance;
                    }

                    else                                      
                        from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", armor.OldDexBonus, TimeSpan.Zero));                    
                }
            }

            PlayerMobile player = from as PlayerMobile;

            if (player != null)            
                player.ResetRegenTimers();            
        }
        
        public override bool OnEquip(Mobile from)
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(50), delegate
            {
                if (this == null) return;
                if (Deleted) return;
                if (from == null) return;
                if (from.Deleted) return;
                if (!from.Alive) return;

                PlayerDungeonArmorProfile dungeonArmor = new PlayerDungeonArmorProfile(from, null);

                if (dungeonArmor.MatchingSet)
                {
                    BaseArmorRating = dungeonArmor.DungeonArmorDetail.TieredArmorRating;

                    int hue = dungeonArmor.DungeonArmorDetail.Hue;
                    int effectHue = dungeonArmor.DungeonArmorDetail.EffectHue;

                    string dungeonArmorName = dungeonArmor.DungeonArmorDetail.DungeonName;

                    if (from.Hidden)
                    {
                        from.AllowedStealthSteps = 0;
                        from.RevealingAction();

                        from.SendMessage("The shifting aura of your dungeon armor reveals you.");
                    }

                    from.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Head);
                    from.PlaySound(0x1ED);

                    UpdateProperties(from);

                    from.PublicOverheadMessage(MessageType.Emote, hue, false, "*a magical aura surrounds " + from.Name + "*");

                    switch (dungeonArmorName)
                    {
                        case "Hythloth": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_HythlothArmor); break;
                        case "Shame": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_ShameArmor); break;
                        case "Destard": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_DestardArmor); break;
                        case "Deceit": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_DeceitArmor); break;
                        case "Covetous": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_CovetousArmor); break;
                        case "Wrong": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_WrongArmor); break;
                        case "Despise": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_DespiseArmor); break;
                        case "Ice": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_IceArmor); break;
                        case "Fire": AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_FireArmor); break;
                    }
                }    
            });

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is PlayerMobile)
            {
                PlayerMobile player = parent as PlayerMobile;

                PlayerDungeonArmorProfile dungeonArmor = new PlayerDungeonArmorProfile(player, null);

                UpdateProperties(player);

                if (dungeonArmor.Pieces == 5)
                {
                    if (player.Hidden)
                    {
                        player.AllowedStealthSteps = 0;
                        player.RevealingAction();

                        player.SendMessage("The shifting aura of your dungeon armor reveals you.");
                    }

                    player.PlaySound(0x5AA);
                    player.PublicOverheadMessage(MessageType.Emote, 0, false, "*a magical aura fades from " + player.Name + "*");
                }
            }
        }

        public override bool CheckBlessed(Mobile mobile, bool isOnDeath = false)
        {
            PlayerMobile player = mobile as PlayerMobile;

            if (player == null)
                return false;

            if (BlessedCharges > 0)
            {
                if (isOnDeath)
                    BlessedCharges--;

                return true;
            }

            else
                return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); //Version

            writer.Write((int)m_Dungeon);
            writer.Write((int)m_Tier);  
        
            //Version 1
            writer.Write((int)m_BlessedCharges);

            //Version 2
            writer.Write((int)m_MaxBlessedCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_Dungeon = (DungeonEnum)reader.ReadInt();
                m_Tier = (ArmorTierEnum)reader.ReadInt();
                m_BlessedCharges = reader.ReadInt();
            }

            if (version >= 2)
            {
                m_MaxBlessedCharges = reader.ReadInt();
            }           
        }

        public class PlayerDungeonArmorProfile
        {
            public DungeonArmorDetail DungeonArmorDetail = null;
            public PlayerMobile Player = null;           
            public bool InPlayerCombat = false;
            
            public bool MatchingSet = false;            
            public int Pieces = 0;

            BaseDungeonArmor lowestTierPiece = null;

            public PlayerDungeonArmorProfile(Mobile wearer, BaseDungeonArmor armorPiece)
            {
                Player = wearer as PlayerMobile;

                if (Player == null)
                {
                    DungeonArmorDetail = new DungeonArmorDetail(DungeonEnum.Unspecified, ArmorTierEnum.Unspecified);
                    return;
                }

                if (Player.LastPlayerCombatTime + Player.PlayerCombatExpirationDelay > DateTime.UtcNow)
                    InPlayerCombat = true;

                DungeonEnum currentDungeon = DungeonEnum.Unspecified;
                ArmorTierEnum lowestTier = (ArmorTierEnum)Enum.GetNames(typeof(ArmorTierEnum)).Length - 1;
                
                List<Layer> m_Layers = new List<Layer>();

                m_Layers.Add(Layer.Helm);
                m_Layers.Add(Layer.Neck);
                m_Layers.Add(Layer.InnerTorso);
                m_Layers.Add(Layer.Arms);
                m_Layers.Add(Layer.Gloves);
                m_Layers.Add(Layer.Pants);

                MatchingSet = true;

                for (int a = 0; a < m_Layers.Count; a++)
                {
                    BaseDungeonArmor dungeonArmor = Player.FindItemOnLayer(m_Layers[a]) as BaseDungeonArmor;

                    if (dungeonArmor == null)
                        MatchingSet = false;
                    else
                    {
                        Pieces++;

                        if (currentDungeon == DungeonEnum.Unspecified)
                        {
                            currentDungeon = dungeonArmor.Dungeon;
                            lowestTierPiece = dungeonArmor;
                        }

                        else
                        {
                            if (currentDungeon != dungeonArmor.Dungeon)
                                MatchingSet = false;
                        }

                        if (dungeonArmor.Tier < lowestTier)
                        {
                            lowestTier = dungeonArmor.Tier;
                            lowestTierPiece = dungeonArmor;
                        }   
                    }                   
                }

                //Single Piece of Armor Being Inspected
                if (armorPiece != null)
                {
                    currentDungeon = armorPiece.Dungeon;
                    lowestTier = armorPiece.Tier;                
                }
               
                DungeonArmorDetail = new DungeonArmorDetail(currentDungeon, lowestTier);
            }            
        }

        public class DungeonArmorDetail
        {
            public DungeonEnum Dungeon;
            public ArmorTierEnum Tier;

            public string DungeonName = "";
            public int Hue = 0;
            public int EffectHue = 0;

            public int TieredArmorRating = 65;

            public ArmorMeditationAllowance MeditationAllowance = ArmorMeditationAllowance.None;
            public int DexPenalty = -10;

            public double MeleeDamageInflictedScalar = 1.0;
            public double MeleeDamageReceivedScalar = 1.0;

            public double SpellDamageInflictedScalar = 1.0;
            public double SpellDamageReceivedScalar = 1.0;

            public double BreathDamageReceivedScalar = 1.0;
            public double BleedDamageReceivedScalar = 1.0;
            public double PoisonDamageReceivedScalar = 1.0;
            public double DiseaseDamageReceivedScalar = 1.0;

            public double SpecialEffectAvoidanceChance = 0;

            public double PoisonDamageInflictedScalar = 1.0;            
            public double NoPoisonChargeSpentChance = 0;
            
            public double BandageSelfTimeReduction = 0;
            public double BandageHealThroughPoisonScalar = 0;

            public double SpecialWeaponAttackBonus = 0;
            
            public double ReducedSpellManaCostChance = 0;
            public double PoisonSpellNoManaCostChance = 0;
            public double ReducedSpellManaCostScalar = .5;
            
            public int BonusStealthSteps = 0;
            public double BackstabDamageInflictedScalar = 1.0;
            public bool StealthLeavesFootprints = false;

            public double ProvokedCreatureDamageInflictedScalar = 1.0;
            public double IgnorePeacemakingBreakChance = 0;
            public double DiscordanceEffectBonus = 0;

            public double EnergySiphonOnMeleeAttackChance = 0;

            public double FlamestrikeOnMeleeAttackChance = 0;
            public double FlamestrikeOnReceiveMeleeHitChance = 0;  
            
            public string[] gumpText = new string[0];

            public DungeonArmorDetail(DungeonEnum dungeonType, ArmorTierEnum tierType)
            {
                Dungeon = dungeonType;
                Tier = tierType;

                int tierLevel = (int)Tier;                
                
                switch (Dungeon)
                {
                    case DungeonEnum.Unspecified:
                    break;

                    case DungeonEnum.Deceit:                      
                        DungeonName = "Deceit";
                        Hue = 1908;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 40 + (5 * tierLevel);

                        DexPenalty = -3;
                        MeditationAllowance = ArmorMeditationAllowance.Half;                        

                        EnergySiphonOnMeleeAttackChance = .20 + (.04 * tierLevel);                       

                        gumpText = new string[] { 
                                                "Deceit Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 50%",
                                                "",
                                                "Chance of Inflicting Energy Siphon on Melee Attack: " + Utility.CreatePercentageString(EnergySiphonOnMeleeAttackChance),                                                
                                                };
                    break;

                    case DungeonEnum.Destard:
                        DungeonName = "Destard";
                        Hue = 1778;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 55 + (5 * tierLevel);

                        DexPenalty = -10;
                        MeditationAllowance = ArmorMeditationAllowance.None;                        

                        MeleeDamageReceivedScalar = 1 - (.13 + (.03 * tierLevel));
                        SpellDamageReceivedScalar = 1 - (.13 + (.03 * tierLevel));

                        BandageSelfTimeReduction = 2 + (.5 * tierLevel);

                        gumpText = new string[] { 
                                                "Destard Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 0%",
                                                "",
                                                "Melee Damage Reduction: " + Utility.CreatePercentageString(1 - MeleeDamageReceivedScalar),
                                                "Spell Damage Reduction: " + Utility.CreatePercentageString(1 - SpellDamageReceivedScalar),
                                                "Bandage Self Timer Reduction: " + BandageSelfTimeReduction.ToString() + " seconds",
                                                };
                    break;

                    case DungeonEnum.Hythloth:
                        DungeonName = "Hythloth";
                        Hue = 1769;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 40 + (5 * tierLevel);

                        DexPenalty = -3;
                        MeditationAllowance = ArmorMeditationAllowance.Half;

                        MeleeDamageInflictedScalar = 1 + (.10 + (.025 * tierLevel));
                        SpecialWeaponAttackBonus = .08 + (.02 * tierLevel);

                        gumpText = new string[] { 
                                                "Hythloth Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),                                               
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 50%",
                                                 "",
                                                "Melee Damage Bonus: " + Utility.CreatePercentageString(MeleeDamageInflictedScalar - 1),
                                                "Special Weapon Attack Chance Bonus: " + Utility.CreatePercentageString(SpecialWeaponAttackBonus),
                                                };
                    break;

                    case DungeonEnum.Shame:
                        DungeonName = "Shame";
                        Hue = 1763;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 30 + (5 * tierLevel);

                        DexPenalty = 0;
                        MeditationAllowance = ArmorMeditationAllowance.All;

                        SpellDamageInflictedScalar = 1 + (.15 + (.05 * tierLevel));
                        ReducedSpellManaCostChance = .15 + (.05 * tierLevel);

                        gumpText = new string[] { 
                                                "Shame Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 100%",                                                
                                                 "",
                                                "Spell Damage Bonus: " + Utility.CreatePercentageString(SpellDamageInflictedScalar - 1),
                                                "Chance to Cast Spell At Reduced Mana Cost: " + Utility.CreatePercentageString(ReducedSpellManaCostChance),
                                                };
                    break;

                    case DungeonEnum.Covetous:
                        DungeonName = "Covetous";
                        Hue = 2212;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 30 + (5 * tierLevel);

                        DexPenalty = 0;
                        MeditationAllowance = ArmorMeditationAllowance.All;

                        PoisonDamageInflictedScalar = 1 + (.2 + (.025 * tierLevel));
                        PoisonSpellNoManaCostChance = .30 + (.05 * tierLevel);
                        NoPoisonChargeSpentChance = .2 + (.025 * tierLevel);

                        gumpText = new string[] { 
                                                "Covetous Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 100%",                                                
                                                 "",
                                                "Poison Damage Inflicted Bonus: " + Utility.CreatePercentageString(PoisonDamageInflictedScalar - 1),
                                                "Chance to Cast Poison Spells At No Mana Cost: " + Utility.CreatePercentageString(PoisonSpellNoManaCostChance),
                                                "Chance to Apply Poison And Not Spend Poison Charge: " + Utility.CreatePercentageString(NoPoisonChargeSpentChance),
                                                };
                    break;

                    case DungeonEnum.Wrong:
                        DungeonName = "Wrong";
                        Hue = 2675;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 40 + (5 * tierLevel);
                        
                        DexPenalty = -3;
                        MeditationAllowance = ArmorMeditationAllowance.Half;

                        BonusStealthSteps = 4 + (1 * tierLevel);
                        BackstabDamageInflictedScalar = 1 + (.40 + (.05 * tierLevel));
                        StealthLeavesFootprints = true;

                        gumpText = new string[] { 
                                                "Wrong Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),                                                
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 50%",
                                                 "",
                                                "Bonus Stealth Steps: " + BonusStealthSteps.ToString(),
                                                "Backstab Damage Inflicted Bonus: " + Utility.CreatePercentageString(BackstabDamageInflictedScalar - 1),
                                                "Stealth Movement Now Leaves Footprints",
                                                };
                    break;

                    case DungeonEnum.Despise:
                        DungeonName = "Despise";
                        Hue = 2516;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 30 + (5 * tierLevel);

                        DexPenalty = 0;
                        MeditationAllowance = ArmorMeditationAllowance.All;

                        ProvokedCreatureDamageInflictedScalar = 1 + (.2 + (.025 * tierLevel));
                        IgnorePeacemakingBreakChance = .30 + (.05 * tierLevel);
                        DiscordanceEffectBonus = .04 + (.015 * tierLevel);

                        gumpText = new string[] { 
                                                "Despise Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 100%",                                                
                                                 "",
                                                "Provoked Creature Damage Inflicted Bonus: " + Utility.CreatePercentageString(ProvokedCreatureDamageInflictedScalar - 1),
                                                "Chance to Avoid Peacemaking Breaking: " + Utility.CreatePercentageString(IgnorePeacemakingBreakChance),
                                                "Discordance Effect Bonus: " + Utility.CreatePercentageString(DiscordanceEffectBonus),
                                                };
                    break;

                    case DungeonEnum.Ice:
                        DungeonName = "Ice";
                        Hue = 2579;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 55 + (5 * tierLevel);

                        DexPenalty = -10;
                        MeditationAllowance = ArmorMeditationAllowance.None;

                        BreathDamageReceivedScalar = 1 - (.40 + (.05 * tierLevel));
                        BleedDamageReceivedScalar = 1 - (.40 + (.05 * tierLevel));
                        PoisonDamageReceivedScalar = 1 - (.40 + (.05 * tierLevel));
                        DiseaseDamageReceivedScalar = 1 - (.40 + (.05 * tierLevel));

                        BandageHealThroughPoisonScalar = .40 + (.05 * tierLevel);

                        SpecialEffectAvoidanceChance = .40 + (.05 * tierLevel);

                        gumpText = new string[] { 
                                                "Ice Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 0%",  
                                                 "",
                                                "Breath Damage Reduction: " + Utility.CreatePercentageString(1 - BreathDamageReceivedScalar),
                                                "Bleed Damage Reduction: " + Utility.CreatePercentageString(1 - BleedDamageReceivedScalar),
                                                "Poison Damage Reduction: " + Utility.CreatePercentageString(1 - PoisonDamageReceivedScalar),
                                                "Disease Damage Reduction: " + Utility.CreatePercentageString(1 - DiseaseDamageReceivedScalar),
                                                "Bandage Heal Through Poison Amount: " + Utility.CreatePercentageString(BandageHealThroughPoisonScalar),
                                                "Special Effect Avoidance Chance: " + Utility.CreatePercentageString(SpecialEffectAvoidanceChance),
                                                };
                    break;

                    case DungeonEnum.Fire:
                        DungeonName = "Fire";
                        Hue = 2635;
                        EffectHue = Hue - 1;

                        TieredArmorRating = 55 + (5 * tierLevel);

                        DexPenalty = -10;
                        MeditationAllowance = ArmorMeditationAllowance.None;

                        BreathDamageReceivedScalar = 1 - (.40 + (.05 * tierLevel));

                        FlamestrikeOnMeleeAttackChance = 0.10 + (.05 * tierLevel); //0.05 + (.025 * tierLevel) Old
                        FlamestrikeOnReceiveMeleeHitChance = 0.10 + (.05 * tierLevel); //0.05 + (.025 * tierLevel) Old

                        gumpText = new string[] { 
                                                "Fire Dungeon Armor",
                                                "Tier " + tierLevel.ToString(),
                                                "",
                                                "Armor Rating: " + TieredArmorRating.ToString(),
                                                "Dex Penalty: " + DexPenalty.ToString(),
                                                "Meditation Allowed: 0%",  
                                                 "",
                                                "Breath Damage Reduction: " + Utility.CreatePercentageString(1 - BreathDamageReceivedScalar),
                                                "Chance of Flamestrike on Melee Attack: " + Utility.CreatePercentageString(FlamestrikeOnMeleeAttackChance),
                                                "Chance of Flamestrike on Being Hit By Melee Attack: " + Utility.CreatePercentageString(FlamestrikeOnReceiveMeleeHitChance),
                                                };
                    break;
                }
            }
        }

        public class DungeonArmorGump : Gump
        {
            BaseDungeonArmor m_BaseDungeonArmor;

            public DungeonArmorGump(BaseDungeonArmor baseDungeonArmor, Mobile from): base(10, 10)
            {
                if (baseDungeonArmor == null || from == null)
                    return;

                m_BaseDungeonArmor = baseDungeonArmor;

                PlayerDungeonArmorProfile dungeonArmor = new PlayerDungeonArmorProfile(from, baseDungeonArmor);

                if (dungeonArmor.DungeonArmorDetail == null)
                    return;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);

                int textHue = 2036;

                AddImage(5, 4, 103);
                AddImage(5, 91, 103);
                AddImage(5, 140, 103);

                AddImage(13, 19, 3604, 2051);
                AddImage(13, 103, 3604, 2051);

                AddImage(-15, -22, 50529, dungeonArmor.DungeonArmorDetail.Hue - 1);
                AddImage(-15, -23, 50527, dungeonArmor.DungeonArmorDetail.Hue - 1);
                AddImage(-12, -23, 50528, dungeonArmor.DungeonArmorDetail.Hue - 1);
                AddImage(-8, -20, 50530, dungeonArmor.DungeonArmorDetail.Hue - 1);
                AddImage(-14, -25, 50531, dungeonArmor.DungeonArmorDetail.Hue - 1);
                AddImage(-14, -24, 60563, dungeonArmor.DungeonArmorDetail.Hue - 1);

                int iStartY = 10;
                
                for (int a = 0; a < dungeonArmor.DungeonArmorDetail.gumpText.Length; a++)
                {
                    AddLabel(157, iStartY, textHue, dungeonArmor.DungeonArmorDetail.gumpText[a]);

                    iStartY += 20;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
            }
        }
    }    
}