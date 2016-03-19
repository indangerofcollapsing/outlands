using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("Sanguineous's corpse")]
    public class Sanguineous : BaseCreature
    {
        public enum BossPhase
        {
            PikeMounted,
            SwordShield,
            Axe,
            Sanguineous
        }

        public enum BloodPower
        {
            None,
            AttackSpeed,
            MartialSkill,
            SorcerousPower,
            ArcaneResistance
        }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextChargeAllowed;
        public TimeSpan NextChargeDelay = TimeSpan.FromSeconds(15);

        public DateTime m_NextAxeThrowAllowed;
        public TimeSpan NextAxeThrowDelay = TimeSpan.FromSeconds(15);

        public DateTime m_NextConfigureTraps;
        public TimeSpan NextConfigureTrapsDelay = TimeSpan.FromSeconds(40);

        public DateTime m_NextHarvestBlood;
        public TimeSpan NextHarvestBloodDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextBloodSpray;
        public TimeSpan NextBloodSprayDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextBloodBurstAllowed;
        public TimeSpan NextBloodBurstDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextSanguineousChargeAllowed;
        public TimeSpan NextSanguineousChargeDelay = TimeSpan.FromSeconds(20);
        
        public DateTime m_ChargeTimeout;
        public TimeSpan ChargeTimeoutDelay = TimeSpan.FromSeconds(10);

        public static TimeSpan BloodDuration = TimeSpan.FromSeconds(10);

        public bool m_IsCharging = false;
        public Mobile m_ChargeTarget;

        public DateTime m_NextAbilityAllowed;
        public double NextAbilityDelayMin = 10;
        public double NextAbilityDelayMax = 5;

        public int damageIntervalThreshold = 500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 50;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public BossPhase m_BossPhase = BossPhase.PikeMounted;
        public BloodPower m_BloodPower = BloodPower.None;

        public List<Mobile> m_Trampled = new List<Mobile>();
        public List<Mobile> m_LightningTargets = new List<Mobile>();

        public List<Mobile> m_ChargeTargets = new List<Mobile>();
        public List<Mobile> m_AxeTargets = new List<Mobile>();
        
        public List<Mobile> m_Creatures = new List<Mobile>();
        public List<Item> m_Traps = new List<Item>();
        public List<Item> m_Items = new List<Item>();

        public const int SwordShieldInterval = 4;
        public const int AxeInterval = 8;
        public const int SanguineousTrueFormInterval = 12;

        public string[] idleSpeech { get { return new string[] {"*stirs*"}; } }
        public string[] combatSpeech { get  { return new string[] {""}; } }
        
        public override bool AlwaysRun { get { return true; } }

        public int ThemeHue = 2118;

        [Constructable]
        public Sanguineous(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Sanguineous";
            SpeechHue = 0x22;

            Body = 400;
            Hue = 2117;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(25000);
            SetStam(25000);
            SetMana(25000);

            SetDamage(20, 40);

            SetSkill(SkillName.Swords, 110);
            SetSkill(SkillName.Fencing, 110);
            SetSkill(SkillName.Macing, 110);
            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 200;

            Horse mount = new Horse();
            mount.Body = 226;
            mount.Hue = 2118;
            mount.Rider = this;

            HairItemID = 8252;
            HairHue = 2118;

            AddItem(new DragonHelm() { Movable = false, Hue = ThemeHue, Name = "Bloody Helm" });
            AddItem(new PlateGorget() { Movable = false, Hue = ThemeHue, Name = "Bloody Gorget" });
            AddItem(new PlateChest() { Movable = false, Hue = ThemeHue, Name = "Bloody Chest" });
            AddItem(new PlateArms() { Movable = false, Hue = ThemeHue, Name = "Bloody Arms" });
            AddItem(new PlateLegs() { Movable = false, Hue = ThemeHue, Name = "Bloody Legs" });
            AddItem(new PlateGloves() { Movable = false, Hue = ThemeHue, Name = "Bloody Gloves" });
            AddItem(new Cloak() { Movable = false, Hue = ThemeHue, Name = "Bloody Cloak" });
         
            Backpack backpack = new Backpack();
            backpack.Movable = false;
            AddItem(backpack);
            
            SanguinPike pike = new SanguinPike();
            pike.Movable = false;
            pike.Hue = ThemeHue;
            pike.Name = "Bloody Pike";
            m_Items.Add(pike);
            AddItem(pike);            

            VikingSword sword = new VikingSword();
            sword.Movable = false;
            sword.Hue = ThemeHue;
            sword.Name = "Bloody Sword";
            m_Items.Add(sword);
            backpack.DropItem(sword);

            DupresShield shield = new DupresShield();
            shield.Movable = false;
            shield.Hue = ThemeHue;
            shield.Name = "Bloody Barricade";
            m_Items.Add(shield);
            backpack.DropItem(shield);

            ExecutionersAxe axe = new ExecutionersAxe();
            axe.Movable = false;
            axe.Hue = ThemeHue;
            axe.Name = "Bloody Axe";
            m_Items.Add(axe);
            backpack.DropItem(axe);
        }

        public virtual int AttackRange
        { 
            get
            {
                switch (m_BossPhase)
                {
                    case BossPhase.PikeMounted: return 1; break;
                    case BossPhase.SwordShield: return 1; break;
                    case BossPhase.Sanguineous: return 2; break;
                }

                return 2;
            }
        }
        
        public override bool AlwaysBoss { get { return true; } }
        public override string TitleReward { get { return "Slayer of Sanguineous"; } }
        public override string BossSpawnMessage { get { return "Sanguineous has arisen and stirs within Wrong Dungeon..."; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType 
        {
            get       
            {
                if (m_BossPhase == BossPhase.Sanguineous)
                    return true;

                return false;
            }
        }

        public override int AttackAnimation 
        {
            get 
            {
                if (m_BossPhase == BossPhase.PikeMounted)
                    return 28;

                if (m_BossPhase == BossPhase.Sanguineous) 
                    return 5;

                return -1; 
            } 
        }

        public override int AttackFrames
        {
            get
            {
                if (m_BossPhase == BossPhase.PikeMounted)
                    return 7;

                if (m_BossPhase == BossPhase.Sanguineous)
                    return 8;

                return 0;
            }
        }

        public override int HurtAnimation { get { if (m_BossPhase == BossPhase.Sanguineous) return 28; return -1; } }
        public override int HurtFrames { get { if (m_BossPhase == BossPhase.Sanguineous) return 8; return 0; } }
        public override bool HurtAnimationPlayForwards { get { if (m_BossPhase == BossPhase.Sanguineous) return false; return true; } }

        public override int IdleAnimation { get { if (m_BossPhase == BossPhase.Sanguineous) return 25; return -1; } }
        public override int IdleFrames { get { if (m_BossPhase == BossPhase.Sanguineous) return 12; return 0; } }
                
        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 1.5;

            SetSpeeds(false);

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            foreach (Item item in m_Items)
            {
                if (item.Hue != ThemeHue)
                    item.Hue = ThemeHue;
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            switch (m_BossPhase)
            {
                case BossPhase.PikeMounted:
                break;

                case BossPhase.SwordShield:
                break;

                case BossPhase.Axe:
                    double bleedChance = .25;

                    SpecialAbilities.FrenzySpecialAbility(bleedChance, this, defender, .5, 15, -1, true, "", "", "*becomes frenzied*");
                break;

                case BossPhase.Sanguineous:
                    bleedChance = .25;

                    SpecialAbilities.BleedSpecialAbility(bleedChance, this, defender, DamageMin, 1.0, -1, true, "", "Their strike causes you to bleed!");
                break;
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (m_BossPhase == BossPhase.Axe)
            {
                double effectChance = .1;

                SpecialAbilities.EnrageSpecialAbility(effectChance, attacker, this, .1, 15, -1, true, "Your attack enrages Sanguineous.", "", "*becomes enraged*");
            }
        }

        public override bool OnBeforeHarmfulSpell()
        {
            if (m_BossPhase == BossPhase.SwordShield)
            {
                double effectChance = .75;

                if (Utility.RandomDouble() <= effectChance)
                {
                    MagicDamageAbsorb = 1;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, .5, false, -1, false, "", "");
                    
                    Animate(30, 5, 1, true, false, 0);
                    PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*parries spell*");
                }
            }

            return true;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {           
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            bool transformation = false;

            if (intervalCount >= SwordShieldInterval && intervalCount < AxeInterval && m_BossPhase != BossPhase.SwordShield)
            {
                SwordShieldTransform();

                transformation = true;
            }

            else if (intervalCount >= AxeInterval && intervalCount < SanguineousTrueFormInterval && m_BossPhase != BossPhase.Axe)
            {
                AxeTransform();

                transformation = true;
            }

            else if (intervalCount >= SanguineousTrueFormInterval && m_BossPhase != BossPhase.Sanguineous)
            {
                TrueFormTransform();

                transformation = true;
            }

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    spawnPercent = (double)intervalCount / (double)totalIntervals;

                    if (m_BossPhase == BossPhase.Sanguineous && !transformation)
                    {
                        if (intervalCount % 5 == 0)
                            SummonTheBlood();

                        else
                        {
                            switch (Utility.RandomMinMax(1, 3))
                            {
                                case 1: BloodBurst(); break;
                                case 2: BloodStorm(); break;
                                case 3: IgniteTheBlood(true); break;                                
                            }
                        }
                    }                    
                }

                else if (!transformation)
                {
                    switch (m_BossPhase)
                    {
                        case BossPhase.PikeMounted:
                        break;

                        case BossPhase.SwordShield:
                            double rangedReflectChance = .75;

                            if (from != null)
                            {
                                if (Utility.RandomDouble() <= rangedReflectChance && from.Weapon != null)
                                {
                                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                                    if (weapon is BaseRanged)
                                        RangedReflect(from, amount);
                                }
                            }
                        break;

                        case BossPhase.Axe:
                        break;

                        case BossPhase.Sanguineous:
                        break;
                    }
                }
            }            

            base.OnDamage(amount, from, willKill);
        }

        #region Transformations

        public void SwordShieldTransform()
        {
            Item sword = null;
            Item shield = null;

            Item leftHand = null;
            Item rightHand = null;

            m_BossPhase = BossPhase.SwordShield;

            IMount mount = this.Mount;

            if (mount != null)
            {
                mount.Rider = null;

                if (mount is Mobile)
                    ((Mobile)mount).Delete();
            }

            BloodCourser courser = new BloodCourser();
            courser.MoveToWorld(Location, Map);

            m_Creatures.Add(courser);
            courser.PlaySound(courser.GetAngerSound());

            courser.m_NextChargeAllowed = DateTime.UtcNow + courser.NextChargeDelay;

            m_IsCharging = false;
            m_ChargeTarget = null;

            SetSpeeds(false);

            sword = Backpack.FindItemByType(typeof(VikingSword));
            shield = Backpack.FindItemByType(typeof(DupresShield));

            leftHand = FindItemOnLayer(Layer.OneHanded);
            rightHand = FindItemOnLayer(Layer.TwoHanded);

            if (leftHand != null)
                Backpack.DropItem(leftHand);

            if (rightHand != null)
                Backpack.DropItem(rightHand);

            if (shield != null)
                EquipItem(shield);

            if (sword != null)
                EquipItem(sword);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*readies bloody sword and shield*");
        }

        public void AxeTransform()
        {
            Item sword = null;
            Item shield = null;

            Item leftHand = null;
            Item rightHand = null;

            m_BossPhase = BossPhase.Axe;

            Item axe = Backpack.FindItemByType(typeof(ExecutionersAxe));

            leftHand = FindItemOnLayer(Layer.OneHanded);
            rightHand = FindItemOnLayer(Layer.TwoHanded);

            if (leftHand != null)
                Backpack.DropItem(leftHand);

            if (rightHand != null)
                Backpack.DropItem(rightHand);

            if (axe != null)
                EquipItem(axe);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*readies bloody axe*"); 
        }

        public void TrueFormTransform()
        {
            m_BossPhase = BossPhase.Sanguineous;

            BodyValue = 741;
            Hue = 0;

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*becomes trueform sanguineous*");

            IgniteTheBlood(false);         
        }

        #endregion

        public TimeSpan GetNextAbilityDelay()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            return TimeSpan.FromSeconds(NextAbilityDelayMin - ((NextAbilityDelayMin - NextAbilityDelayMax) * spawnPercent));
        }

        public void SetSpeeds(bool charging)
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (charging)
            {
                ActiveSpeed = 0.25 - (.075 * spawnPercent);
                PassiveSpeed = 0.25 - (.075 * spawnPercent);
                CurrentSpeed = 0.25 - (.075 * spawnPercent);
            }

            else
            {
                ActiveSpeed = 0.25;
                PassiveSpeed = 0.25;
                CurrentSpeed = 0.25;

                DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 4;

                AbilityInProgress = false;
            }
        }

        #region Ranged Reflect

        public void RangedReflect(Mobile from, int damageAmount)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (damageAmount > DamageMax)
                damageAmount = DamageMax;

            int itemId = 7166;
            int itemHue = 0;

            bool crossbow = true;

            if (weapon is Bow)
            {
                crossbow = false;
                itemId = 3906;
            }

            Animate(30, 5, 1, true, false, 0);
            
            double distance = GetDistanceToSqrt(from.Location);
            double destinationDelay = (double)distance * .06;

            Point3D location = Location;
            Point3D targetLocation = from.Location;
            Map map = from.Map;

            if (crossbow)
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*parries bolt*");
            else
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*parries arrow*");

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, .5, false, -1, false, "", "");

            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (!SpecialAbilities.Exists(from)) return;
                
                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);

                Effects.SendMovingEffect(startLocation, endLocation, itemId, 18, 1, false, false, itemHue, 0);

                Direction = Utility.GetDirection(location, targetLocation);

                PlaySound(0x3B4); //0x51B
                FixedEffect(0x37B9, 10, 16);

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(targetLocation, 1);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double damage = (double)damageAmount;

                        if (mobile is BaseCreature)
                            damage *= 2;

                        int finalDamage = (int)(Math.Ceiling(damage));

                        DoHarmful(mobile);

                        new Blood(BloodDuration).MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                    }                    
                });
            });
        }

        #endregion

        #region Charge

        public void BeginCharge()
        {
            SetSpeeds(true);

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;
            
            Dictionary<Mobile, int> DictPossibleNewCombatants = new Dictionary<Mobile, int>();

            IPooledEnumerable m_NearbyMobiles = Map.GetMobilesInRange(Location, 18);

            foreach (Mobile mobile in m_NearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!mobile.InLOS(this)) continue;
                if (mobile.Hidden) continue;

                int distance = Utility.GetDistance(mobile.Location, Location);

                if (mobile == Combatant && distance <= 4)
                    continue;

                DictPossibleNewCombatants.Add(mobile, distance);
            }

            m_NearbyMobiles.Free();          

            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                TotalValues += pair.Value;
            }

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundNewCombatant = false;

            //Determine Combatant                      
            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    Combatant = pair.Key;
                    foundNewCombatant = true;

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            if (Combatant != null && foundNewCombatant)
            {
                Paralyzed = false;
                CantWalk = false;
                Frozen = false;

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
                DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 1000;

                Effects.PlaySound(Location, Map, GetAngerSound());

                m_IsCharging = true;
                m_ChargeTarget = Combatant;

                SetSpeeds(true);

                Effects.PlaySound(Location, Map, 0x59B);
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*charges*");

                m_Trampled.Clear();               
                m_ChargeTimeout = DateTime.UtcNow + ChargeTimeoutDelay;
            }            
        }

        public void CheckChargeResolved()
        {
            if (m_IsCharging)
            {
                bool chargeComplete = false;
                bool chargeFail = false;

                if (Combatant == null || m_ChargeTarget == null)
                    chargeFail = true;

                else if (DamageIntervalInProgress)
                    chargeFail = true;

                else if (Combatant != m_ChargeTarget || !Combatant.Alive || Combatant.Hidden || GetDistanceToSqrt(Combatant.Location) > 24 || !InLOS(Combatant) || DateTime.UtcNow > m_ChargeTimeout)
                    chargeFail = true;

                if (chargeFail)
                {
                    m_IsCharging = false;
                    m_ChargeTarget = null;

                    SetSpeeds(false);                    
                }

                else
                {
                    if (GetDistanceToSqrt(Combatant) <= 1.5)
                        ResolveCharge();
                }
            }

            else
            {
                m_ChargeTarget = null;

                SetSpeeds(false);
            }
        }

        public void CheckTrample()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 1);

            Queue m_Queue = new Queue();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (m_Trampled.Contains(mobile)) continue;

                if (Combatant != null)
                {
                    if (Utility.GetDistance(Combatant.Location, mobile.Location) >= Utility.GetDistance(Combatant.Location, Location))
                        continue;
                }

                m_Queue.Enqueue(mobile);
            }

            nearbyMobiles.Free();

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();

                Effects.PlaySound(Location, Map, Utility.RandomList(0x3BB, 0x3BA, 0x3B9));
                Effects.PlaySound(mobile.Location, mobile.Map, mobile.GetHurtSound());

                double damage = DamageMin;

                if (Combatant is BaseCreature)
                    damage *= 2;

                new Blood(BloodDuration).MoveToWorld(Combatant.Location, Combatant.Map);

                int bloodCount = 3;

                for (int a = 0; a < bloodCount; a++)
                {
                    new Blood(BloodDuration).MoveToWorld(new Point3D(Combatant.Location.X + Utility.RandomList(-2, 2), Combatant.Location.Y + Utility.RandomList(-2, 2), Combatant.Location.Z), Map);
                }

                if (mobile is PlayerMobile)
                    mobile.Animate(21, 6, 1, true, false, 0);

                else if (mobile is BaseCreature)
                {
                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature.IsHighSeasBodyType)
                        bc_Creature.Animate(2, 14, 1, true, false, 0);

                    else if (bc_Creature.Body != null)
                    {
                        if (bc_Creature.Body.IsHuman)
                            bc_Creature.Animate(21, 6, 1, true, false, 0);

                        else
                            bc_Creature.Animate(2, 4, 1, true, false, 0);
                    }
                }

                mobile.PublicOverheadMessage(MessageType.Regular, 0, false, "*trampled*");

                SpecialAbilities.HinderSpecialAbility(1.0, this, mobile, 1.0, 2, false, -1, false, "", "You have been trampled and knocked down");

                new Blood(BloodDuration).MoveToWorld(Combatant.Location, Combatant.Map);
                AOS.Damage(Combatant, (int)damage, 100, 0, 0, 0, 0);
            }
        }

        public void ResolveCharge()
        {
            if (Combatant == null)
                return;

            m_IsCharging = false;
            m_ChargeTarget = null;

            SetSpeeds(false);

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;

            int bloodCount = 4;

            for (int a = 0; a < bloodCount; a++)
            {
                new Blood(BloodDuration).MoveToWorld(new Point3D(Combatant.Location.X + Utility.RandomList(-2, 2), Combatant.Location.Y + Utility.RandomList(-2, 2), Combatant.Location.Z), Map);
            }

            if (Combatant is PlayerMobile)
                Combatant.Animate(21, 6, 1, true, false, 0);

            else if (Combatant is BaseCreature)
            {
                BaseCreature bc_Combatant = Combatant as BaseCreature;

                if (bc_Combatant.IsHighSeasBodyType)
                    bc_Combatant.Animate(2, 14, 1, true, false, 0);

                else if (bc_Combatant.Body != null)
                {
                    if (bc_Combatant.Body.IsHuman)
                        bc_Combatant.Animate(21, 6, 1, true, false, 0);

                    else
                        bc_Combatant.Animate(2, 4, 1, true, false, 0);
                }
            }

            Combatant.PublicOverheadMessage(MessageType.Regular, 0, false, "*trampled*");

            SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, Combatant, 25, 3, -1, "", "You are knocked back by their charge!");

            double damage = DamageMax;

            if (Combatant is BaseCreature)
                damage *= 2;

            new Blood(BloodDuration).MoveToWorld(Combatant.Location, Combatant.Map);
            AOS.Damage(Combatant, (int)damage, 100, 0, 0, 0, 0);
        }

        #endregion     
   
        #region Axe Throw

        public void AxeThrow()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_AxeTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 20;

            List<Mobile> m_PossibleTargets = new List<Mobile>();

            IPooledEnumerable m_NearbyMobiles = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in m_NearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!mobile.InLOS(this)) continue;
                if (mobile.Hidden) continue;

                m_PossibleTargets.Add(mobile);
            }

            m_NearbyMobiles.Free();

            m_NextAxeThrowAllowed = DateTime.UtcNow + NextAxeThrowDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_PossibleTargets.Count == 0)
                return;

            double directionDelay = .25;
            double initialDelay = 1 - (.5 * spawnPercent);
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            Point3D location = Location;
            Map map = Map;

            Mobile mobileTarget = m_PossibleTargets[Utility.RandomMinMax(0, m_PossibleTargets.Count - 1)];

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*prepares to throw axe*");

                Direction = Utility.GetDirection(Location, targetLocation);

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    Animate(12, 7, 1, true, false, 0);
                    PlaySound(GetAngerSound());

                   Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                   {
                       if (!SpecialAbilities.Exists(this))
                           return;

                       bool validTarget = true;

                       if (mobileTarget == null)
                           validTarget = false;

                       else
                       {
                           if (!SpecialAbilities.MonsterCanDamage(this, mobileTarget)) validTarget = false;
                           if (mobileTarget.Hidden) validTarget = false;
                           if (!Map.InLOS(Location, mobileTarget.Location)) validTarget = false;
                           if (Utility.GetDistance(Location, mobileTarget.Location) > range + 10) validTarget = false;
                       }

                       if (validTarget)
                           targetLocation = mobileTarget.Location;

                       Effects.PlaySound(Location, Map, 0x5D3);

                       IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                       IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobileTarget.Location.X, mobileTarget.Location.Y, mobileTarget.Location.Z + 5), mobileTarget.Map);

                       int itemID = 5114;
                       int itemHue = 38;
                       int hitSound = 0x237;

                       int itemIdToUse = 3912;
                       int itemIdA = 3912;
                       int itemIdB = 3911;

                       switch (Utility.GetDirection(Location, mobileTarget))
                       {
                           case Server.Direction.Up: itemIdToUse = itemIdB; break;
                           case Server.Direction.North: itemIdToUse = itemIdB; break;
                           case Server.Direction.Right: itemIdToUse = itemIdB; break;
                           case Server.Direction.East: itemIdToUse = itemIdB; break;
                           case Server.Direction.Down: itemIdToUse = itemIdA; break;
                           case Server.Direction.South: itemIdToUse = itemIdA; break;
                           case Server.Direction.Left: itemIdToUse = itemIdA; break;
                           case Server.Direction.West: itemIdToUse = itemIdA; break;
                       }

                       Effects.SendMovingEffect(startLocation, endLocation, itemID, 10, 0, false, false, itemHue - 1, 0);
                       
                       Point3D effectStep = Location;                       

                       Queue m_Queue;

                       IPooledEnumerable nearbyMobiles;

                       int distance = Utility.GetDistance(Location, targetLocation);
                       Direction direction;

                       for (int a = 0; a < distance; a++)
                       {
                           if (!SpecialAbilities.Exists(this))
                               return;

                           direction = Utility.GetDirection(effectStep, targetLocation);
                           effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                           Point3D effectLocation = effectStep;
                           int index = a;

                           m_Queue = new Queue();

                           nearbyMobiles = map.GetMobilesInRange(effectStep, 1);

                           foreach (Mobile mobile in nearbyMobiles)
                           {
                               if (mobile == this) continue;
                               if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                               if (m_AxeTargets.Contains(mobile))
                                   continue;
                               else
                                   m_AxeTargets.Add(mobile);                                   

                               m_Queue.Enqueue(mobile);
                           }

                           nearbyMobiles.Free();

                           while (m_Queue.Count > 0)
                           {
                               Mobile mobile = (Mobile)m_Queue.Dequeue();                               

                               double damage = (double)DamageMin;

                               if (mobile is BaseCreature)
                                   damage *= 2;

                               int finalDamage = (int)(Math.Ceiling(damage));

                               DoHarmful(mobile);

                               mobile.PlaySound(hitSound);
                                                              
                               new Blood().MoveToWorld(mobile.Location, mobile.Map);
                               AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                           }
                       }
                   });                    
                });
            });
        }

        #endregion

        #region Configure Traps

        public void ConfigureTraps()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double directionDelay = .25;
            double initialDelay = 1;

            double totalDelay = directionDelay + initialDelay + 1;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextConfigureTraps = DateTime.UtcNow + NextConfigureTrapsDelay;
            AbilityInProgress = true;

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*readies bloody traps*");

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                switch (m_BossPhase)
                {
                    case BossPhase.PikeMounted:
                        Animate(29, 5, 1, false, false, 0);
                        PlaySound(GetAngerSound());
                    break;

                    case BossPhase.SwordShield:
                        Animate(17, 7, 1, false, false, 0);
                        PlaySound(GetAngerSound());
                    break;

                    case BossPhase.Axe:
                        Animate(17, 7, 1, false, false, 0);
                        PlaySound(GetAngerSound());
                    break;

                    case BossPhase.Sanguineous:
                        Animate(11, 10, 1, false, false, 0);
                        PlaySound(GetAngerSound());
                    break;
                }

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    double spawnPercent = (double)intervalCount / (double)totalIntervals;

                    int trapsToPlace = (int)(Math.Round(10 + (20 * spawnPercent)));

                    Queue m_Queue = new Queue();

                    foreach (SanguineousTrap trap in m_Traps)
                    {
                        if (trap == null) continue;
                        if (trap.Deleted) continue;

                        m_Queue.Enqueue(trap);
                    }

                    while (m_Queue.Count > 0)
                    {
                        SanguineousTrap trap = (SanguineousTrap)m_Queue.Dequeue();
                        trap.Delete();
                    }

                    m_Traps.Clear();

                    List<Mobile> m_ValidMobiles = new List<Mobile>();

                    IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 20);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                        m_ValidMobiles.Add(mobile);
                    }

                    nearbyMobiles.Free();

                    List<Point3D> m_TrapLocations = new List<Point3D>();

                    int trapsRemainingToPlace = trapsToPlace;
                    int maxTrapsPerMobile = 3;

                    for (int a = 0; a < maxTrapsPerMobile; a++)
                    {
                        for (int b = 0; b < m_ValidMobiles.Count; b++)
                        {
                            Mobile mobile = m_ValidMobiles[b];

                            Point3D targetLocation;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(mobile.Location, true, true, mobile.Location, mobile.Map, 1, 15, 0, 3, true);

                            if (m_ValidLocations.Count > 0)
                                targetLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                            else
                                targetLocation = mobile.Location;

                            if (!m_TrapLocations.Contains(targetLocation))
                                m_TrapLocations.Add(targetLocation);

                            trapsRemainingToPlace--;

                            if (trapsRemainingToPlace <= 0)
                                break;
                        }

                        if (trapsRemainingToPlace <= 0)
                            break;
                    }

                    if (trapsRemainingToPlace > 0)
                    {
                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, true, Location, Map, trapsRemainingToPlace, 15, 1, 8, true);

                        for (int a = 0; a < m_ValidLocations.Count; a++)
                        {
                            Point3D trapLocation = m_ValidLocations[a];

                            if (!m_TrapLocations.Contains(trapLocation))
                                m_TrapLocations.Add(trapLocation);
                        }
                    }

                    foreach (Point3D point in m_TrapLocations)
                    {
                        SanguineousTrap.TrapType trapType = (SanguineousTrap.TrapType)Utility.RandomMinMax(0, Enum.GetNames(typeof(SanguineousTrap.TrapType)).Length - 1);
                        SanguineousTrap trap = new SanguineousTrap(trapType, this, DateTime.UtcNow + TimeSpan.FromMinutes(5));

                        trap.MoveToWorld(point, Map);
                        m_Traps.Add(trap);

                        Point3D adjustedPoint = point;
                        adjustedPoint.Z++;

                        TimedStatic trapTile = new TimedStatic(6179, 1 - (.5 * spawnPercent));
                        trapTile.Name = "sanguineous trap";
                        trapTile.Hue = 2118;
                        trapTile.MoveToWorld(adjustedPoint, Map);

                        TimedStatic trapGlow = new TimedStatic(14202, 1 - (.5 * spawnPercent));
                        trapGlow.Name = "sanguineous glow";
                        trapGlow.Hue = 2118;
                        trapGlow.MoveToWorld(adjustedPoint, Map);

                        Effects.PlaySound(Location, Map, 0x1F0); 
                    }
                });
            });
        }

        #endregion

        #region Sanguineous Charge

        public void SanguineousCharge()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_ChargeTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            m_NextSanguineousChargeAllowed = DateTime.UtcNow + NextSanguineousChargeDelay;

            int range = 18;

            Dictionary<Mobile, int> DictPossibleNewCombatants = new Dictionary<Mobile, int>();

            IPooledEnumerable m_NearbyMobiles = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in m_NearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!mobile.InLOS(this)) continue;
                if (mobile.Hidden) continue;

                int distance = Utility.GetDistance(mobile.Location, Location);

                if (mobile == Combatant && distance <= 4)
                    continue;

                DictPossibleNewCombatants.Add(mobile, distance);
            }

            m_NearbyMobiles.Free();

            if (DictPossibleNewCombatants.Count == 0)
                return;

            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                TotalValues += pair.Value;
            }

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundNewCombatant = false;

            //Determine Combatant                      
            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    Combatant = pair.Key;
                    foundNewCombatant = true;

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            if (Combatant == null || !foundNewCombatant)
                return;
           
            double directionDelay = .25;
            double initialDelay = 1 - (.5 * spawnPercent);
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = Combatant.Location;
            Map targetMap = Combatant.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*charges*");

                Animate(15, 6, 1, true, false, 0);
                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    bool validCombatant = true;

                    if (Combatant == null)
                        validCombatant = false;

                    else
                    {
                        if (!SpecialAbilities.MonsterCanDamage(this, Combatant)) validCombatant = false;
                        if (Combatant.Hidden) validCombatant = false;
                        if (!Map.InLOS(Location, Combatant.Location)) validCombatant = false;
                        if (Utility.GetDistance(Location, Combatant.Location) > range + 10) validCombatant = false;
                    }

                    if (validCombatant)
                        targetLocation = Combatant.Location;

                    Point3D effectStep = Location;

                    int distance = Utility.GetDistance(effectStep, targetLocation);
                    double stepDelay = .04;

                    Queue m_Queue;

                    IPooledEnumerable nearbyMobiles;

                    for (int a = 0; a < distance; a++)
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Direction direction = Utility.GetDirection(effectStep, targetLocation);
                        effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                        Point3D effectLocation = effectStep;
                        int index = a;

                        Timer.DelayCall(TimeSpan.FromSeconds(index * stepDelay), delegate
                        {
                            int chargeEffect = 14276;
                            int chargeHue = 38;

                            Effects.SendLocationParticles(EffectItem.Create(effectLocation, map, EffectItem.DefaultDuration), chargeEffect, 10, 10, chargeHue - 1, 0, 2023, 0);
                            Effects.PlaySound(effectLocation, map, 0x60B); //0x5C6

                            Point3D adjustedLocation = effectLocation;
                            adjustedLocation.Z++;

                            for (int b = 0; b < 3; b++)
                            {
                                Point3D extraBloodLocation = Utility.GetRandomLocation(adjustedLocation, 1, false);
                                new Blood(BloodDuration).MoveToWorld(extraBloodLocation, map);
                            }

                            new Blood(BloodDuration).MoveToWorld(adjustedLocation, map);

                            int projectiles = 3;
                            int particleSpeed = 8;
                            double distanceDelayInterval = .12;

                            int minRadius = 1;
                            int maxRadius = 5;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(adjustedLocation, true, false, adjustedLocation, Map, projectiles, 20, minRadius, maxRadius, false);

                            if (m_ValidLocations.Count == 0)
                                return;

                            for (int b = 0; b < projectiles; b++)
                            {
                                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 2), Map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), Map);

                                newLocation.Z += 5;

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                            }
                        });

                        m_Queue = new Queue();

                        nearbyMobiles = map.GetMobilesInRange(effectStep, 1);

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == this) continue;
                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                            if (m_ChargeTargets.Contains(mobile))
                                continue;
                            else
                                m_ChargeTargets.Add(mobile);

                            m_Queue.Enqueue(mobile);
                        }

                        nearbyMobiles.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            double damage = (double)DamageMin;

                            if (mobile is BaseCreature)
                                damage *= 2;

                            int finalDamage = (int)(Math.Ceiling(damage));

                            DoHarmful(mobile);

                            mobile.PlaySound(mobile.GetHurtSound());

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                        }
                    }

                    Location = targetLocation;

                    m_Queue = new Queue();
                    nearbyMobiles = map.GetMobilesInRange(targetLocation, 1);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double damage = (double)DamageMax;

                        if (mobile is BaseCreature)
                            damage *= 2;

                        int finalDamage = (int)(Math.Ceiling(damage));
                        double knockbackDamage = 25;

                        DoHarmful(mobile);

                        mobile.PlaySound(mobile.GetHurtSound());

                        SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, knockbackDamage, 2, -1, "", "You are knocked back by their charge!");

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                    }
                });
            });            
        }

        #endregion

        #region Harvest Blood

        public void HarvestBlood()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = (int)(Math.Round(4 + (8 * spawnPercent)));

            int bloodHarvested = 0;

            Queue m_Queue = new Queue();

            IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, range);
                        
            foreach (Item item in nearbyItems)
            {
                if (item is Blood)
                {
                    m_Queue.Enqueue(item);
                    bloodHarvested++;
                }
            }

            nearbyItems.Free();

            if (m_Queue.Count == 0)
            {
                m_NextAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                return;
            }

            PlaySound(0x20F);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*begins blood harvest*");
            
            double directionDelay = 1;
            double initialDelay = 2;           

            double totalDelay = directionDelay + initialDelay + directionDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
           
            Animate(12, 8, 3, true, false, 0);                    

            AbilityInProgress = true;

            m_NextHarvestBlood = DateTime.UtcNow + NextHarvestBloodDelay;
            m_NextAbilityAllowed = m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            
            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();

                int itemID = item.ItemID;
                int itemHue = 0;

                Point3D location = item.Location;

                item.Delete();

                int particleSpeed = 3;

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z), Map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 10), Map);
                
                Effects.PlaySound(location, Map, Utility.RandomList(0x5D9, 0x5DB));
                Effects.SendMovingParticles(startLocation, endLocation, itemID, particleSpeed, 0, false, false, itemHue, 0, 9501, 0, 0, 0x100);
            }

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: m_BloodPower = BloodPower.AttackSpeed; break;
                case 2: m_BloodPower = BloodPower.MartialSkill; break;
                case 3: m_BloodPower = BloodPower.SorcerousPower; break;
                case 4: m_BloodPower = BloodPower.ArcaneResistance; break;
            }
            
            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;
                
                int effectHue = 0;

                switch (m_BloodPower)
                {   
                    case BloodPower.AttackSpeed: effectHue = 2003; break;
                    case BloodPower.MartialSkill: effectHue = 2214; break;
                    case BloodPower.SorcerousPower: effectHue = 2620; break;
                    case BloodPower.ArcaneResistance: effectHue = 2593; break;
                }
                
                TimedStatic bloodGlow = new TimedStatic(14170, initialDelay * 2);
                bloodGlow.Name = "blood glow";
                bloodGlow.Hue = effectHue;
                bloodGlow.MoveToWorld(Location, Map);               
            });

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;

                ActiveSpeed = 0.25;
                PassiveSpeed = 0.25;
                CurrentSpeed = 0.25;

                SetSkill(SkillName.Wrestling, 110);

                SetSkill(SkillName.Magery, 0);
                SetSkill(SkillName.EvalInt, 0);
                SetSkill(SkillName.Meditation, 0);

                SetSkill(SkillName.MagicResist, 50);

                SetSubGroup(AISubgroup.None);
                UpdateAI(false);

                int maxBloodHarvestable = 30;
                int adjustedBloodHarvested = bloodHarvested;

                if (adjustedBloodHarvested > maxBloodHarvestable)
                    adjustedBloodHarvested = maxBloodHarvestable;

                double effectScalar = (double)adjustedBloodHarvested / 30;

                int effectHue = 0;

                switch (m_BloodPower)
                {
                    case BloodPower.AttackSpeed: effectHue = 2003; break;
                    case BloodPower.MartialSkill: effectHue = 2214; break;
                    case BloodPower.SorcerousPower: effectHue = 2620; break;
                    case BloodPower.ArcaneResistance: effectHue = 2593; break; 
                }

                int projectiles = 10;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), Map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(14170), particleSpeed, 0, false, false, effectHue - 1, 0);
                } 
                
                switch (m_BloodPower)
                {
                    case BloodPower.AttackSpeed:
                        PlaySound(0x590);

                        SpecialAbilities.FrenzySpecialAbility(1.0, this, null, effectScalar, 15, -1, true, "", "", "");

                        PublicOverheadMessage(MessageType.Regular, effectHue - 1, false, "*draws frenzy from the blood*");
                    break;

                    case BloodPower.MartialSkill:
                        PlaySound(0x51A);

                        double wrestlingSkill = 110 + (40 * effectScalar);

                        SetSkill(SkillName.Wrestling, wrestlingSkill);

                        PublicOverheadMessage(MessageType.Regular, effectHue - 1, false, "*draws martial prowess from the blood*");
                    break;

                    case BloodPower.SorcerousPower:
                        PlaySound(0x652);

                        int mageryLevel = (int)(Math.Ceiling((double)bloodHarvested / 10));

                        if (mageryLevel == 0)
                            mageryLevel = 1;

                        if (mageryLevel > 3)
                            mageryLevel = 3;

                        switch (mageryLevel)
                        {
                            case 1:
                                SetSkill(SkillName.Magery, 100);
                                SetSkill(SkillName.EvalInt, 100);
                                SetSkill(SkillName.Meditation, 300);                       

                                SetSubGroup(AISubgroup.Mage4);
                            break;

                            case 2:
                                SetSkill(SkillName.Magery, 125);
                                SetSkill(SkillName.EvalInt, 125);
                                SetSkill(SkillName.Meditation, 300);

                                SetSubGroup(AISubgroup.Mage5);
                            break;

                            case 3:
                                SetSkill(SkillName.Magery, 150);
                                SetSkill(SkillName.EvalInt, 150);
                                SetSkill(SkillName.Meditation, 300);                       

                                SetSubGroup(AISubgroup.Mage6);
                            break;
                        }                        
                        
                        UpdateAI(false);

                        DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                        DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                        DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                        DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                        DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                        DictCombatAction[CombatAction.CombatHealSelf] = 0;

                        DictWanderAction[WanderAction.None] = 0;
                        DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                        DictWanderAction[WanderAction.SpellCureSelf] = 0;

                        PublicOverheadMessage(MessageType.Regular, effectHue - 1, false, "*draws sorcerous power from the blood*");
                    break;

                    case BloodPower.ArcaneResistance:
                        PlaySound(0x592);

                        double magicResistSkill = 50 + (250 * effectScalar);

                        SetSkill(SkillName.MagicResist, magicResistSkill);

                        PublicOverheadMessage(MessageType.Regular, effectHue - 1, false, "*draws arcane resistance from the blood*");
                    break;
                }
            });
        }

        #endregion  
  
        #region Blood Spray

        public void BloodSpray()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = (int)(Math.Round(8 + (12 * spawnPercent)));

            List<Point3D> m_PossibleLocations = new List<Point3D>();

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;

                if (m_PossibleLocations.Contains(mobile.Location))
                    continue;

                m_PossibleLocations.Add(mobile.Location);
            }

            nearbyMobiles.Free();

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_PossibleLocations.Count == 0)
                return;

            Combatant = null;

            Point3D targetLocation = m_PossibleLocations[Utility.RandomMinMax(0, m_PossibleLocations.Count - 1)];
            Direction directionToTarget = Utility.GetDirection(Location, targetLocation);

            double directionDelay = .25;
            double initialDelay = 1;
            double postDelay = 1;

            double totalDelay = directionDelay + initialDelay + postDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");            

            AbilityInProgress = true;

            m_NextBloodSpray = DateTime.UtcNow + NextBloodSprayDelay;
            m_NextAbilityAllowed = m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            PlaySound(GetAngerSound());
            Direction = directionToTarget;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(27, 12, 1, true, false, 0);

                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*unleashes a spray of blood*");

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    ResolveBloodSpray(directionToTarget, range, DamageMin);
                    PlaySound(0x383); //0x580

                });
            });

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });            
        }

        public void ResolveBloodSpray(Direction direction, int range, int bleedAmount)
        {
            if (!SpecialAbilities.Exists(this))
                return;

            Dictionary<Point3D, double> m_BreathTiles = new Dictionary<Point3D, double>();

            int breathSound = 0;
            int effectSound = 0;

            double tileDelay = .10;
            int distance = range;

            Point3D startLocation = Location;
            Point3D previousPoint = Location;
            Point3D nextPoint;

            m_BreathTiles.Add(startLocation, 0);

            for (int a = 0; a < distance; a++)
            {
                nextPoint = GetPointByDirection(previousPoint, direction);

                bool canFit = SpellHelper.AdjustField(ref nextPoint, Map, 12, false);

                if (canFit && Map.InLOS(Location, nextPoint))
                {
                    if (!m_BreathTiles.ContainsKey(nextPoint))
                        m_BreathTiles.Add(nextPoint, a * tileDelay);
                }

                int width = (int)(Math.Floor(((double)a) / 3));

                if (width > 0)
                {
                    List<Point3D> perpendicularPoints = SpecialAbilities.GetPerpendicularPoints(previousPoint, nextPoint, width);

                    foreach (Point3D point in perpendicularPoints)
                    {
                        Point3D ppoint = new Point3D(point.X, point.Y, point.Z);

                        canFit = SpellHelper.AdjustField(ref ppoint, Map, 12, false);

                        if (canFit && Map.InLOS(Location, ppoint))
                        {
                            if (!m_BreathTiles.ContainsKey(ppoint))
                                m_BreathTiles.Add(ppoint, a * tileDelay);
                        }
                    }
                }

                previousPoint = nextPoint;
            }

            foreach (KeyValuePair<Point3D, double> pair in m_BreathTiles)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(pair.Value), delegate
                {
                    Point3D bloodLocation = pair.Key;

                    int projectiles;
                    int particleSpeed;
                    double distanceDelayInterval;

                    int minRadius;
                    int maxRadius;

                    List<Point3D> m_ValidLocations = new List<Point3D>();

                    if (bloodLocation != startLocation)
                    {
                        Effects.PlaySound(bloodLocation, Map, Utility.RandomList(0x5D8, 0x5DA));
                        new Blood(BloodDuration).MoveToWorld(bloodLocation, Map);

                        projectiles = 1;
                        particleSpeed = 8;
                        distanceDelayInterval = .12;

                        minRadius = 1;
                        maxRadius = 5;

                        m_ValidLocations = SpecialAbilities.GetSpawnableTiles(bloodLocation, true, false, bloodLocation, Map, projectiles, 20, minRadius, maxRadius, false);

                        if (m_ValidLocations.Count == 0)
                            return;

                        for (int a = 0; a < projectiles; a++)
                        {
                            Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(bloodLocation.X, bloodLocation.Y, bloodLocation.Z + 2), Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), Map);

                            newLocation.Z += 5;

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                        }

                        IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(bloodLocation, 0);

                        Queue m_Queue = new Queue();

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == this) continue;
                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                            m_Queue.Enqueue(mobile);
                        }

                        nearbyMobiles.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            double finalBleedAmount = (double)bleedAmount;

                            if (mobile is BaseCreature)
                                finalBleedAmount *= 2;

                            SpecialAbilities.BleedSpecialAbility(1.0, this, mobile, finalBleedAmount, 1.0, -1, true, "", "You are showered in blood!");
                        }
                    }
                });
            }
        }

        #endregion        

        //Epic Abilities

        #region SummonTheBlood

        public void SummonTheBlood()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 12;
            int summons = 2 + (int)(Math.Round(6 * spawnPercent));

            double totalDelay = 3;

            Combatant = null;

            List<Point3D> m_PossibleLocations = new List<Point3D>();

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;

                if (m_PossibleLocations.Contains(mobile.Location))
                    continue;

                bool tooClose = false;

                foreach (Point3D point in m_PossibleLocations)
                {
                    if (Utility.GetDistance(point, mobile.Location) <= 1)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                m_PossibleLocations.Add(mobile.Location);
            }

            nearbyMobiles.Free();

            List<Point3D> m_SummonLocations = new List<Point3D>();

            for (int a = 0; a < summons; a++)
            {
                if (m_PossibleLocations.Count > 0)
                {
                    int index = Utility.RandomMinMax(0, m_PossibleLocations.Count - 1);

                    m_SummonLocations.Add(m_PossibleLocations[index]);
                    m_PossibleLocations.RemoveAt(index);
                }
            }

            if (m_SummonLocations.Count < summons)
            {
                int locationsNeeded = summons - m_SummonLocations.Count;

                for (int a = 0; a < locationsNeeded; a++)
                {
                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, range, true);

                    if (m_ValidLocations.Count == 0)
                    {
                        if (!m_SummonLocations.Contains(Location))
                            m_SummonLocations.Add(Location);
                    }

                    else
                    {
                        Point3D randomLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        if (!m_SummonLocations.Contains(randomLocation))
                            m_SummonLocations.Add(randomLocation);

                        else
                        {
                            if (!m_SummonLocations.Contains(Location))
                                m_SummonLocations.Add(Location);
                        }
                    }
                }
            }

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Animate(23, 12, 2, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*blood begins to take form*");

            Point3D location = Location;
            Map map = Map;

            for (int a = 0; a < 3; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    for (int b = 0; b < 10; b++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(b * .1), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            foreach (Point3D point in m_SummonLocations)
                            {
                                Point3D bloodLocation = Utility.GetRandomLocation(point, 2, false);
                                bloodLocation.Z = Map.GetSurfaceZ(bloodLocation, 30);

                                new Blood(BloodDuration).MoveToWorld(bloodLocation, Map);
                            }
                        });
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                foreach (Point3D point in m_SummonLocations)
                {
                    Point3D summonLocation = point;
                    summonLocation.Z = Map.GetSurfaceZ(summonLocation, 30);

                    double creatureResult = Utility.RandomDouble();

                    double duration = 10;
                    int intervals = 20;
                    double intervalLength = .5;

                    int absorbRange = 2;

                    int itemId = 14217;
                    int itemHue = 38;

                    double bloodElementalChance = .10 + (.15 * spawnPercent);

                    if (spawnPercent < .5)
                        bloodElementalChance = 0;

                    if (creatureResult <= bloodElementalChance)
                        itemId = 14284;

                    TimedStatic rift = new TimedStatic(itemId, duration);
                    rift.Hue = itemHue;
                    rift.Name = "blood rift";
                    rift.MoveToWorld(point, Map);                   

                    for (int a = 0; a < intervals; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * intervalLength), delegate
                        {
                            if (!SpecialAbilities.Exists(this)) return;
                            if (rift == null) return;
                            if (rift.Deleted) return;

                            Queue m_Queue = new Queue();

                            nearbyMobiles = Map.GetMobilesInRange(rift.Location, absorbRange);

                            foreach (Mobile mobile in nearbyMobiles)
                            {
                                if (mobile == this) continue;
                                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                                m_Queue.Enqueue(mobile);
                            }

                            nearbyMobiles.Free();

                            if (m_Queue.Count > 0)
                                Effects.SendLocationParticles(EffectItem.Create(rift.Location, rift.Map, EffectItem.DefaultDuration), 14195, 20, 20, 38, 0, 0, 0);

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                double damage = (double)(Utility.RandomMinMax(5, 10));

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                int finalDamage = (int)(Math.Round(damage));

                                DoHarmful(mobile);

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z + 10), rift.Map);
                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(rift.Location.X, rift.Location.Y, rift.Location.Z + 10), rift.Map);

                                int bloodItemId = Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F);
                                int bloodHue = 0;
                                int bloodSpeed = 4;

                                mobile.PlaySound(0x354);
                                Effects.PlaySound(rift.Location, rift.Map, 0x354);

                                Effects.SendMovingParticles(startLocation, endLocation, bloodItemId, bloodSpeed, 0, false, false, bloodHue, 0, 9501, 0, 0, 0x100);
                                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration), 14202, 20, 20, 38, 0, 0, 0);

                                new Blood(BloodDuration).MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                            }
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(intervals * intervalLength), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        BaseCreature bc_Summon = null;

                        switch (itemId)
                        {
                            //Blood Spawn
                            case 14217:
                                switch (Utility.RandomMinMax(1, 2))
                                {
                                    case 1: bc_Summon = new BloodSlime(); break;
                                    case 2: bc_Summon = new BloodStalker(); break;
                                }

                                break;

                            //Blood Elemental
                            case 14284:
                                bc_Summon = new BloodElemental();
                                break;
                        }

                        if (bc_Summon == null)
                            return;

                        int projectiles = 6;
                        int particleSpeed = 8;
                        double distanceDelayInterval = .12;

                        int minRadius = 1;
                        int maxRadius = 5;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(summonLocation, true, false, summonLocation, Map, projectiles, 20, minRadius, maxRadius, false);

                        if (m_ValidLocations.Count == 0)
                            return;

                        for (int c = 0; c < projectiles; c++)
                        {
                            Point3D bloodLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(summonLocation.X, summonLocation.Y, summonLocation.Z + 2), Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(bloodLocation.X, bloodLocation.Y, bloodLocation.Z + 50), Map);

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                        }

                        bc_Summon.MoveToWorld(summonLocation, Map);
                        bc_Summon.PlaySound(bc_Summon.GetAngerSound());

                        m_Creatures.Add(bc_Summon);
                    });
                }

                AbilityInProgress = false;
            });
        }

        #endregion

        #region Blood Burst

        public void BloodBurst()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loops = 3 + (int)(Math.Round(5 * spawnPercent));

            double stationaryDelay = loops + 1;

            int radius = 3 + (int)(Math.Ceiling(6 * spawnPercent));

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
            });

            PlaySound(0x580);

            m_NextBloodBurstAllowed = DateTime.UtcNow + NextBloodBurstDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, stationaryDelay, true, 0, false, "", "");

            Animate(10, 8, 1, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*erupts in blood*");

            Point3D location = Location;
            Map map = Map;

            double directionDelay = 1.0;          

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 1, true, 0, false, "", "");

                        Animate(10, 8, 1, true, false, 0);

                        PlaySound(GetAngerSound());
                    });
                }

                List<Point3D> m_GustTargetLocations = new List<Point3D>();

                for (int a = 0; a < radius * 2; a++)
                {
                    m_GustTargetLocations.Add(new Point3D(Location.X - radius + a, Location.Y - radius, Location.Z));
                    m_GustTargetLocations.Add(new Point3D(Location.X + radius, Location.Y - radius + a, Location.Z));
                    m_GustTargetLocations.Add(new Point3D(Location.X + radius - a, Location.Y + radius, Location.Z));
                    m_GustTargetLocations.Add(new Point3D(Location.X - radius, Location.Y + radius - a, Location.Z));
                }

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        foreach (Point3D gustTargetLocation in m_GustTargetLocations)
                        {
                            if (Utility.RandomDouble() <= .1)
                                continue;

                            int variation = Utility.RandomMinMax(1, 5);

                            Timer.DelayCall(TimeSpan.FromSeconds(.01 * (double)variation), delegate
                            {
                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), map);
                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(gustTargetLocation.X, gustTargetLocation.Y, gustTargetLocation.Z + 10), map);

                                int particleSpeed = 8;

                                int itemID = Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F);
                                int itemHue = 0;

                                Effects.SendMovingEffect(startLocation, endLocation, itemID, 5, 0, false, false, itemHue, 0);

                                Effects.PlaySound(startLocation, map, 0x4F1);
                                Effects.PlaySound(gustTargetLocation, map, 0x4F1);
                            });
                        }

                        int adjustedRadius = radius * 2;

                        int bloodItems = adjustedRadius * 5;

                        for (int b = 0; b < bloodItems; b++)
                        {
                            Point3D bloodLocation = Utility.GetRandomLocation(Location, adjustedRadius, false);
                            bloodLocation.Z = Map.GetSurfaceZ(bloodLocation, 30);

                            double bloodDistanceDelay = Utility.GetDistance(Location, bloodLocation) * .03;

                            Timer.DelayCall(TimeSpan.FromSeconds(bloodDistanceDelay), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                new Blood(BloodDuration).MoveToWorld(bloodLocation, Map);
                            });
                        }

                        IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, adjustedRadius);

                        Queue m_Queue = new Queue();

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == this) continue;
                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                            m_Queue.Enqueue(mobile);
                        }

                        nearbyMobiles.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            int impactSound = 0x56D;
                            int impactItemId = 14276; //14122;
                            int impactItemHue = 38;

                            Effects.PlaySound(mobile.Location, mobile.Map, impactSound);
                            Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactItemHue - 1, 0, 0, 0);

                            int minDamage = (int)(Math.Round((double)DamageMin / 2));
                            int maxDamage = DamageMin;

                            int damage = Utility.RandomMinMax(minDamage, maxDamage);

                            if (mobile is BaseCreature)
                                damage *= 2;

                            DoHarmful(mobile);

                            for (int b = 0; b < 2; b++)
                            {
                                new Blood(BloodDuration).MoveToWorld(Utility.GetRandomLocation(mobile.Location, 1, false), mobile.Map);
                            }

                            new Blood(BloodDuration).MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, this, damage, 0, 100, 0, 0, 0);
                        }
                    });
                }
            });
        }

        #endregion

        #region Blood Storm

        public void BloodStorm()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_LightningTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;
            
            Combatant = null;

            int range = (int)(Math.Round(10 + (10 * spawnPercent)));

            int bloodRain = 50 + (int)(Math.Ceiling(150 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)bloodRain / 10));
            int lightningStrikes = loops * 3;

            double initialDelay = 1;
            double lightningStrikeDelay = .15;
            
            double totalDelay = initialDelay + (lightningStrikes * lightningStrikeDelay) + 1;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });
            
            if (!SpecialAbilities.Exists(this))
                return;

            Animate(27, 7, loops * 2, true, false, 0);
            PlaySound(0x5CE);

            PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*creates a bloodstorm*");

            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                for (int a = 0; a < lightningStrikes; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * lightningStrikeDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        List<Mobile> m_ValidMobiles = new List<Mobile>();

                        IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(Location, range);

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == this) continue;
                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                            if (mobile.Hidden) continue;

                            m_ValidMobiles.Add(mobile);
                        }

                        nearbyMobiles.Free();

                        if (m_ValidMobiles.Count == 0)
                            return;

                        Mobile mobileTarget = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];

                        mobileTarget.BoltEffect(0);
                        mobileTarget.PlaySound(0x29);

                        double damage = (double)(Utility.RandomMinMax(10, 20));

                        if (mobileTarget is BaseCreature)
                            damage *= 2;

                        if (m_LightningTargets.Contains(mobileTarget))
                            damage *= .5;

                        else
                            m_LightningTargets.Add(mobileTarget);

                        DoHarmful(mobileTarget);

                        new Blood(BloodDuration).MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, this, (int)damage, 0, 100, 0, 0, 0);
                    });
                }

                for (int a = 0; a < bloodRain; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .03), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Point3D meteorLocation = Utility.GetRandomLocation(Location, range, true);

                        int itemID = Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F);
                        int itemHue = 0;

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(meteorLocation.X - 1, meteorLocation.Y - 1, meteorLocation.Z + 100), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(meteorLocation.X, meteorLocation.Y, meteorLocation.Z + 5), map);

                        int particleSpeed = 16;

                        Effects.SendMovingParticles(startLocation, endLocation, itemID, particleSpeed, 0, false, false, itemHue, 0, 9501, 0, 0, 0x100);

                        double impactDelay = .375;

                        Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            Effects.PlaySound(meteorLocation, map, 0x028);

                            Blood blood = new Blood(BloodDuration);
                            blood.ItemID = itemID;
                            blood.MoveToWorld(meteorLocation, Map);
                        });
                    });
                }

            });            
        }

        #endregion        

        #region Ignite The Blood

        public void IgniteTheBlood(bool ignite)
        {
            if (!SpecialAbilities.Exists(this))
                return;
            
            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            
            Combatant = null;

            int range = (int)(Math.Round(8 + (12 * spawnPercent)));

            double initialDelay = 1;
            double stationaryDelay = 2;

            double totalDelay = initialDelay + stationaryDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            Animate(27, 7, 1, true, false, 0);
            PlaySound(0x5CE);

            if (ignite)
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*prepares to ignite the blood*");

            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Effects.PlaySound(Location, Map, 0x656);                

                int minRange = -1 * range;
                int maxRange = range + 1;

                for (int a = minRange; a < maxRange; a++)
                {
                    for (int b = minRange; b < maxRange; b++)
                    {
                        Point3D location = Location;
                        Map map = Map;

                        Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);
                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        if (!map.InLOS(location, newLocation))
                            continue;

                        double distance = Utility.GetDistanceToSqrt(location, newLocation);
                        double distanceDelay = (distance * .05);

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            int projectiles = 2;
                            int particleSpeed = 8;
                            double distanceDelayInterval = .12;

                            int minRadius = 1;
                            int maxRadius = 5;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(newLocation, true, false, newLocation, Map, projectiles, 20, minRadius, maxRadius, false);

                            if (m_ValidLocations.Count == 0)
                                return;

                            for (int c = 0; c < projectiles; c++)
                            {
                                Point3D bloodLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 2), Map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(bloodLocation.X, bloodLocation.Y, bloodLocation.Z + 50), Map);
                                
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                            }

                            new Blood(BloodDuration).MoveToWorld(newLocation, map);
                            Effects.PlaySound(newLocation, map, 0x027);
                        });
                    }
                }
                
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    if (!ignite)
                        return;

                    int seconds = 5;
                    int intervals = 20;
                    double intervalDelay = .05;

                    int explosions = 1;

                    for (int a = 0; a < seconds; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            for (int b = 0; b < intervals; b++)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(b * intervalDelay), delegate
                                {
                                    if (!SpecialAbilities.Exists(this))
                                        return;

                                    List<Blood> m_Blood = new List<Blood>();

                                    Queue m_Queue = new Queue();
                                
                                    IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, range);

                                    foreach (Item item in nearbyItems)
                                    {
                                        if (item is Blood)
                                        {
                                            Blood blood = item as Blood;
                                            m_Blood.Add(blood);                                                                       
                                        }
                                    }

                                    nearbyItems.Free();

                                    for (int c = 0; c < explosions; c++)
                                    {
                                        if (m_Blood.Count > 0)
                                        {
                                            int index = Utility.RandomMinMax(0, m_Blood.Count - 1);

                                            m_Queue.Enqueue(m_Blood[index]);
                                            m_Blood.RemoveAt(index);
                                        }
                                    }

                                    if (m_Queue.Count > 0)
                                    {
                                        Blood blood = (Blood)m_Queue.Dequeue();

                                        Point3D bloodLocation = blood.Location;
                                        blood.Delete();

                                        int impactSound = 0x591;
                                        int impactItemId = 14616; //14122;
                                        int impactItemHue = 38;

                                        Effects.PlaySound(bloodLocation, Map, impactSound);
                                        Effects.SendLocationParticles(EffectItem.Create(bloodLocation, Map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactItemHue - 1, 0, 0, 0);
                                    
                                        Queue m_BloodQueue = new Queue();

                                        IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(bloodLocation, 1);

                                        foreach (Mobile mobile in nearbyMobiles)
                                        {
                                            if (mobile == this) continue;
                                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                                            m_BloodQueue.Enqueue(mobile);
                                        }

                                        nearbyMobiles.Free();

                                        while (m_BloodQueue.Count > 0)
                                        {
                                            Mobile mobile = (Mobile)m_BloodQueue.Dequeue();

                                            double damage = DamageMin;

                                            if (mobile is BaseCreature)
                                                damage *= 2;

                                            int finalDamage = (int)(Math.Ceiling(damage));

                                            DoHarmful(mobile);

                                            new Blood(BloodDuration).MoveToWorld(mobile.Location, mobile.Map);
                                            AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                                        } 
                                    }
                                });
                            }
                        });
                    }
                });
            });
        }

        #endregion

        public override void OnThink()
        {
            base.OnThink();                        

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (m_BossPhase == BossPhase.PikeMounted)
                CheckChargeResolved();

            else
            {
                m_IsCharging = false;
                m_ChargeTarget = null;
            }

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (Combatant != null && !m_IsCharging && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (m_BossPhase)
                {
                    case BossPhase.PikeMounted:
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1:
                                if (DateTime.UtcNow >= m_NextConfigureTraps)
                                {
                                    ConfigureTraps();
                                    return;
                                }
                            break;

                            case 2:
                                if (DateTime.UtcNow >= m_NextChargeAllowed)
                                {
                                    BeginCharge();
                                    return;
                                }
                            break;                            
                        }
                    break;

                    case BossPhase.SwordShield:
                        switch (Utility.RandomMinMax(1, 1))
                        {
                            case 1:
                                if (DateTime.UtcNow >= m_NextConfigureTraps)
                                {
                                    ConfigureTraps();
                                    return;
                                }
                            break;
                        }
                    break;

                    case BossPhase.Axe:
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1:
                                if (DateTime.UtcNow >= m_NextConfigureTraps)
                                {
                                    ConfigureTraps();
                                    return;
                                }
                            break;

                            case 2:
                                if (DateTime.UtcNow >= m_NextAxeThrowAllowed)
                                {
                                    AxeThrow();
                                    return;
                                }
                            break;
                        }
                    break;

                    case BossPhase.Sanguineous:
                        if (DateTime.UtcNow >= m_NextHarvestBlood)
                        {
                            HarvestBlood();
                            return;
                        }

                        else
                        {
                            switch (Utility.RandomMinMax(1, 3))
                            {
                                case 1:
                                    if (DateTime.UtcNow >= m_NextBloodSpray)
                                    {
                                        BloodSpray();
                                        return;
                                    }
                                break;

                                case 2:
                                    if (DateTime.UtcNow >= m_NextConfigureTraps)
                                    {
                                        ConfigureTraps();
                                        return;
                                    }
                                break;

                                case 3:
                                    if (DateTime.UtcNow >= m_NextSanguineousChargeAllowed)
                                    {
                                        SanguineousCharge();
                                        return;
                                    }
                                break;
                            }
                        }
                        
                    break;

                break;
                }
            }

            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*fumes*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                    PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*rages*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                    PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*seethes*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                    PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*hisses*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                    PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*roars*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }
        }

        protected override bool OnMove(Direction d)
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            switch (m_BossPhase)
            {
                case BossPhase.PikeMounted:
                    Effects.PlaySound(Location, Map, Utility.RandomList(0x12E, 0x12D));

                    if (m_IsCharging)                    
                        CheckTrample();                    

                    CheckChargeResolved();
                break;

                case BossPhase.SwordShield:
                break;

                case BossPhase.Axe:
                break;

                case BossPhase.Sanguineous:
                    Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));
                break;
            }            

            return base.OnMove(d);
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            //BossPersistance.PersistanceItem.DestardBossLastStatusChange = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if (mount != null)
            {
                mount.Rider = null;

                if (mount is Mobile)
                    ((Mobile)mount).Delete();
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            //if (Utility.RandomMinMax(1, 10) == 1)
                //c.AddItem(new LythTheDestroyerStatue());

            //if (Utility.RandomMinMax(1, 20) == 1)
                //c.AddItem(new DestroyersSkull());

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }

            for (int a = 0; a < m_Traps.Count; ++a)
            {
                if (m_Traps[a] != null)
                    m_Traps[a].Delete();
            }

            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)
                    m_Items[a].Delete();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            
            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }

            for (int a = 0; a < m_Traps.Count; ++a)
            {
                if (m_Traps[a] != null)
                    m_Traps[a].Delete();
            }

            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)                
                    m_Items[a].Delete();                
            }
        }
               
        public override int GetAngerSound() 
        { 
            if (m_BossPhase == BossPhase.PikeMounted)
                return 0x2A9;

            if (m_BossPhase == BossPhase.Sanguineous)
                return 0x289;

            return 0x572;
        }

        public override int GetIdleSound()
        {
            if (m_BossPhase == BossPhase.PikeMounted)
                return 0x598;

            if (m_BossPhase == BossPhase.Sanguineous)
                return 0x2C4;

            return 0x572;
        }

        public override int GetAttackSound()
        {
            if (m_BossPhase == BossPhase.Sanguineous)
                return 0x28B;

            return -1; 
        }

        public override int GetHurtSound() 
        {
            if (m_BossPhase == BossPhase.PikeMounted)
                return 0x5FA;

            if (m_BossPhase == BossPhase.Sanguineous)
                return 0x28C;
            
            return 0x5FA;
        }

        public override int GetDeathSound() 
        {
            if (m_BossPhase == BossPhase.Sanguineous)
                return 0x2C1;

            return 0x2AB;
        } 

        public Sanguineous(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(damageIntervalThreshold);
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(totalIntervals);            

            //Version 0
            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }

            writer.Write(m_Traps.Count);
            for (int a = 0; a < m_Traps.Count; a++)
            {
                writer.Write(m_Traps[a]);
            }

            writer.Write(m_Items.Count);
            for (int a = 0; a < m_Items.Count; a++)
            {
                writer.Write(m_Items[a]);
            }

            writer.Write((int)m_BossPhase);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    m_Creatures.Add(creature);
                }

                int trapCount = reader.ReadInt();
                for (int a = 0; a < trapCount; a++)
                {
                    SanguineousTrap trap = (SanguineousTrap)reader.ReadItem();

                    m_Traps.Add(trap);
                }

                int itemCount = reader.ReadInt();
                for (int a = 0; a < itemCount; a++)
                {
                    Item item = reader.ReadItem();
                    
                    m_Items.Add(item);
                }

                m_BossPhase = (BossPhase)reader.ReadInt();
            }

            //------------

            Blessed = false;
            Hidden = false;
        }
    }
}






