using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an oreling corpse")]
    public class RockOreling : BaseCreature
    {
        [Constructable]
        public RockOreling(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rock oreling";
            Body = 305;
            Hue = 1108;
            BaseSoundID = 268;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(200);
            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;  

            Fame = 1500;
            Karma = -1500;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public RockOreling(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
