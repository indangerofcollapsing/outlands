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
    [CorpseName("a giant rat corpse")]
    public class UOACZGiantRat : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZGiantRat() : base()
		{
            Name = "a giant rat";
            Body = 0xD7;
            BaseSoundID = 0x188;

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
            Karma = -300;
		}

        public override bool AlwaysFlee { get { return false; } }

        public override string CorruptedName { get { return "a corrupted giant rat"; } }
        public override string CorruptedCorpseName { get { return "a corrupted giant rat"; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void UOACZCarve(Mobile from, Corpse corpse, Item with)
        {
            base.UOACZCarve(from, corpse, with);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawMeatScraps());

            else
                corpse.DropItem(new UOACZRawMeatScraps());

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(2));
        }
        
        public UOACZGiantRat(Serial serial): base(serial)
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
