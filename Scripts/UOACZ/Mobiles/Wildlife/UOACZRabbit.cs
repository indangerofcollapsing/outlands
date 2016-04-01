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
    [CorpseName("a rabbit corpse")]
    public class UOACZRabbit : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZRabbit() : base()
		{
            Name = "a rabbit";
            Body = 205;

            if (0.5 >= Utility.RandomDouble())
                Hue = Utility.RandomAnimalHue();

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 10);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = 0;
		}

        public override string CorruptedName { get { return "a corrupted rabbit"; } }
        public override string CorruptedCorpseName { get { return "a corrupted rabbit corpse"; } }

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

        public override int GetAttackSound() { return 0xC9; }
        public override int GetHurtSound() { return 0xCA; }
        public override int GetDeathSound() { return 0xCB; }

        public UOACZRabbit(Serial serial): base(serial)
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
