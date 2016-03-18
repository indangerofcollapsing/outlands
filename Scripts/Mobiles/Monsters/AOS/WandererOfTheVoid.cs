using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a wanderer of the void corpse" )]
	public class WandererOfTheVoid : BaseCreature
	{
		[Constructable]
		public WandererOfTheVoid() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a wanderer of the void";
			Body = 316;
			BaseSoundID = 377;
            Hue = 2587;
            
            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            CanSwim = true;

			Fame = 10000;
			Karma = -10000;

			int count = Utility.RandomMinMax( 2, 3 );

			for ( int i = 0; i < count; ++i )
				PackItem( new TreasureMap( 3, Map.Trammel ) );
		}

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.05;
            
            DictCombatSpell[CombatSpell.SpellDamage5] = 8;

            CombatSpecialActionMinDelay = 5;
            CombatSpecialActionMaxDelay = 10;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.CauseWounds] = 1;
        }

		public WandererOfTheVoid( Serial serial ) : base( serial )
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