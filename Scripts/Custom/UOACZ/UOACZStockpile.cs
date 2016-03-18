using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Spells;
using Server.Network;

namespace Server.Items
{
    public class UOACZStockpile : UOACZBreakableStatic
    {
        public static List<UOACZStockpile> m_Instances = new List<UOACZStockpile>();

        public override int OverrideNormalItemId { get { return 11753; } }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId { get { return 11753; } }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId { get { return 11753; } }
        public override int OverrideHeavilyDamagedHue { get { return 0; } }

        public override int OverrideBrokenItemId { get { return 11753; } }
        public override int OverrideBrokenHue { get { return 0; } }

        private string m_DisplayName = "";
        [CommandProperty(AccessLevel.Counselor)]
        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }

        private double m_NextDamageStateNotification = .8;
        [CommandProperty(AccessLevel.Counselor)]
        public double NextDamageStateNotification
        {
            get { return m_NextDamageStateNotification; }
            set { m_NextDamageStateNotification = value; }
        }

        public static int StartingMaxHits = 5000;

        public double NotificationInterval = .2;

        [Constructable]
        public UOACZStockpile(): base()
        {           
            Name = "stockpile";
            ItemID = 11753;

            DeleteOnBreak = true;

            MaxHitPoints = StartingMaxHits;
            HitPoints = StartingMaxHits;

            m_Instances.Add(this);
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, DisplayName + " Stockpile");
            LabelTo(from, "[Durability: " + HitPoints.ToString() + "/" + MaxHitPoints.ToString() + "]");

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;
            if (player.m_UOACZAccountEntry.HumanProfile.Stockpile == null) return;
            
            LabelTo(from, "Your Items: " + player.m_UOACZAccountEntry.HumanProfile.Stockpile.TotalItems.ToString() + " / " + player.m_UOACZAccountEntry.HumanProfile.Stockpile.MaxItems.ToString() + "");                   
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            if (player.IsUOACZUndead)
            {
                base.OnDoubleClick(player);                

                return;
            }

            else if (player.IsUOACZHuman)
            {
                if (!from.InRange(GetWorldLocation(), InteractionRange))
                {
                    from.SendMessage("That is too far away.");
                    return;
                }

                bool needStockpile = false;

                if (player.m_UOACZAccountEntry.HumanProfile.Stockpile == null)
                    needStockpile = true;

                if (player.m_UOACZAccountEntry.HumanProfile.Deleted == null)
                    needStockpile = true;

                if (needStockpile)                
                    player.m_UOACZAccountEntry.HumanProfile.Stockpile = new UOACZStockpileContainer(player.m_UOACZAccountEntry.m_AccountUsername);                

                player.m_UOACZAccountEntry.HumanProfile.Stockpile.AccountEntry = player.m_UOACZAccountEntry;

                Point3D newLocation = new Point3D(Location.X, Location.Y, Location.Z);
                player.m_UOACZAccountEntry.HumanProfile.Stockpile.MoveToWorld(newLocation, from.Map);
                player.m_UOACZAccountEntry.HumanProfile.Stockpile.Z -= 1;

                player.m_UOACZAccountEntry.HumanProfile.Stockpile.Open(from);
            }
        }

        public override void ReceiveDamage(Mobile from, int damage, BreakableStatic.InteractionType interactionType)
        {
            base.ReceiveDamage(from, damage, interactionType);

            double hitPointsPercent = (double)HitPoints / (double)MaxHitPoints;

            if (m_NextDamageStateNotification > 0.05 && hitPointsPercent < m_NextDamageStateNotification && DisplayName != "")
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))                    
                        player.SendMessage(UOACZSystem.yellowTextHue, "The " + DisplayName + " Stockpile is under attack and at " + Utility.CreatePercentageString(m_NextDamageStateNotification) + " durability.");                    
                }

                m_NextDamageStateNotification -= NotificationInterval;
            }

            UOACZEvents.StockpileDamaged(false);
        }

        public override void BeforeBreak(Mobile from, InteractionType interactionType)
        {
            IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, 4);

            Queue m_Queue = new Queue();

            foreach (Item item in nearbyItems)
            {
                if (item is UOACZStockpileContainer)
                    m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                UOACZStockpileContainer stockpileContainer = (UOACZStockpileContainer)m_Queue.Dequeue();
                stockpileContainer.Internalize();
            }

            nearbyItems.Free();

            PublicOverheadMessage(Network.MessageType.Regular, 0, false, "*destroyed*");

            int projectiles = Utility.RandomMinMax(10, 20);

            for (int a = 0; a < projectiles; a++)
            {
                TimedStatic wood = new TimedStatic(Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), 3);
                wood.Name = "broken stockpile";
                
                Point3D woodLocation = new Point3D(Location.X + Utility.RandomMinMax(-5, 5), Location.Y + Utility.RandomMinMax(-5, 5), Location.Z);
                SpellHelper.AdjustField(ref woodLocation, Map, 12, false);

                wood.MoveToWorld(woodLocation, Map);
            }

            int particleSpeed = 8;

            for (int a = 0; a < projectiles; a++)
            {
                int debrisOffsetX = 0;
                int debrisOffsetY = 0;

                switch (Utility.RandomMinMax(1, 11))
                {
                    case 1: debrisOffsetX = -5; debrisOffsetY = 5; break;
                    case 2: debrisOffsetX = -4; debrisOffsetY = 4; break;
                    case 3: debrisOffsetX = -3; debrisOffsetY = 3; break;
                    case 4: debrisOffsetX = -2; debrisOffsetY = 2; break;
                    case 5: debrisOffsetX = -1; debrisOffsetY = 1; break;
                    case 6: debrisOffsetX = 0; debrisOffsetY = 0; break;
                    case 7: debrisOffsetX = 1; debrisOffsetY = -1; break;
                    case 8: debrisOffsetX = 2; debrisOffsetY = -2; break;
                    case 9: debrisOffsetX = 3; debrisOffsetY = -3; break;
                    case 10: debrisOffsetX = 4; debrisOffsetY = -4; break;
                    case 11: debrisOffsetX = 5; debrisOffsetY = -5; break;
                }

                Point3D newLocation = new Point3D(Location.X + debrisOffsetX, Location.Y + debrisOffsetY, Location.Z);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 2), Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), Map);

                newLocation.Z += 5;

                Effects.PlaySound(Location, Map, 0x50F);
                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), particleSpeed, 0, false, false, 0, 0);
            }
        }

        public override void AfterBreak(Mobile from, BreakableStatic.InteractionType interactionType)
        {
            base.AfterBreak(from, interactionType);

            UOACZEvents.StockpileDamaged(true);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (UOACZStockpile.m_Instances.Contains(this))
                UOACZStockpile.m_Instances.Remove(this);
        }

        public UOACZStockpile(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_DisplayName);
            writer.Write(m_NextDamageStateNotification);        
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_DisplayName = reader.ReadString();
            m_NextDamageStateNotification = reader.ReadDouble();

            //--------

            m_Instances.Add(this);
        }
    }
}
