using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
	public class FactionKnight : BaseFactionGuard
	{
		[Constructable]
		public FactionKnight() : base( "the knight" )
		{
            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(700);

            SetDamage(20, 30);

            SetSkill(SkillName.Fencing, 95);
            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

			GenerateBody( false, false );

			AddItem( Immovable( Rehued( new ChainChest(), 2125 ) ) );
			AddItem( Immovable( Rehued( new ChainLegs(), 2125 ) ) );
			AddItem( Immovable( Rehued( new ChainCoif(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateArms(), 2125 ) ) );
			AddItem( Immovable( Rehued( new PlateGloves(), 2125 ) ) );

			AddItem( Immovable( Rehued( new BodySash(), 1254 ) ) );
			AddItem( Immovable( Rehued( new Kilt(), 1254 ) ) );
			AddItem( Immovable( Rehued( new Sandals(), 1254 ) ) );

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new VikingSword() { Movable = false, Hue = 0 }); AddItem(new MetalKiteShield() { Movable = false, Hue = 0 }); break; }
                case 2: { AddItem(new Broadsword() { Movable = false, Hue = 0 }); AddItem(new MetalKiteShield() { Movable = false, Hue = 0 }); break; }
                case 3: { AddItem(new WarMace() { Movable = false, Hue = 0 }); AddItem(new MetalKiteShield() { Movable = false, Hue = 0 }); break; }
                case 4: { AddItem(new Mace() { Movable = false, Hue = 0 }); AddItem(new MetalKiteShield() { Movable = false, Hue = 0 }); break; }
            }

			PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
			PackStrongPotions( 6, 12 );
		}

		public FactionKnight( Serial serial ) : base( serial )
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