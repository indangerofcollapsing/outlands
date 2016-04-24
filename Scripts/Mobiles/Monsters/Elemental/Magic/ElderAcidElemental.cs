using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.ToxicElemental")]
    [CorpseName("an elder acid elemental corpse")]
    public class ElderAcidElemental : BaseCreature
    {
        [Constructable]
        public ElderAcidElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an elder acid elemental";
            Body = 159;
            Hue = 2006;
            BaseSoundID = 263;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(1800);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 40);

            VirtualArmor = 25;

            Fame = 12500;
            Karma = -12500;
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.PierceSpecialAbility(.5, this, defender, 50, 15, -1, true, "", "Their acid momentarily weakens your armor!", "-1");
                       
            Acid acid = new Acid();
            acid.MoveToWorld(defender.Location, Map);

            Effects.PlaySound(defender.Location, Map, Utility.RandomList(0x22F));

            int acidAmount = Utility.RandomMinMax(1, 3);

            for (int a = 0; a < acidAmount; a++)
            {
                Acid extraAcid = new Acid();
                extraAcid.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
            }            
        }

        protected override bool OnMove(Direction d)
        {            
            if (Utility.RandomDouble() <= .5)
            {
                Acid acid = new Acid();
                acid.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }            

            return base.OnMove(d);
        }

        public ElderAcidElemental(Serial serial): base(serial)
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