using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an exorcised pile of equipment")]
    public class HalloweenPoltergeist : BaseDrow
    {

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "",
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "",
                };
            }
        }

        public HalloweenPoltergeist(Serial serial): base(serial)
        {
        }

        [Constructable]
        public HalloweenPoltergeist() : base()
        {
            Name = "a poltergeist";
            Body = 614;
            Hue = 0;
            BaseSoundID = 0x1D2;

            SetStr(75);
            SetDex(75);
            SetInt(75);

            SetHits(1250);

            SetDamage(18, 26);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 100;

            Fame = 25000;
            Karma = -25000;

        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.33;

            CombatEpicActionMinDelay = 15;
            CombatEpicActionMaxDelay = 30;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MeleeBleedAoE] = 25;

        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.2, this, defender, DamageMax, 8.0, -1, true, "", "The blade lacerates you, severing an artery!");
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