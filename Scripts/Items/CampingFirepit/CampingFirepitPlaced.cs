using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Regions;

namespace Server.Custom
{
    public class CampingFirepitPlaced : Item
    {
        public enum QualityType
        {
            Poor,
            Average,
            Good,
            Expert            
        }

        public TimeSpan Duration = TimeSpan.FromMinutes(30); //Length of Firepit Charge
        public TimeSpan TickInterval = TimeSpan.FromSeconds(5); //Frequency of Regen        

        //Hits
        public int hitsRegenMin = 3;
        public int hitsRegenMax = 5;

        public int hitsRegenPoorMax = 5;
        public int hitsRegenAverageMax = 6;
        public int hitsRegenGoodMax = 7;
        public int hitsRegenExpertMax = 8;

        //Stam
        public int stamRegenMin = 5;
        public int stamRegenMax = 7;

        public int stamRegenPoorMax = 7;
        public int stamRegenAverageMax = 8;
        public int stamRegenGoodMax = 9;
        public int stamRegenExpertMax = 10;

        //Mana
        public int manaRegenMin = 3;
        public int manaRegenMax = 5;

        public int manaRegenPoorMax = 5; 
        public int manaRegenAverageMax = 6; 
        public int manaRegenGoodMax = 7; 
        public int manaRegenExpertMax = 8;

        public int EffectRadius = 10;
        public int CombatCheckRadius = 20;

        private QualityType m_QualityLevel = QualityType.Poor;
        [CommandProperty(AccessLevel.GameMaster)]
        public QualityType QualityLevel
        {
            get { return m_QualityLevel; }
            set { m_QualityLevel = value; }
        }

        private bool m_InDungeon;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool InDungeon
        {
            get { return m_InDungeon; }
            set { m_InDungeon = value; }
        }

        private DateTime m_Expiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        public FirepitFire m_FirepitFire;
        public List<Item> m_Items = new List<Item>();           

        private Timer m_Timer;
        
        [Constructable]
        public CampingFirepitPlaced(): base(10756)
        {
            Name = "a camping firepit";
            Weight = 10;
        }

        public CampingFirepitPlaced(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            string qualityText = "poorly made";

            switch (m_QualityLevel)
            {
                case CampingFirepitPlaced.QualityType.Poor: qualityText = "poor quality"; break;
                case CampingFirepitPlaced.QualityType.Average: qualityText = "average quality"; break;
                case CampingFirepitPlaced.QualityType.Good: qualityText = "good quality"; break;
                case CampingFirepitPlaced.QualityType.Expert: qualityText = "expert quality"; break;
            }

            LabelTo(from, "(" + qualityText + ")");                  
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (RootParentEntity != null)
            {
                from.SendMessage("This must be placed on the ground before it may be activated.");
                return;
            }

            from.SendMessage("You put out the fire.");
            Extinguish(); 

            return;            
        }

        public void Light(Mobile from)
        {
            bool success = from.CheckSkill(SkillName.Camping, 0.0, 120.0);

            string response = "Despite your limited knowledge of the outdoors, you fashion a crude campsite.";

            if (success)
            {
                if (from.Skills.Camping.Value >= 50)
                {
                    m_QualityLevel = QualityType.Average;
                    response = "Leveraging your basic knowledge of the outdoors, you fashion a servicable campsite.";
                }

                if (from.Skills.Camping.Value >= 100)
                {
                    m_QualityLevel = QualityType.Good;
                    response = "Harnessing your advanced knowledge of the outdoors, you fashion a very welcoming campsite.";
                }

                if (from.Skills.Camping.Value >= 120)
                {
                    m_QualityLevel = QualityType.Expert;
                    response = "Utilizing your masterful knowledge of the outdoors, you fashion an exceptionally safe and secure campsite.";
                }
            }

            from.SendMessage(response);

            m_Expiration = DateTime.UtcNow + Duration;

            Effects.PlaySound(Location, Map, 0x4B9);

            Point3D location = Location;
            Map map = Map;            

            ItemID = 10749; 

            AddComponents();

            if (from.Region.IsPartOf(typeof(DungeonRegion)))
                m_InDungeon = true;

            FirepitFire firepitFire = new FirepitFire(this);

            m_FirepitFire = firepitFire;
            m_FirepitFire.MoveToWorld(Location, Map);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (this == null) return;
                if (Deleted) return;
                if (Location != location || map != Map) return;               

                Effects.PlaySound(location, map, 0x5CF);

                Point3D flamePoint = new Point3D(location.X, location.Y, location.Z + 5);
                Effects.SendLocationParticles(EffectItem.Create(flamePoint, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 0, 0, 5029, 0);
            });
        }

        public void Extinguish()
        {
            ItemID = 10756;

            Effects.PlaySound(Location, Map, 0x4BB);
            PublicOverheadMessage(MessageType.Emote, 0, false, "*extinguishes*");           

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            for (int a = 0; a < m_Items.Count; ++a)
                m_Items[a].Delete();

            if (m_FirepitFire != null)
            {
                if (!m_FirepitFire.Deleted)
                    m_FirepitFire.Delete();
            }

            Delete();
        }

        public override bool OnDragLift(Mobile from)
        {            
            Extinguish();

            return true;
        }        

        public void AddComponents()
        {
            if (this == null) return;
            if (Deleted) return;

            Item staticFire = new Static(6571);

            staticFire.Name = "fire";
            staticFire.Movable = false;
            staticFire.Light = LightType.Circle300;

            Point3D location = new Point3D(Location.X, Location.Y, Location.Z + 5);

            staticFire.MoveToWorld(location, Map);

            m_Items.Add(staticFire);
        }

        private class InternalTimer : Timer
        {
            private CampingFirepitPlaced m_Firepit;

            public InternalTimer(CampingFirepitPlaced firepit): base(firepit.TickInterval, firepit.TickInterval)
            {
                Priority = TimerPriority.OneSecond;

                m_Firepit = firepit;
            }

            protected override void OnTick()
            {
                if (m_Firepit == null)
                {
                    Stop();
                    return;
                }

                if (m_Firepit.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Firepit.Expiration <= DateTime.UtcNow)
                {
                    Stop();
                    m_Firepit.Extinguish();

                    return;
                }

                Effects.PlaySound(m_Firepit.Location, m_Firepit.Map, 0x5CF);
                Effects.SendLocationParticles(EffectItem.Create(m_Firepit.Location, m_Firepit.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5005);

                IPooledEnumerable mobilesInEffectRadius = m_Firepit.Map.GetMobilesInRange(m_Firepit.Location, m_Firepit.CombatCheckRadius);

                List<Mobile> m_MobilesInCombat = new List<Mobile>();

                Queue m_Queue = new Queue();

                foreach (Mobile mobile in mobilesInEffectRadius)
                {
                    if (mobile.Combatant != null)
                    {
                        if (mobile.Combatant.Alive)
                        {
                            if (!m_MobilesInCombat.Contains(mobile.Combatant))
                                m_MobilesInCombat.Add(mobile.Combatant);
                        }
                    }

                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.IsDeadPet || bc_Creature.IsDeadBondedPet)
                            continue;

                        if (!(bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile))
                            continue;
                    }

                    if (!mobile.Alive) continue;
                    if (!mobile.InRange(m_Firepit.Location, m_Firepit.EffectRadius)) continue;

                    if (mobile.Region is UOACZRegion)
                    {
                        if (mobile is BaseCreature)
                            continue;

                        if (mobile is PlayerMobile)
                        {
                            PlayerMobile player = mobile as PlayerMobile;

                            if (player.IsUOACZUndead)
                                continue;
                        }
                    }
                    
                    m_Queue.Enqueue(mobile);
                }

                mobilesInEffectRadius.Free();

                //Regen
                int hitsRegenMin = m_Firepit.hitsRegenMin;
                int hitsRegenMax = m_Firepit.hitsRegenMax;

                int stamRegenMin = m_Firepit.stamRegenMin;
                int stamRegenMax = m_Firepit.stamRegenMax;

                int manaRegenMin = m_Firepit.manaRegenMin;
                int manaRegenMax = m_Firepit.manaRegenMax;

                switch (m_Firepit.m_QualityLevel)
                {
                    case QualityType.Poor:
                        hitsRegenMax = m_Firepit.hitsRegenPoorMax;
                        stamRegenMax = m_Firepit.stamRegenPoorMax;
                        manaRegenMax = m_Firepit.manaRegenPoorMax;
                    break;

                    case QualityType.Average:
                        hitsRegenMax = m_Firepit.hitsRegenAverageMax;
                        stamRegenMax = m_Firepit.stamRegenAverageMax;
                        manaRegenMax = m_Firepit.manaRegenAverageMax;
                    break;

                    case QualityType.Good:
                        hitsRegenMax = m_Firepit.hitsRegenGoodMax;
                        stamRegenMax = m_Firepit.stamRegenGoodMax;
                        manaRegenMax = m_Firepit.manaRegenGoodMax;
                    break;

                    case QualityType.Expert:
                        hitsRegenMax = m_Firepit.hitsRegenExpertMax;
                        stamRegenMax = m_Firepit.stamRegenExpertMax;
                        manaRegenMax = m_Firepit.manaRegenExpertMax;
                    break;
                }

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    if (mobile == null) continue;
                    if (m_MobilesInCombat.Contains(mobile)) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;
                    if (mobile.LastCombatTime + mobile.CombatExpirationDelay > DateTime.UtcNow) continue;
                    if (mobile.LastPlayerCombatTime + mobile.PlayerCombatExpirationDelay > DateTime.UtcNow) continue;
                    if (mobile.NextFirepitRegenAllowed > DateTime.UtcNow) continue;

                    bool regenOccured = false;

                    int hitRegenAmount = Utility.RandomMinMax(hitsRegenMin, hitsRegenMax);
                    int stamRegenAmount = Utility.RandomMinMax(stamRegenMin, stamRegenMax);
                    int manaRegenAmount = Utility.RandomMinMax(manaRegenMin, manaRegenMax);

                    if (mobile is BaseCreature)
                    {
                        hitRegenAmount *= 5;
                        stamRegenAmount *= 5;
                        manaRegenAmount *= 5;
                    }

                    if (mobile.Hits != mobile.HitsMax)
                    {
                        mobile.Heal(hitRegenAmount);
                        regenOccured = true;
                    }

                    if (mobile.Stam != mobile.StamMax)
                    {
                        mobile.Stam += stamRegenAmount;
                        regenOccured = true;
                    }

                    if (mobile.Mana != mobile.ManaMax)
                    {
                        mobile.Mana += manaRegenAmount;
                        regenOccured = true;
                    }

                    if (regenOccured)                    
                        mobile.FixedParticles(0x376A, 9, 16, 5005, EffectLayer.Waist);                    

                    mobile.NextFirepitRegenAllowed = DateTime.UtcNow + m_Firepit.TickInterval;
                }
            }
        }        

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            for (int a = 0; a < m_Items.Count; ++a)
                m_Items[a].Delete();

            if (m_FirepitFire != null)
            {
                if (!m_FirepitFire.Deleted)
                    m_FirepitFire.Delete();
            }            

            if (!Deleted)
                Delete();
        }
       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //version

            writer.Write((int)m_QualityLevel);
            writer.Write(m_InDungeon);
            writer.Write(m_Expiration);
            writer.Write(m_FirepitFire);

            writer.Write(m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Items = new List<Item>();

            if (version >= 0)
            {
                m_QualityLevel = (QualityType)reader.ReadInt();
                m_InDungeon = reader.ReadBool();
                m_Expiration = reader.ReadDateTime();
                m_FirepitFire = (FirepitFire)reader.ReadItem();

                int itemsCount = reader.ReadInt();
                for (int a = 0; a < itemsCount; a++)
                {
                    Item item = reader.ReadItem();

                    if (item != null)
                        m_Items.Add(item);
                }                
            }

            if (m_Expiration >= DateTime.UtcNow)
            {
                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }

            else
                Extinguish();
        }
    }
}
