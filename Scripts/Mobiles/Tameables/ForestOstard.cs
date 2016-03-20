using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a forest ostard corpse")]
    public class ForestOstard : BaseCreature
    {
        [Constructable]
        public ForestOstard(): this("a forest ostard")
        {
        }

        [Constructable]
        public ForestOstard(string name): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)   
        {
            Name = "a forest ostard";
            
            Body = 0xDB;
        
            Hue = Utility.RandomSlimeHue() | 0x8000;

            BaseSoundID = 0x270;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;  
            
            Fame = 450;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 55;
        }
        
        public override int TamedItemId { get { return 8502; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 6; } }
        public override int TamedBaseMaxDamage { get { return 8; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 75; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                }
            }

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied*");
        }

        public ForestOstard(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}