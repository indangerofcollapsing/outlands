using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a deep sea serpents corpse" )]
	public class DeepSeaSerpent : BaseCreature
	{
		[Constructable]
		public DeepSeaSerpent() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a deep sea serpent";
			Body = 150;
			BaseSoundID = 447;
            
			Hue = Utility.Random( 0x8A0, 5 );

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(400);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 6000;
			Karma = -6000;

			CanSwim = true;
			CantWalk = true;

			if ( Utility.RandomBool() )
			    PackItem( new SulfurousAsh( 10 ) );
			else
			    PackItem( new BlackPearl( 10 ) ); 

            PackItem(new RawFishSteak(6));
		}

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool IsOceanCreature { get { return true; } }        

        public override int Hides { get { return 15; } }
        public override HideType HideType { get { return HideType.Barbed; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void OnDeath(Container c)
        {            
            base.OnDeath(c);

            switch (Utility.Random(400))
            {
                case 0: c.AddItem(new SeaScaleChest()); break;
                case 1: c.AddItem(new SeaScaleCoif()); break;
                case 2: c.AddItem(new SeaScaleLegs()); break;
            }
        }

		public override bool OnBeforeDeath()
		{
			if ( 0.02 >= Utility.RandomDouble() )
				PackItem( new SpecialFishingNet() );

			return base.OnBeforeDeath();
		}
	
		public override int Meat{ get{ return 1; } }		

		public DeepSeaSerpent( Serial serial ) : base( serial )
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
	}
}
