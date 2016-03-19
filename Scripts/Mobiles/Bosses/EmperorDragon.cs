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
    [CorpseName("the emperor dragon's corpse")]
    public class EmperorDragon : BaseCreature
    {
        public override string TitleReward { get { return "Slayer of Lyth"; } }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextKnockbackAllowed;
        public TimeSpan NextKnockbackDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextMassiveBreathAllowed;
        public TimeSpan NextMassiveBreathDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextFireBarrageAllowed;
        public TimeSpan NextFireBarrageDelay = TimeSpan.FromSeconds(20);
        
        public DateTime m_NextFlameMarkAllowed;
        public TimeSpan NextFlameMarkDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextAbilityAllowed;
        public double NextAbilityDelayMin = 10;
        public double NextAbilityDelayMax = 5;
        
        public static int FireBarrageRange = 20;
        public static int FlameMarkRange = 20;

        public int damageIntervalThreshold = 500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 40;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_Creatures = new List<Mobile>();

        public string[] idleSpeech { get { return new string[] {"*paces*"}; } }
        public string[] combatSpeech { get  { return new string[] {""}; } }

        [Constructable]
        public EmperorDragon(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Emperor Dragon";
            Body = 826;
            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(20000);
            SetStam(10000);
            SetMana(0);

            SetDamage(30, 50);            

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 150;
        }

        public override int AttackRange { get { return 2; } }

        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "Lyth the Destroyer has arisen and stirs within Destard Dungeon..."; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }
        public override bool HasAlternateHighSeasAttackAnimation { get { return true; } }

        public override int Meat { get { return 25; } }

        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 1.5;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double minLength = 6;
            double maxLength = 18;

            MassiveBreathRange = (int)(minLength + ((maxLength - minLength) * spawnPercent));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);          
        }        

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {           
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    double spawnPercent = (double)intervalCount / (double)totalIntervals;

                    double minLength = 6;
                    double maxLength = 18;    
                    
                    MassiveBreathRange = (int)(minLength + ((maxLength - minLength) * spawnPercent));

                    if (intervalCount % 5 == 0)
                        CallForAid();

                    else
                    {
                        switch(Utility.RandomMinMax(1, 2))
                        {
                            case 1: FireBreathSpin(); break;
                            case 2: WingBuffet(); break;
                        }
                    }
                }
            }            

            base.OnDamage(amount, from, willKill);
        }

        public TimeSpan GetNextAbilityDelay()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            return TimeSpan.FromSeconds(NextAbilityDelayMin - ((NextAbilityDelayMin - NextAbilityDelayMax) * spawnPercent));
        }

        public void Knockback()
        {
            if (!SpecialAbilities.Exists(Combatant)) return;
            if (!SpecialAbilities.MonsterCanDamage(this, Combatant)) return;

            Point3D targetLocation = Combatant.Location;
            Map map = Combatant.Map;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double initialDelay = 1;
            double totalDelay = 1 + initialDelay;            

            Animate(14, 12, 1, true, false, 0);
            PlaySound(GetAngerSound());

            PublicOverheadMessage(MessageType.Regular, 0, false, "*lashes out violently*");

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextKnockbackAllowed = DateTime.UtcNow + NextKnockbackDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                {
                    AbilityInProgress = false;
                    Combatant = null;
                }
            });

            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;

                double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax)) * 2;

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

                    if (mobile is BaseCreature)
                        damage *= 2;

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobile, damage, 15, -1, "", "The beast flings you aside!");
                } 
            });           
        }

        public void MassiveBreath()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double totalDelay = 3;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            Effects.PlaySound(Location, Map, GetAngerSound());

            Direction direction = Utility.GetDirection(Location, Combatant.Location);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*takes massive breath*");

            m_NextMassiveBreathAllowed = DateTime.UtcNow + NextMassiveBreathDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            SpecialAbilities.DoMassiveBreathAttack(this, Location, direction, MassiveBreathRange, true, BreathType.Fire, false);            
        }

        public void FireBarrage()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, FireBarrageRange);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;

                mobileCount++;
            }
            
            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, FireBarrageRange);

            List<Mobile> m_NearbyMobiles = new List<Mobile>();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;                
                if (Combatant != null)
                {
                    if (mobileCount > 1 && mobile == Combatant)
                        continue;
                }

                m_NearbyMobiles.Add(mobile);
            }

            nearbyMobiles.Free();
            
            if (m_NearbyMobiles.Count == 0)
                return;

            Mobile mobileTarget = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];
            
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int maxExtraFireballs = 10;
            int fireballs = 10 + (int)Math.Ceiling(((double)maxExtraFireballs * spawnPercent));

            double directionDelay = .25;
            double initialDelay = 1;
            double fireballDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)fireballs * fireballDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
                        
            m_NextFireBarrageAllowed = DateTime.UtcNow + NextFireBarrageDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*takes deep breath*");

            Point3D location = Location;
            Map map = Map;   

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(5, 15, 1, true, false, 0);
                PlaySound(GetAngerSound());                            

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this)) 
                        return;

                    for (int a = 0; a < fireballs; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * fireballDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this)) return;
                            if (DamageIntervalInProgress) return;

                            bool mobileTargetValid = true;

                            if (mobileTarget == null)
                                mobileTargetValid = false;

                            else if (mobileTarget.Deleted || !mobileTarget.Alive)
                                mobileTargetValid = false;

                            else
                            {
                                if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= FireBarrageRange)
                                    mobileTargetValid = false;
                            }

                            if (mobileTargetValid)
                            {
                                targetLocation = mobileTarget.Location;
                                targetMap = mobileTarget.Map;
                            }

                            int effectSound = 0x357;
                            int itemID = 0x36D4;
                            int itemHue = 0;

                            int impactSound = 0x226;
                            int impactHue = 0;

                            int xOffset = 0;
                            int yOffset = 0;

                            int distance = Utility.GetDistance(Location, targetLocation);

                            if (distance > 1)
                            {
                                if (Utility.RandomDouble() <= .5)
                                    xOffset = Utility.RandomList(-1, 1);

                                if (Utility.RandomDouble() <= .5)
                                    yOffset = Utility.RandomList(-1, 1);
                            }

                            IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), map);

                            Point3D adjustedLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z);
                            SpellHelper.AdjustField(ref adjustedLocation, targetMap, 12, false);

                            IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 10), targetMap);

                            Effects.PlaySound(location, map, effectSound);
                            Effects.SendMovingEffect(startLocation, endLocation, itemID, 8, 0, false, false, itemHue, 0);

                            double targetDistance = Utility.GetDistanceToSqrt(location, adjustedLocation);
                            double destinationDelay = (double)targetDistance * .06;

                            Direction newDirection = Utility.GetDirection(location, adjustedLocation);

                            if (Direction != newDirection)
                                Direction = newDirection;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                Effects.PlaySound(adjustedLocation, targetMap, impactSound);
                                Effects.SendLocationParticles(EffectItem.Create(adjustedLocation, targetMap, EffectItem.DefaultDuration), 0x3709, 20, 20, impactHue, 0, 0, 0);

                                Queue m_Queue = new Queue();

                                nearbyMobiles = targetMap.GetMobilesInRange(adjustedLocation, 0);

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

                                    int damage = (int)(Math.Round((double)DamageMin / 5));

                                    if (mobile is BaseCreature)
                                        damage *= 3;

                                    else
                                    {
                                        if (Utility.GetDistance(Location, mobile.Location) <= 1)
                                            damage = (int)(Math.Round((double)damage * .5));
                                    }

                                    DoHarmful(mobile);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, this, damage, 0, 100, 0, 0, 0);
                                }
                            });
                        });
                    }
                });
            });
        }
        
        public void FlameMark()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, FlameMarkRange);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;

                mobileCount++;
            }

            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, FlameMarkRange);

            List<Mobile> m_NearbyMobiles = new List<Mobile>();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;                
                if (Combatant != null)
                {
                    if (mobileCount > 1 && mobile == Combatant)
                        continue;
                }

                m_NearbyMobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            if (m_NearbyMobiles.Count == 0)
                return;

            Mobile mobileTarget = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;
                        
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = 2;
            double totalDelay = 1 + directionDelay + initialDelay;

            int maxRadius = 7;
            int radius = 3 + (int)(Math.Ceiling((double)maxRadius * spawnPercent));

            m_NextFlameMarkAllowed = DateTime.UtcNow + NextFlameMarkDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (DamageIntervalInProgress) return;

                Animate(29, 22, 1, true, false, 0);
                PlaySound(GetAngerSound());

                PublicOverheadMessage(MessageType.Regular, 0, false, "*points a mighty claw*");

                TimedStatic flamemark = new TimedStatic(6571, initialDelay);
                flamemark.Hue = 2591;
                flamemark.Name = "flamemark";
                flamemark.MoveToWorld(targetLocation, targetMap);

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this)) return;

                    int damage = DamageMin;
                    
                    int minRange = radius * -1;
                    int maxRange = radius;

                    int effectHue = 0; //2590;

                    Effects.PlaySound(targetLocation, targetMap, 0x306); //0x1F3 //0x306 //0x309

                    for (int a = minRange; a < maxRange + 1; a++)
                    {
                        for (int b = minRange; b < maxRange + 1; b++)
                        {
                            Point3D newPoint = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);
                            SpellHelper.AdjustField(ref newPoint, targetMap, 12, false);

                            int distance = Utility.GetDistance(targetLocation, newPoint);

                            double effectChance = 1.0 - ((double)distance * .05);

                            if (Utility.RandomDouble() > effectChance)
                                continue;

                            Timer.DelayCall(TimeSpan.FromSeconds(distance * .10), delegate
                            {
                                if (Utility.RandomDouble() <= .15)
                                {
                                    SingleFireField singleFireField = new SingleFireField(this, 0, 2, 30, 3, 5, false, false, true, -1, true);
                                    singleFireField.MoveToWorld(newPoint, targetMap);
                                }

                                Effects.PlaySound(newPoint, targetMap, Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580));
                                Effects.SendLocationParticles(EffectItem.Create(newPoint, targetMap, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, effectHue, 0, 5029, 0);

                                IPooledEnumerable mobilesOnTile = targetMap.GetMobilesInRange(newPoint, 0);

                                Queue m_Queue = new Queue();

                                foreach (Mobile mobile in mobilesOnTile)
                                {
                                    if (mobile == this) continue;
                                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                                    
                                    m_Queue.Enqueue(mobile);
                                }

                                mobilesOnTile.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    if (mobile is BaseCreature)
                                        damage *= 3;

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, this, damage, 100, 0, 0, 0, 0);
                                }                                
                            });
                        }
                    }
                });                
            });            
        }

        public void WingBuffet()
        {
            if (!SpecialAbilities.Exists(this))
                return;
            
            int wingFlaps = 3;
            double flapDuration = 0.5;
            
            double minRange = 10;
            double maxRange = 30;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            int range = (int)(minRange + ((maxRange - minRange) * spawnPercent));

            double totalDelay = 3;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) 
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*furiously beats wings*");

            for (int a = 0; a < wingFlaps; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * flapDuration), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;
                    
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(19, 10, 2, true, false, 0);                    
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)wingFlaps * flapDuration), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Point3D location = Location;
                Map map = Map;

                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, range);

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

                    int distance = range - Utility.GetDistance(location, mobile.Location);

                    if (distance < 1)
                        distance = 1;

                    double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax)) * 2;

                    SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, damage, distance, -1, "", "The beast buffets you with its wings!");
                }
            });
        }        

        public void FireBreathSpin()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;
                        
            double maxRotations = 4;
            int rotations = (int)(Math.Ceiling(maxRotations * spawnPercent));

            double totalDelay = 1.5 + ((double)rotations * 1.5);
            
            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
           
            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            Effects.PlaySound(Location, Map, GetAngerSound());

            SpecialAbilities.DoMassiveBreathAttack(this, Location, Direction , 5, true, BreathType.Fire, false);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*takes a massive breath*");

            for (int a = 0; a < rotations; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.5 + (a * 1.5)), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;
                    
                    int newDirectionValue = (int)Direction;
                    newDirectionValue += Utility.RandomList(-3, -4, -5, 3, 4, 5);

                    if (newDirectionValue > 7)
                        newDirectionValue = 0 + (newDirectionValue - 8);

                    else if (newDirectionValue < 0)
                        newDirectionValue = 8 + newDirectionValue;

                    Direction nextDirection = (Direction)(newDirectionValue);

                    SpecialAbilities.DoMassiveBreathAttack(this, Location, nextDirection, MassiveBreathRange, true, BreathType.Fire, false);  
                });
            }            
        }

        public void CallForAid()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double maxExtraCreatures = 4;
            int creatures = 2 + (int)(Math.Ceiling(maxExtraCreatures * spawnPercent));

            double directionDelay = .25;
            double initialDelay = 2;
            double totalDelay = 1 + directionDelay + initialDelay;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");                    

            PublicOverheadMessage(MessageType.Regular, 0, false, "*calls for aid*");

            Effects.PlaySound(Location, Map, GetAngerSound());    

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))   
                    return;                

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(27, 16, 1, true, false, 0);                
            });

            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Effects.PlaySound(Location, Map, GetAngerSound());   

                for (int a = 0; a < creatures; a++)
                {
                    Point3D targetLocation = Location;
                    Map map = Map;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, true, Location, Map, 1, 15, 1, 5, true);

                    if (m_ValidLocations.Count > 0)
                        targetLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    BaseCreature bc_Creature = null;

                    if (spawnPercent <= .25)
                    {
                        switch (Utility.RandomMinMax(1, 1))
                        {
                            case 1: bc_Creature = new RoyalWhelp(); break;
                        }
                    }

                    else if (spawnPercent <= .50)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: bc_Creature = new RoyalWhelp(); break;
                            case 2: bc_Creature = new RoyalWhelp(); break;
                            case 3: bc_Creature = new RoyalDrake(); break;
                        }
                    }

                    else
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: bc_Creature = new RoyalWhelp(); break;
                            case 2: bc_Creature = new RoyalDrake(); break;
                            case 3: bc_Creature = new RoyalDragon(); break;
                        }
                    }

                    if (bc_Creature != null)
                    {
                        m_Creatures.Add(bc_Creature);
                        bc_Creature.MoveToWorld(targetLocation, map);

                        bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                        Effects.SendLocationParticles(EffectItem.Create(targetLocation, map, EffectItem.DefaultDuration), 0x3728, 10, 30, 0, 0, 2023, 0);
                    }
                }
            });  
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextFireBarrageAllowed)
                        {
                            FireBarrage();
                            return;
                        }
                    break;

                    case 2:
                        if (DateTime.UtcNow >= m_NextFlameMarkAllowed)
                        {
                            FlameMark();
                            return;
                        }
                    break;

                    case 3:
                        if (DateTime.UtcNow >= m_NextKnockbackAllowed && Utility.GetDistance(Location, Combatant.Location) <= AttackRange)
                        {
                            Knockback();
                            return;
                        }
                    break;

                    case 4:
                        if (DateTime.UtcNow >= m_NextMassiveBreathAllowed && Utility.GetDistance(Location, Combatant.Location) <= MassiveBreathRange)
                        {
                            MassiveBreath();
                            return;
                        }
                    break;
                }                
            }

            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*roars*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*bares fangs*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*growls*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*flicks tongue*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*snorts*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                        break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }
        }

        public override int GetAngerSound() { return 0x4FC; }
        public override int GetIdleSound() { return 0x4FD; }
        public override int GetAttackSound() { return 0x4FC; }
        public override int GetHurtSound() { return 0x4EC; }
        public override int GetDeathSound() { return 0x4EA; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x63E);
            
            return base.OnMove(d);            
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            BossPersistance.PersistanceItem.DestardBossLastStatusChange = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 10) == 1)
                c.AddItem(new LythTheDestroyerStatue());

            if (Utility.RandomMinMax(1, 20) == 1)
                c.AddItem(new DestroyersSkull());

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
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
        }

        public EmperorDragon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(damageIntervalThreshold);
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(totalIntervals);

            //Version 1
            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            //Version 1
            if (version >= 1)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    m_Creatures.Add(creature);
                }
            }
        }
    }
}

//14 15 1 true false 0 ClawStomp
//23 12 1 true false 0 Large Bite
//24 10 1 true false 0 RUN
//27 16 1 true false 0 ROAR
//28 12 1 true false 0 FAN WINGS