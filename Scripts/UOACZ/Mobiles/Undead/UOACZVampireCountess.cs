using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server
{
    [CorpseName("a vampire countess corpse")]
    public class UOACZVampireCountess : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override BonePileType BonePile { get { return BonePileType.Medium; } }

        public override int DifficultyValue { get { return 7; } }

        public DateTime m_NextFeedAllowed;
        public TimeSpan NextFeedDelay = TimeSpan.FromSeconds(60);

        public DateTime m_NextFeedTick;
        public TimeSpan NextFeedTickDelay = TimeSpan.FromSeconds(2);

        public DateTime m_FeedExpiration;
        public TimeSpan FeedDuration = TimeSpan.FromSeconds(5);

        public bool IsFeeding = false;
        public int feedTick = 0;    

        public List<Mobile> m_Creatures = new List<Mobile>();

        public static int MaxCreatures = 5;

        public override int AttackAnimation { get { return 6; } }
        public override int AttackFrames { get { return 6; } }

		[Constructable]
		public UOACZVampireCountess() : base()
		{
            Name = "a vampire countess";
            Body = 258;
            Hue = 2500;
            BaseSoundID = 0x4B0;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(400);
            SetMana(2000);

            SetDamage(11, 22);

            SetSkill(SkillName.Wrestling, 75);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);            

            VirtualArmor = 25;
            
            Fame = 4000;
            Karma = -4000;                
		}

        public override void SetUniqueAI()
        {
            AISubGroup = AISubGroupType.MeleeMage2;
            UpdateAI(false);

            base.SetUniqueAI();

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;
            DictWanderAction[WanderAction.SpellHealOther100] = 0;
            DictWanderAction[WanderAction.SpellCureOther] = 0;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            Point3D location = Location;
            Map map = Map;

            if (Utility.RandomDouble() <= .10)
            {
                PlaySound(GetAngerSound());

                FixedParticles(0x375A, 10, 15, 5037, 2499, 0, EffectLayer.Waist);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*transfixes target*");

                SpecialAbilities.PetrifySpecialAbility(1.0, this, attacker, 1.0, 5.0, -1, true, "", "You are transfixed by their gaze!", "-1");
            }
            
            else if (Utility.RandomDouble() <= .10)
            {
                Combatant = null;

                if (m_Creatures.Count == MaxCreatures)
                {
                    for (int a = 0; a < m_Creatures.Count; a++)
                    {
                        if (m_Creatures[a] == null) continue;
                        if (m_Creatures[a].Deleted) continue;

                        m_Creatures[a].Delete();
                        break;
                    }
                }

                if (Utility.RandomDouble() <= .66)
                {
                    UOACZGiantBat bat = new UOACZGiantBat();
                    bat.MoveToWorld(location, map);
                    m_Creatures.Add(bat);
                }

                else
                {
                    UOACZVampireBat bat = new UOACZVampireBat();
                    bat.MoveToWorld(location, map);
                    m_Creatures.Add(bat);
                }                    

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

                if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null))
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*transforms*");

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (Deleted || !Alive) return;

                    if (Hidden)
                        RevealingAction();

                    PlaySound(GetAngerSound());
                });
            }

            base.OnGotMeleeAttack(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .10)
            {
                PlaySound(GetAngerSound());
                SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 0.5, 30, -1, true, "", "", "*begins to strike with unnatural quickness*");
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
                    if (!m_Creatures[a].Deleted)
                        m_Creatures[a].Delete();
                }
            }
        }

        public override int GetAngerSound() { return 0x370; }
        public override int GetIdleSound() { return 0x57D; }
        public override int GetAttackSound() { return 0x374; }
        public override int GetHurtSound() { return 0x375; }
        public override int GetDeathSound() { return 0x376; }

        public UOACZVampireCountess(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
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
