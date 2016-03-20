using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName("a pestilent bandage corpse")]
	public class PestilentBandage : BaseCreature
	{		
        [Constructable]
		public PestilentBandage() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a pestilent bandage";
			Body = 154;
			Hue = 2002; 
			BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(750);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);            

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 20000;
			Karma = -20000;			
        }

		public override Poison HitPoison{ get{ return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
		
		public PestilentBandage( Serial serial ) : base( serial )
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
