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
    [CorpseName("a shadow dragon corpse")]
    public class UOACZShadowDragon : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 9; } }

		[Constructable]
		public UOACZShadowDragon() : base()
		{
            Name = "a shadow dragon";
            Body = 106;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(800);
            SetMana(2000);

            SetDamage(13, 26);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            SetSkill(SkillName.Poisoning, 25);

            Fame = 22500;
            Karma = -22500;                  
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
            DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1;
        }

        public override Poison HitPoison { get { return Poison.Regular; } }

        public override int GetIdleSound() { return 0x2D5; }
        public override int GetHurtSound() { return 0x2D1; }

        public UOACZShadowDragon(Serial serial): base(serial)
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
