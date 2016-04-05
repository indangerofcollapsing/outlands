using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells;


namespace Server
{
    public class UOACZTunnel : Item
    {
        public static List<UOACZTunnel> m_Instances = new List<UOACZTunnel>();

        public enum TunnelLocation
        {
            Town,
            Wilderness
        }

        private TunnelLocation m_TunnelType = TunnelLocation.Town;
        [CommandProperty(AccessLevel.GameMaster)]
        public TunnelLocation TunnelType
        {
            get { return m_TunnelType; }
            set { m_TunnelType = value; }
        }

        [Constructable]
        public UOACZTunnel(): base(7025)
        {
            Name = "a tunnel";

            Hue = 2405;      

            Movable = false;
            Visible = false;              

            m_Instances.Add(this);
        }

        public UOACZTunnel(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (from.AccessLevel > AccessLevel.Player)
                LabelTo(from, "(" + m_TunnelType.ToString() + ")");

            else
            {
                if (m_TunnelType == TunnelLocation.Town)
                    from.SendMessage("Using this tunnel will lead to the wilderness.");
                else
                    from.SendMessage("Using this tunnel will lead to town.");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null) return;
            if (!Visible) return;
            if (player.AccessLevel > AccessLevel.Player) return;
            
            if (Utility.GetDistance(from.Location, Location) > 1)
            {
                from.SendMessage("You are too far way from that to use it.");
                return;
            }

            if (player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold > DateTime.UtcNow)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold, false, true, true, true, true);

                player.SendMessage("You have been in combat with another player too recently and must wait " + timeRemaining + " before you may use this a tunnel.");
                return;
            }
            
            List<UOACZTunnel> m_Destinations = new List<UOACZTunnel>();

            foreach (UOACZTunnel tunnel in m_Instances)
            {
                if (tunnel == null) continue;
                if (tunnel.Deleted) continue;
                if (!UOACZRegion.ContainsItem(tunnel)) continue;               
                if (TunnelType == TunnelLocation.Town && tunnel.TunnelType == TunnelLocation.Town) continue;
                if (TunnelType == TunnelLocation.Wilderness && tunnel.TunnelType == TunnelLocation.Wilderness) continue;

                m_Destinations.Add(tunnel);
            }

            if (m_Destinations.Count == 0)
                return;
                        
            Effects.PlaySound(Location, Map, 0x247);
            Effects.SendLocationEffect(Location, Map, 0x3728, 10, 10, 0, 0);

            TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
            dirt.Name = "dirt";
            dirt.MoveToWorld(Location, Map);
            dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*goes into tunnel*");

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            player.m_UOACZAccountEntry.TunnelsUsed++;                   

            for (int b = 0; b < 8; b++)
            {
                dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                dirt.Name = "dirt";

                Point3D dirtLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);
                SpellHelper.AdjustField(ref dirtLocation, from.Map, 12, false);

                dirt.MoveToWorld(dirtLocation, Map);
            }

            UOACZTunnel targetTunnel = m_Destinations[Utility.RandomMinMax(0, m_Destinations.Count - 1)];            
            
            Visible = false;

            player.Location = targetTunnel.Location;
            
            foreach (Mobile follower in player.AllFollowers)
            {
                if (UOACZSystem.IsUOACZValidMobile(follower))
                    follower.Location = targetTunnel.Location;
            }

            if (targetTunnel.TunnelType == TunnelLocation.Wilderness)
                from.SendMessage("The tunnel collapses behind you and you find yourself in the wilderness.");

            else
                from.SendMessage("The tunnel collapses behind you and you find yourself in town.");

            Effects.SendLocationEffect(targetTunnel.Location, targetTunnel.Map, 0x3728, 10, 10, 0, 0); 
            Effects.PlaySound(targetTunnel.Location, targetTunnel.Map, 0x247);

            dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
            dirt.Name = "dirt";
            dirt.MoveToWorld(targetTunnel.Location, Map);
            dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*appears from tunnel*");
                        
            for (int b = 0; b < 8; b++)
            {
                dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                dirt.Name = "dirt";

                Point3D dirtLocation = new Point3D(targetTunnel.Location.X + Utility.RandomList(-2, -1, 1, 2), targetTunnel.Location.Y + Utility.RandomList(-2, -1, 1, 2), targetTunnel.Location.Z);
                SpellHelper.AdjustField(ref dirtLocation, Map, 12, false);                

                dirt.MoveToWorld(dirtLocation, Map);
            }
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write((int)m_TunnelType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_TunnelType = (TunnelLocation)reader.ReadInt();
            }

            //-------

            m_Instances.Add(this);
        }
    }
}