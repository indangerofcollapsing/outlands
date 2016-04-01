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
    [CorpseName("a cow corpse")]
    public class UOACZCow : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZCow() : base()
		{
            Name = "a cow";
            Body = Utility.RandomList(0xD8, 0xE7);
            BaseSoundID = 0x78;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;

            VirtualArmor = 10;
		}

        public override string CorruptedName { get { return "a corrupted cow"; } }
        public override string CorruptedCorpseName { get { return "a corrupted cow corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .20; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void UOACZCarve(Mobile from, Corpse corpse)
        {
            base.UOACZCarve(from, corpse);

            if (Corrupted)
            {
                corpse.DropItem(new UOACZCorruptedRawSteak());
            }

            else
            {
                corpse.DropItem(new UOACZRawSteak());
                corpse.DropItem(new UOACZCheeseWheel());                

                if (Utility.RandomDouble() <= .5)
                    corpse.DropItem(new UOACZRawCutsOfMeat());
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(5));
        }

        public UOACZCow(Serial serial): base(serial)
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
