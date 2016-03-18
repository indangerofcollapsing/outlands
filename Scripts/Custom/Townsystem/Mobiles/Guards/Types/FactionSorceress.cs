using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
    public class FactionSorceress : BaseFactionGuard
    {
        [Constructable]
        public FactionSorceress()
            : base("the sorceress")
        {
            SetStr(125);
            SetDex(75);
            SetInt(100);

            SetHits(300);
            SetMana(2000);

            SetDamage(25, 30);

            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 90);
            SetSkill(SkillName.EvalInt, 90);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 110);

            VirtualArmor = 25;

            GenerateBody(true, false);

            AddItem(Immovable(Rehued(new WizardsHat(), 1325)));
            AddItem(Immovable(Rehued(new Sandals(), 1325)));
            AddItem(Immovable(Rehued(new LeatherGorget(), 1325)));
            AddItem(Immovable(Rehued(new LeatherGloves(), 1325)));
            AddItem(Immovable(Rehued(new LeatherLegs(), 1325)));
            AddItem(Immovable(Rehued(new Skirt(), 1325)));
            AddItem(Immovable(Rehued(new FemaleLeatherChest(), 1325)));

            AddItem(Newbied(Rehued(new QuarterStaff(), 1310)));

            PackItem(new Bandage(Utility.RandomMinMax(30, 40)));
            PackStrongPotions(6, 12);
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatHealSelf] = 2;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
        }

        public FactionSorceress(Serial serial)
            : base(serial)
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