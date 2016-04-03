using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class UOACZFirstRanger : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get{ return new string[0];} }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override int DifficultyValue { get { return 10; } }

        public DateTime m_NextFlurryAllowed;
        public TimeSpan NextFlurryDelay = TimeSpan.FromSeconds(60);

        public DateTime m_NextArrowStormAllowed;
        public TimeSpan NextArrowStormDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(5);

        public int damageIntervalThreshold = 2000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;
        
        [Constructable]
		public UOACZFirstRanger() : base()
		{
            Title = "the first ranger";

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(10000);
            SetStam(5000);

            AttackSpeed = 40;

            SetDamage(15, 25);

            SetSkill(SkillName.Archery, 90);

            SetSkill(SkillName.Tactics, 100);
            
            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

            Fame = 2000;
            Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new StuddedGorget() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedChest() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedLegs() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedArms() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = PrimaryHue });            
            AddItem(new Kilt() { Movable = false, Hue = PrimaryHue });
            AddItem(new Boots() { Movable = false, Hue = PrimaryHue });

            AddItem(new BodySash() { Movable = false, Hue = 2208 });
            AddItem(new Bandana() { Movable = false, Hue = 2208 });
            AddItem(new Cloak() { Movable = false, Hue = 2208 });

            AddItem(new Yumi() { Movable = false, Hue = SecondaryHue, Speed = 40, MaxRange = 16, Name = "a longbow" });
		}

        public override int MaxDistanceAllowedFromHome { get { return 75; } }
        public override bool AlwaysChamp { get { return true; } }

        public override bool IsRangedPrimary { get { return true; } }
        public override int WeaponSwitchRange { get { return 2; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ActiveSpeed = .3;
            CurrentSpeed = .3;
            PassiveSpeed = .3;

            RangeHome = 20;              

            RangePerception = 16;
            DefaultPerceptionRange = 16;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .1 && DateTime.UtcNow >= m_NextFlurryAllowed)
            {
                SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 1.0, 10, -1, true, "", "", "*begins to enter a trance*");

                m_NextFlurryAllowed = DateTime.UtcNow + NextFlurryDelay;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            if (willKill)
                UOACZEvents.HumanChampionDamaged(true);

            else
                UOACZEvents.HumanChampionDamaged(false);
        }
        
        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(Human Champion)", from.NetState);

            base.OnSingleClick(from);
        }

        public void ArrowStorm()
        {
            if (!UOACZSystem.IsUOACZValidMobile(this))
                return;

            int arrowStormRange = 20;

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, arrowStormRange);

            int mobileCount = 0;

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;
                if (mobile is UOACZBaseHuman) continue;
                if (mobile is UOACZBaseWildlife) continue;

                if (mobile is PlayerMobile)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile.IsUOACZHuman && Combatant != pm_Mobile)
                        continue;
                }

                mobileCount++;
            }

            nearbyMobiles.Free();
            nearbyMobiles = Map.GetMobilesInRange(Location, arrowStormRange);

            List<Mobile> m_NearbyMobiles = new List<Mobile>();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == this) continue;
                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                if (!Map.InLOS(Location, mobile.Location)) continue;
                if (mobile.Hidden) continue;
                if (mobile is UOACZBaseHuman) continue;
                if (mobile is UOACZBaseWildlife) continue;

                if (mobile is PlayerMobile)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile.IsUOACZHuman && Combatant != pm_Mobile)
                        continue;                    
                }

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

            int maxExtraArrows = 6;
            int arrows = 6 + (int)Math.Ceiling(((double)maxExtraArrows * spawnPercent));

            double directionDelay = .25;
            double initialDelay = 1;
            double arrowDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)arrows * arrowDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextArrowStormAllowed = DateTime.UtcNow + NextArrowStormDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(this)) return;
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes a storm of arrows*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(this)) return;

                PlaySound(0x522);

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(this)) return;

                    for (int a = 0; a < arrows; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * arrowDelay), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(this))
                                return;                            

                            bool mobileTargetValid = true;

                            if (mobileTarget == null)
                                mobileTargetValid = false;

                            else if (mobileTarget.Deleted || !mobileTarget.Alive)
                                mobileTargetValid = false;

                            else
                            {
                                if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= arrowStormRange)
                                    mobileTargetValid = false;
                            }

                            if (mobileTargetValid)
                            {
                                targetLocation = mobileTarget.Location;
                                targetMap = mobileTarget.Map;
                            }

                            int effectSound = 0x238;
                            int itemID = 3906; //7166
                            int itemHue = 0;

                            int impactSound = 0x234;
                            int impactHue = 0;

                            int xOffset = 0;
                            int yOffset = 0;

                            int distance = Utility.GetDistance(Location, targetLocation);

                            if (distance > 1)
                            {
                                if (Utility.RandomDouble() <= .25)
                                    xOffset = Utility.RandomList(-1, 1);

                                if (Utility.RandomDouble() <= .25)
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
                                if (!UOACZSystem.IsUOACZValidMobile(this))
                                    return;

                                PlaySound(0x238);
                                
                                Queue m_Queue = new Queue();

                                nearbyMobiles = targetMap.GetMobilesInRange(adjustedLocation, 0);

                                foreach (Mobile mobile in nearbyMobiles)
                                {
                                    if (mobile == this) continue;
                                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                    if (mobile is UOACZBaseHuman) continue;
                                    if (mobile is UOACZBaseWildlife) continue;

                                    if (mobile is PlayerMobile)
                                    {
                                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                                        if (pm_Mobile.IsUOACZHuman && Combatant != pm_Mobile)
                                            continue;
                                    }
                                   
                                    m_Queue.Enqueue(mobile);
                                }

                                nearbyMobiles.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    int damage = DamageMin;

                                    if (mobile is BaseCreature)
                                        damage *= 2;

                                    DoHarmful(mobile);

                                    mobile.PlaySound(0x234);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, this, damage, 100, 0, 0, 0, 0);
                                }
                            });
                        });
                    }
                });
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Point3D location = Location;
            Map map = Map;

            //Outside of Valid Combat Zone
            if (Utility.GetDistance(Home, location) > MaxDistanceAllowedFromHome)
            {
                TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                dirt.Name = "dirt";
                dirt.MoveToWorld(location, map);

                dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*returns back to it's hunting ground*");

                Effects.PlaySound(location, map, 0x657);

                int projectiles = 6;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Z);
                    SpellHelper.AdjustField(ref newLocation, map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
                }

                Location = Home;
                Combatant = null;

                return;
            }

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {
                switch (Utility.RandomMinMax(1, 1))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextArrowStormAllowed)
                        {
                            ArrowStorm();
                            return;
                        }
                    break;
                }
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZFirstRanger(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

            //Version 1
            writer.Write(damageProgress);
            writer.Write(intervalCount);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
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
