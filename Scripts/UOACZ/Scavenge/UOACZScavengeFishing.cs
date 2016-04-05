using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;


namespace Server.Custom
{
    public class UOACZScavengeFishing : UOACZBaseScavengeObject
    {
        public override string NoYieldRemainingSingleClickText { get { return "(fished out)"; } }
        public override string LockedSingleClickText { get { return "(locked)"; } }

        public override string NoYieldRemainingText { get { return "It appears to have be devoid of life."; } }
        public override string InteractText { get { return "You begin fishing."; } }

        public override string ScavengeResultSuccessWithTrapText { get { return "Your fishing attempt was successful but something is amiss..."; } }
        public override string ScavengeResultSuccessText { get { return "You have fished up something."; } }
        public override string ScavengeResultFailWithTrapText { get { return "Your fishing attempt yields nothing, but something is amiss..."; } }
        public override string ScavengeResultFailText { get { return "You fish for some time but fail to find anything."; } }

        public override string ScavengeUndeadTrapText { get { return "*noise from fishing draws unwanted attention...*"; } }

        public override TimeSpan ScavengeDuration { get { return TimeSpan.FromSeconds(5); } }

        public UOACZScavengeFishingSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeFishing(): base()
        {
            Name = "fishing spot";
        }

        public override void Init()
        {
            ItemID = 3545;

            InteractionRange = 4;

            MaxPlayerInteractions = 10;
            StartingYieldCount = 30;

            ScavengeDifficulty = Utility.RandomMinMax(75, 150);

            if (Utility.RandomDouble() <= .25)
                TrapType = ScavengeTrapType.Undead;

            base.Init();
        }

        public override void CreateLoot()
        {
            for (int a = 0; a < StartingYieldCount; a++)
            {
                double itemResult = Utility.RandomDouble();

                if (itemResult <= .70)                
                    DropItem(new UOACZFish());                

                else if (itemResult <= .90)                
                    DropItem(new UOACZLargeFish());                

                else if (itemResult <= .98)                
                    DropItem(new UOACZBottleOfWater());                

                else                
                    DropItem(new UOACZHumanUpgradeToken());                
            }

            base.CreateLoot();
        }     

        public override bool CanInteract(PlayerMobile player)
        {
            Item item = player.FindItemOnLayer(Server.Layer.TwoHanded);

            if (!(item is UOACZFishingPole))
            {
                player.SendMessage("You must have a fishing pole equipped in order to use that.");
                return false;
            }

            UOACZFishingPole fishingPole = item as UOACZFishingPole;

            fishingPole.Charges--;

            if (fishingPole.Charges <= 0)
                fishingPole.Delete();

            return base.CanInteract(player);
        }

        public override void DoAction(PlayerMobile player)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player)) return;

            player.Animate(11, 5, 1, true, false, 0);
            player.RevealingAction();

            Point3D location = Location;
            Map map = Map;

            Effects.PlaySound(player.Location, player.Map, 0x33C);

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 10), player.Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z), map);

            Effects.SendMovingEffect(startLocation, endLocation, 574, 5, 0, false, false, 0, 0);

            double distance = Utility.GetDistanceToSqrt(startLocation.Location, endLocation.Location);
            double destinationDelay = (double)distance * .12;

            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
            {
                if (!UOACZPersistance.Active) return;

                Effects.SendLocationEffect(location, map, 0x352D, 16, 4);
                Effects.PlaySound(location, map, 0x364);

                TimedStatic bobber = new TimedStatic(574, ScavengeDuration.TotalSeconds - .5);
                bobber.Name = "fishing bobber";
                bobber.MoveToWorld(location, map);
            });
        }

        public override bool GetScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            bool scavengeResult = false;

            int minimumPlayerValue = 50;

            int playerFishingValue = Utility.RandomMinMax(0, minimumPlayerValue + (int)(Math.Round(player.Skills.Fishing.Value * UOACZSystem.scavengeSkillScalar)));
            int fishingTarget = Utility.RandomMinMax(0, ScavengeDifficulty);

            if (playerFishingValue >= fishingTarget)      
                scavengeResult = true;

            if (scavengeResult)
            {
                Effects.PlaySound(player.Location, player.Map, 0x025);

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableFishingItems++;
            }

            else
                Effects.PlaySound(player.Location, player.Map, 0x023);

            return scavengeResult;
        }

        public override void ResolveTrap(Mobile from)
        {
            PublicOverheadMessage(MessageType.Regular, UOACZSystem.yellowTextHue, false, ScavengeUndeadTrapText);

            int creatures = (int)(Math.Ceiling((double)TrapDifficulty / 50));

            for (int a = 0; a < creatures; a++)
            {
                Point3D spawnLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref spawnLocation, Map, 12, false);

                int threatLevel = UOACZPersistance.m_ThreatLevel - 30;
                UOACZBaseUndead bc_Creature = (UOACZBaseUndead)Activator.CreateInstance(UOACZBaseUndead.GetRandomUndeadType(0, threatLevel));

                if (bc_Creature != null)
                {
                    if (Map.CanSpawnMobile(spawnLocation))
                        bc_Creature.MoveToWorld(spawnLocation, Map);

                    else
                    {
                        if (UOACZSystem.IsUOACZValidMobile(from))
                        {
                            spawnLocation = new Point3D(from.Location.X + Utility.RandomList(-1, 1), from.Location.Y + Utility.RandomList(-1, 1), from.Location.Z);
                            SpellHelper.AdjustField(ref spawnLocation, Map, 12, false);

                            if (Map.CanSpawnMobile(spawnLocation))
                                bc_Creature.MoveToWorld(spawnLocation, Map);

                            else
                                bc_Creature.MoveToWorld(from.Location, Map);
                        }
                    }
                }
            }

            TrapType = ScavengeTrapType.None;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Items.Remove(this);
        }

        public UOACZScavengeFishing(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Spawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Spawner = (UOACZScavengeFishingSpawner)reader.ReadItem();
            }
        }
    }
}