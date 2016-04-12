using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a wailing banshee corpse" )]
	public class WailingBanshee : BaseCreature
	{
		[Constructable]
		public WailingBanshee() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a wailing banshee";
			Body = 310;
			BaseSoundID = 0x482;

            Hue = 1775;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(125);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 50);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 1500;
			Karma = -1500;
		}

        public override int PoisonResistance { get { return 5; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
            
            CombatSpecialActionMinDelay = 5;
            CombatSpecialActionMaxDelay = 10;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.CauseWounds] = 1;
        }

		public WailingBanshee( Serial serial ) : base( serial )
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