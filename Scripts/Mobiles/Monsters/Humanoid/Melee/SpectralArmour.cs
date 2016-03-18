using System; 
using Server.Items; 

namespace Server.Mobiles 
{     
	public class SpectralArmour : BaseCreature 
	{ 
		public override bool DeleteCorpseOnDeath{ get{ return true; } }

		[Constructable] 
		public SpectralArmour() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			Body = 0x190;
            Hue = -1;
			Name = "spectral armour";

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Swords, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);
            
            VirtualArmor = 50;

			Fame = 7000; 
			Karma = -7000;

            AddItem(new PlateChest() { Movable = false, Hue = 0 });
            AddItem(new PlateGloves() { Movable = false, Hue = 0 });

            AddItem(new Halberd() { Movable = false, Hue = 0 });
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AlwaysMurderer { get { return true; } }

        public override void DisplayPaperdollTo(Mobile to)
        {
        }

		public override int GetIdleSound()
		{
			return 0x200;
		}

		public override int GetAngerSound()
		{
			return 0x56;
		}

		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			Gold gold = new Gold( ModifiedGoldWorth() );
			gold.MoveToWorld( Location, Map );

			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
			return true;
		}

		public SpectralArmour( Serial serial ) : base( serial ) 
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