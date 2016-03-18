using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class Slime2 : BaseCreature
	{
		[Constructable]
		public Slime2() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a slime";
			Body = 51;
			BaseSoundID = 456;

			Hue = Utility.RandomSlimeHue();

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(50);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 10);

            VirtualArmor = 25;

			Fame = 300;
			Karma = -300;			
		}

        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (attacker != this && this.Hits > 1)
            {                
                Mobile spawn = new Slime();
                spawn.MoveToWorld(this.Location, this.Map);
                this.Hits /= 2;
                spawn.Hits = this.Hits;                

                Say("The slime divides!");
            }

            base.OnGotMeleeAttack(attacker);
        }

		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.FruitsAndVegies | FoodType.GrainsAndHay | FoodType.Eggs; } }

		public Slime2( Serial serial ) : base( serial )
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
