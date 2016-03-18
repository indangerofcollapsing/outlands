using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
    public class FactionWizard : BaseFactionGuard
    {
        [Constructable]
        public FactionWizard()
            : base("the wizard")
        {
            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(330);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            GenerateBody(false, false);

            AddItem(Immovable(Rehued(new WizardsHat(), 1325)));
            AddItem(Immovable(Rehued(new Sandals(), 1325)));
            AddItem(Immovable(Rehued(new Robe(), 1310)));
            AddItem(Immovable(Rehued(new LeatherGloves(), 1325)));

            AddItem(Newbied(Rehued(new BlackStaff(), 1310)));

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

        public FactionWizard(Serial serial)
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