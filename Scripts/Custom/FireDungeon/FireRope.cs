using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Server.Items;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom
{
    public static class FireRope
    {
        public static void Initialize()
        {
            CommandSystem.Register("AddFireRope", AccessLevel.GameMaster, new CommandEventHandler(AddFireRope_OnCommand));
        }

        public static void AddFireRope_OnCommand(CommandEventArgs args)
        {
            args.Mobile.SendMessage("Target the start point.");
            args.Mobile.Target = new FireRopeTarget1();
        }

        public static Point3D RopeStartLocation { get { return new Point3D(5746, 1441, 2); } }
        public static Point3D RopeEndLocation { get { return new Point3D(5746, 1458, 2); } }

        public static FireRopeController GenerateItem(Map map)
        {
            var rope = new FireRopeController(RopeStartLocation, RopeEndLocation);
            rope.MoveToWorld(RopeStartLocation, map);
            return rope;
        }

        private class FireRopeTarget1 : Target
        {
            public FireRopeTarget1()
                : base(20, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D start = targeted as IPoint3D;
                from.SendMessage("Target the final point.");
                if (start != null)
                    from.Target = new FireRopeTarget2(new Point3D(start.X, start.Y, start.Z));
            }
        }

        private class FireRopeTarget2 : Target
        {
            Point3D m_Start;

            public FireRopeTarget2(Point3D start)
                : base(20, true, TargetFlags.None)
            {
                m_Start = start;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D end = targeted as IPoint3D;

                if (end != null)
                {
                    var ropeController = new FireRopeController(m_Start, new Point3D(end.X, end.Y, end.Z));
                    ropeController.MoveToWorld(m_Start, from.Map);
                }
            }
        }
    }

    

    public class FireRopeController : Item
    {
        public override string DefaultName { get { return "fire rope controller"; } }

        public List<Item> Flames { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Start { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D End { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SecondsBetweenFlip { get; set; }

        private string m_Pattern;
        [CommandProperty(AccessLevel.GameMaster)]
        public string Pattern { get { return m_Pattern; } set { m_Pattern = value; SetupFlame(); } }

        private int m_Damage;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Damage { get { return m_Damage; } set { m_Damage = value; SetupFlame(); } }

        private bool m_Horizontal;

        private InternalTimer m_Timer;

        public FireRopeController(Point3D start, Point3D end)
            : base(0x1F1C)
        {
            Visible = false;
            Movable = false;
            Start = start;
            End = end;
            Flames = new List<Item>();
            m_Damage = 40;
            m_Pattern = "111000";
            SecondsBetweenFlip = 30;
            Timer.DelayCall(TimeSpan.FromTicks(1), SetupFlame);
        }

        public void SetupFlame()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
            }

            while (Flames.Count > 0)
            {
                var flame = Flames[0];
                Flames.RemoveAt(0);
                flame.Delete();
            }

            if (Start.X == End.X) // Horizontal
            {
                m_Horizontal = true;
            }
            else if (Start.Y == End.Y)
            {
                m_Horizontal = false;
            }
            else
            {
                Delete();
                return;
            }

            int xOffset = m_Horizontal ? 0 : 1;
            int yOffset = m_Horizontal ? 1 : 0;

            int count = m_Horizontal ? End.Y - Start.Y : End.X - Start.X;

            if (count <= 0)
            {
                Delete();
                return;
            }

            if (Pattern != null && Pattern.Length > 0)
            {
                FireRopeFlame flame;
                for (int i = 0; i < count; i++)
                {
                    if (String.Equals(Pattern[i % Pattern.Length], '1'))
                    {
                        flame = new FireRopeFlame(m_Damage);
                        flame.MoveToWorld(new Point3D(Start.X + xOffset * i, Start.Y + yOffset * i, Start.Z), Map);
                        Flames.Add(flame);
                    }
                }

                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        public void Shift(bool reverse)
        {
            //Console.WriteLine("Shift: " + reverse.ToString());
            int x, y;

            foreach (Item flame in Flames)
            {
                if (m_Horizontal)
                {
                    y = flame.Y;
                    if (reverse)
                    {
                        if (y - 1 < Start.Y)
                            flame.Y = End.Y;
                        else
                            flame.Y = y - 1;
                    }
                    else
                    {
                        if (y + 1 > End.Y)
                            flame.Y = Start.Y;
                        else
                            flame.Y = y + 1;
                    }
                }
                else
                {
                    x = flame.X;

                    if (reverse)
                    {
                        if (x - 1 < Start.X)
                            flame.X = End.X;
                        else
                            flame.X = x - 1;
                    }
                    else
                    {
                        if (x + 1 > End.X)
                            flame.X = Start.X;
                        else
                            flame.X = x + 1;
                    }

                }
            }
        }

        public FireRopeController(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            if (Flames != null)
            {
                while (Flames.Count > 0)
                {
                    var flame = Flames[0];
                    Flames.RemoveAt(0);
                    flame.Delete();
                }
            }

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_Damage);

            writer.Write(Flames.Count);
            foreach (FireRopeFlame flame in Flames)
                writer.Write(flame);

            writer.Write(Start);
            writer.Write(End);
            writer.Write(m_Horizontal);
            writer.Write(SecondsBetweenFlip);
            writer.Write(Pattern);

        }

        public override void Deserialize(GenericReader reader)
        {

            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Damage = reader.ReadInt();
                    goto case 0;
                case 0:
                    int count = reader.ReadInt();
                    Flames = new List<Item>();
                    for (int i = 0; i < count; i++)
                    {
                        var item = reader.ReadItem();
                        if (item != null)
                            Flames.Add(item);
                    }

                    Start = reader.ReadPoint3D();
                    End = reader.ReadPoint3D();
                    m_Horizontal = reader.ReadBool();
                    SecondsBetweenFlip = reader.ReadInt();
                    Pattern = reader.ReadString();
                    break;
            }

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public class InternalTimer : Timer
        {
            private FireRopeController m_Controller;
            private int m_Tally;
            private bool m_Reverse;

            public InternalTimer(FireRopeController controller)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Controller = controller;
            }

            protected override void OnTick()
            {
                if (m_Controller == null || m_Controller.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Controller.SecondsBetweenFlip > 0 && ++m_Tally >= m_Controller.SecondsBetweenFlip)
                {
                    m_Tally = 0;
                    m_Reverse = !m_Reverse;
                }

                m_Controller.Shift(m_Reverse);
            }
        }
    }

    public class FireRopeFlame : Item
    {
        public override string DefaultName { get { return "flame"; } }

        public int Damage { get; set; }

        private Timer m_Timer;

        public FireRopeFlame(int damage)
            : base(6571)
        {
            Damage = damage;

            Movable = false;
            Light = LightType.Circle300;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature)
            {
                var bcMob = m as BaseCreature;
                if (!bcMob.Controlled)
                    return true;
            }

            int damage = Damage;

            if (!Core.AOS && m.CheckSkill(SkillName.MagicResist, 0.0, 30.0))
            {
                damage /= 2;

                m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
            }

            AOS.Damage(m, null, damage, 0, 100, 0, 0, 0);
            m.PlaySound(0x208);

            return true;
        }

        public FireRopeFlame(Serial serial)
            : base(serial)
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

        private class InternalTimer : Timer
        {
            private FireRopeFlame m_Item;

            private static Queue m_Queue = new Queue();

            public InternalTimer(FireRopeFlame item)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                m_Item = item;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Item.Deleted)
                    return;

                Map map = m_Item.Map;

                if (map != null)
                {
                    var mobiles = m_Item.GetMobilesInRange(0);
                    foreach (Mobile m in mobiles)
                    {
                        if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z)
                            m_Queue.Enqueue(m);
                    }
                    mobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile m = (Mobile)m_Queue.Dequeue();
                        if (m is BaseCreature)
                        {
                            var bcMob = m as BaseCreature;
                            if (!bcMob.Controlled)
                                continue;
                        }

                        int damage = m_Item.Damage;

                        if (!Core.AOS && m.CheckSkill(SkillName.MagicResist, 0.0, 30.0))
                        {
                            damage /= 2;

                            m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                        }

                        AOS.Damage(m, null, damage, 0, 100, 0, 0, 0);
                        m.PlaySound(0x208);
                    }
                }
            }
        }
    }
}
