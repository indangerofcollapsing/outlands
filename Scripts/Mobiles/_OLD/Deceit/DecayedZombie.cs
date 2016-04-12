
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a rotting corpse")]
    public class DecayedZombie : BaseCreature
    {
        [Constructable]
        public DecayedZombie(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 1.0)
        {
            Name = "a decayed zombie";
            Body = 3;
            BaseSoundID = 471;
            Hue = 1175;    

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            AttackSpeed = 20;
            
            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 720;
            Karma = -600;
            
            PackItem(new Bone(2));

            switch (Utility.Random(3))
            {
                case 0: PackItem(new LeftArm()); break;
                case 1: PackItem(new RightArm()); break;
                case 2: PackItem(new Torso()); break;
            }
        }

        public override int PoisonResistance { get { return 5; } }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            
            if (Utility.RandomDouble() < .5)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                Item corpsePart = new Blood();
                corpsePart.ItemID = Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600);

                corpsePart.MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            
        }

        protected override bool OnMove(Direction d)
        {            
            if (Utility.RandomDouble() < .5)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                Item corpsePart = new Blood();
                corpsePart.ItemID = Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600);

                corpsePart.MoveToWorld(Location, Map);
            }            

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

            int corpseItems = Utility.RandomMinMax(2, 3);

            for (int a = 0; a < corpseItems; a++)
            {
                Point3D point = new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z);

                Item corpsePart = new Blood();
                corpsePart.ItemID = Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600);

                corpsePart.MoveToWorld(point, Map);

                new Blood().MoveToWorld(point, Map);
            }            

            return base.OnBeforeDeath();
        }

        public DecayedZombie(Serial serial): base(serial)
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
