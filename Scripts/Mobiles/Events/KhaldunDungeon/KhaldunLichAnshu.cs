using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("anshu's corpse")]
    public class KhaldunLichAnshu : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public int damageIntervalThreshold = 1500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public bool AbilityInProgress = false;

        public List<Mobile> m_Creatures = new List<Mobile>();

        public string[] idleSpeech
        {
            get { return new string[] { "*chants*" }; }
        }

        public string[] combatSpeech
        {
            get { return new string[] { "" }; }
        }

        [Constructable]
        public KhaldunLichAnshu(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Anshu the Breath of Life";
            
            Body = 830;
            Hue = 1154;

            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(30000);
            SetStam(5000);
            SetMana(30000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 300);

            SetSkill(SkillName.MagicResist, 150);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {                           
            UniqueCreatureDifficultyScalar = 1.5;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            CombatEpicActionMinDelay = 10;
            CombatEpicActionMaxDelay = 20;

            DictCombatAction[CombatAction.CombatEpicAction] = 25;
            DictCombatEpicAction[CombatEpicAction.MassiveBoneBreathAttack] = 25;

            SpellHue = Hue - 1;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AlwaysBoss { get { return true; } }        
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }

        public int GetWindItemId(Direction direction, bool small)
        {
            int windItemId = 0;

            if (small)
            {
                switch (direction)
                {
                    case Direction.North: windItemId = 8099; break;
                    case Direction.Right: windItemId = 8099; break;

                    case Direction.West: windItemId = 8104; break;
                    case Direction.Up: windItemId = 8104; break;

                    case Direction.East: windItemId = 8109; break;
                    case Direction.Down: windItemId = 8109; break;

                    case Direction.South: windItemId = 8114; break;
                    case Direction.Left: windItemId = 8114; break;
                }
            }

            else
            {
                switch (direction)
                {
                    case Direction.North: windItemId = 8119; break;
                    case Direction.Right: windItemId = 8119; break;

                    case Direction.West: windItemId = 8124; break;
                    case Direction.Up: windItemId = 8124; break;

                    case Direction.East: windItemId = 8129; break;
                    case Direction.Down: windItemId = 8129; break;

                    case Direction.South: windItemId = 8134; break;
                    case Direction.Left: windItemId = 8134; break;
                }
            }

            return windItemId;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!willKill)
            {
                if (from != null && amount > 10)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < .08)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .04)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .12)
                            DamageEffect(from);
                    }
                }

                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    IntervalEffect();
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void DamageEffect(Mobile from)
        {
            if (Deleted || !Alive) return;
            if (from == null) return;
            if (from.Deleted || !from.Alive) return;

            if (AbilityInProgress)
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            Mobile mobile = from;
            Point3D location = Location;
            Map map = Map;

            int effectHue = Hue - 1;
            
            if (InLOS(mobile) && GetDistanceToSqrt(mobile) <= 16)
            {
                Animate(4, 4, 1, true, false, 0);

                double stationaryDelay = 2;

                Combatant = null;
                NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

                AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
                NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
                NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 8), map);

                Animate(15, 8, 1, true, false, 0); //Staff

                Effects.PlaySound(mobile.Location, mobile.Map, 0x64C);

                Direction direction = Utility.GetDirection(location, mobile.Location);
                int windId = GetWindItemId(direction, true);

                MovingEffect(mobile, windId, 5, 1, false, false, effectHue, 0);

                double distance = Utility.GetDistanceToSqrt(location, mobile.Location);
                double destinationDelay = (double)distance * .08;

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (Deleted || !Alive) return;
                    if (mobile == null) return;
                    if (mobile.Deleted || !mobile.Alive) return;

                    Point3D mobileLocation = mobile.Location;

                    int damage = DamageMin;

                    if (mobile is BaseCreature)
                        damage = (int)((double)damage * 2);

                    int knockbackDistance = 4 + (int)(Math.Ceiling(8 * spawnPercent));

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobile, damage, knockbackDistance, -1, "", "You are knocked back!");

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (Deleted || !Alive) return;

                        int particleSpeed = 5;

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 3), Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(mobileLocation.X, mobileLocation.Y, mobileLocation.Z + 3), map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 0, 0);

                        double newDistance = Utility.GetDistanceToSqrt(Location, mobileLocation);
                        double newDestinationDelay = (double)newDistance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(newDestinationDelay), delegate
                        {                           
                            if (Deleted || !Alive) return;

                            Effects.PlaySound(mobileLocation, map, 0x653);

                            Effects.SendLocationParticles(effectEndLocation, 0x3709, 10, 20, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3779, 10, 60, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3996, 10, 60, effectHue, 0, 5029, 0);

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                            {  
                                if (Deleted || !Alive) return;

                                BaseCreature monsterToSpawn = null;

                                int maxCreatureValue = 1 + (int)(Math.Ceiling(10 * spawnPercent));

                                if (maxCreatureValue > 10)
                                    maxCreatureValue = 10;

                                switch (Utility.RandomMinMax(1, maxCreatureValue))
                                {
                                    case 1: monsterToSpawn = new Zombie(); break;
                                    case 2: monsterToSpawn = new Skeleton(); break;
                                    case 3: monsterToSpawn = new Ghoul(); break;
                                    case 4: monsterToSpawn = new PatchworkSkeleton(); break;
                                    case 5: monsterToSpawn = new ZombieMagi(); break;
                                    case 6: monsterToSpawn = new SkeletalMage(); break;
                                    case 7: monsterToSpawn = new SkeletalKnight(); break;
                                    case 8: monsterToSpawn = new Mummy(); break;
                                    case 9: monsterToSpawn = new SkeletalDrake(); break;
                                    case 10: monsterToSpawn = new RottingCorpse(); break;
                                }

                                if (monsterToSpawn != null)
                                {
                                    monsterToSpawn.EventMinion = true;
                                    monsterToSpawn.MoveToWorld(mobileLocation, map);

                                    m_Creatures.Add(monsterToSpawn);
                                }
                            });
                        });
                    });
                });
            }            
        }

        public void IntervalEffect()
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            AbilityInProgress = true;

            int summonMotions = 5 - (int)(Math.Ceiling(4 * spawnPercent));
            double summonDuration = 1;

            int creaturesToSummon = 2 + (int)(Math.Ceiling(8 * spawnPercent));

            double stationaryDelay = summonMotions * summonDuration + 1;

            double minRange = 8;
            double maxRange = 24;

            Point3D location = Location;
            Map map = Map;

            int range = 10 + (int)(Math.Ceiling(20 * spawnPercent));
            int effectHue = Hue - 1;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon the breath of the living*");

            for (int a = 0; a < summonMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * summonDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);

                    Queue m_Queue = new Queue();

                    IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);                    

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile.Deleted) continue;
                        if (!mobile.Alive) continue;
                        if (mobile == this) continue;
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
                            m_Queue.Enqueue(mobile);
                    }

                    mobilesInRange.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();
                        
                        mobile.MovingEffect(this, 0x3728, 5, 1, false, false, effectHue, 0);
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)summonMotions * summonDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PlaySound(0x64F);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);                

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
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
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    int distance = range - (int)GetDistanceToSqrt(mobile);

                    double damage = DamageMax;

                    if (mobile is BaseCreature)
                        damage *= 2;

                    Direction direction = Utility.GetDirection(location, mobileLocation);
                    int windId = GetWindItemId(direction, false);
                    MovingEffect(mobile, windId, 5, 1, false, false, effectHue, 0); 

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobile, damage, distance, -1, "", "You are knocked back by the breath of life!");
                }

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (Deleted || !Alive) return;

                    for (int a = 0; a < creaturesToSummon; a++)
                    {
                        Point3D spawnLocation = Location;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 10, true);

                        if (m_ValidLocations.Count > 0)
                            spawnLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];                        

                        int particleSpeed = 5;

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 3), Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(spawnLocation.X, spawnLocation.Y, spawnLocation.Z + 3), map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 0, 0);

                        double distance = Utility.GetDistanceToSqrt(Location, spawnLocation);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {  
                            if (Deleted || !Alive) return;

                            Effects.PlaySound(spawnLocation, map, 0x653);

                            Effects.SendLocationParticles(effectEndLocation, 0x3709, 10, 20, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3779, 10, 60, effectHue, 0, 5029, 0);
                            Effects.SendLocationParticles(effectEndLocation, 0x3996, 10, 60, effectHue, 0, 5029, 0);

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                            {
                                if (this == null) return;
                                if (Deleted || !Alive) return;

                                BaseCreature monsterToSpawn = null;

                                int maxCreatureValue = 1 + (int)(Math.Ceiling(10 * spawnPercent));

                                if (maxCreatureValue > 10)
                                    maxCreatureValue = 10;

                                switch (Utility.RandomMinMax(1, maxCreatureValue))
                                {
                                    case 1: monsterToSpawn = new Zombie(); break;
                                    case 2: monsterToSpawn = new Skeleton(); break;
                                    case 3: monsterToSpawn = new Ghoul(); break;
                                    case 4: monsterToSpawn = new PatchworkSkeleton(); break;
                                    case 5: monsterToSpawn = new ZombieMagi(); break;
                                    case 6: monsterToSpawn = new SkeletalMage(); break;
                                    case 7: monsterToSpawn = new SkeletalKnight(); break;
                                    case 8: monsterToSpawn = new Mummy(); break;
                                    case 9: monsterToSpawn = new SkeletalDrake(); break;
                                    case 10: monsterToSpawn = new RottingCorpse(); break;
                                }

                                if (monsterToSpawn != null)
                                {
                                    monsterToSpawn.BossMinion = true;
                                    monsterToSpawn.MoveToWorld(spawnLocation, map);

                                    m_Creatures.Add(monsterToSpawn);
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
            
            if (Utility.RandomDouble() < 0.01 && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*cackles*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*sneers*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*scowls*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*seethes*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*curses*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                Animate(27, 10, 1, true, false, 0); //Sneer
                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
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
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }
        
        public override int GetAngerSound() { return 0x2BC; }
        public override int GetIdleSound() { return 0x2B9; }
        public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x621; }
        public override int GetDeathSound() { return 0x58D; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x654);

            return base.OnMove(d);
        }

        public KhaldunLichAnshu(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

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



