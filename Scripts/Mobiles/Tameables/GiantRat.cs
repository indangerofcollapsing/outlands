using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a giant rat corpse" )]
	[TypeAlias( "Server.Mobiles.Giantrat" )]
	public class GiantRat : BaseCreature
	{
		[Constructable]
		public GiantRat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant rat";
			Body = 0xD7;
			BaseSoundID = 0x188;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 35);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;
            
			Fame = 300;
			Karma = -300;
            
            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        public override int Meat { get { return 1; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8400; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

		public override void GenerateLoot()
		{
			PackItem(new Server.Items.Bandage(1));
		}

		public GiantRat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}