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
    [CorpseName("a bird corpse")]
    public class UOACZBird : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZBird() : base()
		{
            if (Utility.RandomBool())
            {
                Hue = 0x901;

                switch (Utility.Random(3))
                {
                    case 0: Name = "a crow"; break;
                    case 2: Name = "a raven"; break;
                    case 1: Name = "a magpie"; break;
                }
            }

            else
            {
                Hue = Utility.RandomBirdHue();
                Name = NameList.RandomName("bird");
            }

            Body = 6;
            BaseSoundID = 0x1B;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(10);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted bird"; } }
        public override string CorruptedCorpseName { get { return "a corrupted bird corpse"; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void UOACZCarve(Mobile from, Corpse corpse, Item with)
        {
            base.UOACZCarve(from, corpse, with);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawDrumstick());

            else
                corpse.DropItem(new UOACZRawDrumstick());

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Feather(50));
        }

        public UOACZBird(Serial serial): base(serial)
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
