using System;
using Server;

namespace Server.Mobiles
{
	[CorpseName( "a giant ice worm corpse" )]
	public class GiantIceWorm : BaseCreature
	{
		public override bool SubdueBeforeTame { get { return true; } }

		[Constructable]
		public GiantIceWorm() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 89;
			Name = "a giant ice worm";
			BaseSoundID = 0xDC;
            Hue = 2219;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] = 1;
        }

		public GiantIceWorm( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}