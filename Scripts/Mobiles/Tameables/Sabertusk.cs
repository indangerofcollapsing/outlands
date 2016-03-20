using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sabertusk corpse" )]
	public class Sabertusk : BaseCreature
	{
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }
        
		[Constructable]
		public Sabertusk() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sabertusk";

			Body = 1069;
            Hue = 2599;

			BaseSoundID = 660;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(500);            

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 600;
			Karma = -600;

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 115.1;
        }

        public override int TamedItemId { get { return 16381; } }
        public override int TamedItemHue { get { return 2599; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 18; } }
        public override int TamedBaseMaxDamage { get { return 20; } }
        public override double TamedBaseWrestling { get { return 100; } }
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

        public override int AttackAnimation { get { return Utility.RandomList(4, 5); } }
        public override int AttackFrames { get { return 12; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 11; } }

        public override int IdleAnimation { get { return 28; } }
        public override int IdleFrames { get { return 20; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

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

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override void SetTamedAI()
        {
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override int GetAngerSound() { return 0x52D; }
        public override int GetIdleSound() { return 0x3EF; }
        public override int GetAttackSound() { return 0x3EE; }
        public override int GetHurtSound() { return 0x3F1; } 
        public override int GetDeathSound() { return 0x517; } 

		public Sabertusk(Serial serial) : base(serial)
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
