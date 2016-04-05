using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
	[CorpseName( "a scorpion corpse" )]
	public class Scorpion : BaseCreature
	{
		[Constructable]
		public Scorpion() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a scorpion";
			Body = 48;
			BaseSoundID = 397;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(125);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;               

			Fame = 2000;
			Karma = -2000;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 55;
        }

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }        

        public override int TamedItemId { get { return 8420; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 6; } }
        public override int TamedBaseMaxDamage { get { return 8; } }
        public override double TamedBaseWrestling { get { return 65; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 75; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 100; } }        

		public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public Scorpion( Serial serial ) : base( serial )
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
