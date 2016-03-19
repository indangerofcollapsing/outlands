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
    public class UOACZCarpenter : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Carpenter; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZCarpenter() : base()
		{
            Title = "the carpenter";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(350);
            
            SetDamage(10, 20);

            SetSkill(SkillName.Swords, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            //Pants
            if (Female)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: AddItem(new Skirt(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 2: AddItem(new Kilt(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 3: AddItem(new ShortPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                }
            }

            else
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1: AddItem(new ShortPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 2: AddItem(new LongPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                }
            }

            AddItem(new HalfApron(Utility.RandomNeutralHue()) { Movable = false });
            AddItem(new Bandana(Utility.RandomNeutralHue()) { Movable = false });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: AddItem(new Axe() { Movable = false }); break;
                case 2: AddItem(new Hatchet() { Movable = false }); break;
                case 3: AddItem(new TwoHandedAxe() { Movable = false }); break;
                case 4: AddItem(new DoubleAxe() { Movable = false }); break;
            }
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZCarpenter(Serial serial): base(serial)
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
