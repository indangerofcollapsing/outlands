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
    [CorpseName("a skeletal knight corpse")]
    public class UOACZSkeletalKnight : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 4; } }

		[Constructable]
		public UOACZSkeletalKnight() : base()
		{
            Name = "a skeletal knight";
            Body = 57;
            Hue = 2610;
            BaseSoundID = 451;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 200;

            Fame = 7000;
            Karma = -7000;

            Tameable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZSkeletalKnight(Serial serial): base(serial)
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
