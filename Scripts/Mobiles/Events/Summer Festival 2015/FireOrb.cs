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
    public class FireOrb : Item
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

        public int m_MinDamage = 30;  
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinDamage { 
            get { return m_MinDamage; }
            set { m_MinDamage = value; }
        }

        public int m_MaxDamage = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDamage 
        {
            get { return m_MaxDamage; } 
            set { m_MaxDamage = value; }
        }
              
        public int m_NumAttacks = 50;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NumAttacks
        {
            get { return m_NumAttacks; } 
            set 
            {
                if (value < 101 && value > 0)
                    m_NumAttacks = value;

                else m_NumAttacks = 50;
            } 
        }
       
        public int m_Radius = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Radius
        {
            get { return m_Radius; } 
            set 
            {
                if (value <= 12 && value >= 0) 
                    m_Radius = value; 

                else m_Radius = 3; 
            } 
        }
       
        public double m_SpellDelay = .1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double SpellDelay 
        {
            get { return m_SpellDelay; }
            set
            {
                if (value < 1 && value > 0) 
                    m_SpellDelay = value;

                else m_SpellDelay = .1; 
            } 
        }

        public TimeSpan m_ExplodeDelay = TimeSpan.FromSeconds(3);
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExplodeDelay 
        { 
            get { return m_ExplodeDelay; } 
            set
            { 
                if (value <= TimeSpan.FromSeconds(5) && value >= TimeSpan.FromSeconds(0)) 
                    m_ExplodeDelay = value; 

                else m_ExplodeDelay = TimeSpan.FromSeconds(3); 
            } 
        }
        
        public int m_EffectHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectHue 
        {
            get { return m_EffectHue; } 
            set { m_EffectHue = value; }
        }       

        bool m_PlayerAccessible = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerAccessible 
        { 
            get { return m_PlayerAccessible; } 
            set { m_PlayerAccessible = value; } 
        }

        bool m_CanDamageMonsters = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanDamageMonsters
        {
            get { return m_CanDamageMonsters; }
            set { m_CanDamageMonsters = value; } 
        }

        public string m_DisplayText = "*a fiery orb appears*";
        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayText
        {
            get { return m_DisplayText; }
            set { m_DisplayText = value; }
        }
        
        [Constructable]
        public FireOrb(): base(0x36FF)
        {
            Name = "a fiery sphere (ability)";
            Weight = 0;
            LootType = LootType.Blessed;
        }

        public FireOrb(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player && !(m_PlayerAccessible))
            {
                from.SendMessage("You are unworthy to use this item.");
                return;
            }

            if (m_NextUseAllowed > DateTime.UtcNow)
            {
                from.SendMessage("Your fireorb is on cooldown...");
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
            private FireOrb m_FireOrb;

            public InternalTarget(FireOrb fireOrb): base(14, true, TargetFlags.None)
            {
                m_FireOrb = fireOrb;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                IPoint3D p3D = target as IPoint3D;

                if (p3D == null) 
                    return;

                Map map = from.Map;
                int x = p3D.X, y = p3D.Y, z = map.GetAverageZ(x, y);
                Point3D loc = new Point3D(x, y, z);

                TimedStatic orb = new TimedStatic(0x36FE, 5);
                orb.Hue = m_FireOrb.EffectHue;

                if (m_FireOrb.ExplodeDelay > TimeSpan.FromSeconds(0))
                {
                    orb.MoveToWorld(loc, map);
                    Effects.PlaySound(loc, map, 0x306);
                    orb.PublicOverheadMessage(Network.MessageType.Regular, 0, false, m_FireOrb.DisplayText);

                    if (orb.Z < 120)
                        orb.Z += 5;
                }

                m_FireOrb.m_NextUseAllowed = DateTime.UtcNow + m_FireOrb.UsageCooldown;

                Timer.DelayCall(m_FireOrb.ExplodeDelay, delegate
                {
                    if (orb != null || orb.Deleted || !from.Alive || from == null)
                        orb.Delete();

                    Explode(loc, map, from);
                });
            }

            protected void Explode(Point3D loc, Map map, Mobile from)
            {
                int rows = (m_FireOrb.Radius * 2) + 1;
                int columns = (m_FireOrb.Radius * 2) + 1;
                int radius = m_FireOrb.Radius;

                List<Point3D> m_EffectLocations = new List<Point3D>();

                for (int a = 1; a < rows + 1; a++)
                {
                    for (int b = 1; b < columns + 1; b++)
                    {
                        Point3D newPoint = new Point3D(loc.X + (-1 * (radius + 1)) + a, loc.Y + (-1 * (radius + 1)) + b, loc.Z);
                        SpellHelper.AdjustField(ref newPoint, map, 12, false);

                        if (!m_EffectLocations.Contains(newPoint))
                            m_EffectLocations.Add(newPoint);
                    }
                }
               
                double spellDelay = m_FireOrb.SpellDelay;
                int explosionHue = m_FireOrb.EffectHue;

                int cycles = m_FireOrb.NumAttacks;

                int minDamage = m_FireOrb.MinDamage;
                int maxDamage = m_FireOrb.MaxDamage;
                bool canDamageMonster = m_FireOrb.CanDamageMonsters;

                if (m_EffectLocations.Count > 0)
                {
                    for (int a = 0; a < cycles; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * spellDelay), delegate
                        {
                            Effects.PlaySound(loc, map, 0x5CF);

                            Point3D newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];
                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, explosionHue, 0, 5029, 0);

                            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

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

                                SingleFireField singleFireField = new SingleFireField(from, explosionHue, 1, 20, 3, 5, false, canDamageMonster, true, -1, true);
                                singleFireField.MoveToWorld(mobile.Location, map);

                                AOS.Damage(mobile, from, (int)(Math.Round(damage)), 0, 100, 0, 0, 0);
                            }
                        });
                    }
                }
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
            writer.Write(m_NumAttacks);
            writer.Write(m_Radius);
            writer.Write(m_SpellDelay);
            writer.Write(m_ExplodeDelay);
            writer.Write(m_EffectHue);
            writer.Write(m_PlayerAccessible);
            writer.Write(m_CanDamageMonsters);
            writer.Write(m_DisplayText);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_NextUseAllowed = reader.ReadDateTime();
                m_MaxCharges = reader.ReadInt();
                m_Charges = reader.ReadInt();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();
                m_NumAttacks = reader.ReadInt();
                m_Radius = reader.ReadInt();
                m_SpellDelay = reader.ReadDouble();
                m_ExplodeDelay = reader.ReadTimeSpan();
                m_EffectHue = reader.ReadInt();
                m_PlayerAccessible = reader.ReadBool();
                m_CanDamageMonsters = reader.ReadBool();
                m_DisplayText = reader.ReadString();
            }            
        }
    }
}