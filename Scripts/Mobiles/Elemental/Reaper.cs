using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a reapers corpse" )]
	public class Reaper : BaseCreature
	{
		[Constructable]
		public Reaper() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a reaper";
			Body = 47;
			BaseSoundID = 442;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(400);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 50;

			Fame = 3500;
			Karma = -3500;      
		}

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Elemental; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.SuperSlow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Mage3; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }
        
        public override int PoisonResistance { get { return 3; } }

        public override bool DisallowAllMoves { get { return true; } }

        public override int AttackRange { get { return 2; } }        

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }
        		
		public Reaper( Serial serial ) : base( serial )
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
