/***************************************************************************
 *                            FireableCannon.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Multis;

namespace Server.Items
{
    public interface ICannonDamage
    {
        int HitsMax { get; set; }
        int Hits { get; set; }
        void Damage(Mobile from, int Damage);
        void OnDamaged();
    }

    public class CannonComponent : AddonComponent, ICannonDamage
    {
        private static int MinDamage = 20;//Minimum Damage done with a direct hit
        private static int MaxDamage = 40;//Minimum Damage done with a direct hit
        private BaseCannon m_cannon;
        public DateTime NextShot = DateTime.UtcNow;
        public override bool HandlesOnSpeech { get { return true; } }

        #region ICannonDamage vars
        private int m_Hits, m_HitsMax;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits
        {
            get { return m_Hits; }
            set
            {
                if (value > m_HitsMax)
                    m_Hits = m_HitsMax;
                else
                    m_Hits = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitsMax { get { return m_HitsMax; } set { m_HitsMax = value; } }
        #endregion


        public BaseCannon cannon { get { return m_cannon; } }

        public override bool ForceShowProperties { get { return true; } }

        public CannonComponent(int itemID, BaseCannon c)
            : base(itemID)
        {
            m_cannon = c;
            m_HitsMax = 100;
            m_Hits = 100;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from != m_cannon.Owner || !from.InRange(this, 3))
                return;
            string said = e.Speech.ToLower();

        }

        #region ICannonDamage methods
        public void Damage(Mobile from, int damage)
        {
            if (from.Player)
            {
                m_Hits -= damage;
                bool newPacket = true;
                if (newPacket)
                    from.Send(new DamagePacket(from, damage));
                else
                    from.Send(new DamagePacketOld(from, damage));

                if (m_cannon.Owner != null)
                {
                    if (m_cannon.Owner is TurnableCannonGuard)
                        ((TurnableCannonGuard)m_cannon.Owner).OnHarmfulAction(from, false);
                    else if (m_cannon.Owner is DoubloonDockGuard)
                        ((DoubloonDockGuard)m_cannon.Owner).OnHarmfulAction(from, false);
                    else
                        m_cannon.Owner.OnHarmfulAction(from, false);
                }
                from.CriminalAction(true);
                OnDamaged();
            }
        }

        public void OnDamaged()
        {
            if (m_Hits <= 0)
            {
                Effects.PlaySound(Location, Map, 0x207);
                Effects.SendLocationEffect(Location, Map, 0x36BD, 20);
                foreach (AddonComponent a in m_cannon.Components)
                    Effects.SendMovingEffect(this, new Entity(Serial.Zero, new Point3D(X + Utility.RandomMinMax(-6, 6), Y + Utility.RandomMinMax(-6, 6), Z + Utility.RandomMinMax(10, 20)), Map), a.ItemID, 1, 0, false, true);
                Delete();
                if (m_cannon.Owner != null)
                    m_cannon.Owner.Kill();
            }
        }
        #endregion

        /*public override void OnDoubleClick( Mobile from )
		{
			if( from.AccessLevel > AccessLevel.Counselor ) // No checks for staff
			{
				from.Target = new InternalTarget(this);
				return;
			}
			if ( from != m_cannon.Owner )
			{
				from.SendLocalizedMessage( 500364 ); // You can't use that, it belongs to someone else.
				return;
			}
			if( from.Mounted )
			{
				from.SendLocalizedMessage( 1010097 ); // You cannot use this while mounted.
				return;
			}
			if( !from.InRange( m_cannon.Location, 3 ) )
			{
				from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
				return;
			}
			if( NextShot < DateTime.UtcNow )
				from.Target = new InternalTarget(this);
			else
				from.SendMessage( "The cannon is too hot! You must wait {0} seconds before firing again.", ((TimeSpan)(NextShot - DateTime.UtcNow)).Seconds );
		}*/

        public bool CheckFiringAngle(Point3D p, Mobile from)
        {
            //if ( !from.InLOS(p) )
            //{
            //	from.SendLocalizedMessage( 500237 ); // Target cannot be seen.
            //	return false;
            //}
            bool los = false;
            Point3D loc = this.Location;
            int x = p.X - loc.X;
            int y = p.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            //switch (m_cannon.Direction)
            //{
            //case Direction.North: { if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.7) los = true; } break;
            //case Direction.East: { if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.52) los = true; } break;
            //case Direction.South: { if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.52) los = true; } break;
            //case Direction.West: { if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.52) los = true; } break;
            // }
            los = true;
            if (los)
            {
                Point3D p2 = new Point3D(Location.X, Location.Y, Location.Z + 10);
                if (from.InRange(this.Location, 5))
                {
                    IEntity to;
                    to = new Entity(Serial.Zero, new Point3D(p.X, p.Y, p.Z + 5), from.Map);
                    IEntity fro;
                    fro = new Entity(Serial.Zero, p2, from.Map);
                    Effects.SendMovingEffect(fro, to, 0xE73, 1, 0, false, true, 0, 0);
                    Effects.PlaySound(Location, from.Map, 519);
                    Explode(from, new Point3D(p), from.Map);
                    NextShot = DateTime.UtcNow + TimeSpan.FromSeconds(10);// 10 seconds to next time you can fire
                    return true;
                }
                else
                    from.Say("You are too far from cannon!");
            }
            else
                from.SendLocalizedMessage(500237); // Target cannot be seen.
            return false;
        }

        public void Explode(Mobile from, Point3D loc, Map map)
        {

            if (map == null)
                return;

            Effects.PlaySound(loc, map, 0x207);
            Effects.SendLocationEffect(loc, map, 0x36BD, 20);


            IPooledEnumerable eable = map.GetObjectsInRange(loc, 2);
            ArrayList toExplode = new ArrayList();

            int toDamage = 0;

            foreach (object o in eable)
            {
                if (o is Mobile)
                {
                    toExplode.Add(o);
                    ++toDamage;
                }
                else if (o is BaseExplosionPotion && o != this)
                {
                    toExplode.Add(o);
                }
                else if (o is ICannonDamage)
                {
                    toExplode.Add(o);
                }
            }

            BaseBoat boat = BaseBoat.FindBoatAt(loc, map);
            if (boat != null)
                toExplode.Add(boat);

            eable.Free();
            int d = 0; // Damage scalar
            int damage = 0;
            for (int i = 0; i < toExplode.Count; ++i)
            {
                object o;
                o = toExplode[i];

                if (o is Mobile)
                {
                    Mobile m = (Mobile)o;
                    if (m.InRange(loc, 0))
                        d = 1;
                    else if (m.InRange(loc, 1))
                        d = 2;
                    else if (m.InRange(loc, 2))
                        d = 3;
                    if (from != null || (SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false)))
                    {
                        if (from != null)
                            from.DoHarmful(m);
                        damage = Utility.RandomMinMax((MinDamage / d), (MaxDamage / d));
                        if (d == 1)
                            AOS.Damage(m, from, damage, 50, 50, 0, 0, 0); // Same tile 50% physical 50% fire
                        else
                            AOS.Damage(m, from, damage, 0, 100, 0, 0, 0); // 2 tile radius 100% fire damage
                    }
                }

                else if (o is BaseExplosionPotion)
                {
                    BaseExplosionPotion pot = (BaseExplosionPotion)o;
                    pot.Explode(from, false, pot.GetWorldLocation(), pot.Map);
                }

                else if (o is ICannonDamage)
                {
                    ((ICannonDamage)o).Damage(from, Utility.RandomMinMax(MinDamage, MaxDamage));
                }

                else if (o is BaseBoat)
                {
                    ((BaseBoat)o).ReceiveDamage(from, null, Utility.RandomMinMax(MinDamage, MaxDamage), boat.GetRandomDamageType());
                }
            }
        }

        public CannonComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_cannon);
            writer.Write(m_Hits);
            writer.Write(m_HitsMax);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_cannon = reader.ReadItem() as BaseCannon;
            m_Hits = reader.ReadInt();
            m_HitsMax = reader.ReadInt();
        }


        private class InternalTarget : Target
        {
            private CannonComponent m_comp;

            public InternalTarget(CannonComponent comp)
                : base(20, true, TargetFlags.None)
            {
                m_comp = comp;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;
                m_comp.CheckFiringAngle(new Point3D(p), from);
            }
        }
    }

    public class BaseCannon : BaseAddon
    {
        private Mobile m_Owner;
        private CannonComponent m_CCom;

        public new virtual BaseCannonDeed Deed { get { return null; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonComponent CCom
        {
            get { return m_CCom; }
            set { m_CCom = value; }
        }

        public BaseCannon(Direction dir)
            : this(null, dir)
        {
        }

        public BaseCannon(Mobile owner, Direction dir)
        {
            m_Owner = owner;
            Weight = 400;
            Direction = dir;
            switch (Direction)
            {
                case Direction.North: //North
                    {
                        AddComponent(new AddonComponent(0xE8D), 0, 0, 0);
                        AddCannonComponent(0xE8C, 0, 1, 0);
                        AddComponent(new AddonComponent(0xE8B), 0, 2, 0); break;
                    }
                case Direction.South: //South
                    {
                        AddComponent(new AddonComponent(0xE93), 0, 0, 0);
                        AddComponent(new AddonComponent(0xE92), 0, 1, 0);
                        AddCannonComponent(0xE91, 0, 2, 0); break;
                    }
                case Direction.East: //East
                    {
                        AddComponent(new AddonComponent(0xE94), 0, 0, 0);
                        AddComponent(new AddonComponent(0xE95), 1, 0, 0);
                        AddCannonComponent(0xE96, 2, 0, 0); break;
                    }
                case Direction.West: //West
                    {
                        AddComponent(new AddonComponent(0xE8E), 0, 0, 0);
                        AddCannonComponent(0xE8F, 1, 0, 0);
                        AddComponent(new AddonComponent(0xE90), 2, 0, 0); break;
                    }
            }
        }

        private void AddCannonComponent(int itemID, int x, int y, int z)
        {
            AddonComponent component = new CannonComponent(itemID, this);
            component.Name = "cannon";
            m_CCom = (CannonComponent)component;
            AddComponent(component, x, y, z);
        }

        public BaseCannon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Owner);
            writer.Write(m_CCom);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Owner = reader.ReadMobile();
            m_CCom = (CannonComponent)reader.ReadItem();
        }
    }

    #region Cannons and deeds
    public class CannonNorth : BaseCannon
    {
        public override BaseCannonDeed Deed { get { return new CannonNorthDeed(); } }

        [Constructable]
        public CannonNorth()
            : this(null)
        {
        }

        public CannonNorth(Mobile owner)
            : base(owner, Direction.North)
        {
        }

        public CannonNorth(Serial serial)
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

    public class CannonNorthDeed : BaseCannonDeed
    {

        public override BaseCannon FireCannon { get { return new CannonNorth(); } }

        private int m_Hits = 200;
        private int m_HitsMax = 200;
        public override int Hits { get { return m_Hits; } set { m_Hits = value; } }
        public override int HitsMax { get { return m_HitsMax; } set { m_HitsMax = value; } }

        [Constructable]
        public CannonNorthDeed()
            : base()
        {
            Name = "Cannon North Deed";
        }

        public CannonNorthDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class CannonSouth : BaseCannon
    {
        public override BaseCannonDeed Deed { get { return new CannonSouthDeed(); } }

        [Constructable]
        public CannonSouth()
            : this(null)
        {
        }

        public CannonSouth(Mobile owner)
            : base(owner, Direction.South)
        {
        }

        public CannonSouth(Serial serial)
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

    public class CannonSouthDeed : BaseCannonDeed
    {
        public override BaseCannon FireCannon { get { return new CannonSouth(); } }

        private int m_Hits = 100;
        private int m_HitsMax = 100;
        public override int Hits { get { return m_Hits; } set { m_Hits = value; } }
        public override int HitsMax { get { return m_HitsMax; } set { m_HitsMax = value; } }

        [Constructable]
        public CannonSouthDeed()
            : base()
        {
            Name = "Cannon South Deed";
        }

        public CannonSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class CannonEast : BaseCannon
    {
        public override BaseCannonDeed Deed { get { return new CannonEastDeed(); } }

        [Constructable]
        public CannonEast()
            : this(null)
        {
        }

        public CannonEast(Mobile owner)
            : base(owner, Direction.East)
        {
        }

        public CannonEast(Serial serial)
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

    public class CannonEastDeed : BaseCannonDeed
    {
        public override BaseCannon FireCannon { get { return new CannonEast(); } }

        private int m_Hits = 100;
        private int m_HitsMax = 100;
        public override int Hits { get { return m_Hits; } set { m_Hits = value; } }
        public override int HitsMax { get { return m_HitsMax; } set { m_HitsMax = value; } }

        [Constructable]
        public CannonEastDeed()
            : base()
        {
            Name = "Cannon East Deed";
        }

        public CannonEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class CannonWest : BaseCannon
    {
        public override BaseCannonDeed Deed { get { return new CannonWestDeed(); } }

        [Constructable]
        public CannonWest()
            : this(null)
        {
        }

        public CannonWest(Mobile owner)
            : base(owner, Direction.West)
        {
        }

        public CannonWest(Serial serial)
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

    public class CannonWestDeed : BaseCannonDeed
    {
        public override BaseCannon FireCannon { get { return new CannonWest(); } }

        private int m_Hits = 100;
        private int m_HitsMax = 100;
        public override int Hits { get { return m_Hits; } set { m_Hits = value; } }
        public override int HitsMax { get { return m_HitsMax; } set { m_HitsMax = value; } }

        [Constructable]
        public CannonWestDeed()
            : base()
        {
            Name = "Cannon West Deed";
        }

        public CannonWestDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
    #endregion
}
