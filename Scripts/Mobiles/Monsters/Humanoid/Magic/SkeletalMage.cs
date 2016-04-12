using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class SkeletalMage : BaseCreature
	{
		[Constructable]
		public SkeletalMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a skeletal mage";
			Body = 50;
			Hue = 2117;
			BaseSoundID = 451;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(250);
            SetMana(1000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;
		}

        public override int PoisonResistance { get { return 5; } }				

	    public SkeletalMage( Serial serial ) : base( serial )
	    {
	    }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
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
