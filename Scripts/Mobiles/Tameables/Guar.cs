using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a guar corpse")]
    public class Guar : BaseCreature
    {        
        [Constructable]
        public Guar()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a guar";
            Body = 270;
            Hue = 0;
            BaseSoundID = 0x63A;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 40;

            Fame = 500;
            Karma = 0;
        }
        
        public override int TamedItemId { get { return 8458; } }
        public override int TamedItemHue { get { return 548; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public Guar(Serial serial): base(serial)
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