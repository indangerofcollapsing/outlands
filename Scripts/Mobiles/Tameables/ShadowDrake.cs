using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow drake corpse")]
    public class ShadowDrake : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public ShadowDrake(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow drake";
            Body = Utility.RandomList(60, 61);
            Hue = 25000;
            BaseSoundID = 362;            

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(500);
            SetMana(1000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;            

            Fame = 5500;
            Karma = -5500;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 95;
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }

        public override bool CanFly { get { return true; } }

        public override int Meat { get { return 10; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Horned; } }   

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8406; } }
        public override int TamedItemHue { get { return 2500; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 20; } }
        public override int TamedBaseMaxDamage { get { return 22; } }
        public override double TamedBaseWrestling { get { return 90; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 500; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 25; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1;
        }

        public override void SetTamedAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new GateTravelScroll())); } break;
            }
        }

        public ShadowDrake(Serial serial): base(serial)
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
