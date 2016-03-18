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
    [CorpseName("a skeletal drake corpse")]
    public class UOACZSkeletalDrake : UOACZBaseUndead
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

		[Constructable]
		public UOACZSkeletalDrake() : base()
		{
            Name = "a skeletal drake";
            Body = 104;
            BaseSoundID = 0x488;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(600);

            SetDamage(11, 22);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 5500;
            Karma = -5500;                   
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MassiveBoneBreathAttack] = 25;

            MassiveBreathRange = 3;
        }

        public UOACZSkeletalDrake(Serial serial): base(serial)
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
