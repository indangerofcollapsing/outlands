using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShitcodeFragment1 : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromHours(1);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public ShitcodeFragment1(): base(2325)
        {
            Name = "Shitcode Fragment: Version 1.0";
            Hue = 2540;
        }

        public ShitcodeFragment1(Serial serial): base(serial)
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
            m_Messages.Add("if (m_Owner.bc_Creature.PlayerRangeSensitive)");
            m_Messages.Add("Sector sect = m_Owner.bc_Creature.Map.GetSector(m_Owner.bc_Creature);");
            m_Messages.Add("if (!sect.Active)");
            m_Messages.Add("if (m_Owner.bc_Creature.LastActivated + m_Owner.DeactivateDelay < DateTime.UtcNow)");
            m_Messages.Add("m_Owner.Deactivate();");
            m_Messages.Add("return;");
            m_Messages.Add("m_Owner.bc_Creature.LastActivated = DateTime.UtcNow;");
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
