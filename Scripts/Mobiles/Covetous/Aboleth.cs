using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
    [CorpseName("an aboleth corpse")]
    public class Aboleth : BaseCreature
    {
        [Constructable]
        public Aboleth(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an aboleth";
            Body = 316;
            BaseSoundID = 377;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(250);
            SetMana(1000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150); 

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 25;

            CanSwim = true;
        }       

        public override void SetUniqueAI()
        {   
            DictCombatSpell[CombatSpell.SpellDamage5] = 8;

            CombatSpecialActionMinDelay = 5;
            CombatSpecialActionMaxDelay = 10;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.CauseWounds] = 1;

            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override int GetAngerSound(){return 0x594;}
        public override int GetAttackSound(){return 0x34C;}
        public override int GetHurtSound(){return 0x386;}
        public override int GetDeathSound(){return 0x58D;}
        public override int GetIdleSound(){return 0x34E;}
        
        public Aboleth(Serial serial): base(serial)
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
