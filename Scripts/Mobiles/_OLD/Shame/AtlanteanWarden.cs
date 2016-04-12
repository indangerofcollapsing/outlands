using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an atlantean warden corpse")]
    public class AtlanteanWarden : BaseAtlantian
    {
        [Constructable]
        public AtlanteanWarden(): base()
        {
            Name = "an atlantean warden";

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(750);

            SetDamage(15, 30);

            AttackSpeed = 30;

            SetSkill(SkillName.Swords, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;
            
            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new DragonGloves() { Movable = false, Hue = alternateHue, Name = "warfist of Atlantis" });

            AddItem(new RadiantScimitar() { Movable = false, Speed = 30, Hue = 2222, Name = "an atlantean wavecutter" });
            AddItem(new Lantern() { Movable = false, Hue = 2222 });
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.2, this, defender, DamageMax, 8.0, -1, true, "", "Their vicious strike causes you to bleed!");
        }

        public AtlanteanWarden(Serial serial): base(serial)
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
