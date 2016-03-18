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
    [CorpseName("kaltivel's corpse")]
    public class KhaldunLichKaltivel : BaseCreature
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
        public KhaldunLichKaltivel(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Kaltivel the Lorekeeper";

            Body = 830;
            Hue = 2614;

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

            Point3D location = Location;
            Map map = Map;

            int effectHue = Hue - 1;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            double manaFeasterChance = .10 + (.20 * spawnPercent);
            
            Animate(15, 8, 1, true, false, 0); //Staff

            Combatant = null;

            if (Utility.RandomDouble() <= manaFeasterChance)
            {
                MindFlayer manaFeaster = new MindFlayer();

                manaFeaster.Name = "a mana feaster";
                manaFeaster.CorpseNameOverride = "a mana feaster";
                manaFeaster.Hue = Hue;

                manaFeaster.EventMinion = true;

                manaFeaster.MoveToWorld(location, map);

                m_Creatures.Add(manaFeaster);

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
            }            
                     
            if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null))
                PublicOverheadMessage(MessageType.Regular, 0, false, "*blinks*");

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (Deleted || !Alive) return;

                if (Hidden)
                    RevealingAction();                

                FixedParticles(0x3763, 10, 30, 5028, effectHue, 0, EffectLayer.Waist);
                PlaySound(0x659);
            });
        }

        public void IntervalEffect()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            Point3D location = Location;
            Map map = Map;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int loreStatues = 3 + (int)(Math.Ceiling(7 * spawnPercent));

            int effectHue = Hue - 1;

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

            for (int a = 0; a < 2; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    PlaySound(GetAngerSound());
                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;
            });
           
            for (int a = 0; a < 4; a++)
            {
                double duration = 22 - (a * .5);

                Timer.DelayCall(TimeSpan.FromSeconds(a * .5), delegate
                {
                    Effects.PlaySound(location, map, 0x1EC);

                    for (int b = 0; b < loreStatues; b++)
                    {
                        if (Deleted || !Alive) return;

                        Point3D runeLocation;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, false, false, Location, Map, 1, 10, 1, 18, true);

                        if (m_ValidLocations.Count > 0)
                            runeLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                        else
                            return;

                        TimedStatic forbiddenRunes = new TimedStatic(Utility.RandomList(3676, 3679, 3682, 3685, 3688), duration);
                        forbiddenRunes.Name = "forbidden rune";
                        SpellHelper.AdjustField(ref runeLocation, map, 12, false);

                        forbiddenRunes.MoveToWorld(runeLocation, map);

                        TimedStatic timedStatic = new TimedStatic(0x3779, .5);
                        timedStatic.Hue = effectHue;
                        timedStatic.Name = "rune energy";
                        timedStatic.MoveToWorld(runeLocation, map);
                    }
                });                    
            }            

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (Deleted || !Alive) return;

                for (int a = 0; a < loreStatues; a++)
                {                       
                    for (int b = 0; b < 3; b++)
                    {
                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(location, false, false, location, map, 1, 10, 1, 15, true);

                        if (m_ValidLocations.Count > 0)
                            location = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        bool foundStatue = false;

                        IPooledEnumerable itemsNearby = map.GetItemsInRange(location, 2);
                        {
                            foreach (Item item in itemsNearby)
                            {
                                if (item is ForbiddenLoreStatue)
                                {
                                    foundStatue = true;
                                    break;
                                }
                            }
                        }

                        itemsNearby.Free();

                        if (foundStatue || location == Location)
                            continue;

                        int statueAttackInterval = Utility.RandomMinMax(4, 6);
                        int durationCount = (int)(Math.Ceiling(30 / (double)statueAttackInterval));

                        ForbiddenLoreStatue statue = new ForbiddenLoreStatue(10, DamageMin, DamageMin, statueAttackInterval, durationCount);
                        statue.MoveToWorld(location, map);                        

                        break;
                    }
                }
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

        public KhaldunLichKaltivel(Serial serial): base(serial)
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



