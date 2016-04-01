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
    [CorpseName("a llama corpse")]
    public class UOACZLlama : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZLlama() : base()
		{
            Name = "a llama";
            Body = 0xDC;
            BaseSoundID = 0x3F3;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted llama"; } }
        public override string CorruptedCorpseName { get { return "a corrupted bison llama"; } }

        public override double CrudeBoneArmorDropChance { get { return .15; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }
        
        public override void UOACZCarve(Mobile from, Corpse corpse)
        {
            base.UOACZCarve(from, corpse);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawMeatShank());

            else
            {
                corpse.DropItem(new UOACZRawMeatShank());

                if (Utility.RandomDouble() <= .5)
                    corpse.DropItem(new UOACZRawCutsOfMeat());
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(4));
            corpse.DropItem(new UOACZWool(1));            
        }
        
        public UOACZLlama(Serial serial): base(serial)
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
