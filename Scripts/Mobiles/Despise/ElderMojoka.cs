
using Server.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an elder mojoka's corpse")]
    public class ElderMojoka : BaseOrc
    {
        [Constructable]
        public ElderMojoka(): base()
        {
            Name = "an elder mojoka";

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(800);
            SetMana(2000);

            SetDamage(12, 24);

            SetSkill(SkillName.Macing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 25;

            AddItem(new OrcMask() { Movable = false, Hue = 2130 });
            AddItem(new Robe() { Movable = true, Hue = Utility.RandomNondyedHue() });
            AddItem(new BoneGloves() { Movable = false, Hue = 0 });
            AddItem(new Skirt() { Movable = false, Hue = 1775 });

            AddItem(new GnarledStaff());
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 10; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public ElderMojoka(Serial serial): base(serial)
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
