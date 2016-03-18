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
    [CorpseName("a deer corpse")]
    public class UOACZDeer : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZDeer() : base()
		{
            Name = "a deer";
            Body = 0xED;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 35);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted deer"; } }
        public override string CorruptedCorpseName { get { return "a corrupted deer corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .15; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }
        
        public override void UOACZCarve(Mobile from, Corpse corpse, Item with)
        {
            base.UOACZCarve(from, corpse, with);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawMeatShank());

            else            
                corpse.DropItem(new UOACZRawMeatShank());   

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(4));
        }

        public override int GetAngerSound() { return 0x3F9; }
        public override int GetIdleSound() { return 0x3F8; }
        public override int GetAttackSound() { return 0x3FA; }
        public override int GetHurtSound() { return 0x083; }
        public override int GetDeathSound() { return 0x084; }

        public UOACZDeer(Serial serial): base(serial)
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
