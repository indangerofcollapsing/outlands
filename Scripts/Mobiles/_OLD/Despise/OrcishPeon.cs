
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish peon's corpse")]
    public class OrcishPeon : BaseOrc
    {
        [Constructable]
        public OrcishPeon(): base()
        {
            Name = "an orcish peon";           

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(5, 10);

            SetSkill(SkillName.Archery, 50);
            SetSkill(SkillName.Swords, 50);
            SetSkill(SkillName.Macing, 50);
            SetSkill(SkillName.Fencing, 50);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = -1000;

            AddItem(new OrcMask() { Movable = false, Hue = 0 });
            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });
            AddItem(new ThighBoots() { Movable = false, Hue = 0 });

            switch (Utility.RandomMinMax(1, 8))
            {
                case 1: AddItem(new Cutlass()); break;
                case 2: AddItem(new Scimitar()); break;
                case 3: AddItem(new Mace()); break;
                case 4: AddItem(new Club()); break;
                case 5: AddItem(new Maul()); break;
                case 6: AddItem(new WarMace()); break;
                case 7: AddItem(new Kryss()); break;
                case 8: AddItem(new WarFork()); break;                  
            }

            if (Utility.RandomDouble() < .5)
                AddItem(new WoodenShield());
        }

        public override void OnDeath( Container c )
        {           
            base.OnDeath( c );
        }

        public override int DoubloonValue { get { return 4; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public OrcishPeon(Serial serial): base(serial)
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
