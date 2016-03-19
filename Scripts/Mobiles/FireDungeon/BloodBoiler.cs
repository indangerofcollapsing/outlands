using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a burning bat corpse" )]
	public class BloodBoiler : BaseCreature
	{
		[Constructable]
		public BloodBoiler() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood boiler";
			Body = 317;
			BaseSoundID = 0x3E9;
			Hue = 2117;

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits(200);
            SetMana(1000);

			SetDamage( 15, 30 );

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);			
			SetSkill(SkillName.Meditation, 100);
            
            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;
		}

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;
        }
		
		public override bool CanRummageCorpses{ get{ return true; } }		

		public BloodBoiler( Serial serial ) : base( serial )
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
