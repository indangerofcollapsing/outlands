using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Bogle : BaseCreature
	{
        [Constructable]
		public Bogle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bogle";
			Body = 153;
			BaseSoundID = 0x482;
            Hue = 2523;

            SetStr(50);
            SetDex(50);
            SetInt(50);

            SetHits(100);
            SetMana(1000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;		
		}			

		public Bogle( Serial serial ) : base( serial )
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
