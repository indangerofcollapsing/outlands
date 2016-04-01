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
    [CorpseName("a boar corpse")]
    public class UOACZBoar : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZBoar() : base()
		{
            Name = "a boar";
            Body = 0x122;
            BaseSoundID = 0xC4;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            Fame = 300;
            Karma = 0;
		}

        public override bool AlwaysFlee { get { return false; } }

        public override string CorruptedName { get { return "a corrupted boar"; } }
        public override string CorruptedCorpseName { get { return "a corrupted boar corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .15; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void TurnCorrupted(Mobile from)
        {
            base.TurnCorrupted(from);            
        }

        public override void UOACZCarve(Mobile from, Corpse corpse)
        {
            base.UOACZCarve(from, corpse);

            if (Corrupted)
                corpse.DropItem(new UOACZCorruptedRawHam());

            else
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: corpse.DropItem(new UOACZRawHam()); break;
                    case 2: corpse.DropItem(new UOACZRawSausage()); break;
                    case 3: corpse.DropItem(new UOACZRawCutsOfMeat()); break;
                }

                if (Utility.RandomDouble() <= .5)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: corpse.DropItem(new UOACZRawHam()); break;
                        case 2: corpse.DropItem(new UOACZRawSausage()); break;
                        case 3: corpse.DropItem(new UOACZRawCutsOfMeat()); break;
                    }
                }
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(3));
        }

        public UOACZBoar(Serial serial): base(serial)
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
