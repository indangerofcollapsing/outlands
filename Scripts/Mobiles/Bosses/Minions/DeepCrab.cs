using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;

using Server.Multis;

namespace Server.Mobiles
{
    [CorpseName("a deep crab corpse")]
    public class DeepCrab : BaseCreature
    {
        public DateTime m_NextBoatChewAllowed;
        public TimeSpan NextBoatChewDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        [Constructable]
        public DeepCrab(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deep crab";

            Body = 729;
            Hue = 2600;

            BaseSoundID = 0x616;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(6, 12);
            
            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;

            Fame = 500;
            Karma = 0;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

        }

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool IsOceanCreature { get { return true; } }

        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }
        public override bool HasAlternateHighSeasHurtAnimation { get { return true; } }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 50;

            UniqueCreatureDifficultyScalar = 2.0;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > m_NextBoatChewAllowed && BoatOccupied != null)
            {
                if (BoatOccupied.Deleted || BoatOccupied.m_SinkTimer != null) return;                

                Say("*chews on the ship*");
                
                SpecialAbilities.HinderSpecialAbility(1.0, this, this, 1.0, 3, true, 0, false, "", "", "-1");
                
                m_NextBoatChewAllowed = DateTime.UtcNow + NextBoatChewDelay;

                Point3D location = Location;
                Map map = Map;

                for (int a = 0; a < 3; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;
                        
                        LastCombatTime = LastCombatTime + TimeSpan.FromSeconds(1);

                        BoatOccupied.ReceiveDamage(this, null, Utility.RandomMinMax(5, 10), DamageType.Hull);

                        Animate(1, 6, 1, true, false, 0);
                        Effects.PlaySound(location, map, Utility.RandomList(0x134, 0x133));
                    });
                }
            }
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
        
        public DeepCrab(Serial serial): base(serial)
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