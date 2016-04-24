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
	[CorpseName( "bloody reaver's corpse" )]
	public class BloodyReaver : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextChargeAllowed;

        public DateTime m_ChargeTimeout;
        public TimeSpan ChargeTimeoutDelay = TimeSpan.FromSeconds(10);

        public bool m_IsCharging = false;
        public Mobile m_ChargeTarget;

        public DateTime m_NextBloodlustAllowed;

        public DateTime m_NextBloodExplosionAllowed;

        public DateTime m_NextAbilityAllowed;

        public int damageIntervalThreshold = 1000;
        public int totalIntervals = 20;

        public int damageProgress = 0;
        public int intervalCount = 0;

        public List<Mobile> m_Trampled = new List<Mobile>();

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public BloodyReaver() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Bloody Reaver";

            Body = 400;
            Hue = 2117;

			SetStr(100);
			SetDex(100);
			SetInt(25);

			SetHits(20000);
            SetStam(20000);
            SetMana(0);

			SetDamage(20, 40);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

			SetSkill(SkillName.MagicResist, 50);

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 125;

            SkeletalMount mount = new SkeletalMount();
            mount.Hue = 1175;
            mount.Rider = this;

            HairItemID = 8252;
            HairHue = 2118;

            AddItem(new DragonHelm() { Movable = false, Hue = 2117, Name = "Bloody Helm"});
            AddItem(new PlateGorget() { Movable = false, Hue = 2117 });
            AddItem(new PlateChest() { Movable = false, Hue = 2117 });
            AddItem(new PlateArms() { Movable = false, Hue = 2117 });
            AddItem(new PlateLegs() { Movable = false, Hue = 2117 });
            AddItem(new PlateGloves() { Movable = false, Hue = 2117 });
            AddItem(new Cloak() { Movable = false, Hue = 2117 });

            AddItem(new Lance() { Movable = false, Hue = 2117 });
            AddItem(new DupresShield() { Movable = false, Hue = 2117, Name = "Bloody Barricade" });
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.33;
            
            ActiveSpeed = 0.4;
            PassiveSpeed = 0.4;
            CurrentSpeed = 0.4;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
        }

        public override bool MovementRestrictionImmune { get { return true; } }
        public override bool AlwaysBoss { get { return true; } }        
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    intervalCount = (int)(Math.Ceiling((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

                    if (intervalCount < 0)
                        intervalCount = 0;

                    if (intervalCount > totalIntervals)
                        intervalCount = totalIntervals;
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double bloodlustChance = .20 + (.20 * spawnPercent);
            double bleedChance = .20 + +(.20 * spawnPercent);

            if (Utility.RandomDouble() <= bloodlustChance && DateTime.UtcNow > m_NextBloodlustAllowed && DateTime.UtcNow > m_NextAbilityAllowed)
            {
                m_NextBloodlustAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (20 * spawnPercent));
                m_NextAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(10 - (8 * spawnPercent));

                Effects.PlaySound(Combatant.Location, Combatant.Map, 0x28a);
                
                SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 0.5, 30, -1, true, "", "", "*becomes consumed with bloodlust*");

                Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2117, 0, 5029, 0);

                int bloodItem = 5 + (int)(Math.Ceiling(10 * spawnPercent));

                for (int a = 0; a < bloodItem; a++)
                {
                    new Blood().MoveToWorld(new Point3D(defender.Location.X + Utility.RandomMinMax(-3, 3), defender.Location.Y + Utility.RandomMinMax(-3, 3), defender.Location.Z), Map);
                }
            }

            else
                SpecialAbilities.BleedSpecialAbility(bleedChance, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!", "-1");            
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            
            if (Combatant != null && !m_IsCharging && Utility.RandomDouble() <= .20 && DateTime.UtcNow > m_NextBloodExplosionAllowed && DateTime.UtcNow > m_NextAbilityAllowed)
            {
                List<Point3D> m_NearbyBloodLocations = new List<Point3D>();
                Queue m_Queue = new Queue();

                IPooledEnumerable bloodItems = Map.GetItemsInRange(Location, 10);

                foreach (Item item in bloodItems)
                {
                    Blood blood = item as Blood;

                    if (blood != null)
                    {
                        if (blood.Hue == 0 && !m_NearbyBloodLocations.Contains(blood.Location))
                            m_NearbyBloodLocations.Add(blood.Location);
                    }
                }

                bloodItems.Free();

                if (m_NearbyBloodLocations.Count > 0)
                {
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*commands the blood*");

                    m_NextBloodExplosionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (20 * spawnPercent));
                    m_NextAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(10 - (8 * spawnPercent));

                    foreach (Point3D bloodLocation in m_NearbyBloodLocations)
                    {
                        Effects.PlaySound(bloodLocation, Map, 0x56D);
                        Effects.SendLocationParticles(EffectItem.Create(bloodLocation, Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2117, 0, 5029, 0);

                        IPooledEnumerable mobilesOnTile = Map.GetMobilesInRange(bloodLocation, 1);

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

                            double damage = Utility.RandomMinMax(5, 10);

                            if (mobile is BaseCreature)
                                damage *= 1.5;

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                        }
                    }
                }

                return;
            }

            CheckChargeResolved();

            if (Combatant != null && DateTime.UtcNow > m_NextAIChangeAllowed && !m_IsCharging && DateTime.UtcNow > m_NextChargeAllowed && DateTime.UtcNow > m_NextAbilityAllowed && !Paralyzed && !CantWalk && !Frozen)
            {
                Dictionary<Mobile, int> DictPossibleNewCombatants = new Dictionary<Mobile, int>();

                IPooledEnumerable m_NearbyMobiles = Map.GetMobilesInRange(Location, 18);
                
                foreach (Mobile mobile in m_NearbyMobiles)
                {
                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.Hidden || !mobile.InLOS(this) || mobile.AccessLevel > AccessLevel.Player)
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

                    if (!mobile.Alive)
                        validTarget = false;

                    if (mobile.Hidden)
                        validTarget = false;

                    int distance = Utility.GetDistance(mobile.Location, Location);

                    if (distance <= 1)
                        validTarget = false;

                    if (validTarget && mobile != Combatant)                    
                        DictPossibleNewCombatants.Add(mobile, distance);                    
                }

                m_NearbyMobiles.Free();
                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;

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
                        DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 50;
                        foundNewCombatant = true;

                        break;
                    }

                    CumulativeAmount += AdditionalAmount;
                }

                if (Combatant != null && foundNewCombatant)
                {
                    Paralyzed = false;
                    CantWalk = false;
                    Frozen = false;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    m_IsCharging = true;
                    m_ChargeTarget = Combatant;

                    ActiveSpeed = 0.3 - (.1 * spawnPercent);
                    PassiveSpeed = 0.3 - (.1 * spawnPercent);
                    CurrentSpeed = 0.3 - (.1 * spawnPercent);

                    Effects.PlaySound(Location, Map, 0x59B);
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*charges*");

                    m_Trampled.Clear();

                    m_NextChargeAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (20 * spawnPercent));
                    m_NextAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(10 - (8 * spawnPercent));
                    m_ChargeTimeout = DateTime.UtcNow + ChargeTimeoutDelay;                    
                }

                return;
            }            
        }

        public void CheckChargeResolved()
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            
            if (m_IsCharging)
            {
                bool chargeComplete = false;
                bool chargeFail = false;

                if (Combatant == null || m_ChargeTarget == null)
                    chargeFail = true;

                else if (Combatant != m_ChargeTarget || !Combatant.Alive || Combatant.Hidden || GetDistanceToSqrt(Combatant.Location) > 24 || !InLOS(Combatant) || DateTime.UtcNow > m_ChargeTimeout)
                    chargeFail = true;

                if (chargeFail)
                {
                    m_IsCharging = false;
                    m_ChargeTarget = null;

                    ActiveSpeed = 0.4;
                    PassiveSpeed = 0.4;
                    CurrentSpeed = 0.4;
                }

                else
                {
                    if (GetDistanceToSqrt(Combatant) <= 1.5)
                    {
                        m_IsCharging = false;
                        m_ChargeTarget = null;

                        ActiveSpeed = 0.4;
                        PassiveSpeed = 0.4;
                        CurrentSpeed = 0.4;

                        m_NextChargeAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(30 - (20 * spawnPercent));
                        m_NextAbilityAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(10 - (8 * spawnPercent));

                        PublicOverheadMessage(MessageType.Regular, 0, false, "*tramples opponent*");

                        DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 4;

                        Effects.PlaySound(Location, Map, 0x59C);
                        Effects.PlaySound(Combatant.Location, Combatant.Map, Combatant.GetHurtSound());

                        double damage = DamageMax;

                        if (Combatant is BaseCreature)
                            damage *= 1.5;

                        new Blood().MoveToWorld(Combatant.Location, Combatant.Map);

                        int bloodCount = 2 + (int)(Math.Ceiling(6 * spawnPercent));

                        for (int a = 0; a < bloodCount; a++)
                        {
                            new Blood().MoveToWorld(new Point3D(Combatant.Location.X + Utility.RandomList(-2, 2), Combatant.Location.Y + Utility.RandomList(-2, 2), Combatant.Location.Z), Map);
                        }

                        new Blood().MoveToWorld(Combatant.Location, Combatant.Map);
                        SpecialAbilities.BleedSpecialAbility(1.0, this, Combatant, damage, 10.0, -1, true, "", "The reaver's lance impales you, causing you to bleed!", "-1");
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

                        SpecialAbilities.HinderSpecialAbility(1.0, this, Combatant, 1.0, 3, false, -1, false, "", "You have been trampled and can't move!", "-1");
                    }
                }
            }

            else
            {
                DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 4;

                m_ChargeTarget = null;

                ActiveSpeed = 0.4;
                PassiveSpeed = 0.4;
                CurrentSpeed = 0.4;
            }
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
        }

        public override int GetAngerSound() { return 0x2A9; }
        public override int GetIdleSound() { return 0x598; }
        //public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x5FA; }
        public override int GetDeathSound() { return 0x2AB; }        

        protected override bool OnMove(Direction d)
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Effects.PlaySound(Location, Map, Utility.RandomList(0x12E, 0x12D ));

            if (m_IsCharging)
            {
                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 1);

                Queue m_Queue = new Queue();

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player || m_Trampled.Contains(mobile))
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

                    if (Combatant != null && Combatant == mobile)
                        validTarget = false;

                    if (validTarget)
                    {
                        if (!m_Trampled.Contains(mobile))
                            m_Trampled.Add(mobile);

                        m_Queue.Enqueue(mobile);
                    }
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    Effects.PlaySound(Location, Map, Utility.RandomList(0x3BB, 0x3BA, 0x3B9));
                    Effects.PlaySound(mobile.Location, mobile.Map, mobile.GetHurtSound());

                    double damage = DamageMin;

                    if (Combatant is BaseCreature)
                        damage *= 1.5;

                    new Blood().MoveToWorld(Combatant.Location, Combatant.Map);

                    int bloodCount = 1 + (int)(Math.Ceiling(3 * spawnPercent));

                    for (int a = 0; a < bloodCount; a++)
                    {
                        new Blood().MoveToWorld(new Point3D(Combatant.Location.X + Utility.RandomList(-2, 2), Combatant.Location.Y + Utility.RandomList(-2, 2), Combatant.Location.Z), Map);
                    }

                    new Blood().MoveToWorld(Combatant.Location, Combatant.Map);
                    SpecialAbilities.BleedSpecialAbility(1.0, this, Combatant, damage, 10.0, -1, true, "", "The reaver's lance impales you, causing you to bleed!", "-1");
                    AOS.Damage(Combatant, (int)damage, 100, 0, 0, 0, 0); 

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

                    SpecialAbilities.HinderSpecialAbility(1.0, this, mobile, 1.0, 2, false, -1, false, "", "You have been trampled and can't move!", "-1");
                }
            }

            CheckChargeResolved();

            return base.OnMove(d);
        }

        public BloodyReaver(Serial serial): base(serial)
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
