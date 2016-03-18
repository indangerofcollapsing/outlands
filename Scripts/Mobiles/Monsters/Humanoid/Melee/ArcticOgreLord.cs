using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("an arctic ogre lord's corpse")]
    [TypeAlias("Server.Mobiles.ArticOgreLord")]
    public class ArcticOgreLord : BaseCreature
    {
        [Constructable]
        public ArcticOgreLord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an arctic ogre lord";
            Body = 135;
            BaseSoundID = 427;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;
        }

        public override int Meat { get { return 2; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] = 1;
        }

        public override bool OnBeforeDeath()
        {
            PackItem(new Club());

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new EarthquakeScroll())); } break;
            }
        }

        public ArcticOgreLord(Serial serial)
            : base(serial)
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
