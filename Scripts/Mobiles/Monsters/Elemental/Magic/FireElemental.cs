using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;


namespace Server.Mobiles
{
    [CorpseName("a fire elemental corpse")]
    public class FireElemental : BaseCreature
    {
        [Constructable]
        public FireElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a fire elemental";
            Body = 15;
            BaseSoundID = 838;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(200);
            SetMana(1000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 4500;
            Karma = -4500;            

            ControlSlots = 2;

            AddItem(new LightSource());
        }

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public FireElemental(Serial serial): base(serial)
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

            if (BaseSoundID == 274)
                BaseSoundID = 838;
        }
    }
}
