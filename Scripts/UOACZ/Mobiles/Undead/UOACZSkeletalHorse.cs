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
    [CorpseName("a skeletal horse corpse")]
    public class UOACZSkeletalHorse : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 2; } }

		[Constructable]
		public UOACZSkeletalHorse() : base()
		{
            Name = "a skeletal horse";
            Body = 793;
            BaseSoundID = 0;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = -2000;                   
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ActiveSpeed = 0.3;
            PassiveSpeed = 0.4;
        }

        public UOACZSkeletalHorse(Serial serial): base(serial)
		{
		}

        public override int GetAngerSound() { return 0x4BE; }
        public override int GetIdleSound() { return 0x4BD; }
        public override int GetAttackSound() { return 0x4F6; }
        public override int GetHurtSound() { return 0x4BF; }
        public override int GetDeathSound() { return 0x4C0; }

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
