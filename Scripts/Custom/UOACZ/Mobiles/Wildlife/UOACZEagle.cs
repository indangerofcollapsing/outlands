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
    [CorpseName("an eagle corpse")]
    public class UOACZEagle : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZEagle() : base()
		{
            Name = "an eagle";
            Body = 5;
            BaseSoundID = 0x2EE;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted eagle"; } }
        public override string CorruptedCorpseName { get { return "a corrupted eagle corpse"; } }

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
            {
                corpse.DropItem(new UOACZRawBird());

                if (Utility.RandomDouble() <= .33)
                    corpse.DropItem(new UOACZRawDrumstick());
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Feather(75));
        }

        public UOACZEagle(Serial serial): base(serial)
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
