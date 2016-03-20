using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;

namespace Server.Mobiles
{
	[CorpseName( "an elder ice elemental corpse" )]
	public class ElderIceElemental : BaseCreature
	{
		[Constructable]
		public ElderIceElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder ice elemental";
			Body = 161;
			BaseSoundID = 268;

            Hue = 2498;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(750);

            SetDamage(17, 34);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

			Fame = 4000;
			Karma = -4000;

			PackItem( new BlackPearl(6));
		}

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 0.95;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] = 1;
        }

        public ElderIceElemental(Serial serial): base(serial)
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
