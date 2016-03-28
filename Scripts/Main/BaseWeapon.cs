using System;
using System.Text;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Factions;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Guilds;
using Server.SkillHandlers;
using Server.Multis;
using Server.Custom;
using System.Globalization;

namespace Server.Items
{
    public abstract class BaseWeapon : Item, IWeapon, ICraftable, IDurability
    {
        public virtual int BaseMinDamage { get { return 1; } }
        public virtual int BaseMaxDamage { get { return 2; } }
        public virtual int BaseSpeed { get { return 30; } }
        public virtual int BaseMaxRange { get { return 1; } }
        public virtual int BaseHitSound { get { return 0x237; } }
        public virtual int BaseMissSound { get { return 0x23A; } }
        public virtual SkillName BaseSkill { get { return SkillName.Swords; } }
        public virtual WeaponType BaseType { get { return WeaponType.Slashing; } }
        public virtual WeaponAnimation BaseAnimation { get { return WeaponAnimation.Slash1H; } }

        public virtual int InitMinHits { get { return 40; } }
        public virtual int InitMaxHits { get { return 60; } }        

        public virtual bool TrainingWeapon { get { return false; } }

        public virtual int IconItemId { get { return ItemID; } }
        public virtual int IconHue { get { return Hue; } }
        public virtual int IconOffsetX { get { return 0; } } //65 is Baseline X For Gumps
        public virtual int IconOffsetY { get { return 0; } } //80 is Baseline Y For Gumps

        private SkillMod m_SkillMod;

        private SlayerGroupType m_SlayerGroup = SlayerGroupType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerGroupType SlayerGroup
        {
            get { return m_SlayerGroup; }
            set { m_SlayerGroup = value; }
        }    

        private int m_Hits;
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

        private int m_MaxHits;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHits; }
            set { m_MaxHits = value; InvalidateProperties(); }
        }

        private int m_PoisonCharges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get { return m_PoisonCharges; }
            set { m_PoisonCharges = value; InvalidateProperties(); }
        }

        private Poison m_Poison;
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; InvalidateProperties(); }
        }

        public override void QualityChange()
        {
            UnscaleDurability();
            ScaleDurability();

            if (UseSkillMod)
            {
                if (Quality == Quality.Low || Quality == Quality.Regular)
                {
                    if (m_SkillMod != null)
                        m_SkillMod.Remove();

                    m_SkillMod = null;
                }

                else if (Parent is Mobile)                
                    OnEquip(Parent as Mobile);                
            }

            InvalidateProperties();
        }
        
        public override void ResourceChange()
        {
            UnscaleDurability();
            Hue = CraftResources.GetHue(Resource);
            InvalidateProperties();
            ScaleDurability();
        }

        public override void DungeonChange()
        {
            if (Dungeon != DungeonEnum.None)
            {
                DungeonArmor.DungeonArmorDetail detail = new DungeonArmor.DungeonArmorDetail(Dungeon, TierLevel);

                if (detail != null)
                    Hue = detail.Hue;
            }
        }

        private WeaponDurabilityLevel m_DurabilityLevel;
        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponDurabilityLevel DurabilityLevel
        {
            get { return m_DurabilityLevel; }
            set { UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); }
        }

        private int m_MaxRange = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get
            {
                if (m_MaxRange == -1)
                    return BaseMaxRange;

                return m_MaxRange; 
            }

            set { m_MaxRange = value; InvalidateProperties(); }
        }        

        private WeaponAnimation m_Animation = WeaponAnimation.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation Animation
        {
            get
            {
                if (m_Animation == WeaponAnimation.None)
                    return BaseAnimation;

                return m_Animation; 
            }
            set { m_Animation = value; }
        }

        private WeaponType m_Type = WeaponType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponType Type
        {
            get 
            {
                if (m_Type == WeaponType.None)
                    return BaseType;

                return m_Type; 
            }
            set { m_Type = value; }
        }        

        private SkillName m_Skill = SkillName.Wrestling;
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get 
            {
                if (m_Skill == SkillName.Wrestling)
                    return BaseSkill;

                return m_Skill;
            }

            set { m_Skill = value; }
        }        

        private int m_HitSound = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get
            {
                if (m_HitSound == -1)
                    return BaseHitSound;

                return m_HitSound;
            }

            set { m_HitSound = value; }
        }
        
        private int m_MissSound = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MissSound
        {
            get
            {
                if (m_MissSound == -1)
                    return BaseMissSound;

                return m_MissSound; 
            }
            set { m_MissSound = value; }
        }

        private int m_MinDamage = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinDamage
        {
            get
            {
                if (DecorativeEquipment)
                    return 1;

                if (m_MinDamage == -1)
                    return BaseMinDamage;

                return m_MinDamage;
            }

            set { m_MinDamage = value; InvalidateProperties(); }
        }

        private int m_MaxDamage = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDamage
        {
            get
            {
                if (DecorativeEquipment)
                    return 2;

                if (m_MaxDamage == -1)
                    return BaseMaxDamage;

                return m_MaxDamage;
            }

            set { m_MaxDamage = value; InvalidateProperties(); }
        }

        private int m_Speed = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get 
            {
                if (m_Speed == -1)
                    return BaseSpeed;

                return m_Speed;
            }

            set { m_Speed = value; InvalidateProperties(); }
        }

        private WeaponAccuracyLevel m_AccuracyLevel;
        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAccuracyLevel AccuracyLevel
        {
            get { return m_AccuracyLevel; }
            set { m_AccuracyLevel = value; InvalidateProperties(); }
        }

        private WeaponDamageLevel m_DamageLevel;
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

        #region OnBeforeSwing

        public virtual void OnBeforeSwing(Mobile attacker, Mobile defender)
        {
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

            if (canSwing && attacker.HarmfulCheck(defender) && !attacker.Frozen)
            {
                attacker.DisruptiveAction();

                if (attacker.NetState != null)
                    attacker.Send(new Swing(0, attacker, defender));

                if (attacker is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)attacker;

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

        public static int PlayerFistSpeed = 50;

        public virtual TimeSpan GetDelay(Mobile mobile, bool useRawValues)
        {
            int speed = this.Speed;

            if (speed == 0)
                return TimeSpan.FromHours(1.0);

            BaseCreature bc_Creature = mobile as BaseCreature;
            PlayerMobile pm_Mobile = mobile as PlayerMobile;

            //Player: Wrestling Speed Override
            if (pm_Mobile != null && this is Fists)
                speed = PlayerFistSpeed;

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

                //Tamed Creature
                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                    v = (int)(((double)bc_Creature.RawDex + 100) * speed);
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

            if (!defender.Frozen) ;
            PlayHurtAnimation(defender);

            PlayerMobile pm_Attacker = attacker as PlayerMobile;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Attacker);
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Defender);

            DungeonArmor.PlayerDungeonArmorProfile attackerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(attacker, null);
            DungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(defender, null);

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

            //Decorative / Training Weapons
            if (DecorativeEquipment || TrainingWeapon)
                doWeaponSpecialAttack = false;

            //Base Damage
            int damage = ComputeDamage(attacker, defender);

            //Starting Bonus Damage Percent
            int percentageBonus = (int)(damageBonus * 100) - 100;

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
            if (m_SlayerGroup != SlayerGroupType.None && bc_Defender != null)
            {
                if (m_SlayerGroup == bc_Defender.SlayerGroup)
                {
                    defender.FixedEffect(0x37B9, 10, 5);
                    percentageBonus += 50;
                }
            }

            //UOACZ Undead
            if (m_SlayerGroup == SlayerGroupType.Undead)
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

                    int speedBonus = weapon.BaseSpeed - slowestWeaponSpeedPossible;
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

            if (CreatureAttacker)
                ((BaseCreature)attacker).AlterMeleeDamageTo(defender, ref damage);

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
                        damage = (int)((double)damage * 1.0 * UOACZSystem.HumanPlayerVsPlayerDamageScalar * UOACZSystem.GetFatigueScalar(pm_Attacker));

                    else if (pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 1.0 * UOACZSystem.UndeadPlayerVsPlayerDamageScalar * UOACZSystem.GetFatigueScalar(pm_Attacker));

                    else
                        damage = (int)((double)damage * 1.0);
                }

                else if (TamedDefender)
                {
                    if (pm_Attacker.IsUOACZHuman || pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 1.0 * MeleeDamageInflictedScalar * UOACZSystem.GetFatigueScalar(pm_Attacker));

                    else
                        damage = (int)((double)damage * 1.0 * MeleeDamageInflictedScalar);
                }

                else
                {
                    if (pm_Attacker.IsUOACZHuman)
                        damage = (int)((double)damage * 1.0 * UOACZSystem.HumanPlayerVsCreatureDamageScalar);

                    else if (pm_Attacker.IsUOACZUndead)
                        damage = (int)((double)damage * 1.0 * UOACZSystem.UndeadPlayerVsCreatureDamageScalar);

                    else
                        damage = (int)((double)damage * 1.0 * MeleeDamageInflictedScalar);
                }
            }

            //Tamed Creature Attacking
            else if (TamedAttacker)
            {
                PlayerMobile pm_Controller = bc_Attacker.ControlMaster as PlayerMobile;

                if (PlayerDefender)
                {
                    if (pm_Controller.IsUOACZHuman || pm_Controller.IsUOACZUndead)
                        damage = (int)((double)damage * 1.0 * bc_Attacker.PvPMeleeDamageScalar * MeleeDamageReceivedScalar * UOACZSystem.GetFatigueScalar(pm_Controller));

                    else
                        damage = (int)((double)damage * 1.0 * bc_Attacker.PvPMeleeDamageScalar * MeleeDamageReceivedScalar);
                }

                else if (TamedDefender)
                {
                    if (pm_Controller.IsUOACZHuman || pm_Controller.IsUOACZUndead)
                        damage = (int)((double)damage * 1.0 * UOACZSystem.GetFatigueScalar(pm_Controller));

                    else
                        damage = (int)((double)damage * 1.0);
                }

                else
                {
                    if (UOACZSystem.IsUOACZValidMobile(bc_Attacker))
                        damage = (int)((double)damage * 1.0 * UOACZSystem.TamedCreatureVsCreatureDamageScalar);

                    else
                        damage = (int)((double)damage * 1.0);
                }
            }

            //Normal Creature Attacking
            else
            {
                double ProvokedCreatureDamageInflictedScalar = 1.0;

                if (bc_Attacker.BardMaster != null)
                {
                    DungeonArmor.PlayerDungeonArmorProfile bardMasterDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(bc_Attacker.BardMaster, null);

                    if (bardMasterDungeonArmor.MatchingSet && !bardMasterDungeonArmor.InPlayerCombat)
                        ProvokedCreatureDamageInflictedScalar = bardMasterDungeonArmor.DungeonArmorDetail.ProvokedCreatureDamageInflictedScalar;
                }

                if (PlayerDefender)
                    damage = (int)((double)damage * 1.0 * MeleeDamageReceivedScalar);

                else if (TamedDefender)
                {
                    damage = (int)((double)damage * 1.0 * ProvokedCreatureDamageInflictedScalar);

                    if (UOACZSystem.IsUOACZValidMobile(bc_Defender))
                        damage = (int)((double)damage * 1.0 * UOACZSystem.CreatureVsTamedCreatureDamageScalar);
                }
            }

            //Armor Absorption
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

                    double speed = (double)weapon.BaseSpeed;

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
                        switch (weapon.Skill)
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

            //GetDamageTypes(attacker, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

            int damageGiven = damage;

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
            damageGiven = AOS.Damage(defender, attacker, damage, false, 100, 0, 0, 0, 0);

            //Dungeon Weapon
            if ((Dungeon != DungeonEnum.None && TierLevel > 0) && allowDungeonBonuses && pm_Attacker != null && bc_Defender != null)
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

            //Weapon Takes Durability Loss on Hit
            if (LootType != Server.LootType.Blessed && ParentEntity is PlayerMobile)
            {
                if (Utility.RandomDouble() <= .05)
                {
                    HitPoints--;

                    if (HitPoints == 5)
                    {
                        if (Parent is Mobile)
                            ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                    }

                    if (HitPoints == 0)
                        Delete();
                }
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
            if (owner.CheckSkill(SkillName.Parry, chance, 1.0))
            {
                double reduction = 0.65; // 35% base reduction
                // reduce damage 5% further for each mod

                if (Quality == Quality.Exceptional)
                    reduction -= 0.12;

                else
                    reduction -= (((int)m_DamageLevel + (int)m_AccuracyLevel) / 2) * 0.06;

                damage = (int)(damage * reduction);

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
                                Delete();
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

            if (Quality == Quality.Exceptional)
                bonus += 20;

            switch (m_DurabilityLevel)
            {
                case WeaponDurabilityLevel.Durable: bonus += 20; break;
                case WeaponDurabilityLevel.Substantial: bonus += 50; break;
                case WeaponDurabilityLevel.Massive: bonus += 70; break;
                case WeaponDurabilityLevel.Fortified: bonus += 100; break;
                case WeaponDurabilityLevel.Indestructible: bonus += 120; break;
            }

            return bonus;
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

            else if (!from.CanBeginAction(typeof(BaseWeapon)))
                return false;

            else
            {
            }

            return base.CanEquip(from);
        }

        public virtual bool UseSkillMod { get { return true; } }

        public override bool OnEquip(Mobile from)
        {
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

                if (Quality == Quality.Exceptional)
                    mod += 10;

                if (Dungeon != DungeonEnum.None && TierLevel > 0)
                    mod += DungeonWeapon.BaseTactics + (TierLevel * DungeonWeapon.TacticsPerTier);

                if (mod > 0)
                {
                    m_SkillMod = new DefaultSkillMod(SkillName.Tactics, true, mod);
                    from.AddSkillMod(m_SkillMod);
                }
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

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

                m.CheckStatTimers();

                m.Delta(MobileDelta.WeaponDamage);
            }
        }

        public virtual SkillName GetUsedSkill(Mobile m, bool checkSkillAttrs)
        {
            SkillName sk = Skill;

            if (sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value)
                sk = SkillName.Wrestling;            

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
            return false;
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
                if ((pm_Attacker != null || TamedAttacker) && pm_Defender != null)
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
            
            double ourValue;
            double theirValue;

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
                if (pm_Attacker != null)
                {
                    if (pm_Defender != null)
                        chance += 0;

                    else if (TamedDefender)
                        chance += .125;

                    else
                        chance += .25;
                }

                //Tamed Creature Attacking
                else if (TamedAttacker)
                {
                    if (pm_Defender != null)
                        chance += 0;

                    else if (TamedDefender)
                        chance += .125;

                    else
                        chance += .25;
                }

                //Normal Creature Attacking
                else
                {
                    if (pm_Defender != null)
                        chance += .25;

                    else if (TamedDefender)
                        chance += .25;
                }
            }

            #endregion
            
            //Dungeon Weapon
            if (Dungeon != DungeonEnum.None && TierLevel > 0 && pm_Defender == null)
                chance += DungeonWeapon.BaseAccuracy * (DungeonWeapon.AccuracyPerTier * (double)TierLevel);

            if (defender is PlayerMobile && ((PlayerMobile)defender).m_DateTimeDied + TimeSpan.FromSeconds(60) > DateTime.UtcNow)
                return (Utility.RandomDouble() < chance);

            else                            
                return attacker.CheckSkill(atkSkill.SkillName, chance, 1.0);            
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

        public static bool CheckParry(Mobile defender)
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
                double chance = parry / 200;

                return defender.CheckSkill(SkillName.Parry, chance, 1.0);
            }

            else if (!(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged))
            {
                double chance = parry / 200;

                return defender.CheckSkill(SkillName.Parry, chance, 1.0);
            }

            return false;
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
            }

            damage = AOS.Scale(damage, 100 + damageBonus);

            return damage / 100;
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
            phys = 100;
            fire = 0;
            cold = 0;
            pois = 0;
            nrgy = 0;
            chaos = 0;
            direct = 0;
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

            switch (Quality)
            {
                case Quality.Low: bonus -= 20; break;
                case Quality.Exceptional: bonus += 20; break;
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

            min = Math.Max((int)ScaleDamageOld(from, baseMin, false), 1);
            max = Math.Max((int)ScaleDamageOld(from, baseMax, false), 1);
        }

        public virtual double ScaleDamageAOS(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, 100.0, 1.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap, 1.0); // Passively check Anatomy for gain
                attacker.CheckSkill(SkillName.ArmsLore, 0.0, attacker.Skills[SkillName.ArmsLore].Cap, 1.0); // Passively check Arms Lore for gain

                /*
                if (Type == WeaponType.Axe)
                    attacker.CheckSkill(SkillName.Lumberjacking, 0.0, 100.0, 1.0); // Passively check Lumberjacking for gain
                */
            }

            #region Physical bonuses

            double strengthBonus = GetBonus(attacker.Str, 0.300, 100.0, 5.00);
            double anatomyBonus = GetBonus(attacker.Skills[SkillName.Anatomy].Value, 0.500, 100.0, 5.00);
            double tacticsBonus = GetBonus(attacker.Skills[SkillName.Tactics].Value, 0.625, 100.0, 6.25);
            double lumberBonus = GetBonus(attacker.Skills[SkillName.Lumberjacking].Value, 0.200, 100.0, 10.00);

            if (Type != WeaponType.Axe)
                lumberBonus = 0.0;

            #endregion

            #region Modifiers

            int damageBonus = AosAttributes.GetValue(attacker, AosAttribute.WeaponDamage);

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
                attacker.CheckSkill(SkillName.Tactics, 0.0, 125.0, 1.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap, 1.0); // Passively check Anatomy for gain
                attacker.CheckSkill(SkillName.ArmsLore, 0.0, attacker.Skills[SkillName.ArmsLore].Cap, 1.0); // Passively check Arms Lore for gain
            }

            double tacticsBase = attacker.Skills[SkillName.Tactics].Value;
            double tacticsBonus = 0;

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            if (pm_Attacker != null)
            {
                //Remove Dungeon Weapon Tactics Bonus Impact in PvP
                if (pm_Attacker.LastPlayerCombatTime + pm_Attacker.PlayerCombatExpirationDelay > DateTime.UtcNow)
                    tacticsBase -= DungeonWeapon.BaseTactics + (TierLevel * DungeonWeapon.TacticsPerTier);
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
                scale = 90 + ((10 * m_Hits) / m_MaxHits);

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
            SetSaveFlag(ref flags, SaveFlag.Hits, m_Hits != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxHits, m_MaxHits != 0);
            SetSaveFlag(ref flags, SaveFlag.Poison, m_Poison != null);
            SetSaveFlag(ref flags, SaveFlag.PoisonCharges, m_PoisonCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.MinDamage, m_MinDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxDamage, m_MaxDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.HitSound, m_HitSound != -1);
            SetSaveFlag(ref flags, SaveFlag.MissSound, m_MissSound != -1);
            SetSaveFlag(ref flags, SaveFlag.Speed, m_Speed != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxRange, m_MaxRange != -1);
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != (SkillName)(-1));
            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != (WeaponType)(-1));
            SetSaveFlag(ref flags, SaveFlag.Animation, m_Animation != (WeaponAnimation)(-1));

            writer.Write((int)flags);

            if (GetSaveFlag(flags, SaveFlag.DamageLevel))
                writer.Write((int)m_DamageLevel);

            if (GetSaveFlag(flags, SaveFlag.AccuracyLevel))
                writer.Write((int)m_AccuracyLevel);

            if (GetSaveFlag(flags, SaveFlag.DurabilityLevel))
                writer.Write((int)m_DurabilityLevel);
            
            if (GetSaveFlag(flags, SaveFlag.Hits))
                writer.Write((int)m_Hits);

            if (GetSaveFlag(flags, SaveFlag.MaxHits))
                writer.Write((int)m_MaxHits);
            
            if (GetSaveFlag(flags, SaveFlag.Poison))
                Poison.Serialize(m_Poison, writer);

            if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                writer.Write((int)m_PoisonCharges);        

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

                        if (GetSaveFlag(flags, SaveFlag.Hits))
                            m_Hits = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxHits))
                            m_MaxHits = reader.ReadInt();
                        
                        if (GetSaveFlag(flags, SaveFlag.Poison))
                            m_Poison = Poison.Deserialize(reader);

                        if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                            m_PoisonCharges = reader.ReadInt();
                        
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
                        
                        if (version >= 10)
                        {
                           
                        }

                        else
                        {
                           
                        }

                        if (UseSkillMod && (m_DamageLevel != WeaponDamageLevel.Regular) && Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }

                        else if (UseSkillMod && Quality == Quality.Exceptional && Parent is Mobile)                        
                            OnEquip(Parent as Mobile);                        

                        else if (UseSkillMod && (Dungeon != DungeonEnum.None && TierLevel > 0) && Parent is Mobile)
                        {
                            OnEquip(Parent as Mobile);
                        }

                        if (GetSaveFlag(flags, SaveFlag.PoisonPercent))
                            reader.ReadInt(); // LEGACY (No longer used)

                        break;
                    }
                case 4:
                    {
                        goto case 3;
                    }
                case 3:
                    {
                        goto case 2;
                    }
                case 2:
                    {
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
                        
                        m_Poison = Poison.Deserialize(reader);
                        m_PoisonCharges = reader.ReadInt();

                        break;
                    }
            }

            //Version 11
            if (version >= 11)
            {
            }

            //-----
            
            if (Parent is Mobile)
                ((Mobile)Parent).CheckStatTimers();

            if (m_Hits <= 0 && m_MaxHits <= 0)
                m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            
            if (UseSkillMod && m_DamageLevel != WeaponDamageLevel.Regular && Parent is Mobile)
                OnEquip(Parent as Mobile);

            if (UseSkillMod && Quality == Quality.Exceptional && Parent is Mobile)
                OnEquip(Parent as Mobile);
        }
        #endregion

        public BaseWeapon(int itemID): base(itemID)
        {
            Layer = (Layer)ItemData.Quality;

            Quality = Quality.Regular;
            
            m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            Resource = CraftResource.Iron;
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
        
        public int GetElementalDamageHue()
        {
            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

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

            switch (Resource)
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

                default: oreType = 0; break;
            }

            if (oreType != 0)
                list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~

            else if (Name == null)
                list.Add(LabelNumber);

            else
                list.Add(Name);
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return false;
        }

        public virtual int ArtifactRarity
        {
            get { return 0; }
        }

        public virtual int GetLuckBonus()
        {
            CraftResourceInfo resInfo = CraftResources.GetInfo(Resource);

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
                            DungeonArmor.PlayerDungeonArmorProfile attackerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(attacker, null);

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

            if (Quality == Quality.Exceptional)
                list.Add(1060636); // exceptional

            if (RequiredRace == Race.Elf)
                list.Add(1075086); // Elves Only

            if (ArtifactRarity > 0)
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~

            if (this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining)
                list.Add(1060584, ((IUsesRemaining)this).UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_Poison != null && m_PoisonCharges > 0)
                list.Add(1062412 + m_Poison.Level, m_PoisonCharges.ToString());

            base.AddResistanceProperties(list);

            int prop;

            if ((prop = (GetLuckBonus())) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

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

            list.Add(1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString()); // weapon damage ~1_val~ - ~2_val~

            if (Core.ML)
                list.Add(1061167, String.Format("{0}s", Speed)); // weapon speed ~1_val~
            else
                list.Add(1061167, Speed.ToString());

            if (MaxRange > 1)
                list.Add(1061169, MaxRange.ToString()); // range ~1_val~

            if (Layer == Layer.TwoHanded)
                list.Add(1061171); // two-handed weapon
            else
                list.Add(1061824); // one-handed weapon

            if (Core.SE)
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

        public override void DisplayLabelName(Mobile from)
        {
            if (from == null)
                return;

            //Dungeon Weapon
            if (Dungeon != DungeonEnum.None && TierLevel > 0)
            {
                string name = "";

                if (Name != null)
                    name = Name;

                if (name != "")
                    LabelTo(from, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name));
                else
                    base.OnSingleClick(from);

                LabelTo(from, GetDungeonName(Dungeon) + " Dungeon: Tier " + TierLevel.ToString());
                LabelTo(from, "(" + Experience.ToString() + "/" + DungeonWeapon.MaxDungeonExperience.ToString() + " xp) " + " Charges: " + ArcaneCharges.ToString());

                return;
            }

            bool isMagical = SlayerGroup != SlayerGroupType.None || DurabilityLevel != WeaponDurabilityLevel.Regular || AccuracyLevel != WeaponAccuracyLevel.Regular || DamageLevel != WeaponDamageLevel.Regular;

            string displayName = "";

            if (isMagical && !Identified && from.AccessLevel == AccessLevel.Player)
                LabelTo(from, "unidentified " + Name);

            else
            {
                if (Quality == Quality.Exceptional)
                    displayName += "exceptional ";

                if (DurabilityLevel != WeaponDurabilityLevel.Regular)
                    displayName += DurabilityLevel.ToString().ToLower() + " ";

                switch (AccuracyLevel)
                {
                    case WeaponAccuracyLevel.Accurate: displayName += "accurate "; break;
                    case WeaponAccuracyLevel.Surpassingly: displayName += "surpassingly accurate "; break;
                    case WeaponAccuracyLevel.Eminently: displayName += "eminently accurate "; break;
                    case WeaponAccuracyLevel.Exceedingly: displayName += "exceedingly accurate "; break;
                    case WeaponAccuracyLevel.Supremely: displayName += "supremely accurate "; break;
                }

                switch (DamageLevel)
                {
                    case WeaponDamageLevel.Ruin: displayName += "ruin "; break;
                    case WeaponDamageLevel.Might: displayName += "might "; break;
                    case WeaponDamageLevel.Force: displayName += "force "; break;
                    case WeaponDamageLevel.Power: displayName += "power "; break;
                    case WeaponDamageLevel.Vanq: displayName += "vanquishing "; break;
                }
                
                displayName += Name;

                if (SlayerGroup != SlayerGroupType.None)
                    displayName += " of " + SlayerGroup.ToString().ToLower() + " slaying";

                LabelTo(from, displayName);
            }

            if (m_Poison != null && m_PoisonCharges > 0)            
                LabelTo(from, m_PoisonCharges.ToString() + " " + m_Poison.Name.ToLower() + " poison charges");            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
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

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (Quality)quality;

            if (makersMark)
                DisplayCrafter = true;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            /*
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
                                DamageLevel = WeaponDamageLevel.Ruin;
                                AccuracyLevel = WeaponAccuracyLevel.Accurate;

                                break;
                            }

                        case CraftResource.ShadowIron:
                            {
                                Identified = true;
                                tier = 2;
                                DamageLevel = WeaponDamageLevel.Ruin;
                                AccuracyLevel = WeaponAccuracyLevel.Accurate;

                                break;
                            }

                        case CraftResource.Copper:
                            {
                                Identified = true;
                                tier = 3;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;

                                break;
                            }

                        case CraftResource.Bronze:
                            {
                                Identified = true;
                                tier = 4;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;

                                break;
                            }

                        case CraftResource.Gold:
                            {
                                Identified = true;
                                tier = 5;
                                DamageLevel = WeaponDamageLevel.Might;
                                AccuracyLevel = WeaponAccuracyLevel.Surpassingly;

                                break;
                            }

                        case CraftResource.Agapite:
                            {
                                Identified = true;
                                tier = 6;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;

                                break;
                            }

                        case CraftResource.Verite:
                            {
                                Identified = true;
                                tier = 7;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;

                                break;
                            }

                        case CraftResource.Valorite:
                            {
                                Identified = true;
                                tier = 8;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;

                                break;
                            }

                        case CraftResource.Lunite:
                            {
                                Identified = true;
                                tier = 9;
                                DamageLevel = WeaponDamageLevel.Force;
                                AccuracyLevel = WeaponAccuracyLevel.Eminently;

                                break;
                            }
                    }

                    from.SendMessage("The slayer weapon you've been working on is finally completed!");
                }
            }

            //Add small chance of obtain slayer archery wep from colored woods
            else if (from is PlayerMobile && craftSystem is DefCarpentry)
            {
                CraftResource thisResource = CraftResources.GetFromType(resourceType);
                Resource = thisResource;

                PlayerMobile crafter = from as PlayerMobile;
                Double crafterSkill = 0.0;

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
                                break;
                            }

                        case CraftResource.AshWood:
                            {
                                Identified = true;
                                tier = 2;
                                break;
                            }

                        case CraftResource.YewWood:
                            {
                                Identified = true;
                                tier = 3;
                                break;
                            }

                        case CraftResource.Bloodwood:
                            {
                                Identified = true;
                                tier = 4;
                                break;
                            }

                        case CraftResource.Heartwood:
                            {
                                Identified = true;
                                tier = 5;
                                break;
                            }

                        case CraftResource.Frostwood:
                            {
                                Identified = true;
                                tier = 6;
                                break;
                            }
                    }

                    from.SendMessage("The slayer weapon you've been working on is finally completed!");
                }
            }

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

                                break;
                            }

                        case CraftResource.ShadowIron:
                            {
                                Identified = true;
                                tier = 2;

                                break;
                            }

                        case CraftResource.Copper:
                            {
                                Identified = true;
                                tier = 3;

                                break;
                            }

                        case CraftResource.Bronze:
                            {
                                Identified = true;
                                tier = 4;

                                break;
                            }

                        case CraftResource.Gold:
                            {
                                Identified = true;
                                tier = 5;

                                break;
                            }

                        case CraftResource.Agapite:
                            {
                                Identified = true;
                                tier = 6;

                                break;
                            }

                        case CraftResource.Verite:
                            {
                                Identified = true;
                                tier = 7;

                                break;
                            }

                        case CraftResource.Valorite:
                            {
                                Identified = true;
                                tier = 8;

                                break;
                            }

                        case CraftResource.Lunite:
                            {
                                Identified = true;
                                tier = 9;

                                break;
                            }
                    }

                    from.SendMessage("The slayer weapon you've been working on is finally complete!");
                }
            }
            */

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

    public enum WeaponType
    {
        None,
        Axe,		// Axes, Hatches, etc. These can give concussion blows
        Slashing,	// Katana, Broadsword, Longsword, etc. Slashing weapons are poisonable
        Staff,		// Staves
        Bashing,	// War Hammers, Maces, Mauls, etc. Two-handed bashing delivers crushing blows
        Piercing,	// Spears, Warforks, Daggers, etc. Two-handed piercing delivers paralyzing blows
        Polearm,	// Halberd, Bardiche
        Ranged,		// Bow, Crossbows
        Fists		// Fists
    }

    public enum WeaponDamageLevel
    {
        Regular,
        Ruin,
        Might,
        Force,
        Power,
        Vanq
    }

    public enum WeaponAccuracyLevel
    {
        Regular,
        Accurate,
        Surpassingly,
        Eminently,
        Exceedingly,
        Supremely
    }

    public enum WeaponDurabilityLevel
    {
        Regular,
        Durable,
        Substantial,
        Massive,
        Fortified,
        Indestructible
    }

    public enum WeaponAnimation
    {
        None,
        Slash1H = 9,
        Pierce1H = 10,
        Bash1H = 11,
        Bash2H = 12,
        Slash2H = 13,
        Pierce2H = 14,
        ShootBow = 18,
        ShootXBow = 19,
        Wrestle = 31,
        Block = 30,
        Crossbow = 19
    } 
}
