using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an air elemental corpse" )]
	public class AirElemental : BaseCreature
	{
		[Constructable]
		public AirElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an air elemental";
			Body = 13;
			Hue = 0x4001;
			BaseSoundID = 655;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(200);
            SetMana(1000);

            SetDamage(7, 14);            

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;			

			ControlSlots = 2;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath( Container c )
		{			
            base.OnDeath( c );
		}
        
		public AirElemental( Serial serial ) : base( serial )
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
