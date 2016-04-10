using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom;
using Server.Gumps;
using System.Globalization;

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

                    bool success = from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that weapon.");
				}

				else if (targeted is BaseArmor)
				{
                    item = targeted as Item;

                    bool success = from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    from.CloseGump(typeof(ArmsLoreGump));
                    from.SendGump(new ArmsLoreGump(from, item, success, ArmsLoreGump.DisplayMode.Normal));

                    from.SendSound(0x058);

                    if (!success)
                        from.SendMessage("You are uncertain of the full details of that armor.");					
				}

				else				
					from.SendLocalizedMessage( 500352 ); // This is neither weapon nor armor.				
			}
		}
	}

    public class ArmsLoreGump : Gump
    {
        public Mobile m_From;
        public Item m_Item;
        public bool m_Success;
        public DisplayMode m_DisplayMode;

        public enum DisplayMode
        {
            Normal,
            Adjusted
        }

        public ArmsLoreGump(Mobile from, Item item, bool success, DisplayMode displayMode): base(50, 50)
        {
            if (from == null | item == null) return;
            if (from.Deleted || !from.Alive || item.Deleted) return;

            m_From = from;
            m_Item = item;
            m_Success = success;
            m_DisplayMode = displayMode;

            bool showDetailedInfo = m_Success;
            
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;            

            AddPage(0);
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

            BaseWeapon weapon = item as BaseWeapon;
            
            if (weapon != null)
            {
                string weaponNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weapon.Name); 
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
                    #region Normal

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
                        AddLabel(221, 65, textHue, "Damage:");
                        if (showDetailedInfo)
                            AddLabel(280, 65, 2603, weapon.BaseMinDamage + "-" + weapon.BaseMaxDamage);
                        else
                            AddLabel(280, 65, 2603, "?");

                        //Durability
                        AddLabel(205, 85, textHue, "Durability:");
                        if (showDetailedInfo)
                            AddLabel(280, 85, 2603, weapon.HitPoints + "/" + weapon.MaxHitPoints);
                        else
                            AddLabel(280, 85, 2603, "?");


                        //Accuracy
                        AddLabel(209, 105, textHue, "Accuracy:");
                        AddLabel(280, 105, 2603, "+" + Utility.CreatePercentageString(creatureAccuracyBonus));

                        //Tactics
                        AddLabel(220, 125, textHue, "Tactics:");
                        AddLabel(280, 125, 2603, "+" + tacticsBonus);

                        //Slayer
                        if (weapon.SlayerGroup != SlayerGroupType.None)
                            AddLabel(214, 145, 2603, weapon.SlayerGroup.ToString() + " Slaying");

                        //Arcane Charges
                        if (weapon.ArcaneChargesMax > 0)
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

                    #endregion

                    #region Adjusted

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
                        AddLabel(150, 82, textHue, "Accuracy vs Creature:");
                        if (showDetailedInfo)
                            AddLabel(297, 82, 2603, Utility.CreateDecimalPercentageString(accuracyVsCreature, 2));
                        else
                            AddLabel(297, 82, 2603, "?");

                        //Damage vs Creature
                        AddLabel(160, 102, textHue, "Damage vs Creature:");
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
                        AddLabel(165, 165, textHue, "Accuracy vs Player:");
                        if (showDetailedInfo)
                            AddLabel(297, 165, 2603, Utility.CreateDecimalPercentageString(accuracyVsPlayer, 2));
                        else
                            AddLabel(297, 165, 2603, "?");

                        //Daamge vs Player
                        AddLabel(176, 185, textHue, "Damage vs Player:");
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

                    #endregion
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Display Made
                case 1:
                    switch (m_DisplayMode)
                    {
                        case DisplayMode.Normal: m_DisplayMode = DisplayMode.Adjusted; break;
                        case DisplayMode.Adjusted: m_DisplayMode = DisplayMode.Normal; break;
                    }

                    m_From.SendSound(0x055);

                    closeGump = false;
                break;

                //Guide
                    case 2:
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_From.CloseGump(typeof(ArmsLoreGump));
                m_From.SendGump(new ArmsLoreGump(m_From, m_Item, m_Success, m_DisplayMode));
            }
        }
    }
}