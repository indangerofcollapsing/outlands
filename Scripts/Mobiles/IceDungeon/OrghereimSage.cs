using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an orghereim sage corpse" )] 
	public class OrghereimSage : BaseOrghereim 
	{        
        [Constructable] 
		public OrghereimSage() : base() 
		{ 
			Name = "an orghereim sage";

			SetStr( 50 );
			SetDex( 25 );
			SetInt( 100 );

			SetHits( 350 );
            SetMana( 1000 );

			SetDamage( 4, 8 );            

            SetSkill( SkillName.Wrestling, 80);
            SetSkill( SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
			SetSkill( SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

			SetSkill( SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new FeatheredHat() { Movable = false, Hue = itemHue });
            AddItem(new FancyShirt() { Movable = false, Hue = itemHue });
            AddItem(new Cloak() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });
            AddItem(new LongPants() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new Spellbook() { Movable = false, Hue = weaponHue });            
		}

        public override void SetUniqueAI()
        {
            DictCombatTargeting[CombatTargeting.Predator] = 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 6; } }

		public OrghereimSage( Serial serial ) : base( serial )
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