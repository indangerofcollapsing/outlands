using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Spells;
using Server.Items;

namespace Server.Custom
{
    public class FishingSpot : Item
    {
        public double[] m_RarityChances = {0, .85, .95, .99};
        public AquariumItem.Rarity m_Rarity = AquariumItem.Rarity.Common;

        private int m_FishingActionsRemaining = Utility.RandomMinMax(10, 20);
        [CommandProperty(AccessLevel.GameMaster)]
        public int FishingActionsRemaining
        {
            get { return m_FishingActionsRemaining; }
            set { m_FishingActionsRemaining = value; }
        }

        public int m_StartingFishingActions;

        public Dictionary<Mobile, DateTime> m_NextPlayerNewAllowed = new Dictionary<Mobile, DateTime>();

        public DateTime m_Expiration;

        public List<Item> m_Items = new List<Item>();

        private FishingSpotSpawner m_FishingSpotSpawner;
        [CommandProperty(AccessLevel.GameMaster)]
        public FishingSpotSpawner FishingSpotSpawner
        {
            get { return m_FishingSpotSpawner; }
            set { m_FishingSpotSpawner = value; }
        }

        private Timer m_Timer;

        public static List<FishingSpot> m_Instances = new List<FishingSpot>();

        [Constructable]
        public FishingSpot(): base()
        {
            Name = "a fishing spot";

            ItemID = 4014;
            Visible = false;

            m_StartingFishingActions = m_FishingActionsRemaining;
            m_Expiration =  DateTime.UtcNow + TimeSpan.FromHours(24);

            SetRarity();

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            OceanStatic = true;
                                    
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }        

        public void SetRarity()
        {
            double rarityChance = Utility.RandomDouble();

            for (int a = 0; a < m_RarityChances.Length; a++)
            {
                if (rarityChance >= m_RarityChances[a])
                    m_Rarity = (AquariumItem.Rarity)a;
            }
        }

        public virtual void AddComponents()
        {
            if (Deleted)
                return;

            Movable = false;

            m_Instances.Add(this);        
        }        

        public virtual void GroupItem(Item item, int hue, int xOffset, int yOffset, int zOffset, string name)
        {
            if (item == null)
                return;

            item.Name = name;
            item.Hue = hue;
            item.Movable = false;
            item.OceanStatic = true;
                
            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
        }

        public virtual void FishingAction(Mobile from, Point3D location, Map map)
        {
            if (from == null || Deleted)   
                return;            

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (m_FishingActionsRemaining <= 0)
            {
                from.SendMessage("That fishing spot has recently been exhausted.");
                return;
            }

            m_FishingActionsRemaining--;

            if (m_FishingActionsRemaining >= 1)            
                FishingResult(from);                 
            
            else if (m_FishingActionsRemaining == 0)            
                FishingFinalResult(from);                      
        }

        public virtual void FishingResult(Mobile from)
        {
        }

        public virtual void FishingFinalResult(Mobile from)
        {
            Delete();
        }        

        private class InternalTimer : Timer
        {
            private FishingSpot m_FishingSpot;

            public InternalTimer(FishingSpot fishingSpot): base(TimeSpan.Zero, TimeSpan.FromMinutes(5))
            {
                Priority = TimerPriority.OneMinute;
                m_FishingSpot = fishingSpot;
            }

            protected override void OnTick()
            {
                if (m_FishingSpot == null)
                {
                    Stop();

                    return;
                }

                if (m_FishingSpot.Deleted)
                {
                    Stop();

                    return;
                }

                if (m_FishingSpot.m_Expiration < DateTime.UtcNow)
                {
                    Stop();
                    m_FishingSpot.Delete();

                    return;
                }
            }
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            if (!BaseBoat.IsWaterTile(location, m))
                Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            
            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)
                {
                    if (!m_Items[a].Deleted)
                        m_Items[a].Delete();
                }
            }

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            m_Instances.Remove(this);

            if (m_FishingSpotSpawner != null)
            {
                if (!m_FishingSpotSpawner.Deleted)
                    if (m_FishingSpotSpawner.m_FishingSpots != null)
                        m_FishingSpotSpawner.m_FishingSpots.Remove(this);
            }

            Delete();
        }

        public FishingSpot(Serial serial): base(serial)
        {
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_Rarity);
            writer.Write(m_FishingActionsRemaining);
            writer.Write(m_StartingFishingActions);
            writer.Write(m_Expiration);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            writer.Write(m_FishingSpotSpawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_Rarity = (AquariumItem.Rarity)reader.ReadInt();
            m_FishingActionsRemaining = reader.ReadInt();
            m_StartingFishingActions = reader.ReadInt();
            m_Expiration = reader.ReadDateTime();

            int itemsCount = reader.ReadInt();

            for (int i = 0; i < itemsCount; ++i)
            {
                m_Items.Add(reader.ReadItem());
            }

            m_FishingSpotSpawner = (FishingSpotSpawner)reader.ReadItem();

            //------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            m_Instances.Add(this);
        }
    }
}