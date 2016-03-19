using Server;
using System;
using System.Collections;
using Server.Multis;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Guilds;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public class BindDockDeedGump : Gump
    {
        DockDeed m_DockDeed;
        PlayerMobile m_Player;

        public BindDockDeedGump(DockDeed dockDeed, PlayerMobile player): base(50, 50)
        {
            m_DockDeed = dockDeed;
            m_Player = player;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(90, 150, 380, 185, 5054);
            AddBackground(100, 160, 360, 165, 3000);

            AddLabel(110, 175, 0, @"The following number of doubloons will be removed");
            AddLabel(110, 197, 0, @"from your bank box and the dock deed will be bound to you");
            AddHtml(118, 230, 320, 17, @"<center>" + dockDeed.DoubloonCost.ToString() + "</center>", (bool)false, (bool)false);
            AddItem(255, 255, 2539);

            AddButton(170, 285, 247, 248, 1, GumpButtonType.Reply, 0); //Okay
            AddButton(310, 285, 243, 248, 2, GumpButtonType.Reply, 0); //Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1:
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= m_DockDeed.DoubloonCost)
                    {
                        if (Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), m_DockDeed.DoubloonCost))
                        {
                            Doubloon doubloonPile = new Doubloon(m_DockDeed.DoubloonCost);
                            from.SendSound(doubloonPile.GetDropSound());
                            doubloonPile.Delete();

                            m_DockDeed.PlayerClassOwner = m_Player;
                            m_Player.SendMessage("You claim the dock deed as your own.");
                        }

                        else
                            m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this dock deed.");
                    }

                    else
                        m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this dock deed.");

                    break;
            }
        }
    }

    public class ConfirmDockGump : Gump
    {
        DockDeed m_Deed;
        Mobile m_from;

        public ConfirmDockGump(Mobile from, DockDeed deed): base(150, 200)
        {
            m_Deed = deed;
            m_from = from;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 0, 1226);
            this.AddImage(87, 78, 2529);
            this.AddCheck(20, 262, 210, 211, false, (int)Buttons.chkAccept);
            this.AddButton(160, 262, 2450, 248, (int)Buttons.btnOK, GumpButtonType.Reply, 0);
            this.AddLabel(43, 262, 0x0, @"I Accept");
            this.AddLabel(52, 4, 0x0, @"Guild Dock Deed");
            this.AddLabel(25, 40, 0x0, @"Congratulations! You have");
            this.AddLabel(25, 60, 0x0, @"received a Guild Dock deed.");
            this.AddLabel(25, 130, 0x0, @"Stand on a shoreline near");
            this.AddLabel(25, 150, 0x0, @"your guildstone and face");
            this.AddLabel(25, 170, 0x0, @"one of the four cardinal");
            this.AddLabel(25, 190, 0x0, @"directions.  Poor placement");
            this.AddLabel(25, 210, 0x0, @"will result in dock removal");
            this.AddLabel(25, 230, 0x0, @"WITHOUT compensation.");
        }

        public enum Buttons
        {
            btnCancel,
            chkAccept,
            btnOK,
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                if (info.IsSwitched((int)Buttons.chkAccept))                
                    m_Deed.GumpOK(state.Mobile);                
                else
                {
                    state.Mobile.SendMessage("You must accept the terms.");
                    state.Mobile.SendGump(new ConfirmDockGump(state.Mobile, m_Deed));
                }
            }
        }
    }

    public class DockDeed : Item
    {
        public virtual int DoubloonCost { get { return 2500; } }

        [Constructable]
        public DockDeed(): base(0x14F0)
        {
            Name = "a guild dock deed";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
        }

        public DockDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", Name));

                if (PlayerClassRestricted)
                {
                    if (PlayerClassOwner == null)
                    {
                        if (DoubloonCost > 0)
                            ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Requires " + DoubloonCost.ToString() + " doubloons]"));
                    }

                    else
                        ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Bound to " + PlayerClassOwner.RawName + "]"));
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            if (PlayerClassRestricted)
            {
                if (PlayerClassOwner == null)
                {
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= DoubloonCost)
                    {
                        from.CloseAllGumps();
                        from.SendGump(new BindDockDeedGump(this, pm_From));

                        return;
                    }

                    else
                    {
                        from.SendMessage("You must have at least " + DoubloonCost.ToString() + " doubloons in your bank to claim this dockdeed as your own.");
                        return;
                    }
                }

                else
                {
                    if (PlayerClassOwner != pm_From)
                    {
                        from.SendMessage("Only the Pirate owner of this item may use it");
                        return;
                    }
                }
            }
            
            if (!IsChildOf(from.Backpack))            
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            
            else if (from.HasGump(typeof(ConfirmDockGump)))            
                from.SendMessage("You already have a Dock Gump open.");
            
            else if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("You use your GM powers to read the deed.");
                from.SendGump(new ConfirmDockGump(from, this));
            }

            else if (from.Guild == null)            
                from.SendMessage("You must be in a guild to place a dock.");
            
            else if (BaseGuildDock.m_GuildDockDictionary.ContainsKey((Guild)from.Guild))            
                from.SendMessage("Your guild already owns a dock.");
            
            else if (((Guild)from.Guild).Leader != from)            
                from.SendMessage("Only guild leaders can place guild docks.");
            
            else
            {              
                from.SendGump(new ConfirmDockGump(from, this));                
            }
        }

        public void GumpOK(Mobile from)
        {
            if (!IsChildOf(from.Backpack))            
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            
            else if (from.AccessLevel >= AccessLevel.GameMaster)            
                SetDockPlacement(from);
            
            else if (from.Guild == null)            
                from.SendMessage("You must be in a guild to place a dock.");
            
            else if (BaseGuildDock.m_GuildDockDictionary.ContainsKey((Guild)from.Guild))            
                from.SendMessage("Your guild already owns a dock.");

            else if (((Guild)from.Guild).Leader != from)
                from.SendMessage("Only guild leaders can place guild docks.");

            else
            {
                if (from.Z > 5)
                    from.SendMessage("Guild docks must be placed on lower ground.");

                else if (from.Z < -5)
                    from.SendMessage("Guild docks must be placed on higher ground.");

                else
                    SetDockPlacement(from);
            }
        }

        public void SetDockPlacement(Mobile from)
        {
            switch (from.Direction & Direction.Mask)
            {
                case Direction.South: { PlaceDock(from, new Point3D(from.X, from.Y + 9, from.Z)); } break;
                case Direction.East: { PlaceDock(from, new Point3D(from.X + 9, from.Y, from.Z)); } break;
                case Direction.North: { PlaceDock(from, new Point3D(from.X, from.Y - 9, from.Z)); } break;
                case Direction.West: { PlaceDock(from, new Point3D(from.X - 8, from.Y, from.Z)); } break;
                
                default: { from.SendMessage("Please face North, South, East, or West looking out towards an open body of water."); } break;
            }
        }

        private void PlaceDock(Mobile from, object o)
        {
            IPoint3D ip = o as IPoint3D;

            if (ip != null)
            {
                if (ip is Item)
                    ip = ((Item)ip).GetWorldTop();

                Point3D p = new Point3D(ip.X, ip.Y, from.Z - 4);

                Region reg = Region.Find(new Point3D(p), from.Map);

                if (from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing(from, p))
                {
                    switch (from.Direction & Direction.Mask)
                    {
                        case Direction.South: { this.OnPlacement(from, p, new SouthGuildDock((Guild)from.Guild)); } break;
                        case Direction.East: { this.OnPlacement(from, p, new EastGuildDock((Guild)from.Guild)); } break;
                        case Direction.North: { this.OnPlacement(from, p, new NorthGuildDock((Guild)from.Guild)); } break;
                        case Direction.West: { this.OnPlacement(from, p, new WestGuildDock((Guild)from.Guild)); } break;
                        
                        default: { from.SendMessage("Please face North, South, East, or West looking out towards an open body of water."); } break;
                    }
                }
                else if (reg.IsPartOf(typeof(TreasureRegion)))
                    from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                
                else
                    from.SendLocalizedMessage(501265); // Docks can not be created in this area.
            }
        }

        public void OnPlacement(Mobile from, Point3D p, BaseGuildDock dock)
        {
            if (Deleted)
            {
                dock.Delete();
                from.SendMessage("Cannot find the deed.");

                return;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(0142001); // That must be in your pack for you to use it.
                dock.Delete();
            }

            else
            {
                Point3D center = new Point3D(p.X, p.Y, p.Z);
                if (DockPlacement.ValidatePlacement(from, dock, center))
                {
                    dock.MoveToWorld(center, from.Map);
                    Delete();
                }

                else
                {
                    from.SendMessage("Unacceptable dock placement.");
                    dock.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}