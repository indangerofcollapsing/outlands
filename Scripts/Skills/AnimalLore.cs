using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System.Globalization;

namespace Server.SkillHandlers
{
    public class AnimalLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.AnimalLore].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();
            m.SendMessage("What creature would you like to examine?");

            return TimeSpan.FromSeconds(2.5);
        }

        private class InternalTarget : Target
        {
            public InternalTarget(): base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (!from.Alive)
                    from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.

                else if (targeted is BaseCreature)
                {
                    BaseCreature bc_Creature = (BaseCreature)targeted;                    

                    if (bc_Creature.IsHenchman)
                    {
                        from.SendMessage("You feel you would make a better evaluation of that individual with the Anatomy skill.");
                        return;
                    }

                    bool gumpSuccess = false;

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster == from)
                        gumpSuccess = true;

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.AnimalLoreCooldown * 1000);

                    if (from.CheckSkill(SkillName.AnimalLore, 0, 120, 1))
                        gumpSuccess = true;

                    if (!gumpSuccess)
                    {
                        from.SendMessage("You can't think of anything in particular about that creature.");
                        return;
                    }

                    from.SendGump(new AnimalLoreGump(player, bc_Creature, AnimalLoreGump.AnimalLoreGumpPage.Stats));
                }

                else
                    from.SendMessage("That is not a tameable creature.");
            }
        }
    }

    public class AnimalLoreGump : Gump
    {
        public PlayerMobile pm_Player;
        public BaseCreature bc_Creature;
        public AnimalLoreGumpPage m_Page;

        public enum AnimalLoreGumpPage
        {
            Stats,
            Traits,
            Info
        }

        public AnimalLoreGump(PlayerMobile player, BaseCreature creature, AnimalLoreGumpPage page): base(50, 50)
        {
            if (player == null || creature == null)
                return;

            pm_Player = player;
            bc_Creature = creature;
            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int HeaderTextHue = 2603;
            int WhiteTextHue = 2036;
            int MainTextHue = 149; // 149; //2036          
            int GreenTextHue = 0x3F;
            int RedTextHue = 0x22;
            int ValueTextHue = 2036; //2610
            int DifficultyTextHue = 2114;
            int SlayerGroupTextHue = 2606;

            AddImage(8, 4, 1250, 2499); //Background

            string creatureDisplayName = "";

            if (bc_Creature.IsHenchman)
            {
                string rawName = bc_Creature.RawName;
                string title = bc_Creature.Title;

                if (title.Contains(" the "))
                    title.Replace(" the ", "");

                rawName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawName);

                if (title != "")
                    title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);

                creatureDisplayName = rawName + " the " + title;
            }

            else
            {
                BaseCreature creatureInstance = (BaseCreature)Activator.CreateInstance(bc_Creature.GetType());

                if (bc_Creature.Controlled)
                {
                    //Renamed Creature
                    if (creature.Name != bc_Creature.RawName)
                    {
                        string sName = creature.Name;

                        if (sName.IndexOf("an ") > -1)
                            sName = sName.Substring(2, sName.Length - 2);

                        else if (sName.IndexOf("a ") > -1)
                            sName = sName.Substring(1, sName.Length - 1);

                        sName = sName.Trim();

                        creatureDisplayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bc_Creature.RawName) + " the " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sName);
                    }

                    else
                        creatureDisplayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bc_Creature.RawName);
                }

                else
                    creatureDisplayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bc_Creature.RawName);

                if (creatureInstance != null)
                    creatureInstance.Delete();
            }

            AddLabel(Utility.CenteredTextOffset(170, creatureDisplayName), 15, HeaderTextHue, creatureDisplayName);

            int traitsAvailable = 0;

            AddLabel(10, 0, 149, "Guide");
            AddButton(14, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);

            switch (m_Page)
            {
                //Main
                case AnimalLoreGumpPage.Stats:
                    AddLabel(78, 370, WhiteTextHue, "Traits");
                    AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);

                    if (traitsAvailable > 0)            
                        AddLabel(123, 370, GreenTextHue, "(" + traitsAvailable.ToString() + " Available)");
                    
                    AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                    AddLabel(259, 370, WhiteTextHue, "Info");

                    #region Main

                    int shrinkTableIcon = ShrinkTable.Lookup(bc_Creature);

                    if (shrinkTableIcon == 6256)
                        shrinkTableIcon = 7960;

                    if (bc_Creature.IsHenchman)
                    {
                        Custom.BaseHenchman henchman = bc_Creature as Custom.BaseHenchman; 

                        int henchmanIcon = 8454;
                        int henchmanHue = 0;

                        if (bc_Creature.Female)
                            henchmanIcon = 8455;

                        if (!henchman.HenchmanHumanoid)
                        {
                            henchmanIcon = bc_Creature.TamedItemId;
                            henchmanHue = bc_Creature.TamedItemHue;
                        }

                        AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, henchmanIcon, henchmanHue);
                    }

                    else
                    {
                        if (bc_Creature.TamedItemId != -1) //Creature Icon
                            AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, bc_Creature.TamedItemId, bc_Creature.TamedItemHue);
                
                        else
                            AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, shrinkTableIcon, 0);
                    }
                    
                    string creatureDifficulty = Utility.CreateDecimalString(bc_Creature.InitialDifficulty, 1);
                    string slayerGroup = bc_Creature.SlayerGroup.ToString();

                    int level = 2;
                    int experience = bc_Creature.Experience;
                    int maxExperience = bc_Creature.MaxExperience;

                    double passiveTamingSkillGainRemaining = 2.5;
                    string passiveTamingSkillGainRemainingText = Utility.CreateDecimalString(passiveTamingSkillGainRemaining, 1);
                    
                    int hitsAdjusted = bc_Creature.Hits;
                    int hitsMaxAdjusted = bc_Creature.HitsMax;

                    int stamAdjusted = bc_Creature.Stam;
                    int stamMaxAdjusted = bc_Creature.StamMax;

                    int manaAdjusted = bc_Creature.Mana;
                    int manaMaxAdjusted = bc_Creature.ManaMax;

                    int minDamageAdjusted = bc_Creature.DamageMin;
                    int maxDamageAdjusted = bc_Creature.DamageMax;

                    double wrestlingAdjusted = bc_Creature.Skills.Wrestling.Value;
                    double evalIntAdjusted = bc_Creature.Skills.EvalInt.Value;
                    double mageryAdjusted = bc_Creature.Skills.Magery.Value;
                    double magicResistAdjusted = bc_Creature.Skills.MagicResist.Value;
                    double poisoningAdjusted = bc_Creature.Skills.Poisoning.Value;

                    int virtualArmorAdjusted = bc_Creature.VirtualArmor;            

                    //Tamed Scalars 
                    string hitsTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxHitsCreationScalar - 1), 1);
                    int hitsTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxHitsCreationScalar - 1) >= 0)
                    {
                        hitsTamedScalar = "+" + hitsTamedScalar;
                        hitsTamedColor = GreenTextHue;
                    }
                    if (hitsTamedScalar.Length == 3)
                        hitsTamedScalar = hitsTamedScalar.Insert(2, ".0");

                    string stamTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseDexCreationScalar - 1), 1);
                    int stamTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseDexCreationScalar - 1) >= 0)
                    {
                        stamTamedScalar = "+" + stamTamedScalar;
                        stamTamedColor = GreenTextHue;
                    }
                    if (stamTamedScalar.Length == 3)
                        stamTamedScalar = stamTamedScalar.Insert(2, ".0");

                    string manaTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxManaCreationScalar - 1), 1);
                    int manaTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxManaCreationScalar - 1) >= 0)
                    {
                        manaTamedScalar = "+" + manaTamedScalar;
                        manaTamedColor = GreenTextHue;
                    }
                    if (manaTamedScalar.Length == 3)
                        manaTamedScalar = manaTamedScalar.Insert(2, ".0");

                    string damageTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxDamageCreationScalar - 1), 1);
                    int damageTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxDamageCreationScalar - 1) >= 0)
                    {
                        damageTamedScalar = "+" + damageTamedScalar;
                        damageTamedColor = GreenTextHue;
                    }
                    if (damageTamedScalar.Length == 3)
                        damageTamedScalar = damageTamedScalar.Insert(2, ".0");

                    string virtualArmorTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1), 1);
                    int virtualArmorTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1) >= 0)
                    {
                        virtualArmorTamedScalar = "+" + virtualArmorTamedScalar;
                        virtualArmorTamedColor = GreenTextHue;
                    }
                    if (virtualArmorTamedScalar.Length == 3)
                        virtualArmorTamedScalar = virtualArmorTamedScalar.Insert(2, ".0");

                    string wrestlingTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseWrestlingCreationScalar - 1), 1);
                    int wrestlingTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseWrestlingCreationScalar - 1) >= 0)
                    {
                        wrestlingTamedScalar = "+" + wrestlingTamedScalar;
                        wrestlingTamedColor = GreenTextHue;
                    }
                    if (wrestlingTamedScalar.Length == 3)
                        wrestlingTamedScalar = wrestlingTamedScalar.Insert(2, ".0");

                    string evalIntTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseEvalIntCreationScalar - 1), 1);
                    int evalIntTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseEvalIntCreationScalar - 1) >= 0)
                    {
                        evalIntTamedScalar = "+" + evalIntTamedScalar;
                        evalIntTamedColor = GreenTextHue;
                    }
                    if (evalIntTamedScalar.Length == 3)
                        evalIntTamedScalar = evalIntTamedScalar.Insert(2, ".0");

                    string mageryTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMageryCreationScalar - 1), 1);
                    int mageryTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMageryCreationScalar - 1) >= 0)
                    {
                        mageryTamedScalar = "+" + mageryTamedScalar;
                        mageryTamedColor = GreenTextHue;
                    }
                    if (mageryTamedScalar.Length == 3)
                        mageryTamedScalar = mageryTamedScalar.Insert(2, ".0");

                    string magicResistTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMagicResistCreationScalar - 1), 1);
                    int magicResistTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMagicResistCreationScalar - 1) >= 0)
                    {
                        magicResistTamedScalar = "+" + magicResistTamedScalar;
                        magicResistTamedColor = GreenTextHue;
                    }
                    if (magicResistTamedScalar.Length == 3)
                        magicResistTamedScalar = magicResistTamedScalar.Insert(2, ".0");

                    string poisoningTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBasePoisoningCreationScalar - 1), 1);
                    int poisoningTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBasePoisoningCreationScalar - 1) >= 0)
                    {
                        poisoningTamedScalar = "+" + poisoningTamedScalar;
                        poisoningTamedColor = GreenTextHue;
                    }
                    if (poisoningTamedScalar.Length == 3)
                        poisoningTamedScalar = poisoningTamedScalar.Insert(2, ".0");

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                    {

                        AddLabel(160, 50, MainTextHue, "Level:");
                        AddLabel(205, 50, GreenTextHue, level.ToString());

                        AddLabel(170, 70, MainTextHue, "Exp:");
                        AddLabel(205, 70, ValueTextHue, experience.ToString() + " / " + maxExperience.ToString());

                        AddLabel(155, 90, MainTextHue, "Passive Taming");
                        AddLabel(125, 105, MainTextHue, "Skill Gain Remaining");
                        if (passiveTamingSkillGainRemaining > 0)
                            AddLabel(255, 100, GreenTextHue, passiveTamingSkillGainRemainingText);
                        else
                            AddLabel(255, 100, RedTextHue, passiveTamingSkillGainRemainingText);
                    }

                    else
                    {
                        AddLabel(170, 45, MainTextHue, "Creature Difficulty");
                        AddLabel(Utility.CenteredTextOffset(220, creatureDifficulty), 65, DifficultyTextHue, creatureDifficulty);

                        AddLabel(185, 85, MainTextHue, "Slayer Group");
                        AddLabel(205, 105, SlayerGroupTextHue, slayerGroup);

                        hitsAdjusted = (int)((double)bc_Creature.TamedBaseMaxHits * bc_Creature.TamedBaseMaxHitsCreationScalar);
                        hitsMaxAdjusted = hitsAdjusted;

                        stamAdjusted = (int)((double)bc_Creature.TamedBaseDex * bc_Creature.TamedBaseDexCreationScalar);
                        stamMaxAdjusted = stamAdjusted;

                        manaAdjusted = (int)((double)bc_Creature.TamedBaseMaxMana * bc_Creature.TamedBaseMaxManaCreationScalar);
                        manaMaxAdjusted = manaAdjusted;                

                        minDamageAdjusted = (int)((double)bc_Creature.TamedBaseMinDamage * bc_Creature.TamedBaseMinDamageCreationScalar);
                        maxDamageAdjusted = (int)((double)bc_Creature.TamedBaseMaxDamage * bc_Creature.TamedBaseMaxDamageCreationScalar);

                        virtualArmorAdjusted = (int)((double)bc_Creature.TamedBaseVirtualArmor * bc_Creature.TamedBaseVirtualArmorCreationScalar);

                        wrestlingAdjusted = bc_Creature.TamedBaseWrestling * bc_Creature.TamedBaseWrestlingCreationScalar;
                        evalIntAdjusted = bc_Creature.TamedBaseEvalInt * bc_Creature.TamedBaseEvalIntCreationScalar;
                        mageryAdjusted = bc_Creature.TamedBaseMagery * bc_Creature.TamedBaseMageryCreationScalar;                
                        magicResistAdjusted = bc_Creature.TamedBaseMagicResist * bc_Creature.TamedBaseMagicResistCreationScalar;
                        poisoningAdjusted = bc_Creature.TamedBasePoisoning * bc_Creature.TamedBasePoisoningCreationScalar;                
                    }
                    
                    int labelX = 45;
                    int valuesX = 140;
                    int tamedScalarsX = 245;

                    int startY = 125;

                    int rowHeight = 18;
                    int rowSpacer = 0;

                    bool showTamedScalars = false;

                    if (bc_Creature.Tameable)
                    {
                        showTamedScalars = true;

                        if (bc_Creature.IsHenchman)
                            AddLabel(labelX, startY, MainTextHue, "Min Begging:");
                        else
                            AddLabel(labelX, startY, MainTextHue, "Min Taming:");           

                        AddLabel(valuesX, startY, ValueTextHue, bc_Creature.MinTameSkill.ToString());
                        startY += rowHeight;

                        AddLabel(labelX, startY, MainTextHue, "Control Slots:");
                        AddLabel(valuesX, startY, ValueTextHue, bc_Creature.ControlSlots.ToString());               

                        AddLabel(242, startY, MainTextHue, "vs Avg.");

                        startY += rowHeight;
                        startY += rowSpacer;
                    }

                    AddLabel(labelX, startY, MainTextHue, "Hits:");
                    AddLabel(valuesX, startY, ValueTextHue, hitsAdjusted + " / " + hitsMaxAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, hitsTamedColor, hitsTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Stam:");
                    AddLabel(valuesX, startY, ValueTextHue, stamAdjusted + " / " + stamMaxAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, stamTamedColor, stamTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Mana:");
                    AddLabel(valuesX, startY, ValueTextHue, manaAdjusted + " / " + manaMaxAdjusted);
                    if (showTamedScalars && manaAdjusted > 0 && manaMaxAdjusted > 0)
                        AddLabel(tamedScalarsX, startY, manaTamedColor, manaTamedScalar);

                    startY += rowHeight;
                    startY += rowSpacer;
            
                    AddLabel(labelX, startY, MainTextHue, "Damage:");
                    AddLabel(valuesX, startY, ValueTextHue, minDamageAdjusted + " - " + maxDamageAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, damageTamedColor, damageTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Armor:");
                    AddLabel(valuesX, startY, ValueTextHue, virtualArmorAdjusted.ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, virtualArmorTamedColor, virtualArmorTamedScalar);

                    startY += rowHeight;
                    startY += rowSpacer;

                    if (bc_Creature.IsHenchman)
                        AddLabel(labelX, startY, MainTextHue, "Combat Skill:");
                    else
                        AddLabel(labelX, startY, MainTextHue, "Wrestling:");
                    AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(wrestlingAdjusted).ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, wrestlingTamedColor, wrestlingTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Magery:");
                    if (mageryAdjusted == 0)
                        AddLabel(valuesX, startY, ValueTextHue, "-");
                    else
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(mageryAdjusted).ToString());
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, mageryTamedColor, mageryTamedScalar);
                    }

                    startY += rowHeight;
            
                    AddLabel(labelX, startY, MainTextHue, "Eval Int:");
                    if (evalIntAdjusted == 0)
                        AddLabel(valuesX, startY, ValueTextHue, "-");
                    else
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(evalIntAdjusted).ToString());
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, evalIntTamedColor, evalIntTamedScalar);
                    }

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Magic Resist:");
                    AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(magicResistAdjusted).ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, magicResistTamedColor, magicResistTamedScalar);

                    startY += rowHeight;
            
                    AddLabel(labelX, startY, MainTextHue, "Poisoning:");
                    if (bc_Creature.HitPoison != null)
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(poisoningAdjusted).ToString() + " (" + bc_Creature.HitPoison.Name + ")");
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, poisoningTamedColor, poisoningTamedScalar);
                    }

                    else
                        AddLabel(valuesX, startY, ValueTextHue, "-");           

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Poison Resist:");
                    if (bc_Creature.PoisonResistance > 0)
                    {
                        if (bc_Creature.PoisonResistance > 1)
                            AddLabel(valuesX, startY, ValueTextHue, bc_Creature.PoisonResistance.ToString() + " Levels");
                        else
                            AddLabel(valuesX, startY, ValueTextHue, bc_Creature.PoisonResistance.ToString() + " Level");
                    }

                    else
                        AddLabel(valuesX, startY, ValueTextHue, "-");

                    startY += rowHeight;                   

                    #endregion
                break;

                //Traits
                case AnimalLoreGumpPage.Traits:
                    AddLabel(78, 370, WhiteTextHue, "Stats");
                    AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);
                    
                    AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                    AddLabel(259, 370, WhiteTextHue, "Info");

                    #region Traits

                    AddLabel(145, 45, 2606, "Traits");

                    //Loop
                    AddLabel(Utility.CenteredTextOffset(80, "Sturdy"), 60, GreenTextHue, "Sturdy");
                    AddItem(28, 83, 7028, 0);
                    AddButton(80, 93, 2118, 2117, 10 + 0, GumpButtonType.Reply, 0);
                    AddLabel(100, 90, 2550, "Info");                    

                    AddLabel(Utility.CenteredTextOffset(230, "Mender"), 60, WhiteTextHue, "Mender");
                    AddItem(195, 90, 3618, 0);
                    AddButton(240, 93, 2118, 2117, 10 + 1, GumpButtonType.Reply, 0);
                    AddLabel(260, 90, 2550, "Info");

                    AddButton(141, 89, 9909, 9909, 10 + 2, GumpButtonType.Reply, 0); //Select Left
			        AddButton(167, 90, 9903, 9903, 10 + 3, GumpButtonType.Reply, 0); //Select Right
                    
                    //-----

                    AddLabel(92, 334, 68, "Confirm Trait Selection");
			        AddButton(137, 357, 2076, 2075, 9, GumpButtonType.Reply, 0);

                    #endregion
                break;

                //Info
                case AnimalLoreGumpPage.Info:
                    AddLabel(78, 370, WhiteTextHue, "Traits");
                    AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);

                    if (traitsAvailable > 0)            
                        AddLabel(123, 370, GreenTextHue, "(" + traitsAvailable.ToString() + " Available)");
                    
                    AddButton(221, 369, 4029, 4033, 3, GumpButtonType.Reply, 0);
                    AddLabel(259, 370, WhiteTextHue, "Stats");

                    #region Info
                    #endregion
                break;
            }    
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (pm_Player == null || bc_Creature == null)
                return;

            bool closeGump = true;

            switch (m_Page)
            {
                case AnimalLoreGumpPage.Stats:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Traits
                        case 2:
                            m_Page = AnimalLoreGumpPage.Traits;
                            closeGump = false;
                        break;

                        //Traits
                        case 3:
                            m_Page = AnimalLoreGumpPage.Info;
                            closeGump = false;
                        break;
                    }
                break;

                case AnimalLoreGumpPage.Traits:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                            break;

                        //Stats
                        case 2:
                            m_Page = AnimalLoreGumpPage.Stats;
                            closeGump = false;
                            break;

                        //Info
                        case 3:
                            m_Page = AnimalLoreGumpPage.Info;
                            closeGump = false;
                        break;

                        //Confirm Selection
                        case 9:
                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)
                    {
                        //TEST: Finish

                        closeGump = false;
                    }
                break;

                case AnimalLoreGumpPage.Info:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Traits
                        case 2:
                            m_Page = AnimalLoreGumpPage.Traits;
                            closeGump = false;
                        break;

                        //Stats
                        case 3:
                            m_Page = AnimalLoreGumpPage.Stats;
                            closeGump = false;
                        break;
                    }
                break;
            }

            if (!closeGump)
            {
                pm_Player.CloseGump(typeof(AnimalLoreGump));
                pm_Player.SendGump(new AnimalLoreGump(pm_Player, bc_Creature, m_Page));
            }
        }

        public double RoundToTenth(double skillValue)
        {
            double newValue = skillValue;

            newValue *= 10;
            newValue = Math.Round(newValue);
            newValue /= 10;

            return newValue;
        }
    }
}