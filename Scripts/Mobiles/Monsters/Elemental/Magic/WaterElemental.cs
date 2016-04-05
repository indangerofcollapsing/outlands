using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;


namespace Server.Mobiles
{
	[CorpseName( "a water elemental corpse" )]
	public class WaterElemental : BaseCreature
	{
		[Constructable]
		public WaterElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a water elemental";
			Body = 16;
			BaseSoundID = 278;

			SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(200);
            SetMana(1000);

            SetDamage(7, 14);            

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;

            CanSwim = true;
			
			ControlSlots = 2;
		}

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}

		public WaterElemental( Serial serial ) : base( serial )
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
