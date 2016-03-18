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
    [CorpseName("a zombie magi corpse")]
    public class UOACZZombieMagi : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "brainnnnsssss....",
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

        public override int DifficultyValue { get { return 2; } }

		[Constructable]
		public UOACZZombieMagi() : base()
		{
            Name = "a zombie magi";
            Body = 3;
            BaseSoundID = 471;
            Hue = 2585;

            SetStr(25);
            SetDex(25);
            SetInt(50);

            SetHits(175);
            SetMana(1000);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 45);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;

            Tamable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            SetSubGroup(AISubgroup.MeleeMage1);
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
            
            DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 5;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;

            ResolveAcquireTargetDelay = 3;
        }

        public UOACZZombieMagi(Serial serial): base(serial)
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
