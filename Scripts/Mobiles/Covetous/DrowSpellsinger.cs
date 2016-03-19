using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a drow spellsinger corpse")]
    public class DrowSpellsinger : BaseDrow
    {   
        [Constructable]
        public DrowSpellsinger(): base()
        {            
            Name = "a drow spellsinger";              

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(20, 30);            
            
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Fencing, 95);
            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 200);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            AddItem(new LeatherGorget() { Movable = false, Hue = itemHue });
            AddItem(new LeatherChest() { Movable = false, Hue = itemHue });
            AddItem(new LeatherArms() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });

            Utility.AssignRandomHair(this, hairHue);

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new DoubleBladedStaff() { Movable = false, Speed = 40,  Hue = weaponHue, Name = "a drow nightstaff" }); break;
                case 2: AddItem(new BladedStaff() { Movable = false, Speed = 40, Hue = weaponHue, Name = "a drow bladestaff" }); break;
                case 3: AddItem(new BlackStaff() { Movable = false, Speed = 40, Hue = weaponHue, Name = "a drow blackstaff" }); break;
            }
        }

        public override void SetUniqueAI()
        {
            DictCombatSpell[CombatSpell.SpellDispelSummon] = 25;
        }

        public DrowSpellsinger(Serial serial): base(serial)
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