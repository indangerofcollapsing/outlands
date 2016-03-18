using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
	public class FactionPaladin : BaseFactionGuard
	{
		[Constructable]
		public FactionPaladin() : base( "the paladin" )
        {
            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(700);

            SetDamage(20, 30);

            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 75;

            GenerateBody(false, false);

			AddItem( Immovable( Rehued( new PlateChest(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateLegs(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateHelm(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateGorget(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateArms(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateGloves(), 2125 ) ) );

			AddItem( Immovable( Rehued( new BodySash(), 1254 ) ) );
			AddItem( Immovable( Rehued( new Cloak(), 1254 ) ) );

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new Halberd() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 2: { AddItem(new Bardiche() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 3: { AddItem(new Spear() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 4: { AddItem(new WarHammer() { Movable = false, Hue = 0 }); break; }
            }

			AddItem( Immovable( Rehued( new VirtualMountItem( this ), 1254 ) ) );

			PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
			PackStrongPotions( 6, 12 );
		}

		public FactionPaladin( Serial serial ) : base( serial )
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