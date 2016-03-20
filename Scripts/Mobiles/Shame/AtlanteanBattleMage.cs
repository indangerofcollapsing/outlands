using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an atlantean battlemage corpse")]
    public class AtlanteanBattleMage : BaseAtlantian
    {
        [Constructable]
        public AtlanteanBattleMage(): base()
        {
            Name = "an atlantean battlemage";

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(1000);
            SetMana(3000);

            SetDamage(12, 24);

            AttackSpeed = 40;

            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Fencing, 95);
            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            AddItem(new Sandals() { Movable = false, Hue = alternateHue });
            AddItem(new Skirt() { Movable = false, Hue = alternateHue });
            AddItem(new BodySash() { Movable = false, Hue = alternateHue });

            AddItem(new WarCleaver() { Movable = false, Speed = 40, Hue = 2222, Name = "an atlantean deepblade" });            
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public AtlanteanBattleMage(Serial serial): base(serial)
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
