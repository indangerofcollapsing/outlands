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
    [CorpseName("a vampire thrall corpse")]
    public class UOACZVampireThrall : UOACZBaseUndead
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

        public override int AttackAnimation { get { return 6; } }
        public override int AttackFrames { get { return 8; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }

		[Constructable]
		public UOACZVampireThrall() : base()
		{
            Name = "a vampire thrall";
            Body = 722;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(75);
            SetInt(50);

            SetHits(300);
            SetMana(1000);

            SetDamage(9, 18);
            
            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);    

            VirtualArmor = 25;

            Fame = 4000;
            Karma = -4000;                   
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
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .2)
            {
                PlaySound(GetAngerSound());
                SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 0.5, 20, -1, true, "", "", "*begins to strike with unnatural quickness*");
            }
        }

        public override int GetAngerSound() { return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481); }
        public override int GetIdleSound() { return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481); }
        public override int GetAttackSound() { return 372 + 2; }
        public override int GetHurtSound() { return 0x5F9; }
        public override int GetDeathSound() { return 0x5F5; }

        public UOACZVampireThrall(Serial serial): base(serial)
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
