
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orc mojoka's corpse")]
    public class OrcMojoka : BaseOrc
    {
        [Constructable]
        public OrcMojoka(): base()
        {
            Name = "an orcish mojoka";

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(9, 18);

            SetSkill(SkillName.Macing, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 8000;
            Karma = -8000;            

            int dyedColor = Utility.RandomNondyedHue();

            AddItem(new OrcMask() { Movable = false, Hue = 2130 });
            AddItem(new BodySash() { Movable = true, Hue = dyedColor });
            AddItem(new BoneGloves() { Movable = false, Hue = 0 });
            AddItem(new Skirt() { Movable = true, Hue = dyedColor });
            AddItem(new Boots() { Movable = false, Hue = 0 });  
            AddItem(new GnarledStaff());
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int DoubloonValue { get { return 6; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }
        
        public override void OnDeath( Container c )
        {           
            base.OnDeath( c );
        }

        public OrcMojoka(Serial serial): base(serial)
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
