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
    [CorpseName("a goat corpse")]
    public class UOACZGoat : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZGoat() : base()
		{
            Name = "a goat";
            Body = 0xD1;
            BaseSoundID = 0x99;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted goat"; } }
        public override string CorruptedCorpseName { get { return "a corrupted goat corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .10; } }

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

            corpse.DropItem(new Leather(3));
            corpse.DropItem(new UOACZWool(1));
        }
        
        public UOACZGoat(Serial serial): base(serial)
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
