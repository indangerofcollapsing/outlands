using System;
using Server.Mobiles;
using Server.Items;

using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("diseased viscera")]
    public class DiseasedViscera : BaseCreature
    {
        [Constructable]
        public DiseasedViscera(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "diseased viscera";
            Body = 287;
            Hue = Utility.RandomMinMax(1430, 1450);

            BaseSoundID = 0xDB;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(500);

            SetDamage(10, 20);

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

            SetSkill(SkillName.Wrestling, 85);            
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 50);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override bool AlwaysBossMinion { get { return true; } }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }  

        public override bool IsHighSeasBodyType { get { return true; } }

        public override bool OnBeforeDeath()
        {            
            PublicOverheadMessage(MessageType.Regular, 0, false, "*explodes in a shower of bile*");

            for (int a = 0; a < 8; a++)
            {
                Bile bile = new Bile();
                bile.MoveToWorld(new Point3D(X + Utility.RandomMinMax(-3, 3), Y + Utility.RandomMinMax(-3, 3), Z + 1), Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }            

            return base.OnBeforeDeath();
        }

        protected override bool OnMove(Direction d)
        {           
            Blood blood = new Blood();
            blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);

            blood.MoveToWorld(Location, Map);

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
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

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x57F; }        

        public DiseasedViscera(Serial serial): base(serial)
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