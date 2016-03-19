using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class ModAncientWyrm : BaseCreature
    {
        [Constructable]
        public ModAncientWyrm(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ancient wyrm";
            Body = 46;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2000);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 25500;
            Karma = -22500;
        }
               
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }
        
        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_AncientWyrmKilled);
            // END IPY ACHIEVEMENT TRIGGER
        }

        public override int GetIdleSound()        {            return 0x2D3;        }
        public override int GetHurtSound()        {            return 0x2D1;        }            
        
        public ModAncientWyrm(Serial serial): base(serial)
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
