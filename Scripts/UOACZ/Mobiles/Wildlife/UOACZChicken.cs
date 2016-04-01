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
    [CorpseName("a chicken corpse")]
    public class UOACZChicken : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZChicken() : base()
		{
            Name = "a chicken";
            Body = 0xD0;
            BaseSoundID = 0x6E;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted chicken"; } }
        public override string CorruptedCorpseName { get { return "a corrupted chicken corpse"; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void UOACZCarve(Mobile from, Corpse corpse)
        {
            base.UOACZCarve(from, corpse);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawDrumstick());

            else
                corpse.DropItem(new UOACZRawBird());

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Feather(50));
        }

        public override int GetDeathSound() { return 0x072; }

        public UOACZChicken(Serial serial): base(serial)
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
