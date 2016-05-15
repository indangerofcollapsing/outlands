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
    [CorpseName("the gatekeeper's corpse")]
    public class UOACZTheGatekeeper : UOACZBaseUndead
    {
        public override string[] idleSpeech
        {
            get
            {
                return new string[] {       "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };
            }
        }

        public override string[] combatSpeech
        {
            get
            {
                return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };
            }
        }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextRevealAllowed;
        public TimeSpan NextRevealDelay = TimeSpan.FromSeconds(5);

        public DateTime m_NextRaiseDeadAllowed;
        public TimeSpan NextRaiseDeadDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(5);
        
        public int damageIntervalThreshold = 500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 30;

        public int m_OrbMainHue = 0;
        public int m_OrbRevealHue = 2515;
        public int m_OrbAttackHue = 2608;
        public int m_OrbIntervalAttackHue = 2608;

        public int RaiseDeadRange = 12;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public override int DifficultyValue { get { return 11; } }

        public List<Mobile> m_Creatures = new List<Mobile>();
        public Item m_Item;

        [Constructable]
        public UOACZTheGatekeeper(): base()
        {
            Name = "The Gatekeeper";

            Hue = 2587;

            Body = 830;
            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(15000);
            SetStam(5000);
            SetMana(30000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 300);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 50;

            Fame = 20000;
            Karma = -20000;

            Timer.DelayCall(TimeSpan.FromMilliseconds(50), delegate { BuildSkull(); });
        }

        public void BuildSkull()
        {
            Point3D skullLocation = new Point3D(Location.X, Location.Y, Z + 35);

            Static item = new Static(7960);

            m_Item = item;
            m_Item.Hue = m_OrbMainHue;
            m_Item.Name = "Orb of Sight";

            item.MoveToWorld(skullLocation, Map);
        }

        public override void SetUniqueAI()
        {
            AISubGroup = AISubGroupType.Mage5;
            UpdateAI(false);

            base.SetUniqueAI();

            DictCombatTargeting[CombatTargeting.UOACZIgnoreHumanSentry] = 0;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;
            DictWanderAction[WanderAction.SpellHealOther100] = 0;
            DictWanderAction[WanderAction.SpellCureOther] = 0;

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.5;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            SpellHue = 2586;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(Undead Boss)", from.NetState);

            base.OnSingleClick(from);
        }

        public override bool AlwaysBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (willKill)
                UOACZEvents.UndeadBossDamaged(true);

            else
                UOACZEvents.UndeadBossDamaged(false);

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;                   

                    if (intervalCount % 4 == 0)
                        BreathOfTheDead();

                    else
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: ShockBarrage(); break;
                            case 2: ForbiddenLore(); break;
                            case 3: Doom(); break;
                        }
                    }
                }

                if (from != null && amount > 10 && !AbilityInProgress && !DamageIntervalInProgress)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < .1)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .05)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .15)
                            DamageEffect(from);
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void DamageEffect(Mobile from)
        {
            if (AbilityInProgress || DamageIntervalInProgress)
                return;

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: Shock(from); break;
                case 2: Blink(from); break;
                case 3: Hex(from); break;
            }
        }

        public void Shock(Mobile from)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;
            if (Utility.GetDistance(Location, from.Location) > 30) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = m_Item.Location;
            Map map = m_Item.Map;

            int bolts = 1 + (int)(Math.Ceiling(5 * spawnPercent));
            double boltDuration = .25;

            int boltMinDamage = 5;
            int boltMaxDamage = 15;

            Animate(15, 8, 1, true, false, 0); //Staff

            for (int a = 0; a < bolts; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * boltDuration), delegate
                {
                    if (!SpecialAbilities.Exists(this)) return;
                    if (!SpecialAbilities.Exists(from)) return;
                    if (m_Item == null) return;
                    if (m_Item.Deleted) return;
                    if (Utility.GetDistance(location, from.Location) > 30) return;

                    m_Item.Hue = m_OrbAttackHue;

                    Effects.PlaySound(m_Item.Location, m_Item.Map, 0x5C3);

                    TimedStatic discharge = new TimedStatic(0x3779, .5);
                    discharge.Hue = m_OrbAttackHue;
                    discharge.Name = "dissipated energy";
                    discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(m_Item.X, m_Item.Y, m_Item.Z), m_Item.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 5), from.Map);

                    int particleSpeed = 5;

                    Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                    double distance = Utility.GetDistanceToSqrt(location, from.Location);
                    double distanceDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (!SpecialAbilities.Exists(from)) return;
                        if (m_Item == null) return;
                        if (m_Item.Deleted) return;
                        if (Utility.GetDistance(location, from.Location) > 30) return;

                        int damage = Utility.RandomMinMax(boltMinDamage, boltMaxDamage);
                        double duration = Utility.RandomMinMax(2, 4);

                        if (from is BaseCreature)
                        {
                            damage *= 2;
                            duration *= 2;
                        }

                        from.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                        SpecialAbilities.HinderSpecialAbility(1.0, null, from, 1.0, duration, false, -1, false, "", "You have been shocked!", "-1");

                        new Blood().MoveToWorld(from.Location, from.Map);
                        AOS.Damage(from, damage, 0, 100, 0, 0, 0);

                    });
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(bolts * boltDuration), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (m_Item == null) return;
                if (m_Item.Deleted) return;

                m_Item.Hue = m_OrbMainHue;
            });
        }

        public void Blink(Mobile from)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            Point3D location = Location;
            Map map = Map;

            int effectHue = 2613;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Animate(15, 8, 1, true, false, 0); //Staff

            Combatant = null;

            if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null))
                PublicOverheadMessage(MessageType.Regular, 0, false, "*blinks*");

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                if (Hidden)
                    RevealingAction();

                int maxExtraArmor = 50;
                int baseArmor = 50;

                MagicDamageAbsorb = 1;

                FixedParticles(0x375A, 10, 30, 5037, effectHue, 0, EffectLayer.Waist);
                PlaySound(0x659);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    PublicOverheadMessage(MessageType.Regular, 0, false, "*shielded*");

                    MeleeDamageAbsorb = baseArmor + (int)(Math.Ceiling((double)maxExtraArmor * spawnPercent));
                    FixedParticles(0x376A, 9, 64, 5008, effectHue, 0, EffectLayer.Waist);
                });
            });
        }

        public void Hex(Mobile from)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            Animate(15, 8, 1, true, false, 0); //Staff

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.2)), 0x375A, 6, 20, 2073, 0, 5029, 0);
            Effects.PlaySound(from.Location, from.Map, 0x457);

            SpecialAbilities.BacklashSpecialAbility(1.0, this, from, .33, 60, -1, true, "", "You have been struck with an evil hex, slowing your attacks and causing havoc with your spellcasting!");
            SpecialAbilities.CrippleSpecialAbility(1.0, this, from, .25, 60, -1, true, "", "", "-1");
        }

        public void RaiseDead()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            List<Mobile> m_Mobiles = new List<Mobile>();

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, RaiseDeadRange);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;
                if (mobile is UOACZBaseUndead) continue;
                if (mobile is PlayerMobile)
                {
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player.IsUOACZUndead)
                        continue;
                }

                m_Mobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            if (m_Mobiles.Count == 0)
                return;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*raises dead*");

            Mobile mobileTarget = m_Mobiles[Utility.RandomMinMax(0, m_Mobiles.Count - 1)];

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = 1.5;
            double totalDelay = 2 + directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextRaiseDeadAllowed = DateTime.UtcNow + NextRaiseDeadDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            int effectHue = 1153;

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;
                
                Animate(15, 8, 1, true, false, 0); //Staff
                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this)) return;
                    if (!SpecialAbilities.Exists(this)) return;

                    Effects.PlaySound(Location, Map, 0x64C);

                    Direction direction = Utility.GetDirection(location, mobileTarget.Location);
                    int windId = UOACZSystem.GetWindItemId(direction, true);

                    MovingEffect(mobileTarget, windId, 5, 1, false, false, effectHue, 0);

                    double distance = Utility.GetDistanceToSqrt(location, mobileTarget.Location);
                    double destinationDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (!SpecialAbilities.Exists(mobileTarget)) return;

                        int damage = DamageMin;

                        if (mobileTarget is BaseCreature)
                            damage = (int)((double)damage * 2);

                        int knockbackDistance = 4 + (int)(Math.Ceiling(8 * spawnPercent));

                        SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobileTarget, damage, knockbackDistance, -1, "", "You are knocked back!");

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            int particleSpeed = 5;

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 3), Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 3), targetMap);

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, effectHue, 0);

                            double newDistance = Utility.GetDistanceToSqrt(Location, targetLocation);
                            double newDestinationDelay = (double)newDistance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(newDestinationDelay), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                Effects.PlaySound(targetLocation, targetMap, 0x653);

                                Effects.SendLocationParticles(effectEndLocation, 0x3709, 10, 20, effectHue, 0, 5029, 0);
                                Effects.SendLocationParticles(effectEndLocation, 0x3779, 10, 60, effectHue, 0, 5029, 0);
                                Effects.SendLocationParticles(effectEndLocation, 0x3996, 10, 60, effectHue, 0, 5029, 0);

                                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                {
                                    if (!SpecialAbilities.Exists(this))
                                        return;

                                    BaseCreature monsterToSpawn = null;

                                    int maxCreatureValue = 1 + (int)(Math.Ceiling(10 * spawnPercent));

                                    if (maxCreatureValue > 9)
                                        maxCreatureValue = 9;

                                    switch (Utility.RandomMinMax(1, maxCreatureValue))
                                    {
                                        case 1: monsterToSpawn = new UOACZSkeleton(); break;
                                        case 2: monsterToSpawn = new UOACZZombie(); break;
                                        case 3: monsterToSpawn = new UOACZGhoul(); break;
                                        case 4: monsterToSpawn = new UOACZSkeletalCritter(); break;
                                        case 5: monsterToSpawn = new UOACZPatchworkSkeleton(); break;
                                        case 6: monsterToSpawn = new UOACZZombieMagi(); break;
                                        case 7: monsterToSpawn = new UOACZSkeletalMage(); break;
                                        case 8: monsterToSpawn = new UOACZNecromancer(); break;
                                        case 9: monsterToSpawn = new UOACZSkeletalKnight(); break;
                                    }

                                    if (monsterToSpawn != null)
                                    {
                                        monsterToSpawn.MoveToWorld(targetLocation, targetMap);
                                        m_Creatures.Add(monsterToSpawn);
                                    }
                                });
                            });
                        });
                    });
                });
            });
        }        

        public void BreathOfTheDead()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(.5);

            int summonMotions = 5 - (int)(Math.Ceiling(4 * spawnPercent));
            double summonDuration = 1;

            int creaturesToSummon = 1 + (int)(Math.Ceiling(3 * spawnPercent));

            double stationaryDelay = summonMotions * summonDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int maxExtraRange = 10;
            int range = 10 + (int)(Math.Ceiling((double)maxExtraRange * spawnPercent));
            int effectHue = 1153;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon the breath of the living*");

            for (int a = 0; a < summonMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * summonDuration), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);

                    Queue m_Queue = new Queue();

                    IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (mobile is UOACZBaseUndead) continue;
                        if (mobile is PlayerMobile)
                        {
                            PlayerMobile player = mobile as PlayerMobile;

                            if (player.IsUOACZUndead)
                                continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    mobilesInRange.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        mobile.MovingEffect(this, 0x3728, 5, 1, true, false, effectHue, 0);
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)summonMotions * summonDuration), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                PlaySound(0x64F);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                    m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    int distance = range - (int)GetDistanceToSqrt(mobile);

                    double damage = DamageMax;

                    if (mobile is BaseCreature)
                        damage *= 2;

                    Direction direction = Utility.GetDirection(location, mobileLocation);
                    int windId = UOACZSystem.GetWindItemId(direction, false);
                    MovingEffect(mobile, windId, 5, 1, false, false, effectHue, 0);

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobile, damage, distance, -1, "", "You are knocked back by the breath of the dead!");
                }

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    AbilityInProgress = false;
                    DamageIntervalInProgress = false;

                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

                    for (int a = 0; a < creaturesToSummon; a++)
                    {
                        Point3D spawnLocation = Location;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 10, true);

                        if (m_ValidLocations.Count > 0)
                            spawnLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        int particleSpeed = 5;

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 3), Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(spawnLocation.X, spawnLocation.Y, spawnLocation.Z + 3), map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 0, 0);

                        double distance = Utility.GetDistanceToSqrt(Location, spawnLocation);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            Effects.PlaySound(spawnLocation, map, 0x653);

                            Effects.SendLocationParticles(effectEndLocation, 0x3709, 10, 20, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3779, 10, 60, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3996, 10, 60, effectHue, 0, 5029, 0);

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                BaseCreature monsterToSpawn = null;

                                int maxCreatureValue = 1 + (int)(Math.Ceiling(10 * spawnPercent));

                                if (maxCreatureValue > 9)
                                    maxCreatureValue = 9;

                                switch (Utility.RandomMinMax(1, maxCreatureValue))
                                {
                                    case 1: monsterToSpawn = new UOACZSkeleton(); break;
                                    case 2: monsterToSpawn = new UOACZZombie(); break;
                                    case 3: monsterToSpawn = new UOACZGhoul(); break;
                                    case 4: monsterToSpawn = new UOACZSkeletalCritter(); break;
                                    case 5: monsterToSpawn = new UOACZPatchworkSkeleton(); break;
                                    case 6: monsterToSpawn = new UOACZZombieMagi(); break;
                                    case 7: monsterToSpawn = new UOACZSkeletalMage(); break;
                                    case 8: monsterToSpawn = new UOACZNecromancer(); break;
                                    case 9: monsterToSpawn = new UOACZSkeletalKnight(); break;
                                }

                                if (monsterToSpawn != null)
                                {
                                    monsterToSpawn.MoveToWorld(spawnLocation, map);
                                    m_Creatures.Add(monsterToSpawn);
                                }
                            });
                        });
                    }
                });
            });
        }

        public void ShockBarrage()
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(.5);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 24;
            int cycles = 10 + (int)(Math.Ceiling(20 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)cycles / 10));

            double stationaryDelay = 1 + loops;

            int boltMinDamage = 5;
            int boltMaxDamage = 10;

            m_Item.Hue = m_OrbIntervalAttackHue;

            Combatant = null;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*discharges giant surge of energy*");

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            Point3D location = Location;
            Map map = Map;

            for (int a = 0; a < loops; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!SpecialAbilities.Exists(this)) return;
                    if (m_Item == null) return;
                    if (m_Item.Deleted) return;

                    Animate(12, 12, 1, true, false, 0);

                    PlaySound(GetAngerSound());
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (m_Item == null) return;
                if (m_Item.Deleted) return;

                m_Item.Hue = m_OrbMainHue;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

                PlaySound(0x211);
            });

            for (int a = 0; a < cycles; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .06), delegate
                {
                    if (!SpecialAbilities.Exists(this)) return;
                    if (m_Item == null) return;
                    if (m_Item.Deleted) return;

                    List<Mobile> m_ValidMobiles = new List<Mobile>();

                    IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (mobile is UOACZBaseUndead) continue;
                        if (mobile is PlayerMobile)
                        {
                            PlayerMobile player = mobile as PlayerMobile;

                            if (player.IsUOACZUndead)
                                continue;
                        }

                        m_ValidMobiles.Add(mobile);
                    }

                    mobilesInRange.Free();

                    if (m_ValidMobiles.Count > 0)
                    {
                        Mobile target = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];

                        m_Item.Hue = m_OrbIntervalAttackHue;

                        Effects.PlaySound(m_Item.Location, m_Item.Map, 0x211);

                        TimedStatic discharge = new TimedStatic(0x3779, .5);
                        discharge.Hue = m_OrbIntervalAttackHue;
                        discharge.Name = "dissipated energy";
                        discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(m_Item.X, m_Item.Y, m_Item.Z), m_Item.Map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 5), target.Map);

                        int particleSpeed = 5;

                        Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                        double distance = Utility.GetDistanceToSqrt(location, target.Location);
                        double distanceDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this)) return;
                            if (!SpecialAbilities.Exists(target)) return;
                            if (m_Item == null) return;
                            if (m_Item.Deleted) return;
                            if (Utility.GetDistance(location, target.Location) > 30) return;

                            int damage = Utility.RandomMinMax(boltMinDamage, boltMaxDamage);
                            double duration = Utility.RandomMinMax(2, 4);

                            if (target is BaseCreature)
                            {
                                damage *= 3;
                                duration *= 2;
                            }

                            target.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                            SpecialAbilities.HinderSpecialAbility(1.0, null, target, 1.0, duration, false, -1, false, "", "You have been shocked!", "-1");

                            new Blood().MoveToWorld(target.Location, target.Map);
                            AOS.Damage(target, damage, 0, 100, 0, 0, 0);
                        });
                    }
                });
            }
        }

        public void ForbiddenLore()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            Point3D location = Location;
            Map map = Map;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon forbidden lore*");

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loreStatues = 1 + (int)(Math.Ceiling(3 * spawnPercent));

            int effectHue = Hue - 1;

            int loops = 3;
            double stationaryDelay = 3;

            AbilityInProgress = true;
            DamageIntervalInProgress = true;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            for (int a = 0; a < loops; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    PlaySound(GetAngerSound());
                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
            });

            for (int a = 0; a < 4; a++)
            {
                double duration = 22 - (a * .5);

                Timer.DelayCall(TimeSpan.FromSeconds(a * .5), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(location, map, 0x1EC);

                    for (int b = 0; b < loreStatues; b++)
                    {
                        Point3D runeLocation;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, false, false, Location, Map, 1, 10, 1, 18, true);

                        if (m_ValidLocations.Count > 0)
                            runeLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        else
                            continue;

                        TimedStatic forbiddenRunes = new TimedStatic(Utility.RandomList(3676, 3679, 3682, 3685, 3688), duration);
                        forbiddenRunes.Name = "forbidden rune";
                        SpellHelper.AdjustField(ref runeLocation, map, 12, false);

                        forbiddenRunes.MoveToWorld(runeLocation, map);

                        TimedStatic timedStatic = new TimedStatic(0x3779, .5);
                        timedStatic.Hue = effectHue;
                        timedStatic.Name = "rune energy";
                        timedStatic.MoveToWorld(runeLocation, map);
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                for (int a = 0; a < loreStatues; a++)
                {
                    for (int b = 0; b < 3; b++)
                    {
                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(location, false, false, location, map, 1, 10, 1, 15, true);

                        if (m_ValidLocations.Count > 0)
                            location = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        bool foundStatue = false;

                        IPooledEnumerable itemsNearby = map.GetItemsInRange(location, 2);
                        {
                            foreach (Item item in itemsNearby)
                            {
                                if (item is ForbiddenLoreStatue)
                                {
                                    foundStatue = true;
                                    break;
                                }
                            }
                        }

                        itemsNearby.Free();

                        if (foundStatue || location == Location)
                            continue;

                        int statueAttackInterval = Utility.RandomMinMax(4, 6);
                        int durationCount = (int)(Math.Ceiling(30 / (double)statueAttackInterval));

                        ForbiddenLoreStatue statue = new ForbiddenLoreStatue(10, DamageMin, DamageMin, statueAttackInterval, durationCount);
                        statue.MoveToWorld(location, map);

                        break;
                    }
                }
            });
        }

        public void Doom()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*draws energy from the abyss*");
            PlaySound(0x456);

            AbilityInProgress = true;
            DamageIntervalInProgress = true;

            int castingMotions = 3;
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 18;
            int effectHue = Hue - 1;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "Behold mortals, your doom.");
                PlaySound(0x246);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (mobile is UOACZBaseUndead) continue;
                    if (mobile is PlayerMobile)
                    {
                        PlayerMobile player = mobile as PlayerMobile;

                        if (player.IsUOACZUndead)
                            continue;
                    }

                    m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    Effects.PlaySound(mobileLocation, map, 0x246);

                    int maxExtraProjectiles = 8;
                    int projectiles = 4 + (int)(Math.Ceiling((double)maxExtraProjectiles * spawnPercent));
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(mobile.Location, true, false, mobile.Location, mobile.Map, 1, 15, 5, 10, true);

                        if (m_ValidLocations.Count > 0)
                            newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        else
                            continue;

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(mobileLocation.X, mobileLocation.Y, mobileLocation.Z + 5), map);

                        if (Utility.RandomDouble() <= .5)
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 2053, 0);

                        else
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 8707, particleSpeed, 0, false, false, 0, 0);
                    }

                    double damage = DamageMin + (int)(Math.Ceiling((double)DamageMin * spawnPercent));

                    if (mobile is BaseCreature)
                        damage *= 3.0;

                    mobile.SendMessage(149, "An impending doom fills your soul with dread...");
                    mobile.SendSound(0x246);

                    Timer.DelayCall(TimeSpan.FromSeconds(15), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (!SpecialAbilities.Exists(mobile)) return;

                        mobile.SendMessage(149, "Doom approaches....");
                        mobile.SendSound(0x246);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(25), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (!SpecialAbilities.Exists(mobile)) return;

                        mobile.SendMessage(149, "Doom is nearly upon you....");
                        mobile.SendSound(0x246);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (!SpecialAbilities.Exists(mobile)) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) return;

                        mobile.SendMessage(149, "Doom is at hand!");
                        mobile.SendSound(0x246);

                        IEntity doomLocationEntity = new Entity(Serial.Zero, new Point3D(mobile.X, mobile.Y, mobile.Z), map);
                        Effects.SendLocationParticles(doomLocationEntity, 0x3709, 10, 30, 2053, 0, 5052, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x653);

                        projectiles = 4 + (int)(Math.Ceiling((double)maxExtraProjectiles * spawnPercent));
                        int blood = 4 + (int)(Math.Ceiling((double)maxExtraProjectiles * spawnPercent));

                        particleSpeed = 4;

                        mobile.PublicOverheadMessage(MessageType.Regular, 149, false, "*doomed*");

                        for (int a = 0; a < projectiles; a++)
                        {
                            Point3D newLocation;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(mobile.Location, true, false, mobile.Location, mobile.Map, 1, 15, 5, 10, true);

                            if (m_ValidLocations.Count > 0)
                                newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                            else
                                continue;

                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z + 5), map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), map);

                            if (Utility.RandomDouble() <= .5)
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 2053, 0);

                            else
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 8707, particleSpeed, 0, false, false, 0, 0);
                        }

                        for (int a = 0; a < blood; a++)
                        {
                            Point3D bloodlocation = new Point3D(mobile.Location.X + Utility.RandomList(-2, 2), mobile.Location.Y + Utility.RandomList(-2, 2), mobile.Location.Z);
                            SpellHelper.AdjustField(ref bloodlocation, map, 12, false);

                            new Blood().MoveToWorld(bloodlocation, mobile.Map);
                        }

                        AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                    });
                }
            });
        }

        public override void OnThink()
        {
            base.OnThink();            

            if (SpecialAbilities.MonsterCanDamage(this, Combatant) && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextRaiseDeadAllowed)
                        {
                            RaiseDead();
                            return;
                        }
                    break;                    
                }
            }

            //Reveal Hidden)
            if (m_NextRevealAllowed <= DateTime.UtcNow)
            {
                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 30);

                bool creatureWasRevealed = false;

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (!mobile.Hidden) continue;
                    if (mobile is UOACZBaseUndead) continue;
                    if (mobile is PlayerMobile)
                    {
                        PlayerMobile player = mobile as PlayerMobile;

                        if (player.IsUOACZUndead)
                            continue;
                    }

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    creatureWasRevealed = true;

                    mobile.RevealingAction();
                    mobile.SendMessage("You have been revealed by the Ancient Necromancer!");

                    Effects.PlaySound(mobile.Location, mobile.Map, 0x58F);
                    mobile.FixedParticles(0x376A, 10, 20, 5036, m_OrbRevealHue - 1, 0, EffectLayer.CenterFeet);

                    int damage = DamageMax;

                    if (mobile is BaseCreature)
                        damage *= 2;

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);
                }

                if (creatureWasRevealed)
                {
                    if (m_Item != null)
                    {
                        if (!m_Item.Deleted)
                        {
                            TimedStatic discharge = new TimedStatic(0x3779, .5);
                            discharge.Hue = m_OrbRevealHue;
                            discharge.Name = "dissipated energy";
                            discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                            m_Item.PublicOverheadMessage(MessageType.Regular, 0x482, false, "None escapes our gaze.");
                            m_Item.Hue = m_OrbRevealHue;

                            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                            {
                                if (m_Item == null) return;
                                if (m_Item.Deleted) return;

                                m_Item.Hue = m_OrbMainHue;
                            });
                        }
                    }
                }

                m_NextRevealAllowed = DateTime.UtcNow + NextRevealDelay;
            }

            if (DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*cackles*");
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*sneers*");
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*scowls*");
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*seethes*");
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*curses*");
                    break;
                }

                Animate(27, 10, 1, true, false, 0);
                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            int magicItems = 10;

            c.DropItem(new Arrow(150));
            c.DropItem(new Bolt(150));
            c.DropItem(new Bandage(150));

            for (int a = 0; a < magicItems; a++)
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1:
                        BaseWeapon weapon = Loot.RandomWeapon();

                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(2, 5);
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(2, 5);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(2, 5);
                        
                        weapon.Identified = true;

                        c.DropItem(weapon);
                    break;

                    case 2:
                        BaseArmor armor = Loot.RandomArmorOrShield();

                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(2, 5);
                        armor.DurabilityLevel = (ArmorDurabilityLevel)Utility.RandomMinMax(2, 5);

                        armor.Identified = true;

                        c.DropItem(armor);
                    break;
                }
            }

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }

            if (m_Item != null)
                m_Item.Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (!m_Creatures[a].Deleted)
                        m_Creatures[a].Delete();
                }
            }

            if (m_Item != null)
                m_Item.Delete();
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }

        public override int GetAngerSound() { return 0x2BC; }
        public override int GetIdleSound() { return 0x2B9; }
        public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x621; }
        public override int GetDeathSound() { return 0x58D; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x654);

            return base.OnMove(d);
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (m_Item != null)
            {
                Point3D skullLocation = new Point3D(Location.X, Location.Y, Z + 35);
                m_Item.Location = skullLocation;
            }
        }

        public UOACZTheGatekeeper(Serial serial): base(serial)
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

            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }

            writer.Write(m_Item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            m_Creatures = new List<Mobile>();

            int creaturesCount = reader.ReadInt();
            for (int a = 0; a < creaturesCount; a++)
            {
                Mobile creature = reader.ReadMobile();

                if (creature != null)
                    m_Creatures.Add(creature);
            }

            m_Item = reader.ReadItem();
        }
    }
}



