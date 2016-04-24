using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a corrupt spiderling corpse")]
    public class CorruptSpiderling : BaseCreature
    {   
        [Constructable]
        public CorruptSpiderling(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a corrupt spiderling";
            Body = 737;
            Hue = 0;
            BaseSoundID = 0x388;
            Hue = 1501;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(5, 10);

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 33);

            VirtualArmor = 75;

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
           
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override bool AlwaysBossMinion { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            
            if (Utility.RandomDouble() < .25)
            {
                Effects.PlaySound(defender.Location, defender.Map, 0x4F1);
                defender.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);

                defender.SendMessage("You have weakened by the creature's venom!");

                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .25, 10, -1, false, "", "", "-1");
                SpecialAbilities.DisorientSpecialAbility(1.0, this, defender, .15, 10, -1, false, "", "", "-1");
            }            
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

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override int PoisonResistance { get { return 3; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }        
        
        public override int GetAngerSound() { return 0x492; }
        public override int GetIdleSound() { return 0x493; }
        public override int GetAttackSound() { return 0x495; }
        public override int GetHurtSound() { return 0x603; }
        public override int GetDeathSound() { return 0x386; }

        public CorruptSpiderling(Serial serial): base(serial)
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