using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Network;
using System.Collections;
using System.Collections.Generic;



namespace Server.Mobiles
{
	[CorpseName( "DeDOS's corpse" )]
	public class DeDOS : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);
                
        public DateTime m_NextShockAllowed;
        public TimeSpan NextShockDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextTraceRouteAllowed;
        public TimeSpan NextTraceRouteDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextBasicCreatureAllowed;
        public TimeSpan NextBasicCreatureDelay = TimeSpan.FromSeconds(45);

        public DateTime m_NextNullRouteMeleeAllowed;
        public TimeSpan NextNullRouteMeleeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_TraceRouteTimeout;
        public double MaxTraceRouteMinDuration = 1;
        public double MaxTraceRouteMaxDuration = 10;
                
        public DateTime m_NextAbilityAllowed;
        public double MaxAbilityDelaySeconds = 10;
        public double MinAbilityDelaySeconds = 5;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(10);        

        public int damageIntervalThreshold = 2500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 40;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public bool m_KynDragonSpawned = false;
        public bool m_KynDragonKilled = false;

        public List<Mobile> m_Creatures = new List<Mobile>();
        public List<Mobile> m_EngulfedMobiles = new List<Mobile>();
        public List<Mobile> m_TraceRouteTargets = new List<Mobile>();
        public List<Mobile> m_ShockTargets = new List<Mobile>();
        public List<Item> m_Firewalls = new List<Item>();
        public List<Mobile> m_FireWallsTargets = new List<Mobile>();

        public int BandwidthTextHue = 149;
        public int TraceRouteTextHue = 2606;
        public int NullRouteTextHue = 2550;
        public int TrafficTextHue = 149;
        public int IntrusionTextHue = 2628;
        public int BotNetTextHue = 0x3F;
        public int BlackHoleTextHue = 2603;
        public int FirewallTextHue = 2116;

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public DeDOS() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "DeDOS";

            Body = 261;
            Hue = 2525;
            SpeechHue = 2525;

            BaseSoundID = 639;            

			SetStr(100);
			SetDex(25);
			SetInt(100);

			SetHits(80000);
            SetStam(80000);
            SetMana(80000);

			SetDamage(20, 40);

            AttackSpeed = 20;

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 500);
            
			SetSkill(SkillName.MagicResist, 100);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 100;
		}

        public override bool MovementRestrictionImmune { get { return true; } }
        public override int PoisonResistance { get { return 5; } }

        public override bool AlwaysBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            SpellHue = 2587;

            AttackSpeed = 20;

            UniqueCreatureDifficultyScalar = 1.5;
            
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));            
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
                    SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, 5, true, 0, false, "", "");

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    double spawnPercent = (double)intervalCount / (double)totalIntervals;

                    NextAbilityDelay = TimeSpan.FromSeconds(MaxAbilityDelaySeconds - ((MaxAbilityDelaySeconds - MinAbilityDelaySeconds) * spawnPercent));

                    int remainder = intervalCount % 3;

                    double attackBandwidth = (double)intervalCount * 100 / (double)totalIntervals;

                    CreateFlashText(Location, Map, "*Attack bandwidth increased to " + attackBandwidth.ToString() + " gbps*", BandwidthTextHue);

                    switch(remainder)
                    {
                        case 0: NullRouteFull(); break;
                        case 1: BlackHoles(); break;
                        case 2: Firewalls(); break;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(4), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        switch (intervalCount)
                        {
                            case 1: SpawnCreatures(typeof(DeDOSBotNetZombie), 6); break;
                            case 3: SpawnCreatures(typeof(DeDOSBot), 4); break;
                            case 5: SpawnCreatures(typeof(DeDOSNetGremlin), 4); break;
                            case 7: SpawnCreatures(typeof(DeDOSTunneler), 2); break;
                            case 9: SpawnCreatures(typeof(DeDOSTrojanHorse), 2); break;

                            case 11:
                                SpawnCreatures(typeof(DeDOSBotNetZombie), 4);
                                SpawnCreatures(typeof(DeDOSBot), 3);
                            break;

                            case 13:
                                SpawnCreatures(typeof(DeDOSBot), 4);
                                SpawnCreatures(typeof(DeDOSNetGremlin), 3);
                            break;

                            case 15:
                                SpawnCreatures(typeof(DeDOSNetGremlin), 3);
                                SpawnCreatures(typeof(DeDOSTunneler), 2);
                            break;

                            case 17:
                                SpawnCreatures(typeof(DeDOSTunneler), 2);
                                SpawnCreatures(typeof(DeDOSTrojanHorse), 2);
                            break;

                            case 19:
                                SpawnCreatures(typeof(DeDOSBotNetZombie), 6);
                                SpawnCreatures(typeof(DeDOSLargeBot), 2);
                            break;

                            case 21:
                                SpawnCreatures(typeof(DeDOSBot), 4);
                                SpawnCreatures(typeof(DeDOSLargeBot), 2);
                            break;

                            case 23:
                                SpawnCreatures(typeof(DeDOSNetGremlin), 4);
                                SpawnCreatures(typeof(DeDOSLargeBot), 2);
                            break;

                            case 25:
                                SpawnCreatures(typeof(DeDOSTunneler), 3);
                                SpawnCreatures(typeof(DeDOSLargeBot), 2);
                            break;

                            case 27:
                                SpawnCreatures(typeof(DeDOSTrojanHorse), 3);
                                SpawnCreatures(typeof(DeDOSLargeBot), 2);
                            break;

                            case 29:
                                SpawnCreatures(typeof(DeDOSBotNetZombie), 8);
                                SpawnCreatures(typeof(DeDOSMassiveBot), 2);
                            break;

                            case 31:
                                SpawnCreatures(typeof(DeDOSNetGremlin), 6);
                                SpawnCreatures(typeof(DeDOSMassiveBot), 2);
                            break;

                            case 33:
                                SpawnCreatures(typeof(DeDOSTunneler), 2);
                                SpawnCreatures(typeof(DeDOSTrojanHorse), 2);
                                SpawnCreatures(typeof(DeDOSMassiveBot), 2);
                            break;

                            case 35:
                                SpawnCreatures(typeof(DeDOSKynDragon), 1);

                                int projectiles = 6;
                                int particleSpeed = 4;

                                for (int a = 0; a < projectiles; a++)
                                {
                                    Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), Map);

                                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 2586, 0);
                                }

                                m_KynDragonSpawned = true;

                                Hidden = true;
                                Blessed = true;

                                CreateFlashText(Location, Map, "*a kyn dragon intercedes*", BlackHoleTextHue);
                            break;
                        }                       
                    });
                }

                else
                {
                    if (amount > 10 && Utility.RandomDouble() < .125 && SpecialAbilities.MonsterCanDamage(this, from) && Utility.GetDistance(Location, from.Location) <= 18)
                    {
                        PlaySound(GetHurtSound());

                        double result = Utility.RandomDouble();

                        if (result < .8)
                        {
                            Animate(10, 8, 1, true, false, 0);
                            Effects.PlaySound(Location, Map, 0x2F4);

                            CreateFlashText(Location, Map, "*request denied*", TrafficTextHue);

                            int effectHue = 0;

                            DeDOSElectricField electricField = new DeDOSElectricField(this, effectHue, 1, 20, 3, 5, false, false, true, -1, true);
                            electricField.MoveToWorld(from.Location, Map);
                        }

                        else
                        {
                            Combatant = null;

                            int projectiles = 6;
                            int particleSpeed = 4;

                            for (int a = 0; a < projectiles; a++)
                            {
                                Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                                SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), Map);

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 2586, 0);
                            }

                            CreateFlashText(Location, Map, "*reroutes address*", TrafficTextHue);

                            SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null);                               

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                if (Hidden)
                                    RevealingAction();

                                PlaySound(GetAngerSound());
                            });
                        }
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void CreateFlashText(Point3D location, Map map, string text, int textHue)
        {
            TimedStatic flash = new TimedStatic(0x37Be, 1.5);
            flash.Hue = 2950;
            flash.MoveToWorld(location, map);

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                if (flash != null && SpecialAbilities.Exists(this))
                    flash.PublicOverheadMessage(MessageType.Regular, textHue, false, text);
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (m_KynDragonSpawned)
            {
                if (m_KynDragonKilled)
                {
                    Blessed = false;
                    Frozen = false;
                    Hidden = false;
                }

                else
                {
                    Blessed = true;
                    Frozen = true;
                    Hidden = true;
                }
            }
            
            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {                
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextShockAllowed)
                        {
                            Shock();
                            return;
                        }
                    break;

                    case 2:
                        if (DateTime.UtcNow >= m_NextTraceRouteAllowed)
                        {
                            StartTraceRoute();
                            return;
                        }
                    break;

                    case 3:
                        if (DateTime.UtcNow >= m_NextBasicCreatureAllowed)
                        {
                            BasicCreature();
                            return;
                        }
                    break;

                    case 4:
                        if (DateTime.UtcNow >= m_NextNullRouteMeleeAllowed)
                        {
                            NullRouteMelee();
                            return;
                        }
                    break;
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
                });
            }
        }

        #region Normal Abilities

        public void Shock()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            AbilityInProgress = true;

            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;
            m_NextShockAllowed = DateTime.UtcNow + NextShockDelay;

            Animate(10, 8, 1, true, false, 0);

            m_ShockTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 20;
            int cycles = 5 + (int)(Math.Ceiling(15 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)cycles / 10));

            double stationaryDelay = loops + 1;                

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            CreateFlashText(Location, Map, "*intrusion detected...purging*", IntrusionTextHue);

            Point3D location = Location;
            Map map = Map;

            for (int a = 0; a < loops; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;                                      

                    PlaySound(GetAngerSound());
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;  
                
                AbilityInProgress = false;

                PlaySound(0x211);
            });

            for (int a = 0; a < cycles; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .08), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;         

                    List<Mobile> m_ValidMobiles = new List<Mobile>();

                    IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile == null) continue;
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (mobile.Hidden) continue;

                        m_ValidMobiles.Add(mobile);
                    }

                    mobilesInRange.Free();

                    if (m_ValidMobiles.Count > 0)
                    {
                        Mobile target = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];                       

                        Effects.PlaySound(Location, Map, 0x211);

                        TimedStatic discharge = new TimedStatic(0x3779, .5);
                        discharge.Hue = 2587;
                        discharge.Name = "";
                        discharge.MoveToWorld(Location, Map);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 12), Map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 7), target.Map);

                        int particleSpeed = 5;

                        Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                        double distance = Utility.GetDistanceToSqrt(location, target.Location);
                        double distanceDelay = (double)distance * .10;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this)) return;                  
                            if (!SpecialAbilities.MonsterCanDamage(this, target)) return;                          
                            if (Utility.GetDistance(location, target.Location) >= 30) return;
                            
                            double damage = Utility.RandomMinMax(1, 10) + (10 * spawnPercent);
                            double duration = Utility.RandomMinMax(2, 4);

                            if (m_ShockTargets.Contains(target))
                                damage *= .33;     

                            if (target is BaseCreature)                            
                                damage *= 2;

                            if (damage < 1)
                                damage = 1;

                            m_ShockTargets.Add(target);

                            Effects.PlaySound(target.Location, target.Map, 0x5C3);

                            new Blood().MoveToWorld(target.Location, target.Map);
                            AOS.Damage(target, (int)damage, 100, 0, 0, 0, 0);
                        });
                    }
                });
            }
        }

        public void StartTraceRoute()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;
            m_NextTraceRouteAllowed = DateTime.UtcNow + NextTraceRouteDelay;

            Animate(10, 8, 1, true, false, 0);

            int maxDistance = 20;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

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
            nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

            Mobile closestTarget = null;
            int closestTargetDistance = 10000;

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

                int distance = Utility.GetDistance(Location, mobile.Location);

                if (distance < closestTargetDistance)
                {
                    closestTarget = mobile;
                    closestTargetDistance = distance;
                }
            }

            nearbyMobiles.Free();

            if (closestTarget == null)
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = .75;
            double totalDelay = 1 + directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
                        
            m_NextTraceRouteAllowed = DateTime.UtcNow + NextTraceRouteDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            CreateFlashText(Location, Map, "*begins trace route*", TraceRouteTextHue);

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = closestTarget.Location;
            Map targetMap = closestTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;
                
                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    double traceRouteDuration = MaxTraceRouteMinDuration + ((MaxTraceRouteMaxDuration - MaxTraceRouteMinDuration) * spawnPercent);

                    m_TraceRouteTimeout = DateTime.UtcNow + TimeSpan.FromSeconds(traceRouteDuration);
                    m_TraceRouteTargets.Clear();

                    PlaySound(0x456);

                    nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

                    mobileCount = 0;

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (!Map.InLOS(Location, mobile.Location)) continue;
                        if (mobile.Hidden) continue;

                        mobileCount++;
                    }

                    nearbyMobiles.Free();
                    nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

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

                        int distance = Utility.GetDistance(Location, mobile.Location);

                        if (distance < closestTargetDistance)
                        {
                            closestTarget = mobile;
                            closestTargetDistance = distance;
                        }
                    }

                    nearbyMobiles.Free();

                    if (closestTarget == null)
                        return;

                    TraceRoute(Location, closestTarget);
                });
            });
        }

        public void TraceRoute(Point3D location, Mobile target)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(target)) return;

            if (DateTime.UtcNow > m_TraceRouteTimeout)
                return;

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 10), Map);

            int itemId = 14068; //18065
            int itemHue = 2599;

            double speedModifier = 1;

            Effects.SendMovingEffect(startLocation, endLocation, itemId, (int)(15 * speedModifier), 0, false, false, itemHue, 0);

            double distance = Utility.GetDistanceToSqrt(location, target.Location);
            double destinationDelay = (double)distance * .06;

            int ping = (int)Math.Round(destinationDelay * 100);
            
            string pingMessage = "*" + ping.ToString() + " ms ping to " + target.Name + "*";

            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (!SpecialAbilities.Exists(target)) return;

                double spawnPercent = (double)intervalCount / (double)totalIntervals;

                Point3D mobileLocation = target.Location;

                double damage = ((double)DamageMin * .5) + ((double)DamageMin * spawnPercent * .5);

                if (target is BaseCreature)
                    damage *= .5;

                DoHarmful(target);
                m_TraceRouteTargets.Add(target);

                target.PlaySound(0x456);
                target.PublicOverheadMessage(MessageType.Regular, TraceRouteTextHue, false, pingMessage);

                new Blood().MoveToWorld(target.Location, target.Map);
                AOS.Damage(target, this, (int)damage, 100, 0, 0, 0, 0);

                int maxDistance = 20;

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(mobileLocation, maxDistance);

                Mobile closestTarget = null;
                int closestTargetDistance = 10000;

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (!Map.InLOS(mobileLocation, mobile.Location)) continue;
                    if (mobile.Hidden) continue;
                    if (m_TraceRouteTargets.Contains(mobile)) continue;

                    int mobileDistance = Utility.GetDistance(Location, mobile.Location);

                    if (mobileDistance < closestTargetDistance)
                    {
                        closestTarget = mobile;
                        closestTargetDistance = mobileDistance;
                    }
                }

                nearbyMobiles.Free();

                if (closestTarget == null)
                    return;

                TraceRoute(mobileLocation, closestTarget);
            });
        }

        public void BasicCreature()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            CreateFlashText(Location, Map, "*botnet activated*", BotNetTextHue);

            PlaySound(GetAngerSound());

            m_NextBasicCreatureAllowed = DateTime.UtcNow + NextBasicCreatureDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

            Animate(10, 8, 1, true, false, 0);

            AbilityInProgress = true;

            switch(Utility.RandomMinMax(1, 5))
            {
                case 1: SpawnCreatures(typeof(DeDOSBotNetZombie), 1); break;
                case 2: SpawnCreatures(typeof(DeDOSBotNetZombie), 1); break;
                case 3: SpawnCreatures(typeof(DeDOSBot), 1); break;
                case 4: SpawnCreatures(typeof(DeDOSBot), 1); break;
                case 5: SpawnCreatures(typeof(DeDOSNetGremlin), 1); break;
            }            

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                List<Mobile> m_ValidMobiles = new List<Mobile>();

                IPooledEnumerable mobilesInRange = Map.GetMobilesInRange(Location, 20);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile == null) continue;
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (mobile.Hidden) continue;

                    m_ValidMobiles.Add(mobile);
                }

                mobilesInRange.Free();

                Mobile mobileTarget = null;

                if (m_ValidMobiles.Count > 0)
                    mobileTarget = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];

                if (mobileTarget == null)
                    return;

                mobilesInRange = Map.GetMobilesInRange(Location, 50);

                Queue m_Queue = new Queue();

                int zombieCount = 0;

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile is DeDOSBotNetZombie)
                    {
                        m_Queue.Enqueue(mobile);
                        zombieCount++;
                    }
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    DeDOSBotNetZombie zombie = (DeDOSBotNetZombie)m_Queue.Dequeue();

                    zombie.Combatant = mobileTarget;
                    zombie.PlaySound(zombie.GetAngerSound());
                }

                if (zombieCount > 0)
                {
                    mobileTarget.PublicOverheadMessage(MessageType.Regular, BotNetTextHue, false, "*becomes botnet target*");

                    mobileTarget.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);
                    mobileTarget.PlaySound(0x1E1);
                }  

                AbilityInProgress = false;
            });               
        }

        public void NullRouteMelee()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            if (Combatant == null) return;
            if (Utility.GetDistance(Location, Combatant.Location) > 2) return;
            if (!Map.InLOS(Location, Combatant.Location)) return;

            Mobile mobile = Combatant;

            Point3D location = Combatant.Location;
            Map map = Combatant.Map;

            double duration = 15;

            CreateFlashText(Location, Map, "*null routes " + mobile.Name + "*", NullRouteTextHue);
            
            PlaySound(GetAngerSound());

            m_NextNullRouteMeleeAllowed = DateTime.UtcNow + NextNullRouteMeleeDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

            m_EngulfedMobiles.Add(mobile);
            
            SpecialAbilities.HinderSpecialAbility(1.0, null, mobile, 1.0, duration, false, -1, false, "", "You have been 'null routed' and cannot move or speak!");

            mobile.Squelched = true;
            mobile.Hidden = true;

            PlaySound(0x37B);
            Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2586, 0, 2023, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
            {
                if (!SpecialAbilities.Exists(mobile))
                    return;

                mobile.Squelched = false;
                mobile.Hidden = false;

                if (!SpecialAbilities.Exists(this)) return;

                Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x101));

                if (m_EngulfedMobiles.Contains(mobile))
                    m_EngulfedMobiles.Remove(mobile);
            });            
        }

        #endregion

        #region Spawn Functions

        public void SpawnCreatures(Type type, int creatures)
        {
            for (int a = 0; a < creatures; a++)
            {
                Point3D creatureLocation = Location;
                Map creatureMap = Map;

                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                if (m_ValidLocations.Count > 0)
                    creatureLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                BaseCreature creature = (BaseCreature)Activator.CreateInstance(type);

                if (creature == null)
                    continue;

                creature.MoveToWorld(creatureLocation, creatureMap);
                creature.PlaySound(creature.GetAngerSound());

                Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                m_Creatures.Add(creature);
            }
        }

        #endregion

        #region Epic Abilities

        public void NullRouteFull()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            AbilityInProgress = true;

            CreateFlashText(Location, Map, "*Applies Null Route: 24 Hours*", NullRouteTextHue);

            PlaySound(0x652);    

            Animate(10, 8, 1, true, false, 0);

            int range = 24;

            double nullrouteChance = .25 + (spawnPercent * .5);

            int nullroutedCount = 0;

            Queue m_Queue = new Queue();

            List<Mobile> m_ValidMobiles = new List<Mobile>();

            IPooledEnumerable mobilesInRange = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in mobilesInRange)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                m_ValidMobiles.Add(mobile);

                if (Utility.RandomDouble() <= nullrouteChance)
                {
                    m_Queue.Enqueue(mobile);
                    nullroutedCount++;
                }
            }

            mobilesInRange.Free();

            double duration = 15;
            int damage = 20;            

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();
                Point3D mobileLocation = mobile.Location;
                                                
                SpecialAbilities.HinderSpecialAbility(1.0, null, mobile, 1.0, duration, false, -1, false, "", "You have been 'null routed' and cannot move or speak!");

                mobile.Squelched = true;
                mobile.Hidden = true;

                mobile.PlaySound(0x37B);      

                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2586, 0, 2023, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                {
                    if (!SpecialAbilities.Exists(mobile))
                        return;

                    mobile.Squelched = false;
                    mobile.Hidden = false;
                });               
            }

            if (nullroutedCount == 0)
            {
                if (m_ValidMobiles.Count > 0)
                {
                    Mobile mobile = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];
                    
                    SpecialAbilities.HinderSpecialAbility(1.0, null, mobile, 1.0, duration, false, -1, false, "", "You have been 'null routed' and cannot move or speak!");

                    mobile.Squelched = true;
                    mobile.Hidden = true;

                    mobile.PlaySound(0x37B);

                    Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2586, 0, 2023, 0);

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (!SpecialAbilities.Exists(mobile))
                            return;

                        mobile.Squelched = false;
                        mobile.Hidden = false;
                    });  
                }
            }

            AbilityInProgress = false;
        }

        public void BlackHoles()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            AbilityInProgress = true;

            CreateFlashText(Location, Map, "*attempts to scrub bad traffic*", BlackHoleTextHue);

            PlaySound(0x650);

            Animate(10, 8, 1, true, false, 0);

            Point3D location = Location;
            Map map = Map;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 24;

            List<Point3D> m_BlackHoleLocations = new List<Point3D>();

            Queue m_Queue = new Queue();

            IPooledEnumerable mobilesInRange = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in mobilesInRange)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;  
                if (m_BlackHoleLocations.Contains(mobile.Location)) continue;
                if (mobile.Hidden) continue;
                
                m_BlackHoleLocations.Add(mobile.Location);                
            }

            mobilesInRange.Free();

            double seconds = 3.0 - (1.5 * spawnPercent);
            int effectHue = 2578;

            foreach (Point3D point in m_BlackHoleLocations)
            {
                Dictionary<int, Point3D> m_DarkblastComponents = new Dictionary<int, Point3D>();

                m_DarkblastComponents.Add(0x3083, new Point3D(point.X - 1, point.Y - 1, point.Z));
                m_DarkblastComponents.Add(0x3080, new Point3D(point.X - 1, point.Y, point.Z));
                m_DarkblastComponents.Add(0x3082, new Point3D(point.X, point.Y - 1, point.Z));
                m_DarkblastComponents.Add(0x3081, new Point3D(point.X + 1, point.Y - 1, point.Z));
                m_DarkblastComponents.Add(0x307D, new Point3D(point.X - 1, point.Y + 1, point.Z));
                m_DarkblastComponents.Add(0x307F, new Point3D(point.X, point.Y, point.Z));
                m_DarkblastComponents.Add(0x307E, new Point3D(point.X + 1, point.Y, point.Z));
                m_DarkblastComponents.Add(0x307C, new Point3D(point.X, point.Y + 1, point.Z));
                m_DarkblastComponents.Add(0x307B, new Point3D(point.X + 1, point.Y + 1, point.Z));

                foreach (KeyValuePair<int, Point3D> keyPairValue in m_DarkblastComponents)
                {
                    TimedStatic darkblastComponent = new TimedStatic(keyPairValue.Key, seconds);
                    darkblastComponent.Name = "blackhole";
                    darkblastComponent.Hue = effectHue;
                    darkblastComponent.MoveToWorld(keyPairValue.Value, map);
                }

                Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.5)), 14202, 10, 30, effectHue, 0, 5029, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(seconds), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    AbilityInProgress = false;

                    foreach (KeyValuePair<int, Point3D> keyPairValue in m_DarkblastComponents)
                    {
                        Point3D explosionLocation = keyPairValue.Value;
                        Effects.SendLocationParticles(EffectItem.Create(explosionLocation, map, TimeSpan.FromSeconds(0.5)), 0x3709, 10, 30, effectHue, 0, 5029, 0);                        
                    }

                    Effects.PlaySound(location, map, 0x56E);

                    m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(point, 1);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (!map.InLOS(point, mobile.Location)) continue;

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        int minDamage = 20;
                        int maxDamage = 40;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        if (mobile is BaseCreature)
                            damage *= 2;
                        
                        Effects.PlaySound(point, map, 0x20A);

                        SpecialAbilities.KnockbackSpecialAbility(1.0, point, null, mobile, 0, 15, -1, "", "");

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);                       
                        AOS.Damage(mobile, null, (int)damage, 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public void Firewalls()
        {
             if (!SpecialAbilities.Exists(this))
                return;

             for (int a = 0; a < m_Firewalls.Count; ++a)
             {
                 if (m_Firewalls[a] != null)
                 {
                     if (!m_Firewalls[a].Deleted)
                         m_Firewalls[a].Delete();
                 }
             }

             m_Firewalls.Clear();
             m_FireWallsTargets.Clear();

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            CreateFlashText(Location, Map, "*engages firewalls*", FirewallTextHue);

            Animate(10, 8, 1, true, false, 0);

            Point3D location = Location;
            Map map = Map;

            int range = 24;

            List<Item> m_NorthItems = new List<Item>();
            List<Item> m_SouthItems = new List<Item>();
            List<Item> m_WestItems = new List<Item>();
            List<Item> m_EastItems = new List<Item>();

            double totalDelay = 0.5 + ((double)range * .04);
            
            for (int a = 1; a < range - 1; a++)
            {
                for (int b = 0; b < 4; b++)
                {
                    TimedStatic firewall = new TimedStatic(0x3996, 60);
                    firewall.Name = "firewall";
                    firewall.Hue = 2587;

                    Point3D newLocation = location;

                    Direction firewallDirection = Direction.North;
                    
                    switch (b)
                    {
                        //North
                        case 0:
                            newLocation = new Point3D(location.X, location.Y - a, location.Z);
                            firewallDirection = Direction.North;
                        break;

                        //South
                        case 1:
                            newLocation = new Point3D(location.X, location.Y + a, location.Z);
                            firewallDirection = Direction.South;
                        break;

                        //West
                        case 2:
                            firewall.ItemID = 0x398C;

                            newLocation = new Point3D(location.X - a, location.Y, location.Z);
                            firewallDirection = Direction.West;
                        break;

                        //East
                        case 3:
                            firewall.ItemID = 0x398C;

                            newLocation = new Point3D(location.X + a, location.Y, location.Z);
                            firewallDirection = Direction.East;
                        break;
                    }

                    double creationDelay = Utility.GetDistance(location, newLocation) * .04;

                    Timer.DelayCall(TimeSpan.FromSeconds(creationDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) return;
                        if (firewall == null) return;
                        if (firewall.Deleted) return;

                        switch (firewallDirection)
                        {
                            case Server.Direction.North: m_NorthItems.Add(firewall); break;
                            case Server.Direction.South: m_SouthItems.Add(firewall); break;
                            case Server.Direction.West: m_WestItems.Add(firewall); break;
                            case Server.Direction.East: m_EastItems.Add(firewall); break;
                        }

                        m_Firewalls.Add(firewall);

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);
                        firewall.MoveToWorld(newLocation, map);

                        FirewallActivate(newLocation, map);
                    });                   
                }               
            }

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                bool clockwise = true;

                if (Utility.RandomDouble() <= .5)
                    clockwise = false;

                double intervalSpeed = .3 - (.2 * spawnPercent);

                int travelRange = 30;

                for (int a = 0; a < travelRange; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * intervalSpeed), delegate
                    {                        
                        Queue m_Queue = new Queue();

                        //North
                        foreach (Item item in m_NorthItems)
                        {
                            if (item == null) continue;
                            if (item.Deleted) continue;

                            Point3D oldLocation = item.Location;
                            Point3D newLocation;

                            if (clockwise)                            
                                newLocation = new Point3D(oldLocation.X + 1, oldLocation.Y, oldLocation.Z);                               
                            else
                                newLocation = new Point3D(oldLocation.X - 1, oldLocation.Y, oldLocation.Z);

                            bool canFit = SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            if (canFit && map.InLOS(oldLocation, newLocation))                            
                                item.Location = newLocation;                            

                            else
                                m_Queue.Enqueue(item);
                        }

                        while (m_Queue.Count > 0)
                        {
                            Item item = (Item)m_Queue.Dequeue();

                            m_Firewalls.Remove(item);
                            m_NorthItems.Remove(item);

                            item.Delete();
                        }

                        //South
                        foreach (Item item in m_SouthItems)
                        {
                            if (item == null) continue;
                            if (item.Deleted) continue;

                            Point3D oldLocation = item.Location;
                            Point3D newLocation;

                            if (clockwise)
                                newLocation = new Point3D(oldLocation.X - 1, oldLocation.Y, oldLocation.Z);
                            else
                                newLocation = new Point3D(oldLocation.X + 1, oldLocation.Y, oldLocation.Z);

                            bool canFit = SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            if (canFit && map.InLOS(oldLocation, newLocation))
                                item.Location = newLocation;

                            else
                                m_Queue.Enqueue(item);
                        }

                        while (m_Queue.Count > 0)
                        {
                            Item item = (Item)m_Queue.Dequeue();

                            m_Firewalls.Remove(item);
                            m_SouthItems.Remove(item);

                            item.Delete();
                        }

                        //West
                        foreach (Item item in m_WestItems)
                        {
                            if (item == null) continue;
                            if (item.Deleted) continue;

                            Point3D oldLocation = item.Location;
                            Point3D newLocation;

                            if (clockwise)
                                newLocation = new Point3D(oldLocation.X, oldLocation.Y - 1, oldLocation.Z);
                            else
                                newLocation = new Point3D(oldLocation.X, oldLocation.Y + 1, oldLocation.Z);

                            bool canFit = SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            if (canFit && map.InLOS(oldLocation, newLocation))
                                item.Location = newLocation;

                            else
                                m_Queue.Enqueue(item);
                        }

                        while (m_Queue.Count > 0)
                        {
                            Item item = (Item)m_Queue.Dequeue();

                            m_Firewalls.Remove(item);
                            m_WestItems.Remove(item);

                            item.Delete();
                        }

                        //East
                        foreach (Item item in m_EastItems)
                        {
                            if (item == null) continue;
                            if (item.Deleted) continue;

                            Point3D oldLocation = item.Location;
                            Point3D newLocation;

                            if (clockwise)
                                newLocation = new Point3D(oldLocation.X, oldLocation.Y + 1, oldLocation.Z);
                            else
                                newLocation = new Point3D(oldLocation.X, oldLocation.Y - 1, oldLocation.Z);

                            bool canFit = SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            if (canFit && map.InLOS(oldLocation, newLocation))
                                item.Location = newLocation;

                            else
                                m_Queue.Enqueue(item);
                        }

                        while (m_Queue.Count > 0)
                        {
                            Item item = (Item)m_Queue.Dequeue();

                            m_Firewalls.Remove(item);
                            m_EastItems.Remove(item);

                            item.Delete();
                        }

                        foreach (Item item in m_Firewalls)
                        {
                            if (item == null) continue;
                            if (item.Deleted) continue;

                            FirewallActivate(item.Location, item.Map);
                        }
                    });                   
                }

                AbilityInProgress = false;
            });            
        }

        public void FirewallActivate(Point3D location, Map map)
        {
            if (!SpecialAbilities.Exists(this))
                return;

            Queue m_Queue = new Queue();

            IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, 0);

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
        
                double spawnPercent = (double)intervalCount / (double)totalIntervals;

                double damage = ((double)DamageMax * .5) + ((double)DamageMax * .5 * spawnPercent);

                if (mobile is BaseCreature)
                    damage *= 2;
                
                if (m_FireWallsTargets.Contains(mobile))
                    damage *= .5;
                else
                    m_FireWallsTargets.Add(mobile);                

                mobile.PlaySound(0x5c3);
               
                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);

                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                AOS.Damage(mobile, null, (int)damage, 100, 0, 0, 0, 0);
            }
        }

        #endregion

        protected override bool OnMove(Direction d)
        {
            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            foreach (Mobile mobile in m_EngulfedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                mobile.Squelched = false;
                mobile.Hidden = false;      

                Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x101));
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }

            for (int a = 0; a < m_Firewalls.Count; ++a)
            {
                if (m_Firewalls[a] != null)
                {
                    if (!m_Firewalls[a].Deleted)
                        m_Firewalls[a].Delete();
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
                    if (!m_Creatures[a].Deleted)
                        m_Creatures[a].Delete();
                }
            }

            foreach (Mobile mobile in m_EngulfedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                mobile.Squelched = false;
                mobile.Hidden = false;

                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2587;
                ichor.Name = "trace";
                ichor.MoveToWorld(mobile.Location, mobile.Map);

                Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x101));
            }

            for (int a = 0; a < m_Firewalls.Count; ++a)
            {
                if (m_Firewalls[a] != null)
                {
                    if (!m_Firewalls[a].Deleted)
                        m_Firewalls[a].Delete();
                }
            }
        }

        public override int GetAngerSound() { return 0x27F; }
        public override int GetIdleSound() { return 0x280; }
        public override int GetAttackSound() { return 0x281; }
        public override int GetHurtSound() { return 0x282; }
        public override int GetDeathSound() { return 0x283; }
        
        public DeDOS(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            //Version 1
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(m_KynDragonSpawned);
            writer.Write(m_KynDragonKilled);

            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }

            writer.Write(m_EngulfedMobiles.Count);

            for (int a = 0; a < m_EngulfedMobiles.Count; a++)
            {
                writer.Write(m_EngulfedMobiles[a]);
            }

            writer.Write(m_Firewalls.Count);

            for (int a = 0; a < m_Firewalls.Count; a++)
            {
                writer.Write(m_Firewalls[a]);
            }           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 1
            if (version >= 1)
            {
                damageProgress = reader.ReadInt();
                intervalCount = reader.ReadInt();
                m_KynDragonSpawned = reader.ReadBool();
                m_KynDragonKilled = reader.ReadBool();
            }

            int creaturesCount = reader.ReadInt();
            for (int a = 0; a < creaturesCount; a++)
            {
                Mobile creature = reader.ReadMobile();

                if (creature != null)
                    m_Creatures.Add(creature);
            } 

            int engulfedMobiles = reader.ReadInt();

            for (int a = 0; a < engulfedMobiles; a++)
            {
                m_EngulfedMobiles.Add((Mobile)reader.ReadMobile());
            }

            int firewalls = reader.ReadInt();

            for (int a = 0; a < firewalls; a++)
            {
                Item firewall = (Item)reader.ReadItem();

                if (firewall != null)
                {
                    if (!firewall.Deleted)
                        firewall.Delete();
                }
            }
        }
	}
}
