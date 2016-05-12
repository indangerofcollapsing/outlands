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
	[CorpseName( "lord of bone's corpse" )]
	public class LordOfBones : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSkullSpikesAllowed;
        public TimeSpan NextSkullSpikesDelay = TimeSpan.FromSeconds(30);

        public bool m_SummoningSkullSpikes = false;

        public DateTime m_NextThrownSkullAllowed;
        public TimeSpan NextThrownSkullDelay = TimeSpan.FromSeconds(30);

        public int damageIntervalThreshold = 1000;
        public int totalIntervals = 20;

        public int damageProgress = 0;
        public int intervalCount = 0;

        public List<Mobile> m_Creatures = new List<Mobile>();

		[Constructable]
		public LordOfBones() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Lord of Bones";

			Body = 308;
            Hue = 2653;

            BaseSoundID = 0x48D;

			SetStr(100);
			SetDex(50);
			SetInt(25);

			SetHits(20000);
            SetStam(10000);
            SetMana(0);

			SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.MagicResist, 100);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 100;

            PackItem(new Bone(100));
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            CombatEpicActionMinDelay = 20;
            CombatEpicActionMaxDelay = 30;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MassiveBoneBreathAttack] = 25;

            UniqueCreatureDifficultyScalar = 1.33;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override int PoisonResistance { get { return 5; } }
        public override bool AlwaysBoss { get { return true; } }        
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double knockbackChance = .20 + (.40 * spawnPercent);

            if (Utility.RandomDouble() <= knockbackChance)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*flings them aside*");

                double damage = DamageMax;

                SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 10, -1, "", "The creature flings you aside!");

                Combatant = null;
            }
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

            if (Combatant != null)
            {
                if (Utility.RandomDouble() < .1 && DateTime.UtcNow > m_NextThrownSkullAllowed && !m_SummoningSkullSpikes)
                {
                    ThrowSkull(this);
                    return;
                }
            }

            if (Combatant != null)
            {
                if (Utility.RandomDouble() < .1 && DateTime.UtcNow > m_NextSkullSpikesAllowed && !m_SummoningSkullSpikes)
                {
                    SummonSkullSpikes(this);
                    return;
                }
            }

            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed && !m_SummoningSkullSpikes)
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

                Combatant = null;

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public void ThrowSkull(BaseCreature creature)
        {
            if (creature == null) return;
            if (creature.Deleted || !creature.Alive) return;

            if (creature.Combatant != null)
            {
                Mobile combatant = creature.Combatant;
                Point3D location = creature.Location;
                Map map = creature.Map;

                if (combatant.Alive && creature.InLOS(combatant) && creature.GetDistanceToSqrt(combatant) <= 12)
                {
                    Animate(4, 4, 1, true, false, 0);

                    double stationaryDuration = 3;

                    creature.CantWalk = true;
                    creature.Frozen = true;

                    creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(stationaryDuration);
                    creature.LastSwingTime = creature.LastSwingTime + TimeSpan.FromSeconds(stationaryDuration);
                    creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(stationaryDuration);
                    creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDuration);
                    creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDuration);
                    creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDuration);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z + 8), map);

                    int attackSound = 0x5D3;
                    int hitSound = 0x653;

                    Effects.PlaySound(location, map, attackSound);
                    creature.MovingEffect(combatant, 8707, 8, 1, false, false, 2612, 0);

                    double distance = Utility.GetDistanceToSqrt(creature.Location, combatant.Location);
                    double destinationDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (creature == null) return;
                        if (creature.Deleted || !creature.Alive) return;
                        
                        creature.CantWalk = false;
                        creature.Frozen = false;

                        m_NextThrownSkullAllowed = DateTime.UtcNow + NextThrownSkullDelay;                        

                        if (!SpecialAbilities.IsDamagable(combatant))
                            return;

                        Effects.PlaySound(combatant.Location, combatant.Map, hitSound);

                        Effects.SendLocationParticles(EffectItem.Create(combatant.Location, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 20, 2613, 0, 5029, 0);
                        Effects.SendLocationParticles(EffectItem.Create(combatant.Location, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 60, 2613, 0, 5029, 0);
                        Effects.SendLocationParticles(EffectItem.Create(combatant.Location, map, TimeSpan.FromSeconds(0.5)), 0x3996, 10, 60, 2613, 0, 5029, 0);
                        
                        BaseCreature monsterToSpawn = null;

                        double spawnPercent = (double)intervalCount / (double)totalIntervals;

                        int creatureCount = 1 + (int)(Math.Ceiling(3 * spawnPercent));

                        for (int a = 0; a < creatureCount; a++)
                        {
                            switch (Utility.RandomMinMax(1, 10))
                            {
                                case 1: monsterToSpawn = new Skeleton(); break;
                                case 2: monsterToSpawn = new Skeleton(); break;
                                case 3: monsterToSpawn = new Skeleton(); break;
                                case 4: monsterToSpawn = new PatchworkSkeleton(); break;
                                case 5: monsterToSpawn = new PatchworkSkeleton(); break;
                                case 6: monsterToSpawn = new SkeletalKnight(); break;
                                case 7: monsterToSpawn = new UndeadKnight(); break;                                
                                case 8: monsterToSpawn = new RisenKnight(); break;
                                case 9: monsterToSpawn = new RisenNoble(); break;
                                case 10: monsterToSpawn = new SkeletalDrake(); break;
                            }

                            if (monsterToSpawn != null)
                            {
                                monsterToSpawn.BossMinion = true;
                                monsterToSpawn.MoveToWorld(combatant.Location, combatant.Map);
                                monsterToSpawn.BossMinion = true;

                                m_Creatures.Add(monsterToSpawn);
                            }
                        }

                        for (int a = 0; a < 5; a++)
                        {
                            Blood bones = new Blood();
                            bones.Name = "bones";
                            bones.ItemID = Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883);
                            Point3D bonesLocation = new Point3D(combatant.Location.X + Utility.RandomList(-1, 1), combatant.Location.Y + Utility.RandomList(-1, 1), combatant.Location.Z);
                            bones.MoveToWorld(bonesLocation, map);
                        }

                        int damage = creature.DamageMin;

                        if (combatant is BaseCreature)
                            damage = (int)((double)damage * 1.5);

                        new Blood().MoveToWorld(combatant.Location, combatant.Map);
                        AOS.Damage(combatant, creature, damage, 0, 100, 0, 0, 0);
                    });  
                }
            }
        }

        public void SummonSkullSpikes(BaseCreature creature)
        {
            if (creature == null) return;
            if (creature.Deleted || !creature.Alive) return;

            Point3D location = creature.Location;
            Map map = creature.Map;

            m_SummoningSkullSpikes = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int stationaryDuration = 4 - (int)(Math.Ceiling(3 * spawnPercent));       

            creature.Frozen = true;
            creature.CantWalk = true;

            creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(stationaryDuration);
            creature.LastSwingTime = creature.LastSwingTime + TimeSpan.FromSeconds(stationaryDuration);
            creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(stationaryDuration);
            creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDuration);
            creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDuration);
            creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDuration);

            List<Point3D> m_Locations = new List<Point3D>();

            IPooledEnumerable mobilesInArea = Map.GetMobilesInRange(creature.Location, 15);

            foreach (Mobile mobile in mobilesInArea)
            {
                if (mobile == null) continue;
                if (mobile == creature) continue;
                if (mobile.Map != map) continue;
                if (!mobile.Alive) continue;                
                if (!mobile.CanBeDamaged()) continue;
                if (mobile.AccessLevel > AccessLevel.Player) continue;
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

                if (!creature.InLOS(mobile))
                    validTarget = false;

                if (validTarget)
                {
                    if (!m_Locations.Contains(mobile.Location))
                        m_Locations.Add(mobile.Location);
                }
            }

            mobilesInArea.Free();

            foreach (Point3D point in m_Locations)
            {
                int bonePileCount = 3;

                for (int a = 0; a < bonePileCount; a++)
                {
                    TimedStatic bonePile = new TimedStatic(Utility.RandomList(6922, 6923, 6924, 6925, 6926, 6927, 6928, 3786, 3787, 3788, 3789, 3790, 3791, 3792, 3793, 3794), stationaryDuration);

                    bonePile.Name = "bones";
                    bonePile.MoveToWorld(point, Map);
                }
            }

            Effects.PlaySound(location, map, 0x222);
            
            for (int a = 0; a < stationaryDuration; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (creature == null) return;
                    if (creature.Deleted || !creature.Alive) return;

                    creature.Frozen = true;
                    creature.CantWalk = true;

                    creature.PlaySound(GetIdleSound());
                    Animate(18, 5, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDuration), delegate
            {
                if (creature == null) return;                
                if (creature.Deleted) return;
                if (!creature.Alive) return;                

                foreach (Point3D point in m_Locations)
                {
                    Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.25)), 8700, 10, 30, 0, 0, 5029, 0);

                    IPooledEnumerable spikePoint = map.GetMobilesInRange(point, 1);

                    bool hitMobile = false;

                    List<Mobile> m_MobilesHit = new List<Mobile>();

                    foreach (Mobile mobile in spikePoint)
                    {                        
                        if (mobile == creature) continue;
                        if (mobile.Location != point) continue;
                        if (mobile.Map != map) continue;
                        if (!mobile.Alive) continue;                        
                        if (!mobile.CanBeDamaged()) continue;
                        if (mobile.AccessLevel > AccessLevel.Player) continue;

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

                        if (!validTarget)
                            continue;

                        m_MobilesHit.Add(mobile);                        
                    }

                    spikePoint.Free();

                    foreach (Mobile mobile in m_MobilesHit)
                    {
                        int damage = DamageMin;

                        if (mobile is BaseCreature)
                            damage = (int)((double)damage * 2);

                        SpecialAbilities.PierceSpecialAbility(1.0, this, mobile, 50, 30, -1, true, "", "Bone shards pierce your armor, reducing it's effectiveness!", "-1");

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, damage, 100, 0, 0, 0, 0);
                    }

                    Effects.PlaySound(point, map, 0x11D);

                    for (int a = 0; a < 5; a++)
                    {
                        Blood dirt = new Blood();
                        dirt.Name = "bones";
                        dirt.ItemID = Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883);
                        Point3D dirtLocation = new Point3D(point.X + Utility.RandomList(-1, 1), point.Y + Utility.RandomList(-1, 1), point.Z);
                        dirt.MoveToWorld(dirtLocation, Map);
                    }

                    int projectiles = 10;
                    int particleSpeed = 8;
                    double distanceDelayInterval = .12;

                    int minRadius = 1;
                    int maxRadius = 5;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(point, true, false, point, map, projectiles, 20, minRadius, maxRadius, false);

                    if (m_ValidLocations.Count == 0)
                        return;

                    for (int a = 0; a < projectiles; a++)
                    {       
                        Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 2), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                        newLocation.Z += 5;

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                    }

                    IEntity locationEntity = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z - 1), map);
                    Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2497, 0, 5044, 0);
                }
                
                m_NextSkullSpikesAllowed = DateTime.UtcNow + NextSkullSpikesDelay;
                m_SummoningSkullSpikes = false;
                
                creature.Frozen = false;
                creature.CantWalk = false;
            });
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
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }
        }
        
        public override int GetAngerSound() { return 0x4FE; }
        public override int GetIdleSound() { return 0x4ED; }
        public override int GetAttackSound() { return 0x627; }
        public override int GetHurtSound() { return 0x628; }
        public override int GetDeathSound() { return 0x489; }        

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            if (Utility.RandomDouble() < .33)
            {
                Blood dirt = new Blood();
                dirt.Name = "dirt";
                dirt.ItemID = Utility.RandomList(7681, 7682);
                dirt.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            for (int a = 0; a < 50; a++)
            {
                Blood bones = new Blood();
                bones.Hue = 0;
                bones.Name = "bones";
                bones.ItemID = Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883);
                Point3D bonesLocation = new Point3D(Location.X + Utility.RandomMinMax(-4, 4), Location.Y + Utility.RandomMinMax(-4, 4), Location.Z + 2);
                bones.MoveToWorld(bonesLocation, Map);
            }

            Effects.PlaySound(Location, Map, 0x1C7);

            return base.OnBeforeDeath();
        }

        public LordOfBones(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            //Version 1
            writer.Write(damageProgress);
            writer.Write(intervalCount);

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

            m_Creatures = new List<Mobile>();

            //Version 1
            if (version >= 1)
            {
                damageProgress = reader.ReadInt();
                intervalCount = reader.ReadInt();                

                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    if (creature != null)
                        m_Creatures.Add(creature);                    
                }
            }
        }
	}
}
