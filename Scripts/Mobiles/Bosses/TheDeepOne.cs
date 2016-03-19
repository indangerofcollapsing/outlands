using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Multis;

namespace Server.Mobiles
{
    [CorpseName("the deep one's corpse")]
    public class TheDeepOne : BaseCreature
    {
        public override string TitleReward { get { return "Slayer of The Deep One"; } }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextRevealAllowed;
        public TimeSpan NextRevealDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextMeleeAttackAllowed;
        public double NextMeleeAttackDelayMax = 5;
        public double AttackMaxReduction = 2.5;

        public DateTime m_NextSinkBelowAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(90);
        public TimeSpan NextSinkBelowDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(90, 120));

        public DateTime m_SinkBelowExpiration;

        public int SinkBelowDurationBase = 30;
        public int SinkBelowDurationMaxReduction = 25;        

        public DateTime m_NextNonCombatantAttackCheckAllowed;
        public TimeSpan NextNonCombatantAttackDelay = TimeSpan.FromSeconds(2);

        public TimeSpan CombatantTimeout = TimeSpan.FromSeconds(10);

        public DateTime m_LastBoatCombatantSelected;
        public TimeSpan BoatCombatantTimeout = TimeSpan.FromSeconds(10);       

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public bool AbilityInProgress = false;
        public bool DownBelow = false;

        public int MaxDistanceAllowedFromHome = 100;

        public int damageIntervalThreshold = 1000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 25;

        public List<Mobile> m_Creatures = new List<Mobile>();
        public BaseBoat m_BoatCombatant;

        public string[] idleSpeech
        {
            get { return new string[] {"*wades idly*"}; }
        }

        public string[] combatSpeech
        {
            get { return new string[] {""}; }
        }

        [Constructable]
        public TheDeepOne(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "The Deep One";

            Body = 1068;
            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(25000);
            SetStam(10000);

            SetDamage(20, 40);           

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.DetectHidden, 300);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 75;            

            CanSwim = true;
        }

        public override int OceanDoubloonValue { get { return 2000; } }
        public override bool IsOceanCreature { get { return true; } }

        public override int AttackRange { get { return 3; } }

        public override bool RevealImmune { get { return true; } }
        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "The Deep One has arisen and stirs at sea..."; } }
        public override bool AlwaysMurderer { get { return true; } }        

        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 1.5;

            //Has Manually Performed Melees
            NextCombatTime = DateTime.UtcNow + TimeSpan.FromDays(365);

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            int minWater = 1;
            int maxWater = 1;

            if (amount > 50)                
                maxWater += 1;                

            if (amount > 100)            
                maxWater += 1;

            if (Utility.RandomDouble() <= (double)amount / 100)
            {
                int waterCount = Utility.RandomMinMax(minWater, maxWater);

                for (int a = 0; a < waterCount; a++)
                {
                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 5);
                    water.Name = "water";
                    water.Hue = 2120;

                    Point3D waterLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                    SpellHelper.AdjustField(ref waterLocation, Map, 12, false);

                    water.MoveToWorld(waterLocation);
                }
            }

            damageProgress += amount;

            if (damageProgress >= damageIntervalThreshold)
            {
                Effects.PlaySound(Location, Map, 0x656);

                damageProgress = 0;

                if (intervalCount % 5 == 0)
                    SpawnShipTentacles();

                else
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {                       
                        case 1: WaveAttack(); break;
                        case 2: GustAttack(); break;
                        case 3: StormAttack(); break;
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
        }        

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (attacker == null) return;
            if (attacker.Deleted) return;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            BaseBoat sourceBoat = null;
            Point3D location = attacker.Location;
            Map map = attacker.Map;

            if (bc_Attacker != null)
            {
                if (bc_Attacker.BoatOccupied != null)
                {
                    if (!bc_Attacker.BoatOccupied.Deleted && bc_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = bc_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = bc_Attacker.Map;
                    }
                }
            }

            if (pm_Attacker != null)
            {
                if (pm_Attacker.BoatOccupied != null)
                {
                    if (!pm_Attacker.BoatOccupied.Deleted && pm_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = pm_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = pm_Attacker.Map;
                    }
                }
            }

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon != null)
            {
                //Ranged Weapon
                if (weapon is BaseRanged)
                {
                    if (Utility.RandomDouble() <= .06)
                    {
                        Animate(15, 10, 1, true, false, 0); //Block
                        PlaySound(GetAngerSound());

                        double creatureChance = Utility.RandomDouble();

                        if (creatureChance <= .33)
                            SpawnDeepWater(location, map);

                        else if (creatureChance <= .66)
                            SpawnDeepCrab(location, map);

                        else
                            SpawnTentacle(location, map);
                    }
                }

                //Melee Weapon
                else if (weapon is BaseMeleeWeapon || weapon is Fists)
                {
                    if (Utility.RandomDouble() <= .02)
                    {
                        Animate(15, 10, 1, true, false, 0); //Block
                        PlaySound(GetAngerSound());

                        double creatureChance = Utility.RandomDouble();

                        if (creatureChance <= .33)
                            SpawnDeepWater(location, map);

                        else if (creatureChance <= .66)
                            SpawnDeepCrab(location, map);

                        else
                            SpawnTentacle(location, map);
                    }
                }
            }

            base.OnGotMeleeAttack(attacker);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from == null) return;
            if (from.Deleted) return;

            BaseCreature bc_Attacker = from as BaseCreature;
            PlayerMobile pm_Attacker = from as PlayerMobile;

            BaseBoat sourceBoat = null;
            Point3D location = from.Location;
            Map map = from.Map;

            if (bc_Attacker != null)
            {
                if (bc_Attacker.BoatOccupied != null)
                {
                    if (!bc_Attacker.BoatOccupied.Deleted && bc_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = bc_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = bc_Attacker.Map;
                    }
                }
            }

            if (pm_Attacker != null)
            {
                if (pm_Attacker.BoatOccupied != null)
                {
                    if (!pm_Attacker.BoatOccupied.Deleted && pm_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = pm_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = pm_Attacker.Map;
                    }
                }
            }

            if (Utility.RandomDouble() <= .1)
            {
                Animate(15, 10, 1, true, false, 0); //Block
                PlaySound(GetAngerSound());

                double creatureChance = Utility.RandomDouble();

                if (creatureChance <= .33)
                    SpawnDeepWater(location, map);

                else if (creatureChance <= .66)
                    SpawnDeepCrab(location, map);

                else
                    SpawnTentacle(location, map);
            }

            base.OnDamagedBySpell(from);
        }

        public override void OnGotCannonHit(int amount, Mobile from, bool willKill)
        {
            if (willKill || from == null)
                return;

            BaseCreature bc_Attacker = from as BaseCreature;
            PlayerMobile pm_Attacker = from as PlayerMobile;

            BaseBoat sourceBoat = null;
            Point3D location = from.Location;
            Map map = from.Map;

            if (bc_Attacker != null)
            {
                if (bc_Attacker.BoatOccupied != null)
                {
                    if (!bc_Attacker.BoatOccupied.Deleted && bc_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = bc_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = bc_Attacker.Map;
                    }
                }
            }

            if (pm_Attacker != null)
            {
                if (pm_Attacker.BoatOccupied != null)
                {
                    if (!pm_Attacker.BoatOccupied.Deleted && pm_Attacker.BoatOccupied.m_SinkTimer == null)
                    {
                        location = pm_Attacker.BoatOccupied.GetRandomEmbarkLocation(true);
                        map = pm_Attacker.Map;
                    }
                }
            }

            if (Utility.RandomDouble() <= (double)amount / 100)
            {
                if (Utility.RandomDouble() <= .06)
                {
                    Animate(15, 10, 1, true, false, 0); //Block
                    PlaySound(GetAngerSound());

                    double creatureChance = Utility.RandomDouble();

                    if (creatureChance <= .33)
                        SpawnDeepWater(location, map);

                    else if (creatureChance <= .66)
                        SpawnDeepCrab(location, map);

                    else
                        SpawnTentacle(location, map);
                }
            }
        }

        public void SpawnDeepWater(Point3D location, Map map)
        {
            Animate(15, 10, 1, true, false, 0); //Block

            DeepWater deepWater = new DeepWater();

            deepWater.MoveToWorld(location, map);
            deepWater.PlaySound(0x027);
            deepWater.PublicOverheadMessage(MessageType.Regular, 0, false, "*water begins to coalesce*");

            Effects.SendLocationParticles(EffectItem.Create(deepWater.Location, deepWater.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);

            m_Creatures.Add(deepWater);
        }

        public void SpawnDeepCrab(Point3D location, Map map)
        {
            DeepCrab deepCrab = new DeepCrab();

            deepCrab.MoveToWorld(location, map);
            deepCrab.PlaySound(0x027);
            deepCrab.PublicOverheadMessage(MessageType.Regular, 0, false, "*bursts from the below decks*");

            int projectiles = Utility.RandomMinMax(3, 5);
            int particleSpeed = 8;

            for (int a = 0; a < projectiles; a++)
            {
                int debrisOffsetX = 0;
                int debrisOffsetY = 0;

                switch (Utility.RandomMinMax(1, 11))
                {
                    case 1: debrisOffsetX = -5; debrisOffsetY = 5; break;
                    case 2: debrisOffsetX = -4; debrisOffsetY = 4; break;
                    case 3: debrisOffsetX = -3; debrisOffsetY = 3; break;
                    case 4: debrisOffsetX = -2; debrisOffsetY = 2; break;
                    case 5: debrisOffsetX = -1; debrisOffsetY = 1; break;
                    case 6: debrisOffsetX = 0; debrisOffsetY = 0; break;
                    case 7: debrisOffsetX = 1; debrisOffsetY = -1; break;
                    case 8: debrisOffsetX = 2; debrisOffsetY = -2; break;
                    case 9: debrisOffsetX = 3; debrisOffsetY = -3; break;
                    case 10: debrisOffsetX = 4; debrisOffsetY = -4; break;
                    case 11: debrisOffsetX = 5; debrisOffsetY = -5; break;
                }

                Point3D targetPoint = deepCrab.Location;
                Point3D newLocation = new Point3D(targetPoint.X + debrisOffsetX, targetPoint.Y + debrisOffsetY, targetPoint.Z);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetPoint.X, targetPoint.Y, targetPoint.Z + 2), deepCrab.Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 30), deepCrab.Map);

                newLocation.Z += 5;

                Effects.PlaySound(targetPoint, deepCrab.Map, 0x50F);
                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), particleSpeed, 0, false, false, 0, 0);
            }

            m_Creatures.Add(deepCrab);
        }

        public void SpawnTentacle(Point3D location, Map map)
        {
            DeepTentacle deepOneTentacle = new DeepTentacle();

            deepOneTentacle.MoveToWorld(location, map);
            deepOneTentacle.PlaySound(0x353);
            deepOneTentacle.PublicOverheadMessage(MessageType.Regular, 0, false, "*rises from the deep*");

            for (int a = 0; a < 4; a++)
            {
                TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                water.Name = "tentacle slime";
                water.Hue = 2609;

                Point3D slimeLocation = new Point3D(location.X + Utility.RandomList(-1, 1), location.Y + Utility.RandomList(-1, 1), location.Z);
                water.MoveToWorld(location, deepOneTentacle.Map);

                Effects.PlaySound(deepOneTentacle.Location, deepOneTentacle.Map, 0x027);
            }

            m_Creatures.Add(deepOneTentacle);
        }

        public void SpawnShipTentacles()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            Animate(28, 12, 1, true, false, 0);

            Effects.PlaySound(Location, Map, 0x667);
            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 3, true, 0, false, "", "");

            PublicOverheadMessage(MessageType.Regular, 0, false, "*calls for aid from the deep*");

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int tentacles = 1 + (int)(Math.Ceiling(5 * spawnPercent));

            foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
            {
                if (targetBoat.Deleted) continue;

                if (targetBoat.m_SinkTimer != null)
                {
                    if (targetBoat.m_SinkTimer.Running)
                        continue;
                }

                int distance = Utility.GetDistance(Location, targetBoat.Location);

                if (distance > 30)
                    continue;

                if (targetBoat.MobileControlType != MobileControlType.Player)
                    continue;

                for (int a = 0; a < tentacles; a++)
                {
                    SpawnTentacle(targetBoat.GetRandomEmbarkLocation(true), targetBoat.Map);
                }
            }
        }

        public void GustAttack()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loops = 1 + (int)(Math.Ceiling(3 * spawnPercent));

            double stationaryDelay = loops + 1;            
            
            int radius = 10 + (int)(Math.Ceiling(20 * spawnPercent));
            int gustStrength = 10 + (int)(Math.Ceiling(20 * spawnPercent));

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                AbilityInProgress = false;
            });

            PlaySound(0x64F);

            AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);
            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, stationaryDelay, true, 0, false, "", "");

            Animate(30, 12, 1, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes the fury of the winds*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
            {
                if (this == null) return;
                if (this.Deleted) return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;

                        Animate(30, 12, 1, true, false, 0);

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
                       foreach (Point3D gustTargetLocation in m_GustTargetLocations)
                       {
                           if (Utility.RandomDouble() <= .1)
                               continue;

                           IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), map);
                           IEntity endLocation = new Entity(Serial.Zero, new Point3D(gustTargetLocation.X, gustTargetLocation.Y, gustTargetLocation.Z + 5), map);

                           int particleSpeed = 5 + (int)(Math.Round(5 * (double)spawnPercent));

                           Effects.SendMovingEffect(startLocation, endLocation, 0x1FB7, 5, 0, false, false, 0, 0);

                           Effects.PlaySound(startLocation, map, 0x4F1);
                           Effects.PlaySound(gustTargetLocation, map, 0x4F1);
                       }
                   });
                }

                foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                {
                    if (targetBoat.Deleted) continue;

                    if (targetBoat.m_SinkTimer != null)
                    {
                        if (targetBoat.m_SinkTimer.Running)
                            continue;
                    }

                    int distance = targetBoat.GetBoatToLocationDistance(targetBoat, location);

                    if (distance > radius)
                        continue;

                    if (targetBoat.MobileControlType != MobileControlType.Player)
                        continue;

                    //Ship Impact
                    double targetBoatDistance = Utility.GetDistanceToSqrt(location, targetBoat.Location);
                    double targetBoatDelay = (double)distance * .125;
                    double impactBoatnDelay = targetBoatDelay - (targetBoatDelay * .5 * spawnPercent);

                    Timer.DelayCall(TimeSpan.FromSeconds(targetBoatDelay), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;
                        if (targetBoat == null) return;
                        if (targetBoat.Deleted || targetBoat.m_SinkTimer != null) return;

                        if (targetBoat.TillerMan != null)
                        {
                            if (!targetBoat.TillerMan.Deleted)
                                targetBoat.TillerMan.Say("Yar, the wind be tearing our ship an' sails ta' shreds!");
                        }

                        int debrisCount = (int)(Math.Ceiling((double)gustStrength / 2));

                        for (int b = 0; b < debrisCount; b++)
                        {
                            Blood debris = new Blood();
                            debris.Name = "debris";
                            debris.ItemID = Utility.RandomList(8766, 8767, 8768, 8769, 8770, 8771, 8772, 8773, 8774, 8775, 8776, 8777);
                            debris.Hue = 2498;

                            Point3D debrisLocation = targetBoat.GetRandomEmbarkLocation(false);
                            SpellHelper.AdjustField(ref debrisLocation, map, 12, false);

                            debris.MoveToWorld(debrisLocation, map);

                            Effects.PlaySound(debrisLocation, map, 0x235);
                        }

                        double shipDamage = gustStrength * Utility.RandomMinMax(20, 30);

                        if (targetBoat.SailPoints > 0)
                        {
                            shipDamage *= .75;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Sails);

                            shipDamage *= .25;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Hull);
                        }

                        else
                        {
                            shipDamage *= .5;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Hull);
                        }
                    });

                    //Mobile Impacts
                    Queue m_Queue = new Queue();

                    foreach (Mobile mobile in targetBoat.GetMobilesOnBoat(false, false))
                    {
                        if (mobile == this) continue;
                        if (!mobile.CanBeDamaged()) continue;

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

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double targetMobileDistance = Utility.GetDistanceToSqrt(location, mobile.Location);
                        double targetMobileDelay = (double)distance * .125;
                        double impactMobileDelay = targetMobileDelay - (targetMobileDelay * .5 * spawnPercent);

                        Timer.DelayCall(TimeSpan.FromSeconds(impactMobileDelay), delegate
                        {
                            if (this == null) return;
                            if (Deleted || !Alive) return;
                            if (mobile == null) return;
                            if (mobile.Deleted || !mobile.Alive) return;

                            Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                            double damage = DamageMin;

                            SpecialAbilities.KnockbackSpecialAbility(1.0, location, null, mobile, damage, 6, -1, "", "A gust of air knocks you off your feet!");
                        });
                    }
                }
            });
        }

        public void WaveAttack()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loops = 1 + (int)(Math.Ceiling(3 * spawnPercent));

            double stationaryDelay = loops + 1; 

            int radius = 10 + (int)(Math.Ceiling(20 * spawnPercent));
            int waveStrength = 10 + (int)(Math.Ceiling(20 * spawnPercent));

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                AbilityInProgress = false;
            });
            
            Effects.PlaySound(Location, Map, 0x656);            

            AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);
            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, stationaryDelay, true, 0, false, "", "");

            Animate(27, 10, 1, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes the fury of the sea*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
            {
                if (this == null) return;
                if (this.Deleted) return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;

                        Animate(27, 10, 1, true, false, 0);

                        PlaySound(GetAngerSound());
                    });
                }

                int minRange = -1 * radius;
                int maxRange = radius + 1;

                for (int a = minRange; a < maxRange; a++)
                {
                    for (int b = minRange; b < maxRange; b++)
                    {
                        Point3D newLocation = new Point3D(Location.X + a, Location.Y + b, Location.Z);

                        double distance = Utility.GetDistanceToSqrt(location, newLocation);
                        double distanceDelay = (distance * .125);
                        double impactDelay = distanceDelay - (distanceDelay * .5 * spawnPercent);

                        Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                        {
                            if (Utility.RandomDouble() <= .66)
                            {
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);
                            }
                        });

                        Timer.DelayCall(TimeSpan.FromSeconds(impactDelay + 1.25), delegate
                        {
                            if (Utility.RandomDouble() <= .33)
                            {
                                BaseBoat boatCheck = BaseBoat.FindBoatAt(newLocation, map);

                                if (boatCheck == null)
                                {
                                    if (BaseBoat.IsWaterTile(newLocation, map))
                                        Effects.SendLocationEffect(newLocation, map, 0x352D, 7);
                                }

                                else
                                {
                                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                                    water.Name = "water";
                                    water.Hue = 2120;

                                    Effects.PlaySound(newLocation, map, 0x027);
                                    newLocation.Z = boatCheck.Z + 2;

                                    water.MoveToWorld(newLocation, map);
                                }
                            }
                        });
                    }
                }

                foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                {
                    if (targetBoat.Deleted) continue;

                    if (targetBoat.m_SinkTimer != null)
                    {
                        if (targetBoat.m_SinkTimer.Running)
                            continue;
                    }

                    int distance = Utility.GetDistance(Location, targetBoat.Location);

                    if (distance > radius)
                        continue;

                    if (targetBoat.MobileControlType != MobileControlType.Player)
                        continue;

                    //Tillerman Announcement
                    double targetTillermanDistance = Utility.GetDistanceToSqrt(location, targetBoat.Location);
                    double targetTillermanDelay = (double)distance * .125;
                    double impactTillermanDelay = targetTillermanDelay - (targetTillermanDelay * .5 * spawnPercent);

                    Timer.DelayCall(TimeSpan.FromSeconds(impactTillermanDelay), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;
                        if (targetBoat == null) return;
                        if (targetBoat.Deleted || targetBoat.m_SinkTimer != null) return;

                        if (targetBoat.TillerMan != null)
                        {
                            if (!targetBoat.TillerMan.Deleted)
                                targetBoat.TillerMan.Say("Yar, the seas be flooding our ship an' soakin' our gunpowder!");
                        }

                        //Mobile Impacts
                        Queue m_Queue = new Queue();

                        foreach (Mobile mobile in targetBoat.GetMobilesOnBoat(false, false))
                        {
                            if (mobile == this) continue;
                            if (!mobile.CanBeDamaged()) continue;

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

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            double targetMobileDistance = Utility.GetDistanceToSqrt(location, mobile.Location);
                            double targetMobileDelay = (double)distance * .125;
                            double impactMobileDelay = targetMobileDelay - (targetMobileDelay * .5 * spawnPercent);

                            Timer.DelayCall(TimeSpan.FromSeconds(impactMobileDelay), delegate
                            {
                                if (this == null) return;
                                if (Deleted || !Alive) return;
                                if (mobile == null) return;
                                if (mobile.Deleted || !mobile.Alive) return;

                                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                                double damage = DamageMin;

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);
                            });
                        }

                        double shipDamage = waveStrength * Utility.RandomMinMax(20, 30);

                        if (targetBoat.GunPoints > 0)
                        {
                            shipDamage *= .75;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Guns);

                            shipDamage *= .25;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Hull);
                        }

                        else
                        {
                            shipDamage *= .5;
                            targetBoat.ReceiveDamage(null, null, (int)(Math.Ceiling(shipDamage)), DamageType.Hull);
                        }
                    });
                } 
            });                       
        }

        public void StormAttack()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 30;
            int cycles = 10 + (int)(Math.Ceiling(20 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)cycles / 10));

            double stationaryDelay = loops + 2.5;

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                AbilityInProgress = false;
            });

            PlaySound(0x64F);

            AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);
            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, stationaryDelay, true, 0, false, "", "");

            Animate(28, 12, 1, true, false, 0);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes the fury of the skies*");

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
            {
                if (this == null) return;
                if (this.Deleted) return;

                for (int a = 0; a < loops; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;

                        Animate(30, 12, 1, true, false, 0);

                        PlaySound(GetAngerSound());
                    });
                }

                foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                {
                    if (targetBoat.Deleted) continue;

                    if (targetBoat.m_SinkTimer != null)
                    {
                        if (targetBoat.m_SinkTimer.Running)
                            continue;
                    }

                    int distance = Utility.GetDistance(Location, targetBoat.Location);

                    if (distance > 30)
                        continue;

                    if (targetBoat.MobileControlType != MobileControlType.Player)
                        continue;

                    for (int a = 0; a < cycles; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * .125), delegate
                        {
                            if (this == null) return;
                            if (Deleted || !Alive) return;

                            List<Mobile> m_PossibleMobiles = new List<Mobile>();

                            foreach (Mobile mobile in targetBoat.GetMobilesOnBoat(false, false))
                            {
                                if (mobile == null) continue;
                                if (mobile == this) continue;
                                if (!mobile.CanBeDamaged()) continue;
                                if (mobile.Hidden) continue;

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

                                m_PossibleMobiles.Add(mobile);
                            }

                            if (m_PossibleMobiles.Count == 0)
                                return;

                            Mobile target = m_PossibleMobiles[Utility.RandomMinMax(0, m_PossibleMobiles.Count - 1)];

                            BoltEffect(0);
                            PlaySound(0);

                            target.BoltEffect(0);
                            target.PlaySound(0);

                            double damage = Utility.RandomMinMax(5, 15);

                            if (target is BaseCreature)
                                damage *= 2;

                            new Blood().MoveToWorld(target.Location, target.Map);
                            AOS.Damage(target, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);
                        });
                    }
                }
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            //Outside of Valid Combat Zone
            if (Utility.GetDistance(Home, Location) > MaxDistanceAllowedFromHome && !DownBelow && !AbilityInProgress)
            {
                Effects.PlaySound(Location, Map, 0x5A4);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*sinks beneath the waves and returns to it's domain*");

                int radius = 2;

                int minRange = -1 * radius;
                int maxRange = radius + 1;

                Point3D location = Location;
                Map map = Map;

                for (int a = minRange; a < maxRange; a++)
                {
                    for (int b = minRange; b < maxRange; b++)
                    {
                        Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);

                        double distance = Utility.GetDistanceToSqrt(location, newLocation);
                        double distanceDelay = distance * .25;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);
                        });

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay + 1.25), delegate
                        {
                            if (Utility.RandomDouble() <= .66)
                            {
                                BaseBoat boatCheck = BaseBoat.FindBoatAt(newLocation, map);

                                if (boatCheck == null)
                                {
                                    if (BaseBoat.IsWaterTile(newLocation, map))
                                        Effects.SendLocationEffect(newLocation, map, 0x352D, 7);
                                }

                                else
                                {
                                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                                    water.Name = "water";
                                    water.Hue = 2120;

                                    Effects.PlaySound(newLocation, map, 0x027);
                                    newLocation.Z = boatCheck.Z + 2;

                                    water.MoveToWorld(newLocation, map);
                                }
                            }
                        });
                    }
                }

                Location = Home;
                Combatant = null;
                m_BoatCombatant = null;

                return;
            }

            //Reveal Hidden)
            if (m_NextRevealAllowed <= DateTime.UtcNow && !DownBelow && !AbilityInProgress)
            {
                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 30);

                bool creatureWasRevealed = false;

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validMobile = false;

                    BaseCreature bc_Creature = mobile as BaseCreature;
                    PlayerMobile pm_Player = mobile as PlayerMobile;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                            validMobile = true;
                    }

                    if (pm_Player != null)
                        validMobile = true;

                    if (validMobile)
                    {
                        if (mobile.Hidden)
                        {
                            mobile.RevealingAction();
                            mobile.SendMessage("You have been revealed by The Deep One.");
                            creatureWasRevealed = true;

                            Effects.PlaySound(mobile.Location, mobile.Map, 0x652);
                            mobile.FixedParticles(0x373A, 10, 15, 5036, 0, 0, EffectLayer.Head);
                        }
                    }
                }

                nearbyMobiles.Free();

                if (creatureWasRevealed)
                    PublicOverheadMessage(MessageType.Regular, 0x482, false, "NOTHING IN MY DOMAIN ESCAPES MY SIGHT!");

                m_NextRevealAllowed = DateTime.UtcNow + NextRevealDelay;

                return;
            }

            //Change AI Targeting
            if (m_NextAIChangeAllowed <= DateTime.UtcNow && !DownBelow && !AbilityInProgress)
            {
                Combatant = null;

                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*groans*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*gurgle*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*bellows*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*agitates nearby waves*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*thrashes about violently*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;

                return;
            }

            //Sink Below the Waves
            if (m_NextSinkBelowAllowed <= DateTime.UtcNow && !DownBelow && !AbilityInProgress)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*sinks below the waves*");

                int radius = 2;

                int minRange = -1 * radius;
                int maxRange = radius + 1;

                Point3D location = Location;
                Map map = Map;

                for (int a = minRange; a < maxRange; a++)
                {
                    for (int b = minRange; b < maxRange; b++)
                    {
                        Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);

                        double distance = Utility.GetDistanceToSqrt(location, newLocation);
                        double distanceDelay = distance * .25;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);
                        });

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay + 1.25), delegate
                        {
                            if (Utility.RandomDouble() <= .66)
                            {
                                BaseBoat boatCheck = BaseBoat.FindBoatAt(newLocation, map);

                                if (boatCheck == null)
                                {
                                    if (BaseBoat.IsWaterTile(newLocation, map))
                                        Effects.SendLocationEffect(newLocation, map, 0x352D, 7);
                                }

                                else
                                {
                                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                                    water.Name = "water";
                                    water.Hue = 2120;

                                    Effects.PlaySound(newLocation, map, 0x027);
                                    newLocation.Z = boatCheck.Z + 2;

                                    water.MoveToWorld(newLocation, map);
                                }
                            }
                        });
                    }
                }

                DownBelow = true;
                Blessed = true;
                Hidden = true;

                double submergeDuration = Math.Round(SinkBelowDurationBase - (SinkBelowDurationMaxReduction * spawnPercent));

                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, submergeDuration, true, 0, false, "", "");

                m_NextSinkBelowAllowed = DateTime.UtcNow + NextSinkBelowDelay;
                m_SinkBelowExpiration = DateTime.UtcNow + TimeSpan.FromSeconds(submergeDuration);

                return;
            }

            //Rise From The Sea
            if (DownBelow && m_SinkBelowExpiration <= DateTime.UtcNow && !AbilityInProgress)
            {
                Effects.PlaySound(Location, Map, 0x668);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*rises from below the waves*");

                DownBelow = false;
                Blessed = false;
                Hidden = false;

                Combatant = null;
                m_BoatCombatant = null;

                List<BaseBoat> m_PossibleBoatCombatants = new List<BaseBoat>();

                foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                {
                    if (targetBoat.Deleted) continue;
                    if (targetBoat.MobileControlType != MobileControlType.Player) continue;

                    if (targetBoat.m_SinkTimer != null)
                    {
                        if (targetBoat.m_SinkTimer.Running)
                            continue;
                    }

                    int distance = Utility.GetDistance(Location, targetBoat.Location);

                    if (distance > 30)
                        continue;

                    if (targetBoat.MobileControlType != MobileControlType.Player)
                        continue;

                    m_PossibleBoatCombatants.Add(targetBoat);
                }

                if (m_PossibleBoatCombatants.Count > 0)
                {
                    Location = m_PossibleBoatCombatants[Utility.RandomMinMax(0, m_PossibleBoatCombatants.Count - 1)].Location;

                    int radius = 2;

                    int minRange = -1 * radius;
                    int maxRange = radius + 1;

                    Point3D location = Location;
                    Map map = Map;

                    for (int a = minRange; a < maxRange; a++)
                    {
                        for (int b = minRange; b < maxRange; b++)
                        {
                            Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);

                            double distance = Utility.GetDistanceToSqrt(location, newLocation);
                            double distanceDelay = distance * .25;

                            Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                            {                               
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);
                            });

                            Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay + 1.25), delegate
                            {
                                if (Utility.RandomDouble() <= .66)
                                {
                                    BaseBoat boatCheck = BaseBoat.FindBoatAt(newLocation, map);

                                    if (boatCheck == null)
                                    {
                                        if (BaseBoat.IsWaterTile(newLocation, map))
                                            Effects.SendLocationEffect(newLocation, map, 0x352D, 7);
                                    }

                                    else
                                    {
                                        TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                                        water.Name = "water";
                                        water.Hue = 2120;

                                        Effects.PlaySound(newLocation, map, 0x027);
                                        newLocation.Z = boatCheck.Z + 2;

                                        water.MoveToWorld(newLocation, map);
                                    }
                                }
                            });
                        }
                    }
                }
            }

            //Valid Mobile Combatant
            if (Combatant != null && !DownBelow && !AbilityInProgress)
            {
                if (Combatant.Hidden || Combatant.Deleted || !Combatant.Alive || Utility.GetDistance(Location, Combatant.Location) > 30)
                    Combatant = null;

                BaseCreature bc_Combatant = Combatant as BaseCreature;
                PlayerMobile pm_Combatant = Combatant as PlayerMobile;

                //If Combatant is on Boat, Set Boat to Be Target Boat
                if (bc_Combatant != null)
                {
                    if (bc_Combatant.BoatOccupied != null)
                    {
                        if (!bc_Combatant.BoatOccupied.Deleted && bc_Combatant.BoatOccupied.m_SinkTimer == null)
                            m_BoatCombatant = bc_Combatant.BoatOccupied;
                    }
                }

                if (pm_Combatant != null)
                {
                    if (pm_Combatant.BoatOccupied != null)
                    {
                        if (!pm_Combatant.BoatOccupied.Deleted && pm_Combatant.BoatOccupied.m_SinkTimer == null)
                            m_BoatCombatant = pm_Combatant.BoatOccupied;
                    }
                }

                //If in Range of Target Mobile, Don't Move
                if (Combatant != null)
                {
                    if (Utility.GetDistance(Location, Combatant.Location) <= 3)
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, .5, true, 0, false, "", "");
                }

                //If In Range of BoatCombatant, Don't Move
                if (m_BoatCombatant != null)
                {
                    if (!m_BoatCombatant.Deleted && m_BoatCombatant.m_SinkTimer == null && m_BoatCombatant.GetBoatToLocationDistance(m_BoatCombatant, Location) <= 4)
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, .5, true, 0, false, "", "");
                }
            }

            //No Normal Combatant
            if (Combatant == null && !DownBelow && !AbilityInProgress)
            {
                //Haven't Made an Attack in A While
                if (LastCombatTime + CombatantTimeout < DateTime.UtcNow)
                {
                    //Validate Boat Combatant
                    if (m_BoatCombatant != null)
                    {
                        if (m_BoatCombatant.Deleted || m_BoatCombatant.m_SinkTimer != null || Utility.GetDistance(Location, m_BoatCombatant.Location) > 30)
                            m_BoatCombatant = null;

                        else if (m_LastBoatCombatantSelected + BoatCombatantTimeout > DateTime.UtcNow)                        
                            m_BoatCombatant = null;
                    }

                    //Determine Ship Combatant
                    if (m_BoatCombatant == null)
                    {
                        m_LastBoatCombatantSelected = DateTime.UtcNow;

                        Dictionary<BaseBoat, int> m_PossibleBoatCombatants = new Dictionary<BaseBoat, int>();

                        foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                        {
                            if (targetBoat.Deleted) continue;
                            if (targetBoat.MobileControlType != MobileControlType.Player) continue;

                            if (targetBoat.m_SinkTimer != null)
                            {
                                if (targetBoat.m_SinkTimer.Running)
                                    continue;
                            }

                            int distance = Utility.GetDistance(Location, targetBoat.Location);

                            if (distance > 30) continue;

                            int weightValue = 0;
                            int distanceWeight = 0;
                            double hullPercentLost = 1 - ((double)targetBoat.HitPoints / (double)targetBoat.MaxHitPoints);

                            weightValue += (int)(hullPercentLost * 10);
                            weightValue += 11 - (int)(Math.Ceiling((double)distance / 3));

                            m_PossibleBoatCombatants.Add(targetBoat, weightValue);
                        }

                        if (m_PossibleBoatCombatants.Count > 0)
                        {
                            int TotalValues = 0;

                            foreach (KeyValuePair<BaseBoat, int> pair in m_PossibleBoatCombatants)
                            {
                                TotalValues += pair.Value;
                            }

                            double ActionCheck = Utility.RandomDouble();
                            double CumulativeAmount = 0.0;
                            double AdditionalAmount = 0.0;

                            bool foundDirection = true;

                            foreach (KeyValuePair<BaseBoat, int> pair in m_PossibleBoatCombatants)
                            {
                                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                                //Set Ship Target
                                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                                {
                                    m_BoatCombatant = pair.Key;
                                    break;
                                }

                                CumulativeAmount += AdditionalAmount;
                            }
                        }
                    }
                }

                //Move Towards Target Boat
                if (m_BoatCombatant != null && !CantWalk && !Frozen && AIObject.NextMove <= DateTime.UtcNow)
                {
                    if (m_BoatCombatant.m_SinkTimer == null && !m_BoatCombatant.Deleted && m_BoatCombatant.GetBoatToLocationDistance(m_BoatCombatant, Location) > 4)
                    {
                        AIObject.WalkToLocation(m_BoatCombatant.Location, 1, false, 0, 0);
                        DelayNextMovement(CurrentSpeed);   
                    }

                    else
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, .5, true, 0, false, "", "");   
                }
            }

            //Melee Attack: Manually Performed
            if (m_NextMeleeAttackAllowed <= DateTime.UtcNow && !DownBelow && !AbilityInProgress)
            {
                bool validAttackTarget = false;
                
                if (Combatant != null)
                {
                    if (Utility.GetDistance(Location, Combatant.Location) <= 3)
                    {
                        validAttackTarget = true;
                        MeleeAttackMobile(Combatant);
                    }
                }

                if (!validAttackTarget && m_BoatCombatant != null)
                {
                    int distance =  m_BoatCombatant.GetBoatToLocationDistance(m_BoatCombatant, Location);

                    if (m_BoatCombatant.m_SinkTimer == null && !m_BoatCombatant.Deleted && distance <= 4)
                    {
                        validAttackTarget = true;
                        MeleeAttackShip(m_BoatCombatant);
                    }
                }

                if (!validAttackTarget)
                {
                    if (m_NextNonCombatantAttackCheckAllowed <= DateTime.UtcNow)
                    {
                        validAttackTarget = false;

                        List<Mobile> m_NearbyMobiles = new List<Mobile>();

                        IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 3);

                        foreach (Mobile mobile in nearbyMobiles)
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
                                m_NearbyMobiles.Add(mobile);
                        }

                        nearbyMobiles.Free();

                        if (m_NearbyMobiles.Count > 0)
                        {
                            validAttackTarget = true;
                            Combatant = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];
                            MeleeAttackMobile(Combatant);

                            LastCombatTime = DateTime.UtcNow;
                        }

                        if (!validAttackTarget)
                        {
                            List<BaseBoat> m_NearbyBoats = new List<BaseBoat>();

                            foreach (BaseBoat targetBoat in BaseBoat.AllBoatInstances)
                            {
                                if (targetBoat.Deleted) continue;
                                if (targetBoat.MobileControlType != MobileControlType.Player) continue;

                                int distance = Utility.GetDistance(Location, targetBoat.Location);
                                if (distance > 4) continue;   

                                if (targetBoat.m_SinkTimer != null)
                                {
                                    if (targetBoat.m_SinkTimer.Running)
                                        continue;
                                }                          

                                m_NearbyBoats.Add(targetBoat);
                            }

                            if (m_NearbyBoats.Count > 0)
                                MeleeAttackShip(m_NearbyBoats[Utility.RandomMinMax(0, m_NearbyBoats.Count - 1)]);
                        }                        

                        m_NextNonCombatantAttackCheckAllowed = DateTime.UtcNow + NextNonCombatantAttackDelay;
                    }
                }
            }

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed && !DownBelow && !AbilityInProgress)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }         
        }

        public void MeleeAttackMobile(Mobile target)
        {
            if (target == null) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            double attackDelay = NextMeleeAttackDelayMax - (AttackMaxReduction * spawnPercent);

            m_NextMeleeAttackAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(attackDelay);
            LastCombatTime = DateTime.UtcNow;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 2, true, 0, false, "", "");

            Point3D targetLocation = target.Location;
            Map map = target.Map;

            Direction = Utility.GetDirection(Location, target.Location);    

            Timer.DelayCall(TimeSpan.FromSeconds(.2), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                Animate(4, 10, 1, true, false, 0); //Slam                
                PlaySound(GetAttackSound());
            });            

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                BaseBoat boatOccupied = null;
                
                Queue m_Queue = new Queue();

                if (target != null)
                {
                    if (!target.Deleted && target.Alive)
                        m_Queue.Enqueue(target);
                }

                IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(targetLocation, 1);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == this) continue;
                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                        continue;

                    if (target != null)
                    {
                        if (mobile == target)
                            continue;
                    }

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

                    double chanceToHit = .5;

                    if (validTarget && Utility.RandomDouble() <= chanceToHit)
                        m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    double damage = Utility.RandomMinMax(DamageMin, DamageMax);

                    BaseCreature bc_Target = mobile as BaseCreature;
                    PlayerMobile pm_Target = mobile as PlayerMobile;

                    if (bc_Target != null)
                    {
                        damage *= 2;

                        if (bc_Target.BoatOccupied != null)
                        {
                            if (!bc_Target.BoatOccupied.Deleted && bc_Target.BoatOccupied.m_SinkTimer == null)
                                boatOccupied = bc_Target.BoatOccupied;
                        }
                    }

                    if (pm_Target != null)
                    {
                        if (pm_Target.BoatOccupied != null)
                        {
                            if (!pm_Target.BoatOccupied.Deleted && pm_Target.BoatOccupied.m_SinkTimer == null)
                                boatOccupied = pm_Target.BoatOccupied;
                        }
                    }

                    //Manual Parry Handling
                    BaseShield shield = mobile.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                    if (shield != null)
                        damage = shield.OnHit(Weapon as BaseWeapon, (int)damage);

                    BaseWeapon weapon = mobile.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

                    if (!(weapon is BaseRanged) && weapon != null)
                        damage = weapon.WeaponParry(weapon, (int)damage, mobile);

                    if (Utility.RandomDouble() <= .5)
                        Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x137, 0x13B, 0x13D, 0x141));
                    else
                        Effects.PlaySound(mobile.Location, Map, 0x5A4);

                    Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);

                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 5);
                    water.Name = "water";
                    water.Hue = 2120;

                    Point3D waterLocation = new Point3D(mobile.Location.X + Utility.RandomList(-1, 1), mobile.Location.Y + Utility.RandomList(-1, 1), mobile.Location.Z);
                    SpellHelper.AdjustField(ref waterLocation, mobile.Map, 12, false);

                    water.MoveToWorld(waterLocation, mobile.Map);

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, (int)damage, 100, 100, 0, 0, 0);
                }

                if (boatOccupied != null)
                {
                    if (!boatOccupied.Deleted && boatOccupied.m_SinkTimer == null)
                    {
                        int projectiles = Utility.RandomMinMax(3, 5);

                        int particleSpeed = 8;

                        for (int a = 0; a < projectiles; a++)
                        {
                            int debrisOffsetX = 0;
                            int debrisOffsetY = 0;

                            switch (Utility.RandomMinMax(1, 11))
                            {
                                case 1: debrisOffsetX = -5; debrisOffsetY = 5; break;
                                case 2: debrisOffsetX = -4; debrisOffsetY = 4; break;
                                case 3: debrisOffsetX = -3; debrisOffsetY = 3; break;
                                case 4: debrisOffsetX = -2; debrisOffsetY = 2; break;
                                case 5: debrisOffsetX = -1; debrisOffsetY = 1; break;
                                case 6: debrisOffsetX = 0; debrisOffsetY = 0; break;
                                case 7: debrisOffsetX = 1; debrisOffsetY = -1; break;
                                case 8: debrisOffsetX = 2; debrisOffsetY = -2; break;
                                case 9: debrisOffsetX = 3; debrisOffsetY = -3; break;
                                case 10: debrisOffsetX = 4; debrisOffsetY = -4; break;
                                case 11: debrisOffsetX = 5; debrisOffsetY = -5; break;
                            }

                            Point3D targetPoint = boatOccupied.GetRandomEmbarkLocation(true);
                            Point3D newLocation = new Point3D(targetPoint.X + debrisOffsetX, targetPoint.Y + debrisOffsetY, targetPoint.Z);

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetPoint.X, targetPoint.Y, targetPoint.Z + 2), boatOccupied.Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 30), boatOccupied.Map);

                            newLocation.Z += 5;

                            Effects.PlaySound(targetPoint, boatOccupied.Map, 0x50F);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), particleSpeed, 0, false, false, 0, 0);
                        }

                        int minDamage = DamageMin;
                        int maxDamage = DamageMax;

                        int hullDamage = Utility.RandomMinMax(minDamage, maxDamage);

                        boatOccupied.ReceiveDamage(null, null, hullDamage, DamageType.Hull);
                    }
                }
            });
        }

        public void MeleeAttackShip(BaseBoat boat)
        {
            if (boat == null) return;
            if (boat.Deleted || boat.m_SinkTimer != null) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            double attackDelay = NextMeleeAttackDelayMax - (AttackMaxReduction * spawnPercent);

            m_NextMeleeAttackAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(attackDelay);
            LastCombatTime = DateTime.UtcNow;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 2, true, 0, false, "", "");

            Point3D targetLocation = boat.Location;
            Map map = boat.Map;

            Direction = Utility.GetDirection(Location, boat.Location);

            Timer.DelayCall(TimeSpan.FromSeconds(.2), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                Animate(29, 12, 1, true, false, 0); //Huge Swing
                PlaySound(GetAttackSound());
            });            

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                if (boat == null) return;
                if (boat.Deleted) return;

                if (boat.m_SinkTimer != null)
                {
                    if (boat.m_SinkTimer.Running)
                        return;
                }

                int projectiles = Utility.RandomMinMax(3, 5);

                int particleSpeed = 8;

                for (int a = 0; a < projectiles; a++)
                {
                    int debrisOffsetX = 0;
                    int debrisOffsetY = 0;

                    switch (Utility.RandomMinMax(1, 11))
                    {
                        case 1: debrisOffsetX = -5; debrisOffsetY = 5; break;
                        case 2: debrisOffsetX = -4; debrisOffsetY = 4; break;
                        case 3: debrisOffsetX = -3; debrisOffsetY = 3; break;
                        case 4: debrisOffsetX = -2; debrisOffsetY = 2; break;
                        case 5: debrisOffsetX = -1; debrisOffsetY = 1; break;
                        case 6: debrisOffsetX = 0; debrisOffsetY = 0; break;
                        case 7: debrisOffsetX = 1; debrisOffsetY = -1; break;
                        case 8: debrisOffsetX = 2; debrisOffsetY = -2; break;
                        case 9: debrisOffsetX = 3; debrisOffsetY = -3; break;
                        case 10: debrisOffsetX = 4; debrisOffsetY = -4; break;
                        case 11: debrisOffsetX = 5; debrisOffsetY = -5; break;
                    }

                    Point3D targetPoint = boat.GetRandomEmbarkLocation(true);
                    Point3D newLocation = new Point3D(targetPoint.X + debrisOffsetX, targetPoint.Y + debrisOffsetY, targetPoint.Z);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetPoint.X, targetPoint.Y, targetPoint.Z + 2), boat.Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 30), boat.Map);
                    
                    newLocation.Z += 5;

                    Effects.PlaySound(targetPoint, boat.Map, 0x50F);
                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), particleSpeed, 0, false, false, 0, 0);
                }

                int minDamage = DamageMin;
                int maxDamage = (int)(Math.Ceiling((double)DamageMax * 1.5));

                int hullDamage = Utility.RandomMinMax(minDamage, maxDamage);
                
                boat.ReceiveDamage(null, null, hullDamage, DamageType.Hull);
            });      
        }

        public override bool IsHighSeasBodyType { get { return true; } }
        public override bool HasAlternateHighSeasHurtAnimation { get { return true; } }

        public override int GetAngerSound() { return 0x4E2; }
        public override int GetIdleSound() { return 0x4E3; }
        public override int GetAttackSound() {return 0x626; }
        public override int GetHurtSound() { return 0x628; }
        public override int GetDeathSound() { return 0x4F5; }

        protected override bool OnMove(Direction d)
        {     
            if (Utility.RandomDouble() <= .33 && BoatOccupied == null)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x026, 0x025));
                Effects.SendLocationEffect(Location, Map, 0x352D, 7);
            }

            return base.OnMove(d);                       
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            BossPersistance.PersistanceItem.OceanBossLastStatusChange = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.AddItem(new Gold(10000));
            c.AddItem(ShipLoot.GetShipRare());
            
            if (Utility.RandomMinMax(1, 20) == 1)
                c.AddItem(new OrbFromTheDeep());

            if (Utility.RandomMinMax(1, 10) == 1)
                c.AddItem(new TheDeepOneStatue());            
            
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

        public TheDeepOne(Serial serial): base(serial)
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

            m_Creatures = new List<Mobile>();

            if (version >= 1)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    m_Creatures.Add(creature);
                }
            }

            Blessed = false;
            Hidden = false;
        }
    }
}

//Animate(4, 10, 1, true, false, 0); //Slam
//Animate(11, 10, 1, true, false, 0); //Grab
//Animate(12, 12, 1, true, false, 0); //Push
//Animate(15, 10, 1, true, false, 0); //Block
//Animate(23, 10, 1, true, false, 0); //Sink Below*
//Animate(27, 10, 1, true, false, 0); //Bellow
//Animate(28, 12, 1, true, false, 0); //Wave
//Animate(29, 12, 1, true, false, 0); //Huge Swing
//Animate(30, 12, 1, true, false, 0); //Taunt 