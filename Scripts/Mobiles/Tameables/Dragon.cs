using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class Dragon : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public Dragon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dragon";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;           

            if (Utility.Random(250) == 0)            
                Hue = Utility.RandomList(1329, 1900, 1438, 1637);  
          
            else if (Utility.Random(500) == 0)            
                Hue = Utility.RandomList(1645, 1443, 1908, 1336);
            
            else if (Utility.Random(1000) == 0)            
                Hue = 0x5555;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(1000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;

            Tameable = true;
            ControlSlots = 3;
            MinTameSkill = 95;
        }

        public override bool CanFly { get { return true; } }

        public override int TamedItemId { get { return 9780; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return -10; } }

        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 25; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 500; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 25; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }        

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;            
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage1;
            UpdateAI(false);

            DictCombatSpell[CombatSpell.SpellDamage1] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage2] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage3] += 2;
            DictCombatSpell[CombatSpell.SpellDamage4] += 2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;    
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        } 

        public Dragon(Serial serial): base(serial)
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
