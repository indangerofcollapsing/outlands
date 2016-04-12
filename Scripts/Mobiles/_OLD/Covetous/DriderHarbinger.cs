using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drider harbinger corpse" )]
	public class DriderHarbinger : BaseCreature
	{
		[Constructable]
		public DriderHarbinger() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a drider harbinger";
			Body = 72;
			BaseSoundID = 599;
            Hue = 1102;

			SetStr(100);
			SetDex(50);
			SetInt(100);

			SetHits(1500);
            SetMana(2000);

			SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.EvalInt, 100);
			SetSkill(SkillName.Magery, 100);

			SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;			
		}

        public override void SetUniqueAI()
        {
            CastOnlyEnergySpells = true;

            DictCombatSpell[CombatSpell.SpellDamage4] = 20;
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

		public DriderHarbinger( Serial serial ) : base( serial )
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
