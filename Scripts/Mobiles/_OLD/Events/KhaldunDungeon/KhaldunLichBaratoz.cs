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
    [CorpseName("baratoz's corpse")]
    public class KhaldunLichBaratoz : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public int damageIntervalThreshold = 1500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public List<Mobile> m_Creatures = new List<Mobile>();

        public bool AbilityInProgress = false;

        public string[] idleSpeech
        {
            get { return new string[] { "*chants*" }; }
        }

        public string[] combatSpeech
        {
            get { return new string[] { "" }; }
        }

        [Constructable]
        public KhaldunLichBaratoz(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Baratoz the Keeper of the Pit";

            Body = 830;
            Hue = 1102;

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

        public override int PoisonResistance { get { return 5; } }

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
                            if (Utility.RandomDouble() < .1)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .05)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .15)
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
            if (from == null) return;
            if (from.Deleted || !from.Alive) return;

            BaseCreature bc_Defender = from as BaseCreature;
            PlayerMobile pm_Defender = from as PlayerMobile;

            if (AbilityInProgress)
                return;

            double totalValue = 0;

            if (bc_Defender != null)
            {
                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            if (pm_Defender != null)
            {
                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            Animate(15, 8, 1, true, false, 0); //Staff

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.2)), 0x372A, 6, 20, 2636, 0, 5029, 0);
            Effects.PlaySound(from.Location, from.Map, 0x457);

            for (int a = 0; a < 4; a++)
            {
                TimedStatic pitResidue = new TimedStatic(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), 5);
                pitResidue.Name = "pit residue";
                pitResidue.MoveToWorld(new Point3D(from.X + Utility.RandomList(-1, 1), from.Y + Utility.RandomList(-1, 1), from.Z), from.Map);
            }

            SpecialAbilities.DiseaseSpecialAbility(1.0, this, from, 10, 30, 0x62B, true, "", "They have struck you with an otherwordly malady!");
        }

        public void IntervalEffect()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int pitTentacles = 2 + (int)(Math.Ceiling(8 * spawnPercent));

            Effects.PlaySound(Location, Map, 0x653);            

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
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    PlaySound(GetAngerSound());
                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;
            });

            for (int a = 0; a < pitTentacles; a++)
            {
                Point3D location = Location;
                Point3D pitLocation = Location;
                Map map = Map;

                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

                if (m_ValidLocations.Count > 0)
                    pitLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                for (int b = 1; b < 9; b++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(b * .25), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;

                        Effects.PlaySound(pitLocation, map, 0x4CF);

                        TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                        floorCrack.Name = "floor crack";

                        Point3D floorCrackLocation = new Point3D(pitLocation.X + Utility.RandomList(-1, 1), pitLocation.Y + Utility.RandomList(-1, 1), pitLocation.Z);
                        SpellHelper.AdjustField(ref floorCrackLocation, map, 12, false);

                        floorCrack.MoveToWorld(floorCrackLocation, map);                        
                    });
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1.5), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    TimedStatic floorHole = new TimedStatic(7025, 5);
                    floorHole.Name = "pit to below";
                    floorHole.MoveToWorld(pitLocation, map);                   
                });

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    IEntity pitLocationEntity = new Entity(Serial.Zero, new Point3D(pitLocation.X, pitLocation.Y, pitLocation.Z), map);
                    Effects.SendLocationParticles(pitLocationEntity, 0x3709, 10, 30, 2053, 0, 5052, 0);

                    for (int b = 0; b < 4; b++)
                    {
                        TimedStatic pitPlasm = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 5);
                        pitPlasm.Name = "pit plasm";
                        pitPlasm.Hue = 2052;

                        Point3D pitPlasmLocation = new Point3D(pitLocation.X + Utility.RandomList(-1, 1), pitLocation.Y + Utility.RandomList(-1, 1), pitLocation.Z);
                        SpellHelper.AdjustField(ref pitPlasmLocation, map, 12, false);

                        pitPlasm.MoveToWorld(pitPlasmLocation, map);
                    }

                    BaseCreature m_PitCreature = null;

                    double creatureResult = Utility.RandomDouble();

                    if (creatureResult <= .10)
                    {
                        m_PitCreature = new FountainOfEvil();
                        m_PitCreature.Name = "enveloping darkness";
                        m_PitCreature.CorpseNameOverride = "enveloping darkness corpse";
                        m_PitCreature.Hue = 2250;
                        m_PitCreature.BodyValue = 780;
                    }

                    else if (creatureResult <= .25)
                    {
                        m_PitCreature = new HookHorror();
                        m_PitCreature.Name = "a clawfiend";
                        m_PitCreature.CorpseNameOverride = "a clawfiend's corpse";
                        m_PitCreature.Hue = 1107;
                        m_PitCreature.BodyValue = 303;
                    }

                    else if (creatureResult <= .50)
                    {
                        m_PitCreature = new SvirfneblinRogue();
                        m_PitCreature.Name = "an underling";
                        m_PitCreature.CorpseNameOverride = "an underling's corpse";
                        m_PitCreature.Hue = 2076;
                        m_PitCreature.BodyValue = 776;
                    }

                    else                    
                        m_PitCreature = new PitTentacle();                    

                    if (m_PitCreature != null)
                    {
                        m_PitCreature.BossMinion = true;
                        m_PitCreature.MoveToWorld(pitLocation, map);

                        m_Creatures.Add(m_PitCreature);
                    }
                });
            }            
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

        public KhaldunLichBaratoz(Serial serial): base(serial)
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



