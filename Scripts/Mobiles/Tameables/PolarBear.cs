using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a polar bear corpse")]
    public class PolarBear : BaseCreature
    {
        [Constructable]
        public PolarBear(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a polar bear";
            Body = 213;
            BaseSoundID = 0xA3;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(225);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;             

            Fame = 1500;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 70;
        }
        
        public override int TamedItemId { get { return 8417; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 275; } }
        public override int TamedBaseMinDamage { get { return 9; } }
        public override int TamedBaseMaxDamage { get { return 11; } }
        public override double TamedBaseWrestling { get { return 70; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
           
            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (attacker is PlayerMobile)
                        effectChance = .02;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.EnrageSpecialAbility(effectChance, attacker, this, .25, 10, -1, true, "Your attack enrages the target.", "", "*becomes enraged*");            
        }
        
        public PolarBear(Serial serial): base(serial)
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