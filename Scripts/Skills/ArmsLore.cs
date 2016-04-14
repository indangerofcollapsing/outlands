using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom;
using Server.Gumps;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Server.SkillHandlers
{
	public class ArmsLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500349 ); // What item do you wish to get information about?

			return TimeSpan.FromSeconds( 2.5 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() : base( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                Item item = null;

                if (targeted is BaseWeapon)
                {
                    item = targeted as Item;

                    bool success = from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that weapon.");
                }

                else if (targeted is BaseShield)
                {
                    item = targeted as Item;

                    bool success = from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that shield.");
                }

                else if (targeted is BaseArmor)
                {
                    item = targeted as Item;

                    bool success = from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that armor.");
                }

                else if (targeted is BaseInstrument)
                {
                    item = targeted as Item;

                    bool success = from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that instrument.");
                }

                else
                    from.SendMessage("You cannot inspect that.");				
			}
		}
	}

    public class ArmsLoreGump : Gump
    {
        public Mobile m_From;
        public Item m_Item;
        public bool m_Success;
        public DisplayMode m_DisplayMode = DisplayMode.Normal;
        public BardMode m_BardMode = BardMode.Provocation;

        public enum DisplayMode
        {
            Normal,
            Adjusted
        }

        public enum BardMode
        {
            Provocation,
            Peacemaking,
            Discordance
        }

        public ArmsLoreGump(Mobile from, Item item, bool success, DisplayMode displayMode, BardMode bardMode): base(50, 50)
        {
            if (from == null | item == null) return;
            if (from.Deleted || !from.Alive || item.Deleted) return;

            m_From = from;
            m_Item = item;
            m_Success = success;
            m_DisplayMode = displayMode;
            m_BardMode = bardMode;

            bool showDetailedInfo = true;
            bool showDurability = m_Success;
            
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;            

            AddPage(0);            

            BaseWeapon weapon = item as BaseWeapon;
            BaseArmor armor = item as BaseArmor;
            BaseShield shield = item as BaseShield;
            BaseInstrument instrument = item as BaseInstrument;
            
            #region Weapon

            if (weapon != null)
            {
                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                string weaponNameText = weapon.Name;

                if (weaponNameText == null)
                    weaponNameText = "";

                weaponNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weaponNameText);

                if (weaponNameText == null)
                    weaponNameText = "";

                string weaponTypeText = "";      
          
                if (weapon.TierLevel > 0 && weapon.Dungeon != DungeonEnum.None)
                    weaponTypeText = Item.GetDungeonName(weapon.Dungeon) + " Dungeon: Tier " + weapon.TierLevel.ToString();

                else if (!(weapon.Resource == CraftResource.Iron || weapon.Resource == CraftResource.RegularWood))
                    weaponTypeText = CraftResources.GetCraftResourceName(weapon.Resource);

                double creatureAccuracyBonus = weapon.GetHitChanceBonus(true, false);
                double playerAccuracyBonus = weapon.GetHitChanceBonus(true, true);

                double tacticsBonus = 0;

                double swingDelay = weapon.GetDelay(from, true).TotalSeconds;

                double accuracyVsCreature = weapon.GetSimulatedHitChance(from, 100, false);
                double accuracyVsPlayer = weapon.GetSimulatedHitChance(from, 100, true);

                double armsLoreCreatureDamageScalarBonus = weapon.GetArmsLoreDamageBonus(from, false);
                double armsLorePlayerDamageScalarBonus = weapon.GetArmsLoreDamageBonus(from, true);

                double damageVsCreatureScalar = (weapon.GetDamageScalar(from, true, false) + armsLoreCreatureDamageScalarBonus) * BaseWeapon.PlayerVsCreatureDamageScalar;
                int damageVsCreatureMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsCreatureScalar);
                int damageVsCreatureMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsCreatureScalar);

                double damageVsCreatureSlayerScalar = (weapon.GetDamageScalar(from, true, false) + armsLoreCreatureDamageScalarBonus) + BaseWeapon.SlayerDamageScalarBonus * BaseWeapon.PlayerVsCreatureDamageScalar;
                int damageVsCreatureSlayerMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsCreatureSlayerScalar);
                int damageVsCreatureSlayerMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsCreatureSlayerScalar);

                double damageVsPlayerScalar = (weapon.GetDamageScalar(from, true, true) + armsLorePlayerDamageScalarBonus) * BaseWeapon.PlayerVsPlayerDamageScalar;
                int damageVsPlayerMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsPlayerScalar);
                int damageVsPlayerMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsPlayerScalar);

                if (damageVsCreatureMin < 1)
                    damageVsCreatureMin = 1;

                if (damageVsCreatureMax < 1)
                    damageVsCreatureMax = 1;

                if (damageVsCreatureSlayerMin < 1)
                    damageVsCreatureSlayerMin = 1;

                if (damageVsCreatureSlayerMax < 1)
                    damageVsCreatureSlayerMax = 1;

                if (damageVsPlayerMin < 1)
                    damageVsPlayerMin = 1;

                if (damageVsPlayerMax < 1)
                    damageVsPlayerMax = 1;

                if (weapon.m_SkillMod != null)
                {
                    if (weapon.m_SkillMod.Skill == SkillName.Tactics)
                        tacticsBonus = weapon.m_SkillMod.Value;
                }

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
                        AddLabel(16, -3, 149, "Guide");

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, weaponNameText), 25, 2603, weaponNameText);
                        AddLabel(Utility.CenteredTextOffset(115, weaponTypeText), 45, 2603, weaponTypeText);

                        //Image
                        AddItem(100 + weapon.IconOffsetX, 95 + weapon.IconOffsetY, weapon.IconItemId, weapon.IconHue);

                        //Display Mode
                        AddLabel(235, 25, 149, "Base Values");
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);

                        //Speed
                        AddLabel(232, 45, textHue, "Speed:");
                        if (showDetailedInfo)
                            AddLabel(281, 45, 2603, weapon.Speed.ToString());
                        else
                            AddLabel(281, 45, 2603, "?");

                        //Damage
                        AddLabel(222, 65, textHue, "Damage:");
                        if (showDetailedInfo)
                            AddLabel(280, 65, 2603, weapon.BaseMinDamage + "-" + weapon.BaseMaxDamage);
                        else
                            AddLabel(280, 65, 2603, "?");

                        //Durability
                        AddLabel(209, 85, textHue, "Durability:");
                        if (showDetailedInfo && showDurability)
                            AddLabel(280, 85, 2603, weapon.HitPoints + "/" + weapon.MaxHitPoints);
                        else
                            AddLabel(280, 85, 2603, "?/?");

                        //Accuracy
                        AddLabel(212, 105, textHue, "Accuracy:");
                        AddLabel(280, 105, 2603, "+" + Utility.CreatePercentageString(creatureAccuracyBonus));

                        //Tactics
                        AddLabel(222, 125, textHue, "Tactics:");
                        AddLabel(280, 125, 2603, "+" + tacticsBonus);

                        //Slayer
                        if (weapon.SlayerGroup != SlayerGroupType.None)
                        {
                            string slayerName = weapon.SlayerGroup.ToString() + " Slaying";
                            AddLabel(Utility.CenteredTextOffset(280, slayerName), 145, 2603, slayerName);
                        }

                        //Arcane Charges
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(169, 165, textHue, "Arcane Charges:");
                            AddLabel(280, 165, 2603, weapon.ArcaneCharges.ToString());
                        }

                        //Experience
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(199, 185, textHue, "Experience:");
                            AddLabel(279, 185, 2603, weapon.Experience.ToString() + "/250");
                        }

                        //Effect Chance
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(26, 165, textHue, "Effect Chance:");
                            AddLabel(127, 165, 2603, "4.1%");
                        }

                        //Effect
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(25, 185, textHue, "Effect:");
                            AddLabel(75, 185, 2603, "Firestorm");
                        }
                    break;
                        
                    case DisplayMode.Adjusted:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
			            AddLabel(16, -3, 149, "Guide");

                        //Display Mode
                        AddLabel(235, 25, 149, "Your Values");
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, weaponNameText), 25, 2603, weaponNameText);
                        AddLabel(Utility.CenteredTextOffset(115, weaponTypeText), 45, 2603, weaponTypeText);
		            
                        //Image
                        AddItem(100 + weapon.IconOffsetX, 95 + weapon.IconOffsetY, weapon.IconItemId, weapon.IconHue);

                        //Swing Speed
                        AddLabel(213, 45, textHue, "Swing Delay:");
                        if (showDetailedInfo)
                            AddLabel(297, 45, 2603, Utility.CreateDecimalString(swingDelay, 2) + "s");
                        else
                            AddLabel(297, 45, 2603, "?");

                        //Accuracy vs Creature
                        AddLabel(153, 82, textHue, "Accuracy vs Creature:");
                        if (showDetailedInfo)
                            AddLabel(297, 82, 2603, Utility.CreateDecimalPercentageString(accuracyVsCreature, 2));
                        else
                            AddLabel(297, 82, 2603, "?");

                        //Damage vs Creature
                        AddLabel(162, 102, textHue, "Damage vs Creature:");
                        if (showDetailedInfo)
                            AddLabel(297, 102, 2603, damageVsCreatureMin.ToString() + "-" + damageVsCreatureMax.ToString());
                        else
                            AddLabel(297, 102, 2603, "?");

                        //Damage If Slayer Type
                        if (weapon.SlayerGroup != SlayerGroupType.None)
                        {
                            AddLabel(147, 122, textHue, "Damage If Slayer Type:");

                            if (showDetailedInfo)
                                AddLabel(297, 122, 2603, damageVsCreatureSlayerMin.ToString() + "-" + damageVsCreatureSlayerMax.ToString());
                            else
                                AddLabel(297, 122, 2603, "?");
                        }

                        //Accuracy vs Player
                        AddLabel(168, 165, textHue, "Accuracy vs Player:");
                        if (showDetailedInfo)
                            AddLabel(297, 165, 2603, Utility.CreateDecimalPercentageString(accuracyVsPlayer, 2));
                        else
                            AddLabel(297, 165, 2603, "?");

                        //Daamge vs Player
                        AddLabel(177, 185, textHue, "Damage vs Player:");
                        if (showDetailedInfo)
                            AddLabel(297, 185, 2603, damageVsPlayerMin.ToString() + "-" + damageVsPlayerMax.ToString());
                        else
                            AddLabel(297, 185, 2603, "?");
                       
                        //Effect Chance
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(26, 165, textHue, "Effect Chance:");
                            AddLabel(127, 165, 2603, "4.1%");
                        }

                        //Effect
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(25, 185, textHue, "Effect:");
                            AddLabel(75, 185, 2603, "Firestorm");
                        }
                    break;
                }
            }

            #endregion

            #region Armor

            if (armor != null && shield == null)
            {
                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                string armorNameText = armor.Name;

                if (armorNameText == null)
                    armorNameText = "";

                armorNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(armorNameText);

                if (armorNameText == null)
                    armorNameText = "";

                string armorTypeText = "";

                if (armor.TierLevel > 0 && armor.Dungeon != DungeonEnum.None)
                    armorTypeText = Item.GetDungeonName(weapon.Dungeon) + " Dungeon: Tier " + armor.TierLevel.ToString();

                else if (!(armor.Resource == CraftResource.Iron || armor.Resource == CraftResource.RegularWood || armor.Resource == CraftResource.RegularLeather))
                    armorTypeText = CraftResources.GetCraftResourceName(armor.Resource);

                double armorValue = (double)armor.RevertArmorBase;

                string meditationText = "0%";
                string suitThemeText = "Resistance";                

                double fullSuitArmorValue = armor.ArmorBase;
                double fullSuitDexPenalty = 0;
                double fullSuitMeditation = 0;

                string fullSuitMeditationText = "0%";

                List<string> m_SuitEffects = new List<string>();
                List<string> m_SuitEffectValues = new List<string>();

                switch (armor.Dungeon)
                {
                }

                switch (armor.MeditationAllowance)
                {
                    case ArmorMeditationAllowance.None: meditationText = "0%"; break;
                    case ArmorMeditationAllowance.Quarter: meditationText = "25%"; break;
                    case ArmorMeditationAllowance.Half: meditationText = "50%"; break;
                    case ArmorMeditationAllowance.ThreeQuarter: meditationText = "75%"; break;
                    case ArmorMeditationAllowance.All: meditationText = "100%"; break;
                }

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Guide                        
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
                        AddLabel(16, -3, 149, "Guide");                        
                      
                        //Display Mode
                        if (armor.TierLevel > 0)
                        {
                            AddLabel(231, 25, 149, "Base Values");
                            AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);
                        }

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, armorNameText), 25, 2603, armorNameText);
                        AddLabel(Utility.CenteredTextOffset(115, armorTypeText), 45, 2603, armorTypeText);

                        //Image
                        AddItem(85 + armor.IconOffsetX, 80 + armor.IconOffsetY, armor.IconItemId, armor.IconHue);

                        //Properties
                        AddLabel(231, 45, textHue, "Armor:");
                        AddLabel(281, 45, 2603, Utility.CreateDecimalString(armorValue, 1));

                        AddLabel(212, 65, textHue, "Dex Loss:");
                        AddLabel(280, 65, 2603, armor.DexBonus.ToString());

                        AddLabel(212, 85, textHue, "Durability:");
                        if (showDurability)
                            AddLabel(280, 85, 2603, armor.HitPoints.ToString() + "/" + armor.MaxHitPoints.ToString());
                        else
                            AddLabel(280, 85, 2603, "?/?");

                        AddLabel(207, 105, textHue, "Meditation:");
                        AddLabel(280, 105, 2603, meditationText);

                        if (armor.TierLevel > 0)
                        {
                            AddLabel(175, 165, textHue, "Arcane Charges:");
                            AddLabel(280, 165, 2603, armor.ArcaneCharges.ToString());
                        }

                        if (armor.TierLevel > 0)
                        {
                            AddLabel(207, 185, textHue, "Experience:");
                            AddLabel(279, 185, 2603, armor.Experience.ToString() + "/250");
                        }

                        //Theme
                        if (armor.TierLevel > 0)
                        {
                            AddLabel(Utility.CenteredTextOffset(113, "Suit Theme"), 165, textHue, "Suit Theme");
                            AddLabel(Utility.CenteredTextOffset(115, suitThemeText), 185, 2603, suitThemeText);
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
			            AddLabel(16, -3, 149, "Guide");

                        //Display Mode
                        AddLabel(209, 25, 149, "Full Suit Effects");
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);                        
                        
                        //Description
                        AddLabel(82, 25, 2603, "Full Suit");
                        AddLabel(54, 45, 2603, armorTypeText);
			            
                        //Image
                        AddItem(85 + armor.IconOffsetX, 80 + armor.IconOffsetY, armor.IconItemId, armor.IconHue);
			           
                        //Suit Effects
                        int startY = 45;

                        for (int a = 0; a < m_SuitEffects.Count; a++)
                        {
                            AddLabel(Utility.CenteredTextOffset(260, m_SuitEffects[a]), startY, textHue, m_SuitEffects[a]);
                            AddLabel(Utility.CenteredTextOffset(260, m_SuitEffectValues[a]), startY + 20, 2603, m_SuitEffectValues[a]);

                            startY += 40;
                        }                        			            
			          
                        //Properties
                        AddLabel(33, 145, textHue, "Total Armor:");
			            AddLabel(115, 145, 2603, fullSuitArmorValue.ToString());

                        AddLabel(51, 165, textHue, "Dex Loss:");			           
			            AddLabel(115, 165, 2603, fullSuitDexPenalty.ToString());

                        AddLabel(46, 185, textHue, "Meditation:");
			            AddLabel(115, 185, 2603, fullSuitMeditationText);			            
                    break;
                }
            }

            #endregion

            #region Shield

            if (shield != null)
            {
                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                string shieldNameText = shield.Name;

                if (shieldNameText == null)
                    shieldNameText = "";

                shieldNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shieldNameText);

                if (shieldNameText == null)
                    shieldNameText = "";

                string shieldTypeText = "";

                if (shield.TierLevel > 0 && shield.Dungeon != DungeonEnum.None)
                    shieldTypeText = Item.GetDungeonName(weapon.Dungeon) + " Dungeon: Tier " + shield.TierLevel.ToString();

                else if (!(shield.Resource == CraftResource.Iron || shield.Resource == CraftResource.RegularWood))
                    shieldTypeText = CraftResources.GetCraftResourceName(shield.Resource);

                double armorValue = shield.ArmorBase;
                double dexPenalty = shield.DexBonus;

                double parryChance = from.Skills[SkillName.Parry].Value * BaseShield.ShieldParrySkillScalar;
                double damageReduction = 1.0 - BaseShield.ShieldParryDamageScalar;

                string meditationText = "0%";
                string effectText = "Resistance";                

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
			            AddLabel(16, -3, 149, "Guide");

                        //Display Mode
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);
                        AddLabel(231, 25, 149, "Base Values");

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, shieldNameText), 25, 2603, shieldNameText);
                        AddLabel(Utility.CenteredTextOffset(115, shieldTypeText), 45, 2603, shieldTypeText);

                        //Image			           
                        AddItem(85 + shield.IconOffsetX, 80 + shield.IconOffsetY, shield.IconItemId, shield.IconHue);
                        			            
                        //Properties
                        AddLabel(228, 45, textHue, "Armor:");
                        AddLabel(281, 45, 2603, armorValue.ToString());

                        AddLabel(210, 65, textHue, "Dex Loss:");			            
			            AddLabel(280, 65, 2603, dexPenalty.ToString());

                        AddLabel(210, 85, textHue, "Durability:");
                        if (success)
                            AddLabel(280, 85, 2603, shield.HitPoints.ToString() + "/" + shield.MaxHitPoints.ToString());
                        else
                            AddLabel(280, 85, 2603, "?/?");

                        AddLabel(205, 105, textHue, "Meditation:");
			            AddLabel(280, 105, 2603, meditationText);

                        if (shield.TierLevel > 0)
                        {
                            AddLabel(174, 165, textHue, "Arcane Charges:");
                            AddLabel(280, 165, 2603, shield.ArcaneCharges.ToString());
                        }

                        if (shield.TierLevel > 0)
                        {
                            AddLabel(206, 185, textHue, "Experience:");
                            AddLabel(279, 185, 2603, shield.Experience.ToString() + "/250");
                        }
			            
                        //Shield Effect
                        if (shield.TierLevel > 0)
                        {
                            AddLabel(Utility.CenteredTextOffset(113, "Shield Effect"), 165, textHue, "Shield Effect");
                            AddLabel(Utility.CenteredTextOffset(115, effectText), 185, 2603, effectText);
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
			            AddLabel(16, -3, 149, "Guide");

                        //Display Mode
                        AddLabel(231, 25, 149, "Your Values");
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, shieldNameText), 25, 2603, shieldNameText);
                        AddLabel(Utility.CenteredTextOffset(115, shieldTypeText), 45, 2603, shieldTypeText);

                        //Image
                        AddItem(85 + shield.IconOffsetX, 80 + shield.IconOffsetY, shield.IconItemId, shield.IconHue);
                        		
	                    //Values
                        string parryChanceText = Utility.CreateDecimalPercentageString(parryChance, 1);
                        string parryReductionText = Utility.CreateDecimalPercentageString(damageReduction, 0);

                        AddLabel(218, 45, textHue, "Parry Chance");
                        AddLabel(Utility.CenteredTextOffset(260, parryChanceText), 65, 2603, parryChanceText);

                        AddLabel(190, 84, textHue, "Parry Damage Reduction");
                        AddLabel(Utility.CenteredTextOffset(260, parryReductionText), 104, 2603, parryReductionText);

                        //Properties
                        AddLabel(68, 145, textHue, "Armor:");
                        AddLabel(115, 145, 2603, armorValue.ToString());

                        AddLabel(49, 165, textHue, "Dex Loss:");
                        AddLabel(115, 165, 2603, dexPenalty.ToString());

                        AddLabel(44, 185, textHue, "Meditation:");
                        AddLabel(115, 185, 2603, meditationText);
                    break;
                }
            }

            #endregion

            #region Instrument

            if (instrument != null)
            {
                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                string instrumentNameText = instrument.Name;

                if (instrumentNameText == null)
                    instrumentNameText = "";

                instrumentNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(instrumentNameText);

                if (instrumentNameText == null)
                    instrumentNameText = "";

                string instrumentTypeText = "";

                if (instrument.TierLevel > 0 && instrument.Dungeon != DungeonEnum.None)
                    instrumentTypeText = Item.GetDungeonName(instrument.Dungeon) + " Dungeon: Tier " + instrument.TierLevel.ToString();

                else if (instrument.Resource != CraftResource.RegularWood)
                    instrumentTypeText = CraftResources.GetCraftResourceName(instrument.Resource);

                double bardSkillBonus = BaseInstrument.GetBardBonusSkill(m_From, null, instrument);
                double effectiveSkill = 0;                

                switch (m_BardMode)
                {
                    case BardMode.Provocation: effectiveSkill = m_From.Skills[SkillName.Provocation].Value + bardSkillBonus; break;
                    case BardMode.Peacemaking: effectiveSkill = m_From.Skills[SkillName.Peacemaking].Value + bardSkillBonus; break;
                    case BardMode.Discordance: effectiveSkill = m_From.Skills[SkillName.Discordance].Value + bardSkillBonus; break;
                }

                double normal10 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 10);
                double slayer10 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 10);

                double normal20 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 20);
                double slayer20 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 20);

                double normal30 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 30);
                double slayer30 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 30);

                double normal40 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 40);
                double slayer40 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 40);

                double normal50 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 50);
                double slayer50 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 50);

                double normalMinimum = effectiveSkill * BaseInstrument.MinimumEffectiveChanceScalar;
                double slayerMinimum = (effectiveSkill + BaseInstrument.SlayerSkillBonus) * BaseInstrument.MinimumEffectiveChanceScalar;

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
			            AddLabel(16, -3, 149, "Guide");
                        
                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, instrumentNameText), 25, 2603, instrumentNameText);
                        AddLabel(Utility.CenteredTextOffset(115, instrumentTypeText), 45, 2603, instrumentTypeText);

                        //Display Mode
                        AddLabel(231, 25, 149, "Base Values");
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);
			            
                        //Image
                        AddItem(75 + instrument.IconOffsetX, 95 + instrument.IconOffsetY, instrument.IconItemId, instrument.IconHue);

                        //Properties
                        AddLabel(212, 45, textHue, "Durability:");
                        AddLabel(281, 45, 2603, instrument.UsesRemaining.ToString() + "/" + instrument.InitMaxUses.ToString());

                        AddLabel(207, 65, textHue, "Bard Skill:");
                        AddLabel(280, 65, 2603, "+" + bardSkillBonus);

                        if (instrument.SlayerGroup != SlayerGroupType.None)
                        {
                            string slayerName = instrument.SlayerGroup.ToString() + " Slaying";
                            AddLabel(Utility.CenteredTextOffset(280, slayerName), 85, 2603, slayerName);
                        }

                        if (instrument.TierLevel > 0)
                        {
                            AddLabel(169, 165, textHue, "Arcane Charges:");
                            AddLabel(280, 165, 2603, instrument.ArcaneCharges.ToString());
                        }

                        if (instrument.TierLevel > 0)
                        {
                            AddLabel(199, 185, textHue, "Experience:");
                            AddLabel(279, 185, 2603, instrument.Experience.ToString() + "/250");
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Guide
                        AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
                        AddLabel(16, -3, 149, "Guide");

                        //Display Mode
                        AddButton(327, 29, 1210, 1209, 1, GumpButtonType.Reply, 0);
                        AddLabel(231, 25, 149, "Your Values");

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, instrumentNameText), 25, 2603, instrumentNameText);
                        AddLabel(Utility.CenteredTextOffset(115, instrumentTypeText), 45, 2603, instrumentTypeText);

                        //Image
                        AddItem(75 + instrument.IconOffsetX, 95 + instrument.IconOffsetY, instrument.IconItemId, instrument.IconHue);

                        //Skill Type
                        AddLabel(65, 149, 2562, "Displaying");
                        AddButton(33, 174, 2223, 2223, 3, GumpButtonType.Reply, 0);
                        AddButton(138, 174, 2224, 2224, 4, GumpButtonType.Reply, 0);

                        switch (m_BardMode)
                        {
                            case BardMode.Provocation: AddLabel(61, 170, BaseInstrument.ProvokedTextHue, "Provocation"); break;
                            case BardMode.Peacemaking: AddLabel(61, 170, BaseInstrument.PacifiedTextHue, "Peacemaking"); break;
                            case BardMode.Discordance: AddLabel(61, 170, BaseInstrument.DiscordedTextHue, "Discordance"); break;
                        }

                        AddLabel(40, 190, textHue, "Effective Skill:");
                        AddLabel(136, 190, 2603, Utility.CreateDecimalString(effectiveSkill, 1));

                        AddLabel(172, 50, 2562, "Success vs Difficulty Value");
                        AddLabel(245, 70, textHue, "Normal");
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(296, 70, textHue, "Slayer");

                        AddLabel(186, 90, textHue, "Diff");
                        AddLabel(221, 90, textHue, "10:");
			            AddLabel(250, 90, 2603, Utility.CreateDecimalPercentageString(normal10, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 90, 2603, Utility.CreateDecimalPercentageString(slayer10, 1));

                        AddLabel(186, 110, textHue, "Diff 20:");
                        AddLabel(250, 110, 2603, Utility.CreateDecimalPercentageString(normal20, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 110, 2603, Utility.CreateDecimalPercentageString(slayer20, 1));

                        AddLabel(186, 130, textHue, "Diff 30:");
                        AddLabel(250, 130, 2603, Utility.CreateDecimalPercentageString(normal30, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 130, 2603, Utility.CreateDecimalPercentageString(slayer30, 1));

                        AddLabel(186, 150, textHue, "Diff 40:");
                        AddLabel(250, 150, 2603, Utility.CreateDecimalPercentageString(normal40, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 150, 2603, Utility.CreateDecimalPercentageString(slayer40, 1));

                        AddLabel(186, 170, textHue, "Diff 50:");
                        AddLabel(250, 170, 2603, Utility.CreateDecimalPercentageString(normal50, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 170, 2603, Utility.CreateDecimalPercentageString(slayer50, 1));

                        AddLabel(187, 190, textHue, "Minimum:");
                        AddLabel(250, 190, 2603, Utility.CreateDecimalPercentageString(normalMinimum, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 190, 2603, Utility.CreateDecimalPercentageString(slayerMinimum, 1));				           
                    break;
                }
            }

            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_Item == null) return;
            if (m_From.Deleted || !m_From.Alive || m_Item.Deleted) return;

            BaseWeapon weapon = m_Item as BaseWeapon;
            BaseArmor armor = m_Item as BaseArmor;
            BaseShield shield = m_Item as BaseShield;
            BaseInstrument instrument = m_Item as BaseInstrument;

            bool isWeapon = weapon != null;
            bool isArmor = armor != null && shield == null;
            bool isShield = shield != null;
            bool isInstrument = instrument != null;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Display Made
                case 1:                    
                    switch (m_DisplayMode)
                    {
                        case DisplayMode.Normal:
                            if (isArmor && !isShield)
                            {
                                if (armor.TierLevel > 0)
                                {
                                    m_DisplayMode = DisplayMode.Adjusted;
                                    m_From.SendSound(0x055);
                                }
                            }

                            else
                            {
                                m_DisplayMode = DisplayMode.Adjusted;
                                m_From.SendSound(0x055);
                            }
                        break;

                        case DisplayMode.Adjusted:
                            m_DisplayMode = DisplayMode.Normal;
                            m_From.SendSound(0x055);
                        break;
                    }                                        

                    closeGump = false;
                break;

                //Guide
                    case 2:
                    closeGump = false;
                break;

                //Barding Skill Previous
                case 3:
                    if (isInstrument)
                    {
                        switch (m_BardMode)
                        {
                            case BardMode.Provocation: m_BardMode = BardMode.Discordance; break;
                            case BardMode.Peacemaking: m_BardMode = BardMode.Provocation; break;
                            case BardMode.Discordance: m_BardMode = BardMode.Peacemaking; break;
                        }
                    }

                    closeGump = false;
                break;

                //Barding Skill Right
                case 4:
                if (isInstrument)
                {
                    switch (m_BardMode)
                    {
                        case BardMode.Provocation: m_BardMode = BardMode.Peacemaking; break;
                        case BardMode.Peacemaking: m_BardMode = BardMode.Discordance; break;
                        case BardMode.Discordance: m_BardMode = BardMode.Provocation; break;
                    }
                }

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_From.CloseGump(typeof(ArmsLoreGump));
                m_From.SendGump(new ArmsLoreGump(m_From, m_Item, m_Success, m_DisplayMode, m_BardMode));
            }
        }
    }
}