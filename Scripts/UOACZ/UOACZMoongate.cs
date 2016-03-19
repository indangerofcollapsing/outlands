using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server
{
    public class UOACZMoongate : Item
    {
        public static List<UOACZMoongate> m_Instances = new List<UOACZMoongate>();

        public enum GateDirectionType
        {
            Entrance,
            Exit
        }

        private GateDirectionType m_GateDirection = GateDirectionType.Entrance;
        [CommandProperty(AccessLevel.GameMaster)]
        public GateDirectionType GateDirection
        {
            get { return m_GateDirection; }
            set
            {
                m_GateDirection = value;
            }
        }

        [Constructable]
        public UOACZMoongate(): base(0xF6C)
        {
            Visible = false;

            Movable = false;
            Light = LightType.Circle300;

            Hue = 2500;        

            m_Instances.Add(this);
        }

        public UOACZMoongate(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            switch (m_GateDirection)
            {
                case GateDirectionType.Entrance:
                    LabelTo(from, "To UOACZ");
                break;

                case GateDirectionType.Exit:
                    LabelTo(from, "To Britannia");
                break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!Visible)
            {
                if (from.AccessLevel == AccessLevel.Player)
                    return;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!player.Alive)
            {
                player.SendMessage("You are dead and cannot do that.");
                return;
            }

            //UOACZ Restrictions
            if (player.AccessLevel == AccessLevel.Player)
            {                
                if (Utility.GetDistance(from.Location, Location) >= 3)
                    return;

                switch (m_GateDirection)
                {
                    case GateDirectionType.Entrance:
                        if (player.Criminal)
                        {
                            player.SendMessage("You are currently a criminal and cannot enter.");
                            return;
                        }

                        if (player.Followers > 0)
                        {
                            player.SendMessage("You must stable your followers before entering.");
                            return;
                        }

                        if (player.HasTrade)
                        {
                            player.SendMessage("You must cancel your pending trade before entering.");
                            return;
                        }

                        if (!from.CanBeginAction(typeof(IncognitoSpell)))
                        {
                            from.SendMessage("You may not enter while under the effects of incognito.");
                            return;
                        }

                        if (!from.CanBeginAction(typeof(PolymorphSpell)))
                        {
                            from.SendMessage("You may not enter while under the effects of polymorph.");
                            return;
                        }

                        if (DisguiseTimers.IsDisguised(from))
                            from.SendMessage("You may not enter while being disguised.");

                        if (DateTime.UtcNow < player.LastCombatTime + UOACZSystem.CombatDelayBeforeEnteringMoongate)
                        {
                            DateTime cooldown = player.LastCombatTime + UOACZSystem.CombatDelayBeforeEnteringMoongate;

                            string nextActivationAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, cooldown, false, false, false, true, true);

                            player.SendMessage("You have been in combat recently and must wait another " + nextActivationAllowed + " before entering.");
                            return;
                        }

                        if (player.m_UOACZAccountEntry.NextEntryAllowed > DateTime.UtcNow && player.AccessLevel == AccessLevel.Player)
                        {
                            string nextEntranceAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.NextEntryAllowed, false, false, false, true, true);
                            from.SendMessage("You may not use this for another " + nextEntranceAllowed + ".");

                            return;
                        }
                    break;

                    case GateDirectionType.Exit:
                        if (player.HasTrade)
                        {
                            player.SendMessage("You must cancel your pending trade before exiting.");
                            return;
                        }
                        
                        if (DateTime.UtcNow < player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold)
                        {
                            DateTime cooldown = player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold;

                            string nextActivationAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, cooldown, false, false, false, true, true);

                            player.SendMessage("You have been in PvP recently and must wait another " + nextActivationAllowed + " before exiting.");
                            return;
                        }

                        if (player.m_UOACZAccountEntry.NextEntryAllowed > DateTime.UtcNow && player.AccessLevel == AccessLevel.Player)
                        {
                            string nextEntranceAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.NextEntryAllowed, false, false, false, true, true);
                            from.SendMessage("You may not use this for another " + nextEntranceAllowed + ".");

                            return;
                        }
                    break;
                }
            }

            switch (m_GateDirection)
            {
                case GateDirectionType.Entrance:
                    player.SendSound(0x103);
                    
                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        player.MoveToWorld(UOACZPersistance.DefaultHumanLocation, UOACZPersistance.DefaultMap);                        
                        return;
                    }

                    UOACZSystem.PlayerEnterUOACZRegion(player);
                break;

                case GateDirectionType.Exit:
                    player.SendSound(0x0FC);

                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        player.MoveToWorld(UOACZPersistance.DefaultBritainLocation, UOACZPersistance.DefaultBritainMap);
                        return;
                    }

                    UOACZDestination destination = UOACZDestination.GetRandomExit(player.Murderer);

                    if (destination != null)
                        player.MoveToWorld(destination.Location, destination.Map);

                    else                    
                        player.MoveToWorld(UOACZPersistance.DefaultBritainLocation, UOACZPersistance.DefaultBritainMap);                                     
                break;
            }

            if (player.AccessLevel == AccessLevel.Player)
                player.m_UOACZAccountEntry.NextEntryAllowed = DateTime.UtcNow + UOACZSystem.DelayBetweenMoongateActivation;            
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write((int)m_GateDirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_GateDirection = (GateDirectionType)reader.ReadInt();
            }

            //-------

            m_Instances.Add(this);
        }
    }
}