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
    [CorpseName("a skeletal critter corpse")]
    public class UOACZSkeletalCritter : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 1; } }

		[Constructable]
		public UOACZSkeletalCritter() : base()
		{
            Name = "a skeletal critter";
            Body = 302;
            BaseSoundID = 959;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 45);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;                    
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();            

            ActiveSpeed = 0.3;
            PassiveSpeed = 0.4;
        }

        public override int GetAngerSound() { return 0x625; }
        public override int GetIdleSound() { return 0x624; }
        public override int GetAttackSound() { return 0x636; }
        public override int GetHurtSound() { return 0x637; }
        public override int GetDeathSound() { return 0x26F; } //0x3A1

        public UOACZSkeletalCritter(Serial serial): base(serial)
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
