using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
    [CorpseName("a shattered skull")]
    public class HalloweenFlamingSkull : BaseCreature
    {
        [Constructable]
        public HalloweenFlamingSkull() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a flaming skull";
            Body = 562;
            BaseSoundID = 412;
            Hue = 1260;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(1600);
            SetMana(4000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 50;

            Fame = 25000;
            Karma = -25000;

            PackItem(new Bone(30));
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.2;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        protected override bool OnMove(Direction d)
        {

            if (Utility.RandomDouble() < .25)
            {
                Effects.PlaySound(Location, Map, 0x208);

                SingleFireField singleFireField = new SingleFireField(this, 0, 1, 20, 3, 5, false, false, true, -1, true);
                singleFireField.Hue = Hue;
                singleFireField.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (!willKill && amount > 10 && Utility.RandomDouble() < .2)
            {
                Effects.PlaySound(Location, Map, 0x208);

                SingleFireField singleFireField = new SingleFireField(this, 0, 1, 20, 3, 5, false, false, true, -1, true);
                singleFireField.Hue = Hue;
                singleFireField.MoveToWorld(Location, Map);
            }
        }

        public HalloweenFlamingSkull(Serial serial) : base(serial)
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