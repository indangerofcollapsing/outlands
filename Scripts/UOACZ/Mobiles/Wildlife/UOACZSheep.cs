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
    [CorpseName("a sheep corpse")]
    public class UOACZSheep : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZSheep() : base()
		{
            Name = "a sheep";
            Body = 0xCF;
            BaseSoundID = 0xD6;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted sheep"; } }
        public override string CorruptedCorpseName { get { return "a corrupted sheep corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .10; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }
        
        public override void UOACZCarve(Mobile from, Corpse corpse)
        {
            base.UOACZCarve(from, corpse);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawMeatScraps());

            else
                corpse.DropItem(new UOACZRawMeatScraps());

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(3));
            corpse.DropItem(new UOACZWool(1));
        }
        
        public UOACZSheep(Serial serial): base(serial)
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
