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
    [CorpseName("a nightmare corpse")]
    public class UOACZNightmare : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Small; } }

        public override int DifficultyValue { get { return 5; } }

		[Constructable]
		public UOACZNightmare() : base()
		{
            Name = "Nightmare";

            switch (Utility.Random(3))
            {
                case 0: { BodyValue = 116; break; }
                case 1: { BodyValue = 178; break; }
                case 2: { BodyValue = 179; break; }
            }

            SetStr(50);
            SetDex(50);
            SetInt(50);

            SetHits(250);
            SetMana(1000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 4000;
            Karma = -4000;                
		}

        public override void SetUniqueAI()
        {
            AISubGroup = AISubGroupType.MeleeMage1;
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

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;

            ActiveSpeed = 0.3;
            PassiveSpeed = 0.4;
        }

        public override int GetAngerSound() { return 0x4BE; }
        public override int GetIdleSound() { return 0x4BD; }
        public override int GetAttackSound() { return 0x4F6; }
        public override int GetHurtSound() { return 0x4BF; }
        public override int GetDeathSound() { return 0x4C0; }

        public UOACZNightmare(Serial serial): base(serial)
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
