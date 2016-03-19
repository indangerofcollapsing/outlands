using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("deep water corpse")]
    public class DeepWater : BaseCreature
    {
        [Constructable]
        public DeepWater(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "deep water";

            Body = 51;
            Hue = 2600;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(300);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 200);

            VirtualArmor = 25;

            Fame = 4500;
            Karma = -4500;
            
            CanSwim = true;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

            PackItem(new BlackPearl(2));
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 50;

            UniqueCreatureDifficultyScalar = 1.5;
        }

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool IsOceanCreature { get { return true; } }

        protected override bool OnMove(Direction d)
        {
            Blood blood = new Blood();
            blood.Hue = Hue;
            blood.Name = "water";
            blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

            blood.MoveToWorld(Location, Map);

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Timer.DelayCall(TimeSpan.FromSeconds(10), delegate
            {
                if (c != null)
                    c.Delete();
            });
        }

        public override int GetAngerSound() { return 0x1C8; }
        public override int GetIdleSound() { return 0x27; }
        public override int GetAttackSound() { return 0x026; }
        public override int GetHurtSound() { return 0x50; }
        public override int GetDeathSound() { return 0x04E; }

        public DeepWater(Serial serial): base(serial)
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
