using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "daemonic overlord's corpse" )]
	public class DaemonicOverlord : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextFireBlastAllowed;
        public TimeSpan NextFireBlastDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextMeteorRainAllowed;
        public TimeSpan NextMeteorRainDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityAllowedDelay = TimeSpan.FromSeconds(3);
               
        public int damageIntervalThreshold = 1000;        
        public int totalIntervals = 20;

        public int damageProgress = 0;
        public int intervalCount = 0;

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }        

		[Constructable]
		public DaemonicOverlord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Daemonic Overlord";

            Body = 1071;
            Hue = 2600;

            BaseSoundID = 357;

			SetStr(100);
			SetDex(75);
			SetInt(25);

			SetHits(30000);
            SetStam(15000);
            SetMana(20000);

			SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 125);

            SetSkill(SkillName.MagicResist, 125);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 50;
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;
            
            UniqueCreatureDifficultyScalar = 1.33;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override bool AlwaysBoss { get { return true; } }       
        public override bool AlwaysMurderer { get { return true; } }
               
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double flameAttackChance = .20 + (.20 * spawnPercent);

            if (Utility.RandomDouble() <= flameAttackChance)
                FlameAttack(defender);            
        }

        public void FlameAttack(Mobile target)
        {
            if (target == null) return;
            if (!target.Alive || target.Deleted) return;
            
            target.FixedParticles(0x3709, 10, 30, 5052, 2613, 0, EffectLayer.LeftFoot);
            target.PlaySound(Utility.RandomList(0x5CF));

            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.5)), 0x3996, 50, 40, 2613, 0, 5029, 0);

            double damage = DamageMin;

            if (target is BaseCreature)
                damage *= 1.5;

            AOS.Damage(target, (int)damage, 0, 100, 0, 0, 0);
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
                }
            }            

            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && DateTime.UtcNow > m_NextMeteorRainAllowed && DateTime.UtcNow > m_NextAbilityAllowed)
            {
                double spawnPercent = (double)intervalCount / (double)totalIntervals;

                int meteors = 30 + (int)(Math.Ceiling(50 * spawnPercent));

                int loops = (int)(Math.Ceiling((double)meteors / 10));

                double stationaryDelay = loops + 2.5;

                m_NextMeteorRainAllowed = DateTime.UtcNow + NextMeteorRainDelay + TimeSpan.FromSeconds(stationaryDelay);
                m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityAllowedDelay + TimeSpan.FromSeconds(stationaryDelay);
                
                PlaySound(GetAngerSound());

                AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);

                NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

                Animate(23, 10, 1, true, false, 0);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*rains down hellfire*");

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

                            Animate(23, 10, 1, true, false, 0);                           

                            PlaySound(GetAngerSound());
                            Effects.PlaySound(location, map, 0x5CF);
                        });
                    }                    

                    for (int a = 0; a < meteors; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                        {
                            if (this == null) return;
                            if (Deleted || !Alive) return;

                            Point3D meteorLocation = new Point3D(location.X + Utility.RandomMinMax(-8, 8), location.Y + Utility.RandomMinMax(-8, 8), location.Z);

                            IEntity startLocation = new Entity(Serial.Zero, new Point3D(meteorLocation.X-1, meteorLocation.Y-1, meteorLocation.Z + 100), map);
                            IEntity endLocation = new Entity(Serial.Zero, new Point3D(meteorLocation.X, meteorLocation.Y, meteorLocation.Z + 5), map);

                            int particleSpeed = 8 + (int)(Math.Round(8 * (double)spawnPercent));

                            Effects.SendMovingParticles(startLocation, endLocation, 0x36D4, particleSpeed, 0, false, false, 2613, 0, 9501, 0, 0, 0x100);

                            double impactDelay = .75 - (.375 * spawnPercent);

                            Timer.DelayCall(TimeSpan.FromSeconds(impactDelay), delegate
                            {
                                if (this == null) return;
                                if (Deleted || !Alive) return;

                                Effects.PlaySound(endLocation, map, 0x56E);
                                Effects.SendLocationParticles(endLocation, 0x3709, 10, 20, 2613, 0, 5029, 0);                               

                                Blood rocks = new Blood();
                                rocks.Name = "rocks";
                                rocks.Hue = 2615;
                                rocks.ItemID = Utility.RandomList(4967, 4970, 4973);

                                Point3D rockLocation = new Point3D(meteorLocation.X, meteorLocation.Y, meteorLocation.Z);

                                rocks.MoveToWorld(rockLocation, map);

                                double firefieldChance = .20 + (.20 * spawnPercent);

                                if (Utility.RandomDouble() <= firefieldChance)
                                {
                                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                                    {
                                        SingleFireField singleFireField = new SingleFireField(null, 2613, 1, 30, 3, 5, false, false, true, -1, true);
                                        singleFireField.MoveToWorld(rockLocation, map);
                                    });
                                }

                                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(meteorLocation, 1);

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

                                    double damage = Utility.RandomMinMax((int)(Math.Round((double)DamageMin / 2)),  DamageMin);

                                    if (mobile is BaseCreature)
                                        damage *= 1.5;

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                }
                            });
                        });
                    }
                });

                return;
            }

            if (Combatant != null && DateTime.UtcNow > m_NextFireBlastAllowed && DateTime.UtcNow > m_NextAbilityAllowed)
            {
                if (Combatant.Alive && !Combatant.Hidden && InLOS(Combatant) && GetDistanceToSqrt(Combatant) <= 12)
                {
                    m_NextFireBlastAllowed = DateTime.UtcNow + NextFireBlastDelay;
                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityAllowedDelay;

                    double effectTime = 1.5;
                    double actionsCooldown = 3;

                    PlaySound(GetAngerSound());

                    AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(effectTime);
                    NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(effectTime);

                    NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

                    Animate(6, 10, 1, true, false, 0);

                    int itemId = 0x573E;
                    int itemHue = 0;

                    Point3D location = Location;
                    Point3D targetLocation = Combatant.Location;
                    Map map = Combatant.Map;

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;
                        
                        Effects.PlaySound(targetLocation, map, 0x5FC);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 20), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 10), map);
                        
                        Effects.SendMovingParticles(startLocation, endLocation, 0x36D4, 8, 0, false, false, 2613, 0, 9501, 0, 0, 0x100);

                        double distance = GetDistanceToSqrt(targetLocation);
                        double destinationDelay = (double)distance * .06;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            double spawnPercent = (double)intervalCount / (double)totalIntervals;

                            Effects.PlaySound(targetLocation, map, 0x357);

                            Dictionary<Point3D, double> m_ExplosionLocations = new Dictionary<Point3D, double>();

                            m_ExplosionLocations.Add(targetLocation, 0);

                            int radius = 2 + (int)(Math.Ceiling(6 * spawnPercent));

                            int minRange = -1 * radius;
                            int maxRange = radius + 1;

                            for (int a = minRange; a < maxRange; a++)
                            {
                                for (int b = minRange; b < maxRange; b++)
                                {
                                    Point3D explosionPoint = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);

                                    int distanceFromCenter = Utility.GetDistance(targetLocation, explosionPoint);

                                    double fireburstChance = .25 + (.25 * spawnPercent);

                                    if (Utility.RandomDouble() <= fireburstChance)
                                    {
                                        if (!m_ExplosionLocations.ContainsKey(explosionPoint))
                                            m_ExplosionLocations.Add(explosionPoint, distance);
                                    }
                                }
                            }

                            foreach (KeyValuePair<Point3D, double> pair in m_ExplosionLocations)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(pair.Value * .10), delegate
                                {
                                    Point3D explosionLocation = pair.Key;
                                    SpellHelper.AdjustField(ref explosionLocation, map, 12, false);

                                    Effects.PlaySound(pair.Key, map, 0x56D);
                                    Effects.SendLocationParticles(EffectItem.Create(explosionLocation, map, TimeSpan.FromSeconds(0.5)), 0x36BD, 20, 10, 2613, 0, 5044, 0);
                                    
                                    IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(pair.Key, 0);

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

                                        double damage = DamageMin;

                                        if (mobile is BaseCreature)
                                            damage *= 1.5;

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);                                                                                
                                    }
                                });
                            }                            
                        });
                    });
                }

                return;
            }
            
            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetAngerSound() { return 0x572; } //0x4F9
        public override int GetIdleSound() { return 0x167; }
        public override int GetAttackSound() { return 0x60B; } //0x632
        public override int GetHurtSound() { return 0x4F7; }
        public override int GetDeathSound() { return 0x4E0; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x4CF));

            return base.OnMove(d);
        }

        public DaemonicOverlord(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

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
