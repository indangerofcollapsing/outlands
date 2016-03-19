using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShitcodeFragment2 : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromHours(1);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public ShitcodeFragment2(): base(2325)
        {
            Name = "Shitcode Fragment: Version 2.0";
            Hue = 2587;
        }

        public ShitcodeFragment2(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (ParentEntity != null)
            {
                from.SendMessage("That may not be used while inside a container.");
                return;
            }

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (m_NextUseAllowed > DateTime.UtcNow && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You may only use this item once every 60 minutes.");
                return;
            }   

            StartItemAction(player);
        }

        public void StartItemAction(PlayerMobile player)
        {
            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = Location;
            Map targetMap = Map;

            targetLocation.Z += 2;

            TimedStatic shit = new TimedStatic(2496, 30);
            shit.Name = "shitcode";
            shit.Hue = 2110;
            shit.MoveToWorld(targetLocation, map);

            List<string> m_Messages = new List<string>();

            m_Messages.Add("COMPILING...");
            m_Messages.Add("COMPILING...");
            m_Messages.Add("COMPILING...");
            m_Messages.Add("Queue m_Queue = new Queue();");
            m_Messages.Add("if (item is UOACZSpawner || item is UOACZStatic ||");
            m_Messages.Add("item is UOACZBreakableStatic || item is UOACZWayPoint ||");
            m_Messages.Add("item is UOACZMoongate || item is UOACZDestination || item is Teleporter || item is LOSBlocker ||");
            m_Messages.Add("item is UOACZConstructionTile || item is UOACZConstructionObjectEffectTargeter || item is UOACZTunnel ||");
            m_Messages.Add("item is UOACZSpawnRedirector || item is UOACZSpawnAreaBlocker");
            m_Messages.Add("m_Queue.Enqueue(item);");
            m_Messages.Add("ERROR");
            m_Messages.Add("ERROR");
            m_Messages.Add("ERROR");
            m_Messages.Add("ERROR");
            m_Messages.Add("ERROR");
            m_Messages.Add("ERROR");           

            for (int a = 0; a < m_Messages.Count; a++)
            {
                string nextMessage = m_Messages[a];
                
                Timer.DelayCall(TimeSpan.FromSeconds(a * .5), delegate
                {
                    if (this == null) return;
                    if (Deleted) return;

                    if (Location != location)
                    {
                        if (shit != null)
                        {
                            if (!shit.Deleted)
                                shit.Delete();
                        }

                        return;
                    }

                    Effects.PlaySound(Location, Map, 0x428);
                    PublicOverheadMessage(Network.MessageType.Emote, Hue, false, nextMessage);

                    Point3D newLocation = new Point3D(location.X + Utility.RandomList(-1, 1), location.Y + Utility.RandomList(-1, 1), location.Z);
                    SpellHelper.AdjustField(ref newLocation, map, 12, false);
                    
                    TimedStatic shitcodeResidue = new TimedStatic(Utility.RandomList(4651, 4652, 4653, 4654), 30);
                    shitcodeResidue.Name = "shitcode error";
                    shitcodeResidue.Hue = 1190;
                    shitcodeResidue.MoveToWorld(newLocation, map);
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
