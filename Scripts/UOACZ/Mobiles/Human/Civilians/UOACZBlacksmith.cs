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
    public class UOACZBlacksmith : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Blacksmith; } }
        public override double StockpileContributionScalar { get { return 1; } }
        
        [Constructable]
		public UOACZBlacksmith() : base()
		{
            Title = "the blacksmith";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(400);
            
            SetDamage(9, 16);

            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 50);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            CreateRandomOutfit();

            AddItem(new WarMace() { Movable = false });
            AddItem(new MetalShield() { Movable = false });

            AddItem(new PlateArms() { Movable = false });
            AddItem(new PlateGloves() { Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZBlacksmith(Serial serial): base(serial)
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
