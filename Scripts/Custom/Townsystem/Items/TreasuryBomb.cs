using Server;
using Server.Targeting;
using System;

namespace Server.Custom.Townsystem
{
    public class TreasuryBombPlans : Item
    {
        public static readonly int TreasuryCost = 2500;
        public override string DefaultName { get { return "Treasury Bomb Plans"; } }

        [Constructable]
        public TreasuryBombPlans()
            : base(0x14F0)
        {
        }

        public TreasuryBombPlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TreasuryBomb : Item
    {
        private BombTimer m_Timer;

        [Constructable]
        public TreasuryBomb()
            : base(0x5736)
        {
            Hue = 1000;
            Name = "Treasury Bomb";
        }

        public TreasuryBomb(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent != from.Backpack)
            {
                from.SendMessage("This must be in your pack to use it!");
            }
            else if (Faction.Find(from) == null)
            {
                from.SendMessage("You must be in a faction to use this item.");
            }
            else
            {
                from.Target = new BombTarget(this);
            }
            
        }

        public static void OnTarget(TreasuryBomb bomb, Mobile from, TreasuryWall wall)
        {
            if (from == null || wall == null || bomb == null)
                return;
            Town town = wall.m_Town;
            Town attackTown = Town.Find(from);
			Town defendTown = wall.m_Town;
			Town ownerTown = wall.m_Town.ControllingTown;

            if (town == null)
                return;

            if (attackTown == defendTown)
            {
                from.SendMessage("You cannot attack an ally treasury!");
            }
            else if (ownerTown == defendTown)
            {
                from.SendMessage("The treasury is currently protected from attack.");
            }
			else if (town.HomeFaction == null || town.ControllingTown == null || town == town.ControllingTown)
            {
                from.SendMessage("The town treasury is currently protected from attack.");
            }
            else
            {
                bomb.MoveToWorld(wall.Location, Map.Felucca);
                bomb.Movable = false;
                bomb.m_Timer = new BombTimer(bomb, wall);
                bomb.m_Timer.Start();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            base.OnAfterDelete();
        }

        public void OnExplode(TreasuryWall wall)
        {
            Effects.SendLocationEffect(new Point3D(Location.X, Location.Y, Location.Z+5), Map, 0x36CB, 10);
            bool toDelete = false;

            foreach (TreasuryWall tw in wall.m_Town.TreasuryWalls)
                if (--tw.Hits <= 0)
                    toDelete = true;

            Delete();

            if (toDelete)
                wall.m_Town.SetTreasuryWalls(TreasuryWallTypes.None);
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

        private class BombTarget : Target
        {
            TreasuryBomb m_Bomb;

            public BombTarget(TreasuryBomb bomb)
                : base(3, false, TargetFlags.None)
            {
                m_Bomb = bomb;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is TreasuryWall))
                {
                    from.SendMessage("You cannot blow that up.");
                    return;
                }

                TreasuryBomb.OnTarget(m_Bomb, from, ((TreasuryWall)targeted));
            }
        }

        private class BombTimer : Timer
        {
            TreasuryBomb m_Bomb;
            TreasuryWall m_Wall;
            int count = 15;

            public BombTimer(TreasuryBomb bomb, TreasuryWall wall)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            {
                m_Bomb = bomb;
                m_Wall = wall;
            }

            protected override void OnTick()
            {
                if (count <= 0)
                {
                    m_Bomb.OnExplode(m_Wall);
                    Stop();
                    return;
                }
                m_Bomb.PublicOverheadMessage(Network.MessageType.Regular, 0x24, true, (count--).ToString());
            }
        }
    }
}
