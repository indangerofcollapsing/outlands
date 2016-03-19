using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish grunt's corpse")]
    public class OrcishGrunt : BaseOrc
    {
        [Constructable]
        public OrcishGrunt(): base()
        {
            Name = "an orcish grunt";

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(550);

            SetDamage(20, 30);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Fencing, 90);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

            Fame = 4500;
            Karma = -4500;

            AddItem(new OrcMask() { Movable = false, Hue = 2130 });
            AddItem(new RingmailChest() { Movable = false, Hue = 0 });
            AddItem(new RingmailLegs() { Movable = false, Hue = 0 });
            AddItem(new BoneArms() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });
            AddItem(new BodySash() { Movable = false, Hue = 2051 });
            AddItem(new Boots() { Movable = false, Hue = 0 });

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: AddItem(new TwoHandedAxe()); break;
                case 2: AddItem(new DoubleAxe()); break;
                case 3: AddItem(new WarHammer()); break;
                case 4: AddItem(new QuarterStaff()); break;
                case 5: AddItem(new Spear()); break;
            }
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 8; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public override void OnDeath( Container c )
        {           
            base.OnDeath( c );

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_OrcKilled);
            // END IPY ACHIEVEMENT TRIGGER
        }

        public override int Meat { get { return 4; } }

        public OrcishGrunt(Serial serial): base(serial)
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
