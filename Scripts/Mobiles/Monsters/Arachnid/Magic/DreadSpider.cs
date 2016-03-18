using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dread spider corpse" )]
	public class DreadSpider : BaseCreature
	{
		[Constructable]
		public DreadSpider () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a dread spider";
			Body = 11;
			BaseSoundID = 1170;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(250);
            SetMana(1000);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;

            PackItem(new SpidersSilk(15));
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }   
    
		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

    		switch( Utility.Random( 1000 ) )
    		{
         		case 1: { c.AddItem( new Diamond( ) ); } break;
         		case 2: { c.AddItem( new Ruby( ) ); } break;
         		case 3: { c.AddItem( new HealPotion( ) ); } break;
         		case 4: { c.AddItem( new HealScroll( ) ); } break;         			
    		}
		}

		public DreadSpider( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 1170;
		}
	}
}
