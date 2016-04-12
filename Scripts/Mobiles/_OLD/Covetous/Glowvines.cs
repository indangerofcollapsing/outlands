using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a glowvine corpse" )]
    public class Glowvines : BaseCreature
	{
        [Constructable]
		public Glowvines() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "glowvines";
			Body = 8;
			BaseSoundID = 684;
            Hue = 196;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(250);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;	

			Fame = 1000;
			Karma = -1000;
            
            CanSwim = true;            
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        
        protected override bool OnMove(Direction d)
        {            
            Blood blood = new Blood();
            blood.Hue = 196;
            blood.Name = "translucent slime";
            blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

            blood.MoveToWorld(Location, Map);            
            
            return base.OnMove(d);            
        }

        public Glowvines(Serial serial): base(serial)
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
