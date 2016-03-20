using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a skitter corpse")]
	public class Skitter : BaseCreature
	{
		[Constructable]
		public Skitter () : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            Name = "a skitter";
            Body = 730;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 70.0;

            Fame = 1000;
            Karma = -1000;
        }

        public override int TamedItemId { get { return 8510; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 5; } }
        public override int TamedBaseMaxDamage { get { return 7; } }
        public override double TamedBaseWrestling { get { return 75; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 75; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }        

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetAngerSound() { return 0x2A9; }
        public override int GetIdleSound() { return 0x2A8; }
        public override int GetAttackSound() { return 0x622; }
        public override int GetHurtSound() { return 0x623; }
        public override int GetDeathSound() { return 0x5D5; }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied*");
        }

        public Skitter(Serial serial): base(serial)
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