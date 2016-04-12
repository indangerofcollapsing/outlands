using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public class DeDOSDataCrystal : Item
    {
        public static List<DeDOSDataCrystal> m_Instances = new List<DeDOSDataCrystal>();

        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromSeconds(30);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        public static int TextHue = 2603;
        public static int DataHue = 0x3F;

        private string m_PlayerName = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string PlayerName
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }
        }
        
        [Constructable]
        public DeDOSDataCrystal(): base(7888)
        {
            Name = "a DeDOS data crystal";
            Hue = 2575;

            m_Instances.Add(this);
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_PlayerName == "")
                LabelTo(from, "(double click to attune)");

            else
                LabelTo(from, "(attuned to " + m_PlayerName + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (ParentEntity != null)
            {
                if (m_PlayerName != from.Name)
                {
                    m_PlayerName = from.Name;
                    from.SendMessage("You attune the data crystal to your 'frequency'. It will not receive data while inside a container, however.");
                } 

                else
                    from.SendMessage("That may not be used while inside a container.");
                return;
            }            

            if (m_NextUseAllowed > DateTime.UtcNow && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You may only use this item once every 30 seconds.");
                return;
            }

            if (m_PlayerName != from.Name)
            {
                m_PlayerName = from.Name;
                from.SendMessage("You attune the data crystal to your 'frequency'.");
            }            

            TransmitData(from);            
        }

        public virtual void VisualEffect(bool send)
        {
            int projectiles = 1;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                IEntity effectStartLocation;
                IEntity effectEndLocation;

                if (send)
                {
                    effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                    effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), Map);
                }

                else
                {
                    effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), Map);
                    effectEndLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 5), Map);
                }

                Effects.SendMovingParticles(effectStartLocation, effectEndLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);
            }
        }

        public void TransmitData(Mobile from)
        {
            m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;

            List<DeDOSDataCrystal> m_ValidCrystals = new List<DeDOSDataCrystal>();
                        
            foreach (DeDOSDataCrystal crystal in m_Instances)
            {
                if (crystal == null) continue;
                if (crystal.Deleted) continue;
                if (crystal == this) continue;
                if (crystal.ParentEntity != null) continue;               

                m_ValidCrystals.Add(crystal);
            }            

            if (m_ValidCrystals.Count == 0)
            {
                from.SendMessage("No data crystals appear to be currently attuned.");
                return;
            }

            Effects.PlaySound(Location, Map, 0x2F4);
            PublicOverheadMessage(MessageType.Regular, TextHue, false, "*transmitting data*");

            DeDOSDataCrystal targetCrystal = m_ValidCrystals[Utility.RandomMinMax(0, m_ValidCrystals.Count - 1)];
            Effects.PlaySound(targetCrystal.Location, targetCrystal.Map, 0x2F4);

            string fromText = from.Name;
            
            Effects.PlaySound(targetCrystal.Location, targetCrystal.Map, 0x2F4);
            targetCrystal.PublicOverheadMessage(MessageType.Regular, TextHue, false, "*incoming data from " + from.Name + "*");

            for (int a = 0; a < 5; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1 + (a * 1)), delegate
                {
                    if (targetCrystal == null) return;
                    if (targetCrystal.Deleted) return;
                    if (targetCrystal.ParentEntity != null) return;

                    string message = "";

                    int messageLength = Utility.RandomMinMax(5, 15);

                    for (int b = 0; b < messageLength; b++)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: message += "0"; break;
                            case 2: message += "1"; break;
                        }
                    }

                    VisualEffect(true);

                    Effects.PlaySound(targetCrystal.Location, targetCrystal.Map, 0x2F4);
                    targetCrystal.PublicOverheadMessage(MessageType.Regular, DataHue, false, message);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(6), delegate
            {
                if (this == null) return;
                if (Deleted) return;
                if (ParentEntity != null) return;

                ReceiveData(from, targetCrystal);
            });
        }

        public void ReceiveData(Mobile from, DeDOSDataCrystal targetCrystal)
        {
            if (this == null) return;
            if (Deleted) return;
            if (ParentEntity != null) return;

            if (targetCrystal == null) return;
            if (targetCrystal.Deleted) return;
            if (targetCrystal.ParentEntity != null) return;

            Effects.PlaySound(targetCrystal.Location, targetCrystal.Map, 0x2F4);
            targetCrystal.PublicOverheadMessage(MessageType.Regular, TextHue, false, "*sending response*");

            string targetName = "an unknown individual";

            if (targetCrystal.m_PlayerName != "")
                targetName = targetCrystal.m_PlayerName;

            Effects.PlaySound(Location, Map, 0x2F4);
            PublicOverheadMessage(MessageType.Regular, TextHue, false, "*receiving response from " + targetName + "*");

            for (int a = 0; a < 5; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1 + (a * 1)), delegate
                {
                    if (this == null) return;
                    if (Deleted) return;
                    if (ParentEntity != null) return;

                    string message = "";

                    int messageLength = Utility.RandomMinMax(5, 15);

                    for (int b = 0; b < messageLength; b++)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: message += "0"; break;
                            case 2: message += "1"; break;
                        }
                    }

                    VisualEffect(false);

                    Effects.PlaySound(Location, Map, 0x2F4);
                    PublicOverheadMessage(MessageType.Regular, DataHue, false, message);
                });
            }
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

        public DeDOSDataCrystal(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version

            //Version 0
            writer.Write(m_PlayerName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_PlayerName = reader.ReadString();

            //------

            m_Instances.Add(this);
        }
    }
}