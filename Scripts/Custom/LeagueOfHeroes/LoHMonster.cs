using System;
using Server.Items;
using Server.Custom;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a monster corpse")]
    public class LoHMonster : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public int m_PlayersAtStartOfLoHEvent = 0;
        
        public override int MaxDistanceAllowedFromHome { get { return 50; } }

        public DateTime m_NextAbilityAllowed;
        public bool AbilityInProgress = false;

        public double NextAbilityDelayMin = 10;
        public double NextAbilityDelayMax = 5;

        public double SpawnPercent = 0;

        public virtual string[] idleSpeech { get { return new string[0]; } }
        public virtual string[] combatSpeech { get { return new string[0]; } }

        public virtual int MinBaseHits { get { return 1500; } }
        public virtual int MaxBaseHits { get { return 2000; } }

        public virtual int MinExtraHitsPerPlayer { get { return 750; } }
        public virtual int MaxExtraHitsPerPlayer { get { return 1000; } }

        public virtual double ExtraDamageScalarPerPlayer { get { return .02; } }

        public virtual double WrestlingAdjustmentScalarPerPlayer { get { return .02; } }
        public virtual double MageryAdjustmentScalarPerPlayer { get { return .02; } }
        public virtual double EvalIntAdjustmentScalarPerPlayer { get { return .02; } }
        public virtual double MagicResistAdjustmentScalarPerPlayer { get { return .02; } }

        public virtual double VirtualArmorAdjustmentScalarPerPlayer { get { return .02; } }

        [Constructable]
        public LoHMonster(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "LoH Monster";

            Body = 62;
            Hue = 2503;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(2500);
            SetStam(10000);
            SetMana(10000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 50;

            Fame = 10000;
            Karma = -10000;
        }

        public virtual void ConfigureCreature()
        {
            SetHits(Utility.RandomMinMax(MinBaseHits, MaxBaseHits) + (m_PlayersAtStartOfLoHEvent * Utility.RandomMinMax(MinExtraHitsPerPlayer, MaxExtraHitsPerPlayer)));

            int adjustedDamageMin = (int)Math.Round(DamageMin * (1 + ((double)m_PlayersAtStartOfLoHEvent * ExtraDamageScalarPerPlayer)));
            int adjustedDamageMax = (int)Math.Round(DamageMax * (1 + ((double)m_PlayersAtStartOfLoHEvent * ExtraDamageScalarPerPlayer)));

            SetDamage(adjustedDamageMin, adjustedDamageMax);
            
            SetSkill(SkillName.Wrestling, Skills.Wrestling.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * WrestlingAdjustmentScalarPerPlayer)));
            SetSkill(SkillName.Archery, Skills.Archery.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * WrestlingAdjustmentScalarPerPlayer)));
            SetSkill(SkillName.Fencing, Skills.Fencing.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * WrestlingAdjustmentScalarPerPlayer)));
            SetSkill(SkillName.Macing, Skills.Macing.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * WrestlingAdjustmentScalarPerPlayer)));
            SetSkill(SkillName.Swords, Skills.Swords.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * WrestlingAdjustmentScalarPerPlayer)));

            SetSkill(SkillName.Magery, Skills.Magery.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * MageryAdjustmentScalarPerPlayer)));
            SetSkill(SkillName.EvalInt, Skills.EvalInt.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * EvalIntAdjustmentScalarPerPlayer)));

            SetSkill(SkillName.MagicResist, Skills.MagicResist.Value * (1 + ((double)m_PlayersAtStartOfLoHEvent * MagicResistAdjustmentScalarPerPlayer)));

            VirtualArmor = (int)(Math.Round((double)VirtualArmor * (1 + ((double)m_PlayersAtStartOfLoHEvent * VirtualArmorAdjustmentScalarPerPlayer))));
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ActiveSpeed = 0.25;
            PassiveSpeed = 0.25;
            CurrentSpeed = 0.25;

            SetGroup(AIGroup.EvilMonster);
            SetSubGroup(AISubgroup.None);
            UpdateAI(false);

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }

        public virtual void SpawnComplete()
        {
        }

        public override bool AlwaysLoHBoss { get { return true; } }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanRummageCorpses { get { return false; } }       
        public override bool ShowFameTitle { get { return false; } }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            SpawnPercent = ((double)HitsMax - (double)Hits) / (double)HitsMax;
        }

        public TimeSpan GetNextAbilityDelay()
        {
            return TimeSpan.FromSeconds(NextAbilityDelayMin - ((NextAbilityDelayMin - NextAbilityDelayMax) * SpawnPercent));
        }

        public override void OnThink()
        {
            base.OnThink();

            SpawnPercent = ((double)HitsMax - (double)Hits) / (double)HitsMax;

            Point3D location = Location;
            Map map = Map;

            //Outside of Valid Home Region
            if (Utility.GetDistance(Home, location) > MaxDistanceAllowedFromHome)
            {
                TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                dirt.Name = "dirt";
                dirt.MoveToWorld(location, map);

                dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*returns to familiar territory*");

                Effects.PlaySound(location, map, 0x657);

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

                Location = Home;
                Combatant = null;

                return;
            }

            if (!AbilityInProgress && Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null && idleSpeech.Length > 0)
                    Say(idleSpeech[Utility.RandomMinMax(0, idleSpeech.Length - 1)]);

                else if (Combatant != null && combatSpeech.Length > 0)
                    Say(combatSpeech[Utility.RandomMinMax(0, combatSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }
        }

        public LoHMonster(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_PlayersAtStartOfLoHEvent);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_PlayersAtStartOfLoHEvent = reader.ReadInt();
            }
        }
    }
}
