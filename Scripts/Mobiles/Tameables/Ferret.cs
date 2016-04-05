using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a ferret corpse")]
    public class Ferret : BaseCreature
    {
        [Constructable]
        public Ferret(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a ferret";
            Body = 279;
            Hue = 0;
            BaseSoundID = 0x5e4;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(50);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 30;

            Fame = 400;
            Karma = 0;
        }
        
        public override int TamedItemId { get { return 11672; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 15; } }

        public override int TamedBaseMaxHits { get { return 75; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 75; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public Ferret(Serial serial): base(serial)
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