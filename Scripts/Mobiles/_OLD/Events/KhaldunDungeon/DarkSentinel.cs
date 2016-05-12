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
using System.Collections.Generic;


namespace Server.Mobiles
{
	[CorpseName( "dark sentinel's corpse" )]
	public class DarkSentinel : BaseCreature
	{
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(60);

        public int damageIntervalThreshold = 1000;
        public int totalIntervals = 20;

        public int damageProgress = 0;
        public int intervalCount = 0;

        public List<Mobile> m_Creatures = new List<Mobile>();

        public List<Point3D> m_BackupLocations = new List<Point3D>();

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

        public enum RiftType
        {
            Normal,
            SpawnRiftlings,
            EnergyBolts
        }

		[Constructable]
		public DarkSentinel() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "Dark Sentinel";

            Body = 318;
            Hue = 2076;

            BaseSoundID = 0x165;

			SetStr(100);
			SetDex(75);
			SetInt(25);

			SetHits(20000);
            SetStam(15000);
            SetMana(10000);

			SetDamage(20, 40);

            AttackSpeed = 50;

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            SetSkill(SkillName.DetectHidden, 85);

            VirtualArmor = 50;

			Fame = 10000;
			Karma = -10000;			
		}

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.DetectHidden] = 2;

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

        public override bool RevealImmune { get { return true; } }
        public override bool AlwaysBoss { get { return true; } } 
        public override bool AlwaysMurderer { get { return true; } }

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

            if (!Hidden && IsStealthing)
                IsStealthing = false;
            
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

                Combatant = null;

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;

                return;
            }  

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed && !Hidden)
            {
                if (Combatant != null)
                {
                    double stationaryDuration = 3;

                    RiftType riftType = RiftType.Normal;

                    double randomValue = Utility.RandomDouble();

                    if (randomValue <= .33)
                    {
                        stationaryDuration = 3;
                        riftType = RiftType.Normal;
                    }

                    else if (randomValue <= .66)
                    {
                        stationaryDuration = 8;
                        riftType = RiftType.EnergyBolts;
                    }

                    else
                    {
                        stationaryDuration = 13;
                        riftType = RiftType.SpawnRiftlings;
                    }

                    CantWalk = true;
                    Frozen = true;
                    
                    AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                    LastSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);

                    NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(2);
                    NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(2);
                    NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(2);
                    NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(2);
                    
                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;

                    Effects.PlaySound(Location, Map, 0x5CE);
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(0.5)), 6899, 10, 30, 1108, 0, 5029, 0);

                    for (int a = 0; a < 2; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                        {
                            if (this == null) return;
                            if (!Alive || Deleted) return;

                            CantWalk = true;
                            Frozen = true;

                            PlaySound(GetIdleSound());
                            Animate(6, 4, 1, true, false, 0);
                        });
                    }

                    Point3D location = Location;
                    Point3D riftLocation = Location; 
                    Map map = Map;

                    Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                    {
                        if (this == null) return;
                        if (!Alive || Deleted) return;

                        Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(0.5)), 3546, 10, 150, 1108, 0, 5029, 0);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                    {
                        if (this == null) return;
                        if (!Alive || Deleted) return;

                        CantWalk = false;
                        Frozen = false;

                        if (SpecialAbilities.VanishAbility(this, stationaryDuration, true, 0x5C8, 4, 8, true, m_BackupLocations))                        
                            PublicOverheadMessage(MessageType.Regular, 0, false, "*disappears into the rift*");                      

                        AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDuration);
                        NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDuration);

                        List<Mobile> m_PossibleMobiles = new List<Mobile>();
                        IPooledEnumerable mobilesInArea;

                        double spawnPercent = (double)intervalCount / (double)totalIntervals;

                        switch (riftType)
                        {
                            case RiftType.Normal:
                            break;

                            case RiftType.EnergyBolts:
                                m_PossibleMobiles = new List<Mobile>();
                                mobilesInArea = map.GetMobilesInRange(riftLocation, 15);
                                
                                foreach (Mobile mobile in mobilesInArea)
                                {
                                    if (mobile == null) continue;
                                    if (mobile == this) continue;
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

                                    if (validTarget)
                                        m_PossibleMobiles.Add(mobile);                                          
                                }
                                
                                mobilesInArea.Free();

                                int boltCount = 10 + (int)(Math.Ceiling(25 * spawnPercent));

                                if (boltCount > 35)
                                    boltCount = 35;

                                for (int a = 0; a < boltCount; a++)
                                {
                                    Timer.DelayCall(TimeSpan.FromSeconds(a * .2), delegate
                                    {
                                        if (this == null) return;
                                        if (Deleted || !Alive) return;

                                        m_PossibleMobiles = new List<Mobile>();
                                        mobilesInArea = map.GetMobilesInRange(riftLocation, 15);

                                        foreach (Mobile mobile in mobilesInArea)
                                        {
                                            if (mobile == null) continue;
                                            if (mobile == this) continue;
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

                                            if (!InLOS(mobile)) continue;

                                            m_PossibleMobiles.Add(mobile);                                          
                                        }

                                        mobilesInArea.Free();

                                        if (m_PossibleMobiles.Count == 0)
                                            return;

                                        Mobile target = m_PossibleMobiles[Utility.RandomMinMax(0, m_PossibleMobiles.Count - 1)];

                                        Effects.PlaySound(riftLocation, map, 0x5C6);

                                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(riftLocation.X, riftLocation.Y, riftLocation.Z + 8), map);
                                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 6), map);

                                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x36D4, 5, 0, false, false, 2614, 0);
                                     
                                        double distance = this.GetDistanceToSqrt(target.Location);
                                        double destinationDelay = (double)distance * .10;

                                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                                        {
                                            if (this == null) return;
                                            if (!this.Alive || this.Deleted) return;
                                            if (target == null) return;
                                            if (!target.Alive || target.Deleted) return;

                                            Effects.PlaySound(Location, map, 0x5CF);

                                            int damage = Utility.RandomMinMax(5, 15);

                                            if (target is BaseCreature)
                                                damage = (int)((double)damage * 1.5);

                                            AOS.Damage(target, damage, 0, 100, 0, 0, 0);
                                            new Blood().MoveToWorld(target.Location, target.Map);

                                            IEntity locationEntity = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 5), map);
                                            Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2614, 0, 5044, 0);
                                        });
                                    });
                                }
                            break;

                            case RiftType.SpawnRiftlings:
                                List<BaseCreature> m_CreaturesToMove = new List<BaseCreature>();

                                m_PossibleMobiles = new List<Mobile>();
                                mobilesInArea = map.GetMobilesInRange(riftLocation, 20);

                                foreach (Mobile mobile in mobilesInArea)
                                {
                                    if (mobile == null) continue;
                                    if (mobile == this) continue;
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
                                    
                                    m_PossibleMobiles.Add(mobile);                                          
                                }

                                mobilesInArea.Free();

                                int riftlingCount = 2 + (int)(Math.Ceiling(6 * spawnPercent));
                                
                                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                                {
                                    if (this == null) return;
                                    if (!Alive || Deleted) return;

                                    for (int a = 0; a < riftlingCount; a++)
                                    {
                                        double randomCreature = Utility.RandomDouble();

                                        if (randomCreature <= .30)
                                        {
                                            Imp riftling = new Imp();

                                            riftling.Hue = 2615;
                                            riftling.Tameable = false;
                                            riftling.Name = "an impish riftling";
                                            riftling.CorpseNameOverride = "an impish riftling corpse";

                                            riftling.BossMinion = true;

                                            riftling.MoveToWorld(riftLocation, map);
                                            m_Creatures.Add(riftling);
                                            m_CreaturesToMove.Add(riftling);
                                        }

                                        else if (randomCreature <= .60)
                                        {
                                            MongbatLord riftling = new MongbatLord();

                                            riftling.Hue = 2615;
                                            riftling.Tameable = false;
                                            riftling.Name = "a bat-like riftling";
                                            riftling.CorpseNameOverride = "a bat-like riftling corpse";

                                            riftling.BossMinion = true;

                                            riftling.MoveToWorld(riftLocation, map);
                                            m_Creatures.Add(riftling);
                                            m_CreaturesToMove.Add(riftling);
                                        }

                                        else if (randomCreature <= .80)
                                        {
                                            Gazer riftling = new Gazer();

                                            riftling.Hue = 2615;
                                            riftling.Tameable = false;
                                            riftling.Name = "a gazing riftling";
                                            riftling.CorpseNameOverride = "a gazing riftling corpse";

                                            riftling.BossMinion = true;

                                            riftling.MoveToWorld(riftLocation, map);
                                            m_Creatures.Add(riftling);
                                            m_CreaturesToMove.Add(riftling);
                                        }

                                        else
                                        {
                                            Aboleth riftling = new Aboleth();

                                            riftling.Hue = 2615;
                                            riftling.Tameable = false;
                                            riftling.Name = "a strange riftling";
                                            riftling.CorpseNameOverride = "a strange riftling corpse";

                                            riftling.BossMinion = true;

                                            riftling.MoveToWorld(riftLocation, map);
                                            m_Creatures.Add(riftling);
                                            m_CreaturesToMove.Add(riftling);
                                        }
                                    }

                                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                    {
                                        if (this == null) return;
                                        if (!Alive || Deleted) return;

                                        foreach (BaseCreature creature in m_CreaturesToMove)
                                        {
                                            if (creature == null) continue;
                                            if (!creature.Alive || creature.Deleted) continue;
                                            if (creature.AIObject == null) continue;

                                            creature.AIObject.WalkRandom(0, 1, 2);
                                            Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(0.5)), 14201, 10, 20, 2614, 0, 5029, 0);                                        
                                        }
                                    });
                                });
                            break;
                        }                                          
                    });
                }
            }            
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
                
        public override int GetAngerSound() { return 0x52D; }
        public override int GetIdleSound() { return 0x572; }
        public override int GetAttackSound() { return 0x516; } //0x5D7
        public override int GetHurtSound() { return 0x5D5; }
        public override int GetDeathSound() { return 0x517; }        

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x60B));

            return base.OnMove(d);
        }

        public DarkSentinel(Serial serial): base(serial)
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
