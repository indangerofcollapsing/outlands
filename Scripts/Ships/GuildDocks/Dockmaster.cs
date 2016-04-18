using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.Pirates;
using Server.Multis;
using Server.Guilds;
using Server.Regions;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
    public class Dockmaster : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        private List<DockEntry> _dockedBoats = new List<DockEntry>();

        private BaseGuildDock m_Dock;
        public List<BaseBoat> m_RepairingBoats = new List<BaseBoat>();
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Guild Guild
        {
            get { if (m_Dock == null) return null; return m_Dock.m_Guild; }
        }

        [Constructable]
        public Dockmaster()
            : base("the Dockmaster")
        {
            m_Dock = null;
        }

        public Dockmaster(BaseGuildDock dock)
            : base("the dockmaster")
        {
            m_Dock = dock;

            if (m_Dock != null && Guild != null)
            {
                DisplayGuildTitle = true;
                GuildTitle = "dockmaster";
            }
        }

        public override void InitSBInfo()
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Dock != null)
                m_Dock.Delete();

            base.OnAfterDelete();
        }

        public override void InitOutfit()
        {
            this.Body = 0x190;
            this.Name = NameList.RandomName("male");
            Title = "the dockmaster";

            AddItem(new Shirt(Utility.RandomDyedHue()));
            AddItem(new FullApron(Utility.RandomNeutralHue()));
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new Boots());

            AddItem(new Hammer());

            Utility.AssignRandomHair(this);
        }

        public Dockmaster(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 16))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            string said = e.Speech;

            /*
            if (!e.Handled && from is PlayerMobile && from.InRange(this.Location, 16))
            {
                if (said.IndexOf("repair") != -1)
                {
                    BaseBoat boat = BaseBoat.FindBoatAt(from.Location, from.Map);

                    if (boat == null)
                    {
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "You must be in a boat to request repairs.");
                    }

                    else if (boat.Owner != from)
                    {
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "Only the owner of a boat can request repairs.");
                    }

                    else if (m_RepairingBoats.Contains(boat))
                    {
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "Your boat is already being repaired.");
                    }

                    else if (boat.m_LastCombatTime + boat.TimeNeededToBeOutOfCombat >= DateTime.UtcNow)
                    {
                        from.SendMessage("The ship has been been in combat too recently to initiate dock repairs.");
                        return;
                    }

                    else if (boat.m_TimeLastMoved + boat.DryDockMinimumLastMovement >= DateTime.UtcNow)
                    {
                        from.SendMessage("The ship has not been stationary long enough to to initiate dock repairs.");
                        return;
                    }

                    else if (boat.HitPoints == boat.MaxHitPoints && boat.SailPoints == boat.MaxSailPoints && boat.GunPoints == boat.MaxGunPoints)
                    {
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "Your boat is not damaged.");
                        return;
                    }

                    else if (Guild == null)
                    {
                        int costPerTick = 50;

                        Timer m_Timer = new RepairTimer(boat, this, costPerTick);

                        m_Timer.Start();
                        m_RepairingBoats.Add(boat);
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "As you are not affiliated with this dock, we will charge a fee of 50 gold per repair interval. Repairs will begin momentarily.");
                    }

                    else if (from.Guild == null || from.Guild != Guild)
                    {
                        int costPerTick = 25;

                        Timer m_Timer = new RepairTimer(boat, this, costPerTick);

                        m_Timer.Start();
                        m_RepairingBoats.Add(boat);
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "As you are not affiliated with this dock, we will charge a fee of 25 gold per repair interval. Repairs will begin momentarily.");
                    }

                    else
                    {
                        int costPerTick = 0;

                        Timer m_Timer = new RepairTimer(boat, this, costPerTick);

                        m_Timer.Start();
                        m_RepairingBoats.Add(boat);
                        PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "Very good! We'll initiate repairs shortly!.");
                    }
                }

                else if (said.IndexOf("demolish") != -1)
                    Demolish(from);
            }
            */

            base.OnSpeech(e);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.WriteEncodedInt(_dockedBoats.Count);

            foreach (DockEntry de in _dockedBoats)
                de.Serialize(writer);

            writer.WriteItem(m_Dock);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadEncodedInt();
                        for (int i = 0; i < count; i++)
                            _dockedBoats.Add(new DockEntry(reader));

                        goto case 0;
                    }

                case 0:
                    {
                        m_Dock = (BaseGuildDock)reader.ReadItem();
                        break;
                    }
            }
        }

        private void Demolish(Mobile from)
        {
            if (m_Dock == null || m_Dock.m_Guild == null || m_Dock.m_Guild.Leader != from)
                return;

            MultiComponentList mcl = m_Dock.Components;

            IPooledEnumerable eable = this.Map.GetObjectsInBounds(new Rectangle2D(m_Dock.X + mcl.Min.X, m_Dock.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (IEntity o in eable)
            {
                if (o == this || o == m_Dock)
                    continue;

                if (o is Item && ((Item)o).Visible == false)
                    continue;

                int tileX = m_Dock.X - o.X - mcl.Min.X;
                int tileY = m_Dock.Y - o.Y + mcl.Min.Y;

                if (tileX < 0 || tileX > mcl.Tiles.Length || tileY < 0 || tileY > mcl.Tiles[tileX].Length || mcl.Tiles[tileX][tileY].Length == 0)
                    continue;

                eable.Free();
                PublicOverheadMessage(MessageType.Regular, SpeechHue, true, "You must clear the dock and surrounding waters prior to re-deeding.");
                return;
            }

            eable.Free();

            this.Delete();

            DockDeed dockDeed = new DockDeed();
            dockDeed.PlayerClassOwner = from;
            from.AddToBackpack(dockDeed);

            from.SendMessage("A new dock deed has been placed in your pack.");
        }

        private class DockEntry
        {
            private BaseDockedBoat _boat;
            private Mobile _owner;
            private DateTime _time;

            public BaseDockedBoat Boat { get { return _boat; } }
            public Mobile Owner { get { return _owner; } }
            public DateTime Time { get { return _time; } }

            public DockEntry(Mobile owner, BaseDockedBoat boat, DateTime time)
            {
                _boat = boat;
                _owner = owner;
                _time = time;
            }

            public void Serialize(GenericWriter writer)
            {
                writer.WriteEncodedInt(0); // version

                writer.WriteItem((BaseDockedBoat)_boat);
                writer.WriteMobile((Mobile)_owner);
                writer.Write((DateTime)_time);
            }

            public DockEntry(GenericReader reader)
            {
                int version = reader.ReadEncodedInt();

                switch (version)
                {
                    case 0:
                        {
                            _boat = reader.ReadItem() as BaseDockedBoat;
                            _owner = (Mobile)reader.ReadMobile();
                            _time = (DateTime)reader.ReadDateTime();
                            break;
                        }
                }

            }
        }

        private class RepairTimer : Timer
        {
            private BaseBoat m_Boat;
            private Dockmaster m_Dockmaster;
            private int m_CostPerTick;

            public RepairTimer(BaseBoat boat, Dockmaster master, int costPerTick)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5))
            {
                m_Boat = boat;
                m_Dockmaster = master;
                m_CostPerTick = costPerTick;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Dockmaster == null || m_Dockmaster.Deleted || m_Boat == null || m_Boat.Deleted || m_Boat.Owner == null)
                {
                    if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                        m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                    Stop();

                    return;
                }

                if (m_Boat.TimeLastMoved + m_Boat.DryDockMinimumLastMovement >= DateTime.UtcNow)
                {
                    if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                        m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                    Stop();

                    m_Boat.Owner.SendMessage("The ship must remain stationary to be continue repairs.");
                    return;
                }

                if (m_Boat.LastCombatTime + m_Boat.TimeNeededToBeOutOfCombat >= DateTime.UtcNow)
                {
                    if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                        m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                    Stop();

                    m_Boat.Owner.SendMessage("The ship has been in combat too recently to continue repairs.");
                    return;
                }

                if (m_Dockmaster.InRange(m_Boat.Location, 15))
                {
                    int repairPointsPerTick = (int)(ShipRepairTool.hullRepairPercent * (double)m_Boat.MaxHitPoints);
                    int repairPointsLeft = repairPointsPerTick;

                    int shipHoldGold = 0;
                    int ownerBackpackGold = 0;
                    int ownerBankGold = Banker.GetBalance(m_Boat.Owner);

                    //Check Owner's Backpack
                    if (m_Boat.Owner.Backpack != null)
                    {
                        foreach (Item item in m_Boat.Owner.Backpack.Items)
                        {
                            if (item.GetType() == typeof(Gold))
                                ownerBackpackGold += item.Amount;
                        }
                    }

                    //Check Ship's Hold
                    if (m_Boat.Hold != null)
                    {
                        foreach (Item item in m_Boat.Hold.Items)
                        {
                            if (item.GetType() == typeof(Gold))
                                shipHoldGold += item.Amount;
                        }
                    }

                    //Player Must Pay For Repair Fees + Enough Gold Exists Between Player's Pack, Ship's Hold, and Owner's Bank to Pay for Repair Costs
                    if (m_CostPerTick > 0 && (ownerBackpackGold + shipHoldGold + +ownerBankGold) >= m_CostPerTick)
                    {
                        int costRemaining = m_CostPerTick;

                        //Backpack
                        if (ownerBackpackGold > 0)
                        {
                            foreach (Item item in m_Boat.Owner.Backpack.Items)
                            {
                                if (costRemaining == 0)
                                    break;

                                if (item.GetType() == typeof(Gold))
                                {
                                    if (item.Amount > costRemaining)
                                    {
                                        item.Amount -= costRemaining;
                                        costRemaining = 0;
                                    }

                                    else
                                    {
                                        costRemaining -= item.Amount;
                                        item.Delete();
                                    }
                                }
                            }
                        }

                        //Ship's Hold
                        if (shipHoldGold > 0)
                        {
                            foreach (Item item in m_Boat.Hold.Items)
                            {
                                if (costRemaining == 0)
                                    break;

                                if (item.GetType() == typeof(Gold))
                                {
                                    if (item.Amount > costRemaining)
                                    {
                                        item.Amount -= costRemaining;
                                        costRemaining = 0;
                                    }

                                    else
                                    {
                                        costRemaining -= item.Amount;
                                        item.Delete();
                                    }
                                }
                            }
                        }

                        //Owner's Bank
                        if (ownerBankGold > 0)
                            Banker.Withdraw(m_Boat.Owner, costRemaining);

                        Gold goldPile = new Gold(m_CostPerTick);
                        Effects.PlaySound(m_Boat.Location, m_Boat.Map, goldPile.GetDropSound());
                        goldPile.Delete();
                    }

                    else
                    {
                        if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                            m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                        this.Stop();

                        m_Dockmaster.PublicOverheadMessage(MessageType.Regular, m_Dockmaster.SpeechHue, true, "You do not have enough gold in between your backpack, ship's hold, and bank to continue making repairs to this ship.");

                        return;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate { Effects.PlaySound(m_Boat.Location, m_Boat.Map, 0x23D); });

                    if (!m_Dockmaster.Mounted && m_Dockmaster.Body.IsHuman)
                        Timer.DelayCall(TimeSpan.FromSeconds(0.5), delegate { m_Dockmaster.Animate(11, 5, 1, true, false, 0); });

                    //Hull
                    if (m_Boat.HitPoints < m_Boat.MaxHitPoints && repairPointsLeft > 0)
                    {
                        if (m_Boat.HitPoints + repairPointsLeft < m_Boat.MaxHitPoints)
                        {
                            int amountToRepair = m_Boat.MaxHitPoints - m_Boat.HitPoints;

                            m_Boat.HitPoints = m_Boat.MaxHitPoints;
                            repairPointsLeft -= amountToRepair;
                        }

                        else
                        {
                            m_Boat.HitPoints += repairPointsPerTick;
                            repairPointsLeft -= repairPointsPerTick;
                        }
                    }

                    //Sail
                    if (m_Boat.SailPoints < m_Boat.MaxSailPoints && repairPointsLeft > 0)
                    {
                        if (m_Boat.SailPoints + repairPointsLeft < m_Boat.MaxSailPoints)
                        {
                            int amountToRepair = m_Boat.MaxSailPoints - m_Boat.SailPoints;

                            m_Boat.SailPoints = m_Boat.MaxSailPoints;
                            repairPointsLeft -= amountToRepair;
                        }

                        else
                        {
                            m_Boat.SailPoints += repairPointsPerTick;
                            repairPointsLeft -= repairPointsPerTick;
                        }
                    }

                    //Gun
                    if (m_Boat.GunPoints < m_Boat.MaxGunPoints && repairPointsLeft > 0)
                    {
                        if (m_Boat.GunPoints + repairPointsLeft < m_Boat.MaxGunPoints)
                        {
                            int amountToRepair = m_Boat.MaxGunPoints - m_Boat.GunPoints;

                            m_Boat.GunPoints = m_Boat.MaxGunPoints;
                            repairPointsLeft -= amountToRepair;
                        }

                        else
                        {
                            m_Boat.GunPoints += repairPointsPerTick;
                            repairPointsLeft -= repairPointsPerTick;
                        }
                    }

                    if (m_Boat.HitPoints == m_Boat.MaxHitPoints && m_Boat.SailPoints == m_Boat.MaxSailPoints && m_Boat.GunPoints == m_Boat.MaxGunPoints)
                    {
                        if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                            m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                        this.Stop();

                        m_Dockmaster.PublicOverheadMessage(MessageType.Regular, m_Dockmaster.SpeechHue, true, "Your ship has been fully repaired.");

                        return;
                    }
                }

                else
                {
                    m_Boat.Owner.SendMessage("Your boat is too far away to repair any further.");

                    if (m_Dockmaster.m_RepairingBoats.Contains(m_Boat))
                        m_Dockmaster.m_RepairingBoats.Remove(m_Boat);

                    this.Stop();
                }
            }
        }

    }
}