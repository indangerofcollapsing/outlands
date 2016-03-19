using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class UOACZBaseConstructionDeed : Item
    {
        public virtual UOACZConstructionTile.ConstructionObjectType ConstructionType { get { return UOACZConstructionTile.ConstructionObjectType.Fortification; } }
        public virtual Type ConstructableObject { get { return typeof(UOACZBoilingOilCauldron); } }
        public virtual string DisplayName { get { return "boiling oil cauldron"; } }
        
        [Constructable]
        public UOACZBaseConstructionDeed(): base(0x14F0)
        {
            Name = "a construction deed";
            Weight = 0.1;
        }

        public UOACZBaseConstructionDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + DisplayName + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            player.SendMessage("Target the construction site for this item.");
            player.Target = new ConstructionTileTarget(this);  
        }

        public class ConstructionTileTarget : Target
        {
            private UOACZBaseConstructionDeed m_ConstructionDeed;

            public ConstructionTileTarget(UOACZBaseConstructionDeed constructionDeed): base(25, false, TargetFlags.None, false)
            {
                m_ConstructionDeed = constructionDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;
                if (m_ConstructionDeed == null) return;
                if (m_ConstructionDeed.Deleted) return;

                if (!m_ConstructionDeed.IsChildOf(from.Backpack))
                {
                    player.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                if (target is UOACZConstructionTile)
                {
                    UOACZConstructionTile constructionTile = target as UOACZConstructionTile;

                    if (Utility.GetDistance(player.Location, constructionTile.Location) > 1 || (Math.Abs(player.Location.Z - constructionTile.Location.Z) > 20))
                    {
                        player.SendMessage("You are too far away from that construction location.");
                        return;
                    }

                    if (constructionTile.Constructable != null)
                    {
                        player.SendMessage("That construction location already has something under construction.");
                        return;
                    }

                    constructionTile.PlaceObject(player, m_ConstructionDeed);
                }

                else
                {
                    player.SendMessage("That is not a valid construction location.");
                    return;
                }               
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}