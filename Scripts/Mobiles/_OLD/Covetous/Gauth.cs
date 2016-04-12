using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gauth corpse" )]
	public class Gauth : BaseCreature
	{
		[Constructable]
		public Gauth () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gauth";
			Body = 778;
			BaseSoundID = 377;

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 50 );

			SetHits( 75 );
            SetMana( 1000 );

			SetDamage( 4, 8 );

            SetSkill(SkillName.Wrestling, 50);
			SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

			Fame = 900;
			Karma = -900;

			VirtualArmor = 25;		
		}

        public override void SetUniqueAI()
        {   
            DictCombatSpell[CombatSpell.SpellDamage5] = 6;

            CombatSpecialActionMinDelay = 5;
            CombatSpecialActionMaxDelay = 10;
            
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.CauseWounds] = 1;
            
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override bool CanFly { get { return true; } }

        public Gauth(Serial serial): base(serial)
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
