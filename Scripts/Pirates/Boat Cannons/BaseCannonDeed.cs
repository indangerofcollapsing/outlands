/***************************************************************************
 *                             BaseCannonDeed.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.Items;

namespace Server.Custom.Pirates
{
    public enum CannonTypes { Light, Heavy }

    public abstract class BaseCannonDeed : Item
    {
        public abstract BaseCannon Addon { get; }
        public abstract CannonTypes CannonType { get; }

        public BaseCannonDeed()
            : base(0x14F2)
        {
            Weight = 1.0;
        }

        public BaseCannonDeed(Serial serial)
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

            if (Weight == 0.0)
                Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private BaseCannonDeed m_Deed;

            public InternalTarget(BaseCannonDeed deed)
                : base(-1, true, TargetFlags.None)
            {
                m_Deed = deed;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is IPoint3D))
                    return;

                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || m_Deed.Deleted)
                    return;

                if (m_Deed.IsChildOf(from.Backpack))
                {
                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);
                    BaseBoat boat = BaseBoat.FindBoatAt(p, map);                   

                    if (boat != null)
                    {                        
                        bool res = (boat.CannonFit(m_Deed.CannonType, p) && !boat.HasCannonAt(new Point3D(p.X, p.Y, p.Z)));

                        if (!(boat.Owner == from))
                            from.SendMessage("Only the owner of the boat may place cannons.");
                        else if (res)
                        {
                            from.Say("Placing cannon at: " + p.X.ToString() + " , " + p.Y.ToString() + " , " + p.Z.ToString());
                                
                            m_Deed.Delete();
                            BaseCannon addon = m_Deed.Addon;
                            addon.Visible = false;
                            addon.m_Boat = boat;
                            addon.MoveToWorld(new Point3D(p.X, p.Y, p.Z + 2), map);
                            addon.SetFacing(boat.Facing);
                            addon.Visible = true;
                            boat.AddCannon(addon);
                        }

                        else
                        {
                            from.SendLocalizedMessage(500269); // You cannot build that there.
                        }
                       
                    }
                    else
                        from.SendMessage("You must target a boat location.");
                }
                else
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }

    public abstract class BaseCannonPlans : Item
    {
        public BaseCannonPlans()
            : base(0x14F2)
        {
            Weight = 1.0;
        }

        public BaseCannonPlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadInt();
                reader.ReadInt();
                reader.ReadInt();
                reader.ReadInt();
            }
        }
    }
}
