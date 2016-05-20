using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vampire bat corpse")]
    public class VampireBat : BaseCreature
    {
        [Constructable]
        public VampireBat(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vampire bat";
            Body = 317;
            Hue = 2105;
            BaseSoundID = 0x642;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(250);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 110.1;

            Fame = 500;
            Karma = -500;
        }

        public override string TamedDisplayName { get { return "Vampire Bat"; } }

        public override int TamedItemId { get { return 9777; } }
        public override int TamedItemHue { get { return 2105; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 200; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 0; } }

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

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override bool CanFly { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .20;
                }
            }

            if (Utility.RandomDouble() <= effectChance)
            {
                int healingAmount = (int)((double)HitsMax * .25);

                Hits += healingAmount;

                this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

                Blood blood = new Blood();
                blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);

                if (defender is PlayerMobile)
                    SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, 0x44D, true, "", "The creature sinks its fangs into you, causing you to bleed!", "-1");

                else
                {
                    SpecialAbilities.DisorientSpecialAbility(1.0, this, defender, .15, 10, -1, false, "", "", "-1");
                    SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, 0x44D, true, "", "The creature sinks its fangs into you, stunning you and causing you to bleed!", "-1");
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        public VampireBat(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}