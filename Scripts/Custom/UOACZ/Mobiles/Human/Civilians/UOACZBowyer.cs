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
    public class UOACZBowyer : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Bowyer; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZBowyer() : base()
		{
            Title = "the bowyer";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(350);
            
            SetDamage(8, 16);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            CreateRandomOutfit();

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: AddItem(new Bow() { Movable = false }); break;
                case 2: AddItem(new Bow() { Movable = false }); break;
                case 3: AddItem(new Crossbow() { Movable = false }); break;
                case 4: AddItem(new HeavyCrossbow() { Movable = false }); break;
            }
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictCombatRange[CombatRange.WeaponAttackRange] = 10;
            DictCombatRange[CombatRange.Withdraw] = 1;
        }

        public UOACZBowyer(Serial serial): base(serial)
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
