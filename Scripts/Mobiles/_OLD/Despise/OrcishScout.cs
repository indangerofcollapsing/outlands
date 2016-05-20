using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish scout corpse")]
    public class OrcishScout : BaseOrc
    {
        [Constructable]
        public OrcishScout(): base()
        {
            Name = "an orcish scout";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);

            SetSkill(SkillName.Archery, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.DetectHidden, 75);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            AddItem(new OrcHelm() { Movable = false, Hue = 0 });
            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });
            AddItem(new ThighBoots() { Movable = false, Hue = 0 });

            AddItem(new Bow());
            AddItem(new Arrow(10));
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int DoubloonValue { get { return 6; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.PlaySound(0x234);
        }

        public override int GetAttackSound() { return 0x238; }

        public OrcishScout(Serial serial): base(serial)
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
