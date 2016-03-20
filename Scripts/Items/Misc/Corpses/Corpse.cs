using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Achievements;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class Corpse : Container, ICarvable
    {
        private Mobile m_Owner;				// Whos corpse is this?
        private Mobile m_Killer;				// Who killed the owner?
        private bool m_Carved;				// Has this corpse been carved?
        public bool m_IsBones;				// Turned to bones?
        private bool m_KilledIdentified;     //IPY : Sean. Has a detective identified the killer?

        private List<Mobile> m_Looters;				// Who's looted this corpse?
        private List<Item> m_EquipItems;			// List of items equiped when the owner died. Ingame, these items display /on/ the corpse, not just inside
        private List<Mobile> m_Aggressors;			// Anyone from this list will be able to loot this corpse; we attacked them, or they attacked us when we were freely attackable

        private string m_CorpseName;			// Value of the CorpseNameAttribute attached to the owner when he died -or- null if the owner had no CorpseNameAttribute; use "the remains of ~name~"
        private bool m_NoBones;				// If true, this corpse will not turn into bones

        private bool m_VisitedByTaxidermist;	// Has this corpse yet been visited by a taxidermist?
        private bool m_Channeled;			// Has this corpse yet been used to channel spiritual energy? (AOS Spirit Speak)

        // For notoriety:
        private AccessLevel m_AccessLevel;			// Which AccessLevel the owner had when he died
        private Guild m_Guild;				// Which Guild the owner was in when he died
        private int m_Kills;				// How many kills the owner had when he died
        private bool m_Criminal;				// Was the owner criminal when he died?

        private DateTime m_TimeOfDeath;			// What time was this corpse created?

        private HairInfo m_Hair;					// This contains the hair of the owner
        private FacialHairInfo m_FacialHair;		// This contains the facial hair of the owner

        public static readonly TimeSpan MonsterLootRightSacrifice = TimeSpan.FromMinutes(2.0);

        public static readonly TimeSpan InstancedCorpseTime = TimeSpan.FromMinutes(3.0);

        //IPY: Sean.
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IdentifiedKiller
        {
            get { return m_KilledIdentified; }
            set { m_KilledIdentified = value; }
        }


        public void AddCarvedItem(Item carved, Mobile carver)
        {
            this.DropItem(carved);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool InstancedCorpse
        {
            get
            {
                if (!Core.SE)
                    return false;

                return (DateTime.UtcNow > m_TimeOfDeath + InstancedCorpseTime);
            }
        }

        public override bool IsDecoContainer
        {
            get { return false; }
        }

        private Dictionary<Mobile, List<Item>> m_InstancedItems;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDeath
        {
            get { return m_TimeOfDeath; }
            set { m_TimeOfDeath = value; }
        }

        public override bool DisplayWeight { get { return false; } }

        public HairInfo Hair { get { return m_Hair; } }
        public FacialHairInfo FacialHair { get { return m_FacialHair; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Carved
        {
            get { return m_Carved; }
            set { m_Carved = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool VisitedByTaxidermist
        {
            get { return m_VisitedByTaxidermist; }
            set { m_VisitedByTaxidermist = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Channeled
        {
            get { return m_Channeled; }
            set { m_Channeled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel AccessLevel
        {
            get { return m_AccessLevel; }
        }

        public List<Mobile> Aggressors
        {
            get { return m_Aggressors; }
        }

        public List<Mobile> Looters
        {
            get { return m_Looters; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Killer
        {
            get { return m_Killer; }
        }

        public List<Item> EquipItems
        {
            get { return m_EquipItems; }
        }

        public Guild Guild
        {
            get { return m_Guild; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills
        {
            get { return m_Kills; }
            set { m_Kills = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Criminal
        {
            get { return m_Criminal; }
            set { m_Criminal = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
        }

        public void TurnToBones()
        {
            if (Deleted)
                return;

            m_IsBones = true;
            ProcessDelta();
            SendRemovePacket();
            ItemID = Utility.Random(0xECA, 9); // bone graphic
            Hue = 0;
            ProcessDelta();

            m_NoBones = true;
            BeginDecay(m_BoneDecayTime);
        }

        private static TimeSpan m_DefaultDecayTime = TimeSpan.FromMinutes(15.0);
        private static TimeSpan m_BoneDecayTime = TimeSpan.FromMinutes(15.0);

        private Timer m_DecayTimer;
        private DateTime m_DecayTime;

        public void BeginDecay(TimeSpan delay)
        {
            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            m_DecayTime = DateTime.UtcNow + delay;

            m_DecayTimer = new InternalTimer(this, delay);
            m_DecayTimer.Start();
        }

        public override void OnAfterDelete()
        {
            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            m_DecayTimer = null;
        }

        private class InternalTimer : Timer
        {
            private Corpse m_Corpse;

            public InternalTimer(Corpse c, TimeSpan delay)
                : base(delay)
            {
                m_Corpse = c;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (!m_Corpse.m_NoBones)
                    m_Corpse.TurnToBones();
                else
                    m_Corpse.Delete();
            }
        }

        public static string GetCorpseName(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.CorpseNameOverride != null && bc_Creature.CorpseNameOverride != "")
                    return bc_Creature.CorpseNameOverride;
            }

            XmlData x = (XmlData)XmlAttach.FindAttachment(m, typeof(XmlData), "CorpseName");
            if (x != null)
            {
                return x.Data;
            }

            Type t = m.GetType();

            object[] attrs = t.GetCustomAttributes(typeof(CorpseNameAttribute), true);

            if (attrs != null && attrs.Length > 0)
            {
                CorpseNameAttribute attr = attrs[0] as CorpseNameAttribute;

                if (attr != null)
                    return attr.Name;
            }

            return null;
        }

        public static void Initialize()
        {
            Mobile.CreateCorpseHandler += new CreateCorpseHandler(Mobile_CreateCorpseHandler);
        }

        public static Container Mobile_CreateCorpseHandler(Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> initialContent, List<Item> equipItems)
        {
            bool shouldFillCorpse = true;

            Corpse c = null;

            if (owner == null)
                return c;

            c = new Corpse(owner, hair, facialhair, shouldFillCorpse ? equipItems : new List<Item>());

            if (c == null)
                return c;
            
            owner.Corpse = c;

            if (shouldFillCorpse)
            {
                for (int i = 0; i < initialContent.Count; ++i)
                {
                    Item item = initialContent[i];

                    if (item == null)
                        continue;

                    if (Core.AOS && owner.Player && item.Parent == owner.Backpack)
                        c.AddItem(item);

                    else
                        c.DropItem(item);

                    if (owner.Player && Core.AOS)
                        c.SetRestoreInfo(item, item.Location);
                }
            }

            else            
                c.Carved = true; // TODO: Is it needed?            

            Point3D loc = owner.Location;
            Map map = owner.Map;

            if (map == null || map == Map.Internal)
            {
                loc = owner.LogoutLocation;
                map = owner.LogoutMap;
            }

            c.MoveToWorld(loc, map);

            //Creature or Player Ocean Corpse Advanced Decay Rate
            if (!(c.Map == null || c.Map == Map.Internal))
            {
                if (c.Map.Tiles != null)
                {
                    LandTile landTile = c.Map.Tiles.GetLandTile(c.X, c.Y);
                    StaticTile[] tiles = c.Map.Tiles.GetStaticTiles(c.X, c.Y, true);

                    bool landHasWater = false;
                    bool staticHasWater = false;

                    if ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311))
                    {
                        landHasWater = true;
                    }

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];
                       
                        bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                        if (isWater)                            
                            staticHasWater = true; 
                    }

                    if ((landHasWater || staticHasWater) && c.TotalItems == 0)
                    {
                        if (c.m_DecayTimer != null)                        
                            c.m_DecayTimer.Stop();                        

                        //Empty PlayerMobile is 30 Seconds Decay
                        if (c.Owner is PlayerMobile)                        
                            c.BeginDecay(TimeSpan.FromSeconds(30));                        

                        //Empty Creature is 10 Seconds Decay
                        else                        
                            c.BeginDecay(TimeSpan.FromSeconds(10));                        
                    }
                }
            }

            return c;
        }

        public override bool IsPublicContainer { get { return true; } }

        public Corpse(Mobile owner, List<Item> equipItems)
            : this(owner, null, null, equipItems)
        {
        }

        public Corpse(Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> equipItems)
            : base(0x2006)
        {
            // To supress console warnings, stackable must be true
            Stackable = true;
            Amount = owner.Body; // protocol defines that for itemid 0x2006, amount=body
            Stackable = false;

            Movable = false;
            Hue = owner.Hue;
            Direction = owner.Direction;
            Name = owner.Name;

            m_Owner = owner;

            m_CorpseName = GetCorpseName(owner);

            m_TimeOfDeath = DateTime.UtcNow;

            m_AccessLevel = owner.AccessLevel;
            m_Guild = owner.Guild as Guild;
            m_Kills = owner.ShortTermMurders;
            m_Criminal = owner.Criminal;

            m_Hair = hair;
            m_FacialHair = facialhair;

            // This corpse does not turn to bones if: the owner is not a player
            m_NoBones = !owner.Player;

            m_Looters = new List<Mobile>();
            m_EquipItems = equipItems;

            m_Aggressors = new List<Mobile>(owner.Aggressors.Count + owner.Aggressed.Count);
            bool addToAggressors = !(owner is BaseCreature);

            TimeSpan lastTime = TimeSpan.MaxValue;

            for (int i = 0; i < owner.Aggressors.Count; ++i)
            {
                AggressorInfo info = owner.Aggressors[i];

                if ((DateTime.UtcNow - info.LastCombatTime) < lastTime)
                {
                    m_Killer = info.Attacker;
                    lastTime = (DateTime.UtcNow - info.LastCombatTime);
                }

                if (addToAggressors && !info.CriminalAggression)
                    m_Aggressors.Add(info.Attacker);
            }

            for (int i = 0; i < owner.Aggressed.Count; ++i)
            {
                AggressorInfo info = owner.Aggressed[i];

                if ((DateTime.UtcNow - info.LastCombatTime) < lastTime)
                {
                    m_Killer = info.Defender;
                    lastTime = (DateTime.UtcNow - info.LastCombatTime);
                }

                if (addToAggressors)
                    m_Aggressors.Add(info.Defender);
            }

            if (!addToAggressors)
            {
                BaseCreature bc = (BaseCreature)owner;

                Mobile master = bc.GetMaster();
                if (master != null)
                    m_Aggressors.Add(master);

                List<DamageStore> rights = BaseCreature.GetLootingRights(bc.DamageEntries, bc.HitsMax);
                for (int i = 0; i < rights.Count; ++i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight)
                        m_Aggressors.Add(ds.m_Mobile);
                }
            }

            BeginDecay(m_DefaultDecayTime);            
        }

        private void AssignInstancedLoot()
        {
            if (m_InstancedItems == null)
                m_InstancedItems = new Dictionary<Mobile, List<Item>>();
        }

        public Corpse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)10); // version

            writer.WriteDeltaTime(m_TimeOfDeath);

            List<KeyValuePair<Item, Point3D>> list = (m_RestoreTable == null ? null : new List<KeyValuePair<Item, Point3D>>(m_RestoreTable));
            int count = (list == null ? 0 : list.Count);

            writer.Write(count);

            for (int i = 0; i < count; ++i)
            {
                KeyValuePair<Item, Point3D> kvp = list[i];
                Item item = kvp.Key;
                Point3D loc = kvp.Value;

                writer.Write(item);

                if (item.Location == loc)
                {
                    writer.Write(false);
                }
                else
                {
                    writer.Write(true);
                    writer.Write(loc);
                }
            }

            writer.Write(m_VisitedByTaxidermist);

            writer.Write(m_DecayTimer != null);

            if (m_DecayTimer != null)
                writer.WriteDeltaTime(m_DecayTime);

            writer.Write(m_Looters);
            writer.Write(m_Killer);

            writer.Write((bool)m_Carved);

            writer.Write(m_Aggressors);

            writer.Write(m_Owner);

            writer.Write(m_NoBones);

            writer.Write((string)m_CorpseName);

            writer.Write((int)m_AccessLevel);
            writer.Write((Guild)m_Guild);
            writer.Write((int)m_Kills);
            writer.Write((bool)m_Criminal);

            writer.Write(m_EquipItems);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 10:
                    {
                        m_TimeOfDeath = reader.ReadDeltaTime();

                        goto case 9;
                    }
                case 9:
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; ++i)
                        {
                            Item item = reader.ReadItem();

                            if (reader.ReadBool())
                                SetRestoreInfo(item, reader.ReadPoint3D());
                            else if (item != null)
                                SetRestoreInfo(item, item.Location);
                        }

                        goto case 8;
                    }
                case 8:
                    {
                        m_VisitedByTaxidermist = reader.ReadBool();

                        goto case 7;
                    }
                case 7:
                    {
                        if (reader.ReadBool())
                            BeginDecay(reader.ReadDeltaTime() - DateTime.UtcNow);

                        goto case 6;
                    }
                case 6:
                    {
                        m_Looters = reader.ReadStrongMobileList();
                        m_Killer = reader.ReadMobile();

                        goto case 5;
                    }
                case 5:
                    {
                        m_Carved = reader.ReadBool();

                        goto case 4;
                    }
                case 4:
                    {
                        m_Aggressors = reader.ReadStrongMobileList();

                        goto case 3;
                    }
                case 3:
                    {
                        m_Owner = reader.ReadMobile();

                        goto case 2;
                    }
                case 2:
                    {
                        m_NoBones = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        m_CorpseName = reader.ReadString();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 10)
                            m_TimeOfDeath = DateTime.UtcNow;

                        if (version < 7)
                            BeginDecay(m_DefaultDecayTime);

                        if (version < 6)
                            m_Looters = new List<Mobile>();

                        if (version < 4)
                            m_Aggressors = new List<Mobile>();

                        m_AccessLevel = (AccessLevel)reader.ReadInt();
                        reader.ReadInt(); // guild reserve
                        m_Kills = reader.ReadInt();
                        m_Criminal = reader.ReadBool();

                        m_EquipItems = reader.ReadStrongItemList();

                        break;
                    }
            }
        }

        public override void SendInfoTo(NetState state, bool sendOplPacket)
        {
            base.SendInfoTo(state, sendOplPacket);

            if (((Body)Amount).IsHuman && ItemID == 0x2006)
            {
                if (state.ContainerGridLines)
                {
                    state.Send(new CorpseContent6017(state.Mobile, this));
                }
                else
                {
                    state.Send(new CorpseContent(state.Mobile, this));
                }

                state.Send(new CorpseEquip(state.Mobile, this));
            }
        }

        public bool IsCriminalAction(Mobile from)
        {
            if (from == m_Owner || from.AccessLevel >= AccessLevel.GameMaster)
                return false;

            Party p = Party.Get(m_Owner);

            if (p != null && p.Contains(from))
            {
                PartyMemberInfo pmi = p[m_Owner];

                if (pmi != null && pmi.CanLoot)
                    return false;
            }

            return (NotorietyHandlers.CorpseNotoriety(from, this) == Notoriety.Innocent);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (!base.CheckItemUse(from, item))
                return false;

            if (item != this)
                return CanLoot(from, item);

            return true;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (!base.CheckLift(from, item, ref reject))
                return false;

            TimeSpan lootTime = DateTime.UtcNow - m_TimeOfDeath;

            if (lootTime.TotalSeconds < 0.75)
            {
                Custom.AntiFastLoot.RegisterFastloot(from, lootTime);
                return false;
            }

            return CanLoot(from, item);
        }

        public override void OnItemUsed(Mobile from, Item item)
        {
            base.OnItemUsed(from, item);

            if (from != m_Owner)
                from.RevealingAction();

            if (item != this && IsCriminalAction(from))
                from.CriminalAction(true);

            if (from.AccessLevel == Server.AccessLevel.Player && !m_Looters.Contains(from))
                m_Looters.Add(from);
        }
        
        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            if (item != this && from != m_Owner)
                from.RevealingAction();

            if (item != this && IsCriminalAction(from))
                from.CriminalAction(true);
            
            if (from.AccessLevel == AccessLevel.Player && !m_Looters.Contains(from))
                m_Looters.Add(from);

            //Ocean Creature Advanced Decay Rate
            LandTile landTile = this.Map.Tiles.GetLandTile(this.X, this.Y);
            StaticTile[] tiles = this.Map.Tiles.GetStaticTiles(this.X, this.Y, true);

            bool landHasWater = false;
            bool staticHasWater = false;

            if ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311))
            {
                landHasWater = true;
            }

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];
                bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                if (isWater)
                {
                    staticHasWater = true;
                }
            }           

            //Last Item Lifted
            if ((landHasWater || staticHasWater) && this.TotalItems <= 1)
            {
                if (m_DecayTimer != null)
                {
                    m_DecayTimer.Stop();
                }

                //Empty PlayerMobile is 30 Seconds Decay
                if (this.Owner is PlayerMobile)
                {
                    BeginDecay(TimeSpan.FromSeconds(30));
                }

                //Empty Creature is 10 Seconds Decay
                else
                {
                    BeginDecay(TimeSpan.FromSeconds(10));
                }
            }
        }

        private class OpenCorpseEntry : ContextMenuEntry
        {
            public OpenCorpseEntry()
                : base(6215, 2)
            {
            }

            public override void OnClick()
            {
                Corpse corpse = Owner.Target as Corpse;

                if (corpse != null && Owner.From.CheckAlive())
                    corpse.Open(Owner.From, false);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Core.AOS && m_Owner == from && from.Alive)
                list.Add(new OpenCorpseEntry());
        }

        private Dictionary<Item, Point3D> m_RestoreTable;

        public bool GetRestoreInfo(Item item, ref Point3D loc)
        {
            if (m_RestoreTable == null || item == null)
                return false;

            return m_RestoreTable.TryGetValue(item, out loc);
        }

        public void SetRestoreInfo(Item item, Point3D loc)
        {
            if (item == null)
                return;

            if (m_RestoreTable == null)
                m_RestoreTable = new Dictionary<Item, Point3D>();

            m_RestoreTable[item] = loc;
        }

        public void ClearRestoreInfo(Item item)
        {
            if (m_RestoreTable == null || item == null)
                return;

            m_RestoreTable.Remove(item);

            if (m_RestoreTable.Count == 0)
                m_RestoreTable = null;
        }

        public bool CanLoot(Mobile from, Item item)
        {
            if (from.AccessLevel == AccessLevel.Player && !from.InRange(this.GetWorldLocation(), 2))
                return false;

            Region region = Region.Find(Location, Map);          

            if (!IsCriminalAction(from))
                return true;

            Map map = this.Map;

            if (map == null || (map.Rules & MapRules.HarmfulRestrictions) != 0)
                return false;

            return true;
        }

        public bool CheckLoot(Mobile from, Item item)
        {
            if (!CanLoot(from, item))
            {
                if (m_Owner == null || !m_Owner.Player)
                    from.SendLocalizedMessage(1005035); // You did not earn the right to loot this creature!
                else
                    from.SendLocalizedMessage(1010049); // You may not loot this corpse.

                return false;
            }
            else if (IsCriminalAction(from))
            {
                if (m_Owner == null || !m_Owner.Player)
                    from.SendLocalizedMessage(1005036); // Looting this monster corpse will be a criminal act!
                else
                    from.SendLocalizedMessage(1005038); // Looting this corpse will be a criminal act!
            }

            return true;
        }

        public virtual void Open(Mobile from, bool checkSelfLoot)
        {
            if (from.AccessLevel > AccessLevel.Player || from.InRange(this.GetWorldLocation(), 2))
            {

                #region Self Looting
                bool selfLoot = (checkSelfLoot && (from == m_Owner));

                if (selfLoot)
                {
                    List<Item> items = new List<Item>(this.Items);

                    bool gathered = false;
                    bool didntFit = false;

                    Container pack = from.Backpack;

                    bool checkRobe = true;

                    for (int i = 0; !didntFit && i < items.Count; ++i)
                    {
                        Item item = items[i];
                        Point3D loc = item.Location;

                        if ((item.Layer == Layer.Hair || item.Layer == Layer.FacialHair) || !item.Movable || !GetRestoreInfo(item, ref loc))
                            continue;

                        if (checkRobe)
                        {
                            DeathRobe robe = from.FindItemOnLayer(Layer.OuterTorso) as DeathRobe;

                            if (robe != null)
                            {
                                if (Core.SE)
                                {
                                    robe.Delete();
                                }
                                else
                                {
                                    Map map = from.Map;

                                    if (map != null && map != Map.Internal)
                                        robe.MoveToWorld(from.Location, map);
                                }
                            }
                        }

                        if (m_EquipItems.Contains(item) && from.EquipItem(item))
                        {
                            gathered = true;
                        }
                        else if (pack != null && pack.CheckHold(from, item, false, true))
                        {
                            item.Location = loc;
                            pack.AddItem(item);
                            gathered = true;
                        }
                        else
                        {
                            didntFit = true;
                        }
                    }

                    if (gathered && !didntFit)
                    {
                        m_Carved = true;

                        if (ItemID == 0x2006)
                        {
                            ProcessDelta();
                            SendRemovePacket();
                            ItemID = Utility.Random(0xECA, 9); // bone graphic
                            Hue = 0;
                            ProcessDelta();
                        }

                        from.PlaySound(0x3E3);
                        from.SendLocalizedMessage(1062471); // You quickly gather all of your belongings.
                        return;
                    }

                    if (gathered && didntFit)
                        from.SendLocalizedMessage(1062472); // You gather some of your belongings. The rest remain on the corpse.
                }

                #endregion

                if (!CheckLoot(from, null))
                    return;                

                base.OnDoubleClick(from);

                if (from != m_Owner)
                    from.RevealingAction();
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            Open(from, Core.AOS);
        }

        public override bool CheckContentDisplay(Mobile from)
        {
            return false;
        }

        public override bool DisplaysContent { get { return false; } }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (ItemID == 0x2006) // Corpse form
            {
                if (m_CorpseName != null)
                    list.Add(m_CorpseName);
                else
                    list.Add(1046414, this.Name); // the remains of ~1_NAME~
            }
            else // Bone form
            {
                list.Add(1046414, this.Name); // the remains of ~1_NAME~
            }
        }

        public override void OnAosSingleClick(Mobile from)
        {
            int hue = Notoriety.GetHue(NotorietyHandlers.CorpseNotoriety(from, this));
            ObjectPropertyList opl = this.PropertyList;

            if (opl.Header > 0)
                from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, opl.Header, Name, opl.HeaderArgs));
        }

        public override void OnSingleClick(Mobile from)
        {
            int hue = Notoriety.GetHue(NotorietyHandlers.CorpseNotoriety(from, this));

            if (ItemID == 0x2006) // Corpse form
            {
                if (m_CorpseName != null)
                    from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, hue, 3, "", m_CorpseName));
                else
                    from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, 1046414, "", Name));
            }
            else // Bone form
            {
                from.Send(new MessageLocalized(Serial, ItemID, MessageType.Label, hue, 3, 1046414, "", Name));
            }
        }

        public void Carve(Mobile from, Item item)
        {
            if (IsCriminalAction(from) && this.Map != null && (this.Map.Rules & MapRules.HarmfulRestrictions) != 0)
            {
                if (m_Owner == null || !m_Owner.Player)
                    from.SendLocalizedMessage(1005035); // You did not earn the right to loot this creature!
                else
                    from.SendLocalizedMessage(1010049); // You may not loot this corpse.

                return;
            }

            Mobile dead = m_Owner;

            if (m_Carved || dead == null)
            {
                from.SendLocalizedMessage(500485); // You see nothing useful to carve from the corpse.
            }

            else if (((Body)Amount).IsHuman && ItemID == 0x2006)
            {                
                // IPY ACHIEVEMENT TRIGGER: As a blue or gray player, cut the corpse of a red player
                if (from.Player && m_Owner.Player)
                {                    
                    if (from.ShortTermMurders < 5 && NotorietyHandlers.CorpseNotoriety(from, this) == Notoriety.Murderer)                    
                        AchievementSystemImpl.Instance.TickProgress(from, AchievementTriggers.Trigger_PlayerCorpseCut);                    
                }
                // END IPY ACHIEVEMENT TRIGGER

                new Blood(0x122D).MoveToWorld(Location, Map);

                new Torso().MoveToWorld(Location, Map);
                new LeftLeg().MoveToWorld(Location, Map);
                new LeftArm().MoveToWorld(Location, Map);
                new RightLeg().MoveToWorld(Location, Map);
                new RightArm().MoveToWorld(Location, Map);

                Head head = new Head(HeadType.Regular, dead);

                PlayerMobile pm_Dead = dead as PlayerMobile;

                if (pm_Dead != null)
                {
                    if (pm_Dead.Murderer)
                        head.PlayerType = PlayerType.Murderer;

                    if (pm_Dead.LastPlayerKilledBy != null)
                    {
                        head.Killer = pm_Dead.LastPlayerKilledBy;
                        head.KillerName = pm_Dead.LastPlayerKilledBy.RawName;
                        
                        if (pm_Dead.LastPlayerKilledBy.Murderer)
                            head.KillerType = PlayerType.Murderer;

                        else
                            head.KillerType = PlayerType.None;
                    } 
                }

                head.MoveToWorld(Location, Map);

                m_Carved = true;                                

                ProcessDelta();
                SendRemovePacket();
                ItemID = Utility.Random(0xECA, 9); // bone graphic
                Hue = 0;
                ProcessDelta();

                if (IsCriminalAction(from))
                    from.CriminalAction(true);
            }

            else if (dead is BaseCreature)            
                ((BaseCreature)dead).OnCarve(from, this, item);
            
            else            
                from.SendLocalizedMessage(500485); // You see nothing useful to carve from the corpse.            
        }
    }
}