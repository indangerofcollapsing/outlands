using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a royal drake corpse")]
    public class RoyalDrake : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public RoyalDrake(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a royal drake";

            Body = 61;
            Hue = 2504;

            BaseSoundID = 362;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;            

            Fame = 5500;
            Karma = -5500;
        }

        public override int Meat { get { return 10; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override bool CanFly { get { return true; } }

        public override void SetUniqueAI()
        {
            CombatSpecialActionMinDelay = 9;
            CombatSpecialActionMaxDelay = 18;

            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 50;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);            
           
            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new GateTravelScroll())); } break;
            }            
        }

        public RoyalDrake(Serial serial): base(serial)
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
