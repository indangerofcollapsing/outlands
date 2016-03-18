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
    public class UOACZNoble : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Noble; } }
        public override double StockpileContributionScalar { get { return .33; } }
        
        [Constructable]
		public UOACZNoble() : base()
		{
            Title = "the noble";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(400);
            
            SetDamage(8, 16);

            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            int colorTheme = Utility.RandomDyedHue();

            if (Female)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: AddItem(new FancyDress(colorTheme) { Movable = false }); break;
                    case 2:
                        AddItem(new FancyShirt(colorTheme) { Movable = false });
                        AddItem(new Skirt(colorTheme) { Movable = false });
                        break;
                    case 3:
                        AddItem(new Surcoat(colorTheme) { Movable = false });
                        AddItem(new Skirt(colorTheme) { Movable = false });
                    break;
                }

                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: AddItem(new FeatheredHat(colorTheme) { Movable = false }); break;
                    case 2: AddItem(new Bonnet(colorTheme) { Movable = false }); break;
                }

                AddItem(new LeatherGloves(colorTheme) { Movable = false });
                AddItem(new Cloak(colorTheme) { Movable = false });
                AddItem(new Boots() { Movable = false });
            }

            else
            {
                
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1: AddItem(new Surcoat(colorTheme) { Movable = false }); break;
                    case 2: AddItem(new FancyShirt(colorTheme) { Movable = false }); break;
                }

                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: AddItem(new FeatheredHat(colorTheme) { Movable = false }); break;
                }

                AddItem(new ShortPants(colorTheme) { Movable = false });
                AddItem(new ThighBoots() { Movable = false });
                AddItem(new Cloak(colorTheme) { Movable = false });
                AddItem(new LeatherGloves(colorTheme) { Movable = false });
            }

            AddItem(new Broadsword() { Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZNoble(Serial serial): base(serial)
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
