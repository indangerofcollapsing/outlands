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
    [CorpseName("a skeletal corpse")]
    public class UOACZSkeleton : UOACZBaseUndead
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
		public UOACZSkeleton() : base()
		{
            Name = "a skeleton";
            Body = Utility.RandomList(50, 56);
            BaseSoundID = 0x48D;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 45);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

            Tameable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }        

        public UOACZSkeleton(Serial serial): base(serial)
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
