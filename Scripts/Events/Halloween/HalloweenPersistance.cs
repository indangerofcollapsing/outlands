﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Accounting;
using Server.Custom;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server
{
    public static class HalloweenPersistance
    {
        public static HalloweenPersistanceItem PersistanceItem;

        public static List<TrickOrTreatPlayerEntry> m_TrickOrTreatPlayerEntries = new List<TrickOrTreatPlayerEntry>();

        public static void Initialize()
        {
            CommandSystem.Register("HalloweenTrickOrTreat", AccessLevel.GameMaster, new CommandEventHandler(HalloweenTrickOrTreat));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new HalloweenPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new HalloweenPersistanceItem();

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                });
            });
        }

        #region Commands

        [Usage("HalloweenTrickOrTreat")]
        [Description("Enable Halloween Trick or Treat Handling")]
        public static void HalloweenTrickOrTreat(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (arg.Length == 0)
            {
                if (m_TrickOrTreatEnabled)
                {
                    m_TrickOrTreatEnabled = false;
                    player.SendMessage("Halloween Trick or Treating: Disabled");
                }

                else
                {
                    m_TrickOrTreatEnabled = true;
                    player.SendMessage("Halloween Trick or Treating: Enabled");
                }
            }

            if (arg.Length == 1)
            {
                try
                {
                    string value = arg.GetString(0);

                    if (value == "enable")
                    {
                        if (m_TrickOrTreatEnabled)
                        {
                            player.SendMessage("Halloween Trick or Treating is already enabled.");
                            return;
                        }

                        else
                        {
                            player.SendMessage("Halloween Trick or Treating: Enabled.");
                            m_TrickOrTreatEnabled = true;

                            return;
                        }
                    }

                    else if (value == "disable")
                    {
                        if (!m_TrickOrTreatEnabled)
                        {
                            player.SendMessage("Halloween Trick or Treating is already disabled.");
                            return;
                        }

                        else
                        {
                            player.SendMessage("Halloween Trick or Treating: Disabled.");
                            m_TrickOrTreatEnabled = false;

                            return;
                        }
                    }
                }

                catch
                {
                    player.SendMessage("Error in arguments. Usage: [HalloweenTrickOrTreat enable or [HalloweenTrickOrTreat disable");
                }
            }
        }

        #endregion

        #region Properties

        private static bool m_TrickOrTreatEnabled = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool TrickOrTreatEnabled
        {
            get { return m_TrickOrTreatEnabled; }
            set
            {
                m_TrickOrTreatEnabled = value;

                if (!m_TrickOrTreatEnabled)
                    DeleteTrickOrTreatEntries();
            }
        }

        #endregion

        public static void DeleteTrickOrTreatEntries()
        {
            Queue m_Queue = new Queue();

            foreach (TrickOrTreatPlayerEntry entry in HalloweenPersistance.m_TrickOrTreatPlayerEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                m_Queue.Enqueue(entry);
            }

            while (m_Queue.Count > 0)
            {
                TrickOrTreatPlayerEntry entry = (TrickOrTreatPlayerEntry)m_Queue.Dequeue();
                entry.Delete();
            }

            HalloweenPersistance.m_TrickOrTreatPlayerEntries.Clear();
        }

        public static void TrickOrTreat(BaseCreature creature, Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null) return;
            if (!HalloweenPersistance.TrickOrTreatEnabled) return;

            TrickOrTreatPlayerEntry trickOrTreatEntry = null;

            bool foundPlayer = false;

            foreach (TrickOrTreatPlayerEntry playerEntry in HalloweenPersistance.m_TrickOrTreatPlayerEntries)
            {
                if (playerEntry.m_Player == player)
                {
                    trickOrTreatEntry = playerEntry;
                    foundPlayer = true;
                }
            }

            if (!foundPlayer)
            {
                trickOrTreatEntry = new TrickOrTreatPlayerEntry(player);
                HalloweenPersistance.m_TrickOrTreatPlayerEntries.Add(trickOrTreatEntry);
            }

            bool foundCreatureMatch = false;
            bool interactionReady = true;

            TimeSpan InteractionCooldown = TimeSpan.FromMinutes(60);

            foreach (KeyValuePair<BaseCreature, DateTime> interaction in trickOrTreatEntry.m_Interactions)
            {
                BaseCreature interactionCreature = interaction.Key;
                DateTime interactionTime = interaction.Value;

                if (interactionCreature == null) continue;
                if (interactionTime == null) continue;

                if (interactionCreature == creature)
                {
                    foundCreatureMatch = true;

                    if (DateTime.UtcNow < interactionTime + InteractionCooldown)                    
                        interactionReady = false;                    

                    break;
                }
            }

            if (foundCreatureMatch)
            {
                if (interactionReady)
                {
                    trickOrTreatEntry.m_Interactions[creature] = DateTime.UtcNow;
                    ResolveTrickOrTreat(creature, player, false);
                }

                else                
                    ResolveTrickOrTreat(creature, player, false);                
            }

            else
            {
                trickOrTreatEntry.m_Interactions.Add(creature, DateTime.UtcNow);
                ResolveTrickOrTreat(creature, player, true);
            }
        }

        public static void ResolveTrickOrTreat(BaseCreature creature, PlayerMobile player, bool allowed)
        {
            if (creature == null || player == null)
                return;

            double trickChance = .33;

            if (allowed)
            {
                double result = Utility.RandomDouble();

                if (result <= trickChance)
                    Trick(creature, player);

                else
                    Treat(creature, player);
            }

            else
            {
                creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Nay, I shan't give thee anything else for a while, methinks.");
                return;   
            }
        }

        public static void Trick(BaseCreature creature, PlayerMobile player)
        {
            if (creature == null || player == null)
                return;

            int trickTextHue = 0x22;

            creature.PublicOverheadMessage(MessageType.Regular, trickTextHue, false, "Trick it is then!");
            creature.PlaySound(0x246);

            double damageAmount = 0;
            int duration = 0;

            switch (Utility.RandomMinMax(1, 15))
            {                
                case 1:
                    SpecialAbilities.BacklashSpecialAbility(1.0, null, player, .75, 60, -1, true, "", "");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Ack! A curse! Causing your spellcasting proficiency to suffer!", player.NetState);
                break;

                case 2:
                    double bleedAmount = (double)player.HitsMax * .66;

                    for (int a = 0; a < 5; a++)
                    {
                        Point3D newLocation = new Point3D(player.Location.X + Utility.RandomList(-1, 1), player.Location.Y + Utility.RandomList(-1, 1), player.Location.Z);
                        
                        new Blood().MoveToWorld(newLocation, player.Map);
                    }

                    SpecialAbilities.BleedSpecialAbility(1.0, null, player, bleedAmount, 8, -1, true, "", "");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Curses! A dagger hidden in an apple!", player.NetState);
                break;

                case 3:
                    Point3D creatureLocation = creature.Location;
                    Point3D playerLocation = player.Location;

                    int projectiles = 15;
                    int particleSpeed = 8;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                        SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), player.Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 10), player.Map);

                        Effects.SendMovingParticles(effectStartLocation, effectEndLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);
                    }

                    player.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);                    

                    int damage = (int)(Math.Round((double)player.HitsMax * .33));

                    duration = 5;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, duration, false, -1, false, "", "");

                    new Blood().MoveToWorld(player.Location, player.Map);
                    AOS.Damage(player, damage, 0, 100, 0, 0, 0);

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Electric candy! What an age we live in!", player.NetState);
                break;

                case 4:
                    SpecialAbilities.DiseaseSpecialAbility(1.0, null, player, 3, 60, -1, true, "", "");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Blast! Expired candy!", player.NetState);
                break;

                case 5:
                    TimedStatic glue = new TimedStatic(4650, 30);
                    glue.Name = "glue";
                    glue.Hue = 2067;
                    glue.MoveToWorld(player.Location, player.Map);                    

                    SpecialAbilities.EntangleSpecialAbility(1.0, null, player, 1.0, 30, -1, true, "", "");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Your feet have been glued to the floor!", player.NetState);
                break;

                case 6:
                    damageAmount = (double)player.HitsMax * .5;
                    SpecialAbilities.FlamestrikeSpecialAbility(1.0, null, player, damageAmount, 1, -1, true, "", "Spicy candy! So hot!");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Spicy candy! So hot!", player.NetState);
                break;

                case 7:      
                    damageAmount = (double)player.HitsMax * .5;

                    Direction direction = Utility.GetDirection(creature.Location, player.Location);

                    int windItemId = 8099;

                    switch (direction)
                    {
                        case Direction.North: windItemId = 8099; break;
                        case Direction.Right: windItemId = 8099; break;

                        case Direction.West: windItemId = 8104; break;
                        case Direction.Up: windItemId = 8104; break;

                        case Direction.East: windItemId = 8109; break;
                        case Direction.Down: windItemId = 8109; break;

                        case Direction.South: windItemId = 8114; break;
                        case Direction.Left: windItemId = 8114; break;
                    }

                    creature.MovingEffect(player, windItemId, 5, 1, false, false, 0, 0);
                    player.PlaySound(0x64C);

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "This candy totally blows...", player.NetState);

                    SpecialAbilities.KnockbackSpecialAbility(1.0, creature.Location, null, player, damageAmount, 20, -1, "", "");
                break;

                case 8:
                    SpecialAbilities.PetrifySpecialAbility(1.0, null, player, 1.0, 15, -1, true, "", "");

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Rock candy!", player.NetState);
                break;

                case 9:
                    if (player.Poison == null)
                    {
                        Poison poison = Poison.GetPoison(2);
                        player.ApplyPoison(null, poison);
                    }

                    player.FixedEffect(0x372A, 10, 30, 2208, 0);
                    Effects.PlaySound(player.Location, player.Map, 0x22F);

                    int residueCount = Utility.RandomMinMax(3, 4);

                    for (int a = 0; a < residueCount; a++)
                    {
                        Point3D poisonPoint = new Point3D(player.Location.X + Utility.RandomList(-1, 1), player.Location.Y + Utility.RandomList(-1, 1), player.Location.Z);
                        SpellHelper.AdjustField(ref poisonPoint, player.Map, 12, false);

                        TimedStatic poisonResidue = new TimedStatic(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), 5);
                        poisonResidue.Hue = 2208;
                        poisonResidue.Name = "poison residue";
                        poisonResidue.MoveToWorld(poisonPoint, player.Map);
                    }

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Poisoned candy! Who would do such a thing!", player.NetState);
                break;

                case 10:
                    bool canPolymorph = true;

                    if (!player.CanBeginAction(typeof(PolymorphSpell)))
                        canPolymorph = false;

                    if (!player.CanBeginAction(typeof(PolymorphPotion)))
                        canPolymorph = false;

                    if (!player.CanBeginAction(typeof(IncognitoSpell)) || player.IsBodyMod)
                        canPolymorph = false;

                    if (DisguiseTimers.IsDisguised(player))
                        canPolymorph = false;

                    if (KinPaint.IsWearingKinPaint(player))
                        canPolymorph = false;                    

                    if (!canPolymorph)
                    {
                        player.SendMessage("Hmm...Nothing seems to have happened. Or did it?");
                        return;
                    }

                    player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);                    

                    List<int> m_PossibleBodyValues = new List<int>();

                    m_PossibleBodyValues.Add(3); //Zombie
                    m_PossibleBodyValues.Add(50); //Skeleton
                    m_PossibleBodyValues.Add(56); //Skeleton
                    m_PossibleBodyValues.Add(153); //Ghoul
                    m_PossibleBodyValues.Add(302); //Skeletal Critter
                    m_PossibleBodyValues.Add(309); //Patchwork Skeleton
                    m_PossibleBodyValues.Add(148); //Necromancer
                    m_PossibleBodyValues.Add(793); //Skeletal Horse
                    m_PossibleBodyValues.Add(317); //Giant Bat
                    m_PossibleBodyValues.Add(252); //Corpse Bride
                    m_PossibleBodyValues.Add(57); //Skeletal Knight
                    m_PossibleBodyValues.Add(116); //Nightmare
                    m_PossibleBodyValues.Add(24); //Lich
                    m_PossibleBodyValues.Add(154); //Mummy
                    m_PossibleBodyValues.Add(104); //Skeletal Drake
                    m_PossibleBodyValues.Add(740); //Skeletal Drake
                    m_PossibleBodyValues.Add(308); //Giant Skeleton

                    player.BodyMod = m_PossibleBodyValues[Utility.RandomMinMax(0, m_PossibleBodyValues.Count - 1)];
                    player.HueMod = 0;

                    player.PlaySound(0x3BD);

                    BaseArmor.ValidateMobile(player);

                    duration = 120;

                    player.BeginAction(typeof(PolymorphPotion));
                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (player == null)
                            return; 

                        player.EndAction(typeof(PolymorphPotion));
                    });

                    player.BeginAction(typeof(PolymorphSpell));
                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (player == null)
                            return; 

                        player.BodyMod = 0;
                        player.HueMod = -1;
                        player.EndAction(typeof(PolymorphSpell));

                        BaseArmor.ValidateMobile(player);
                    });

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Hmm, something tastes odd about this candy.", player.NetState);
                break;

                case 11:
                    Item innerLegs = player.FindItemOnLayer(Layer.InnerLegs);
                    Item outerLegs = player.FindItemOnLayer(Layer.OuterLegs);
                    Item pants = player.FindItemOnLayer(Layer.Pants);

                    int layersFound = 0;

                    if (innerLegs != null)
                    {
                        player.Backpack.DropItem(innerLegs);
                        layersFound++;
                    }

                    if (outerLegs != null)
                    {
                        player.Backpack.DropItem(outerLegs);
                        layersFound++;
                    }

                    if (pants != null)
                    {
                        player.Backpack.DropItem(pants);
                        layersFound++;
                    }

                    if (layersFound > 0)
                    {
                        player.PlaySound(0x503);

                        if (player.NetState != null)
                            player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Your pants appear to have fallen down. How embarrassing!", player.NetState);
                    }

                    else                     
                        player.SendMessage("Nothing seems to have happened. Or did it?...");                    
                break;

                case 12:

                    player.FixedParticles(0x374A, 10, 15, 5028, 2604, 0, EffectLayer.Waist);
                    player.PlaySound(0x5DA);

                    player.Animate(22, 6, 1, true, false, 0); //Fall Forward

                    player.Stam = 0;

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "You feel drowsy and fall on your face!", player.NetState);
                break;

                case 13:
                    player.BAC = 60;
                    player.PlaySound(0x5A9);

                    BaseBeverage.CheckHeaveTimer(player);

                    player.Animate(34, 5, 1, true, false, 0);

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "What was in that candy??? *hic*", player.NetState);
                break;

                case 14:
                    Warp(creature, player);
                break;

                case 15:
                    Warp(creature, player);
                break;
            }
        }

        public static void Warp(BaseCreature creature, PlayerMobile player)
        {
            if (creature == null || player == null)
                return;

            List<Point3D> m_Points = new List<Point3D>();

            m_Points.Add(new Point3D(1323, 1624, 55)); //Britain Throneroom
            m_Points.Add(new Point3D(2674, 2244, -20)); //Buccs Den Tunnels
            m_Points.Add(new Point3D(2187, 1408, -2)); //Cove Orc Fort
            m_Points.Add(new Point3D(1398, 3742, -21)); //Jhelom Fighting Pit
            m_Points.Add(new Point3D(3786, 2246, 20)); //Magincia Parliment
            m_Points.Add(new Point3D(2590, 482, 60)); //Minoc Mountain
            m_Points.Add(new Point3D(4707, 1122, 0)); //Moonglow Telescope
            m_Points.Add(new Point3D(3728, 1360, 5)); //Nujelm Chessboard
            m_Points.Add(new Point3D(3707, 2651, 20)); //Occlo Farmlands
            m_Points.Add(new Point3D(3024, 3536, 35)); //Serpents Hold Farmlands
            m_Points.Add(new Point3D(551, 2119, 0)); //Skara Brae Animal Pen
            m_Points.Add(new Point3D(1906, 2732, 40)); //Trinsic Cafe
            m_Points.Add(new Point3D(3014, 797, 0)); //Vesper Docks
            m_Points.Add(new Point3D(534, 992, 5)); //Yew Center

            player.PlaySound(0x0FE);

            int projectiles = 6;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 5), player.Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), player.Map);

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 2604, 0);
            } 

            player.Location = m_Points[Utility.RandomMinMax(0, m_Points.Count - 1)];

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (player == null)
                    return;

                player.PlaySound(0x0FE);

                projectiles = 6;
                particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                    SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), player.Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 10), player.Map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 2604, 0);
                }
            });            

            if (player.NetState != null)
                player.PrivateOverheadMessage(MessageType.Regular, 0, false, "Egads, how did I get here?", player.NetState);
        }

        public static void Treat(BaseCreature creature, PlayerMobile player)
        {
            if (creature == null || player == null)return;
            if (player.Backpack == null) return;

            int treatTextHue = 0x3F;

            creature.PublicOverheadMessage(MessageType.Regular, treatTextHue, false, "Have a treat!");

            Item biggestPile = null;
            int biggestPileCount = 0;
            Item[] candyPile;

            if (Utility.RandomDouble() <= .90)
            {
                switch(Utility.RandomMinMax(1, 3))
                {
                    case 1:
                        candyPile = player.Backpack.FindItemsByType(typeof(PlainOldCandy1));

                        biggestPile = null;
                        biggestPileCount = 0;

                        foreach (Item item in candyPile)
                        {
                            if (item.Amount > biggestPileCount)
                            {
                                biggestPile = item;
                                biggestPileCount = item.Amount;
                            }
                        }

                        if (biggestPile != null)                
                            biggestPile.Amount++;                

                        else
                        {
                            PlainOldCandy1 newCandy = new PlainOldCandy1();

                            player.Backpack.DropItem(newCandy);                    
                        }
                    break;

                    case 2:
                        candyPile = player.Backpack.FindItemsByType(typeof(PlainOldCandy2));

                        biggestPile = null;
                        biggestPileCount = 0;

                        foreach (Item item in candyPile)
                        {
                            if (item.Amount > biggestPileCount)
                            {
                                biggestPile = item;
                                biggestPileCount = item.Amount;
                            }
                        }

                        if (biggestPile != null)                
                            biggestPile.Amount++;                

                        else
                        {
                            PlainOldCandy2 newCandy = new PlainOldCandy2();

                            player.Backpack.DropItem(newCandy);                    
                        }
                    break;

                    case 3:
                        candyPile = player.Backpack.FindItemsByType(typeof(PlainOldCandy3));

                        biggestPile = null;
                        biggestPileCount = 0;

                        foreach (Item item in candyPile)
                        {
                            if (item.Amount > biggestPileCount)
                            {
                                biggestPile = item;
                                biggestPileCount = item.Amount;
                            }
                        }

                        if (biggestPile != null)                
                            biggestPile.Amount++;                

                        else
                        {
                            PlainOldCandy3 newCandy = new PlainOldCandy3();

                            player.Backpack.DropItem(newCandy);                    
                        }
                    break;

                }                

                player.PlaySound(0x5AA);
                player.SendMessage("You receive a some plain old candy.");
            }

            else
            {
                HalloweenTicket ticket = new HalloweenTicket();

                player.Backpack.DropItem(ticket);

                player.PlaySound(0x5BC);
                player.SendMessage("You receive a halloween ticket.");
            }
        }
        
        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0  
            writer.Write(m_TrickOrTreatEnabled);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_TrickOrTreatEnabled = reader.ReadBool();
            }
        }
    }

    public class HalloweenPersistanceItem : Item
    {
        public override string DefaultName { get { return "HalloweenPersistance"; } }

        public HalloweenPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public HalloweenPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            HalloweenPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            HalloweenPersistance.PersistanceItem = this;
            HalloweenPersistance.Deserialize(reader);
        }
    }  

    public class TrickOrTreatPlayerEntry : Item
    {
        public PlayerMobile m_Player;
        public Dictionary<BaseCreature, DateTime> m_Interactions = new Dictionary<BaseCreature, DateTime>();

        [Constructable]
        public TrickOrTreatPlayerEntry(PlayerMobile player)
        {
            m_Player = player;

            HalloweenPersistance.m_TrickOrTreatPlayerEntries.Add(this);
        }

        public TrickOrTreatPlayerEntry(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version

            writer.Write(m_Player);

            writer.Write(m_Interactions.Count);
            foreach (KeyValuePair<BaseCreature, DateTime> entry in m_Interactions)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;

                int interactions = reader.ReadInt();
                for (int a = 0; a < interactions; a++)
                {
                    BaseCreature bc_Creature = reader.ReadMobile() as BaseCreature;
                    DateTime eventTime = reader.ReadDateTime();

                    if (bc_Creature == null)
                        continue;

                    m_Interactions.Add(bc_Creature, eventTime);
                }
            }

            //-------------

            HalloweenPersistance.m_TrickOrTreatPlayerEntries.Add(this);
        }
    }
}
