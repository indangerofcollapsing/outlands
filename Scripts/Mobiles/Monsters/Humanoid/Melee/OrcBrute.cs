using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcBrute : BaseCreature
    {
        [Constructable]
        public OrcBrute(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 189;

            Name = "an orc brute";
            BaseSoundID = 0x45A;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1250);

            SetDamage(25, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;
        }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.15;
        }
        
        public override bool CanRummageCorpses { get { return true; } }
        
        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public OrcBrute(Serial serial): base(serial)
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
