using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class UOACZSkinner : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Skinner; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZSkinner() : base()
		{
            Title = "the skinner";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(350);

            SetDamage(9, 18);

            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            AddItem(new ThighBoots() { Movable = false });
            AddItem(new LeatherLegs() { Movable = false });
            AddItem(new LeatherChest() { Movable = false });
            AddItem(new LeatherGloves() { Movable = false });
            AddItem(new BodySash(Utility.RandomDyedHue()) { Movable = false });

            AddItem(new Kryss() { Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZSkinner(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
