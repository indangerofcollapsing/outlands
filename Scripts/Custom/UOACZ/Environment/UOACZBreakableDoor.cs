using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZBreakableDoor : UOACZBreakableStatic
    {
        public override int OverrideNormalItemId { get { return 13889; } }
        public override int OverrideNormalHue { get { return 2405; } }

        public override int OverrideLightlyDamagedItemId { get { return 13889; } }
        public override int OverrideLightlyDamagedHue { get { return 2405; } }

        public override int OverrideHeavilyDamagedItemId { get { return 13889; } }
        public override int OverrideHeavilyDamagedHue { get { return 2405; } }

        public override int OverrideBrokenItemId { get { return 1291; } }
        public override int OverrideBrokenHue { get { return 2406; } }

        public virtual int InteractSound { get { return 0x0F4; } }

        public enum DoorFacingType
        {
            EastWest,
            NorthSouth
        }

        public virtual DoorFacingType DoorFacing { get { return DoorFacingType.EastWest; } }

        [Constructable]
        public UOACZBreakableDoor(): base()
        {
            InteractionRange = 2;
        }

        public UOACZBreakableDoor(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.IsUOACZHuman && DamageState != DamageStateType.Broken)
            {
                if (Utility.GetDistance(player.Location, Location) > InteractionRange)
                {
                    player.SendMessage("You are too far away to use that.");
                    return;
                }

                if (!from.CanBeginAction(typeof(UOACZBreakableDoor)))
                {
                    from.SendMessage("You must wait a few moments before you may enter or exit again.");
                    return;
                }

                if (player.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                {
                    from.SendMessage("You are an outcast and not allowed to use this.");
                    return;
                }

                Direction directionToDoor = Utility.GetDirection(Location, player.Location);
                Direction exitDirection = Server.Direction.Down;

                if (DoorFacing == DoorFacingType.EastWest)
                {
                    switch (directionToDoor)
                    {
                        case Direction.Up: exitDirection = Direction.South; break;
                        case Direction.North: exitDirection = Direction.South; break;
                        case Direction.Right: exitDirection = Direction.South; break;

                        case Direction.East: exitDirection = Direction.South; break;

                        case Direction.Left: exitDirection = Direction.North; break;
                        case Direction.South: exitDirection = Direction.North; break;
                        case Direction.Down: exitDirection = Direction.North; break;

                        case Direction.West: exitDirection = Direction.North; break;
                    }
                }

                else
                {

                    switch (directionToDoor)
                    {
                        case Direction.Left: exitDirection = Direction.East; break;
                        case Direction.West: exitDirection = Direction.East; break;
                        case Direction.Up: exitDirection = Direction.East; break;

                        case Direction.North: exitDirection = Direction.East; break;

                        case Direction.Right: exitDirection = Direction.West; break;
                        case Direction.East: exitDirection = Direction.West; break;
                        case Direction.Down: exitDirection = Direction.West; break;

                        case Direction.South: exitDirection = Direction.West; break;
                    }
                }

                Point3D newLocation = SpecialAbilities.GetPointByDirection(Location, exitDirection);

                player.Location = newLocation;
                player.PlaySound(InteractSound);
                player.RevealingAction();

                from.BeginAction(typeof(UOACZBreakableDoor));

                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                {
                    if (from != null)
                        from.EndAction(typeof(UOACZBreakableDoor));
                });                
            }

            else
                base.OnDoubleClick(from);            
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
