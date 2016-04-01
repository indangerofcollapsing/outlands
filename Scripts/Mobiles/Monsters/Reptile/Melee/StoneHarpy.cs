using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a stone harpy corpse")]
    public class StoneHarpy : BaseCreature
    {
        [Constructable]
        public StoneHarpy(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a stone harpy";
            Body = 73;
            BaseSoundID = 402;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);
            
            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

            Fame = 4500;
            Karma = -4500; 
        }

        public override bool HasFeathers { get { return true; } }

        public override bool CanFly { get { return true; } }

        public override int GetAttackSound() { return 916; }
        public override int GetAngerSound() { return 916; }
        public override int GetDeathSound() { return 917; }
        public override int GetHurtSound() { return 919; }
        public override int GetIdleSound() { return 918; }  

        public StoneHarpy(Serial serial): base(serial)
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
