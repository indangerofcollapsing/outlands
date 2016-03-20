using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a royal dragon corpse")]
    public class RoyalDragon : BaseCreature
    {
        [Constructable]
        public RoyalDragon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a royal dragon";

            Body = 59;
            Hue = 2504;

            BaseSoundID = 362;  

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(1000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 100;

            Fame = 15000;
            Karma = -15000;
        }

        public override int Meat { get { return 5; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override bool CanFly { get { return true; } }       

        public override void SetUniqueAI()
        {
            CombatSpecialActionMinDelay = 8;
            CombatSpecialActionMaxDelay = 16;

            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 50;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);          
        } 

        public RoyalDragon(Serial serial): base(serial)
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
