using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;
using Server.Custom.Misc;

namespace Server.Custom.RushChallenge
{
    // the prize for winning
    public class RushChallengeGoodiebag : MetalBox
    {
        [Constructable]
        public RushChallengeGoodiebag(PlayerMobile winner, string monster_name, int monster_hue)
        {
            Hue = 2407;
            Weight = 1.0;
            Name = String.Format("a reward for slaying {0}", monster_name);

            int seed = Utility.Random(100);
            if (40 > seed)
            {	// 40%
                DropItem(TitleDye.RandomCommonTitleDye());
            }
            else if (60 > seed)
            {	// 20%
                AddItem(new RunebookDyeTub() { UsesRemaining = 5, LootType = LootType.Regular });
            }
            else if (70 > seed)
            {	// 10% lvl 4,5 or 6 TMap
                DropItem(new TreasureMap(Utility.Random(4, 3), Map.Felucca));
            }
            else if (85 > seed)
            {	// 15%
                DropItem(TitleDye.RandomUncommonTitleDye());
            }
            else if (97 > seed)
            {	// 12% glove chance.
                DropItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gloves));
                DropItem(new ArcaneDust());
            }
            else if (99 > seed)
            {	// 2%
                WispLantern i = new WispLantern();
                i.Hue = monster_hue;
                i.SetWispHue(monster_hue);
                i.SetWispName(String.Format("Essence of {0}", monster_name));
                DropItem(i);
            }
            else
            {	// 1%
                if (Utility.RandomBool())
                {
                    DropItem(PowerScroll.CreateRandom(5, 20));
                }
                else
                {
                    var i = new SavageMask();
                    i.Name = String.Format("{0}'s Visage", monster_name);
                    i.LootType = LootType.Blessed;
                    DropItem(i);
                }
            }

            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(SkillScroll.Generate(winner, 120.0, 0));
            DropItem(SkillScroll.Generate(winner, 120.0, 1));
            DropItem(SkillScroll.Generate(winner, 120.0, 2));
            DropItem(new RareCloth());

            // random high level usable weapon OR weapon enhancement.
            if (Utility.RandomBool())
            {
                var weapon = Loot.RandomWeapon();
                if (weapon != null)
                {
                    weapon.DamageLevel = (WeaponDamageLevel)(3 + Utility.Random(3));
                    weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                    if (Utility.Random(50) == 0)
                        weapon.Slayer = SlayerName.Silver;
                    DropItem(weapon);
                }
            }
            else
            {
                DropItem(new Server.Custom.Ubercrafting.WeaponDamageEnhancer());
            }
        }

        public RushChallengeGoodiebag(Serial serial)
            : base(serial)
        {
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