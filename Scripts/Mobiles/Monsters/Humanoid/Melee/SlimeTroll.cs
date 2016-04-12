using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a slime troll corpse" )]
	public class SlimeTroll : BaseCreature
	{
		[Constructable]
		public SlimeTroll () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a slime troll";
			Body = Utility.RandomList( 53, 54 );
			BaseSoundID = 461;
			Hue = 2002;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(600);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 40);

            VirtualArmor = 25;

			Fame = 7500;
			Karma = -7500;
		}

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override int PoisonResistance { get { return 3; } }

        public override bool CanRummageCorpses { get { return true; } }

        protected override bool OnMove(Direction d)
        {           
            Blood blood = new Blood();
            blood.Hue = 2002;
            blood.Name = "slime";
            blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

            blood.MoveToWorld(Location, Map);

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
        }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}				
       	
		public SlimeTroll( Serial serial ) : base( serial )
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
