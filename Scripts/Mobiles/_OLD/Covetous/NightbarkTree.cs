using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a nightbark tree corpse" )]
	public class NightbarkTree : BaseCreature
	{
		[Constructable]
		public NightbarkTree() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a nightbark tree";
			Body = 47;
			BaseSoundID = 442;

            Hue = Utility.RandomNeutralHue();

			SetStr( 75 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 500 );

			SetDamage( 12, 24 );

            SetSkill( SkillName.Wrestling, 80);
            SetSkill( SkillName.Tactics, 100);           

			SetSkill( SkillName.MagicResist, 25 );			

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 75;

			PackItem( new Log( 10 ) );
			PackItem( new MandrakeRoot( 5 ) );            
		}

        public override bool DisallowAllMoves { get { return true; } }

        public override int PoisonResistance { get { return 5; } }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
        }

        public override int AttackRange { get { return 3; }}

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.EntangleSpecialAbility(.15, this, defender, 1.0, 4.0, -1, false, "", "The creature entangles you with its branches!", "-1");
        }        

		public NightbarkTree( Serial serial ) : base( serial )
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
