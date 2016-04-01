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
    [CorpseName("a rat corpse")]
    public class UOACZRat : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZRat() : base()
		{
            Name = "a rat";
            Body = 238;
            BaseSoundID = 0xCC;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 10);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = -150;
		}

        public override string CorruptedName { get { return "a corrupted rat"; } }
        public override string CorruptedCorpseName { get { return "a corrupted rat corpse"; } }

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
        }
        
        public UOACZRat(Serial serial): base(serial)
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
