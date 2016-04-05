
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a corrupt warmage's corpse")]
    public class CorruptWarmage : BaseCorrupt
    {
        [Constructable]
        public CorruptWarmage(): base()
        {
            Name = "a corrupt warmage";

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(250);
            SetMana(1000);

            SetDamage(8, 16);

            AttackSpeed = 30;

            SetSkill(SkillName.Swords, 75);
            SetSkill(SkillName.Fencing, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 50;

            Fame = 3000;
            Karma = -3000;

            PackReg(6);

            Utility.AssignRandomHair(this, 0);

            AddItem(new Sandals() { Hue = 1175, Movable = false });
            AddItem(new BoneHelm() { Hue = 1766, Movable = false });
            AddItem(new BoneChest() { Hue = 1766, Movable = false });
            AddItem(new Kilt() { Hue = 1175, Movable = false });

            AddItem(new Scythe() { Hue = 1175, Movable = false, Speed = 30 });
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }
        
        public CorruptWarmage(Serial serial): base(serial)
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
