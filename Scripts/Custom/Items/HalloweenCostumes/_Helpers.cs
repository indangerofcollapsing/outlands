using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public static class Helpers
    {
        public static Type[] HalloweenCostumePieces =
        {
            typeof(AlizzaApron), typeof(AlizzaDress), typeof(AlizzaHeadband), typeof(AlizzaShoes), typeof(AlizzaCatLantern),
            typeof(AmazingLadyBracelet), typeof(AmazingLadyBoots), typeof(AmazingLadySword), typeof(AmazingLadyBustier), typeof(AmazingLadySkirt),
            typeof(GirlScoutSash), typeof(GirlScoutBonnet), typeof(GirlScoutKilt), typeof(GirlScoutBoots), typeof(GirlScoutShirt),
            typeof(KantmissBow), typeof(KantmissChest), typeof(KantmissLegs), typeof(KantmissArms), typeof(KantmissBoots),
            typeof(MonkSandals), typeof(MonkNecklace), typeof(MonkHalloweenRobe),
            typeof(OrcHolloweenMask), typeof(OrcChest), typeof(OrcAxe), typeof(OrcBoots),//typeof(OrcHalloweenHelm),
            typeof(PilgrimBoots), typeof(PilgrimFloppyHat), typeof(PilgrimHalfApron),
            typeof(PimpHat), typeof(PimpCape), typeof(PimpShoes), typeof(PimpWatch), typeof(PimpNecklace),
            typeof(PirateHalloweenBoots), typeof(PirateCutlass), typeof(PirateHat), typeof(PiratePants), typeof(PirateHalloweenShirt), typeof(PirateHalloweenDoublet),
            typeof(PlumbersHat), typeof(PlumbersShirt), typeof(PlumbersOverallsPt1), typeof(PlumbersOverallsPt2),
            typeof(PrincessBonnet),typeof(PrincessDress),typeof(PrincessBoots),typeof(PrincessNecklace),typeof(PrincessRing),
            typeof(SkeletonHead),typeof(SkeletonArms),typeof(SkeletonChest),typeof(SkeletonLegs),
            typeof(StealthWarriorShirt),typeof(StealthWarriorPants),typeof(StealthWarriorSandals),typeof(StealthWarriorBandana),
            typeof(VikingGodWarMace),typeof(VikingGodCloak),typeof(VikingGodHelm),typeof(VikingGodLegs),typeof(VikingGodChest),
            typeof(WitchSetRobe),typeof(WitchSandals),typeof(WitchHat),
        };

        public static void AddRandomHalloweenCostumePieceToPlayer(PlayerMobile player)
        {
            player.AddToBackpack(GetRandomHalloweenCostumePiece());

        }

        public static void AddRandomHalloweenCostumePieceToMob(BaseCreature mob)
        {
            mob.PackItem(GetRandomHalloweenCostumePiece());
        }

        public static Item GetRandomHalloweenCostumePiece()
        {
            return (Item)Activator.CreateInstance(HalloweenCostumePieces[Utility.Random(0, HalloweenCostumePieces.Length - 1)]);
        }

        public static bool CutCloth(Item item, Mobile from)
        {
            if (!item.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            Cloth cloth = new Cloth();
            cloth.Amount = 1;
            cloth.Hue = item.Hue;

            item.Delete();

            from.AddToBackpack(cloth);
            from.SendMessage("You cut the item into cloth.");

            return false;
        }

        public static bool CutLeather(Item item, Mobile from)
        {
            if (!item.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            Leather cloth = new Leather();
            cloth.Amount = 1;
            cloth.Hue = item.Hue;

            item.Delete();

            from.AddToBackpack(cloth);
            from.SendMessage("You cut the item into cloth.");

            return false;
        }
    }
}
