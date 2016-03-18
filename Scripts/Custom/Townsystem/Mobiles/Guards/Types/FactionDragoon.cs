using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
	public class FactionDragoon : BaseFactionGuard
	{
		[Constructable]
		public FactionDragoon() : base( "the dragoon" )
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

			AddItem( Immovable( Rehued( new Cloak(), 1645 ) ) );

			AddItem( Immovable( Rehued( new PlateChest(), 1645 ) ) );
			AddItem( Immovable( Rehued( new PlateLegs(), 1109 ) ) );
			AddItem( Immovable( Rehued( new PlateArms(), 1109 ) ) );
			AddItem( Immovable( Rehued( new PlateGloves(), 1109 ) ) );
			AddItem( Immovable( Rehued( new PlateGorget(), 1109 ) ) );
			AddItem( Immovable( Rehued( new PlateHelm(), 1109 ) ) );

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new Halberd() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 2: { AddItem(new Bardiche() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 3: { AddItem(new Spear() { Movable = false, Hue = 0, Layer = Layer.FirstValid }); AddItem(new OrderShield() { Movable = false, Hue = 0 }); break; }
                case 4: { AddItem(new WarHammer() { Movable = false, Hue = 0 }); break; }
            }

			AddItem( Immovable( Rehued( new VirtualMountItem( this ), 1109 ) ) );

			PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
			PackStrongPotions( 6, 12 );
		}

		public FactionDragoon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}