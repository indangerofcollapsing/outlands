using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ImmortalFlameWaterBucket : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ImmortalFlameBoss Boss { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Home { get; set; }
        

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

        public bool IsHome
        {
            get
            {
                return Location == Home;
            }
            set
            {
                MoveToWorld(Home);
            }
        }

        public override string DefaultName { get { return "a water bucket"; } }

        public ImmortalFlameWaterBucket(ImmortalFlameBoss boss)
            : base(0xFFA)
        {
            Boss = boss;
        }

        public override void OnDoubleClick(Mobile from)
        {
            //var instructions = from.Backpack.FindItemByType(typeof(DousingInstructions));

            //if (instructions == null)
            //{
            //    from.SendMessage("You aren't sure how to use this...");
            //}   
            //else if (!IsChildOf(from.Backpack))
            //{
            //    from.SendMessage("That must be in your pack to use it.");
            //}
            //else
            //{
                from.SendMessage("What would you like to use this on?");
                from.Target = new InternalTarget(this);
            //}
        }

        public override bool CheckLift(Mobile from, Item item, ref Network.LRReason reject)
        {
            bool ret = base.CheckLift(from, item, ref reject);

            if (ret)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(30), Delete);
            }

            return ret;
        }

        public ImmortalFlameWaterBucket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((Point3D)Home);
            writer.Write((Mobile)Boss);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Home = reader.ReadPoint3D();
            Boss = reader.ReadMobile() as ImmortalFlameBoss;
        }

        public class InternalTarget : Target
        {
            private ImmortalFlameWaterBucket m_Bucket;

            public InternalTarget(ImmortalFlameWaterBucket bucket)
                : base(3, false, TargetFlags.None)
            {
                m_Bucket = bucket;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Bucket == null || m_Bucket.Deleted)
                    return;

                ImmortalFlameBoss boss = targeted as ImmortalFlameBoss;

                if (boss != null && boss == m_Bucket.Boss)
                {
                    from.SendMessage("You empty the bucket on the flames.");
                    m_Bucket.Consume();
                    boss.BucketUsed(m_Bucket);
                }
            }
        }
    }
}
