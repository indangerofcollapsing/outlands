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
    [CorpseName("a skeletal mage corpse")]
    public class UOACZSkeletalMage : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 3; } }

		[Constructable]
		public UOACZSkeletalMage() : base()
		{
            Name = "a skeletal mage";
            Body = 50;
            Hue = 2117;
            BaseSoundID = 451;

            SetStr(50);
            SetDex(25);
            SetInt(75);

            SetHits(150);
            SetMana(1000);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 150;

            Fame = 3000;
            Karma = -3000;

            Tameable = true;
            ControlSlots = 2;
		}

        public override void SetUniqueAI()
        {
            SetSubGroup(AISubgroup.Mage1);
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

        public UOACZSkeletalMage(Serial serial): base(serial)
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
