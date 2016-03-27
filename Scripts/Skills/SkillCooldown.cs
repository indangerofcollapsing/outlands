using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server
{
    public class SkillCooldown
    {
        public static double AlchemyCooldown = DefAlchemy.CraftSystem.Delay;
        public static double AnatomyCooldown = 2.0;
        public static double AnimalLoreCooldown = 2.0;
        public static double ItemIDCooldown = 1.0;
        public static double ArmsLoreCooldown = 2.0;
        public static double ParryCooldown = .5; //Only Used For Skill Gain Throttling
        public static double BeggingSuccessCooldown = 10.0;
        public static double BeggingFailureCooldown = 5.0;
        public static double BlacksmithCooldown = DefBlacksmithy.CraftSystem.Delay;
        public static double BowcraftCooldown = DefCarpentry.CraftSystem.Delay;
        public static double PeacemakingSuccessCooldown = 10;
        public static double PeacemakingFailureCooldown = 5;
        public static double CampingCooldown = 3;
        public static double CarpentryCooldown = DefCarpentry.CraftSystem.Delay;
        public static double CartographyCooldown = DefCartography.CraftSystem.Delay;
        public static double CookingCooldown = DefCooking.CraftSystem.Delay;
        public static double DetectHiddenCooldown = 5;
        public static double DiscordanceSuccessCooldown = 10;
        public static double DiscordanceFailureCooldown = 5;
        public static double EvalIntCooldown = 2.0;
        public static double HealingSelfCooldown = 10.0;
        public static double HealingOtherCooldown = 7.5;
        public static double FishingCooldown = 10.0;
        public static double ForensicsCooldown = 2.0;
        public static double HerdingSuccessCooldown = 10;
        public static double HerdingFailureCooldown = 5;
        public static double HidingCooldown = 10;
        public static double ProvocationSuccessCooldown = 10;
        public static double ProvocationFailureCooldown = 5;
        public static double InscribeCooldown = DefInscription.CraftSystem.Delay;
        public static double LockpickingCooldown = 3;
        public static double MageryCooldown = 10; //Only Used For Skill Gain Throttling
        public static double MagicResistCooldown = 3; //Only Used For Skill Gain Throttling
        public static double TacticsCooldown = 1.0;
        public static double SnoopingCooldown = 1.5;
        public static double MusicianshipCooldown = 5.0;
        public static double PoisoningCooldown = 2.0;
        public static double ArcheryCooldown = 1.0;
        public static double SpiritSpeakCooldown = 2.0;
        public static double StealingCooldown = 10.0;
        public static double TailoringCooldown = DefTailoring.CraftSystem.Delay;
        public static double AnimalTamingCooldown = 10.0; //Only Used For Skill Gain Throttling
        public static double TasteIDCooldown = 1.0;
        public static double TinkeringCooldown = DefTinkering.CraftSystem.Delay;
        public static double TrackingCooldown = 10.0;
        public static double VeterinarySelfCooldown = 10.0;
        public static double VeterinaryOtherCooldown = 7.5;
        public static double SwordsCooldown = 1.0;
        public static double MacingCooldown = 1.0;
        public static double FencingCooldown = 1.0;
        public static double WrestlingCooldown = 1.0;
        public static double LumberjackingCooldown = 10.0; //Only Used For Skill Gain Throttling
        public static double MiningCooldown = 10.0; //Only Used For Skill Gain Throttling
        public static double MeditationValidCooldown = 10.0;
        public static double MeditationInvalidCooldown = 5.0;
        public static double StealthCooldown = 10.0;
        public static double RemoveTrapCooldown = 3.0;
    }
}