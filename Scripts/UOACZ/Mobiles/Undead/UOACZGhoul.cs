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
    [CorpseName("a ghoulish corpse")]
    public class UOACZGhoul : UOACZBaseUndead
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
		public UOACZGhoul() : base()
		{
            Name = "a ghoul";
            Body = 153;
            BaseSoundID = 0x482;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(100);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 45);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;                    
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();            
        }

        public UOACZGhoul(Serial serial): base(serial)
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
