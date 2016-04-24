
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an armored titan corpse")]
    public class ArmoredTitan : BaseCreature
    {
        [Constructable]
        public ArmoredTitan(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 189;
            Hue = 1777;

            Name = "an armored titan";
            BaseSoundID = 604;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(2500);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 150;

            Fame = 11500;
            Karma = -11500;       
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }
        
        public override bool CanRummageCorpses { get { return true; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .25)
            {
                SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 3, -1, false, "", "The titan's massive strike stuns you!", "-1");
                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .20, 10, -1, false, "", "", "-1");
                SpecialAbilities.DisorientSpecialAbility(1.0, this, defender, .10, 10, -1, true, "", "", "-1");
            }
        }        

        public ArmoredTitan(Serial serial): base(serial)
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
