using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a bear corpse" )]
	public class BrownBear : BaseCreature
	{
        public override bool DropsGold { get { return false; } }
        public override double MaxSkillScrollWorth { get { return 0.0; } }
		[Constructable]
		public BrownBear() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a brown bear";
			Body = 167;
			BaseSoundID = 0xA3;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(175);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 450;
			Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 60;
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 12; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8472; } }
        public override int TamedItemHue { get { return 442; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 70; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Global_AllowAbilities)
            {
                double effectChance = .15;

                if (Controlled && ControlMaster != null)
                {
                    if (ControlMaster is PlayerMobile)
                    {
                        if (attacker is PlayerMobile)
                            effectChance = .02;
                        else
                            effectChance = .25;
                    }
                }

                SpecialAbilities.EnrageSpecialAbility(effectChance, attacker, this, .25, 10, -1, true, "Your attack enrages the target.", "", "*becomes enraged*");
            }
        }
		
		public BrownBear( Serial serial ) : base( serial )
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