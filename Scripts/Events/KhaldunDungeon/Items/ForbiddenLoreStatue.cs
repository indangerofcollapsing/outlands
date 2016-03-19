using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    public class ForbiddenLoreStatue : Item
    {
        public override bool Decays { get { return false; } }

        public List<Item> m_Items = new List<Item>();

        public InternalTimer m_Timer;

        private int m_Range = 15;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int Range
        {
            get { return m_Range; }
            set { m_Range = value; }
        }

        private int m_MinDamage = 20;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int MinDamage
        {
            get { return m_MinDamage; }
            set { m_MinDamage = value; }
        }

        private int m_MaxDamage = 30;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int MaxDamage
        {
            get { return m_MaxDamage; }
            set { m_MaxDamage = value; }
        }

        private double m_EffectInterval = 5;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double EffectInterval
        {
            get { return m_EffectInterval; }
            set { m_EffectInterval = value; }
        }

        private int m_EffectCount = 0;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int EffectCount
        {
            get { return m_EffectCount; }
            set { m_EffectCount = value; }
        }

        private int m_EffectMaxCount = 0;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int EffectMaxCount
        {
            get { return m_EffectMaxCount; }
            set { m_EffectMaxCount = value; }
        }

        [Constructable]
        public ForbiddenLoreStatue(int range, int minDamage, int maxDamage, double effectInterval, int effectMaxCount): base(4643)
        {
            Name = "forbidden lore";

            Hue = 2614;
            Movable = false;

            m_Range = range;
            m_MinDamage = minDamage;
            m_MaxDamage = maxDamage;
            m_EffectInterval = effectInterval;
            m_EffectMaxCount = effectMaxCount;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public void AddComponents()
        {
            if (Deleted)
                return;

            Static pillar = new Static(3094);
            pillar.Hue = 2603;
            pillar.Name = "forbidden lore";

            GroupItem(pillar, 0, 0, 6);

            Static flame = new Static(6571);
            flame.Hue = 2603;
            flame.Name = "forbidden lore";

            GroupItem(flame, 0, 0, 13);

            Start();
        }

        public void GroupItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item == null)
                return;
            
            item.Movable = false;

            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public void Start()
        {
            Effects.PlaySound(Location, Map, 0x20F);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public class InternalTimer : Timer
        {
            public ForbiddenLoreStatue m_ForebiddenLoreStatue;

            public InternalTimer(ForbiddenLoreStatue forebiddenLoreStatue): base(TimeSpan.FromSeconds(forebiddenLoreStatue.m_EffectInterval), TimeSpan.FromSeconds(forebiddenLoreStatue.m_EffectInterval))
            {
                m_ForebiddenLoreStatue = forebiddenLoreStatue;
            }

            protected override void OnTick()
            {
                if (m_ForebiddenLoreStatue == null)
                {
                    Stop();
                    return;
                }

                if (m_ForebiddenLoreStatue.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_ForebiddenLoreStatue.EffectCount >= m_ForebiddenLoreStatue.EffectMaxCount)
                {
                    m_ForebiddenLoreStatue.Delete();
                    Stop();

                    return;
                }
                
                m_ForebiddenLoreStatue.EffectCount++;

                Point3D location = m_ForebiddenLoreStatue.Location;
                Map map = m_ForebiddenLoreStatue.Map;

                int effectHue = m_ForebiddenLoreStatue.Hue - 1;

                int minDamage = m_ForebiddenLoreStatue.MinDamage;
                int maxDamage = m_ForebiddenLoreStatue.MaxDamage;

                List<Mobile> m_ValidMobiles = new List<Mobile>();

                IPooledEnumerable mobileInRange = m_ForebiddenLoreStatue.Map.GetMobilesInRange(location, m_ForebiddenLoreStatue.Range);

                foreach (Mobile mobile in mobileInRange)
                {
                    if (UOACZRegion.ContainsItem(m_ForebiddenLoreStatue))
                    {  
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (!map.InLOS(location, mobile.Location)) continue;
                        if (mobile.Hidden) continue;
                        if (mobile is UOACZBaseUndead) continue;
                        if (mobile is PlayerMobile)
                        {
                            PlayerMobile player = mobile as PlayerMobile;

                            if (player.IsUOACZUndead)
                                continue;
                        }

                        m_ValidMobiles.Add(mobile);
                    }

                    else
                    {
                        if (!SpecialAbilities.MonsterCanDamage(null, mobile))
                            continue; 
                    }
                }

                mobileInRange.Free();

                if (m_ValidMobiles.Count == 0)
                    return;

                Mobile target = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];
                Point3D targetLocation = target.Location;

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 14), map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);

                int particleSpeed = 5;

                Effects.PlaySound(location, map, 0x227);
                Effects.SendMovingParticles(startLocation, endLocation, 0x36D4, particleSpeed, 0, false, false, effectHue, 0, 9501, 0, 0, 0x100);
                
                Point3D newLocation = new Point3D(location.X, location.Y, location.Z + 14);

                TimedStatic timedStatic = new TimedStatic(0x3779, .5);
                timedStatic.Hue = effectHue;
                timedStatic.Name = "dissipated energy";
                timedStatic.MoveToWorld(newLocation, map);                

                double distance = Utility.GetDistanceToSqrt(location, targetLocation);
                double destinationDelay = (double)distance * .08;

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (m_ForebiddenLoreStatue.Deleted) return;                   
                    if (target == null) return;
                    if (target.Deleted || !target.Alive) return;
                    if (Utility.GetDistanceToSqrt(m_ForebiddenLoreStatue.Location, targetLocation) >= 30) return;

                    int damage = Utility.RandomMinMax(minDamage, maxDamage);

                    if (target is BaseCreature)
                        damage *= 2;

                    Effects.PlaySound(location, map, 0x208);
                    
                    target.FixedParticles(0x36BD, 20, 20, 5044, effectHue, 0, EffectLayer.Head);
                    //Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.5)), 0x3996, 10, 20, effectHue, 0, 5029, 0);

                    new Blood().MoveToWorld(targetLocation, map);
                    AOS.Damage(target, damage, 0, 100, 0, 0, 0);
                });                
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();

            if (!Deleted)
                Delete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
 	        base.OnLocationChange(oldLocation);

            for (int a = 0; a < m_Items.Count; a++)
            {
                Item item = m_Items[a];

                if (item.ItemID == 3094)
                    item.Location = new Point3D(Location.X, Location.Y, Location.Z + 6);

                if (item.ItemID == 6571)
                    item.Location = new Point3D(Location.X, Location.Y, Location.Z + 13);
            }
        }

        public ForbiddenLoreStatue(Serial serial): base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

             writer.Write(m_Range);
             writer.Write(m_MinDamage);
             writer.Write(m_MaxDamage);
             writer.Write(m_EffectInterval);
             writer.Write(m_EffectCount);
             writer.Write(m_EffectMaxCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Items = new List<Item>();

            //Version 0
            int itemsCount = reader.ReadInt();
            for (int i = 0; i < itemsCount; ++i)
            {
                m_Items.Add(reader.ReadItem());
            }

            m_Range = reader.ReadInt();
            m_MinDamage = reader.ReadInt();
            m_MaxDamage = reader.ReadInt();
            m_EffectInterval = reader.ReadDouble();
            m_EffectCount = reader.ReadInt();
            m_EffectMaxCount = reader.ReadInt();

            //---------
            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}