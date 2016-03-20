using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Spells.Spellweaving;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a golden balron corpse")]
    public class GoldenBalron : BaseCreature
    {
        [Constructable]
        public GoldenBalron(): base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.8, 0.9)
        {
            Name = "the golden balron";
            Body = 40;
            BaseSoundID = 357;
            Hue = 0x8A5;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(15000);
            SetStam(5000);
            SetMana(10000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 115);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 75000;
            Karma = -75000;
        }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.0;
        }
             
        public override bool AlwaysMurderer { get { return true; } }        
        public override bool CanRummageCorpses { get { return true; } }
        
        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }               

        public GoldenBalron(Serial serial): base(serial)
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