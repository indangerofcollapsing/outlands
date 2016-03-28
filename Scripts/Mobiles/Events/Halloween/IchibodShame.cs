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

using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "ichibod shame's corpse" )]
	public class IchibodShame : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextChargeAllowed;
        public TimeSpan NextChargeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextPumpkinTossAllowed;
        public TimeSpan NextPumpkinTossDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextBouncingPumpkinAllowed;
        public TimeSpan NextBouncingPumpkinDelay = TimeSpan.FromSeconds(30);

        public DateTime m_BouncingPumpkinTimeout;
        public TimeSpan MaxBouncingBumpkinDuration = TimeSpan.FromSeconds(5);

        public DateTime m_ChargeTimeout;
        public TimeSpan MaxChargeDuration = TimeSpan.FromSeconds(10);

        public bool m_ChargeInProgress = false;
        
        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(5);

        public int damageIntervalThreshold = 1000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 40;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_Trampled = new List<Mobile>();
        public List<Mobile> m_BouncingPumpkinTargets = new List<Mobile>();

        public List<Mobile> m_Creatures = new List<Mobile>();

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public IchibodShame() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Ichibod Shame";

            Body = 400;
            Hue = 23999;

			SetStr(100);
			SetDex(100);
			SetInt(25);

			SetHits(40000);
            SetStam(40000);
            SetMana(0);

			SetDamage(20, 40);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);
            
			SetSkill(SkillName.MagicResist, 100);

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 150;

            Horse mount = new Horse();
            mount.Hue = 2076;
            mount.Rider = this;

            HairItemID = -1;

            AddItem(new PlateGorget() { Movable = false, Hue = 1763, Name = "Shame Dungeon Armor" });
            AddItem(new PlateChest() { Movable = false, Hue = 1763, Name = "Shame Dungeon Armor" });
            AddItem(new PlateArms() { Movable = false, Hue = 1763, Name = "Shame Dungeon Armor" });
            AddItem(new PlateLegs() { Movable = false, Hue = 1763, Name = "Shame Dungeon Armor" });
            AddItem(new PlateGloves() { Movable = false, Hue = 1763, Name = "Shame Dungeon Armor" });
            AddItem(new Cloak() { Movable = false, Hue = 2076 });

            AddItem(new Scythe() { Movable = false, Hue = 2500 });
		}

        public override bool MovementRestrictionImmune { get { return true; } }
        public override bool AlwaysBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.33;
            
            ActiveSpeed = 0.3;
            PassiveSpeed = 0.3;
            CurrentSpeed = 0.3;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            double bleedChance = .20 + +(.20 * spawnPercent);

            SpecialAbilities.BleedSpecialAbility(bleedChance, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!");
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
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    double spawnPercent = (double)intervalCount / (double)totalIntervals;

                    PumpkinBombs();
                }
            }

            base.OnDamage(amount, from, willKill);
        }       

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            CheckChargeResolved();            

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextChargeAllowed)
                        {
                            StartCharge();

                            return;
                        }
                    break;

                    case 2:
                        if (DateTime.UtcNow >= m_NextPumpkinTossAllowed)
                        {
                            PumpkinToss();

                            return;
                        }
                    break;

                    case 3:
                        if (DateTime.UtcNow >= m_NextBouncingPumpkinAllowed)
                        {
                            StartBouncingPumpkinThrow();

                            return;
                        }
                    break;
                }
            }
        }

        public void PumpkinBombs()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = 1;
            double bombDelay = 2 - (1.0 * spawnPercent);
            double totalDelay = 1 + directionDelay + initialDelay + bombDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
            
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            Effects.PlaySound(Location, Map, 0x666);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*readies an awful lot of pumpkins*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                int totalLoops = 1 + (int)(Math.Floor(totalDelay));

                for (int a = 1; a < totalLoops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .75), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        PlaySound(GetIdleSound());
                        Animate(29, 5, 1, true, false, 0);
                    });
                }

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    List<Point3D> m_Locations = new List<Point3D>();

                    int range = 20;

                    IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);

                    int mobileCount = 0;

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                        if (!m_Locations.Contains(mobile.Location))
                            m_Locations.Add(mobile.Location);
                    }

                    nearbyMobiles.Free();

                    foreach (Point3D point in m_Locations)
                    {
                        TimedStatic pumpkinBomb = new TimedStatic(Utility.RandomList(18065, 18069), bombDelay);
                        pumpkinBomb.Name = "pumpkin bomb";
                        pumpkinBomb.MoveToWorld(point, Map);
                    }
                    
                    Timer.DelayCall(TimeSpan.FromSeconds(bombDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        foreach (Point3D point in m_Locations)
                        {
                            TimedStatic pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                            pumpkinExplosion.Name = "smashed pumpkin";
                            pumpkinExplosion.Hue = 1359;
                            pumpkinExplosion.MoveToWorld(point, Map);

                            for (int a = 0; a < 8; a++)
                            {
                                Point3D newLocation = new Point3D(point.X + Utility.RandomList(-2, -1, 1, 2), point.Y + Utility.RandomList(-2, -1, 1, 2), point.Z);
                                SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                                pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                                pumpkinExplosion.Name = "smashed pumpkin";
                                pumpkinExplosion.Hue = 1359;
                                pumpkinExplosion.MoveToWorld(newLocation, Map);
                            }

                            int projectiles = 4;
                            int particleSpeed = 8;
                            double distanceDelayInterval = .12;

                            int minRadius = 1;
                            int maxRadius = 5;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(point, true, false, point, map, projectiles, 20, minRadius, maxRadius, false);

                            if (m_ValidLocations.Count == 0)
                                return;

                            for (int a = 0; a < projectiles; a++)
                            {
                                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 2), map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                                newLocation.Z += 5;

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(4650, 4651, 4653, 4655), particleSpeed, 0, false, false, 1358, 0);
                            }

                            Effects.PlaySound(point,Map, 0x306);
                            Effects.SendLocationEffect(point, Map, 0x36BD, 30, 10, 0, 0);

                            Queue m_Queue = new Queue();

                            IPooledEnumerable m_NearbyMobiles = map.GetMobilesInRange(point, 1);

                            foreach (Mobile mobile in m_NearbyMobiles)
                            {
                                if (mobile == this) continue;
                                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                            }

                            m_NearbyMobiles.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                int damage = DamageMax;

                                if (mobile is BaseCreature)
                                    damage = (int)((double)damage * 3);

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, damage, 100, 0, 0, 0, 0);
                            }
                        }
                    });
                });
            });

            /*

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDuration), delegate
            {
                if (creature == null) return;                
                if (creature.Deleted) return;
                if (!creature.Alive) return;                

                foreach (Point3D point in m_Locations)
                {
                    Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.25)), 8700, 10, 30, 0, 0, 5029, 0);

                    IPooledEnumerable spikePoint = map.GetMobilesInRange(point, 1);

                    bool hitMobile = false;

                    List<Mobile> m_MobilesHit = new List<Mobile>();

                    foreach (Mobile mobile in spikePoint)
                    {                        
                        if (mobile == creature) continue;
                        if (mobile.Location != point) continue;
                        if (mobile.Map != map) continue;
                        if (!mobile.Alive) continue;                        
                        if (!mobile.CanBeDamaged()) continue;
                        if (mobile.AccessLevel > AccessLevel.Player) continue;

                        bool validTarget = false;

                        PlayerMobile pm_Target = mobile as PlayerMobile;
                        BaseCreature bc_Target = mobile as BaseCreature;

                        if (pm_Target != null)
                            validTarget = true;

                        if (bc_Target != null)
                        {
                            if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                                validTarget = true;
                        }

                        if (!validTarget)
                            continue;

                        m_MobilesHit.Add(mobile);                        
                    }

                    spikePoint.Free();

                    foreach (Mobile mobile in m_MobilesHit)
                    {
                        int damage = DamageMin;

                        if (mobile is BaseCreature)
                            damage = (int)((double)damage * 2);

                        SpecialAbilities.PierceSpecialAbility(1.0, this, mobile, .50, 30, -1, true, "", "Bone shards pierce your armor, reducing it's effectiveness!");

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, damage, 100, 0, 0, 0, 0);
                    }

                    Effects.PlaySound(point, map, 0x11D);

                    for (int a = 0; a < 5; a++)
                    {
                        Blood dirt = new Blood();
                        dirt.Name = "bones";
                        dirt.ItemID = Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883);
                        Point3D dirtLocation = new Point3D(point.X + Utility.RandomList(-1, 1), point.Y + Utility.RandomList(-1, 1), point.Z);
                        dirt.MoveToWorld(dirtLocation, Map);
                    }

                    int projectiles = 10;
                    int particleSpeed = 8;
                    double distanceDelayInterval = .12;

                    int minRadius = 1;
                    int maxRadius = 5;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(point, true, false, point, map, projectiles, 20, minRadius, maxRadius, false);

                    if (m_ValidLocations.Count == 0)
                        return;

                    for (int a = 0; a < projectiles; a++)
                    {       
                        Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 2), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                        newLocation.Z += 5;

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                    }

                    IEntity locationEntity = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z - 1), map);
                    Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2497, 0, 5044, 0);
                }
                
                m_NextSkullSpikesAllowed = DateTime.UtcNow + NextSkullSpikesDelay;
                m_SummoningSkullSpikes = false;
                
                creature.Frozen = false;
                creature.CantWalk = false;
            });
            }
             * */
        }

        public void StartCharge()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            int chargeRange = 20;
            int minChangeDistance = 5;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, chargeRange);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;
                if (Utility.GetDistance(Location, mobile.Location) < minChangeDistance) continue;

                mobileCount++;
            }

            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, chargeRange);

            List<Mobile> m_NearbyMobiles = new List<Mobile>();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;
                if (Utility.GetDistance(Location, mobile.Location) < minChangeDistance) continue;
                if (Combatant != null)
                {
                    if (mobileCount > 1 && mobile == Combatant)
                        continue;
                }

                m_NearbyMobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            if (m_NearbyMobiles.Count == 0)
            {
                m_NextChargeAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                return;
            }

            Mobile mobileTarget = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];

            Combatant = mobileTarget;

            DelayNextCombatTime(NextChargeDelay.TotalSeconds);

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;
            m_ChargeTimeout = DateTime.UtcNow + MaxChargeDuration;

            m_ChargeInProgress = true;
            AbilityInProgress = true;
            
            DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 500;     
            
            Effects.PlaySound(Location, Map, 0x59B);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*charges*");

            m_Trampled.Clear();           
            
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            ActiveSpeed = .25 - (.05 * spawnPercent);
            PassiveSpeed = .25 - (.05 * spawnPercent);
            CurrentSpeed = .25 - (.05 * spawnPercent); 
           
            Paralyzed = false;
            CantWalk = false;
            Frozen = false;            
        }

        public void CheckChargeResolved()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!m_ChargeInProgress)
                return;

            bool chargeExpired = false;
            bool clearCharge = false;

            if (Combatant == null)
                chargeExpired = true;

            else if (!Combatant.Alive || Combatant.Hidden || Utility.GetDistance(Location, Combatant.Location) > 24 || DateTime.UtcNow > m_ChargeTimeout)
                chargeExpired = true;

            if (chargeExpired)            
                clearCharge = true;

            else if (SpecialAbilities.Exists(Combatant))
            {
                if (Utility.GetDistance(Location, Combatant.Location) <= 1)
                {
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*tramples opponent*");

                    Effects.PlaySound(Location, Map, 0x59C);
                    Effects.PlaySound(Combatant.Location, Combatant.Map, Combatant.GetHurtSound());

                    double damage = DamageMax;

                    if (Combatant is BaseCreature)
                        damage *= 1.5;

                    new Blood().MoveToWorld(Combatant.Location, Combatant.Map);
                    
                    SpecialAbilities.BleedSpecialAbility(1.0, this, Combatant, damage, 10.0, -1, true, "", "Ichibod's scythe carves you fiercely, causing you to bleed!");

                    AOS.Damage(Combatant, (int)damage, 100, 0, 0, 0, 0);

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

                    SpecialAbilities.HinderSpecialAbility(1.0, this, Combatant, 1.0, 1, false, -1, false, "", "You have been trampled and can't move!");

                    clearCharge = true;
                }
            }

            if (clearCharge)
                ClearCharge();
        }

        public void ClearCharge()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 5;

            ActiveSpeed = 0.3;
            PassiveSpeed = 0.3;
            CurrentSpeed = 0.3;

            m_ChargeInProgress = false;
            AbilityInProgress = false;

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

            m_Trampled.Clear();

            NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        public void PumpkinToss()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            int maxDistance = 20;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile == this) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;

                mobileCount++;
            }

            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, maxDistance);

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

            int maxExtraPumpkins = 10;
            int pumpkins = 10 + (int)Math.Ceiling(((double)maxExtraPumpkins * spawnPercent));

            double directionDelay = .25;
            double initialDelay = 1;
            double pumpkinDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)pumpkins * pumpkinDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextPumpkinTossAllowed = DateTime.UtcNow + NextPumpkinTossDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*readies several pumpkins*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                int totalLoops = 1 + (int)(Math.Floor(totalDelay));

                for (int a = 1; a < totalLoops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .75), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Animate(26, 5, 1, true, false, 0);
                    });
                }               

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    for (int a = 0; a < pumpkins; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * pumpkinDelay), delegate
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
                                if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= maxDistance)
                                    mobileTargetValid = false;
                            }

                            if (mobileTargetValid)
                            {
                                targetLocation = mobileTarget.Location;
                                targetMap = mobileTarget.Map;
                            }

                            int effectSound = Utility.RandomList(0x5D3, 0x5D2, 0x5A2, 0x580);
                            int itemID = Utility.RandomList(3178, 3179, 3180);
                            int itemHue = 0;

                            int impactSound = Utility.RandomList(0x5DE, 0x5DA, 0x5D8);
                            int impactHue = 0;

                            int xOffset = 0;
                            int yOffset = 0;

                            int distance = Utility.GetDistance(Location, targetLocation);

                            double damageScalar = 1;

                            if (distance >= 6)
                            {
                                if (Utility.RandomDouble() <= .5)                                
                                    xOffset = Utility.RandomMinMax(-3, 3);

                                if (Utility.RandomDouble() <= .5)       
                                    yOffset = Utility.RandomMinMax(-3, 3);                                
                            }

                            else if (distance >= 4)
                            {
                                if (Utility.RandomDouble() <= .5)      
                                    xOffset = Utility.RandomMinMax(-2, 2);

                                if (Utility.RandomDouble() <= .5)      
                                    yOffset = Utility.RandomMinMax(-2, 2);                                
                            }

                            else if (distance >= 2)
                            {
                                if (Utility.RandomDouble() <= .5)
                                    xOffset = Utility.RandomMinMax(-1, 1);

                                if (Utility.RandomDouble() <= .5)
                                    yOffset = Utility.RandomMinMax(-1, 1);
                            }

                            else                            
                                damageScalar = .5;                            

                            IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 15), map);

                            Point3D adjustedLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z);
                            SpellHelper.AdjustField(ref adjustedLocation, targetMap, 12, false);

                            IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 10), targetMap);

                            Effects.PlaySound(location, map, effectSound);
                            Effects.SendMovingEffect(startLocation, endLocation, itemID, 4, 0, false, false, itemHue, 0);

                            double targetDistance = Utility.GetDistanceToSqrt(location, adjustedLocation);
                            double destinationDelay = (double)targetDistance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                Effects.PlaySound(adjustedLocation, targetMap, impactSound);

                                TimedStatic pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                                pumpkinExplosion.Name = "smashed pumpkin";
                                pumpkinExplosion.Hue = 1359;
                                pumpkinExplosion.MoveToWorld(adjustedLocation, targetMap);

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

                                    double damage = (double)DamageMin / 2;

                                    if (mobile is BaseCreature)
                                        damage *= 2;

                                    damage *= damageScalar;

                                    DoHarmful(mobile);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, this, (int)damage, 0, 100, 0, 0, 0);
                                }
                            });
                        });
                    }
                });
            });
        }

        public void StartBouncingPumpkinThrow()
        {
            if (!SpecialAbilities.Exists(this))
                return;

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

            m_NextBouncingPumpkinAllowed = DateTime.UtcNow + NextBouncingPumpkinDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*readies bouncing pumpkin*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = closestTarget.Location;
            Map targetMap = closestTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(26, 5, 1, true, false, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    m_BouncingPumpkinTimeout = DateTime.UtcNow + MaxBouncingBumpkinDuration;
                    m_BouncingPumpkinTargets.Clear();

                    PlaySound(Utility.RandomList(0x5D3, 0x5D2));
                    
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

                    BouncingPumpkin(Location, closestTarget);
                });
            });
        }

        public void BouncingPumpkin(Point3D location, Mobile target)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(target)) return;

            if (DateTime.UtcNow > m_BouncingPumpkinTimeout)
                return;

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 10), Map);

            int itemId = 18065;
            int itemHue = 0; //2599

            double speedModifier = 1;

            Effects.SendMovingEffect(startLocation, endLocation, itemId, (int)(15 * speedModifier), 0, true, false, itemHue, 0);           

            double distance = Utility.GetDistanceToSqrt(location, target.Location);
            double destinationDelay = (double)distance * .04;

            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (!SpecialAbilities.Exists(target)) return;

                Point3D mobileLocation = target.Location;

                int damage = DamageMin;

                DoHarmful(target);
                m_BouncingPumpkinTargets.Add(target);

                target.PlaySound(0x4F4);

                TimedStatic pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                pumpkinExplosion.Name = "smashed pumpkin";
                pumpkinExplosion.Hue = 1359;
                pumpkinExplosion.MoveToWorld(target.Location, Map);
                
                new Blood().MoveToWorld(target.Location, target.Map);
                AOS.Damage(target, this, damage, 0, 100, 0, 0, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(.05), delegate
                {
                    if (!SpecialAbilities.Exists(target)) 
                        return;

                    SpecialAbilities.KnockbackSpecialAbility(1.0, location, null, target, 10, 10, 0, "", "");
                });

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
                    if (m_BouncingPumpkinTargets.Contains(mobile)) continue;

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

                BouncingPumpkin(mobileLocation, closestTarget);                 
            });        
        }

        protected override bool OnMove(Direction d)
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (m_ChargeInProgress)
            {
                m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;
                m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;               
                
                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 1);                

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (!Map.InLOS(Location, mobile.Location)) continue;
                    if (mobile == Combatant) continue;
                    if (m_Trampled.Contains(mobile)) continue;

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    m_Trampled.Add(mobile);

                    Effects.PlaySound(Location, Map, Utility.RandomList(0x3BB, 0x3BA, 0x3B9));
                    Effects.PlaySound(mobile.Location, mobile.Map, mobile.GetHurtSound());

                    double damage = DamageMin;

                    if (Combatant is BaseCreature)
                        damage *= 1.5;

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    SpecialAbilities.BleedSpecialAbility(1.0, this, mobile, damage, 10.0, -1, true, "", "Ichibod's scythe grazes you, causing you to bleed!");

                    AOS.Damage(mobile, (int)damage, 100, 0, 0, 0, 0);

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

                    SpecialAbilities.HinderSpecialAbility(1.0, this, mobile, 1.0, 1, false, -1, false, "", "You have been trampled and can't move!");
                }                

                CheckChargeResolved();
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x12E, 0x12D));

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if (mount != null)
                mount.Rider = null;

            if (mount is Mobile)
                ((Mobile)mount).Kill();

            for (int a = 0; a < 50; a++)
            {
                Blood blood = new Blood();               
                Point3D bloodLocation = new Point3D(Location.X + Utility.RandomMinMax(-4, 4), Location.Y + Utility.RandomMinMax(-4, 4), Location.Z + 2);

                blood.MoveToWorld(bloodLocation, Map);
            }

            Effects.PlaySound(Location, Map, 0x5DD);

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
                        
            //c.DropItem(DungeonArmor.CreateDungeonArmor(DungeonEnum.Shame, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified));
            //c.DropItem(DungeonArmor.CreateDungeonArmor(DungeonEnum.Shame, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified));        
        }

        public override int GetAngerSound() { return 0x2A9; }
        public override int GetIdleSound() { return 0x598; }
        //public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x5FA; }
        public override int GetDeathSound() { return 0x2AB; }

        public IchibodShame(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            //Version 1
            writer.Write(damageProgress);
            writer.Write(intervalCount);
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
            }
        }
	}
}
