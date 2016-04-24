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
    public class UOACZButcher : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Butcher; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZButcher() : base()
		{
            Title = "the butcher";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(350);
            
            SetDamage(9, 18);

            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

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

            //Shoes            
            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new Shoes(Utility.RandomNeutralHue()) { Movable = false }); break;
                case 4: AddItem(new Sandals(Utility.RandomNeutralHue()) { Movable = false }); break;
            }

            AddItem(new FullApron(Utility.RandomNeutralHue()) { Hue = 2118, Movable = false });
            AddItem(new LeatherGloves() { Hue = 2118, Movable = false });
            AddItem(new Cleaver() { Hue = 2116, Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.20, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!", "-1");
        }

        public UOACZButcher(Serial serial): base(serial)
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
