using System;
using System.Text;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;
using Server.Factions;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Spells.Spellweaving;
using Server.Guilds;
using Server.SkillHandlers;
using Server.Multis;
using Server.Custom;
using System.Globalization;

namespace Server.Items
{
    public interface ISlayer
    {
        SlayerName Slayer { get; set; }
        SlayerName Slayer2 { get; set; }
        SlayerName TempSlayer { get; set; }
    }

    public abstract class BaseWeapon : Item, IWeapon, IFactionItem, ICraftable, ISlayer, IDurability
    {
        private string m_EngravedText;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set { m_EngravedText = value; InvalidateProperties(); }
        }

        public virtual int IconItemId { get { return ItemID; } }
        public virtual int IconHue { get { return Hue; } }
        public virtual int IconOffsetX { get { return 0; } } //65 is Baseline X For Gumps
        public virtual int IconOffsetY { get { return 0; } } //80 is Baseline Y For Gumps

        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get { return m_FactionState; }
            set
            {
                m_FactionState = value;

                if (m_FactionState == null)
                    Hue = CraftResources.GetHue(Resource);

                LootType = (m_FactionState == null ? LootType.Regular : LootType.Blessed);
            }
        }
        #endregion

        #region Var declarations

        // Instance values. These values are unique to each weapon.
        private WeaponDamageLevel m_DamageLevel;
        private WeaponAccuracyLevel m_AccuracyLevel;
        private WeaponDurabilityLevel m_DurabilityLevel;
        private WeaponQuality m_Quality;
        private Mobile m_Crafter;
        private Poison m_Poison;
        private int m_PoisonCharges;
        private bool m_Identified;
        private int m_Hits;
        private int m_MaxHits;
        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;
        private SlayerName m_TempSlayer;
        private SkillMod m_SkillMod, m_MageMod;
        private CraftResource m_Resource;
        private bool m_PlayerConstructed;

        private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
        private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

        private AosAttributes m_AosAttributes;
        private AosWeaponAttributes m_AosWeaponAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_AosElementDamages;

        private int m_StrReq, m_DexReq, m_IntReq;
        private int m_MinDamage, m_MaxDamage;
        private int m_HitSound, m_MissSound;
        private int m_Speed;
        private int m_MaxRange;
        private SkillName m_Skill;
        private WeaponType m_Type;
        private WeaponAnimation m_Animation;              

        private BaseDungeonArmor.DungeonEnum m_Dungeon = BaseDungeonArmor.DungeonEnum.Unspecified;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDungeonArmor.DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set
            {
                m_Dungeon = value;

                BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(m_Dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }

        private int m_DungeonTier = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DungeonTier
        {
            get { return m_DungeonTier; }
            set { m_DungeonTier = value; }
        }        

        private int m_DungeonExperience = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DungeonExperience
        {
            get { return m_DungeonExperience; }
            set { m_DungeonExperience = value; }
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

        private int m_MaxBlessedCharges = DungeonWeapon.BaseMaxBlessedCharges;
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

        #endregion

        #region Virtual Properties
        public virtual WeaponAbility PrimaryAbility { get { return null; } }
        public virtual WeaponAbility SecondaryAbility { get { return null; } }

        public virtual int DefMaxRange { get { return 1; } }
        public virtual int DefHitSound { get { return 0; } }
        public virtual int DefMissSound { get { return 0; } }
        public virtual SkillName DefSkill { get { return SkillName.Swords; } }
        public virtual WeaponType DefType { get { return WeaponType.Slashing; } }
        public virtual WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

        public virtual int AosStrengthReq { get { return 0; } }
        public virtual int AosDexterityReq { get { return 0; } }
        public virtual int AosIntelligenceReq { get { return 0; } }
        public virtual int AosMinDamage { get { return 0; } }
        public virtual int AosMaxDamage { get { return 0; } }
        public virtual int AosSpeed { get { return 0; } }
        public virtual float MlSpeed { get { return 0.0f; } }
        public virtual int AosMaxRange { get { return DefMaxRange; } }
        public virtual int AosHitSound { get { return DefHitSound; } }
        public virtual int AosMissSound { get { return DefMissSound; } }
        public virtual SkillName AosSkill { get { return DefSkill; } }
        public virtual WeaponType AosType { get { return DefType; } }
        public virtual WeaponAnimation AosAnimation { get { return DefAnimation; } }

        public virtual int OldStrengthReq { get { return 0; } }
        public virtual int OldDexterityReq { get { return 0; } }
        public virtual int OldIntelligenceReq { get { return 0; } }
        public virtual int OldMinDamage { get { return 0; } }
        public virtual int OldMaxDamage { get { return 0; } }
        public virtual int OldSpeed { get { return 0; } }
        public virtual int OldMaxRange { get { return DefMaxRange; } }
        public virtual int OldHitSound { get { return DefHitSound; } }
        public virtual int OldMissSound { get { return DefMissSound; } }
        public virtual SkillName OldSkill { get { return DefSkill; } }
        public virtual WeaponType OldType { get { return DefType; } }
        public virtual WeaponAnimation OldAnimation { get { return DefAnimation; } }

        public virtual int InitMinHits { get { return 0; } }
        public virtual int InitMaxHits { get { return 0; } }

        public virtual bool CanFortify { get { return true; } }

        public override int PhysicalResistance { get { return m_AosWeaponAttributes.ResistPhysicalBonus; } }
        public override int FireResistance { get { return m_AosWeaponAttributes.ResistFireBonus; } }
        public override int ColdResistance { get { return m_AosWeaponAttributes.ResistColdBonus; } }
        public override int PoisonResistance { get { return m_AosWeaponAttributes.ResistPoisonBonus; } }
        public override int EnergyResistance { get { return m_AosWeaponAttributes.ResistEnergyBonus; } }

        public virtual SkillName DamageSkill { get { return SkillName.Tactics; } }
        #endregion

        #region Getters & Setters
        
        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get { return m_AosAttributes; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosWeaponAttributes WeaponAttributes
        {
            get { return m_AosWeaponAttributes; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses
        {
            get { return m_AosSkillBonuses; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosElementAttributes AosElementDamages
        {
            get { return m_AosElementDamages; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Cursed
        {
            get { return m_Cursed; }
            set { m_Cursed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Consecrated
        {
            get { return m_Consecrated; }
            set { m_Consecrated = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get { return m_Identified; }
            set { m_Identified = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_Hits; }
            set
            {
                if (m_Hits == value)
                    return;

                if (value > m_MaxHits)
                    value = m_MaxHits;

                m_Hits = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHits; }
            set { m_MaxHits = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get { return m_PoisonCharges; }
            set { m_PoisonCharges = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleDurability();

                m_Quality = value;

                ScaleDurability();

                if (UseSkillMod)
                {
                    if (m_Quality == WeaponQuality.Low || m_Quality == WeaponQuality.Regular)
                    {
                        if (m_SkillMod != null)
                            m_SkillMod.Remove();

                        m_SkillMod = null;
                    }

                    else if (Parent is Mobile)
                    {
                        OnEquip(Parent as Mobile);
                    }
                }

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
        {
            get { return m_Slayer; }
            set { m_Slayer = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get { return m_Slayer2; }
            set { m_Slayer2 = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName TempSlayer
        {
            get { return m_TempSlayer; }
            set { m_TempSlayer = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue(m_Resource); InvalidateProperties(); ScaleDurability(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDurabilityLevel DurabilityLevel
        {
            get { return m_DurabilityLevel; }
            set { UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get { return m_PlayerConstructed; }
            set { m_PlayerConstructed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get { return (m_MaxRange == -1 ? Core.AOS ? AosMaxRange : OldMaxRange : m_MaxRange); }
            set { m_MaxRange = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation Animation
        {
            get { return (m_Animation == (WeaponAnimation)(-1) ? Core.AOS ? AosAnimation : OldAnimation : m_Animation); }
            set { m_Animation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponType Type
        {
            get { return (m_Type == (WeaponType)(-1) ? Core.AOS ? AosType : OldType : m_Type); }
            set { m_Type = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return (m_Skill == (SkillName)(-1) ? Core.AOS ? AosSkill : OldSkill : m_Skill); }
            set { m_Skill = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get { return (m_HitSound == -1 ? Core.AOS ? AosHitSound : OldHitSound : m_HitSound); }
            set { m_HitSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MissSound
        {
            get { return (m_MissSound == -1 ? Core.AOS ? AosMissSound : OldMissSound : m_MissSound); }
            set { m_MissSound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinDamage
        {
            get 
            {
                if (DecorativeEquipment)
                    return 1;

                return (m_MinDamage == -1 ? Core.AOS ? AosMinDamage : OldMinDamage : m_MinDamage); 
            }

            set { m_MinDamage = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDamage
        {
            get 
            {
                if (DecorativeEquipment)
                    return 2;

                return (m_MaxDamage == -1 ? Core.AOS ? AosMaxDamage : OldMaxDamage : m_MaxDamage); 
            }

            set { m_MaxDamage = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get
            {
                if (m_Speed != -1)
                    return m_Speed;
                else if (Core.AOS)
                    return AosSpeed;

                return OldSpeed;
            }
            set { m_Speed = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get { return (m_StrReq == -1 ? Core.AOS ? AosStrengthReq : OldStrengthReq : m_StrReq); }
            set { m_StrReq = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexRequirement
        {
            get { return (m_DexReq == -1 ? Core.AOS ? AosDexterityReq : OldDexterityReq : m_DexReq); }
            set { m_DexReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntRequirement
        {
            get { return (m_IntReq == -1 ? Core.AOS ? AosIntelligenceReq : OldIntelligenceReq : m_IntReq); }
            set { m_IntReq = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAccuracyLevel AccuracyLevel
        {
            get { return m_AccuracyLevel; }
            set { m_AccuracyLevel = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDamageLevel DamageLevel
        {
            get
            {
                return m_DamageLevel;
            }

            set
            {
                if (m_DamageLevel != value)
                {
                    m_DamageLevel = value;

                    if (UseSkillMod)
                    {
                        if (m_DamageLevel == WeaponDamageLevel.Regular)
                        {
                            if (m_SkillMod != null)
                                m_SkillMod.Remove();

                            m_SkillMod = null;
                        }

                        if (Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }
                    }

                    InvalidateProperties();
                }
            }
        }

        #endregion

        //Begin Combat Sequence
        #region OnBeforeSwing
        public virtual void OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);

            if (a != null && !a.OnBeforeSwing(attacker, defender))
                WeaponAbility.ClearCurrentAbility(attacker);

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null && !move.OnBeforeSwing(attacker, defender))
                SpecialMove.ClearCurrentMove(attacker);
        }

        #endregion

        #region OnSwing
        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {
            return OnSwing(attacker, defender, 1.0);
        }

        public virtual TimeSpan OnSwing(Mobile attacker, Mobile defender, double damageBonus)
        {
            bool canSwing = true;

            #region AOS - Not Used
            if (Core.AOS)
            {
                canSwing = (!attacker.Paralyzed && !attacker.Frozen);

                if (canSwing)
                {
                    Spell sp = attacker.Spell as Spell;

                    canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
                }
            }
            #endregion

            if (canSwing && attacker.HarmfulCheck(defender) && !attacker.Frozen)
            {
                attacker.DisruptiveAction();

                if (attacker.NetState != null)
                    attacker.Send(new Swing(0, attacker, defender));

                if (attacker is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)attacker;
                    WeaponAbility ab = bc.GetWeaponAbility();

                    if (ab != null)
                    {
                        if (bc.WeaponAbilityChance > Utility.RandomDouble())
                            WeaponAbility.SetCurrentAbility(bc, ab);
                        else
                            WeaponAbility.ClearCurrentAbility(bc);
                    }

                    bc.OnSwing(defender);
                }

                if (CheckHit(attacker, defender))
                    OnHit(attacker, defender, damageBonus);
                else
                    OnMiss(attacker, defender);
            }

            return GetDelay(attacker, false);
        }
        #endregion

        #region GetDelay

        public virtual TimeSpan GetDelay(Mobile mobile, bool useRawValues)
        {
            int speed = this.Speed;

            if (speed == 0)
                return TimeSpan.FromHours(1.0);

            BaseCreature bc_Creature = mobile as BaseCreature;
            PlayerMobile pm_Mobile = mobile as PlayerMobile;
            
            //Player: Wrestling Speed Override
            if (pm_Mobile != null && this is Fists)
                speed = 50;

            //Creature Override for Combat Speed
            if (bc_Creature != null)            
                speed = bc_Creature.AttackSpeed;  

            //Frenzy Effect
            if (bc_Creature != null)
            {
                double totalValue;

                bc_Creature.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Frenzy, out totalValue);

                double speedMultiplier = 1;

                speedMultiplier += totalValue;

                speed = (int)((double)speed * speedMultiplier);
            }

            if (pm_Mobile != null)
            {
                double totalValue;

                pm_Mobile.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Frenzy, out totalValue);

                double speedMultiplier = 1;

                speedMultiplier += totalValue;

                speed = (int)((double)speed * speedMultiplier);
            }

            double delayInSeconds;            

            int v; 

            //Normal Creature Formula Override
            if (bc_Creature != null)
            {
                double dexBase = bc_Creature.RawDex;
                double stamBase = bc_Creature.Stam;

                if (dexBase > 125)
                    dexBase = 125;

                if (stamBase > 125)
                    stamBase = 125;

                dexBase /= 2;
                stamBase /= 2;

                if (useRawValues)
                    v = (int)(((double)bc_Creature.RawDex + 100) * speed);
                else
                    v = (int)(((double)dexBase + (double)stamBase + 100) * speed);
            }

            //Player
            else
            {
                if (useRawValues)
                    v = (int)(((double)mobile.RawDex + 100) * speed);
                else
                    v = (int)(((double)mobile.Stam + 100) * speed);
            }

            if (v <= 0)
                v = 1;

            delayInSeconds = 15000.0 / (double)v;

            if (useRawValues)
                return TimeSpan.FromSeconds(delayInSeconds);

            //Cripple Effect
            if (bc_Creature != null)
            {
                double totalValue;

                bc_Creature.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Cripple, out totalValue);

                double speedMultiplier = 1;

                speedMultiplier += totalValue;

                delayInSeconds *= speedMultiplier;
            }

            //Discordance Effect
            if (bc_Creature != null)
            {
                int discordancePenalty = 1;

                if (SkillHandlers.Discordance.GetEffect(bc_Creature, ref discordancePenalty))
                    delayInSeconds *= (1 + (double)(Math.Abs(discordancePenalty)) / 100);
            }

            return TimeSpan.FromSeconds(delayInSeconds);
        }

        #endregion

        #region OnMiss
        public virtual void OnMiss(Mobile attacker, Mobile defender)
        {
            PlaySwingAnimation(attacker);

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;
            
            if ((attacker.Body.IsHuman && attacker.Weapon is Fists) || (attacker is PlayerMobile && attacker.Weapon is Fists))
            {
                PlayerMobile player = attacker as PlayerMobile;

                bool customAttackSound = false;

                if (player != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                    if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead && attacker.AttackSound != -1)
                    {
                        attacker.PlaySound(attacker.AttackSound);
                        defender.PlaySound(attacker.AttackSound);

                        customAttackSound = true;
                    }
                }

                if (!customAttackSound)
                    Timer.DelayCall(TimeSpan.FromSeconds(.75), delegate { GetWrestlingSounds(attacker, defender, false); });
            }

            else
            {
                attacker.PlaySound(GetMissAttackSound(attacker, defender));
                defender.PlaySound(GetMissDefendSound(attacker, defender));
            }

            //Stealth Attack Cancel
            attacker.StealthAttackActive = false;            
            
            WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);

            if (ability != null)
                ability.OnMiss(attacker, defender);

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null)
                move.OnMiss(attacker, defender);
        }

        #endregion
                
        public void OnHit(Mobile attacker, Mobile defender)
        {
            OnHit(attacker, defender, 1.0);
        }

        public virtual void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if (attacker == null || defender == null)
                return;

            PlaySwingAnimation(attacker);

            if (!defender.Frozen);
                PlayHurtAnimation(defender);

            PlayerMobile pm_Attacker = attacker as PlayerMobile;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Attacker);
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Defender);

            BaseDungeonArmor.PlayerDungeonArmorProfile attackerDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(attacker, null);
            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(defender, null);

            bool dungeonArmorStealth = false;
            int effectHue = 0;

            if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat)
            {
                dungeonArmorStealth = true;
                effectHue = attackerDungeonArmor.DungeonArmorDetail.EffectHue;
            }

            bool doWeaponSpecialAttack = false;
            bool doStealthAttack = false;

            bool allowDungeonBonuses = true;
            bool allowDungeonAttack = true;

            bool immuneToSpecials = false;

            bool PlayerAttacker = (pm_Attacker != null);
            bool CreatureAttacker = (bc_Attacker != null);
            bool PlayerDefender = (pm_Defender != null);
            bool CreatureDefender = (bc_Defender != null);
            bool TamedAttacker = false;
            bool TamedDefender = false;

            if (PlayerDefender)
            {
                allowDungeonBonuses = false;
                allowDungeonAttack = false;                
            }

            if (CreatureDefender)
            {
                if (bc_Defender.ImmuneToSpecialAttacks)
                    immuneToSpecials = true;
            }

            if (CreatureAttacker)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile)
                    TamedAttacker = true;
            }

            if (CreatureDefender)
            {
                if (bc_Defender.Controlled && bc_Defender.ControlMaster is PlayerMobile)
                    TamedDefender = true;
            }

            bool allowStealthAttack = true;

            //Stealth Attack
            if (attacker.StealthAttackActive && allowStealthAttack)
            {
                attacker.PlaySound(0x51D);
                defender.PlaySound(0x51D);

                doStealthAttack = true;
                allowDungeonAttack = false;

                attacker.SendMessage("You strike your target from the shadows!");

                if (dungeonArmorStealth)                
                    Effects.SendLocationParticles(EffectItem.Create(attacker.Location, attacker.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
            }          

            //Normal Attack       
            else
            {
                bool playDefenderSound = true;

                if (bc_Defender != null)
                {
                    double ignoreSoundChance = 0;

                    if (bc_Defender.IsMiniBoss())
                        ignoreSoundChance = .75;

                    if (bc_Defender.IsBoss())
                        ignoreSoundChance = .80;

                    if (bc_Defender.IsLoHBoss())
                        ignoreSoundChance = .85;

                    if (bc_Defender.IsEventBoss())
                        ignoreSoundChance = .90;

                    if (Utility.RandomDouble() <= ignoreSoundChance)
                        playDefenderSound = false;
                }
    
                if ((attacker.Body.IsHuman && attacker.Weapon is Fists) || (PlayerAttacker && attacker.Weapon is Fists))
                {
                    if (playDefenderSound)
                    {
                        bool customAttackSound = false;
                        bool customDefendSound = false;
                        
                        if (pm_Attacker != null)
                        {
                            if (pm_Attacker.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead && pm_Attacker.AttackSound != -1)
                                customAttackSound = true;                            
                        }

                        if (customAttackSound)
                        {
                            attacker.PlaySound(attacker.AttackSound);
                            defender.PlaySound(GetHitDefendSound(attacker, defender));
                        }

                        else
                            Timer.DelayCall(TimeSpan.FromSeconds(.75), delegate { GetWrestlingSounds(attacker, defender, true); });
                    }
                }

                else
                {
                    attacker.PlaySound(GetHitAttackSound(attacker, defender));

                    if (playDefenderSound)
                        defender.PlaySound(GetHitDefendSound(attacker, defender));                
                } 
           }           
            
            //Weapon Special Attack Chance
            if (PlayerAttacker && CreatureDefender && !immuneToSpecials)
            {
                double chance = 0.1;
                double armsLoreSkillBonus = 0.15 * (attacker.Skills[SkillName.ArmsLore].Value / 100);
                double stealthAttackBonus = .25;
                double dungeonArmorBonus = 0;
                double expertiseBonus = 0;
                               
                //Special Ability Effect: Expertise
                if (pm_Attacker != null)
                    pm_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Expertise, out expertiseBonus);

                if (!doStealthAttack)
                    stealthAttackBonus = 0;

                if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat && defender is BaseCreature)
                    dungeonArmorBonus = attackerDungeonArmor.DungeonArmorDetail.SpecialWeaponAttackBonus;                

                double result = Utility.RandomDouble();
                double totalChance = chance + armsLoreSkillBonus + stealthAttackBonus + dungeonArmorBonus + expertiseBonus;      

                //Success
                if (result <= totalChance)
                {
                    doWeaponSpecialAttack = true;
                    allowDungeonAttack = false;
                }
            } 
            
            if (pm_Attacker != null)
            {
                if (pm_Attacker.IsUOACZUndead)
                    doWeaponSpecialAttack = false;

                if (pm_Attacker.IsUOACZHuman && defender is PlayerMobile)
                    doWeaponSpecialAttack = false;  
            }

            //Override fro Decorative Weapons
            if (DecorativeEquipment)
                doWeaponSpecialAttack = false;

            //Override for Training Weapons
            if (this is ButcherKnife || this is Cleaver || this is Dagger || this is SkinningKnife || this is MagicWand || this is Hatchet
                || this is Pickaxe || this is FireworksWand || this is ShepherdsCrook || this is TrainingHammer || this is TrainingBow)
            {
                doWeaponSpecialAttack = false;
            }  

            //Base Damage
            int damage = ComputeDamage(attacker, defender);
            
            //Bonus Damage Percent
            int percentageBonus = 0;

            //Damage Bonus From Weapon Ability & Special Move
            WeaponAbility weaponAbility = WeaponAbility.GetCurrentAbility(attacker);
            SpecialMove specialMove = SpecialMove.GetCurrentMove(attacker);

            if (weaponAbility != null)
                percentageBonus += (int)(weaponAbility.DamageScalar * 100) - 100;

            if (specialMove != null)
                percentageBonus += (int)(specialMove.GetDamageScalar(attacker, defender) * 100) - 100;

            percentageBonus += (int)(damageBonus * 100) - 100;

            //Damage Bonus to Creatures
            if (PlayerAttacker && CreatureDefender)
            {
                //Arms Lore Bonus: Tamed Defender
                if (bc_Defender.Controlled && bc_Defender.ControlMaster is PlayerMobile)
                    percentageBonus += (int)(10 * (attacker.Skills[SkillName.ArmsLore].Value / 100));

                //Arms Lore Bonus: Creature Defender
                else
                    percentageBonus += (int)(20 * (attacker.Skills[SkillName.ArmsLore].Value / 100));
            }

            //Slayer Weapon Damage Bonus
            CheckSlayerResult cs = CheckSlayers(attacker, defender);

            if (cs != CheckSlayerResult.None)
            {
                if (cs == CheckSlayerResult.Slayer)
                    defender.FixedEffect(0x37B9, 10, 5);

                percentageBonus += 50;
            }

            //UOACZ Silver
            if (Slayer == SlayerName.Silver || Slayer2 == SlayerName.Silver || TempSlayer == SlayerName.Silver)
            {
                bool slayerValid = false;

                int uoaczSlayerBonus = 50;

                if (defender is UOACZBaseWildlife)
                {
                    UOACZBaseWildlife wildlife = defender as UOACZBaseWildlife;

                    if (wildlife.Corrupted)
                        slayerValid = true;
                }

                if (defender is UOACZBaseUndead)
                    slayerValid = true;

                if (pm_Defender != null)
                {
                    if (pm_Defender.IsUOACZUndead)
                    {
                        slayerValid = true;
                        uoaczSlayerBonus = 25;
                    }
                }

                if (slayerValid)
                {
                    defender.FixedEffect(0x37B9, 10, 5);
                    percentageBonus += uoaczSlayerBonus;
                }
            }

            int discordancePenalty = 0;            

            #region Stealth AttackBonus

            //Stealth Attack Bonus
            int stealthBonus = 0;

            if (attacker.StealthAttackActive)
            {
                //Valid Weapon
                if (weapon != null && !immuneToSpecials)
                {                   
                    int fastestWeaponSpeedPossible = 60;
                    int slowestWeaponSpeedPossible = 20;

                    int speedBonus = weapon.OldSpeed - slowestWeaponSpeedPossible;
                    double speedScalar = 1 / ((double)fastestWeaponSpeedPossible - (double)slowestWeaponSpeedPossible);

                    double stealthWeaponBonus = 0;

                    stealthWeaponBonus = 350;
                    stealthWeaponBonus += 450 * (double)speedBonus * speedScalar;

                    if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat)                    
                        stealthWeaponBonus *= attackerDungeonArmor.DungeonArmorDetail.BackstabDamageInflictedScalar;
                    
                    //Player Attacking
                    if (PlayerAttacker)
                    {
                        if (PlayerDefender)
                            stealthBonus = 0;

                        else if (TamedDefender)
                            stealthBonus = (int)(stealthWeaponBonus * .2);

                        else
                        {
                            if (bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss())
                                stealthBonus = (int)(stealthWeaponBonus * .4);
                            else
                                stealthBonus = (int)stealthWeaponBonus;
                        }

                        if (pm_Attacker.IsUOACZHuman || pm_Attacker.IsUOACZUndead)
                            stealthBonus = (int)(stealthWeaponBonus * .5);
                    }

                    //Tamed Creature Attacking
                    else if (TamedAttacker)
                    {
                        if (PlayerDefender)
                            stealthBonus = 0;

                        else if (TamedDefender)
                            stealthBonus = 200;

                        else
                            stealthBonus = 400;

                        if (attacker.Region is UOACZRegion)
                            stealthBonus = 100;
                    }

                    //Normal Creature Attacking
                    else 
                    {
                        if (PlayerDefender)
                            stealthBonus = 50;

                        else if (TamedDefender)
                            stealthBonus = 50;
                    }
                }

                attacker.StealthAttackActive = false;
            }
            
            stealthBonus = (int)(Math.Round((double)stealthBonus * attacker.BackstabDamageScalar));

            percentageBonus += stealthBonus;

            #endregion

            //Sneak Attack Bonus: Tamed Creature Capable of Stealth Attacking a "Distracted" Combatant
            if (TamedAttacker && !PlayerDefender)
            {
                if (bc_Attacker.DictWanderAction[WanderAction.Stealth] > 0 && stealthBonus == 0 && !(bc_Attacker.Region is UOACZRegion))
                {
                    if (defender.Combatant != bc_Attacker && Utility.RandomDouble() <= .25)
                    {                        
                        bc_Attacker.PlaySound(bc_Attacker.GetAngerSound());
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*sneak attack!*");

                        int sneakBonus = 25;

                        if (PlayerDefender)
                            stealthBonus = 0;

                        else if (TamedDefender)
                            sneakBonus = 100;

                        else
                            sneakBonus = 200;

                        sneakBonus = (int)(Math.Round((double)sneakBonus * attacker.BackstabDamageScalar));

                        percentageBonus += sneakBonus;
                    }
                }
            }
            
            //Enrage Effect
            if (bc_Attacker != null)
            {
                double totalValue;

                bc_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Enrage, out totalValue);

                int enrageBonus = (int)(totalValue * 100);

                percentageBonus += enrageBonus;                
            }

            if (pm_Attacker != null)
            {
                double totalValue;

                pm_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Enrage, out totalValue);

                int enrageBonus = (int)(totalValue * 100);

                percentageBonus += enrageBonus;
            }

            //Iron Fists
            if (pm_Attacker != null && weapon is Fists)
            {
                double totalValue;

                pm_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.IronFists, out totalValue);

                if (defender is PlayerMobile)
                    totalValue *= .5;
                
                int ironFistBonus = (int)(totalValue * 100);
                
                percentageBonus += ironFistBonus;
            }                          

            damage = AOS.Scale(damage, 100 + percentageBonus);

            //Attacker is Creature: If Defender is Player and Has Armor Set Bonus (Will reduce damage to player)
            if (CreatureAttacker)
                ((BaseCreature)attacker).AlterMeleeDamageTo(defender, ref damage);

            //Defender is Creature: If Attack is Player and Has Dungeon Suit (Will increase damage to monster)
            if (CreatureDefender)
                ((BaseCreature)defender).AlterMeleeDamageFrom(attacker, ref damage);
            
            double MeleeDamageInflictedScalar = 1.0;
            double MeleeDamageReceivedScalar = 1.0;

            if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat)
                MeleeDamageInflictedScalar = attackerDungeonArmor.DungeonArmorDetail.MeleeDamageInflictedScalar;

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)
                MeleeDamageReceivedScalar = defenderDungeonArmor.DungeonArmorDetail.MeleeDamageReceivedScalar;
            
            //Player Attacking
            if (PlayerAttacker)
            {
                if (PlayerDefender)
                {
                    if (pm_Attacker.IsUOACZHuman)
                        damage = (int)((double)damage * 0.9 * UOACZSystem.HumanPlayerVsPlayerDamageScalar * UOACZSystem.GetFatigueScalar(pm_Attacker)); 
     
                    else if (pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 0.9 * UOACZSystem.UndeadPlayerVsPlayerDamageScalar * UOACZSystem.GetFatigueScalar(pm_Attacker)); 

                    else
                        damage = (int)((double)damage * 0.9);                              
                }

                else if (TamedDefender)
                {
                    if (pm_Attacker.IsUOACZHuman || pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 1.3 * MeleeDamageInflictedScalar * UOACZSystem.GetFatigueScalar(pm_Attacker));

                    else
                        damage = (int)((double)damage * 1.3 * MeleeDamageInflictedScalar);
                }

                else
                {
                    if (pm_Attacker.IsUOACZHuman)
                        damage = (int)((double)damage * 1.1 * UOACZSystem.HumanPlayerVsCreatureDamageScalar);

                    else if (pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 1.1 * UOACZSystem.UndeadPlayerVsCreatureDamageScalar);

                    else
                        damage = (int)((double)damage * 1.1 * MeleeDamageInflictedScalar);
                }                
            }

            //Tamed Creature Attacking
            else if (TamedAttacker)
            {
                PlayerMobile pm_Controller = bc_Attacker.ControlMaster as PlayerMobile;

                if (PlayerDefender)
                {
                    if (pm_Controller.IsUOACZHuman || pm_Controller.IsUOACZUndead)
                        damage = (int)((double)damage * 1 * bc_Attacker.PvPMeleeDamageScalar * MeleeDamageReceivedScalar * UOACZSystem.GetFatigueScalar(pm_Controller));
                    
                    else
                        damage = (int)((double)damage * 1 * bc_Attacker.PvPMeleeDamageScalar * MeleeDamageReceivedScalar);
                }

                else if (TamedDefender)
                {
                    if (pm_Controller.IsUOACZHuman || pm_Controller.IsUOACZUndead)
                        damage = (int)((double)damage * 1.25 * UOACZSystem.GetFatigueScalar(pm_Controller));

                    else
                        damage = (int)((double)damage * 1.25);
                }

                else
                {
                    if (UOACZSystem.IsUOACZValidMobile(bc_Attacker))
                        damage = (int)((double)damage * 1 * UOACZSystem.TamedCreatureVsCreatureDamageScalar);

                    else
                        damage = (int)((double)damage * 1);
                }
            }

            //Normal Creature Attacking
            else 
            {
                double ProvokedCreatureDamageInflictedScalar = 1.0;

                if (bc_Attacker.BardMaster != null)
                {
                    BaseDungeonArmor.PlayerDungeonArmorProfile bardMasterDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(bc_Attacker.BardMaster, null);

                    if (bardMasterDungeonArmor.MatchingSet && !bardMasterDungeonArmor.InPlayerCombat)
                        ProvokedCreatureDamageInflictedScalar = bardMasterDungeonArmor.DungeonArmorDetail.ProvokedCreatureDamageInflictedScalar;
                }

                if (PlayerDefender)
                    damage = (int)((double)damage * 1 * MeleeDamageReceivedScalar);

                else if (TamedDefender)
                {
                    damage = (int)((double)damage * 1.25 * ProvokedCreatureDamageInflictedScalar);

                    if (UOACZSystem.IsUOACZValidMobile(bc_Defender))
                        damage = (int)((double)damage * 1 * UOACZSystem.CreatureVsTamedCreatureDamageScalar);
                }                             
            }
            
            //Armor Absorption: Player vs Player Melee Hits Use Standard OSI Armor Damage Reduction. All Other Melee Hits Use Custom Formulas
            if (PlayerAttacker && PlayerDefender)
                damage = defender.AbsorbDamage(attacker, damage, true, true);

            else
                damage = defender.AbsorbDamage(attacker, damage, true, false);            
                        
            //Weapon and Shield Parrying
            BaseShield defenderShield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;
            BaseWeapon defenderWeapon = defender.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (defenderShield != null)
                damage = defenderShield.OnHit(this, damage, attacker);

            else if (defenderWeapon != null)
            {
                if (!(defenderWeapon is BaseRanged) && !(attacker is PlayerMobile) && (defender is PlayerMobile))
                    damage = defenderWeapon.WeaponParry(defenderWeapon, damage, defender);
            }
                        
            #region Weapon Special Attack Effects

            //Resolve Weapon Special Ability Effects
            if (doWeaponSpecialAttack)
            {
                if (weapon != null || weapon is Fists)
                {
                    SpecialAbilityEffect effect = SpecialAbilityEffect.Bleed;

                    double speed = (double)weapon.OldSpeed;

                    double minSpeed = 25;
                    double maxSpeed = 60;
                    double speedRange = maxSpeed - minSpeed;
                    double speedInterval = 1 / speedRange;

                    double minDuration = 3;
                    double maxDuration = 5;

                    double expirationSeconds = 3;
                    double value = 1;

                    defender.FixedEffect(0x5683, 10, 20);

                    //Wrestling
                    if (weapon is Fists)
                    {
                        value = .2;
                        expirationSeconds = 12;                         

                        if (CreatureDefender)                        
                            SpecialAbilities.StunSpecialAbility(1, attacker, bc_Defender, value, expirationSeconds, 0x510, true, "Your strike stuns your target!", "Their attack stuns you!");
                    }

                    //Weapon Attack
                    else
                    {
                        switch (weapon.DefSkill)
                        {
                            case SkillName.Archery:
                                minDuration = 3;
                                maxDuration = 6;

                                value = 1;
                                expirationSeconds = maxDuration - ((maxDuration - minDuration) * (speed - minSpeed) * speedInterval);

                                if (doStealthAttack)
                                    expirationSeconds = maxDuration;                           

                                if (CreatureDefender)                                
                                    SpecialAbilities.HinderSpecialAbility(1, attacker, bc_Defender, value, expirationSeconds, false, 0x51c, true, "Your shot hinders your target!", "Their attack hinders you!");
                            break;

                            case SkillName.Fencing:
                                minDuration = 6;
                                maxDuration = 12;

                                value = .33;
                                expirationSeconds = maxDuration - ((maxDuration - minDuration) * (speed - minSpeed) * speedInterval);

                                damage += (int)Math.Round((double)damage * .5);

                                if (doStealthAttack)
                                    expirationSeconds = maxDuration;

                                SpecialAbilities.CrippleSpecialAbility(1, attacker, bc_Defender, value, expirationSeconds, 0x520, true, "Your attack slows your target!", "Their attack slows you!");
                            break;

                            case SkillName.Macing:
                                double stamPercent = (double)defender.Stam / (double)defender.StamMax;

                                double baseDamage = (double)damage * .5;
                                double extraDamage = (double)damage * .75 * (1 - stamPercent);
                                double crushDamage = (int)Math.Round((baseDamage + extraDamage));

                                if (doStealthAttack)
                                    crushDamage *= .33;

                                if (crushDamage < 1)
                                    crushDamage = 1;                                

                                if (CreatureDefender)
                                {
                                    damage += (int)crushDamage;

                                    attacker.SendMessage("You land a crushing blow on your target!");
                                    defender.SendMessage("They land a crushing blow on you!");

                                    defender.FixedEffect(0x5683, 10, 20);
                                    Effects.PlaySound(attacker.Location, attacker.Map, 0x525);
                                }
                            break;

                            case SkillName.Swords:
                                expirationSeconds = 8;
                                value = damage;

                                if (doStealthAttack)
                                    value *= .33;

                                if (CreatureDefender)                                
                                    SpecialAbilities.BleedSpecialAbility(1, attacker, bc_Defender, value, expirationSeconds, 0x51e, true, "Your attack causes your target to bleed!", "Their attack causes you to bleed!");
                                
                            break;
                        }
                    }
                }
            }

            #endregion
            
            AddBlood(attacker, defender, damage);

            //Override for Melee Attacks (Damage Absorption Calculated within BaseWeapon: Normally Occurs within AoS.Damage)
            int phys = 0;
            int fire = 100;
            int cold = 0;
            int pois = 0;
            int nrgy = 0;
            int chaos = 0;
            int direct = 0;

            //GetDamageTypes(attacker, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);
            
            int damageGiven = damage;

            //Weapon Ability has OnBefore Damage Effects
            if (weaponAbility != null && !weaponAbility.OnBeforeDamage(attacker, defender))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
                weaponAbility = null;
            }

            //Special Move has OnBefore Damage Effects
            if (specialMove != null && !specialMove.OnBeforeDamage(attacker, defender))
            {
                SpecialMove.ClearCurrentMove(attacker);
                specialMove = null;
            }

            int adjustedDamageDisplayed = damage;

            if (bc_Defender != null)
            {
                //Discordance
                if (SkillHandlers.Discordance.GetEffect(bc_Defender, ref discordancePenalty))
                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * (1 + (double)(Math.Abs(discordancePenalty)) / 100));

                //Ship Combat
                if (BaseBoat.UseShipBasedDamageModifer(attacker, bc_Defender))
                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToCreatureScalar); 
            }
            
            if (pm_Defender != null)
            {
                //Ship Combat
                if (BaseBoat.UseShipBasedDamageModifer(attacker, pm_Defender))
                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToPlayerScalar); 
            }

            #region Reactive Armor

            //Enhanced Spellbook: Wizard --- Player Can Get Above 20, But Only Should Be Against Monsters
            if (attacker is PlayerMobile && defender.MeleeDamageAbsorb > 20 && !(defender.Region is UOACZRegion))
                defender.MeleeDamageAbsorb = 20;

            int reactiveArmorAbsorption = defender.MeleeDamageAbsorb;

            if (reactiveArmorAbsorption > 0)
            {
                if (reactiveArmorAbsorption > damage)
                {
                    int react = damage / 5;

                    defender.MeleeDamageAbsorb -= Math.Max(damage, 2);
                    damage = 0;

                    attacker.Damage(react, defender);

                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(defender, HueableSpell.ReactiveArmor);

                    attacker.PlaySound(0x1F1);
                    attacker.FixedEffect(0x374A, 10, 16, spellHue, 0);
                }

                else
                {
                    damage -= reactiveArmorAbsorption;
                    defender.MeleeDamageAbsorb = 0;
                    defender.SendLocalizedMessage(1005556); // Your reactive armor spell has been nullified.

                    DefensiveSpell.Nullify(defender);
                }
            }

            #endregion           

            if (damage < 1)
                damage = 1;

            //Display Player Melee Damage
            if (PlayerAttacker)
            {
                if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                    pm_Attacker.SendMessage(pm_Attacker.PlayerMeleeDamageTextHue, "You attack " + defender.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                    defender.PrivateOverheadMessage(MessageType.Regular, pm_Attacker.PlayerMeleeDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Attacker.NetState);
            }

            //Display Follower Melee Damage
            if (CreatureAttacker)
            {
                if ((bc_Attacker is BladeSpirits || bc_Attacker is EnergyVortex) && bc_Attacker.SummonMaster is PlayerMobile)
                {
                    PlayerMobile pm_Controller = bc_Attacker.SummonMaster as PlayerMobile;

                    if (bc_Attacker.GetDistanceToSqrt(pm_Controller) <= 20)
                    {
                        if (pm_Controller.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                            pm_Controller.SendMessage(pm_Controller.PlayerFollowerDamageTextHue, "Follower: " + bc_Attacker.Name + " attacks " + defender.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                        if (pm_Controller.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, pm_Controller.PlayerFollowerDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Controller.NetState);
                    }
                }

                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile)
                {
                    PlayerMobile pm_Controller = bc_Attacker.ControlMaster as PlayerMobile;

                    if (bc_Attacker.GetDistanceToSqrt(pm_Controller) <= 20)
                    {
                        if (pm_Controller.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                            pm_Controller.SendMessage(pm_Controller.PlayerFollowerDamageTextHue, "Follower: " + bc_Attacker.Name + " attacks " + defender.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                        if (pm_Controller.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, pm_Controller.PlayerFollowerDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Controller.NetState);
                    }
                }
            }

            //Provoked Creature Melee Damage
            if (CreatureAttacker)
            {
                if (bc_Attacker.BardProvoked && bc_Attacker.BardMaster is PlayerMobile)
                {
                    PlayerMobile playerBard = bc_Attacker.BardMaster as PlayerMobile;

                    if (bc_Attacker.GetDistanceToSqrt(playerBard) <= 20)
                    {
                        if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                            playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Attacker.Name + " inflicts " + adjustedDamageDisplayed.ToString() + " damage on " + defender.Name + ".");

                        if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerBard.NetState);
                    }
                }
            }
            
            //Assign Final Damage to Target
            damageGiven = AOS.Damage(defender, attacker, damage, false, phys, fire, cold, pois, nrgy);

            //Dungeon Weapon
            if (DungeonTier > 0 && allowDungeonBonuses && pm_Attacker != null && bc_Defender != null)
            {
                //Damage Tracking: Used for Experience Gains
                bool entryFound = false;

                foreach (DungeonWeaponDamageEntry dungeonWeaponDamageEntry in bc_Defender.DungeonWeaponDamageEntries)
                {
                    if (dungeonWeaponDamageEntry.Weapon == this)
                    {
                        dungeonWeaponDamageEntry.Damage += damageGiven;
                        entryFound = true;

                        break;
                    }
                }

                if (!entryFound)
                {
                    DungeonWeaponDamageEntry dungeonWeaponDamageEntry = new DungeonWeaponDamageEntry();
                    dungeonWeaponDamageEntry.Player = pm_Attacker;
                    dungeonWeaponDamageEntry.Weapon = this;
                    dungeonWeaponDamageEntry.Damage = damageGiven;

                    bc_Defender.DungeonWeaponDamageEntries.Add(dungeonWeaponDamageEntry);
                }

                //Effect Chance
                if (allowDungeonAttack)
                    DungeonWeapon.CheckResolveSpecialEffect(this, pm_Attacker, bc_Defender);                              
            }            

            //Weapon Takes Durability Loss on Hit (4% Chance)
            if (m_MaxHits > 0 && Utility.Random(25) == 0 && LootType != Server.LootType.Blessed)
            {
                int wepDamage = GetGuildReducedDamage(1);

                if (m_Hits > 0)
                    HitPoints -= wepDamage;

                //Loses Permanent Durability
                else if (m_MaxHits > 1)
                {
                    MaxHitPoints -= wepDamage;

                    if (Parent is Mobile)
                        ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                }

                //Weapon Breaks
                else
                    Delete();
            }                      

            //Player Attack Post-Hit Effects
            if (PlayerAttacker)            
                pm_Attacker.OnGaveMeleeAttack(defender);

            //Player Defending Post-Hit Effects
            if (PlayerDefender)
                pm_Defender.OnGotMeleeAttack(attacker);    

            //Creature Attack Post-Hit Effects
            if (CreatureAttacker)
                bc_Attacker.OnGaveMeleeAttack(defender);

            //Creature Defending Post-Hit Effects
            if (CreatureDefender)
                bc_Defender.OnGotMeleeAttack(attacker);

            //Weapon Ability Post-Hit Effects
            if (weaponAbility != null)
                weaponAbility.OnHit(attacker, defender, damage);

            //Special Move Post-Hit Effects
            if (specialMove != null)
                specialMove.OnHit(attacker, defender, damage); 
        }

        public int GetGuildReducedDamage(int wear)
        {
            var parent = this.Parent as Mobile;
            if (parent != null && parent.Guild != null && parent.Guild is Guild)
            {
                var guild = parent.Guild as Guild;
                bool procRateMet = Utility.RandomDouble() <= guild.BonusChance(GuildBonus.LessWeaponDamage);
                if (guild.HasBonus(GuildBonus.LessWeaponDamage) && procRateMet)
                    return wear - 1;
            }
            return wear;
        }
        
        public int WeaponParry(BaseWeapon weapon, int damage, Mobile defender)
        {
            if (weapon == null || defender == null)
                return damage;

            Mobile owner = weapon.Parent as Mobile;

            if (owner == null)
                return damage;

            double parrySkill = owner.Skills[SkillName.Parry].Value;
            double baseChance = (parrySkill / 100.0) / 2.0;
            double chanceBonus = .167 * (owner.Skills[SkillName.ArmsLore].Value / 100);

            double chance = baseChance + chanceBonus;

            if (chance < 0.01)
                chance = 0.01;

            //Successful Parry Check
            if (owner.CheckSkill(SkillName.Parry, chance))
            {
                double reduction = 0.65; // 35% base reduction
                // reduce damage 5% further for each mod
                if (Quality == WeaponQuality.Exceptional)
                    reduction -= 0.12;

                else
                    reduction -= (((int)m_DamageLevel + (int)m_AccuracyLevel) / 2) * 0.06;

                damage = (int)(damage * reduction);

                if (owner.AccessLevel > AccessLevel.Player)
                    owner.PrivateOverheadMessage(Network.MessageType.Regular, 0x22, false, String.Format("Parry Damage Modifier: {0:0.00}", 1-reduction), owner.NetState);

                //Parry Visual Effect
                owner.FixedEffect(0x37B9, 10, 16);

                //(5% Chance of Durability Damage)
                if (5 > Utility.Random(100) && LootType != LootType.Blessed)
                {
                    int wear = Utility.Random(2);

                    //66% Chance of Possible Durability Damage
                    if (wear > 0 && MaxHitPoints > 0)
                    {
                        //Durability Damage
                        if (HitPoints >= wear)
                        {
                            HitPoints -= wear;
                            wear = 0;
                        }

                        else
                        {
                            wear -= HitPoints;
                            HitPoints = 0;
                        }

                        if (wear > 0)
                        {
                            //Weapon Almost Broken
                            if (MaxHitPoints > wear)
                            {
                                MaxHitPoints -= wear;

                                if (Parent is Mobile)
                                    ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }

                            //Weapon
                            else
                            {
                                Delete();
                            }
                        }
                    }
                }
            }

            return damage;
        }

        public virtual WeaponAnimation GetAnimation()
        {
            return Animation;
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseWeapon weap = newItem as BaseWeapon;

            if (weap == null)
                return;

            weap.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            weap.m_AosElementDamages = new AosElementAttributes(newItem, m_AosElementDamages);
            weap.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            weap.m_AosWeaponAttributes = new AosWeaponAttributes(newItem, m_AosWeaponAttributes);
        }

        public virtual void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
            m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;
            InvalidateProperties();
        }

        public virtual void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * scale) + 99) / 100;
            m_MaxHits = ((m_MaxHits * scale) + 99) / 100;
            InvalidateProperties();
        }

        public int GetDurabilityBonus()
        {
            int bonus = 0;

            if (m_Quality == WeaponQuality.Exceptional)
                bonus += 20;

            switch (m_DurabilityLevel)
            {
                case WeaponDurabilityLevel.Durable: bonus += 20; break;
                case WeaponDurabilityLevel.Substantial: bonus += 50; break;
                case WeaponDurabilityLevel.Massive: bonus += 70; break;
                case WeaponDurabilityLevel.Fortified: bonus += 100; break;
                case WeaponDurabilityLevel.Indestructible: bonus += 120; break;
            }

            if (Core.AOS)
            {
                bonus += m_AosWeaponAttributes.DurabilityBonus;

                CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);
                CraftAttributeInfo attrInfo = null;

                if (resInfo != null)
                    attrInfo = resInfo.AttributeInfo;

                if (attrInfo != null)
                    bonus += attrInfo.WeaponDurability;
            }

            return bonus;
        }

        public int GetLowerStatReq()
        {
            if (!Core.AOS)
                return 0;

            int v = m_AosWeaponAttributes.LowerStatReq;

            CraftResourceInfo info = CraftResources.GetInfo(m_Resource);

            if (info != null)
            {
                CraftAttributeInfo attrInfo = info.AttributeInfo;

                if (attrInfo != null)
                    v += attrInfo.WeaponLowerRequirements;
            }

            if (v > 100)
                v = 100;

            return v;
        }

        public static void BlockEquip(Mobile m, TimeSpan duration)
        {
            if (m.BeginAction(typeof(BaseWeapon)))
                new ResetEquipTimer(m, duration).Start();
        }

        private class ResetEquipTimer : Timer
        {
            private Mobile m_Mobile;

            public ResetEquipTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(BaseWeapon));
            }
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
                return true;

            if (this.Layer == Layer.TwoHanded && layer == Layer.OneHanded)
            {
                m.SendLocalizedMessage(500214); // You already have something in both hands.
                return true;
            }
            else if (this.Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight))
            {
                m.SendLocalizedMessage(500215); // You can only wield one weapon at a time.
                return true;
            }

            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            if (!Ethics.Ethic.CheckTrade(from, to, newOwner, this))
                return false;

            return base.AllowSecureTrade(from, to, newOwner, accepted);
        }

        public virtual Race RequiredRace { get { return null; } }	//On OSI, there are no weapons with race requirements, this is for custom stuff

        public override bool CanEquip(Mobile from)
        {
            if (!Ethics.Ethic.CheckEquip(from, this))
                return false;

            if (RequiredRace != null && from.Race != RequiredRace)
            {
                if (RequiredRace == Race.Elf)
                    from.SendLocalizedMessage(1072203); // Only Elves may use this.
                else
                    from.SendMessage("Only {0} may use this.", RequiredRace.PluralName);

                return false;
            }

            else if (from.Dex < DexRequirement)
            {
                from.SendMessage("You are not nimble enough to equip that.");
                return false;
            }

            else if (from.Str < AOS.Scale(StrRequirement, 100 - GetLowerStatReq()))
            {
                from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                return false;
            }

            else if (from.Int < IntRequirement)
            {
                from.SendMessage("You are not smart enough to equip that.");
                return false;
            }

            else if (!from.CanBeginAction(typeof(BaseWeapon)))
            {
                return false;
            }

            else
            {
            }

            return base.CanEquip(from);
        }

        public virtual bool UseSkillMod { get { return true; } }

        public override bool OnEquip(Mobile from)
        {
            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if ((strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = from;

                string modName = this.Serial.ToString();

                if (strBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                if (dexBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                if (intBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
            }

            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon != null)
            {
                TimeSpan timeDifference = this.GetDelay(from, false) - weapon.GetDelay(from, false);
                from.NextCombatTime = from.NextCombatTime.AddSeconds(timeDifference.TotalSeconds);
            }

            if (this is BaseRanged)
            {
                DateTime nextFireMinimum = DateTime.UtcNow + TimeSpan.FromMilliseconds((double)BaseRanged.RangedShotDelay(from.Dex));

                if (from.NextCombatTime < nextFireMinimum)
                    from.NextCombatTime = nextFireMinimum;
            }

            if (UseSkillMod)
            {
                if (m_SkillMod != null)
                    m_SkillMod.Remove();

                int mod = (int)m_DamageLevel * 5;

                if (m_Quality == WeaponQuality.Exceptional)
                    mod += 10;

                if (DungeonTier > 0)
                    mod += DungeonWeapon.BaseTactics + (DungeonTier * DungeonWeapon.TacticsPerTier);

                if (mod > 0)
                {
                    m_SkillMod = new DefaultSkillMod(DamageSkill, true, mod);
                    from.AddSkillMod(m_SkillMod);
                }
            }

            if (Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30)
            {
                if (m_MageMod != null)
                    m_MageMod.Remove();

                m_MageMod = new DefaultSkillMod(SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon);
                from.AddSkillMod(m_MageMod);
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                if (Core.AOS)
                    m_AosSkillBonuses.AddTo(from);

                from.CheckStatTimers();
                from.Delta(MobileDelta.WeaponDamage);
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;
                BaseWeapon weapon = m.Weapon as BaseWeapon;

                string modName = this.Serial.ToString();

                m.RemoveStatMod(modName + "Str");
                m.RemoveStatMod(modName + "Dex");
                m.RemoveStatMod(modName + "Int");

                if (weapon != null)
                {
                    TimeSpan timeDifference = weapon.GetDelay(m, false) - this.GetDelay(m, false);
                    m.NextCombatTime = m.NextCombatTime.AddSeconds(timeDifference.TotalSeconds);

                    //TimeSpan t = -this.GetDelay(m) + weapon.GetDelay(m);
                    //m.NextCombatTime += (int)t.TotalMilliseconds;
                }

                if (UseSkillMod && m_SkillMod != null)
                {
                    m_SkillMod.Remove();
                    m_SkillMod = null;
                }

                if (m_MageMod != null)
                {
                    m_MageMod.Remove();
                    m_MageMod = null;
                }

                if (Core.AOS)
                    m_AosSkillBonuses.Remove();

                ImmolatingWeaponSpell.StopImmolating(this);

                m.CheckStatTimers();

                m.Delta(MobileDelta.WeaponDamage);
            }
        }

        public virtual SkillName GetUsedSkill(Mobile m, bool checkSkillAttrs)
        {
            SkillName sk;

            if (checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0)
            {
                double swrd = m.Skills[SkillName.Swords].Value;
                double fenc = m.Skills[SkillName.Fencing].Value;
                double arch = m.Skills[SkillName.Archery].Value;
                double mcng = m.Skills[SkillName.Macing].Value;
                double val;

                sk = SkillName.Swords;
                val = swrd;

                if (fenc > val) { sk = SkillName.Fencing; val = fenc; }
                if (arch > val) { sk = SkillName.Archery; val = arch; }
                if (mcng > val) { sk = SkillName.Macing; val = mcng; }
            }

            else if (m_AosWeaponAttributes.MageWeapon != 0)
            {
                if (m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value)
                    sk = SkillName.Magery;
                else
                    sk = Skill;
            }

            else
            {
                sk = Skill;

                if (sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value)
                    sk = SkillName.Wrestling;
            }

            return sk;
        }

        public virtual double GetAttackSkillValue(Mobile attacker, Mobile defender)
        {
            return attacker.Skills[GetUsedSkill(attacker, true)].Value;
        }

        public virtual double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            return defender.Skills[GetUsedSkill(defender, true)].Value;
        }

        private static bool CheckAnimal(Mobile m, Type type)
        {
            return AnimalForm.UnderTransformation(m, type);
        }

        public virtual bool CheckHit(Mobile attacker, Mobile defender)
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];
            Skill defSkill = defender.Skills[defWeapon.Skill];
            
            double atkValue = atkWeapon.GetAttackSkillValue(attacker, defender);
            double defValue = defWeapon.GetDefendSkillValue(attacker, defender);

            PlayerMobile pm_Attacker = attacker as PlayerMobile;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;

            bool PlayerAttacker = (pm_Attacker != null);
            bool CreatureAttacker = (bc_Attacker != null);
            bool PlayerDefender = (pm_Defender != null);
            bool CreatureDefender = (bc_Defender != null);
            bool TamedAttacker = false;
            bool TamedDefender = false;
            
            if (CreatureAttacker)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile)
                    TamedAttacker = true;
            }

            if (CreatureDefender)
            {
                if (bc_Defender.Controlled && bc_Defender.ControlMaster is PlayerMobile)
                    TamedDefender = true;
            }

            //Tamed Creature Skill Caps for Combat
            if (bc_Attacker != null && defender is PlayerMobile)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile)
                {
                    if (atkValue > 100)
                        atkValue = 100;
                }
            }
            
            if (bc_Defender != null)
            {
                if (bc_Defender.Controlled && bc_Defender.ControlMaster is PlayerMobile)
                {
                    if (defValue > 100)
                        defValue = 100;

                    if (attacker is PlayerMobile)
                    {
                        if (defValue > 100)
                            defValue = 100;
                    }
                }
            }

            //UOACZ Defensive Wrestling PvP Cap
            if (UOACZSystem.IsUOACZValidMobile(attacker) && UOACZSystem.IsUOACZValidMobile(defender))
            {
                if ((PlayerAttacker || TamedAttacker) && PlayerDefender)
                {
                    if (defValue > 100)
                        defValue = 100;
                }
            }
            
            #region Weapon Special Attack: Stun ToHit Bonuses/Penalties

            //Special Ability Effect on Creature: Stun
            if (bc_Attacker != null)
            {
                double totalValue;

                bc_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Stun, out totalValue);

                atkValue -= totalValue;

                if (atkValue < 0)
                    atkValue = 0;
            }

            if (bc_Defender != null)
            {
                double totalValue;

                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Stun, out totalValue);

                defValue -= totalValue;

                if (defValue < 0)
                    defValue = 0;
            }            

            #endregion

            double ourValue, theirValue;

            if (atkValue <= -50.0)
                atkValue = -49.9;

            if (defValue <= -50.0)
                defValue = -49.9;

            ourValue = (atkValue + 50.0);
            theirValue = (defValue + 50.0);

            double chance = ourValue / (theirValue * 2.0);
            
            double weaponBonus = GetHitChanceBonus(attacker, defender);

            chance += weaponBonus;

            double courageBonus = 0;

            //Special Ability Effect: Courage
            if (bc_Attacker != null)
            {
                bc_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Courage, out courageBonus);
                chance += (int)(courageBonus * 100);
            }

            else if (pm_Attacker != null)
            {
                pm_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Courage, out courageBonus);
                chance += (int)(courageBonus * 100);
            }

            //Special Ability Effect: Evasion
            double evasionBonus = 0;

            if (bc_Defender != null)
            {
                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Evasion, out evasionBonus);
                chance -= (int)(courageBonus * 100);
            }

            else if (pm_Defender != null)
            {
                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Evasion, out evasionBonus);
                chance -= (int)(evasionBonus * 100);
            }

            #region StealthAttack ToHit Bonus

            //Stealth Attack Accuracy Bonus
            if (attacker.StealthAttackActive)
            {
                //Player Attacking
                if (PlayerAttacker)
                {
                    if (PlayerDefender)
                        chance += 0;

                    else if (TamedDefender)
                        chance += .125;

                    else
                        chance += .25;
                }

                //Tamed Creature Attacking
                else if (TamedAttacker)
                {
                    if (PlayerDefender)
                       chance += 0;

                    else if (TamedDefender)
                        chance += .125;

                    else
                        chance += .25;
                }

                //Normal Creature Attacking
                else
                {
                    if (PlayerDefender)
                        chance += .25;

                    else if (TamedDefender)
                        chance += .25;
                }
            }

            #endregion

            //Dungeon Weapon
            if (DungeonTier > 0 && pm_Defender == null)
                chance += DungeonWeapon.BaseAccuracy * (DungeonWeapon.AccuracyPerTier * (double)DungeonTier);            
            
            if (defender is PlayerMobile && ((PlayerMobile)defender).m_DateTimeDied + TimeSpan.FromSeconds(60) > DateTime.UtcNow)
                return (Utility.RandomDouble() < chance);

            else
                return attacker.CheckSkill(atkSkill.SkillName, chance);
        }        

        #region Sounds
        public virtual int GetHitAttackSound(Mobile attacker, Mobile defender)
        {
            int sound = attacker.GetAttackSound();

            if (sound == -1)
                sound = HitSound;

            return sound;
        }

        public virtual int GetHitDefendSound(Mobile attacker, Mobile defender)
        {
            PlayerMobile defenderPlayer = defender as PlayerMobile;

            if (defenderPlayer != null)
            {
                UOACZPersistance.CheckAndCreateUOACZAccountEntry(defenderPlayer);

                if (defenderPlayer.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                {
                    int sound;

                    if (defenderPlayer.Female)
                        sound = Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B);
                    else
                        sound = Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x5F6, 0x436, 0x437, 0x43B, 0x43C);

                    return sound;
                }
            }

            if (defender is UOACZBaseHuman)
            {
                int sound;

                if (defender.Female)
                    sound = Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B);
                else
                    sound = Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x5F6, 0x436, 0x437, 0x43B, 0x43C);

                return sound;
            }

            return defender.GetHurtSound();
        }

        public virtual int GetMissAttackSound(Mobile attacker, Mobile defender)
        {
            if (attacker.GetAttackSound() == -1)
                return MissSound;
            else
                return -1;
        }

        public virtual int GetMissDefendSound(Mobile attacker, Mobile defender)
        {
            return -1;
        }
        #endregion

        public static bool CheckParry(Mobile defender) //ADDED BY IPY FACTIONS
        {
            return CheckParry(defender, null);
        }

        public static bool CheckParry(Mobile defender, Mobile attacker)
        {
            if (defender == null)
                return false;

            BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            double parry = defender.Skills[SkillName.Parry].Value;

            if (shield != null)
            {
                double chance = parry / 400.0;	// As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

                if (chance < 0) // chance shouldn't go below 0
                    chance = 0;

                // Parry/Bushido over 100 grants a 5% bonus.
                if (parry >= 100.0)
                    chance += 0.05;

                // Low dexterity lowers the chance.
                if (defender.Dex < 80)
                    chance = chance * (20 + defender.Dex) / 100;

                return defender.CheckSkill(SkillName.Parry, chance);
            }

            else if (!(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged))
            {
                BaseWeapon weapon = defender.Weapon as BaseWeapon;

                double divisor = (weapon.Layer == Layer.OneHanded) ? 48000.0 : 41140.0;

                double chance = parry / divisor;

                // Parry or Bushido over 100 grant a 5% bonus.
                if (parry >= 100.0)
                {
                    chance += 0.05;
                }

                // Low dexterity lowers the chance.
                if (defender.Dex < 80)
                    chance = chance * (20 + defender.Dex) / 100;

                return defender.CheckSkill(SkillName.Parry, chance);
            }

            return false;
        }

        public virtual int GetPackInstinctBonus(Mobile attacker, Mobile defender)
        {
            if (attacker.Player || defender.Player)
                return 0;

            BaseCreature bc = attacker as BaseCreature;

            if (bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controlled && !bc.Summoned))
                return 0;

            Mobile master = bc.ControlMaster;

            if (master == null)
                master = bc.SummonMaster;

            if (master == null)
                return 0;

            int inPack = 1;

            IPooledEnumerable eable = defender.GetMobilesInRange(1);
            foreach (Mobile m in eable)
            {
                if (m != attacker && m is BaseCreature)
                {
                    BaseCreature tc = (BaseCreature)m;

                    if ((tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controlled && !tc.Summoned))
                        continue;

                    Mobile theirMaster = tc.ControlMaster;

                    if (theirMaster == null)
                        theirMaster = tc.SummonMaster;

                    if (master == theirMaster && tc.Combatant == defender)
                        ++inPack;
                }
            }
            eable.Free();

            if (inPack >= 5)
                return 100;
            else if (inPack >= 4)
                return 75;
            else if (inPack >= 3)
                return 50;
            else if (inPack >= 2)
                return 25;

            return 0;
        }

        private static bool m_InDoubleStrike;

        public static bool InDoubleStrike
        {
            get { return m_InDoubleStrike; }
            set { m_InDoubleStrike = value; }
        }

        public virtual double GetAosDamage(Mobile attacker, int bonus, int dice, int sides)
        {
            int damage = Utility.Dice(dice, sides, bonus) * 100;
            int damageBonus = 0;

            // Inscription bonus
            int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;

            damageBonus += inscribeSkill / 200;

            if (inscribeSkill >= 1000)
                damageBonus += 5;

            if (attacker.Player)
            {
                // Int bonus
                damageBonus += (attacker.Int / 10);

                // SDI bonus
                damageBonus += AosAttributes.GetValue(attacker, AosAttribute.SpellDamage);

                TransformContext context = TransformationSpellHelper.GetContext(attacker);

                if (context != null && context.Spell is ReaperFormSpell)
                    damageBonus += ((ReaperFormSpell)context.Spell).SpellDamageBonus;
            }

            damage = AOS.Scale(damage, 100 + damageBonus);

            return damage / 100;
        }

        #region Do<AoSEffect>
        public virtual void DoManaDrain(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            defender.Mana -= Utility.Random(1, Math.Min(100, defender.Mana));

            defender.FixedParticles(0x374A, 10, 15, 5032, EffectLayer.Head);
            defender.PlaySound(0x1F8);
        }

        public virtual void DoParalyze(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            defender.Paralyze(attacker, Utility.Random(5, 5));

            defender.PlaySound(0x204);
            defender.FixedEffect(0x376A, 6, 1);
        }

        public virtual void DoPoison(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            Server.Spells.Third.PoisonSpell.ApplyEffect(attacker, defender);

            defender.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
            defender.PlaySound(0x474);
        }

        public virtual void DoCurse(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            Server.Spells.Fourth.CurseSpell.ApplyEffect(attacker, defender);
        }

        public virtual void DoMagicArrow(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            double damage = GetAosDamage(attacker, 10, 1, 4);

            attacker.MovingParticles(defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0);
            attacker.PlaySound(0x1E5);

            SpellHelper.Damage(TimeSpan.FromSeconds(1.0), defender, attacker, damage, 0, 100, 0, 0, 0);
        }

        public virtual void DoHarm(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            double damage = GetAosDamage(attacker, 17, 1, 5);

            if (!defender.InRange(attacker, 2))
                damage *= 0.25; // 1/4 damage at > 2 tile range
            else if (!defender.InRange(attacker, 1))
                damage *= 0.50; // 1/2 damage at 2 tile range

            defender.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
            defender.PlaySound(0x0FC);

            SpellHelper.Damage(TimeSpan.Zero, defender, attacker, damage, 0, 0, 100, 0, 0);
        }

        public virtual void DoFireball(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            double damage = GetAosDamage(attacker, 19, 1, 5);

            attacker.MovingParticles(defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
            attacker.PlaySound(0x15E);

            SpellHelper.Damage(TimeSpan.FromSeconds(1.0), defender, attacker, damage, 0, 100, 0, 0, 0);
        }

        public virtual void DoLightning(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            double damage = GetAosDamage(attacker, 23, 1, 4);

            defender.BoltEffect(0);

            SpellHelper.Damage(TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100);
        }

        public virtual void DoDispel(Mobile attacker, Mobile defender)
        {
            bool dispellable = false;

            if (defender is BaseCreature)
                dispellable = ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead;

            if (!dispellable)
                return;

            if (!attacker.CanBeHarmful(defender, false))
                return;

            attacker.DoHarmful(defender);

            Spells.MagerySpell sp = new Spells.Sixth.DispelSpell(attacker, null);

            if (sp.CheckResisted(defender))
            {
                defender.FixedEffect(0x3779, 10, 20);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(defender, defender.Map, 0x201);

                defender.Delete();
            }
        }

        public virtual void DoLowerAttack(Mobile from, Mobile defender)
        {
            if (HitLower.ApplyAttack(defender))
            {
                defender.PlaySound(0x28E);
                Effects.SendTargetEffect(defender, 0x37BE, 1, 4, 0xA, 3);
            }
        }

        public virtual void DoLowerDefense(Mobile from, Mobile defender)
        {
            if (HitLower.ApplyDefense(defender))
            {
                defender.PlaySound(0x28E);
                Effects.SendTargetEffect(defender, 0x37BE, 1, 4, 0x23, 3);
            }
        }

        public virtual void DoIPYAreaAttack(Mobile attacker, Mobile defender, Action<Mobile, Mobile> d)
        {
            Map map = attacker.Map;

            if (map == null)
                return;

            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = attacker.GetMobilesInRange(5);

            foreach (Mobile m in eable)
            {
                if (attacker != m && defender != m && SpellHelper.ValidIndirectTarget(attacker, m) && attacker.CanBeHarmful(m, false) && attacker.InLOS(m) && IsIPYWeaponAttributeTarget(m, CheckSlayers(attacker, m)))
                    list.Add(m);
            }

            eable.Free();

            if (list.Count == 0)
                return;

            for (int i = 0; i < list.Count; ++i)
            {
                Mobile m = list[i];
                d(attacker, m);
            }
        }

        public virtual bool IsIPYWeaponAttributeTarget(Mobile defender, CheckSlayerResult cs)
        {
            return (defender is BaseCreature) && //must be a creature
                !((BaseCreature)defender).Controlled && //cant be controlled
                cs == CheckSlayerResult.Slayer; //must be against a slayer
        }

        public virtual void DoAreaAttack(Mobile from, Mobile defender, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy)
        {
            Map map = from.Map;

            if (map == null)
                return;

            List<Mobile> list = new List<Mobile>();

            int range = Core.ML ? 5 : 10;

            IPooledEnumerable eable = from.GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if (from != m && defender != m && SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false) && (!Core.ML || from.InLOS(m)))
                    list.Add(m);
            }

            eable.Free();

            if (list.Count == 0)
                return;

            Effects.PlaySound(from.Location, map, sound);

            for (int i = 0; i < list.Count; ++i)
            {
                Mobile m = list[i];

                double scalar = Core.ML ? 1.0 : (11 - from.GetDistanceToSqrt(m)) / 10;
                double damage = GetBaseDamage(from);

                if (scalar <= 0)
                {
                    continue;
                }
                else if (scalar < 1.0)
                {
                    damage *= (11 - from.GetDistanceToSqrt(m)) / 10;
                }

                from.DoHarmful(m, true);
                m.FixedEffect(0x3779, 1, 15, hue, 0);
                AOS.Damage(m, from, (int)damage, phys, fire, cold, pois, nrgy);
            }
        }
        #endregion

        public virtual CheckSlayerResult CheckSlayers(Mobile attacker, Mobile defender)
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkWeapon.Slayer);
            SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkWeapon.Slayer2);
            SlayerEntry tempSlayer = SlayerGroup.GetEntryByName(atkWeapon.TempSlayer);

            if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender) || tempSlayer != null && tempSlayer.Slays(defender))
                return CheckSlayerResult.Slayer;

            if (!Core.SE)
            {
                ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

                if (defISlayer == null)
                    defISlayer = defender.Weapon as ISlayer;

                if (defISlayer != null)
                {
                    SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
                    SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);
                    SlayerEntry defTempSlayer = SlayerGroup.GetEntryByName(defISlayer.TempSlayer);

                    if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(attacker) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(attacker) || defTempSlayer != null && defTempSlayer.Group.OppositionSuperSlays(attacker))
                        return CheckSlayerResult.Opposition;
                }
            }

            return CheckSlayerResult.None;
        }

        public virtual void AddBlood(Mobile attacker, Mobile defender, int damage)
        {
            if (damage > 0)
            {
                int minBlood = 1;
                int maxBlood = 2;

                //Player Enhancement Customization: Vicious
                bool vicious = PlayerEnhancementPersistance.IsCustomizationEntryActive(attacker, CustomizationType.Vicious);

                if (vicious)
                {
                    minBlood++;
                    maxBlood++;
                }

                int bloodCount = Utility.RandomMinMax(minBlood, maxBlood);

                for (int a = 0; a < bloodCount; a++)
                {
                    new Blood().MoveToWorld(new Point3D(
                        defender.X + Utility.RandomMinMax(-1, 1),
                        defender.Y + Utility.RandomMinMax(-1, 1),
                        defender.Z), defender.Map);
                }
            }
        }

        public virtual void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            if (wielder is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)wielder;

                phys = bc.PhysicalDamage;
                fire = bc.FireDamage;
                cold = bc.ColdDamage;
                pois = bc.PoisonDamage;
                nrgy = bc.EnergyDamage;
                chaos = bc.ChaosDamage;
                direct = bc.DirectDamage;
            }

            else
            {
                fire = m_AosElementDamages.Fire;
                cold = m_AosElementDamages.Cold;
                pois = m_AosElementDamages.Poison;
                nrgy = m_AosElementDamages.Energy;
                chaos = m_AosElementDamages.Chaos;
                direct = m_AosElementDamages.Direct;

                phys = 100 - fire - cold - pois - nrgy - chaos - direct;

                CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

                if (resInfo != null)
                {
                    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

                    if (attrInfo != null)
                    {
                        int left = phys;

                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponColdDamage, ref cold, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponEnergyDamage, ref nrgy, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponFireDamage, ref fire, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponPoisonDamage, ref pois, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponChaosDamage, ref chaos, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponDirectDamage, ref direct, left);

                        phys = left;
                    }
                }
            }
        }

        private int ApplyCraftAttributeElementDamage(int attrDamage, ref int element, int totalRemaining)
        {
            if (totalRemaining <= 0)
                return 0;

            if (attrDamage <= 0)
                return totalRemaining;

            int appliedDamage = attrDamage;

            if ((appliedDamage + element) > 100)
                appliedDamage = 100 - element;

            if (appliedDamage > totalRemaining)
                appliedDamage = totalRemaining;

            element += appliedDamage;

            return totalRemaining - appliedDamage;
        }

        public virtual void GetBaseDamageRange(Mobile attacker, out int min, out int max)
        {
            if (attacker is BaseCreature)
            {
                BaseCreature c = (BaseCreature)attacker;

                if (c.DamageMin >= 0)
                {
                    min = c.DamageMin;
                    max = c.DamageMax;

                    return;
                }

                if (this is Fists && !attacker.Body.IsHuman)
                {
                    min = attacker.Str / 25;
                    max = attacker.Str / 25;

                    return;
                }
            }

            PlayerMobile playerAttacker = attacker as PlayerMobile;

            if (UOACZSystem.IsUOACZValidMobile(playerAttacker))
            {
                if (playerAttacker.IsUOACZHuman && this is Fists)
                {
                    min = 4;
                    max = 8;                                 

                    return;
                }

                if (playerAttacker.IsUOACZUndead)
                {
                    min = playerAttacker.m_UOACZAccountEntry.UndeadProfile.DamageMin;
                    max = playerAttacker.m_UOACZAccountEntry.UndeadProfile.DamageMax;  

                    return;
                }
            }

            min = MinDamage;
            max = MaxDamage;
        }

        public virtual double GetBaseDamage(Mobile attacker)
        {
            int min, max;

            GetBaseDamageRange(attacker, out min, out max);

            return Utility.RandomMinMax(min, max);
        }

        public virtual double GetBonus(double value, double scalar, double threshold, double offset)
        {
            double bonus = value * scalar;

            if (value >= threshold)
                bonus += offset;

            return bonus / 100;
        }

        public virtual double GetHitChanceBonus(Mobile from, Mobile target)
        {
            double bonus = 0;

            if (target is PlayerMobile)
            {
                switch (m_AccuracyLevel)
                {
                    case WeaponAccuracyLevel.Accurate: bonus += 0.01; break;
                    case WeaponAccuracyLevel.Surpassingly: bonus += 0.02; break;
                    case WeaponAccuracyLevel.Eminently: bonus += 0.03; break;
                    case WeaponAccuracyLevel.Exceedingly: bonus += 0.04; break;
                    case WeaponAccuracyLevel.Supremely: bonus += 0.05; break;
                }
            }

            else
            {
                switch (m_AccuracyLevel)
                {
                    case WeaponAccuracyLevel.Accurate: bonus += 0.03; break;
                    case WeaponAccuracyLevel.Surpassingly: bonus += 0.06; break;
                    case WeaponAccuracyLevel.Eminently: bonus += 0.09; break;
                    case WeaponAccuracyLevel.Exceedingly: bonus += 0.12; break;
                    case WeaponAccuracyLevel.Supremely: bonus += 0.15; break;
                }
            }

            return bonus;
        }

        public virtual int GetDamageBonus()
        {
            int bonus = VirtualDamageBonus;

            switch (m_Quality)
            {
                case WeaponQuality.Low: bonus -= 20; break;
                case WeaponQuality.Exceptional: bonus += 20; break;
            }

            switch (m_DamageLevel)
            {
                case WeaponDamageLevel.Ruin: bonus += 15; break;
                case WeaponDamageLevel.Might: bonus += 20; break;
                case WeaponDamageLevel.Force: bonus += 25; break;
                case WeaponDamageLevel.Power: bonus += 30; break;
                case WeaponDamageLevel.Vanq: bonus += 35; break;
            }

            return bonus;
        }

        public virtual void GetStatusDamage(Mobile from, out int min, out int max)
        {
            int baseMin, baseMax;

            GetBaseDamageRange(from, out baseMin, out baseMax);

            if (Core.AOS)
            {
                min = Math.Max((int)ScaleDamageAOS(from, baseMin, false), 1);
                max = Math.Max((int)ScaleDamageAOS(from, baseMax, false), 1);
            }
            else
            {
                min = Math.Max((int)ScaleDamageOld(from, baseMin, false), 1);
                max = Math.Max((int)ScaleDamageOld(from, baseMax, false), 1);
            }
        }

        public virtual double ScaleDamageAOS(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, 125.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap); // Passively check Anatomy for gain
                attacker.CheckSkill(SkillName.ArmsLore, 0.0, attacker.Skills[SkillName.ArmsLore].Cap); // Passively check Arms Lore for gain

                if (Type == WeaponType.Axe)
                    attacker.CheckSkill(SkillName.Lumberjacking, 0.0, 100.0); // Passively check Lumberjacking for gain
            }

            #region Physical bonuses
            /*
			 * These are the bonuses given by the physical characteristics of the mobile.
			 * No caps apply.
			 */
            double strengthBonus = GetBonus(attacker.Str, 0.300, 100.0, 5.00);
            double anatomyBonus = GetBonus(attacker.Skills[SkillName.Anatomy].Value, 0.500, 100.0, 5.00);
            double tacticsBonus = GetBonus(attacker.Skills[SkillName.Tactics].Value, 0.625, 100.0, 6.25);
            double lumberBonus = GetBonus(attacker.Skills[SkillName.Lumberjacking].Value, 0.200, 100.0, 10.00);

            if (Type != WeaponType.Axe)
                lumberBonus = 0.0;
            #endregion

            #region Modifiers
            /*
			 * The following are damage modifiers whose effect shows on the status bar.
			 * Capped at 100% total.
			 */
            int damageBonus = AosAttributes.GetValue(attacker, AosAttribute.WeaponDamage);

            // Horrific Beast transformation gives a +25% bonus to damage.
            if (TransformationSpellHelper.UnderTransformation(attacker, typeof(HorrificBeastSpell)))
                damageBonus += 25;

            // Divine Fury gives a +10% bonus to damage.
            if (Spells.Chivalry.DivineFurySpell.UnderEffect(attacker))
                damageBonus += 10;

            int defenseMasteryMalus = 0;

            // Defense Mastery gives a -50%/-80% malus to damage.
            if (Server.Items.DefenseMastery.GetMalus(attacker, ref defenseMasteryMalus))
                damageBonus -= defenseMasteryMalus;

            int discordanceEffect = 0;

            // Discordance gives a -2%/-48% malus to damage.
            if (SkillHandlers.Discordance.GetEffect(attacker, ref discordanceEffect))
                damageBonus -= discordanceEffect * 2;

            if (damageBonus > 100)
                damageBonus = 100;
            #endregion

            double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus + ((double)(GetDamageBonus() + damageBonus) / 100.0);

            return damage + (int)(damage * totalBonus);
        }

        public virtual int VirtualDamageBonus { get { return 0; } }

        public virtual int ComputeDamageAOS(Mobile attacker, Mobile defender)
        {
            return (int)ScaleDamageAOS(attacker, GetBaseDamage(attacker), true);
        }

        public virtual double ScaleDamageOld(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, 125.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap); // Passively check Anatomy for gain
                attacker.CheckSkill(SkillName.ArmsLore, 0.0, attacker.Skills[SkillName.ArmsLore].Cap); // Passively check Arms Lore for gain
            }

            double tacticsBase = attacker.Skills[SkillName.Tactics].Value;
            double tacticsBonus = 0;

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            if (pm_Attacker != null)
            {
                //Remove Dungeon Weapon Tactics Bonus Impact in PvP
                if (pm_Attacker.LastPlayerCombatTime + pm_Attacker.PlayerCombatExpirationDelay > DateTime.UtcNow)                
                    tacticsBase -= DungeonWeapon.BaseTactics + (DungeonTier * DungeonWeapon.TacticsPerTier);                           
            }

            if (tacticsBase > 100)
            {
                tacticsBonus = tacticsBase - 100;
                tacticsBase = 100;
            };

            double tacticsScalar = .5 + (.5 * (tacticsBase / 100)) + (.01 * tacticsBonus);
            double anatomyScalar = 1 + (.2 * (attacker.Skills[SkillName.Anatomy].Value / 100));
            double virtualDamageScalar = 1 + ((double)VirtualDamageBonus / 100);
            
            damage *= tacticsScalar;
            damage *= anatomyScalar;
            damage *= virtualDamageScalar;

            return ScaleDamageByDurability((int)damage);
        }

        public virtual int ScaleDamageByDurability(int damage)
        {
            int scale = 100;

            if (m_MaxHits > 0 && m_Hits < m_MaxHits)
                scale = 80 + ((20 * m_Hits) / m_MaxHits);

            return AOS.Scale(damage, scale);
        }

        public virtual int ComputeDamage(Mobile attacker, Mobile defender)
        {
            return (int)ScaleDamageOld(attacker, GetBaseDamage(attacker), !(defender is PlayerMobile) || ((PlayerMobile)defender).m_DateTimeDied + TimeSpan.FromSeconds(60) < DateTime.UtcNow);
        }

        public virtual void PlayHurtAnimation(Mobile from)
        {
            int action;
            int frames;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.IsUOACZUndead)
                {
                    if (player.m_UOACZAccountEntry.UndeadProfile.AttackAnimation != -1)
                    {
                        player.Animate(player.m_UOACZAccountEntry.UndeadProfile.HurtAnimation, player.m_UOACZAccountEntry.UndeadProfile.HurtAnimationFrames, 1, true, false, 0);
                        return;
                    }
                }
            }
                       
            if (from is BaseCreature)
            {
                BaseCreature bc_From = from as BaseCreature;
                
                double hurtSoundIgnoreChance = 0;

                if (bc_From.IsMiniBoss())                
                    hurtSoundIgnoreChance = .75;

                if (bc_From.IsBoss())
                    hurtSoundIgnoreChance = .80;

                if (bc_From.IsLoHBoss())
                    hurtSoundIgnoreChance = .85;

                if (bc_From.IsEventBoss())
                    hurtSoundIgnoreChance = .90;

                if (Utility.RandomDouble() <= hurtSoundIgnoreChance)
                    return;

                //Manual Override
                if (bc_From.HurtAnimation > -1 && bc_From.HurtFrames > 0)
                {
                    bc_From.Animate(bc_From.HurtAnimation, bc_From.HurtFrames, 1, bc_From.HurtAnimationPlayForwards, false, 0);
                    return;
                }

                //High Seas Creature Animation Override
                if (bc_From.IsHighSeasBodyType)
                {
                    if (bc_From.HasAlternateHighSeasHurtAnimation)
                        bc_From.Animate(Utility.RandomList(15), 5, 1, bc_From.HurtAnimationPlayForwards, false, 0);
                    else
                        bc_From.Animate(Utility.RandomList(15, 23), 5, 1, bc_From.HurtAnimationPlayForwards, false, 0);

                    return;
                }
            }

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                    {
                        action = 7;
                        frames = 5;
                        break;
                    }
                case BodyType.Monster:
                    {
                        action = 10;
                        frames = 4;
                        break;
                    }
                case BodyType.Human:
                    {
                        action = 20;
                        frames = 5;
                        break;
                    }
                default: return;
            }

            if (from.Mounted)
                return;

            from.Animate(action, frames, 1, true, false, 0);
        }

        public virtual void PlaySwingAnimation(Mobile from)
        {
            int action;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.IsUOACZUndead)
                {
                    if (player.m_UOACZAccountEntry.UndeadProfile.AttackAnimation != -1)
                    {
                        player.Animate(player.m_UOACZAccountEntry.UndeadProfile.AttackAnimation, player.m_UOACZAccountEntry.UndeadProfile.AttackAnimationFrames, 1, true, false, 0);
                        return;
                    }
                }
            }
           
            if (from is BaseCreature)
            {
                BaseCreature bc_From = from as BaseCreature;

                //Manual Override
                if (bc_From.AttackAnimation > -1 && bc_From.AttackFrames > 0)
                {
                    bc_From.Animate(bc_From.AttackAnimation, bc_From.AttackFrames, 1, bc_From.AttackAnimationPlayForwards, false, 0);
                    return;
                }

                //High Seas Creature Animation Override
                if (bc_From.IsHighSeasBodyType)
                {
                    if (bc_From.IsHighSeasBodyType)
                    {
                        if (bc_From.HasAlternateHighSeasAttackAnimation)
                            from.Animate(Utility.RandomList(5, 6), 8, 1, bc_From.AttackAnimationPlayForwards, false, 0);
                        else
                            from.Animate(Utility.RandomList(5, 6), 6, 1, bc_From.AttackAnimationPlayForwards, false, 0);

                        return;
                    }
                }
            }

            switch (from.Body.Type)
            {
                case BodyType.Sea:
                case BodyType.Animal:
                    {
                        action = Utility.Random(5, 2);
                        break;
                    }
                case BodyType.Monster:
                    {
                        switch (Animation)
                        {
                            default:
                            case WeaponAnimation.Wrestle:
                            case WeaponAnimation.Bash1H:
                            case WeaponAnimation.Pierce1H:
                            case WeaponAnimation.Slash1H:
                            case WeaponAnimation.Bash2H:
                            case WeaponAnimation.Pierce2H:
                            case WeaponAnimation.Slash2H: action = Utility.Random(4, 3); break;
                            case WeaponAnimation.ShootBow: return; // 7
                            case WeaponAnimation.ShootXBow: return; // 8
                        }

                        break;
                    }
                case BodyType.Human:
                    {
                        if (!from.Mounted)
                            action = (int)this.GetAnimation();

                        else
                        {
                            switch (Animation)
                            {
                                default:
                                case WeaponAnimation.Wrestle:
                                case WeaponAnimation.Bash1H:
                                case WeaponAnimation.Pierce1H:
                                case WeaponAnimation.Slash1H: action = 26; break;
                                case WeaponAnimation.Bash2H:
                                case WeaponAnimation.Pierce2H:
                                case WeaponAnimation.Slash2H: action = 29; break;
                                case WeaponAnimation.ShootBow: action = 27; break;
                                case WeaponAnimation.ShootXBow: action = 28; break;
                            }
                        }

                        break;
                    }
                default: return;
            }

            from.Animate(action, 7, 1, true, false, 0);
        }

        #region Serialization/Deserialization
        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = 11;
            int version_and_uoacFlags = (version);

            writer.Write((int)version_and_uoacFlags); // version and flags
           
            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.DamageLevel, m_DamageLevel != WeaponDamageLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.AccuracyLevel, m_AccuracyLevel != WeaponAccuracyLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.DurabilityLevel, m_DurabilityLevel != WeaponDurabilityLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.Quality, m_Quality != WeaponQuality.Regular);
            SetSaveFlag(ref flags, SaveFlag.Hits, m_Hits != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxHits, m_MaxHits != 0);
            SetSaveFlag(ref flags, SaveFlag.Slayer, m_Slayer != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.Poison, m_Poison != null);
            SetSaveFlag(ref flags, SaveFlag.PoisonCharges, m_PoisonCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Identified, m_Identified != false);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            SetSaveFlag(ref flags, SaveFlag.DexReq, m_DexReq != -1);
            SetSaveFlag(ref flags, SaveFlag.IntReq, m_IntReq != -1);
            SetSaveFlag(ref flags, SaveFlag.MinDamage, m_MinDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxDamage, m_MaxDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.HitSound, m_HitSound != -1);
            SetSaveFlag(ref flags, SaveFlag.MissSound, m_MissSound != -1);
            SetSaveFlag(ref flags, SaveFlag.Speed, m_Speed != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxRange, m_MaxRange != -1);
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != (SkillName)(-1));
            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != (WeaponType)(-1));
            SetSaveFlag(ref flags, SaveFlag.Animation, m_Animation != (WeaponAnimation)(-1));
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != CraftResource.Iron);
            SetSaveFlag(ref flags, SaveFlag.xAttributes, !m_AosAttributes.IsEmpty);            
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, m_PlayerConstructed);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Slayer2, m_Slayer2 != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.ElementalDamages, !m_AosElementDamages.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.EngravedText, !String.IsNullOrEmpty(m_EngravedText));

            writer.Write((int)flags);

            if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                writer.Write((int)m_DamageLevel);

            if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                writer.Write((int)m_AccuracyLevel);

            if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                writer.Write((int)m_DurabilityLevel);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.Write((int)m_Quality);

            if (GetSaveFlag(flags, SaveFlag.Hits))
                writer.Write((int)m_Hits);

            if (GetSaveFlag(flags, SaveFlag.MaxHits))
                writer.Write((int)m_MaxHits);

            if (GetSaveFlag(flags, SaveFlag.Slayer))
                writer.Write((int)m_Slayer);

            if (GetSaveFlag(flags, SaveFlag.Poison))
                Poison.Serialize(m_Poison, writer);

            if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                writer.Write((int)m_PoisonCharges);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write((Mobile)m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Identified))
                writer.Write((bool)m_Identified);

            if (GetSaveFlag(flags, SaveFlag.StrReq))
                writer.Write((int)m_StrReq);

            if (GetSaveFlag(flags, SaveFlag.DexReq))
                writer.Write((int)m_DexReq);

            if (GetSaveFlag(flags, SaveFlag.IntReq))
                writer.Write((int)m_IntReq);

            if (GetSaveFlag(flags, SaveFlag.MinDamage))
                writer.Write((int)m_MinDamage);

            if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                writer.Write((int)m_MaxDamage);

            if (GetSaveFlag(flags, SaveFlag.HitSound))
                writer.Write((int)m_HitSound);

            if (GetSaveFlag(flags, SaveFlag.MissSound))
                writer.Write((int)m_MissSound);

            if (GetSaveFlag(flags, SaveFlag.Speed))
                writer.Write((int)m_Speed);

            if (GetSaveFlag(flags, SaveFlag.MaxRange))
                writer.Write((int)m_MaxRange);

            if (GetSaveFlag(flags, SaveFlag.Skill))
                writer.Write((int)m_Skill);

            if (GetSaveFlag(flags, SaveFlag.Type))
                writer.Write((int)m_Type);

            if (GetSaveFlag(flags, SaveFlag.Animation))
                writer.Write((int)m_Animation);

            if (GetSaveFlag(flags, SaveFlag.Resource))
                writer.Write((int)m_Resource);

            if (GetSaveFlag(flags, SaveFlag.xAttributes))
                m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                m_AosSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Slayer2))
                writer.Write((int)m_Slayer2);

            if (GetSaveFlag(flags, SaveFlag.ElementalDamages))
                m_AosElementDamages.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.EngravedText))
                writer.Write((string)m_EngravedText);

            //Version 11
            writer.Write((int)m_Dungeon);
            writer.Write(m_DungeonTier);
            writer.Write(m_DungeonExperience);
            writer.Write(m_BlessedCharges);
            writer.Write(m_MaxBlessedCharges);          
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            DamageLevel = 0x00000001,
            AccuracyLevel = 0x00000002,
            DurabilityLevel = 0x00000004,
            Quality = 0x00000008,
            Hits = 0x00000010,
            MaxHits = 0x00000020,
            Slayer = 0x00000040,
            Poison = 0x00000080,
            PoisonCharges = 0x00000100,
            Crafter = 0x00000200,
            Identified = 0x00000400,
            StrReq = 0x00000800,
            DexReq = 0x00001000,
            IntReq = 0x00002000,
            MinDamage = 0x00004000,
            MaxDamage = 0x00008000,
            HitSound = 0x00010000,
            MissSound = 0x00020000,
            Speed = 0x00040000,
            MaxRange = 0x00080000,
            Skill = 0x00100000,
            Type = 0x00200000,
            Animation = 0x00400000,
            Resource = 0x00800000,
            xAttributes = 0x01000000,
            xWeaponAttributes = 0x02000000,
            PlayerConstructed = 0x04000000,
            SkillBonuses = 0x08000000,
            Slayer2 = 0x10000000,
            ElementalDamages = 0x20000000,
            EngravedText = 0x40000000,
            PoisonPercent = unchecked((int)0x80000000) // IPY2 LEGACY : No longer used (because it was broken)
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version_and_uoacflags = reader.ReadInt();            
            int version = version_and_uoacflags & 0xFFFF;
            
            switch (version)
            {
                case 11:
                case 10:
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                        {
                            m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();

                            if (m_DamageLevel > WeaponDamageLevel.Vanq)
                                m_DamageLevel = WeaponDamageLevel.Ruin;
                        }

                        if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                        {
                            m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();

                            if (m_AccuracyLevel > WeaponAccuracyLevel.Supremely)
                                m_AccuracyLevel = WeaponAccuracyLevel.Accurate;
                        }

                        if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                        {
                            m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();

                            if (m_DurabilityLevel > WeaponDurabilityLevel.Indestructible)
                                m_DurabilityLevel = WeaponDurabilityLevel.Durable;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            m_Quality = (WeaponQuality)reader.ReadInt();
                        else
                            m_Quality = WeaponQuality.Regular;

                        if (GetSaveFlag(flags, SaveFlag.Hits))
                            m_Hits = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxHits))
                            m_MaxHits = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Slayer))
                            m_Slayer = (SlayerName)reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Poison))
                            m_Poison = Poison.Deserialize(reader);

                        if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                            m_PoisonCharges = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Identified))
                            m_Identified = (version <= 6 || reader.ReadBool());

                        if (GetSaveFlag(flags, SaveFlag.StrReq))
                            m_StrReq = reader.ReadInt();
                        else
                            m_StrReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.DexReq))
                            m_DexReq = reader.ReadInt();
                        else
                            m_DexReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.IntReq))
                            m_IntReq = reader.ReadInt();
                        else
                            m_IntReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.MinDamage))
                            m_MinDamage = reader.ReadInt();
                        else
                            m_MinDamage = -1;

                        if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                            m_MaxDamage = reader.ReadInt();
                        else
                            m_MaxDamage = -1;

                        if (GetSaveFlag(flags, SaveFlag.HitSound))
                            m_HitSound = reader.ReadInt();
                        else
                            m_HitSound = -1;

                        if (GetSaveFlag(flags, SaveFlag.MissSound))
                            m_MissSound = reader.ReadInt();
                        else
                            m_MissSound = -1;

                        if (GetSaveFlag(flags, SaveFlag.Speed))
                            m_Speed = reader.ReadInt();
                        else
                            m_Speed = -1;

                        if (GetSaveFlag(flags, SaveFlag.MaxRange))
                            m_MaxRange = reader.ReadInt();
                        else
                            m_MaxRange = -1;

                        if (GetSaveFlag(flags, SaveFlag.Skill))
                            m_Skill = (SkillName)reader.ReadInt();
                        else
                            m_Skill = (SkillName)(-1);

                        if (GetSaveFlag(flags, SaveFlag.Type))
                            m_Type = (WeaponType)reader.ReadInt();
                        else
                            m_Type = (WeaponType)(-1);

                        if (GetSaveFlag(flags, SaveFlag.Animation))
                            m_Animation = (WeaponAnimation)reader.ReadInt();
                        else
                            m_Animation = (WeaponAnimation)(-1);

                        if (GetSaveFlag(flags, SaveFlag.Resource))
                            m_Resource = (CraftResource)reader.ReadInt();
                        else
                            m_Resource = CraftResource.Iron;

                        if (GetSaveFlag(flags, SaveFlag.xAttributes))
                            m_AosAttributes = new AosAttributes(this, reader);
                        else
                            m_AosAttributes = new AosAttributes(this);

                        if (version >= 10)
                        {
                            m_AosWeaponAttributes = new AosWeaponAttributes(this);
                        }

                        else
                        {
                            if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
                                m_AosWeaponAttributes = new AosWeaponAttributes(this, reader);
                            else
                                m_AosWeaponAttributes = new AosWeaponAttributes(this);
                        }

                        if (UseSkillMod && (m_DamageLevel != WeaponDamageLevel.Regular)&& Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }

                        else if (UseSkillMod && m_Quality == WeaponQuality.Exceptional && Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }

                        else if (UseSkillMod && DungeonTier > 0 && Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }

                        if (version < 7 && m_AosWeaponAttributes.MageWeapon != 0)
                            m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeapon;

                        if (Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 && Parent is Mobile)
                        {
                            m_MageMod = new DefaultSkillMod(SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon);
                            ((Mobile)Parent).AddSkillMod(m_MageMod);
                        }

                        if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                            m_PlayerConstructed = true;

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                            m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            m_AosSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(flags, SaveFlag.Slayer2))
                            m_Slayer2 = (SlayerName)reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.ElementalDamages))
                            m_AosElementDamages = new AosElementAttributes(this, reader);
                        else
                            m_AosElementDamages = new AosElementAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.EngravedText))
                            m_EngravedText = reader.ReadString();

                        if (GetSaveFlag(flags, SaveFlag.PoisonPercent))
                            reader.ReadInt(); // LEGACY (No longer used)

                        break;
                    }
                case 4:
                    {
                        m_Slayer = (SlayerName)reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        m_StrReq = reader.ReadInt();
                        m_DexReq = reader.ReadInt();
                        m_IntReq = reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_Identified = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        m_MaxRange = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version == 0)
                            m_MaxRange = 1; // default

                        if (version < 5)
                        {
                            m_Resource = CraftResource.Iron;
                            m_AosAttributes = new AosAttributes(this);
                            m_AosWeaponAttributes = new AosWeaponAttributes(this);
                            m_AosElementDamages = new AosElementAttributes(this);
                            m_AosSkillBonuses = new AosSkillBonuses(this);
                        }

                        m_MinDamage = reader.ReadInt();
                        m_MaxDamage = reader.ReadInt();

                        m_Speed = reader.ReadInt();

                        m_HitSound = reader.ReadInt();
                        m_MissSound = reader.ReadInt();

                        m_Skill = (SkillName)reader.ReadInt();
                        m_Type = (WeaponType)reader.ReadInt();
                        m_Animation = (WeaponAnimation)reader.ReadInt();
                        m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();
                        m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();
                        m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();
                        m_Quality = (WeaponQuality)reader.ReadInt();

                        m_Crafter = reader.ReadMobile();

                        m_Poison = Poison.Deserialize(reader);
                        m_PoisonCharges = reader.ReadInt();                       

                        break;
                    }
            }

            //Version 11
            if (version >= 11)
            {         
                Dungeon = (BaseDungeonArmor.DungeonEnum)reader.ReadInt();
                DungeonTier = reader.ReadInt();
                DungeonExperience = reader.ReadInt();
                BlessedCharges = reader.ReadInt();
                MaxBlessedCharges = reader.ReadInt();                 
            }

            //-----

            if (m_StrReq == OldStrengthReq)
                m_StrReq = -1;

            if (m_DexReq == OldDexterityReq)
                m_DexReq = -1;

            if (m_IntReq == OldIntelligenceReq)
                m_IntReq = -1;

            if (m_MinDamage == OldMinDamage)
                m_MinDamage = -1;

            if (m_MaxDamage == OldMaxDamage)
                m_MaxDamage = -1;

            if (m_HitSound == OldHitSound)
                m_HitSound = -1;

            if (m_MissSound == OldMissSound)
                m_MissSound = -1;

            if (m_Speed == OldSpeed)
                m_Speed = -1;

            if (m_MaxRange == OldMaxRange)
                m_MaxRange = -1;

            if (m_Skill == OldSkill)
                m_Skill = (SkillName)(-1);

            if (m_Type == OldType)
                m_Type = (WeaponType)(-1);

            if (m_Animation == OldAnimation)
                m_Animation = (WeaponAnimation)(-1);                     

            if (Core.AOS && Parent is Mobile)
                m_AosSkillBonuses.AddTo((Mobile)Parent);

            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if (this.Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = (Mobile)this.Parent;

                string modName = this.Serial.ToString();

                if (strBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                if (dexBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                if (intBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
            }

            if (Parent is Mobile)
                ((Mobile)Parent).CheckStatTimers();

            if (m_Hits <= 0 && m_MaxHits <= 0)            
                m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);            

            if (version < 6)
                m_PlayerConstructed = true; // we don't know, so, assume it's crafted

            if (UseSkillMod && m_DamageLevel != WeaponDamageLevel.Regular && Parent is Mobile)
                OnEquip(Parent as Mobile);

            if (UseSkillMod && m_Quality == WeaponQuality.Exceptional && Parent is Mobile)
                OnEquip(Parent as Mobile);   
        }
        #endregion

        public BaseWeapon(int itemID)
            : base(itemID)
        {
            Layer = (Layer)ItemData.Quality;

            m_Quality = WeaponQuality.Regular;
            m_StrReq = -1;
            m_DexReq = -1;
            m_IntReq = -1;
            m_MinDamage = -1;
            m_MaxDamage = -1;
            m_HitSound = -1;
            m_MissSound = -1;
            m_Speed = -1;
            m_MaxRange = -1;
            m_Skill = (SkillName)(-1);
            m_Type = (WeaponType)(-1);
            m_Animation = (WeaponAnimation)(-1);

            m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_Resource = CraftResource.Iron;

            m_AosAttributes = new AosAttributes(this);
            m_AosWeaponAttributes = new AosWeaponAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_AosElementDamages = new AosElementAttributes(this);
        }

        public BaseWeapon(Serial serial)
            : base(serial)
        {
        }

        private string GetNameString()
        {
            string name = this.Name;

            if (name == null)
                name = String.Format("#{0}", LabelNumber);

            return name;
        }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set { base.Hue = value; InvalidateProperties(); }
        }

        public int GetElementalDamageHue()
        {
            int phys, fire, cold, pois, nrgy, chaos, direct;
            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);
            //Order is Cold, Energy, Fire, Poison, Physical left

            int currentMax = 50;
            int hue = 0;

            if (pois >= currentMax)
            {
                hue = 1267 + (pois - 50) / 10;
                currentMax = pois;
            }

            if (fire >= currentMax)
            {
                hue = 1255 + (fire - 50) / 10;
                currentMax = fire;
            }

            if (nrgy >= currentMax)
            {
                hue = 1273 + (nrgy - 50) / 10;
                currentMax = nrgy;
            }

            if (cold >= currentMax)
            {
                hue = 1261 + (cold - 50) / 10;
                currentMax = cold;
            }

            return hue;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            int oreType;

            switch (m_Resource)
            {
                case CraftResource.DullCopper: oreType = 1053108; break; // dull copper
                case CraftResource.ShadowIron: oreType = 1053107; break; // shadow iron
                case CraftResource.Copper: oreType = 1053106; break; // copper
                case CraftResource.Bronze: oreType = 1053105; break; // bronze
                case CraftResource.Gold: oreType = 1053104; break; // golden
                case CraftResource.Agapite: oreType = 1053103; break; // agapite
                case CraftResource.Verite: oreType = 1053102; break; // verite
                case CraftResource.Valorite: oreType = 1053101; break; // valorite
                case CraftResource.Lunite: oreType = 1053101; break; // lunite
                case CraftResource.SpinedLeather: oreType = 1061118; break; // spined
                case CraftResource.HornedLeather: oreType = 1061117; break; // horned
                case CraftResource.BarbedLeather: oreType = 1061116; break; // barbed
                case CraftResource.RedScales: oreType = 1060814; break; // red
                case CraftResource.YellowScales: oreType = 1060818; break; // yellow
                case CraftResource.BlackScales: oreType = 1060820; break; // black
                case CraftResource.GreenScales: oreType = 1060819; break; // green
                case CraftResource.WhiteScales: oreType = 1060821; break; // white
                case CraftResource.BlueScales: oreType = 1060815; break; // blue
                default: oreType = 0; break;
            }

            if (oreType != 0)
                list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~
            else if (Name == null)
                list.Add(LabelNumber);
            else
                list.Add(Name);

            /*
             * Want to move this to the engraving tool, let the non-harmful 
             * formatting show, and remove CLILOCs embedded: more like OSI
             * did with the books that had markup, etc.
             * 
             * This will have a negative effect on a few event things imgame 
             * as is.
             * 
             * If we cant find a more OSI-ish way to clean it up, we can 
             * easily put this back, and use it in the deserialize
             * method and engraving tool, to make it perm cleaned up.
             */

            if (!String.IsNullOrEmpty(m_EngravedText))
                list.Add(1062613, m_EngravedText);

            /* list.Add( 1062613, Utility.FixHtml( m_EngravedText ) ); */
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return (m_AosAttributes.SpellChanneling != 0);
        }

        public virtual int ArtifactRarity
        {
            get { return 0; }
        }

        public virtual int GetLuckBonus()
        {
            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

            if (resInfo == null)
                return 0;

            CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

            if (attrInfo == null)
                return 0;

            return attrInfo.WeaponLuck;
        }

        public bool AttemptWeaponPoison(Mobile attacker, Mobile defender)
        {
            if (Poison != null && PoisonCharges > 0)
            {
                bool canPoison = true;

                // only prevent charge loss on non players
                if (defender.Poison != null && defender is BaseCreature)
                {
                    //Target Already Poisoned with Same Level of Poison or Greater
                    if (defender.Poison.Level >= Poison.Level)
                        canPoison = false;
                }

                if (canPoison)
                {
                    double poisoningSkill = attacker.Skills[SkillName.Poisoning].Value;

                    double baseChance = 0.25;
                    double bonusChance = (attacker.Skills.Poisoning.Value / 100) * 0.15;

                    bool ignoreCharge = false;

                    double loseChargeChance = 1 - ((poisoningSkill / 100) * .5);

                    if (loseChargeChance < .25)
                        loseChargeChance = .25;

                    double chance = baseChance + bonusChance;

                    int effectHue = 0;

                    if (Utility.RandomDouble() <= chance)
                    {
                        int poisonLevel = Poison.Level;

                        if (defender is BaseCreature)
                        {
                            BaseDungeonArmor.PlayerDungeonArmorProfile attackerDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(attacker, null);

                            if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat)
                            {
                                if (Utility.RandomDouble() <= attackerDungeonArmor.DungeonArmorDetail.NoPoisonChargeSpentChance)
                                {
                                    ignoreCharge = true;
                                    effectHue = attackerDungeonArmor.DungeonArmorDetail.EffectHue;
                                }
                            }

                            if (poisonLevel < 4)
                            {
                                double poisonUpgradeChance = (attacker.Skills.Poisoning.Value / 100) * .25;
                                double upgradeResult = Utility.RandomDouble();

                                if (upgradeResult <= poisonUpgradeChance)
                                {
                                    poisonLevel++;
                                    attacker.SendMessage("Through your knowledge of poisons you improve the quality of your poison dose.");
                                }
                            }
                        }

                        //Player Enhancement Customization: Venomous
                        bool venomous = PlayerEnhancementPersistance.IsCustomizationEntryActive(attacker, CustomizationType.Venomous);

                        if (venomous)                        
                            CustomizationAbilities.Venomous(defender); 
                        
                        Poison poison = Poison.GetPoison(poisonLevel);

                        defender.ApplyPoison(attacker, poison);

                        if (!ignoreCharge)
                        {
                            Effects.PlaySound(attacker.Location, attacker.Map, 0x64B);
                            Effects.SendLocationParticles(EffectItem.Create(attacker.Location, attacker.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);

                            --PoisonCharges;
                        }
                    }

                    else
                    {
                        if (Utility.RandomDouble() <= loseChargeChance && !ignoreCharge)                          
                            --PoisonCharges;                        
                    }

                    return true;
                }
            }

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.Name); // crafted by ~1_NAME~

            #region Factions
            if (m_FactionState != null)
                list.Add(1041350); // faction item
            #endregion

            if (m_AosSkillBonuses != null)
                m_AosSkillBonuses.GetProperties(list);

            if (m_Quality == WeaponQuality.Exceptional)
                list.Add(1060636); // exceptional

            if (RequiredRace == Race.Elf)
                list.Add(1075086); // Elves Only

            if (ArtifactRarity > 0)
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~

            if (this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining)
                list.Add(1060584, ((IUsesRemaining)this).UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_Poison != null && m_PoisonCharges > 0)
                list.Add(1062412 + m_Poison.Level, m_PoisonCharges.ToString());

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (m_TempSlayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_TempSlayer);
                if (entry != null)
                    list.Add(entry.Title);
            }


            base.AddResistanceProperties(list);

            int prop;

            //if ( Core.ML && this is BaseRanged && ( (BaseRanged) this ).Balanced )
            //    list.Add( 1072792 ); // Balanced

            if ((prop = m_AosWeaponAttributes.UseBestSkill) != 0)
                list.Add(1060400); // use best weapon skill

            //if ((prop = (GetDamageBonus() + m_AosAttributes.WeaponDamage)) != 0)
            //    list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            //if ((prop = (GetHitChanceBonus() + m_AosAttributes.AttackChance)) != 0)
            //    list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitColdArea) != 0)
                list.Add(1060416, prop.ToString()); // hit cold area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitDispel) != 0)
                list.Add(1060417, prop.ToString()); // hit dispel ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitEnergyArea) != 0)
                list.Add(1060418, prop.ToString()); // hit energy area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitFireArea) != 0)
                list.Add(1060419, prop.ToString()); // hit fire area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitFireball) != 0)
                list.Add(1060420, prop.ToString()); // hit fireball ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitHarm) != 0)
                list.Add(1060421, prop.ToString()); // hit harm ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechHits) != 0)
                list.Add(1060422, prop.ToString()); // hit life leech ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLightning) != 0)
                list.Add(1060423, prop.ToString()); // hit lightning ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLowerAttack) != 0)
                list.Add(1060424, prop.ToString()); // hit lower attack ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLowerDefend) != 0)
                list.Add(1060425, prop.ToString()); // hit lower defense ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitMagicArrow) != 0)
                list.Add(1060426, prop.ToString()); // hit magic arrow ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechMana) != 0)
                list.Add(1060427, prop.ToString()); // hit mana leech ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitPhysicalArea) != 0)
                list.Add(1060428, prop.ToString()); // hit physical area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitPoisonArea) != 0)
                list.Add(1060429, prop.ToString()); // hit poison area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechStam) != 0)
                list.Add(1060430, prop.ToString()); // hit stamina leech ~1_val~%

            if (ImmolatingWeaponSpell.IsImmolating(this))
                list.Add(1111917); // Immolated

            //if ( Core.ML && this is BaseRanged && ( prop = ( (BaseRanged) this ).Velocity ) != 0 )
            //    list.Add( 1072793, prop.ToString() ); // Velocity ~1_val~%

            if ((prop = m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = GetLowerStatReq()) != 0)
                list.Add(1060435, prop.ToString()); // lower requirements ~1_val~%

            if ((prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_AosWeaponAttributes.MageWeapon) != 0)
                list.Add(1060438, (30 - prop).ToString()); // mage weapon -~1_val~ skill

            if ((prop = m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_AosWeaponAttributes.SelfRepair) != 0)
                list.Add(1060450, prop.ToString()); // self repair ~1_val~

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if (Core.ML && (prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

            if (phys != 0)
                list.Add(1060403, phys.ToString()); // physical damage ~1_val~%

            if (fire != 0)
                list.Add(1060405, fire.ToString()); // fire damage ~1_val~%

            if (cold != 0)
                list.Add(1060404, cold.ToString()); // cold damage ~1_val~%

            if (pois != 0)
                list.Add(1060406, pois.ToString()); // poison damage ~1_val~%

            if (nrgy != 0)
                list.Add(1060407, nrgy.ToString()); // energy damage ~1_val

            if (Core.ML && chaos != 0)
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%

            if (Core.ML && direct != 0)
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%

            list.Add(1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString()); // weapon damage ~1_val~ - ~2_val~

            if (Core.ML)
                list.Add(1061167, String.Format("{0}s", Speed)); // weapon speed ~1_val~
            else
                list.Add(1061167, Speed.ToString());

            if (MaxRange > 1)
                list.Add(1061169, MaxRange.ToString()); // range ~1_val~

            int strReq = AOS.Scale(StrRequirement, 100 - GetLowerStatReq());

            if (strReq > 0)
                list.Add(1061170, strReq.ToString()); // strength requirement ~1_val~

            if (Layer == Layer.TwoHanded)
                list.Add(1061171); // two-handed weapon
            else
                list.Add(1061824); // one-handed weapon

            if (Core.SE || m_AosWeaponAttributes.UseBestSkill == 0)
            {
                switch (Skill)
                {
                    case SkillName.Swords: list.Add(1061172); break; // skill required: swordsmanship
                    case SkillName.Macing: list.Add(1061173); break; // skill required: mace fighting
                    case SkillName.Fencing: list.Add(1061174); break; // skill required: fencing
                    case SkillName.Archery: list.Add(1061175); break; // skill required: archery
                }
            }

            if (m_Hits >= 0 && m_MaxHits > 0)
                list.Add(1060639, "{0}\t{1}", m_Hits, m_MaxHits); // durability ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            //Dungeon Weapon
            if (DungeonTier > 0)
            {
                string name = "";

                if (Name != null)
                    name = Name;

                if (name != "")
                    LabelTo(from, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name));
                else
                    base.OnSingleClick(from);
                
                LabelTo(from, BaseDungeonArmor.GetDungeonName(Dungeon) + " Dungeon: Tier " + DungeonTier.ToString());
                LabelTo(from, "(" + DungeonExperience.ToString() + "/" + DungeonWeapon.MaxDungeonExperience.ToString() + " xp) " + " Charges: " + BlessedCharges.ToString());
                
                return;
            }

            List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            if (DisplayLootType)
            {
                if (LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            #region Factions
            if (m_FactionState != null)
                attrs.Add(new EquipInfoAttribute(1041350)); // faction item
            #endregion

            if (m_Quality == WeaponQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)m_Quality));

            if (m_Identified || from.AccessLevel >= AccessLevel.GameMaster)
            {
                if (m_Slayer != SlayerName.None)
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                    if (entry != null)
                        attrs.Add(new EquipInfoAttribute(entry.Title));
                }

                if (m_Slayer2 != SlayerName.None)
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                    if (entry != null)
                        attrs.Add(new EquipInfoAttribute(entry.Title));
                }

                if (m_TempSlayer != SlayerName.None)
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName(m_TempSlayer);
                    if (entry != null)
                        attrs.Add(new EquipInfoAttribute(entry.Title));
                }

                if (m_DurabilityLevel != WeaponDurabilityLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038000 + (int)m_DurabilityLevel));

                if (m_DamageLevel != WeaponDamageLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038015 + (int)m_DamageLevel));

                if (m_AccuracyLevel != WeaponAccuracyLevel.Regular)
                    attrs.Add(new EquipInfoAttribute(1038010 + (int)m_AccuracyLevel));
            }

            else if (m_Slayer != SlayerName.None || m_Slayer2 != SlayerName.None || m_TempSlayer != SlayerName.None || m_DurabilityLevel != WeaponDurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular)
                attrs.Add(new EquipInfoAttribute(1038000)); // Unidentified           

            int number;

            if (Name == null)
                number = LabelNumber;
            else
            {
                this.LabelTo(from, Name);
                number = 1041000;
            }

            if (DecorativeEquipment)
                LabelTo(from, "[Decorative]");
            
            EquipmentInfo eqInfo = new EquipmentInfo(number, m_Crafter, false, attrs.ToArray());

            from.Send(new DisplayEquipmentInfo(this, eqInfo));           

            if (m_Poison != null && m_PoisonCharges > 0)
            {
                if (m_PoisonCharges == 1)
                    LabelTo(from, m_Poison.Name + " Poison: " + m_PoisonCharges.ToString());

                else
                    LabelTo(from, m_Poison.Name + " Poison: " + m_PoisonCharges.ToString());
            }           
        }

        public void GetWrestlingSounds(Mobile attacker, Mobile defender, bool hit)
        {
            if (hit)
            {
                int hitSound = Utility.RandomList(0x3B2, 0x3B6, 0x3B6);

                attacker.PlaySound(hitSound);
                defender.PlaySound(hitSound);
            }

            else
            {
                int missSound = Utility.RandomList(0x238, 0x239);

                attacker.PlaySound(missSound);
                defender.PlaySound(missSound);
            }            
        }

        private static BaseWeapon m_Fists; // This value holds the default--fist--weapon


        public static BaseWeapon Fists
        {
            get { return m_Fists; }
            set { m_Fists = value; }
        }

        #region ICraftable Members

        private Dictionary<int, List<SlayerName>> ingotSlayerTiers = new Dictionary<int, List<SlayerName>>()
        {
            { 1,  new List<SlayerName>() { SlayerName.EarthShatter, SlayerName.SummerWind, SlayerName.LizardmanSlaughter, SlayerName.OrcSlaying } },
            { 2,  new List<SlayerName>() { SlayerName.SpidersDeath, SlayerName.SnakesBane, SlayerName.TrollSlaughter } },
            { 3,  new List<SlayerName>() { SlayerName.FlameDousing, SlayerName.WaterDissipation, SlayerName.Terathan } },
            { 4,  new List<SlayerName>() { SlayerName.OgreTrashing, SlayerName.GargoylesFoe, SlayerName.ElementalHealth } },
            { 5,  new List<SlayerName>() { SlayerName.ArachnidDoom, SlayerName.DaemonDismissal, SlayerName.BloodDrinking } },
            { 6,  new List<SlayerName>() { SlayerName.DragonSlaying, SlayerName.Repond } },
            { 7,  new List<SlayerName>() { SlayerName.ElementalBan, SlayerName.ReptilianDeath } },
            { 8,  new List<SlayerName>() { SlayerName.Exorcism, SlayerName.Silver } },
            { 9,  new List<SlayerName>() { SlayerName.Exorcism, SlayerName.Silver } },
        };

        private Dictionary<int, List<SlayerName>> woodSlayerTiers = new Dictionary<int, List<SlayerName>>()
        {
            { 1,  new List<SlayerName>() { SlayerName.EarthShatter, SlayerName.SummerWind, SlayerName.LizardmanSlaughter, SlayerName.OrcSlaying, SlayerName.SpidersDeath, SlayerName.SnakesBane, SlayerName.TrollSlaughter } },
            { 2,  new List<SlayerName>() { SlayerName.FlameDousing, SlayerName.WaterDissipation, SlayerName.Terathan, SlayerName.OgreTrashing, SlayerName.GargoylesFoe, SlayerName.ElementalHealth } },
            { 3,  new List<SlayerName>() { SlayerName.ArachnidDoom, SlayerName.DaemonDismissal, SlayerName.BloodDrinking } },
            { 4,  new List<SlayerName>() { SlayerName.DragonSlaying, SlayerName.Repond } },
            { 5,  new List<SlayerName>() { SlayerName.ElementalBan, SlayerName.ReptilianDeath } },
            { 6,  new List<SlayerName>() { SlayerName.Exorcism, SlayerName.Silver } },

        };

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (WeaponQuality)quality;

            if (makersMark)
                Crafter = from;

            PlayerConstructed = true;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            //For Runic Hammer
            if (tool is BaseRunicTool)
            {
                CraftResource thisResource = CraftResources.GetFromType(resourceType);

                //MUST USE THE SAME INGOT COLOR FOR THE RUNIC HAMMER TO GIVE MAGIC PROPERTIES!!!!!!
                if (thisResource == ((BaseRunicTool)tool).Resource)
                {
                    Resource = thisResource;

                    CraftContext context = craftSystem.GetContext(from);

                    if (context != null && context.DoNotColor)
                        Hue = 0;

                    int tier = 0;

                    switch (thisResource)
                    {
                        case CraftResource.DullCopper:
                            {
                                Identified = true;
                                tier = 1;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Durable;
                                DamageLevel = WeaponDamageLevel.Ruin;
                                AccuracyLevel = WeaponAccuracyLevel.Accurate;                               

                                break;
                            }

                        case CraftResource.ShadowIron:
                            {
                                Identified = true;
                                tier = 2;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Durable;
                                DamageLevel = WeaponDamageLevel.Ruin;
                                AccuracyLevel = WeaponAccuracyLevel.Accurate;                                

                                break;
                            }

                        case CraftResource.Copper:
                            {
                                Identified = true;
                                tier = 3;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Fortified;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;                                

                                break;
                            }

                        case CraftResource.Bronze:
                            {
                                Identified = true;
                                tier = 4;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Fortified;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;                                

                                break;
                            }

                        case CraftResource.Gold:
                            {
                                Identified = true;
                                tier = 5;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;                               

                                break;
                            }

                        case CraftResource.Agapite:
                            {
                                Identified = true;
                                tier = 6;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;                               

                                break;
                            }

                        case CraftResource.Verite:
                            {
                                Identified = true;
                                tier = 7;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;                                

                                break;
                            }

                        case CraftResource.Valorite:
                            {
                                Identified = true;
                                tier = 8;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;                                

                                break;
                            }

                        case CraftResource.Lunite:
                            {
                                Identified = true;
                                tier = 9;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];
                                //DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;                                

                                break;
                            }
                    }

                    from.SendMessage("The slayer weapon you've been working on is finally completed!");
                }
            }

            //Add small chance of obtain slayer archery wep from colored woods
            else if (from is PlayerMobile && (craftSystem is DefBowFletching || craftSystem is DefCarpentry))
            {
                CraftResource thisResource = CraftResources.GetFromType(resourceType);
                Resource = thisResource;

                PlayerMobile crafter = from as PlayerMobile;
                Double crafterSkill = 0.0;
                if (craftSystem is DefBowFletching)
                    crafterSkill = crafter.Skills.Fletching.Value;
                else //if (craftSystem is DefCarpentry)
                    crafterSkill = crafter.Skills.Carpentry.Value;
                Double slayerCraftChance = 0.03; //3%

                //Check if there is any bonus from having higher than 100
                if (crafterSkill > 100)
                {
                    Double bonusFromSkill = crafterSkill - 100;
                    slayerCraftChance += bonusFromSkill / 1000.0;
                }
                CraftContext context = craftSystem.GetContext(from);

                if (context != null && context.DoNotColor)
                    Hue = 0;

                if (thisResource != CraftResource.RegularWood && Utility.RandomDouble() < slayerCraftChance)
                {
                    int tier = 0;
                    switch (thisResource)
                    {
                        case CraftResource.OakWood:
                            {
                                Identified = true;
                                tier = 1;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }
                        case CraftResource.AshWood:
                            {
                                Identified = true;
                                tier = 2;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }
                        case CraftResource.YewWood:
                            {
                                Identified = true;
                                tier = 3;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }

                        case CraftResource.Bloodwood:
                            {
                                Identified = true;
                                tier = 4;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }

                        case CraftResource.Heartwood:
                            {
                                Identified = true;
                                tier = 5;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }

                        case CraftResource.Frostwood:
                            {
                                Identified = true;
                                tier = 6;
                                Slayer = woodSlayerTiers[tier][Utility.RandomMinMax(0, woodSlayerTiers[tier].Count - 1)];
                                break;
                            }
                    }
                    from.SendMessage("The slayer weapon you've been working on is finally completed!");
                }
            }

            //UO ANCORP - colored ingots give a small chance of crafting a slayer weapon.
            // The non runic is split from the runic one, so that it is easier for us to modify in the future in case it has different behavior
            else if (from is PlayerMobile && craftSystem is DefBlacksmithy)
            {
                CraftResource thisResource = CraftResources.GetFromType(resourceType);
                Resource = thisResource;

                var crafter = from as PlayerMobile;
                var crafterBlackSmithSkill = crafter.Skills.Blacksmith.Value;
                var slayerCraftChance = 0.06; //6%

                //Check if there is any bonus from having higher than 100 blacksmithy
                if (crafterBlackSmithSkill > 100)
                {
                    var extraSkill = crafterBlackSmithSkill - 100;
                    slayerCraftChance += extraSkill / 1000;
                }

                CraftContext context = craftSystem.GetContext(from);

                if (context != null && context.DoNotColor)
                    Hue = 0;

                int tier = 0;

                if (thisResource != CraftResource.Iron && Utility.RandomDouble() < slayerCraftChance)
                {
                    switch (thisResource)
                    {
                        case CraftResource.DullCopper:
                            {
                                Identified = true;
                                tier = 1;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.ShadowIron:
                            {
                                Identified = true;
                                tier = 2;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Copper:
                            {
                                Identified = true;
                                tier = 3;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Bronze:
                            {
                                Identified = true;
                                tier = 4;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Gold:
                            {
                                Identified = true;
                                tier = 5;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Agapite:
                            {
                                Identified = true;
                                tier = 6;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Verite:
                            {
                                Identified = true;
                                tier = 7;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Valorite:
                            {
                                Identified = true;
                                tier = 8;
                                Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                                break;
                            }

                        case CraftResource.Lunite:
                        {
                            Identified = true;
                            tier = 9;
                            Slayer = ingotSlayerTiers[tier][Utility.RandomMinMax(0, ingotSlayerTiers[tier].Count - 1)];

                            break;
                        }
                    }

                    from.SendMessage("The slayer weapon you've been working on is finally complete!");
                }
            }

            return quality;
        }

        #endregion
    }

    public enum CheckSlayerResult
    {
        None,
        Slayer,
        Opposition
    }
}
