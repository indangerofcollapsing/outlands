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
    [CorpseName("lodestone's corpse")]
    public class Lodestone : BaseCreature
    {
        public override string TitleReward { get { return "Slayer of Lodestone"; } }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextRockWaveAllowed;
        public TimeSpan NextRockWaveDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextRockslideAllowed;
        public TimeSpan NextRockslideDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextBoulderAllowed;
        public TimeSpan NextBoulderDelay = TimeSpan.FromSeconds(45);

        public DateTime m_NextHarmonicRefractorAllowed;
        public TimeSpan NextHarmonicRefractorDelay = TimeSpan.FromSeconds(120);

        public DateTime m_NextAbilityAllowed;
        public double NextAbilityDelayMin = 10;
        public double NextAbilityDelayMax = 5;

        public int damageIntervalThreshold = 500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 50;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_RockWaveTargets = new List<Mobile>();
        public List<Mobile> m_RockslideTargets = new List<Mobile>();

        public List<Mobile> m_Creatures = new List<Mobile>();
        public List<Item> m_Items = new List<Item>();

        public string[] idleSpeech { get { return new string[] {"*lumbers*"}; } }
        public string[] combatSpeech { get  { return new string[] {""}; } }

        public int ThemeHue = 2590; //2709;

        [Constructable]
        public Lodestone(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Lodestone";

            Body = 829;
            Hue = ThemeHue;
            BaseSoundID = 268;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(25000);
            SetStam(25000);
            SetMana(0);

            SetDamage(30, 50);            

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 100;
        }

        public virtual int AttackRange { get { return 2; } }

        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "Lodestone has arisen and stirs within Shame Dungeon..."; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return 1; } }
        public override int IdleFrames { get { return 15; } }
                
        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 1.5;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));            
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);          
        }

        public override bool OnBeforeHarmfulSpell()
        {
            HarmonicRefractor harmonicRefractor = null;
            int closestDistance = 100000;

            foreach (HarmonicRefractor refractor in HarmonicRefractor.m_Instances)
            {
                if (refractor == null) continue;
                if (refractor.Deleted) continue;

                int distance = Utility.GetDistance(Location, refractor.Location);

                if (distance <= 200)
                {
                    if (distance < closestDistance)
                    {
                        harmonicRefractor = refractor;
                        closestDistance = distance;
                    }
                }
            }
            
            if (harmonicRefractor != null)
            {
                TimedStatic reflection = new TimedStatic(0x375A, 2);
                reflection.Name = "spell refraction";
                reflection.Hue = ThemeHue;
                Point3D refractionLocation = harmonicRefractor.Location;
                refractionLocation.Z += 10;
                reflection.MoveToWorld(refractionLocation, Map);
                Effects.PlaySound(Location, Map, 0x1E9);

                harmonicRefractor.PublicOverheadMessage(MessageType.Regular, 0, false, "*captures harmonic frequency*");

                for (int a = 0; a < 4; a++)
                {
                    Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);

                    reflection = new TimedStatic(0x375A, 2);
                    reflection.Name = "spell refraction";
                    reflection.Hue = ThemeHue;
                    reflection.MoveToWorld(newLocation, Map);                    
                }

                PublicOverheadMessage(MessageType.Regular, 0, false, "*spell refracted*");
                Effects.PlaySound(harmonicRefractor.Location, Map, 0x1E9);
                               
                MagicDamageAbsorb = 1;
            }           

            return true;
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

                    if (intervalCount % 8 == 0)
                    {
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 2, true, 0, false, "", "", "-1");

                        Animate(15, 12, 1, true, false, 0);
                        PlaySound(GetAngerSound());

                        List<Type> m_CreatureTypes = new List<Type>();

                        int creaturePower = (int)(Math.Round(10 + (20 * spawnPercent)));
                        int creaturePowerRemaining = creaturePower;

                        int maxCreatureStrength = (int)(Math.Round(2 + (10 * spawnPercent)));

                        if (maxCreatureStrength > 10)
                            maxCreatureStrength = 10;

                        for (int a = 0; a < 100; a++)
                        {
                            Type type = null;

                            switch (Utility.RandomMinMax(1, maxCreatureStrength))
                            {
                                case 1: type = typeof(EarthElemental); creaturePowerRemaining -= 3; break;
                                case 2: type = typeof(DullCopperElemental); creaturePowerRemaining -= 4; break;
                                case 3: type = typeof(CopperElemental); creaturePowerRemaining -= 5; break;
                                case 4: type = typeof(BronzeElemental); creaturePowerRemaining -= 6; break;
                                case 5: type = typeof(ShadowIronElemental); creaturePowerRemaining -= 7; break;
                                case 6: type = typeof(GoldenElemental); creaturePowerRemaining -= 8; break;
                                case 7: type = typeof(AgapiteElemental); creaturePowerRemaining -= 9; break;
                                case 8: type = typeof(VeriteElemental); creaturePowerRemaining -= 10; break;
                                case 9: type = typeof(ValoriteElemental); creaturePowerRemaining -= 11; break;
                                case 10: type = typeof(LuniteElemental); creaturePowerRemaining -= 12; break;
                            }

                            if (creaturePowerRemaining <= 0)
                                break;

                            if (type != null)
                                m_CreatureTypes.Add(type);
                        }

                        foreach (Type type in m_CreatureTypes)
                        {
                            SpawnCreatures(type, 1);
                        }
                    }

                    else
                    {
                        int abilities = 3;

                        if (HarmonicRefractor.m_Instances.Count < 5)
                            abilities++;

                        switch (Utility.RandomMinMax(1, abilities))
                        {                           
                            case 1: GroundSlam(); break;                            
                            case 2: BoulderStorm(); break;
                            case 3: Rockalanche(); break;
                            case 4: GoBelow(); break;
                        }
                    }                   
                }

                else
                {
                    if (from != null && !AbilityInProgress && !DamageIntervalInProgress)
                    {
                        BaseWeapon weapon = from.Weapon as BaseWeapon;

                        double rockfallChance = 0;
                        double rockfallScalar = .001;

                        if (from is PlayerMobile)
                        {
                            if (weapon != null)
                            {
                                //Ranged Weapon
                                if (weapon is BaseRanged)
                                    rockfallScalar = .006;

                                //Melee Weapon
                                else if (weapon is BaseMeleeWeapon || weapon is Fists)
                                    rockfallScalar = .001;
                            }

                            //Other: Spell or Item
                            else
                                rockfallScalar = .004;
                        }

                        //Monster Damage
                        else
                             rockfallScalar = .001;

                        rockfallChance = (double)amount * rockfallScalar;

                        if (Utility.RandomDouble() <= rockfallChance)
                        {
                            if (SpecialAbilities.MonsterCanDamage(this, from) && Utility.GetDistance(Location, from.Location) <= 30)
                                RockFall(from.Location, from.Map);
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
        #region Rock Fall

        public void RockFall(Point3D location, Map map)
        {
            if (!SpecialAbilities.Exists(this))
                return;

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X - 1, location.Y - 1, location.Z + 100), map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int rockType = 1;

            if (spawnPercent >= .40)
                rockType = 2;

            if (spawnPercent >= .80)
                rockType = 3;

            double damage = 15 + (5 * (double)rockType);

            int rockItemId = 6004;
            int rockHue = ThemeHue;

            switch (rockType)
            {
                case 1: break; rockItemId = Utility.RandomList(6003, 6004, 4970, 4973); break;
                case 2: break; rockItemId = Utility.RandomList(6001, 4963, 4967); break;
                case 3: break; rockItemId = Utility.RandomList(6002); break;
            }

            int speed = 8;
            double impactDelay = .6;

            double duration = 3;

            Effects.SendMovingParticles(startLocation, endLocation, rockItemId, speed, 0, false, false, rockHue - 1, 0, 9501, 0, 0, 0x100);

            Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                TimedStatic rock = new TimedStatic(rockItemId, duration);
                rock.Name = "rock";
                rock.Hue = rockHue;
                rock.ItemID = rockItemId;
                rock.MoveToWorld(location, map);

                TimedStatic dirt = new TimedStatic(rockItemId, duration);
                dirt.Name = "dirt";
                dirt.ItemID = Utility.RandomList(7681, 7682);

                Point3D newLocation = location;

                Point3D dirtLocation = new Point3D(newLocation.X + Utility.RandomList(-1, 1), newLocation.Y + Utility.RandomList(-1, 1), newLocation.Z);
                SpellHelper.AdjustField(ref dirtLocation, map, 12, false);

                dirt.MoveToWorld(dirtLocation, Map);

                Effects.PlaySound(newLocation, map, 0x11D);

                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

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
                        damage *= 2;

                    DoHarmful(mobile);

                    int finalDamage = (int)(Math.Round(damage));

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                }
            });
        }

        #endregion

        #region Rock Wave

        public void RockWave()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_RockWaveTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            int range = 20;

            Combatant = null;
            
            List<Mobile> m_ValidMobiles = new List<Mobile>();

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;
                if (!map.InLOS(location, mobile.Location)) continue;

                m_ValidMobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_ValidMobiles.Count == 0)
                return;

            Point3D targetLocation = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)].Location;
            Map targetMap = map;

            double directionDelay = .25;
            double initialDelay = 1 - (.5 * spawnPercent);
            double intervalDelay = .12 - (.06 * spawnPercent);

            int distance = Utility.GetDistance(location, targetLocation);
            double distanceDelay = (double)distance * intervalDelay;

            double totalDelay = directionDelay + initialDelay + distanceDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextRockWaveAllowed = DateTime.UtcNow + NextRockWaveDelay + TimeSpan.FromSeconds(totalDelay);
            AbilityInProgress = true;            

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes rock wave*");

            Direction baseDirectionToTarget = Utility.GetDirection(location, targetLocation);
            Direction = baseDirectionToTarget;
                                    
            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(15, 12, 1, true, false, 0);
               
                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(location, map, 0x21F);                    
                    
                    int baseDistance = Utility.GetDistance(location, targetLocation);
                    int maxRange = 30;

                    Point3D currentLocation = location;

                    Dictionary<Point3D, bool> m_RockLocations = new Dictionary<Point3D, bool>();

                    m_RockLocations.Add(targetLocation, true);

                    for (int a = 0; a < maxRange; a++)
                    {
                        int distanceFromStart = Utility.GetDistance(location, currentLocation);

                        if (currentLocation == targetLocation)
                            currentLocation = SpecialAbilities.GetPointByDirection(currentLocation, baseDirectionToTarget);

                        else
                        {
                            if (distanceFromStart > baseDistance)
                                currentLocation = SpecialAbilities.GetPointByDirection(currentLocation, baseDirectionToTarget);

                            else
                            {
                                Direction directionToTarget = Utility.GetDirection(currentLocation, targetLocation);
                                currentLocation = SpecialAbilities.GetPointByDirection(currentLocation, directionToTarget);
                            }
                        }

                        int radius = 1;

                        if (spawnPercent >= .25)
                            radius = 2;

                        if (spawnPercent >= .75)
                            radius = 3;

                        int minRadius = radius * -1;
                        int maxRadius = radius;

                        for (int b = minRadius; b < maxRadius + 1; b++)
                        {
                            for (int c = minRadius; c < maxRadius + 1; c++)
                            {
                                Point3D rockLocation =  new Point3D(currentLocation.X + b, currentLocation.Y + c, currentLocation.Z);

                                double rockChance = .33 + (.33 * spawnPercent);

                                bool rock = Utility.RandomDouble() <= rockChance;
                                
                                if (!m_RockLocations.ContainsKey(rockLocation))
                                    m_RockLocations.Add(rockLocation, rock);
                            }
                        }    
                    }

                    if (m_RockLocations.Count > 0)
                    {
                        foreach (KeyValuePair<Point3D, bool> rockInstance in m_RockLocations)
                        {
                            Point3D rockLocation = rockInstance.Key;
                            bool isRock = rockInstance.Value;
                            
                            if (!SpellHelper.AdjustField(ref rockLocation, map, 12, false))
                                continue;

                            int distanceFromStart = Utility.GetDistance(location, rockLocation);
                            double rockDistanceDelay = (double)distanceFromStart * intervalDelay;

                            Timer.DelayCall(TimeSpan.FromSeconds(rockDistanceDelay), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                double effectDuration = 3;

                                if (isRock)
                                {
                                    TimedStatic rock = new TimedStatic(Utility.RandomList(2276, 2275, 2282, 2281, 2274, 2280, 2273, 2272, 2277, 2279), effectDuration);
                                    rock.Name = "rock";
                                    rock.Hue = ThemeHue;
                                    rock.MoveToWorld(rockLocation, map);

                                    Queue m_Queue = new Queue();

                                    nearbyMobiles = map.GetMobilesInRange(rockLocation, 0);

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

                                        double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax));

                                        if (mobile is BaseCreature)
                                            damage *= 2;

                                        if (m_RockWaveTargets.Contains(mobile))
                                            continue;

                                        m_RockWaveTargets.Add(mobile);

                                        int finalDamage = (int)Math.Round(damage);

                                        DoHarmful(mobile);

                                        int hurtSound = mobile.GetHurtSound();

                                        if (mobile is PlayerMobile)
                                        {
                                            if (mobile.Female)
                                                hurtSound = 0x14D;
                                            else
                                                hurtSound = 0x156;
                                        }

                                        mobile.PlaySound(hurtSound);

                                        for (int a = 0; a < 3; a++)
                                        {
                                            Point3D bloodLocation = new Point3D(mobile.Location.X + Utility.RandomList(-1, 1), mobile.Location.Y + Utility.RandomList(-1, 1), mobile.Location.Z);
                                            SpellHelper.AdjustField(ref bloodLocation, map, 12, false);

                                            new Blood().MoveToWorld(bloodLocation, mobile.Map);
                                        }

                                        mobile.PlaySound(mobile.GetHurtSound());

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                                    }
                                }

                                else
                                {
                                    TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), effectDuration);
                                    floorCrack.Name = "floor crack";
                                    floorCrack.MoveToWorld(rockLocation, targetMap);
                                }
                            });
                        }
                    }
                });
            });
        }

        #endregion

        #region Rockslide

        public void Rockslide()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_RockslideTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            int range = 20;

            Combatant = null;

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

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_ValidMobiles.Count == 0)
                return;

            Point3D targetLocation = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)].Location;
            Map targetMap = map;
            
            double directionDelay = .25;
            double initialDelay = 1 - (.5 * spawnPercent);
            double durationDelay = 1;

            double totalDelay = directionDelay + initialDelay + durationDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextRockslideAllowed = DateTime.UtcNow + NextRockslideDelay + TimeSpan.FromSeconds(totalDelay);
            AbilityInProgress = true;

            Direction = Utility.GetDirection(location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*begins a rockslide*");

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(27, 15, 1, true, false, 0);
                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(location, map, 0x21F);

                    int effectCount = (int)Math.Round(10 + (40 * spawnPercent));
                    int effectRadius = 1;

                    if (spawnPercent >= .25)
                        effectRadius = 2;

                    if (spawnPercent >= .5)
                        effectRadius = 3;

                    if (spawnPercent >= .75)
                        effectRadius = 4;
                    
                    int rows = (effectRadius * 2) + 1;
                    int columns = (effectRadius * 2) + 1;

                    List<Point3D> m_EffectLocations = new List<Point3D>();

                    for (int a = 1; a < rows + 1; a++)
                    {
                        for (int b = 1; b < columns + 1; b++)
                        {
                            Point3D newPoint = new Point3D(targetLocation.X + (-1 * (effectRadius + 1)) + a, targetLocation.Y + (-1 * (effectRadius + 1)) + b, targetLocation.Z);

                            SpellHelper.AdjustField(ref newPoint, map, 12, false);

                            if (!m_EffectLocations.Contains(newPoint))
                                m_EffectLocations.Add(newPoint);
                        }
                    }

                    if (m_EffectLocations.Count > 0)
                    {         
                        for (int a = 0; a < effectCount; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                            {
                                if (!SpecialAbilities.Exists(this))
                                    return;

                                Point3D newLocation;

                                if (Utility.RandomDouble() <= .20)
                                    newLocation = targetLocation;
                                else
                                    newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];
                                
                                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(newLocation.X - 1, newLocation.Y - 1, newLocation.Z + 50), map);
                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                                int rockItemId = Utility.RandomList(6001, 6002, 6003, 6004, 4963, 4967, 4970, 4973);
                                int rockHue = ThemeHue;

                                Effects.SendMovingParticles(startLocation, endLocation, rockItemId, 8, 0, false, false, rockHue - 1, 0, 9501, 0, 0, 0x100);

                                double duration = 3;

                                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                {
                                    if (!SpecialAbilities.Exists(this))
                                        return;

                                    TimedStatic rock = new TimedStatic(rockItemId, duration);
                                    rock.Name = "rock";
                                    rock.Hue = rockHue;
                                    rock.ItemID = rockItemId;
                                    rock.MoveToWorld(newLocation, map);

                                    TimedStatic dirt = new TimedStatic(rockItemId, duration);
                                    dirt.Name = "dirt";
                                    dirt.ItemID = Utility.RandomList(7681, 7682);

                                    Point3D dirtLocation = new Point3D(newLocation.X + Utility.RandomList(-1, 1), newLocation.Y + Utility.RandomList(-1, 1), newLocation.Z);
                                    SpellHelper.AdjustField(ref dirtLocation, map, 12, false);
                                    
                                    dirt.MoveToWorld(dirtLocation, Map);

                                    Effects.PlaySound(newLocation, map, 0x11D);

                                    IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

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

                                        int minDamage = (int)(Math.Round((double)DamageMin * .5));
                                        int maxDamage = (int)(Math.Round((double)DamageMin));

                                        double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                        if (mobile is BaseCreature)
                                            damage *= 2;

                                        if (m_RockslideTargets.Contains(mobile))
                                            damage *= .5;

                                        DoHarmful(mobile);

                                        m_RockslideTargets.Add(mobile);

                                        int finalDamage = (int)(Math.Round(damage));

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                                    }
                                });
                            });
                        }
                    }                    
                });
            });
        }

        #endregion

        #region Boulder

        public void Boulder()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            int range = 20;

            Combatant = null;

            List<Mobile> m_ValidMobiles = new List<Mobile>();

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;
                if (!map.InLOS(location, mobile.Location)) continue;

                m_ValidMobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_ValidMobiles.Count == 0)
                return;

            Point3D targetLocation = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)].Location;
            Map targetMap = map;

            double directionDelay = .25;
            double initialDelay = 1;

            double totalDelay = directionDelay + initialDelay + 1;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextBoulderAllowed = DateTime.UtcNow + NextBoulderDelay + TimeSpan.FromSeconds(totalDelay);
            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            Direction = Utility.GetDirection(location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;
                
                Animate(15, 12, 1, false, false, 0);
                PlaySound(GetAngerSound());

                PublicOverheadMessage(MessageType.Regular, 0, false, "*readies a boulder*");
                
                TimedStatic boulder = new TimedStatic(4534, 5);
                boulder.Name = "rock";
                boulder.Hue = ThemeHue;

                Point3D boulderLocation = location;
                boulderLocation.Z += 50;

                boulder.MoveToWorld(boulderLocation, map);
                
                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    if (boulder != null)
                        boulder.Delete();

                    Effects.PlaySound(location, map, 0x5D2);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(boulderLocation.X, boulderLocation.Y, boulderLocation.Z), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);

                    int rockItemId = 4534;
                    int rockHue = ThemeHue;

                    int speed = 5;                   

                    Effects.SendMovingParticles(startLocation, endLocation, rockItemId, speed, 0, false, false, rockHue - 1, 0, 9501, 0, 0, 0x100);

                    double distance = Utility.GetDistanceToSqrt(location, targetLocation);
                    double distanceDelay = (double)distance * .11;

                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Effects.PlaySound(targetLocation, map, 0x308);

                        int projectiles = 8;
                        int particleSpeed = 5;

                        for (int a = 0; a < projectiles; a++)
                        {
                            Point3D newLocation = new Point3D(targetLocation.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), targetLocation.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), targetLocation.Z);
                            
                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + Utility.RandomMinMax(5, 15)), map);

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6003, 6004, 4970, 4973), particleSpeed, 0, false, false, ThemeHue - 1, 0);
                        }

                        IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(targetLocation, 1);

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

                            int minDamage = (int)(Math.Round((double)DamageMax * .75));
                            int maxDamage = (int)(Math.Round((double)DamageMax));

                            double damage = Utility.RandomMinMax(minDamage, maxDamage);

                            if (mobile is BaseCreature)
                                damage *= 2;                           

                            DoHarmful(mobile);

                            int finalDamage = (int)(Math.Round(damage));

                            SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, 10, 3, -1, "", "You are knocked off your feet!");

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                        }

                        int guarCount = 0;

                        foreach (BaseCreature creature in m_Creatures)
                        {
                            if (creature is Guar)
                                guarCount++;
                        }

                        if (guarCount < 5)
                        {
                            RockGuar rockguar = new RockGuar();

                            rockguar.MoveToWorld(targetLocation, map);
                            rockguar.PlaySound(rockguar.GetAngerSound());

                            m_Creatures.Add(rockguar);
                        }
                    });
                });
            });
        }

        #endregion   
 
        #region Create Harmonic Refractor

        public void CreateHarmonicRefractor()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double directionDelay = .25;
            double initialDelay = 1;

            double totalDelay = directionDelay + initialDelay + 1;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextHarmonicRefractorAllowed = DateTime.UtcNow + NextHarmonicRefractorDelay;
            AbilityInProgress = true;

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*reaches into the earth*");

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(11, 18, 1, false, false, 0);

                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    HarmonicRefractor refractor = new HarmonicRefractor();

                    refractor.Name = "small harmonic refractor";

                    int itemStyle = 730;

                    refractor.ItemID = itemStyle;
                    refractor.NormalItemId = itemStyle;
                    refractor.LightlyDamagedItemId = itemStyle;
                    refractor.HeavilyDamagedItemId = itemStyle;

                    refractor.MaxHitPoints = 250;
                    refractor.HitPoints = 250;

                    m_Items.Add(refractor);

                    Point3D refractorLocation = Location;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                    if (m_ValidLocations.Count > 0)
                        refractorLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    int movementIntervals = 50;
                    double intervalDelay = .03;

                    refractorLocation.Z -= movementIntervals;

                    refractor.MoveToWorld(refractorLocation, Map);

                    for (int a = 0; a < 20; a++)
                    {
                        Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);
                        SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                        TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                        floorCrack.Name = "floor crack";
                        floorCrack.MoveToWorld(newLocation, Map);
                    }

                    for (int a = 0; a < movementIntervals; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * intervalDelay), delegate
                        {
                            if (refractor != null)
                            {
                                if (!refractor.Deleted)
                                    refractor.Z += 1;
                            }
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds((double)movementIntervals * intervalDelay), delegate
                    {
                        if (refractor == null) return;
                        if (refractor.Deleted) return;

                        TimedStatic reflection = new TimedStatic(0x375A, 2);
                        reflection.Name = "spell refraction";
                        reflection.Hue = ThemeHue;
                        Point3D refractionLocation = refractor.Location;
                        refractionLocation.Z += 10;
                        reflection.MoveToWorld(refractionLocation, Map);

                        Effects.PlaySound(refractionLocation, Map, 0x1E9);

                        refractor.PublicOverheadMessage(MessageType.Regular, 0, false, "*begins capturing spell harmonics*");
                    });
                });
            });
        }

        #endregion

        //Epic Abilities

        #region Boulderstorm

        public void BoulderStorm()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            int range = 20;
            int boulderCount = 2;

            if (spawnPercent >= .40)
                boulderCount = 3;

            if (spawnPercent >= .80)
                boulderCount = 4;

            Combatant = null;

            List<Point3D> m_PossibleLocations = new List<Point3D>();

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;
                if (!map.InLOS(location, mobile.Location)) continue;

                if (m_PossibleLocations.Contains(mobile.Location))
                    continue;

                m_PossibleLocations.Add(mobile.Location);
            }

            nearbyMobiles.Free();

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_PossibleLocations.Count == 0)
                return;

            double directionDelay = .25;
            double initialDelay = 1;

            double totalDelay = directionDelay + initialDelay + ((double)boulderCount * .5);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextBoulderAllowed = DateTime.UtcNow + NextBoulderDelay + TimeSpan.FromSeconds(totalDelay);
            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;
                
                Animate(15, 12, 1, false, false, 0);
                PlaySound(GetAngerSound());

                PublicOverheadMessage(MessageType.Regular, 0, false, "*readies several boulders*");

                int iStartY = location.Z + 50;

                for (int a = 0; a < boulderCount; a++)
                {
                    TimedStatic boulder = new TimedStatic(4534, 5);
                    boulder.Name = "rock";
                    boulder.Hue = ThemeHue;

                    Point3D boulderLocation = location;
                    boulderLocation.Z = iStartY;

                    iStartY += 20;

                    boulder.MoveToWorld(boulderLocation, map);

                    Point3D targetLocation = m_PossibleLocations[Utility.RandomMinMax(0, m_PossibleLocations.Count - 1)];
                
                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay + (a * .5)), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        if (boulder != null)
                            boulder.Delete();

                        Effects.PlaySound(location, map, 0x5D2);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(boulderLocation.X, boulderLocation.Y, boulderLocation.Z), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);

                        int rockItemId = 4534;
                        int rockHue = ThemeHue;

                        int speed = 5;                   

                        Effects.SendMovingParticles(startLocation, endLocation, rockItemId, speed, 0, false, false, rockHue - 1, 0, 9501, 0, 0, 0x100);

                        double distance = Utility.GetDistanceToSqrt(location, targetLocation);
                        double distanceDelay = (double)distance * .11;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            Effects.PlaySound(targetLocation, map, 0x308);

                            int projectiles = 8;
                            int particleSpeed = 5;

                            for (int b = 0; b < projectiles; b++)
                            {
                                Point3D newLocation = new Point3D(targetLocation.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), targetLocation.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), targetLocation.Z);
                            
                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + Utility.RandomMinMax(5, 15)), map);

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6003, 6004, 4970, 4973), particleSpeed, 0, false, false, ThemeHue - 1, 0);
                            }

                            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(targetLocation, 1);

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

                                double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax));

                                if (mobile is BaseCreature)
                                    damage *= 2;                           

                                DoHarmful(mobile);

                                int finalDamage = (int)(Math.Round(damage));

                                SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, 10, 3, -1, "", "You are knocked off your feet!");

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                            }
                    
                            RockGuar rockguar = new RockGuar();

                            rockguar.MoveToWorld(targetLocation, map);
                            rockguar.PlaySound(rockguar.GetAngerSound());

                            m_Creatures.Add(rockguar);                            
                        });
                    });
                }
            });
        }

        #endregion   

        #region Rockalanche

        public void Rockalanche()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_RockslideTargets.Clear();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            int range = 20;

            Combatant = null;

            List<Point3D> m_PossibleLocations = new List<Point3D>();
            List<Point3D> m_TargetLocations = new List<Point3D>();

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (mobile.Hidden) continue;

                if (!m_PossibleLocations.Contains(mobile.Location))
                    m_PossibleLocations.Add(mobile.Location);
            }

            nearbyMobiles.Free();

            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            if (m_PossibleLocations.Count == 0)
                return;

            int targets = (int)Math.Round(2 + (4 * spawnPercent));

            for (int a = 0; a < targets; a++)
            {
                if (m_PossibleLocations.Count > 0)
                {
                    int index = Utility.RandomMinMax(0, m_PossibleLocations.Count - 1);

                    m_TargetLocations.Add(m_PossibleLocations[index]);
                    m_PossibleLocations.RemoveAt(index);
                }
            }

            double directionDelay = .25;
            double initialDelay = 1 - (.5 * spawnPercent);
            double durationDelay = 1;

            double totalDelay = directionDelay + initialDelay + durationDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            m_NextRockslideAllowed = DateTime.UtcNow + NextRockslideDelay + TimeSpan.FromSeconds(totalDelay);
            AbilityInProgress = true;            

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*summons a rockalanche*");

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(27, 15, 1, true, false, 0);
                PlaySound(GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Effects.PlaySound(location, map, 0x21F);

                    int effectCount = (int)Math.Round(10 + (40 * spawnPercent));
                    int effectRadius = 1;

                    if (spawnPercent >= .25)
                        effectRadius = 2;

                    if (spawnPercent >= .5)
                        effectRadius = 3;

                    if (spawnPercent >= .75)
                        effectRadius = 4;

                    foreach (Point3D point in m_TargetLocations)
                    {
                        int rows = (effectRadius * 2) + 1;
                        int columns = (effectRadius * 2) + 1;

                        List<Point3D> m_EffectLocations = new List<Point3D>();

                        for (int a = 1; a < rows + 1; a++)
                        {
                            for (int b = 1; b < columns + 1; b++)
                            {
                                Point3D newPoint = new Point3D(point.X + (-1 * (effectRadius + 1)) + a, point.Y + (-1 * (effectRadius + 1)) + b, point.Z);

                                SpellHelper.AdjustField(ref newPoint, map, 12, false);

                                if (!m_EffectLocations.Contains(newPoint))
                                    m_EffectLocations.Add(newPoint);
                            }
                        }

                        if (m_EffectLocations.Count > 0)
                        {
                            for (int a = 0; a < effectCount; a++)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                                {
                                    if (!SpecialAbilities.Exists(this))
                                        return;

                                    Point3D newLocation;

                                    if (Utility.RandomDouble() <= .20)
                                        newLocation = point;
                                    else
                                        newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];

                                    SpellHelper.AdjustField(ref newLocation, map, 12, false);

                                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(newLocation.X - 1, newLocation.Y - 1, newLocation.Z + 50), map);
                                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                                    int rockItemId = Utility.RandomList(6001, 6002, 6003, 6004, 4963, 4967, 4970, 4973);
                                    int rockHue = ThemeHue;

                                    Effects.SendMovingParticles(startLocation, endLocation, rockItemId, 8, 0, false, false, rockHue - 1, 0, 9501, 0, 0, 0x100);

                                    double duration = 3;

                                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                    {
                                        if (!SpecialAbilities.Exists(this))
                                            return;

                                        TimedStatic rock = new TimedStatic(rockItemId, duration);
                                        rock.Name = "rock";
                                        rock.Hue = rockHue;
                                        rock.ItemID = rockItemId;
                                        rock.MoveToWorld(newLocation, map);

                                        TimedStatic dirt = new TimedStatic(rockItemId, duration);
                                        dirt.Name = "dirt";
                                        dirt.ItemID = Utility.RandomList(7681, 7682);

                                        Point3D dirtLocation = new Point3D(newLocation.X + Utility.RandomList(-1, 1), newLocation.Y + Utility.RandomList(-1, 1), newLocation.Z);
                                        SpellHelper.AdjustField(ref dirtLocation, map, 12, false);

                                        dirt.MoveToWorld(dirtLocation, Map);

                                        Effects.PlaySound(newLocation, map, 0x11D);

                                        IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

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

                                            int minDamage = (int)(Math.Round((double)DamageMin * .5));
                                            int maxDamage = (int)(Math.Round((double)DamageMin));

                                            double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                            if (mobile is BaseCreature)
                                                damage *= 2;

                                            if (m_RockslideTargets.Contains(mobile))
                                                damage *= .5;

                                            DoHarmful(mobile);

                                            m_RockslideTargets.Add(mobile);

                                            int finalDamage = (int)(Math.Round(damage));

                                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                            AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                                        }
                                    });
                                });
                            }
                        }
                    }
                });
            });
        }

        #endregion        

        #region GroundSlam

        public void GroundSlam()
        {
            if (!SpecialAbilities.Exists(this))
                return;
            
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            Combatant = null;
            
            double directionDelay = .25;
            double initialDelay = 2;
            double totalDelay = directionDelay + initialDelay + 1;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay() + TimeSpan.FromSeconds(.5);        

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*shakes the earth*");
                                    
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

                    Effects.PlaySound(location, map, 0x220);

                    int radius = (int)(Math.Round(6 + (6 * spawnPercent)));
                    
                    int minRadius = radius * -1;
                    int maxRadius = radius;

                    Point3D newLocation = Location;

                    for (int a = minRadius; a < maxRadius + 1; a++)
                    {
                        for (int b = minRadius; b < maxRadius + 1; b++)
                        {
                            Point3D rockLocation =  new Point3D(newLocation.X + a, newLocation.Y + b, newLocation.Z);

                            int distance = Utility.GetDistance(Location, rockLocation);

                            double effectChance = 1 - ((double)distance * .025);

                            if (Utility.RandomDouble() <= effectChance)
                            {
                                SpellHelper.AdjustField(ref rockLocation, map, 12, false);

                                double duration = 5;

                                TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), duration);
                                floorCrack.Name = "floor crack";
                                floorCrack.MoveToWorld(rockLocation, map);
                                
                                int dustHue = 2074;
                                Effects.SendLocationParticles(EffectItem.Create(rockLocation, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, dustHue, 0, 5029, 0);                                
                            }                            
                        }
                    }

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, radius);

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

                        int minDamage = (int)(Math.Round((double)DamageMin * .75));
                        int maxDamage = (int)(Math.Round((double)DamageMin * 1.25));

                        double damage = Utility.RandomMinMax(minDamage, maxDamage);
                       
                        double knockbackDamage = DamageMin;
                                
                        if (mobile is BaseCreature)
                        {
                            damage *= 2;
                            knockbackDamage *= 2;
                        }                                

                        int finalDamage = (int)Math.Round(damage);
                        int finalKnockbackDamage = (int)Math.Round(knockbackDamage);

                        DoHarmful(mobile);

                        int knockbackDistance = (int)(Math.Round(8 + (8 * spawnPercent)));

                        SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, finalKnockbackDamage, knockbackDistance, -1, "", "The ground shakes and you are knocked off your feet!");
                                
                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                    }                        
                });
            });
        }

        #endregion

        #region Go Below

        public void GoBelow()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double initialDelay = 1.5;
            double totalDelay = 8;

            Blessed = true;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay() + TimeSpan.FromSeconds(.5);            

            PublicOverheadMessage(MessageType.Regular, 0, false, "*returns to the earth*");

            Animate(23, 15, 1, false, false, 0);
            Effects.PlaySound(Location, Map, 0x21E);

            Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Hidden = true;

                HarmonicRefractor refractor = new HarmonicRefractor();

                m_Items.Add(refractor);

                Point3D refractorLocation = Location;

                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                if (m_ValidLocations.Count > 0)
                    refractorLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                         
                int movementIntervals = 50;
                double intervalDelay = .03;

                refractorLocation.Z -= movementIntervals;

                refractor.MoveToWorld(refractorLocation, Map);

                for (int a = 0; a < 20; a++)
                {
                    Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);
                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                    TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                    floorCrack.Name = "floor crack";
                    floorCrack.MoveToWorld(newLocation, Map); 
                }

                for (int a = 0; a < movementIntervals; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * intervalDelay), delegate
                    {
                        if (refractor != null)
                        {
                            if (!refractor.Deleted)
                                refractor.Z += 1;
                        }
                    });                   
                }

                Timer.DelayCall(TimeSpan.FromSeconds((double)movementIntervals * intervalDelay), delegate
                {
                    if (refractor == null) return;
                    if (refractor.Deleted) return;

                    TimedStatic reflection = new TimedStatic(0x375A, 2);
                    reflection.Name = "spell refraction";
                    reflection.Hue = ThemeHue;
                    Point3D refractionLocation = refractor.Location;
                    refractionLocation.Z += 10;
                    reflection.MoveToWorld(refractionLocation, Map);

                    Effects.PlaySound(refractionLocation, Map, 0x1E9);

                    refractor.PublicOverheadMessage(MessageType.Regular, 0, false, "*begins capturing spell harmonics*");
                });
            });

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                double riseTime = 1.25;

                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, riseTime, true, 0, false, "", "", "-1");

                Hidden = false;   

                PublicOverheadMessage(MessageType.Regular, 0, false, "*rises from the earth*");

                Animate(23, 15, 1, true, false, 0);

                Effects.PlaySound(Location, Map, 0x21E);

                for (int a = 0; a < 20; a++)
                {
                    Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);
                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                    TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                    floorCrack.Name = "floor crack";
                    floorCrack.MoveToWorld(newLocation, Map);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(riseTime), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Blessed = false;

                    AbilityInProgress = false;
                    DamageIntervalInProgress = false;
                    m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

                });
            });
        }

        #endregion

        #region Spawn Creatures

        public void SpawnCreatures(Type type, int creatures)
        {
            if (!SpecialAbilities.Exists(this))
                return;

            for (int a = 0; a < creatures; a++)
            {
                Point3D creatureLocation = Location;
                Map creatureMap = Map;

                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                if (m_ValidLocations.Count > 0)
                    creatureLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                BaseCreature creature = (BaseCreature)Activator.CreateInstance(type);

                if (creature == null)
                    return;

                creature.BossMinion = true;
                creature.Hidden = true;

                m_Creatures.Add(creature);

                creature.MoveToWorld(creatureLocation, Map);
                creature.PlaySound(creature.GetIdleSound());

                SpecialAbilities.HinderSpecialAbility(1.0, null, creature, 1.0, 2, true, 0, false, "", "", "-1");

                TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 3);
                floorCrack.Name = "floor crack";
                floorCrack.MoveToWorld(creatureLocation, Map);

                for (int b = 0; b < 4; b++)
                {
                    Point3D newLocation = new Point3D(creatureLocation.X + Utility.RandomList(-2, -1, 1, 2), creatureLocation.Y + Utility.RandomList(-2, -1, 1, 2), creatureLocation.Z);
                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                    floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 3);
                    floorCrack.Name = "floor crack";
                    floorCrack.MoveToWorld(newLocation, Map);
                }     

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (!SpecialAbilities.Exists(creature))
                        return;

                    creature.Hidden = false;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, creature, 1.0, 1, true, 0, false, "", "", "-1");

                    int animation = Utility.RandomMinMax(2, 3);
                    int frameCount = 4;

                    creature.PlaySound(creature.GetAngerSound());
                    creature.Animate(animation, frameCount, 1, false, false, 1);
                });
            }
        }

        #endregion        

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            ActiveSpeed = .5 - (.2 * spawnPercent);
            PassiveSpeed = .6 - (.2 * spawnPercent);

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                int abilities = 3;

                if (HarmonicRefractor.m_Instances.Count < 5)
                    abilities++;

                switch (Utility.RandomMinMax(1, abilities))
                {       
                    case 1:
                        if (DateTime.UtcNow >= m_NextRockWaveAllowed)
                        {                           
                            RockWave();
                            return;
                        }
                    break;

                    case 2:
                        if (DateTime.UtcNow >= m_NextRockslideAllowed)
                        {
                            Rockslide();
                            return;
                        }
                    break;

                    case 3:
                        if (DateTime.UtcNow >= m_NextBoulderAllowed)
                        {
                            Boulder();
                            return;
                        }                        
                    break;

                    case 4:
                        if (DateTime.UtcNow >= m_NextHarmonicRefractorAllowed)
                        {
                            CreateHarmonicRefractor();
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
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*grumbles*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*crunches*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*shambles*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*groans*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*cracks*");

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

            if (Utility.RandomDouble() <= .5)
            {
                TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 14);
                floorCrack.Name = "floor crack";
                floorCrack.MoveToWorld(Location, Map);            
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

            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)                
                    m_Items[a].Delete();                
            }
        }

        public Lodestone(Serial serial): base(serial)
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

            writer.Write(m_Items.Count);
            for (int a = 0; a < m_Items.Count; a++)
            {
                writer.Write(m_Items[a]);
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

                int itemCount = reader.ReadInt();
                for (int a = 0; a < itemCount; a++)
                {
                    Item item = reader.ReadItem();

                    m_Items.Add(item);
                }
            }

            //------------

            Blessed = false;
            Hidden = false;
        }
    }
}

//Animations
//Body = 829;

//1 15 Idle
//4 10 Swing Up
//5 15 Slam Down
//10 8 Get Hit
//11 15 Kneel Grab
//15 12 Summon
//17 12 Idle
//23 15 Forwards: Rise From Ground
//23 15 Backwards: Sink into From Ground
//25 Basic Idle
//27 15 Taunt
//28 6 Basic Get Hit






