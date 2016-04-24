using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a blood courser corpse")]
	public class BloodCourser : BaseCreature
	{
        public DateTime m_NextChargeAllowed;
        public TimeSpan NextChargeDelay = TimeSpan.FromSeconds(10);

		[Constructable]
		public BloodCourser () : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            Name = "a blood courser";

            Body = 226;
            Hue = 2118;

            BaseSoundID = 0xA8;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(2500);
            SetStam(2500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;
        }

        public override bool MovementRestrictionImmune { get { return true; } }
        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .2;

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, 1.0, 15, -1, true, "", "", "*becomes frenzied*");
        }
        
        public override void SetUniqueAI()
        {
        }

        public void Charge()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;

            int range = 18;

            Dictionary<Mobile, int> DictPossibleNewCombatants = new Dictionary<Mobile, int>();

            IPooledEnumerable m_NearbyMobiles = Map.GetMobilesInRange(Location, range);

            foreach (Mobile mobile in m_NearbyMobiles)
            {
                if (mobile == this) continue;
                if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                if (!mobile.InLOS(this)) continue;
                if (mobile.Hidden) continue;

                int distance = Utility.GetDistance(mobile.Location, Location);

                if (mobile == Combatant && distance <= 4)
                    continue;

                DictPossibleNewCombatants.Add(mobile, distance);
            }            

            m_NearbyMobiles.Free();

            if (DictPossibleNewCombatants.Count == 0)
                return;

            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                TotalValues += pair.Value;
            }

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundNewCombatant = false;

            //Determine Combatant                      
            foreach (KeyValuePair<Mobile, int> pair in DictPossibleNewCombatants)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    Combatant = pair.Key;
                    foundNewCombatant = true;

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            if (Combatant != null && foundNewCombatant)
            {
                double directionDelay = .25;
                double initialDelay = .5;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

                Point3D location = Location;
                Map map = Map;

                Point3D targetLocation = Combatant.Location;
                Map targetMap = Combatant.Map;

                Direction = Utility.GetDirection(Location, targetLocation);

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    PublicOverheadMessage(MessageType.Regular, 0, false, "*charges*");

                    Animate(5, 5, 1, true, false, 0);
                    PlaySound(GetAngerSound());                    

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this))
                            return;

                        bool validCombatant = true;

                        if (Combatant == null)
                            validCombatant = false;

                        else
                        {
                            if (!SpecialAbilities.MonsterCanDamage(this, Combatant)) validCombatant = false;
                            if (Combatant.Hidden) validCombatant = false;
                            if (!Map.InLOS(Location, Combatant.Location)) validCombatant = false;
                            if (Utility.GetDistance(Location, Combatant.Location) > range + 10) validCombatant = false;
                        }

                        if (validCombatant)
                            targetLocation = Combatant.Location;

                        Point3D effectStep = Location;

                        int distance = Utility.GetDistance(effectStep, targetLocation);
                        double stepDelay = .04;
                                                
                        Queue m_Queue;

                        IPooledEnumerable nearbyMobiles;
                        
                        for (int a = 0; a < distance; a++)
                        {                            
                            if (!SpecialAbilities.Exists(this))
                                return;

                            Direction direction = Utility.GetDirection(effectStep, targetLocation);
                            effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                            Point3D effectLocation = effectStep;
                            int index = a;

                            Timer.DelayCall(TimeSpan.FromSeconds(index * stepDelay), delegate
                            {
                                Effects.SendLocationParticles(EffectItem.Create(effectLocation, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2117, 0, 2023, 0);
                                Effects.PlaySound(effectLocation, map, 0x5C6);

                                Point3D adjustedLocation = effectLocation;
                                adjustedLocation.Z++;

                                new Blood().MoveToWorld(adjustedLocation, map);
                            });

                            m_Queue = new Queue();

                            nearbyMobiles = map.GetMobilesInRange(effectStep, 0);

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

                                double damage = (double)DamageMin;

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                int finalDamage = (int)(Math.Ceiling(damage));

                                DoHarmful(mobile);

                                mobile.PlaySound(mobile.GetHurtSound());

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                            }
                        }

                        Location = targetLocation;

                        m_Queue = new Queue();
                        nearbyMobiles = map.GetMobilesInRange(targetLocation, 0);

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

                            double damage = (double)DamageMin;

                            if (mobile is BaseCreature)
                                damage *= 2;

                            int finalDamage = (int)(Math.Ceiling(damage));
                            double knockbackDamage = 25;

                            DoHarmful(mobile);

                            mobile.PlaySound(mobile.GetHurtSound());

                            SpecialAbilities.KnockbackSpecialAbility(1.0, location, this, mobile, knockbackDamage, 2, -1, "", "You are knocked back by their charge!");

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                        }                        
                    });
                });
            }   
        }

        public override void OnThink()
        {
            if (Combatant != null && DateTime.UtcNow >= m_NextChargeAllowed)
                Charge();

            base.OnThink();
        }

        protected override bool OnMove(Direction d)
        {
            PlaySound(Utility.RandomList(0x129, 0x12A));

            new Blood().MoveToWorld(Location, Map);      

            return base.OnMove(d);
        }

		public BloodCourser( Serial serial ) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
            base.Serialize(writer);
            writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
            base.Deserialize(reader);
			int version = reader.ReadInt();
        }
	}
} 
