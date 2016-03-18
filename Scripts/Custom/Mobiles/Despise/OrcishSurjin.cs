using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish surjin corpse")]
    public class OrcishSurjin : BaseOrc
    {
        [Constructable]
        public OrcishSurjin(): base()
        {
            Name = "an orcish surjin";

            SetStr(50);
            SetDex(50);
            SetInt(50);

            SetHits(400);

            SetDamage(8, 16);

            SetSkill(SkillName.Archery, 85);
            SetSkill(SkillName.Swords, 85);
            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Healing, 75);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            AddItem(new OrcMask() { Movable = false, Hue = 2130 });
            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new FullApron() { Movable = false, Hue = 2707 });
            AddItem(new LeatherLegs() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 1775 });
            AddItem(new Boots() { Movable = false, Hue = 2051 });

            AddItem(new BoneHarvester() { Movable = false, Hue = 1775, Name = "bloody surjin knife" });
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 8; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public override bool OnBeforeDeath()
        {
            AwardDailyAchievementForKiller(PvECategory.KillOrcishSurijins);

            return base.OnBeforeDeath();
        }
        
        public override void OnDeath( Container c )
        {           
            base.OnDeath( c );

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_OrcKilled);
            // END IPY ACHIEVEMENT TRIGGER
        }

        public OrcishSurjin(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
