using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;
using Server.Mobiles;
using Server.Custom;
using Server.Spells;

namespace Server.Items
{
	public class UOACZFishingPole : Item
	{
        private int m_Charges = 50;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public static int MaxCharges = 50;

		[Constructable]
		public UOACZFishingPole() : base( 0x0DC0 )
		{
            Name = "fishing pole";

			Layer = Layer.TwoHanded;
			Weight = 3.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "uses remaining: " + m_Charges.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;

            Item item = from.FindItemOnLayer(Server.Layer.TwoHanded);

            if (item != this)
            {
                from.SendMessage("You must equip this fishing pole in order to use it.");
                return;
            }

            if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
            {
                player.SendMessage("You must wait a moment before using that.");
                return;
            }

            from.SendMessage("Target the fishing location to fish from.");
            from.Target = new UOACZFishingTarget(this);
        }

        public class UOACZFishingTarget : Target
        {
            private UOACZFishingPole m_UOACZFishingPole;

            public UOACZFishingTarget(UOACZFishingPole fishingPole): base(15, true, TargetFlags.None)
            {
                m_UOACZFishingPole = fishingPole;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_UOACZFishingPole.Deleted || m_UOACZFishingPole.RootParent != from) return;
                if (from == null) return;
                if (from.Deleted || !from.Alive) return;

                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
                {
                    player.SendMessage("You must wait a moment before using that.");
                    return;
                }

                IPoint3D targetLocation = target as IPoint3D;

                if (targetLocation == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;
                
                SpellHelper.GetSurfaceTop(ref targetLocation);

                Point3D location = new Point3D(targetLocation);

                UOACZScavengeFishing scavengeFishing = null;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(location, 1);

                foreach (Item item in nearbyItems)
                {
                    if (item is UOACZScavengeFishing)
                    {
                        scavengeFishing = item as UOACZScavengeFishing;
                        break;
                    }
                }

                nearbyItems.Free();

                if (target is UOACZScavengeFishing)
                    scavengeFishing = target as UOACZScavengeFishing;

                if (scavengeFishing != null)
                {
                    if (Utility.GetDistance(player.Location, scavengeFishing.Location) > scavengeFishing.InteractionRange)
                    {
                        from.SendMessage("You are too far away to use that.");
                        return;
                    }

                    int interactionCount = scavengeFishing.GetInteractions(player);

                    if (scavengeFishing.YieldsRemaining == 0 || interactionCount >= scavengeFishing.MaxPlayerInteractions)
                    {
                        player.SendMessage(scavengeFishing.NoYieldRemainingText);
                        return;
                    }

                    scavengeFishing.Interact(player);
                }

                else
                {
                    from.SendMessage("There doesn't appear to be fish there. You should look for water with fish swimming near the surface.");
                    return;
                }
            }
        }

		public override bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
		{
			if ( base.CheckConflictingLayer( m, item, layer ) )
				return true;

			if ( layer == Layer.OneHanded )
			{
				m.SendLocalizedMessage( 500214 ); // You already have something in both hands.
				return true;
			}

			return false;
		}

		public UOACZFishingPole( Serial serial ) : base( serial )
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version    

            //Version 0
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
	}
}