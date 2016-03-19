using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using Server.Regions;
using Server.Custom;

namespace Server.Items
{
    public class NewbieDungeonRune : Item
    {
        public TimeSpan Cooldown = TimeSpan.FromMinutes(10);
        
        Point3D newbieDungeonTeleportLocation = new Point3D(6725, 437, 47);
        Map newbieDungeonMap = Map.Felucca;

        [Constructable]
        public NewbieDungeonRune(): base(0x1F14)
        {
            Name = "newbie dungeon rune";
            Hue = 2953;

            LootType = LootType.Newbied;
        }

        public NewbieDungeonRune(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(double click to teleport to the newbie dungeon)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(player.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }            

            if (!player.Young)
            {
                player.SendMessage("Only Young players may use this.");
                return;
            }

            if (player.Region is NewbieDungeonRegion)
            {
                player.SendMessage("You are already inside the newbie dungeon.");
                return;
            }

            if (!from.CanBeginAction(typeof(NewbieDungeonRune)))
            {
                from.SendMessage("You may only teleport to the newbie dungeon once every 10 minutes.");
                return;
            }

            if (Server.Misc.WeightOverloading.IsOverloaded(from))
            {
                from.SendMessage("Thou art too encumbered to use this item at the moment.");
                return;
            }

            if (from.Criminal)
            {
                from.SendMessage("You are currently a criminal and cannot use this item at the moment.");
                return;
            }

            if (!SpellHelper.CheckTravel(from, TravelCheckType.RecallFrom))
            {
                from.SendMessage("Usage of that item is not allowed in this region.");
                return;
            }
            
            if (DateTime.UtcNow < player.LastPlayerCombatTime + TimeSpan.FromSeconds(30))
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + TimeSpan.FromSeconds(30), false, true, true, true, true);
                player.SendMessage("You have been in combat too recently and must wait another " + timeRemaining + " before you may use this again.");

                return;
            }

            if (DateTime.UtcNow < player.LastPlayerCombatTime + TimeSpan.FromSeconds(60))
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + TimeSpan.FromSeconds(60), false, true, true, true, true);
                player.SendMessage("You have been in player-versus-player combat too recently and must wait another " + timeRemaining + " before you may use this again.");

                return;
            }

            Point3D location = player.Location;
            Map map = player.Map;

            player.SendMessage("You begin the teleportation ritual.");

            player.RevealingAction();

            player.BeginAction(typeof(NewbieDungeonRune));

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 5, true, 0, false, "", "");

            for (int a = 0; a < 5; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (player == null) return;
                    if (player.Deleted || !player.Alive)
                    {
                        player.EndAction(typeof(NewbieDungeonRune));
                        return;
                    }

                    player.Animate(17, 7, 1, true, false, 0);                    
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5.05), delegate
            {
                if (player == null) return;
                if (player.Deleted || !player.Alive)
                {
                    player.EndAction(typeof(NewbieDungeonRune));
                    return;
                }

                if (DateTime.UtcNow < player.LastPlayerCombatTime + TimeSpan.FromSeconds(30))
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + TimeSpan.FromSeconds(30), false, true, true, true, true);
                    player.SendMessage("You have been in combat too recently and must wait another " + timeRemaining + " before you may use this again.");
                    
                    player.EndAction(typeof(NewbieDungeonRune));

                    return;
                }

                if (DateTime.UtcNow < player.LastPlayerCombatTime + TimeSpan.FromSeconds(60))
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + TimeSpan.FromSeconds(60), false, true, true, true, true);
                    player.SendMessage("You have been in player-versus-player combat too recently and must wait another " + timeRemaining + " before you may use this again.");
                    
                    player.EndAction(typeof(NewbieDungeonRune));

                    return;
                }

                player.Location = newbieDungeonTeleportLocation;
                player.Map = newbieDungeonMap;
                player.SendSound(0x652);

                player.SendMessage("You teleport to the newbie dungeon.");

                Timer.DelayCall(Cooldown, delegate
                {
                    if (player != null)
                        player.EndAction(typeof(NewbieDungeonRune));
                });
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}