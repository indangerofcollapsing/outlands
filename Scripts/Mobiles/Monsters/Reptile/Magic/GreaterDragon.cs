using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class GreaterDragon : BaseCreature
    {
        [Constructable]
        public GreaterDragon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "a greater dragon";
            Body = 12;            
            Hue = 2213;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(1800);
            SetMana(3000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 22000;
            Karma = -15000;         
        }
        
        public override bool CanFly { get { return true; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void GenerateLoot()
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        public GreaterDragon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
