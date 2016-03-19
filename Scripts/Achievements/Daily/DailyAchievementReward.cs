using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Achievements
{
    public class DailyAchievementReward : MetalBox
    {

        public DailyAchievementReward(PlayerMobile player)
        {
            Hue = Utility.RandomNondyedHue();
            Weight = 1.0;
            DropItem(SkillScroll.Generate(player, 120.0, 1));
            DropItem(SkillScroll.Generate(player, 120.0, 1));
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            if (Utility.RandomDouble() > 0.9)
                DropItem(new DragonLotteryTicket());

            int seed = Utility.Random(100);
            if (40 > seed)
            {	// 40%
                DropItem(TitleDye.RandomCommonTitleDye());
            }
            else if (60 > seed)
            {	// 20%

                DropItem(TitleDye.RandomUncommonTitleDye());
            }
            else if (70 > seed)
            {	// 10% lvl 4,5 or 6 TMap
                DropItem(new TreasureMap(Utility.Random(4, 3), Map.Felucca));
            }
            else if (85 > seed)
            {	// 15%
                DropItem(TitleDye.RandomRareTitleDye());
            }
            else if (97 > seed)
            {	// 12% glove chance.

                DropItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gloves));
                DropItem(new ArcaneDust());
            }
            else if (99 > seed)
            {	// 2%
                WispLantern i = new WispLantern();

                switch (Utility.RandomList(1, 2, 3, 4))
                {
                    case 1:
                        i.Hue = 16385;
                        i.SetWispHue(16385);
                        i.SetWispName("Essence of Air");
                        break;
                    case 2:
                        i.Hue = 2112;
                        i.SetWispHue(2112);
                        i.SetWispName("Essence of Earth");
                        break;
                    case 3:
                        i.Hue = 1266;
                        i.SetWispHue(1266);
                        i.SetWispName("Essence of Water");
                        break;
                    case 4:
                        i.Hue = 1357;
                        i.SetWispHue(1357);
                        i.SetWispName("Essence of Fire");
                        break;
                }
                DropItem(i);
            }
            else
            {	// 1%
                Item i = new SavageMask();
                i.Name = "The Immortal Flame's Visage";
                i.LootType = LootType.Blessed;
                DropItem(i);
            }
        }

        public DailyAchievementReward(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
