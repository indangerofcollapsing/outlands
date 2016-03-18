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
    public class UOACZAlchemist : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Alchemist; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZAlchemist() : base()
		{
            Title = "the alchemist";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            AddItem(new Dagger() { Movable = false });
            AddItem(new Robe(Utility.RandomPinkHue()) { Movable = false });
            AddItem(new Sandals() { Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictCombatAction[CombatAction.AttackOnly] = 10;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;
        }

        public UOACZAlchemist(Serial serial): base(serial)
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
