using System;
using Server.Items;
using Server.Custom;
using Server.Network;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class LoHChaosBeastEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHChaosBeast); } }
        public override string DisplayName { get { return "Chaos Beast"; } }

        public override string AnnouncementText { get { return "A Chaos Beast has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2619; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a chaost beast corpse")]
    public class LoHChaosBeast : LoHMonster
    {
        public DateTime m_NextWeirdBarrageAllowed;
        public TimeSpan NextWeirdBarrageDelay = TimeSpan.FromSeconds(20);

        [Constructable]
        public LoHChaosBeast(): base()
        {
            Name = "Chaos Beast";

            Body = 725;
            Hue = 2619;

            BaseSoundID = 461;
            
            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 300);

            SetSkill(SkillName.MagicResist, 200);  

            VirtualArmor = 75;

            Fame = 10000;
            Karma = -10000;            
        }        
        
        public override void ConfigureCreature()
        {
            base.ConfigureCreature();
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            SetSubGroup(AISubgroup.MeleeMage4);
            UpdateAI(false);
        }

        public void WeirdBarrage()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            int range = (int)(Math.Round(8 + (8 * SpawnPercent)));

            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, range);

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
            nearbyMobiles = Map.GetMobilesInRange(Location, range);

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

            int maxExtraFireballs = 20;
            int fireballs = 20 + (int)Math.Ceiling(((double)maxExtraFireballs * SpawnPercent));

            double directionDelay = .25;
            double initialDelay = 1;
            double fireballDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)fireballs * fireballDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

            m_NextWeirdBarrageAllowed = DateTime.UtcNow + NextWeirdBarrageDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            PublicOverheadMessage(MessageType.Regular, 0, false, "*starts to get weird*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(15, 10, 1, true, false, 0);
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

                            bool mobileTargetValid = true;

                            if (mobileTarget == null)
                                mobileTargetValid = false;

                            else if (mobileTarget.Deleted || !mobileTarget.Alive)
                                mobileTargetValid = false;

                            else
                            {
                                if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= range)
                                    mobileTargetValid = false;
                            }

                            if (mobileTargetValid)
                            {
                                targetLocation = mobileTarget.Location;
                                targetMap = mobileTarget.Map;
                            }

                            int effectSound = 0x56D; //0x357
                            int itemID = Utility.RandomList(572, 731, 2330, 2429, 2445, 2451, 2459, 2481, 2487, 2491, 2505, 2508, 2537, 2586,
                                2598, 2648, 2886, 3179, 3164, 3570, 3612, 3629, 3616, 3740, 3762, 3820, 3906, 4006, 4650, 4646, 4963, 5037, 5368);
                            int itemHue = 2619;

                            int impactSound = 0x226;
                            int impactHue = 2619;

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
                            Effects.SendMovingEffect(startLocation, endLocation, itemID, 8, 0, false, false, itemHue - 1, 0);

                            double targetDistance = Utility.GetDistanceToSqrt(location, adjustedLocation);
                            double destinationDelay = (double)targetDistance * .06;

                            Direction newDirection = Utility.GetDirection(location, adjustedLocation);

                            if (Direction != newDirection)
                                Direction = newDirection;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                Effects.PlaySound(adjustedLocation, targetMap, impactSound);
                                Effects.SendLocationParticles(EffectItem.Create(adjustedLocation, targetMap, EffectItem.DefaultDuration), 0x3709, 20, 20, impactHue - 1, 0, 0, 0);

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

                                    int damage = (int)(Math.Round((double)DamageMin / 6));

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

        public override void OnThink()
        {
 	         base.OnThink();

             if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress)
             {
                 if (DateTime.UtcNow >= m_NextWeirdBarrageAllowed)
                 {
                     WeirdBarrage();
                     return;
                 }
             }
        }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }

        public override int GetAngerSound() { return 0x452; }
        public override int GetAttackSound() { return 0x453; }
        public override int GetHurtSound() { return 0x454; }
        public override int GetDeathSound() { return 0x455; }
        public override int GetIdleSound() { return 0x451; }

        public LoHChaosBeast(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
