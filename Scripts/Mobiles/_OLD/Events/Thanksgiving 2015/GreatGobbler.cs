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
	[CorpseName( "great gobblers's corpse" )]
	public class GreatGobbler : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextEnrageAllowed;
        public TimeSpan EnrageDuration = TimeSpan.FromSeconds(15);
        public TimeSpan NextEnrageDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextHeartyWingBuffetAllowed;      
        public TimeSpan NextHeartyWingBuffetDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextRoastAllowed;
        public TimeSpan NextRoastDelay = TimeSpan.FromSeconds(30);
        
        public int NormalHue = 2635;
        public int EnragedHue = 2117;
        
        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(5);

        public int damageIntervalThreshold = 2000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_Creatures = new List<Mobile>();

		[Constructable]
		public GreatGobbler() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "The Great Gobbler";

            Body = 1026;
            Hue = NormalHue;

            BaseSoundID = 0x6E;

            SetStr(100);
            SetDex(100);
            SetInt(25);

            SetHits(40000);
            SetStam(40000);
            SetMana(0);

            AttackSpeed = 40;

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 75;

            Fame = 8000;
            Karma = -8000;
		}

        public override bool CanFly { get { return true; } }

        public override bool AlwaysBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {
            AttackSpeed = 40;
            SetDamage(20, 40);

            ActiveSpeed = .35;
            PassiveSpeed = .35;
            CurrentSpeed = .35;   

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

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            double bleedChance = .10 + (.10 * spawnPercent);

            if (defender is PlayerMobile)
            {
                int sound = 0;

                if (defender.Female)
                    sound = Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B);
                else
                    sound = Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x5F6, 0x436, 0x437, 0x43B, 0x43C);

                defender.PlaySound(sound);
            }

            if (Utility.RandomDouble() <= bleedChance)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*pecks with razor sharp beak*");

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, -1, true, "", "Their razor sharp beak causes you to bleed!");
            }

            Point3D defenderLocation = defender.Location;
            Map defenderMap = defender.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (defender == null) return;

                if (!defender.Alive)
                {
                    defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*turned to giblets*");

                    FeatherExplosion(defenderLocation, defenderMap, 20);
                    DamageCorpse(defenderLocation, defenderMap, true);
                }
            });
        }

        public static void FeatherExplosion(Point3D location, Map map, int feathers)
        {
            int projectiles = feathers;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Z);
                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3578, 3579), particleSpeed, 0, false, false, 0, 0);
            }
        }

        public static void DamageCorpse(Point3D location, Map map, bool largeExplosion)
        {
            int projectiles = 5;
            int particleSpeed = 8;

            int minRadius = 1;
            int maxRadius = 5;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(location, true, false, location, map, projectiles, 20, minRadius, maxRadius, false);

            if (m_ValidLocations.Count == 0)
                return;

            for (int b = 0; b < projectiles; b++)
            {
                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 2), map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), map);

                newLocation.Z += 5;

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(4651, 4652, 4653, 4654, 5701), particleSpeed, 0, false, false, 0, 0);
            }

            for (int a = 0; a < 4; a++)
            {
                Point3D newPoint = new Point3D(location.X + Utility.RandomList(-2, -1, 1, 2), location.Y + Utility.RandomList(-2, -1, 1, 2), location.Z);
                SpellHelper.AdjustField(ref newPoint, map, 12, false);

                new Blood().MoveToWorld(newPoint, map);
            }

            int radius = 2;
            int explosionSound = Utility.RandomList(0x5DA, 0x580);

            if (largeExplosion)
            {
                explosionSound = 0x309;
                radius = 4;

                List<int> m_ExtraParts = new List<int>();

                m_ExtraParts.Add(Utility.RandomList(7407)); //Entrail
                m_ExtraParts.Add(Utility.RandomList(6929)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6930)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6937)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6938)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6931)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6932)); //Bones

                m_ExtraParts.Add(Utility.RandomList(4650)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4651)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4652)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4653)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4654)); //Blood
                m_ExtraParts.Add(Utility.RandomList(5701)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4655)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7439)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7438)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7436)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7433)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7431)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7428)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7425)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7410)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7415)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7416)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7418)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7420)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7425)); //Blood

                double extraPartChance = .5;   

                int minRange = radius * -1;
                int maxRange = radius;

                List<Point3D> m_ExplosionPoints = new List<Point3D>();

                for (int a = minRange; a < maxRange + 1; a++)
                {
                    for (int b = minRange; b < maxRange + 1; b++)
                    {
                        Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                        SpellHelper.AdjustField(ref newPoint, map, 12, false);

                        if (map.InLOS(location, newPoint))
                            m_ExplosionPoints.Add(newPoint);
                    }
                }

                for (int a = 0; a < m_ExplosionPoints.Count; a++)
                {
                    if (Utility.RandomDouble() <= extraPartChance)
                    {
                        Point3D explosionPoint = m_ExplosionPoints[a];

                        int itemId = m_ExtraParts[Utility.RandomMinMax(0, m_ExtraParts.Count - 1)];

                        int distance = Utility.GetDistance(location, explosionPoint);

                        Timer.DelayCall(TimeSpan.FromSeconds(distance * .15), delegate
                        {
                            TimedStatic gore = new TimedStatic(itemId, 10);
                            gore.Name = "gore";
                            gore.MoveToWorld(explosionPoint, map);

                            if (Utility.RandomDouble() <= .5)
                            {
                                Item item = null;

                                switch (Utility.RandomMinMax(1, 8))
                                {
                                    //case 1: item = new CookedBird(); break;
                                    //case 2: item = new ChickenLeg(); break;
                                    //case 3: item = new LambLeg(); item.Name = "chicken drumstick"; break;
                                    case 4: item = new Feather(); break;
                                    case 5: item = new Feather(); break;
                                    case 6: item = new Feather(); break;
                                    case 7: item = new Feather(); break;
                                    case 8: item = new Feather(); break;
                                }

                                if (item != null)
                                    item.MoveToWorld(explosionPoint, map);
                            }
                        });
                    }
                }
            }

            Effects.PlaySound(location, map, explosionSound);
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

                    SummonTurkeys();
                }
            }

            base.OnDamage(amount, from, willKill);
        }       

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextEnrageAllowed)
                        {
                            Enrage();
                            return;
                        }
                    break;

                    
                    case 2:
                        if (DateTime.UtcNow >= m_NextHeartyWingBuffetAllowed)
                        {
                            HeartyWingBuffet();
                            return;
                        }
                    break;
                        
                    case 3:
                        if (DateTime.UtcNow >= m_NextRoastAllowed)
                        {
                            Roast();
                            return;
                        }
                    break;                    
                }
            }
        }

        public void SummonTurkeys()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = 1;
            double totalDelay = 1 + directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");
            
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
            });

            Effects.PlaySound(Location, Map, 0x4d7);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*establishes new pecking order*");

            Point3D location = Location;
            Map map = Map;

            PlaySound(GetAngerSound());
            Animate(28, 10, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                PlaySound(0x468);

                int projectiles = 50;
                double projectileDelay = .02;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * projectileDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                        SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), Map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3578, 3579), particleSpeed, 0, false, false, 0, 0);
                    });
                }

                Timer.DelayCall(TimeSpan.FromSeconds(projectiles * projectileDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    spawnPercent = (double)intervalCount / (double)totalIntervals;

                    int turkeys = 5 + (int)(Math.Ceiling(7 * spawnPercent));

                    for (int a = 0; a < turkeys; a++)
                    {
                        Point3D creatureLocation = Location;
                        Map creatureMap = Map;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                        if (m_ValidLocations.Count > 0)
                            creatureLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        LesserGobbler creature = new LesserGobbler();
                        creature.MoveToWorld(creatureLocation, creatureMap);
                        creature.PlaySound(creature.GetAngerSound());

                        Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                        m_Creatures.Add(creature);
                    }
                });
            });
        }

        public void Enrage()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double directionDelay = .25;
            double initialDelay = 1;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextEnrageAllowed = DateTime.UtcNow + NextEnrageDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
            });

            Effects.PlaySound(Location, Map, 0x4d7);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*enters a fowl mood*");

            Point3D location = Location;
            Map map = Map;

            PlaySound(GetAngerSound());
            Animate(16, 8, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                PlaySound(0x46A);

                FeatherExplosion(Location, Map, 20);

                SpecialAbilities.FrenzySpecialAbility(1.0, this, null, 1.0, EnrageDuration.TotalSeconds, 0, false, "", "", "");
                SpecialAbilities.EnrageSpecialAbility(1.0, null, this, .5, EnrageDuration.TotalSeconds, 0, false, "", "", "");

                Hue = EnragedHue;

                ActiveSpeed = .25;
                PassiveSpeed = .25;
                CurrentSpeed = .25;

                Timer.DelayCall(EnrageDuration, delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Hue = NormalHue;

                    ActiveSpeed = .35;
                    PassiveSpeed = .35;
                    CurrentSpeed = .35;
                });
            });
        }

        public void HeartyWingBuffet()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int wings = 50 + (int)(Math.Ceiling(150 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)wings / 10));
            double totalDelay = loops + 2.5;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextHeartyWingBuffetAllowed = DateTime.UtcNow + NextHeartyWingBuffetDelay;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
            });

            Effects.PlaySound(Location, Map, 0x4d7);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*prepares hearty wing buffet*");

            Point3D location = Location;
            Map map = Map;

            PlaySound(GetAngerSound());
            Animate(16, 8, 1, true, false, 0);

            int radius = 10;

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

                        Animate(16, 8, 1, true, false, 0);

                        PlaySound(GetAngerSound());
                        Effects.PlaySound(location, map, 0x5CF);
                    });
                }

                for (int a = 0; a < wings; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        Point3D wingLocation = new Point3D(location.X + Utility.RandomMinMax(radius * -1, radius), location.Y + Utility.RandomMinMax(radius * -1, radius), location.Z);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(wingLocation.X - 1, wingLocation.Y - 1, wingLocation.Z + 100), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(wingLocation.X, wingLocation.Y, wingLocation.Z + 5), map);

                        int particleSpeed = 8 + (int)(Math.Round(8 * (double)spawnPercent));

                        int itemId = Utility.RandomList(5641, 2489, 5639);

                        Effects.SendMovingParticles(startLocation, endLocation, itemId, particleSpeed, 0, false, false, 0, 0, 9501, 0, 0, 0x100);

                        double impactDelay = .75 - (.375 * spawnPercent);

                        Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                        {
                            if (!SpecialAbilities.Exists(this))
                                return;

                            Effects.PlaySound(endLocation, map, Utility.RandomList(0x357, 0x359));
                            Effects.SendLocationParticles(endLocation, 0x3709, 10, 20, 0, 0, 5029, 0);

                            Item item = null;

                            switch (itemId)
                            {
                                //case 5641: item = new LambLeg(); item.Name = "chicken drumstick"; break;
                                //case 2489: item = new CookedBird();  break;
                                //case 5639: item = new ChickenLeg(); break;
                            }

                            Point3D foodLocation = new Point3D(wingLocation.X, wingLocation.Y, wingLocation.Z);

                            item.MoveToWorld(foodLocation, map);

                            double firefieldChance = .15 + (.15 * spawnPercent);

                            if (Utility.RandomDouble() <= firefieldChance)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                                {
                                    SingleFireField singleFireField = new SingleFireField(null, 0, 1, 30, 3, 5, false, false, true, -1, true);
                                    singleFireField.MoveToWorld(foodLocation, map);
                                });
                            }

                            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(wingLocation, 1);

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

                                double damage = DamageMax;

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                            }
                        });
                    });
                }
            });
        }

        public void Roast()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            int fireBarrageRange = 18;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, fireBarrageRange);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;

                mobileCount++;
            }

            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, fireBarrageRange);

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

            m_NextRoastAllowed = DateTime.UtcNow + NextRoastDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*begins a roast*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(16, 8, 1, true, false, 0);
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
                                if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= fireBarrageRange)
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

        protected override bool OnMove(Direction d)
        {
            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
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
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 8; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return 26; } }
        public override int IdleFrames { get { return 8; } }
        
        //28 10 1 true false 0 //Wing Fan
        //16 8 1 true false 0 //Taunt

        public override int GetAngerSound() { return 0x4D7; }
        public override int GetIdleSound() { return 0x4DA; }
        public override int GetAttackSound() { return 0x46A; }
        public override int GetHurtSound() { return 0x46C; }
        public override int GetDeathSound() { return 0x4D6; }        

        public GreatGobbler(Serial serial): base(serial)
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
