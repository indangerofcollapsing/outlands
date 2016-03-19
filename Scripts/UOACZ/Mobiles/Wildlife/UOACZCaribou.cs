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
    [CorpseName("a caribou corpse")]
    public class UOACZCaribou : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZCaribou() : base()
		{
            Name = "a caribou";
            Body = 0xEA;
            Hue = 1843;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 300;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted caribou"; } }
        public override string CorruptedCorpseName { get { return "a corrupted caribou corpse"; } }

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
            {
                corpse.DropItem(new UOACZRawMeatShank());

                if (Utility.RandomDouble() <= .5)
                    corpse.DropItem(new UOACZRawCutsOfMeat());
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(4));
        }

        public override int GetAngerSound() { return 0x3F9; }
        public override int GetIdleSound() { return 0x3F8; }
        public override int GetAttackSound() { return 0x82; }
        public override int GetHurtSound() { return 0x83; }
        public override int GetDeathSound() { return 0x84; }

        public UOACZCaribou(Serial serial): base(serial)
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
