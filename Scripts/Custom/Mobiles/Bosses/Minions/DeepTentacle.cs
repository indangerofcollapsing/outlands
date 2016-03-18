using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a tentacle corpse")]
    public class DeepTentacle : BaseCreature
    {
        public DateTime m_NextShipEntangleAllowed;
        public TimeSpan NextShipEntangleDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15));
        public TimeSpan ShipEntangleDuration = TimeSpan.FromSeconds(3);

        [Constructable]
        public DeepTentacle(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deep tentacle";

            Body = 743;
            Hue = 0;
            BaseSoundID = 0x388;            

            SetStr(50);
            SetDex(100);
            SetInt(25);

            SetHits(300);

            SetDamage(5, 10);            

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 500;
            Karma = -500;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;
        }        

        public override void SetUniqueAI()
        {   
            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 50;

            UniqueCreatureDifficultyScalar = 2.0;
        }

        public override int AttackRange { get { return 3; } }

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool IsOceanCreature { get { return true; } }

        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 8; } }

        public override int HurtAnimation { get { return 1; } }
        public override int HurtFrames { get { return 6; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender); 
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextShipEntangleAllowed <= DateTime.UtcNow && BoatOccupied != null)
            {
                if (BoatOccupied.Deleted || BoatOccupied.m_SinkTimer != null)
                    return;

                BoatOccupied.TempSpeedModifier = 0;
                BoatOccupied.TempSpeedModifierExpiration = DateTime.UtcNow + ShipEntangleDuration;                

                PublicOverheadMessage(Network.MessageType.Regular, 0, false, "*entangles the ship*");

                Animate(2, 8, 1, true, false, 0);
                Effects.PlaySound(Location, Map, 0x34D);

                m_NextShipEntangleAllowed = DateTime.UtcNow + NextShipEntangleDelay;
            }
        } 

        protected override bool OnMove(Direction d)
        {           
            Blood blood = new Blood();
            blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);
            blood.Name = "tentacle slime";
            blood.Hue = 2609;

            blood.MoveToWorld(Location, Map);            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
            {
                if (c != null)
                    c.Delete();
            });
        }
        
        public override int GetAngerSound() { return 0x383; }
        public override int GetIdleSound() { return 0x382; }
        public override int GetAttackSound() { return 0x384; }
        public override int GetHurtSound() { return 0x385; }
        public override int GetDeathSound() { return 0x386; }

        public DeepTentacle(Serial serial): base(serial)
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