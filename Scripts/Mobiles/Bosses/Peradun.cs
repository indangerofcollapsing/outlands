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
    [CorpseName("peradun the overseer's corpse")]
    public class Peradun : BaseCreature
    {
        public override string TitleReward { get { return "Slayer of Peradun"; } }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextFlameSlamAllowed;
        public TimeSpan NextFlameSlamDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextFireBarrageAllowed;
        public TimeSpan NextFireBarrageDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextLavaOrbAllowed;
        public TimeSpan NextLavaOrbDelay = TimeSpan.FromSeconds(60);

        public DateTime m_NextAbilityAllowed;
        public double NextAbilityDelayMin = 10;
        public double NextAbilityDelayMax = 5;

        public int LavaDuration = 600;

        public List<Item> m_Firewalls = new List<Item>();
        public List<Mobile> m_FireWallsTargets = new List<Mobile>();

        public List<LavaOrb> m_LavaOrbs = new List<LavaOrb>();

        public int damageIntervalThreshold = 1000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 25;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_Creatures = new List<Mobile>();

        public string[] idleSpeech { get { return new string[] {"*rages*"}; } }
        public string[] combatSpeech { get  { return new string[] {""}; } }

        [Constructable]
        public Peradun(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Peradun the Overseer";

            Body = 1071;
            Hue = 1358;
            BaseSoundID = 357;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(25000);
            SetStam(25000);
            SetStam(25000);

            SetDamage(20, 40);            

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 200);

            SetSkill(SkillName.MagicResist, 100);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 50;
        }

        public virtual int AttackRange { get { return 2; } }

        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "Peradun the Overseer has arisen and stirs within Hythloth Dungeon..."; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 5; } }
        public override int AttackFrames { get { return 6; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 7; } }

        public override int IdleAnimation { get { return 26; } }
        public override int IdleFrames { get { return 10; } }
        
        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 1.5;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
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

                    if (intervalCount % 4 == 0)
                    {
                        int creatureCount = (int)(Math.Round(2 + (3 * spawnPercent)));
                        SpawnCreatures(typeof(LavaElemental), creatureCount);
                    }

                    else
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: Hellfire(); break;
                            case 2: Firewalls(); break;                          
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

        //Normal Abilities

        #region Flameslam

        public void FlameSlam()
        {
            if (!SpecialAbilities.Exists(Combatant)) return;
            if (!SpecialAbilities.MonsterCanDamage(this, Combatant)) return;

            Point3D targetLocation = Combatant.Location;
            Map targetMap = Combatant.Map;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double initialDelay = 1 - (.5 * spawnPercent);
            double totalDelay = 1 + initialDelay;

            Animate(4, 10, 1, true, false, 0);
            PlaySound(GetAngerSound());            

            PublicOverheadMessage(MessageType.Regular, 0, false, "*raises a fiery fist*");

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextFlameSlamAllowed = DateTime.UtcNow + NextFlameSlamDelay + TimeSpan.FromSeconds(totalDelay);
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
                if (!SpecialAbilities.Exists(this))
                    return;

                PlaySound(0x591);  

                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(targetLocation, 1);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();               

                Point3D firefieldLocation = targetLocation;             

                int itemHue = 2075;

                int interval = 1;
                int duration = 2;
                int minDamage = 3;
                int maxDamage = 5;

                SingleFireField singleFireField = new SingleFireField(null, itemHue, interval, duration, minDamage, maxDamage, false, false, true, -1, true);
                singleFireField.ItemID = 14742;
                singleFireField.MoveToWorld(firefieldLocation, targetMap);

                singleFireField = new SingleFireField(null, itemHue, interval, duration, minDamage, maxDamage, false, false, true, -1, true);
                singleFireField.ItemID = 14732;
                singleFireField.MoveToWorld(firefieldLocation, targetMap);

                firefieldLocation.X--;

                singleFireField = new SingleFireField(null, itemHue, interval, duration, minDamage, maxDamage, false, false, true, -1, true);
                singleFireField.ItemID = 14732;
                singleFireField.MoveToWorld(firefieldLocation, targetMap);

                firefieldLocation.X++;
                firefieldLocation.Y--;

                singleFireField = new SingleFireField(null, itemHue, interval, duration, minDamage, maxDamage, false, false, true, -1, true);
                singleFireField.ItemID = 14742;
                singleFireField.MoveToWorld(firefieldLocation, targetMap);

                Point3D explosionLocation = targetLocation;

                explosionLocation.X--;
                explosionLocation.Y--;

                TimedStatic fireslam = new TimedStatic(14013, 1);
                fireslam.Name = "";
                fireslam.Hue = 2075;
                fireslam.MoveToWorld(explosionLocation, targetMap);

                explosionLocation.X += 2;

                fireslam = new TimedStatic(14013, 1);
                fireslam.Name = "";
                fireslam.Hue = 2075;
                fireslam.MoveToWorld(explosionLocation, targetMap);

                explosionLocation.Y += 2;

                fireslam = new TimedStatic(14013, 1);
                fireslam.Name = "";
                fireslam.Hue = 2075;
                fireslam.MoveToWorld(explosionLocation, targetMap);

                explosionLocation.X -= 2;

                fireslam = new TimedStatic(14013, 1);
                fireslam.Name = "";
                fireslam.Hue = 2075;
                fireslam.MoveToWorld(explosionLocation, targetMap);

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax));

                    if (mobile is BaseCreature)
                        damage *= 1.5;

                    int finalDamage = (int)(Math.Ceiling(damage));

                    DoHarmful(mobile);

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);                    
                }
            });
        }

        #endregion

        #region Fire Barrage

        public void FireBarrage()
        {
            if (!SpecialAbilities.Exists(this))
                return;         

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Combatant = null;

            int range = 20;
            
            int fireballs = 1;

            double directionDelay = 0;
            double initialDelay = 1 - (.5 * spawnPercent);
            double fireballDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)fireballs * fireballDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextFireBarrageAllowed = DateTime.UtcNow + NextFireBarrageDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes rage*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(27, 12, 1, true, false, 0);
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
                            
                            int mobileCount = 0;

                            int effectSound = 0x4D0;
                            int itemID = 18150;
                            int itemHue = 1257;

                            int impactSound = 0x591;
                            int impactItemId = 0x3709;
                            int impactHue = 2074;

                            List<Mobile> m_ValidMobiles = new List<Mobile>();

                            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);                           

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

                            foreach(Mobile mobile in m_ValidMobiles)
                            {
                                Point3D targetLocation = mobile.Location;
                                Map targetMap = mobile.Map;

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 25), map);

                                Point3D adjustedLocation = new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z);
                                adjustedLocation.Z = targetMap.GetSurfaceZ(adjustedLocation, 30);

                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 5), targetMap);

                                Effects.PlaySound(location, map, effectSound);

                                int particleSpeed = (int)Math.Round(6 + (6 * spawnPercent));

                                Effects.SendMovingEffect(startLocation, endLocation, itemID, particleSpeed, 0, false, false, itemHue, 0);
                                
                                double targetDistance = Utility.GetDistanceToSqrt(location, adjustedLocation);
                                double destinationDelay = (double)targetDistance * (.08 - (.04 * spawnPercent));

                                double damageScalar = 1.0;
                                double distanceScalar = (targetDistance / 20) * .5;

                                if (distanceScalar > .5)
                                    distanceScalar = .5;

                                if (distanceScalar < 0)
                                    distanceScalar = 0;

                                damageScalar += distanceScalar;
                                
                                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                                {
                                    Effects.PlaySound(adjustedLocation, targetMap, impactSound);
                                    Effects.SendLocationParticles(EffectItem.Create(adjustedLocation, targetMap, EffectItem.DefaultDuration), impactItemId, 20, 20, impactHue, 0, 0, 0);

                                    Queue m_Queue = new Queue();

                                    nearbyMobiles = targetMap.GetMobilesInRange(adjustedLocation, 0);

                                    foreach (Mobile mobileTarget in nearbyMobiles)
                                    {
                                        if (mobileTarget == this) continue;
                                        if (!SpecialAbilities.MonsterCanDamage(this, mobileTarget)) continue;

                                        m_Queue.Enqueue(mobile);
                                    }

                                    nearbyMobiles.Free();

                                    while (m_Queue.Count > 0)
                                    {
                                        Mobile mobileTarget = (Mobile)m_Queue.Dequeue();

                                        double damage = (double)DamageMin;

                                        if (mobileTarget is BaseCreature)
                                            damage *= 2;

                                        damage *= damageScalar;

                                        int finalDamage = (int)(Math.Ceiling(damage));

                                        DoHarmful(mobile);

                                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                                        AOS.Damage(mobileTarget, this, finalDamage, 100, 0, 0, 0, 0);
                                    }
                                });
                            }
                        });
                    }
                });
            });
        }

        #endregion

        #region Lava Orb

        public void LavaOrb()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loops = 4;

            if (spawnPercent > .25)
                loops = 3;

            if (spawnPercent > .50)
                loops = 2;

            if (spawnPercent > .75)
                loops = 1;

            double initialDelay = 1.0;
            double totalDelay = initialDelay + loops;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextLavaOrbAllowed = DateTime.UtcNow + NextLavaOrbDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Animate(23, 10, 1, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*begins summoning ritual*");

            Point3D location = Location;
            Map map = Map;
            
            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Animate(23, 10, 1, true, false, 0);

                        PlaySound(GetAngerSound());                        
                    });

                    for (int b = 0; b < 3; b++)
                    {
                        double runeDelay = a * b * .33;

                        Timer.DelayCall(TimeSpan.FromSeconds(runeDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;                            

                            Point3D newLocation = new Point3D(location.X + Utility.RandomList(-2, -1, 1, 2), location.Y + Utility.RandomList(-2, -1, 1, 2), location.Z);
                            newLocation.Z = map.GetSurfaceZ(newLocation, 30);

                            Effects.PlaySound(newLocation, map, 0x56B);

                            TimedStatic rune = new TimedStatic(Utility.RandomList(3685, 3688, 3682, 3676, 3679), totalDelay - runeDelay);
                            rune.Name = "summoning rune";
                            rune.Hue = 1358;
                            rune.MoveToWorld(newLocation, map);
                        });
                    }                    
                }
            });

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;               

                int range = 20;

                List<Mobile> m_NearbyMobiles = new List<Mobile>();

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);

                int mobileCount = 0;

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                    if (!Map.InLOS(Location, mobile.Location)) continue;
                    if (mobile.Hidden) continue;

                    m_NearbyMobiles.Add(mobile);
                }

                nearbyMobiles.Free();

                Point3D targetLocation = Location;
                Map targetMap = Map;

                if (m_NearbyMobiles.Count > 0)
                {
                    Mobile mobileTarget = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];
                    targetLocation = mobileTarget.Location;
                }

                int particleSpeed = 8;
                double impactDelay = .4;

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 50), targetMap);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X + 1, targetLocation.Y, targetLocation.Z), targetMap);

                int itemId = 13935;
                int itemHue = 1357;

                Effects.SendMovingParticles(startLocation, endLocation, itemId, particleSpeed, 0, false, false, itemHue, 0, 9501, 0, 0, 0x100);

                Effects.PlaySound(targetLocation, targetMap, 0x5d3);

                Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    int orbRadius = (int)Math.Round(2 + (4 * spawnPercent));

                    LavaOrb lavaOrb = new LavaOrb();

                    lavaOrb.Radius = orbRadius;
                    lavaOrb.m_Owner = this;
                    lavaOrb.MoveToWorld(targetLocation, targetMap);

                    m_LavaOrbs.Add(lavaOrb);

                    Effects.PlaySound(targetLocation, targetMap, 0x11d);

                    Queue m_Queue = new Queue();

                    nearbyMobiles = targetMap.GetMobilesInRange(targetLocation, 0);

                    foreach (Mobile mobileTarget in nearbyMobiles)
                    {
                        if (mobileTarget == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobileTarget)) continue;

                        m_Queue.Enqueue(mobileTarget);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobileTarget = (Mobile)m_Queue.Dequeue();

                        double damage = (double)DamageMin;

                        if (mobileTarget is BaseCreature)
                            damage *= 2;

                        int finalDamage = (int)(Math.Ceiling(damage));

                        DoHarmful(mobileTarget);

                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, this, finalDamage, 100, 0, 0, 0, 0);
                    }

                    for (int a = 0; a < 30; a++)
                    {
                        Point3D newLocation = new Point3D(targetLocation.X + Utility.RandomMinMax(-3, 3), targetLocation.Y + Utility.RandomMinMax(-3, 3), targetLocation.Z);
                        newLocation.Z = targetMap.GetSurfaceZ(targetLocation, 30);

                        TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 14);
                        floorCrack.Name = "floor crack";
                        floorCrack.MoveToWorld(newLocation, targetMap);
                    }

                    int radius = 3;
                    int minRange = radius * -1;
                    int maxRange = radius;

                    for (int a = minRange; a < maxRange + 1; a++)
                    {
                        for (int b = minRange; b < maxRange + 1; b++)
                        {
                            Point3D newLocation = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);
                            newLocation.Z = targetMap.GetSurfaceZ(targetLocation, 30);

                            if (Utility.RandomDouble() <= .5)
                            {
                                TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 14);
                                floorCrack.Name = "floor crack";
                                floorCrack.MoveToWorld(newLocation, targetMap);
                            }

                            else
                            {
                                int dustHue = 2074;
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, targetMap, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, dustHue, 0, 5029, 0);
                            }
                        }
                    }
                });
            });
        }

        #endregion

        //Epic Abilities 

        #region Hellfire

        public void Hellfire()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loops = (int)(Math.Round(4 + (4 * spawnPercent)));

            double totalDelay = loops + 1.0;

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay() + TimeSpan.FromSeconds(.5);

            Animate(23, 10, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*calls down hellfire*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Animate(23, 10, 1, true, false, 0);

                        PlaySound(GetAngerSound());
                        Effects.PlaySound(location, map, 0x5CF);
                    });
                }

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        int hellfireRange = 25;

                        List<Point3D> m_Locations = new List<Point3D>();

                        IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, hellfireRange);

                        int mobileCount = 0;

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (mobile == this) continue;
                            if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;                            
                            if (mobile.Hidden) continue;

                            if (!m_Locations.Contains(mobile.Location))
                                m_Locations.Add(mobile.Location);
                        }

                        nearbyMobiles.Free();

                        int particleSpeed = 8 + (int)(Math.Round(8 * (double)spawnPercent));
                        double impactDelay = .6 - (.3 * spawnPercent);
                        
                        foreach (Point3D point in m_Locations)
                        {
                            IEntity startLocation = new Entity(Serial.Zero, new Point3D(point.X - 1, point.Y - 1, point.Z + 50), map);
                            IEntity endLocation = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 5), map);
                            
                            Effects.SendMovingParticles(startLocation, endLocation, 0x36D4, particleSpeed, 0, false, false, 0, 0, 9501, 0, 0, 0x100);

                            Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                double firefieldChance = 1.0;

                                if (Utility.RandomDouble() <= firefieldChance)
                                {
                                    LavaField lavaField = new LavaField(this, 0, 1, LavaDuration, 3, 5, false, false, true, -1, true);
                                    lavaField.MoveToWorld(point, map);
                                }

                                Effects.PlaySound(endLocation, map, 0x56E);
                                Effects.SendLocationParticles(endLocation, 0x3709, 10, 20, 2074, 0, 5029, 0);
                                
                                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(point, 0);

                                Queue m_Queue = new Queue();

                                foreach (Mobile mobile in mobilesOnTile)
                                {
                                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                        continue;

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

                                    if (validTarget)
                                        m_Queue.Enqueue(mobile);
                                }

                                mobilesOnTile.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    int minDamage = (int)(Math.Ceiling((double)DamageMax / 4));
                                    int maxDamage = (int)(Math.Ceiling((double)DamageMax / 2));

                                    double damage = (double)(Utility.RandomMinMax(minDamage, maxDamage));

                                    if (mobile is BaseCreature)
                                        damage *= 2;

                                    DoHarmful(mobile);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, (int)damage, 100, 0, 0, 0, 0);
                                }
                            });                            
                        }
                    });
                }
            });
        }

        #endregion

        #region Firewalls

        public void Firewalls()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 24;
            double totalDelay = 0.5 + ((double)range * .04);

            Combatant = null;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay() + TimeSpan.FromSeconds(.5);
            
            Animate(27, 12, 1, true, false, 0);            

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*begins to harness flames*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
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

                List<Item> m_NorthItems = new List<Item>();
                List<Item> m_SouthItems = new List<Item>();
                List<Item> m_WestItems = new List<Item>();
                List<Item> m_EastItems = new List<Item>();

                for (int a = 1; a < range - 1; a++)
                {
                    for (int b = 0; b < 4; b++)
                    {
                        TimedStatic firewall = new TimedStatic(0x3996, 60);
                        firewall.Name = "flames";
                        firewall.Hue = 2075;

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

                            int adjustedZ = map.GetSurfaceZ(newLocation, 30);
                            newLocation.Z = adjustedZ;
                            
                            bool canFit = true;
                            bool inLOS = true; //map.InLOS(location, newLocation);

                            if (canFit && inLOS)
                            {
                                switch (firewallDirection)
                                {
                                    case Server.Direction.North: m_NorthItems.Add(firewall); break;
                                    case Server.Direction.South: m_SouthItems.Add(firewall); break;
                                    case Server.Direction.West: m_WestItems.Add(firewall); break;
                                    case Server.Direction.East: m_EastItems.Add(firewall); break;
                                }

                                firewall.MoveToWorld(newLocation, map);

                                m_Firewalls.Add(firewall);
                                FirewallActivate(newLocation, map);
                            }

                            else
                                firewall.Delete();
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

                                int adjustedZ = map.GetSurfaceZ(newLocation, 30);
                                newLocation.Z = adjustedZ;

                                bool canFit = true;

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

                                int adjustedZ = map.GetSurfaceZ(newLocation, 30);
                                newLocation.Z = adjustedZ;

                                bool canFit = true; //SpellHelper.AdjustField(ref newLocation, map, 12, false);

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

                                int adjustedZ = map.GetSurfaceZ(newLocation, 30);
                                newLocation.Z = adjustedZ;

                                bool canFit = true; //SpellHelper.AdjustField(ref newLocation, map, 12, false);

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

                                int adjustedZ = map.GetSurfaceZ(newLocation, 30);
                                newLocation.Z = adjustedZ;

                                bool canFit = true; //SpellHelper.AdjustField(ref newLocation, map, 12, false);

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
                });  
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

                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);

                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                AOS.Damage(mobile, null, (int)damage, 100, 0, 0, 0, 0);
            }
        }

        #endregion

        #region Spawn Creatures

        public void SpawnCreatures(Type type, int creatures)
        {
            for (int a = 0; a < creatures; a++)
            {
                Point3D creatureLocation = Location;
                Map creatureMap = Map;

                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                if (m_ValidLocations.Count > 0)
                    creatureLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                Effects.PlaySound(Location, Map, 0x5CE);
                Effects.SendLocationParticles(EffectItem.Create(creatureLocation, creatureMap, TimeSpan.FromSeconds(0.5)), 6899, 10, 30, 2074, 0, 5029, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (!SpecialAbilities.Exists(this)) 
                        return;

                    Effects.SendLocationParticles(EffectItem.Create(creatureLocation, creatureMap, TimeSpan.FromSeconds(0.5)), 3546, 10, 75, 2074, 0, 5029, 0);

                    BaseCreature creature = (BaseCreature)Activator.CreateInstance(type);

                    if (creature == null)
                        return;

                    Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        creature.MoveToWorld(creatureLocation, creatureMap);
                        creature.PlaySound(creature.GetAngerSound());

                        m_Creatures.Add(creature);
                    });
                });
            }
        }

        #endregion

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
                switch (Utility.RandomMinMax(1, 3))
                {                    
                    case 1:
                        if (DateTime.UtcNow >= m_NextFlameSlamAllowed && Utility.GetDistance(Location, Combatant.Location) <= AttackRange)
                        {                           
                            FlameSlam();
                            return;
                        }
                    break;

                    case 2:
                        if (DateTime.UtcNow >= m_NextFireBarrageAllowed)
                        {
                            FireBarrage();                          
                            return;
                        }
                    break;

                    case 3:
                        if (DateTime.UtcNow >= m_NextLavaOrbAllowed)
                        {
                            LavaOrb();                    
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
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*steams*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*sneers*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*brandishes claws*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*bellows*");

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
            Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            if (Utility.RandomDouble() <= .33)
            {
                TimedStatic fire = new TimedStatic(0xDE3, 1);
                fire.Name = "fire";
                fire.Hue = 2075;
                fire.MoveToWorld(Location, Map);                
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

        public Peradun(Serial serial): base(serial)
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

            writer.Write(m_Firewalls.Count);
            for (int a = 0; a < m_Firewalls.Count; a++)
            {
                writer.Write(m_Firewalls[a]);
            } 

            writer.Write(m_LavaOrbs.Count);
            for (int a = 0; a < m_LavaOrbs.Count; a++)
            {
                writer.Write(m_LavaOrbs[a]);
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

            //Version 0
            if (version >= 0)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    m_Creatures.Add(creature);
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

                int lavaOrbsCount = reader.ReadInt();
                for (int a = 0; a < lavaOrbsCount; a++)
                {
                    LavaOrb lavaOrb = (LavaOrb)reader.ReadItem();

                    if (lavaOrb != null)
                    {
                        if (!lavaOrb.Deleted)
                            m_LavaOrbs.Add(lavaOrb);
                    }
                }
            }
        }
    }
}

//Fire Sound: 0x1DD, 0x208, 0x225, 0x226, 0x227, 0x345, 0x346, 0x347, 0x348, 0x349, 0x34A                            
//More Fire: 0x356, 0x357, 0x358, 0x359, 0x44B, 0x4B9, 0x4BA, 0x4BB, 0x5AC, 0x5AE
//Misc Fire Sounds: 0x1F3, 0x15E, 0x15F, 0x160, 0x4D0, 0x56D, 0x58E, 0x591, 0x5CA,
//Fire: 0x5CF, 0x666
//Explosions: 0x11B, 0x11C, 0x11D, 0x11E, 0x305, 0x306, 0x307, 0x308, 0x309, 0x4CF
//More Explosions: 0x590
//Hiss: 0x1DE, 0x22F, 0x230, 0x231

//Lava Lizards, Fire Elementals

//Animations
//4 10 1 true false 0 (Swipe / Throw)
//5 6 1 true false 0 (Quick Swipe)
//10 7 1 true false 0 (Roar / Get Hit?)
//11 8 1 true false 0 (Pick Up Off Ground)
//12 10 1 true false 0 (Taunt)
//15 8 1 true false 0 (Quick Taunt / Get Hit?)
//23 10 1 true false 0 (Large Roar / Get Hit?)
//26 12 1 true false 0 (Special Idle)
//27 12 1 true false 0 (Large Taunt)





