using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
    [CorpseName("a smashed pumpkin")]
    public class HalloweenPossessedPumpkin : BaseCreature
    {
        [Constructable]
        public HalloweenPossessedPumpkin() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a possessed pumpkin";
            Body = 725;
            Hue = 1356;
            BaseSoundID = 0x1C8;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(1400);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Poisoning, 80);

            VirtualArmor = 50;

            Fame = 25000;
            Karma = -25000;
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 0.62;
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.PierceSpecialAbility(.25, this, defender, .5, 15, -1, true, "", "Acidic juice momentarily weakens your armor!");

            if (Global_AllowAbilities)
            {
                Acid acid = new Acid();
                acid.MoveToWorld(defender.Location, Map);

                Effects.PlaySound(defender.Location, Map, Utility.RandomList(0x22F));

                int acidAmount = Utility.RandomMinMax(0, 1);

                for (int a = 0; a < acidAmount; a++)
                {
                    Acid extraAcid = new Acid();
                    extraAcid.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
                }
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            SpecialAbilities.EntangleSpecialAbility(0.15, this, attacker, 1.0, 5, -1, true, "", "Decaying vines entwine your limbs!");
        }

        protected override bool OnMove(Direction d)
        {
            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() <= .5)
                {
                    Acid acid = new Acid();
                    acid.MoveToWorld(Location, Map);

                    Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
                }
            }

            return base.OnMove(d);
        }

        public HalloweenPossessedPumpkin(Serial serial) : base(serial)
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