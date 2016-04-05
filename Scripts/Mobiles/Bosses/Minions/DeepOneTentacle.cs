using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a tentacle corpse")]
    public class DeepOneTentacle : BaseCreature
    {   
        [Constructable]
        public DeepOneTentacle(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deep one's tentacle";

            Body = 743;
            Hue = 0;
            BaseSoundID = 0x388;            

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(10, 20);

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 500;
            Karma = -500;
        }        

        public override void SetUniqueAI()
        {   
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            UniqueCreatureDifficultyScalar = 2.0;
        }

        public override bool AlwaysBossMinion { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender); 
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (c != null)
                    c.Delete();
            });
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }        
        
        public override int GetAngerSound() { return 0x383; }
        public override int GetIdleSound() { return 0x382; }
        public override int GetAttackSound() { return 0x384; }
        public override int GetHurtSound() { return 0x385; }
        public override int GetDeathSound() { return 0x386; }

        public DeepOneTentacle(Serial serial): base(serial)
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