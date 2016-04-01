using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a parrot corpse")]
    public class Parrot : BaseCreature
    {        
        [Constructable]
        public Parrot(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Body = 831;
            Name = ("a parrot");

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 25.0;
        }

        public override int TamedItemId { get { return 11675; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 20; } }

        public override int TamedBaseMaxHits { get { return 75; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
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

        public override bool HasFeathers { get { return true; } }

        public override bool CanFly { get { return true; } }
        
        public Parrot(Serial serial): base(serial)
        {
        }

        public override int GetAngerSound() { return 0x1B;  }
        public override int GetIdleSound() { return 0x1C;  }
        public override int GetAttackSound()  {return 0x1D; }
        public override int GetHurtSound() { return 0x1E; }
        public override int GetDeathSound()  {   return 0x1F;  }

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