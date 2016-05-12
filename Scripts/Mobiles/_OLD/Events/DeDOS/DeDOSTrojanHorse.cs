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
	[CorpseName( "trojan horse's corpse" )]
	public class DeDOSTrojanHorse : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextChargeAllowed;
        public TimeSpan NextChargeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_ChargeTimeout;
        public TimeSpan MaxChargeDuration = TimeSpan.FromSeconds(10);

        public bool m_ChargeInProgress = false;

        public List<Mobile> m_Trampled = new List<Mobile>();

		[Constructable]
		public DeDOSTrojanHorse() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "trojan horse";

            BodyValue = 117;
            Hue = 2075;

			SetStr(100);
			SetDex(50);
			SetInt(25);

			SetHits(1000);
            SetStam(1000);
            SetMana(0);

			SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);
            
			SetSkill(SkillName.MagicResist, 100);

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 75;
		}

        public override bool MovementRestrictionImmune { get { return true; } }
        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
            
            ActiveSpeed = 0.3;
            PassiveSpeed = 0.3;
            CurrentSpeed = 0.3;

            UniqueCreatureDifficultyScalar = 2;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }   

        public override void OnThink()
        {
            base.OnThink();

            CheckChargeResolved();            

            if (Combatant != null && !Frozen)
            {
                if (DateTime.UtcNow >= m_NextChargeAllowed)
                {
                    StartCharge();
                    return;
                }
            }
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
            
            LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(NextChargeDelay.TotalSeconds);            

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;            
            m_ChargeTimeout = DateTime.UtcNow + MaxChargeDuration;

            m_ChargeInProgress = true;
            
            DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 500;     
            
            Effects.PlaySound(Location, Map, 0x59B);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*charges*");

            m_Trampled.Clear();           
            
            ActiveSpeed = .25;
            PassiveSpeed = .25;
            CurrentSpeed = .25;
           
            Paralyzed = false;
            CantWalk = false;
            Frozen = false;            
        }

        public void CheckChargeResolved()
        {
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

                    SpecialAbilities.HinderSpecialAbility(1.0, this, Combatant, 1.0, 1, false, -1, false, "", "You have been trampled and can't move!", "-1");

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

            m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;

            m_Trampled.Clear();

            LastSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        }

        protected override bool OnMove(Direction d)
        {
            if (m_ChargeInProgress)
            {
                if (Utility.RandomDouble() <= .15)
                {
                    Effects.PlaySound(Location, Map, 0x2F4);

                    int effectHue = 0;

                    DeDOSElectricField electricField = new DeDOSElectricField(this, effectHue, 1, 20, 3, 5, false, false, true, -1, true);
                    electricField.MoveToWorld(Location, Map);
                }

                m_NextChargeAllowed = DateTime.UtcNow + NextChargeDelay;             
                
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
                        damage *= 2;

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);                   

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

                    SpecialAbilities.HinderSpecialAbility(1.0, this, mobile, 1.0, 1, false, -1, false, "", "You have been trampled and can't move!", "-1");
                }                

                CheckChargeResolved();
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x12E, 0x12D));

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);  
        }

        public override int GetAngerSound() { return 0x4BE; }
        public override int GetIdleSound() { return 0x4BD; }
        public override int GetAttackSound() { return 0x4F6; }
        public override int GetHurtSound() { return 0x4BF; }
        public override int GetDeathSound() { return 0x4C0; }

        public DeDOSTrojanHorse(Serial serial): base(serial)
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
