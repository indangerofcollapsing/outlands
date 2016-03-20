using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a corrupt reaver's corpse")]
    public class CorruptReaver : BaseCorrupt
    {
        [Constructable]
        public CorruptReaver(): base()
        {
            Name = "a corrupt reaver";            

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(850);

            AttackSpeed = 50;

            SetDamage(15, 25);

            SetSkill(SkillName.Swords, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

            Fame = 5000;
            Karma = -5000;            

            Utility.AssignRandomHair(this, 0);

            AddItem(new BoneHelm() { Hue = 1766, Movable = false });
            AddItem(new BoneChest() { Hue = 1766, Movable = false });
            AddItem(new PlateArms() { Hue = 1175, Movable = false });
            AddItem(new PlateGorget() { Hue = 1175, Movable = false });
            AddItem(new ChainLegs() { Hue = 1175, Movable = false });
            AddItem(new Boots() { Hue = 1175, Movable = false });
            AddItem(new RingmailGloves() { Hue = 1175, Movable = false });

            AddItem(new HeavyOrnateAxe() { Movable = false, Hue = 1908, Speed = 50, Layer = Layer.FirstValid, Name = "Ebon Battle Axe" });
            AddItem(new MetalShield() { Movable = false, Hue = 1908 });
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public CorruptReaver(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    } 
}
