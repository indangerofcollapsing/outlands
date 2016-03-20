using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a water elemental corpse")]
    public class SummonedWaterElemental : BaseCreature
    {
        [Constructable]
        public SummonedWaterElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a water elemental";
            Body = 16;
            BaseSoundID = 278;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(14, 16);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);
            
            SetSkill(SkillName.MagicResist, 200);            

            VirtualArmor = 50;

            ControlSlots = 2;
        }

        public SummonedWaterElemental(Serial serial): base(serial)
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