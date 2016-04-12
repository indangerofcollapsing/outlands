using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a hollow one's corpse")]
    public class HollowOne : BaseCreature
    {        
        [Constructable]
        public HollowOne(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a hollow one";
            Body = 259;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(400);           

            SetDamage(10, 20);

            AttackSpeed = 30;

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);
           
            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 8500;
            Karma = -8500;
        }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 2.0;
        }

        public override int PoisonResistance { get { return 5; } }
                
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x4E3; }
        public override int GetIdleSound() { return 0x4E2; }
        public override int GetAttackSound() { return 0x60B; }
        public override int GetHurtSound() { return 0x52B; }
        public override int GetDeathSound() { return 0x4E0; }

        public HollowOne(Serial serial): base(serial)
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
