using Server;
using System;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	[CorpseName( "a cu sidhe corpse" )]
	public class CuSidhe : BaseMount
	{
		[Constructable]
		public CuSidhe() : this( "a cu sidhe" )
		{
		}

		[Constructable]
		public CuSidhe( string name ) : base( name, 277, 0x3E91, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{			
			double chance = Utility.RandomDouble() * 23301;

			if ( chance <= 1 )
				Hue = 0x489;
			else if ( chance < 50 )
				Hue = Utility.RandomList( 0x657, 0x515, 0x4B1, 0x481, 0x482, 0x455 );
			else if ( chance < 500 )
				Hue = Utility.RandomList( 0x97A, 0x978, 0x901, 0x8AC, 0x5A7, 0x527 );

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(1500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 5000;
            Karma = 5000;	
		}	

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Race != Race.Elf && from == ControlMaster && from.AccessLevel == AccessLevel.Player )
			{
				Item pads = from.FindItemOnLayer( Layer.Shoes );				
			}
			
			base.OnDoubleClick( from );
		}
		
		public CuSidhe( Serial serial ) : base( serial )
		{
		}

		public override int GetIdleSound() { return 0x577; }
		public override int GetAttackSound() { return 0x576; }
		public override int GetAngerSound() { return 0x578; }
		public override int GetHurtSound() { return 0x576; }
		public override int GetDeathSound()	{ return 0x579; }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Name == "a Cu Sidhe" )
				Name = "a cu sidhe";
		}
	}
}
