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
using Server.Accounting;

namespace Server.Custom
{
    //Warning: Do Not Remove Any Entries From Here
    public enum CustomizationType
    {
        Hoarder,
        BenchPlayer,
        Carnage,
        Drunkard,
        Artisan,

        Vicious,
        Vanish,
        Mouthy,
        ViolentDeath,
        Venomous,

        Traveler,
        PhaseShift,
        Blink,
        Geomancer,
        Shadowskin,

        CustomerLoyalty,
        Shielded,
        SmoothSailing,
        Reborn,
        Tremors,
        
        Herdsman,
        Rancher,

        //Emotes
        EmoteFrequency,

        EmoteYes, 
        EmoteNo, 
        EmoteHiccup, 
        EmoteConfused, 
        EmoteGroan, 
        EmoteBurp, 
        EmoteGreet, 
        EmoteLaugh, 
        EmoteClap, 
        EmoteCough, 
        EmoteCry, 
        EmoteFart, 
        EmoteSurprised, 
        EmoteAnger, 
        EmoteKiss,       
        EmoteHurt, 
        EmoteOops, 
        EmotePuke, 
        EmoteYell, 
        EmoteShush, 
        EmoteSick, 
        EmoteSleep, 
        EmoteWhistle,       
        EmoteSpit,        
    }

    public static class PlayerCustomization
    {
        public static void OnLoginAudit(PlayerMobile player)
        {
            if (player == null)
                return;

            //Hoarder
            bool hoarder = PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.Hoarder);

            if (hoarder)
            {
                if (player.BankBox != null)
                {
                    if (player.BankBox.MaxItems == 125)
                        player.BankBox.MaxItems += 25;
                }
            }

            //Bench Player
            bool benchPlayer = PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.BenchPlayer);

            if (benchPlayer)
            {
                Account account = player.Account as Account;

                if (account == null)
                    return;

                if (account.CharacterLimit == 5)
                    account.CharacterLimit = 6;
            }
        }

        public static void OnUnlockCustomization(PlayerMobile player, CustomizationType customization)
        {
            if (player == null)
                return;

            switch (customization)
            {
                case CustomizationType.Hoarder:
                    BankBox bankBox = player.BankBox;

                    if (bankBox != null)
                        bankBox.MaxItems += 25;
                break;

                case CustomizationType.BenchPlayer:
                    Account account = player.Account as Account;
                    account.CharacterLimit = 6;
                break;
            }
        }

        public static CustomizationType GetRandomCustomizationType()
        {
            int customizationCount = Enum.GetNames(typeof(CustomizationType)).Length;

            CustomizationType customization = (CustomizationType)Utility.RandomMinMax(0, customizationCount - 1);

            return customization;
        }

        public static PlayerCustomizationDetail GetCustomizationDetail(CustomizationType playerCustomization)
        {
            PlayerCustomizationDetail customizationDetail = new PlayerCustomizationDetail();

            switch (playerCustomization)
            {
                case CustomizationType.Hoarder:
                    customizationDetail.m_Name = "Hoarder";
                    customizationDetail.m_Description = new string[] { "Increases bank maximum item limit to 150 items" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 2473;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = -3;
                    customizationDetail.m_IconOffsetY = 5;
                    customizationDetail.m_Selectable = false;
                    break;

                case CustomizationType.BenchPlayer:
                    customizationDetail.m_Name = "Bench Player";
                    customizationDetail.m_Description = new string[] { "Unlocks a 6th character slot", "" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 1115;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 2;
                    customizationDetail.m_IconOffsetY = -3;
                    customizationDetail.m_Selectable = false;
                    break;

                case CustomizationType.Vicious:
                    customizationDetail.m_Name = "Vicious";
                    customizationDetail.m_Description = new string[] { "When a player damages a target or inflicts bleed attacks, they ", 
                                                                       "create more blood" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 4653;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 0;
                    customizationDetail.m_IconOffsetY = 0;
                    customizationDetail.m_Selectable = true;
                    break;


                case CustomizationType.Artisan:
                    customizationDetail.m_Name = "Artisan";
                    customizationDetail.m_Description = new string[] { "When the player crafts an exceptional, marked item, they will",
                                                                       "use upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4021;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = -3;
                    customizationDetail.m_IconOffsetY = 5;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.CustomerLoyalty:
                    customizationDetail.m_Name = "Customer Loyalty";
                    customizationDetail.m_Description = new string[] { "Vendors will bow and salute the player when they purchase",
                                                                        "items at stores" };
                    customizationDetail.m_Cost = 50000;
                    customizationDetail.m_IconItemId = 3009;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 5;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Blink:
                    customizationDetail.m_Name = "Blink";
                    customizationDetail.m_Description = new string[] { "The Teleport spell has upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 14123;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = -10;
                    customizationDetail.m_IconOffsetY = -50;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Vanish:
                    customizationDetail.m_Name = "Vanish";
                    customizationDetail.m_Description = new string[] { "The Invisibility spell has upgraded visual and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 2584;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = -5;
                    customizationDetail.m_IconOffsetY = 5;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Shadowskin:
                    customizationDetail.m_Name = "Shadowskin";
                    customizationDetail.m_Description = new string[] { "Usage of the Incognito spell grants the player the shadow skin",
                                                                   "hue for a brief period" };
                    customizationDetail.m_Cost = 500000;
                    customizationDetail.m_IconItemId = 3589;
                    customizationDetail.m_IconHue = 1102;
                    customizationDetail.m_IconOffsetX = 10;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Mouthy:
                    customizationDetail.m_Name = "Mouthy";
                    customizationDetail.m_Description = new string[] { "When the player eats or drinks they have upgraded visuals and",
                                                                   "sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 2520;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 3;
                    customizationDetail.m_IconOffsetY = 10;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Geomancer:
                    customizationDetail.m_Name = "Geomancer";
                    customizationDetail.m_Description = new string[] { "The Wall of Stone spell has upgraded visuals and sound",
                                                                   "effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 105;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 10;
                    customizationDetail.m_IconOffsetY = -5;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Reborn:
                    customizationDetail.m_Name = "Reborn";
                    customizationDetail.m_Description = new string[] { "When the player is resurrected, it has",
                                                                   "upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 3616;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 5;
                    customizationDetail.m_IconOffsetY = 8;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.SmoothSailing:
                    customizationDetail.m_Name = "Smooth Sailing";
                    customizationDetail.m_Description = new string[] { "When the player is onboard a moving ship they own",
                                                                   "they will occasionally generate ocean sounds" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 5364;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 0;
                    customizationDetail.m_IconOffsetY = 0;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.PhaseShift:
                    customizationDetail.m_Name = "Phase Shift";
                    customizationDetail.m_Description = new string[] { "The Recall spell has upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 14703; //14155
                    customizationDetail.m_IconHue = 2498;
                    customizationDetail.m_IconOffsetX = 0;
                    customizationDetail.m_IconOffsetY = -95;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Carnage:
                    customizationDetail.m_Name = "Carnage";
                    customizationDetail.m_Description = new string[] { "When the player kills another player, the target has a chance",
                                                                    "to explode in a shower of gore" };
                    customizationDetail.m_Cost = 500000;
                    customizationDetail.m_IconItemId = 7392;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = -2;
                    customizationDetail.m_IconOffsetY = 2;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Shielded:
                    customizationDetail.m_Name = "Shielded";
                    customizationDetail.m_Description = new string[] { "The Magic Reflect spell has upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 7028;
                    customizationDetail.m_IconHue = 2595;
                    customizationDetail.m_IconOffsetX = -3;
                    customizationDetail.m_IconOffsetY = 2;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Traveler:
                    customizationDetail.m_Name = "Traveler";
                    customizationDetail.m_Description = new string[] { "The Gate Travel spells has upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 6891;
                    customizationDetail.m_IconHue = 2616;
                    customizationDetail.m_IconOffsetX = 5;
                    customizationDetail.m_IconOffsetY = -25;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Tremors:
                    customizationDetail.m_Name = "Tremors";
                    customizationDetail.m_Description = new string[] { "The Earthquake spell has upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 7025;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 7;
                    customizationDetail.m_IconOffsetY = -3;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Venomous:
                    customizationDetail.m_Name = "Venomous";
                    customizationDetail.m_Description = new string[] { "When the player inflicts poison via spell or melee, it has",
                                                                   "upgraded visuals and sound effects" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 3850;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 5;
                    customizationDetail.m_IconOffsetY = 7;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.ViolentDeath:
                    customizationDetail.m_Name = "Violent Death";
                    customizationDetail.m_Description = new string[] { "When the player dies, they have a chance to",
                                                                   "explode in a shower of gore" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 7398;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 4;
                    customizationDetail.m_IconOffsetY = 0;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Drunkard:
                    customizationDetail.m_Name = "Drunkard";
                    customizationDetail.m_Description = new string[] { "When the player becomes drunk, they will",
                                                                   "make a scene of themselves" };
                    customizationDetail.m_Cost = 250000;
                    customizationDetail.m_IconItemId = 2462;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 0;
                    customizationDetail.m_IconOffsetY = 0;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Herdsman:
                    customizationDetail.m_Name = "Herdsman";
                    customizationDetail.m_Description = new string[] { "Increases Stable Slots by 5 (Cumulative)" };
                    customizationDetail.m_Cost = 150000;
                    customizationDetail.m_IconItemId = 3713;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 10;
                    customizationDetail.m_IconOffsetY = 0;
                    customizationDetail.m_Selectable = true;
                    break;

                case CustomizationType.Rancher:
                    customizationDetail.m_Name = "Rancher";
                    customizationDetail.m_Description = new string[] { "Increases Stable Slots by 10 (Cumulative)" };
                    customizationDetail.m_Cost = 300000;
                    customizationDetail.m_IconItemId = 3896;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 5;
                    customizationDetail.m_IconOffsetY = -18;
                    customizationDetail.m_Selectable = true;
                break;

                //Emotes
                case CustomizationType.EmoteFrequency:
                    customizationDetail.m_Name = "Emote Frequency";
                    customizationDetail.m_Description = new string[] { "Changes the cooldown of using Emotes",
                                                                        "from 2 minutes to 30 seconds" };
                    customizationDetail.m_Cost = 200000;
                    customizationDetail.m_IconItemId = 6160;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteYes:
                    customizationDetail.m_Name = "Emote: Yes";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Yes' emote.",
                                                                        "Usable by typing [EmoteYes" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2589;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteNo:
                    customizationDetail.m_Name = "Emote: No";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'No' emote.",
                                                                           "Usable by typing [EmoteNo" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2612;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteHiccup:
                    customizationDetail.m_Name = "Emote: Hiccup";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Hiccup' emote.",
                                                                           "Usable by typing [EmoteHiccup" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2527;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteConfused:
                    customizationDetail.m_Name = "Emote: Confused";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Confused' emote.",
                                                                               "Usable by typing [EmoteConfused" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2500;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteGroan:
                    customizationDetail.m_Name = "Emote: Groan";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Groan' emote.",
                                                                                   "Usable by typing [EmoteGroan" };
                    customizationDetail.m_Cost = 100000;
                   customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2610;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteBurp:
                    customizationDetail.m_Name = "Emote: Burp";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Burp' emote.",
                                                                       "Usable by typing [EmoteBurp" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2658;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteGreet:
                    customizationDetail.m_Name = "Emote: Greet";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Greet' emote.",
                                                                   "Usable by typing [EmoteGreet" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2652;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteLaugh:
                    customizationDetail.m_Name = "Emote: Laugh";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Laugh' emote.",
                                                                       "Usable by typing [EmoteLaugh" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2644;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteClap:
                    customizationDetail.m_Name = "Emote: Clap";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Clap' emote.",
                                                                       "Usable by typing [EmoteClap" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2214;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteCough:
                    customizationDetail.m_Name = "Emote: Cough";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Cough' emote.",
                                                                       "Usable by typing [EmoteCough" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2210;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteCry:
                    customizationDetail.m_Name = "Emote: Cry";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Cry' emote.",
                                                                       "Usable by typing [EmoteCry" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2579;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteFart:
                    customizationDetail.m_Name = "Emote: Fart";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Fart' emote.",
                                                                       "Usable by typing [EmoteFart" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2961;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteSurprised:
                    customizationDetail.m_Name = "Emote: Surprised";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Surprised' emote.",
                                                                       "Usable by typing [EmoteSurprised" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2115;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteAnger:
                    customizationDetail.m_Name = "Emote: Anger";
                    customizationDetail.m_Description = new string[] { "Unlocks the Anger emote.",
                                                                       "Usable by typing [EmoteAnger" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2964;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteKiss:
                    customizationDetail.m_Name = "Emote: Kiss";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Kiss' emote.",
                                                                       "Usable by typing [EmoteKiss" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2515;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteHurt:
                    customizationDetail.m_Name = "Emote: Hurt";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Hurt' emote.",
                                                                       "Usable by typing [EmoteHurt" };
                    customizationDetail.m_Cost = 100000;
                   customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2102;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteOops:
                    customizationDetail.m_Name = "Emote: Oops";
                    customizationDetail.m_Description = new string[] { "Unlocks the Oops emote.",
                                                                       "Usable by typing [EmoteOops" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2583;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmotePuke:
                    customizationDetail.m_Name = "Emote: Puke";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Puke' emote.",
                                                                       "Usable by typing [EmotePuke" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 0;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteYell:
                    customizationDetail.m_Name = "Emote: Yell";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Yell' emote.",
                                                                       "Usable by typing [EmoteYell" };
                    customizationDetail.m_Cost = 100000;
                   customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2122;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteShush:
                    customizationDetail.m_Name = "Emote: Shush";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Shush' emote.",
                                                                        "Usable by typing [EmoteShush" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2651;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteSick:
                    customizationDetail.m_Name = "Emote: Sick";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Sick' emote.",
                                                                            "Usable by typing [EmoteSick" };
                    customizationDetail.m_Cost = 100000;
                  customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2638;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteSleep:
                    customizationDetail.m_Name = "Emote: Sleep";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Sleep' emote.",
                                                                                "Usable by typing [EmoteSleep" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2575;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteWhistle:
                    customizationDetail.m_Name = "Emote: Whistle";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Whistle' emote.",
                                                                                    "Usable by typing [EmoteWhistle" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2635;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;

                case CustomizationType.EmoteSpit:
                    customizationDetail.m_Name = "Emote: Spit";
                    customizationDetail.m_Description = new string[] { "Unlocks the 'Spit' emote.",
                                                                                            "Usable by typing [EmoteSpit" };
                    customizationDetail.m_Cost = 100000;
                    customizationDetail.m_IconItemId = 4810;
                    customizationDetail.m_IconHue = 2541;
                    customizationDetail.m_IconOffsetX = 8;
                    customizationDetail.m_IconOffsetY = 3;
                    customizationDetail.m_Selectable = true;
                break;
            }

            return customizationDetail;
        }
    }

    public class PlayerCustomizationDetail
    {
        public string m_Name = "Customization Detail";
        public string[] m_Description;
        public int m_Cost = 50000;
        public int m_IconItemId = 4015;
        public int m_IconHue = 0;
        public int m_IconOffsetX = 0;
        public int m_IconOffsetY = 0;
        public bool m_Selectable = true;

        public PlayerCustomizationDetail()
        {
        }
    }

    public class PlayerCustomizationEntry
    {
        public CustomizationType m_CustomizationType = CustomizationType.Artisan;
        public bool m_Unlocked = false;
        public bool m_Active = false;
        public bool m_Selectable = true;

        public PlayerCustomizationEntry(CustomizationType customizationType, bool unlocked, bool active, bool selectable)
        {
            m_CustomizationType = customizationType;
            m_Unlocked = unlocked;
            m_Active = active;
            m_Selectable = selectable;
        }
    }
}