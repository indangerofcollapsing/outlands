using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a rock guar corpse")]
    public class RockGuar : BaseCreature
    {
        [Constructable]
        public RockGuar(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rock guar";

            Body = 270;
            Hue = 2590;
            BaseSoundID = 0x63A;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 150;

            Fame = 500;
            Karma = 0;
        }

        public override bool AlwaysBossMinion { get { return true; } }
        
        public RockGuar(Serial serial): base(serial)
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