using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a deer corpse")]
    public class Hind : BaseCreature
    {
        [Constructable]
        public Hind(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a hind";
            Body = 0xED;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 35);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25; 

            Fame = 300;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 30.0;
        }

        public override int TamedItemId { get { return 8404; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 100; } }
        public override int TamedBaseMinDamage { get { return 5; } }
        public override int TamedBaseMaxDamage { get { return 7; } }
        public override double TamedBaseWrestling { get { return 55; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }        
        
        public Hind(Serial serial): base(serial)
        {
        }

        public override int GetAttackSound(){return 0x82; }
        public override int GetHurtSound(){return 0x83;}
        public override int GetDeathSound()  {return 0x84; }

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