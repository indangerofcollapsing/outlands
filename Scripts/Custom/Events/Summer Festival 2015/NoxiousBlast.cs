using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;

namespace Server
{
    public class NoxiousBlast : Item
    {        
        public DateTime m_NextUseAllowed = DateTime.UtcNow;

        public TimeSpan UsageCooldown = TimeSpan.FromSeconds(2);
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Cooldown 
        { 
            get { return UsageCooldown; }
            set
            {
                UsageCooldown = value; 
                m_NextUseAllowed = DateTime.UtcNow; 
            }
        }

        public int m_MaxCharges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges 
        {
            get { return m_MaxCharges; } 
            set { m_MaxCharges = value; }
        }
        
        public int m_Charges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        { 
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public int m_MinDamage = 10;        
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinDamage { get { return m_MinDamage; } set { m_MinDamage = value; } }

        public int m_MaxDamage = 15;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDamage { get { return m_MaxDamage; } set { m_MaxDamage = value; } }
       
        Poison m_CenterPoisonLevel = Poison.Deadly;
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison CenterPoisonLevel 
        {
            get { return m_CenterPoisonLevel; }
            set { m_CenterPoisonLevel = value; } 
        }

        public int m_Radius = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Radius { get { return m_Radius; } set { if (value < 12 && value > 0) m_Radius = value; else m_Radius = 12; } }
        
        public double m_SpellDelay = .05;
        [CommandProperty(AccessLevel.GameMaster)]
        public double SpellDelay { get { return m_SpellDelay; } set { if (value < 1 && value > 0) m_SpellDelay = value; else m_SpellDelay = .1; } }
             
        public int m_EffectHue = 2005;
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectHue { get { return m_EffectHue; } set { m_EffectHue = value; } }        
               
        bool m_PlayerAccessable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerAccessible { get { return m_PlayerAccessable; } set { m_PlayerAccessable = value; } }
               
        bool m_CanDamageMonsters = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanDamageMonsters { get { return m_CanDamageMonsters; } set { m_CanDamageMonsters = value; } }

        String m_DisplayText = "Vas Ort In Nox Grav";
        [CommandProperty(AccessLevel.GameMaster)]
        public String DisplayText { get { return m_DisplayText; } set { m_DisplayText = value; } }

        [Constructable]
        public NoxiousBlast(): base(0x46E6)
        {
            Name = "a noxious blast (ability)";
            Weight = 0;
            LootType = LootType.Blessed;
        }

        public NoxiousBlast(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player && !(m_PlayerAccessable))
            {
                from.SendMessage("You are unworthy to use this item.");
                return;
            }

            if (m_NextUseAllowed > DateTime.UtcNow)
            {
                from.SendMessage("Your noxious blast is on cooldown...");
                return;
            }

            from.Target = new InternalTarget(this);
        }

        public void updateCharges()
        {
            Charges += System.Convert.ToInt32(DateTime.UtcNow) + System.Convert.ToInt32(m_NextUseAllowed);

            if (Charges > MaxCharges)
                Charges = MaxCharges;
        }

        private class InternalTarget : Target
        {
            private NoxiousBlast m_NoxiousBlast;

            public InternalTarget(NoxiousBlast noxiousBlast): base(14, true, TargetFlags.None)
            {
                m_NoxiousBlast = noxiousBlast;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                IPoint3D p3D = target as IPoint3D;

                if (p3D == null) 
                    return;

                Map map = from.Map;
                int x = p3D.X, y = p3D.Y, z = map.GetAverageZ(x, y);
                Point3D loc = new Point3D(x, y, z);

                m_NoxiousBlast.m_NextUseAllowed = DateTime.UtcNow + m_NoxiousBlast.UsageCooldown;

                from.PublicOverheadMessage(Network.MessageType.Regular, 0, false, m_NoxiousBlast.DisplayText);
                NoxBlast(loc, map, from);
            }

            protected void NoxBlast(Point3D loc, Map map, Mobile from)
            {
                double effectTime = 1.5;
                double actionsCooldown = 3;

                from.Animate(16, 10, 1, true, false, 0);
                SpellHelper.Turn(from, loc);

                int itemId = 0x46E6;
                int itemHue = m_NoxiousBlast.EffectHue;

                int explosionHue = m_NoxiousBlast.EffectHue;

                Poison poisonLevel = m_NoxiousBlast.CenterPoisonLevel;

                int minDamage = m_NoxiousBlast.MinDamage;
                int maxDamage = m_NoxiousBlast.MaxDamage;
                double spellDelay = m_NoxiousBlast.SpellDelay;
                bool canDamageMonster = m_NoxiousBlast.CanDamageMonsters;

                Point3D location = from.Location;
                Point3D targetLocation = loc;

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (this == null) return;
                    if (!from.Alive) return;

                    Effects.PlaySound(targetLocation, map, 0x5FC);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 10), map);

                    Effects.SendMovingParticles(startLocation, endLocation, itemId, 8, 0, false, false, itemHue, 0, 9501, 0, 0, 0x100);

                    double distance = Math.Sqrt((Math.Pow(loc.X - location.X, 2) + Math.Pow(loc.Y - location.Y, 2)));
                    double destinationDelay = (double)distance * .06;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        Effects.PlaySound(targetLocation, map, 0x357);

                        int radius = Utility.RandomMinMax(2, 4);

                        int minRange = radius * -1;
                        int maxRange = radius;

                        for (int a = minRange; a < maxRange + 1; a++)
                        {
                            for (int b = minRange; b < maxRange + 1; b++)
                            {
                                Point3D newPoint = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);
                                SpellHelper.AdjustField(ref newPoint, map, 12, false);

                                int distanceDelay = System.Convert.ToInt32((Math.Pow(loc.X - newPoint.X, 2) + Math.Pow(loc.Y - newPoint.Y, 2)));

                                Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay * spellDelay), delegate
                                {
                                    Effects.PlaySound(newPoint, map, Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580));
                                    Effects.SendLocationParticles(EffectItem.Create(newPoint, map, TimeSpan.FromSeconds(0)), 0x3728, 10, 14, explosionHue, 0, 5029, 0);

                                    IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newPoint, 0);

                                    Queue m_Queue = new Queue();

                                    foreach (Mobile mobile in mobilesOnTile)
                                    {
                                        if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                            continue;

                                        bool validTarget = false;

                                        PlayerMobile pm_Target = mobile as PlayerMobile;
                                        BaseCreature bc_Target = mobile as BaseCreature;

                                        if (pm_Target != null)
                                            validTarget = true;

                                        if (bc_Target != null)
                                        {
                                            if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile || canDamageMonster)
                                                validTarget = true;
                                        }

                                        if (validTarget)
                                            m_Queue.Enqueue(mobile);
                                    }

                                    mobilesOnTile.Free();

                                    while (m_Queue.Count > 0)
                                    {
                                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                                        double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                        if (mobile is BaseCreature)
                                            damage *= 1.5;

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        Effects.PlaySound(mobile.Location, mobile.Map, 0x22F);

                                        mobile.ApplyPoison(mobile, poisonLevel); // Will handle poison levels too
                                        AOS.Damage(mobile, from, (int)(Math.Round(damage)), 0, 100, 0, 0, 0);
                                    }
                                });
                            }
                        }
                    });
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 

            //Version 0
            writer.Write(m_NextUseAllowed);
            writer.Write(m_MaxCharges);
            writer.Write(m_Charges);
            writer.Write(m_MinDamage);
            writer.Write(m_MaxDamage);

            if (m_CenterPoisonLevel == null)
                writer.Write(-1);
            else
                writer.Write((int)m_CenterPoisonLevel.Level);

            writer.Write(m_Radius);
            writer.Write(m_SpellDelay);
            writer.Write(m_EffectHue);
            writer.Write(m_PlayerAccessable);
            writer.Write(m_CanDamageMonsters);
            writer.Write(m_DisplayText);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_NextUseAllowed = reader.ReadDateTime();
                m_MaxCharges = reader.ReadInt();
                m_Charges = reader.ReadInt();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();

                int poisonLevel = reader.ReadInt();

                m_CenterPoisonLevel = null;

                if (poisonLevel > -1)
                    m_CenterPoisonLevel = Poison.GetPoison(poisonLevel);

                m_Radius = reader.ReadInt();
                m_SpellDelay = reader.ReadDouble();
                m_EffectHue = reader.ReadInt();
                m_PlayerAccessable = reader.ReadBool();
                m_CanDamageMonsters = reader.ReadBool();
                m_DisplayText = reader.ReadString();
            }            
        }
    }
}