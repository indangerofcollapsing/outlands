
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a creeper corpse")]
    public class Creeper : BaseCreature
    {
        [Constructable]
        public Creeper(): base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a creeper";

            Body = 302;
            BaseSoundID = 959;
            Hue = 2130;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.DetectHidden, 75);

            VirtualArmor = 25;

            Fame = 300;
            Karma = -300;            
        }

        public override int PoisonResistance { get { return 5; } }

        public override void OnDeath(Container c) 
        {
            base.OnDeath(c);
        }

        public Creeper(Serial serial) : base(serial)
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
