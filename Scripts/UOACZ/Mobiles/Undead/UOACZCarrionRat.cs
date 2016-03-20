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
    [CorpseName("a carrion rat corpse")]
    public class UOACZCarrionRat : UOACZBaseUndead
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
		public UOACZCarrionRat() : base()
		{
            Name = "a carrion rat";

            Body = 0xD7;
            BaseSoundID = 0x188;
            Hue = 2076;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZCarrionRat(Serial serial): base(serial)
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
