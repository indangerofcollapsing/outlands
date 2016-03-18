using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    [CorpseName("a rotting corpse")]
    public class UOACZRagWitch : UOACZBaseUndead
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

        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(10);

		[Constructable]
		public UOACZRagWitch() : base()
		{
            Name = "a rag witch";
            Body = 740;
            Hue = 2500;
            BaseSoundID = 0x482;

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(350);
            SetMana(3000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;                      
		}

        public override void SetUniqueAI()
        {
            SetSubGroup(AISubgroup.Mage3);
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
            
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null))
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }
        }

        public override int AttackAnimation { get { return 5; } }
        public override int AttackFrames { get { return 8; } }

        public override int GetAngerSound() { return 0x284; }
        public override int GetIdleSound() { return 0x285; }
        public override int GetAttackSound() { return 0x286; }
        public override int GetHurtSound() { return 0x287; }
        public override int GetDeathSound() { return 0x288; }

        public UOACZRagWitch(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
