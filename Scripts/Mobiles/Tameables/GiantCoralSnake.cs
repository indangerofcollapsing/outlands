using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a giant coral snake corpse")]
    public class GiantCoralSnake : BaseCreature
    {
        [Constructable]
        public GiantCoralSnake(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant coral snake";
            Body = 726;
            Hue = 0;
            BaseSoundID = 219;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 15);

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 60;

            Fame = 500;
            Karma = -500;

            PackItem(new Bone(6));
        }
        
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int TamedItemId { get { return 17046; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 70; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 50; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }        

        public GiantCoralSnake(Serial serial): base(serial)
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