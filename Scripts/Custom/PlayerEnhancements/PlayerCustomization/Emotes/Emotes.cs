using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Custom;
using Server.Items;

namespace Server
{
    public static class Emotes
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
            });

            CommandSystem.Register("EmoteHiccup", AccessLevel.Player, new CommandEventHandler(EmoteHiccup));
            CommandSystem.Register("EmoteConfused", AccessLevel.Player, new CommandEventHandler(EmoteConfused));
            CommandSystem.Register("EmoteGroan", AccessLevel.Player, new CommandEventHandler(EmoteGroan));
            CommandSystem.Register("EmoteBurp", AccessLevel.Player, new CommandEventHandler(EmoteBurp));
            CommandSystem.Register("EmoteGreet", AccessLevel.Player, new CommandEventHandler(EmoteGreet));
            CommandSystem.Register("EmoteLaugh", AccessLevel.Player, new CommandEventHandler(EmoteLaugh));
            CommandSystem.Register("EmoteClap", AccessLevel.Player, new CommandEventHandler(EmoteClap));
            CommandSystem.Register("EmoteCough", AccessLevel.Player, new CommandEventHandler(EmoteCough));
            CommandSystem.Register("EmoteCry", AccessLevel.Player, new CommandEventHandler(EmoteCry));
            CommandSystem.Register("EmoteFart", AccessLevel.Player, new CommandEventHandler(EmoteFart));
            CommandSystem.Register("EmoteSurprised", AccessLevel.Player, new CommandEventHandler(EmoteSurprised));
            CommandSystem.Register("EmoteAnger", AccessLevel.Player, new CommandEventHandler(EmoteAnger));
            CommandSystem.Register("EmoteKiss", AccessLevel.Player, new CommandEventHandler(EmoteKiss));
            CommandSystem.Register("EmoteNo", AccessLevel.Player, new CommandEventHandler(EmoteNo));
            CommandSystem.Register("EmoteHurt", AccessLevel.Player, new CommandEventHandler(EmoteHurt));
            CommandSystem.Register("EmoteOops", AccessLevel.Player, new CommandEventHandler(EmoteOops));
            CommandSystem.Register("EmotePuke", AccessLevel.Player, new CommandEventHandler(EmotePuke));
            CommandSystem.Register("EmoteYell", AccessLevel.Player, new CommandEventHandler(EmoteYell));
            CommandSystem.Register("EmoteShush", AccessLevel.Player, new CommandEventHandler(EmoteShush));
            CommandSystem.Register("EmoteSick", AccessLevel.Player, new CommandEventHandler(EmoteSick));
            CommandSystem.Register("EmoteSleep", AccessLevel.Player, new CommandEventHandler(EmoteSleep));
            CommandSystem.Register("EmoteWhistle", AccessLevel.Player, new CommandEventHandler(EmoteWhistle));
            CommandSystem.Register("EmoteYes", AccessLevel.Player, new CommandEventHandler(EmoteYes));
            CommandSystem.Register("EmoteSpit", AccessLevel.Player, new CommandEventHandler(EmoteSpit));
        }

        [Usage("EmoteHiccup")]
        [Description("Activates the Hiccup emote")]
        public static void EmoteHiccup(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteHiccup);
        }

        [Usage("EmoteConfused")]
        [Description("Activates the Confused emote")]
        public static void EmoteConfused(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteConfused);
        }

        [Usage("EmoteGroan")]
        [Description("Activates the Groan emote")]
        public static void EmoteGroan(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteGroan);
        }

        [Usage("EmoteBurp")]
        [Description("Activates the Burp emote")]
        public static void EmoteBurp(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteBurp);
        }

        [Usage("EmoteGreet")]
        [Description("Activates the Greet emote")]
        public static void EmoteGreet(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteGreet);
        }

        [Usage("EmoteLaugh")]
        [Description("Activates the Laugh emote")]
        public static void EmoteLaugh(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteLaugh);
        }

        [Usage("EmoteClap")]
        [Description("Activates the Clap emote")]
        public static void EmoteClap(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteClap);
        }

        [Usage("EmoteCough")]
        [Description("Activates the Cough emote")]
        public static void EmoteCough(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteCough);
        }

        [Usage("EmoteCry")]
        [Description("Activates the Cry emote")]
        public static void EmoteCry(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteCry);
        }

        [Usage("EmoteFart")]
        [Description("Activates the Fart emote")]
        public static void EmoteFart(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteFart);
        }

        [Usage("EmoteSurprised")]
        [Description("Activates the Surprised emote")]
        public static void EmoteSurprised(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteSurprised);
        }

        [Usage("EmoteAnger")]
        [Description("Activates the Anger emote")]
        public static void EmoteAnger(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteAnger);
        }

        [Usage("EmoteKiss")]
        [Description("Activates the Kiss emote")]
        public static void EmoteKiss(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteKiss);
        }

        [Usage("EmoteNo")]
        [Description("Activates the No emote")]
        public static void EmoteNo(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteNo);
        }

        [Usage("EmoteHurt")]
        [Description("Activates the Hurt emote")]
        public static void EmoteHurt(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteHurt);
        }

        [Usage("EmoteOops")]
        [Description("Activates the Oops emote")]
        public static void EmoteOops(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteOops);
        }

        [Usage("EmotePuke")]
        [Description("Activates the Puke emote")]
        public static void EmotePuke(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmotePuke);
        }

        [Usage("EmoteYell")]
        [Description("Activates the Yell emote")]
        public static void EmoteYell(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteYell);
        }

        [Usage("EmoteShush")]
        [Description("Activates the Shush emote")]
        public static void EmoteShush(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteShush);
        }

        [Usage("EmoteSick")]
        [Description("Activates the Sick emote")]
        public static void EmoteSick(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteSick);
        }

        [Usage("EmoteSleep")]
        [Description("Activates the Sleep emote")]
        public static void EmoteSleep(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteSleep);
        }

        [Usage("EmoteWhistle")]
        [Description("Activates the Whistle emote")]
        public static void EmoteWhistle(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteWhistle);
        }

        [Usage("EmoteYes")]
        [Description("Activates the Yes emote")]
        public static void EmoteYes(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteYes);
        }

        [Usage("EmoteSpit")]
        [Description("Activates the Spit emote")]
        public static void EmoteSpit(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            DoEmote(player, CustomizationType.EmoteSpit);
        }

        public static void DoEmote(PlayerMobile player, CustomizationType emote)
        {
            if (player == null)
                return;

            if (player.Squelched || player.StealthSquelchedExpiration > DateTime.UtcNow)
            {
                player.SendMessage("You cannot speak at the moment.");
                return;
            }

            if (!player.Alive && player.AccessLevel == AccessLevel.Player)
            {
                player.SendMessage("You must be alive in order to use an Emote.");
                return;
            }

            bool active = PlayerEnhancementPersistance.IsCustomizationEntryActive(player, emote);

            if (!active && player.AccessLevel == AccessLevel.Player)
            {
                player.SendMessage("You have not unlocked and activated this Emote.");
                return;
            }

            if (DateTime.UtcNow < player.NextEmoteAllowed && player.AccessLevel == AccessLevel.Player)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.NextEmoteAllowed, false, true, true, true, true);

                player.SendMessage("You may not use another emote for " + timeRemaining + ".");
                return;
            }

            bool shortCooldown = PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.EmoteFrequency);

            TimeSpan cooldown = PlayerMobile.EmoteCooldownLong;

            if (shortCooldown)
                cooldown = PlayerMobile.EmoteCooldownShort;

            player.NextEmoteAllowed = DateTime.UtcNow + cooldown;

            string text = "";  
            int sound = 0;                     

            switch (emote)
            {
                case CustomizationType.EmoteYes:
                    text = "*nods*";

                    if (player.Female)
                        sound = Utility.RandomList(0x337);
                    else
                        sound = Utility.RandomList(0x449);
                break;

                case CustomizationType.EmoteNo:
                    text = "*shakes head*";

                    if (player.Female)
                        sound = Utility.RandomList(0x322);
                    else
                        sound = Utility.RandomList(0x432);
                break;

                case CustomizationType.EmoteHiccup:
                    text = "*hiccup*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31E);
                    else
                        sound = Utility.RandomList(0x42E);
                break;

                case CustomizationType.EmoteConfused:
                    text = "*huh*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31F);
                    else
                        sound = Utility.RandomList(0x42F);
                break;

                case CustomizationType.EmoteGroan:
                    text = "*groans*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31B);
                    else
                        sound = Utility.RandomList(0x42B);
                break;

                case CustomizationType.EmoteBurp:
                    text = "*burp*";

                    if (player.Female)
                        sound = Utility.RandomList(0x30E);
                    else
                        sound = Utility.RandomList(0x41D);
                break;

                case CustomizationType.EmoteGreet:
                    text = "*waves*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31D);
                    else
                        sound = Utility.RandomList(0x42D);
                break;

                case CustomizationType.EmoteLaugh:
                    text = "*laughs*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31A, 0x321);
                    else
                        sound = Utility.RandomList(0x42A, 0x431);
                break;

                case CustomizationType.EmoteClap:
                    text = "*claps*";

                    if (player.Female)
                        sound = Utility.RandomList(0x30C);
                    else
                        sound = Utility.RandomList(0x41B);
                break;

                case CustomizationType.EmoteCough:
                    text = "*coughs*";

                    if (player.Female)
                        sound = Utility.RandomList(0x311, 0x312);
                    else
                        sound = Utility.RandomList(0x420, 0x421);
                break;

                case CustomizationType.EmoteCry:
                    text = "*cries*";

                    if (player.Female)
                        sound = Utility.RandomList(0x313);
                    else
                        sound = Utility.RandomList(0x422);
                break;

                case CustomizationType.EmoteFart:
                    text = "*farts*";

                    if (player.Female)
                        sound = Utility.RandomList(0x318);
                    else
                        sound = Utility.RandomList(0x428);
                break;

                case CustomizationType.EmoteSurprised:
                    text = "*looks surprised*";

                    if (player.Female)
                        sound = Utility.RandomList(0x319, 0x323, 0x32B);
                    else
                        sound = Utility.RandomList(0x429, 0x433, 0x43D);
                break;

                case CustomizationType.EmoteAnger:
                    text = "*looks angry*";

                    if (player.Female)
                        sound = Utility.RandomList(0x31C);
                    else
                        sound = Utility.RandomList(0x42C);
                break;

                case CustomizationType.EmoteKiss:
                    text = "*kisses*";

                    if (player.Female)
                        sound = Utility.RandomList(0x320);
                    else
                        sound = Utility.RandomList(0x430);
                break;

                case CustomizationType.EmoteHurt:
                    text = "*looks hurt*";

                    if (player.Female)
                        sound = Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B);
                    else
                        sound = Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x5F6, 0x436, 0x437, 0x43B, 0x43C);
                break;

                case CustomizationType.EmoteOops:
                    text = "*oops*";

                    if (player.Female)
                        sound = Utility.RandomList(0x32C);
                    else
                        sound = Utility.RandomList(0x43E);
                break;

                case CustomizationType.EmotePuke:
                    text = "*pukes*";

                    if (player.Female)
                        sound = Utility.RandomList(0x32D);
                    else
                        sound = Utility.RandomList(0x43F);
                break;

                case CustomizationType.EmoteYell:
                    text = "*yells*";

                    if (player.Female)
                        sound = Utility.RandomList(0x32E, 0x338);
                    else
                        sound = Utility.RandomList(0x440, 0x44A);
                break;

                case CustomizationType.EmoteShush:
                    text = "*shh*";

                    if (player.Female)
                        sound = Utility.RandomList(0x32F);
                    else
                        sound = Utility.RandomList(0x441);
                break;

                case CustomizationType.EmoteSick:
                    text = "*looks sick*";

                    if (player.Female)
                        sound = Utility.RandomList(0x30D, 0x331, 0x332);
                    else
                        sound = Utility.RandomList(0x41F, 0x443, 0x444);
                break;

                case CustomizationType.EmoteSleep:
                    text = "*looks tired*";

                    if (player.Female)
                        sound = Utility.RandomList(0x333, 0x336);
                    else
                        sound = Utility.RandomList(0x445, 0x448);
                break;

                case CustomizationType.EmoteWhistle:
                    text = "*whistles*";

                    if (player.Female)
                        sound = Utility.RandomList(0x335);
                    else
                        sound = Utility.RandomList(0x447);
                break;

                case CustomizationType.EmoteSpit:
                    text = "*spits*";

                    if (player.Female)
                        sound = Utility.RandomList(0x334);
                    else
                        sound = Utility.RandomList(0x446);
                break;
            }

            player.PublicOverheadMessage(MessageType.Emote, 0, false, text);
            player.PlaySound(sound);
        }
    } 
}