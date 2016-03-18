using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Spells.Fourth;

namespace Server.Mobiles
{
	[CorpseName( "a lava surger corpse" )]
	public class LavaSurger : BaseCreature
	{
		[Constructable]
		public LavaSurger () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lava surger";
			Body = 16;
			BaseSoundID = 362;
			Hue = 2117;

			SetStr( 100 );
			SetDex( 75 );
			SetInt( 100 );

			SetHits(1500);
            SetMana(3000);

			SetDamage( 20, 40 );

            SetSkill(SkillName.Wrestling, 120);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

			SetSkill(SkillName.EvalInt, 75);
			SetSkill(SkillName.Magery, 75);
			SetSkill(SkillName.Meditation, 100);

			Fame = 35500;
			Karma = -32500;

			VirtualArmor = 25;
		}

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;
            
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;

		}

		public LavaSurger( Serial serial ) : base( serial )
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
