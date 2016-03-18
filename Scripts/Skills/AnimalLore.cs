using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

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

                    if (bc_Creature is TankDragon)
                    {
                        from.SendMessage("That seems like a good tank for Fire Dungeon.");
                        return;
                    }

                    bool gumpSuccess = false;

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster == from)
                        gumpSuccess = true;

                    //Passive Skill Check
                    if (from.CheckTargetSkill(SkillName.AnimalLore, bc_Creature, 0.0, 120.0))
                    {
                        try {                
                            from.SendGump(new AnimalLoreGump(bc_Creature));
                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                        gumpSuccess = false;
                    }

                    else if (!gumpSuccess)
                        from.SendMessage("You can't think of anything in particular about that creature.");

                    try {
                        if (gumpSuccess)
                            from.SendGump(new AnimalLoreGump(bc_Creature));
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }

                else
                    from.SendMessage("That is not a tameable creature.");
            }
        }
    }

    public class AnimalLoreGump : Gump
    {
        public AnimalLoreGump(BaseCreature bc_Creature)
            : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            int specialTextHue = 5;
            int textHue = 2036;

            AddImage(8, 4, 1250); //Background

            BaseCreature creature = (BaseCreature)Activator.CreateInstance(bc_Creature.GetType());

            if (creature != null)
            {                
                if (bc_Creature.IsHenchman)
                    AddLabel(80, 17, textHue, bc_Creature.RawName + " " + bc_Creature.Title);   //Henchman Name + Title            

                else
                {   
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

                            AddLabel(80, 17, textHue, bc_Creature.RawName + " the " + sName);  //Creature Name + Creature Type
                        }

                        else
                            AddLabel(80, 17, textHue, bc_Creature.RawName); //Creature Type
                    }

                    else
                        AddLabel(80, 17, textHue, bc_Creature.RawName); //Creature Type
                }

                creature.Delete();
            }

            else
                AddLabel(100, 17, textHue, bc_Creature.RawName); //Creature Type            

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

            //--------------

            int hitsAdjusted = bc_Creature.Hits;
            int hitsMaxAdjusted = bc_Creature.HitsMax;

            int stamAdjusted = bc_Creature.Stam;
            int stamMaxAdjusted = bc_Creature.StamMax;

            int manaAdjusted = bc_Creature.Mana;
            int manaMaxAdjusted = bc_Creature.ManaMax;

            int minDamageAdjusted = bc_Creature.DamageMin;
            int maxDamageAdjusted = bc_Creature.DamageMax;

            double wrestlingAdjusted = bc_Creature.Skills.Wrestling.Base;
            double evalIntAdjusted = bc_Creature.Skills.EvalInt.Base;
            double mageryAdjusted = bc_Creature.Skills.Magery.Base;
            double meditationAdjusted = bc_Creature.Skills.Meditation.Base;
            double magicResistAdjusted = bc_Creature.Skills.MagicResist.Base;
            double poisoningAdjusted = bc_Creature.Skills.Poisoning.Base;

            int virtualArmorAdjusted = bc_Creature.VirtualArmor;

            int GreenTextHue = 0x3F;
            int RedTextHue = 0x22;

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

            string meditationTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMeditationCreationScalar - 1), 1);
            int meditationTamedColor = RedTextHue;
            if ((bc_Creature.TamedBaseMeditationCreationScalar - 1) >= 0)
            {
                meditationTamedScalar = "+" + meditationTamedScalar;
                meditationTamedColor = GreenTextHue;
            }
            if (meditationTamedScalar.Length == 3)
                meditationTamedScalar = meditationTamedScalar.Insert(2, ".0");

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

            string virtualArmorTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1), 1);
            int virtualArmorTamedColor = RedTextHue;
            if ((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1) >= 0)
            {
                virtualArmorTamedScalar = "+" + virtualArmorTamedScalar;
                virtualArmorTamedColor = GreenTextHue;
            }
            if (virtualArmorTamedScalar.Length == 3)
                virtualArmorTamedScalar = virtualArmorTamedScalar.Insert(2, ".0");

            if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
            {
                AddLabel(175, 60, specialTextHue, "Exp:");

                if (bc_Creature.MaxExperience > 0)
                    AddLabel(210, 60, specialTextHue, bc_Creature.Experience.ToString() + " / " + bc_Creature.MaxExperience.ToString());
                else
                    AddLabel(210, 60, specialTextHue, "-");

                AddLabel(170, 80, specialTextHue, "Kills:");
                AddLabel(210, 80, specialTextHue, bc_Creature.CreaturesKilled.ToString());

                if (bc_Creature.IsHenchman)
                {
                    if (bc_Creature.ResurrectionsRemaining >= 0)
                        AddLabel(160, 100, specialTextHue, "Resurrections Left: " + bc_Creature.ResurrectionsRemaining.ToString());
                }

                else
                {
                    AddLabel(155, 100, specialTextHue, "Bonded:");

                    if (bc_Creature.IsBonded)
                        AddLabel(210, 100, specialTextHue, "Yes");
                    else
                        AddLabel(210, 100, 5, "No");
                }
            }

            else
            {
                hitsAdjusted = (int)((double)bc_Creature.TamedBaseMaxHits * bc_Creature.TamedBaseMaxHitsCreationScalar);
                hitsMaxAdjusted = hitsAdjusted;

                stamAdjusted = (int)((double)bc_Creature.TamedBaseDex * bc_Creature.TamedBaseDexCreationScalar);
                stamMaxAdjusted = stamAdjusted;

                manaAdjusted = (int)((double)bc_Creature.TamedBaseMaxMana * bc_Creature.TamedBaseMaxManaCreationScalar);
                manaMaxAdjusted = manaAdjusted;                

                minDamageAdjusted = (int)((double)bc_Creature.TamedBaseMinDamage * bc_Creature.TamedBaseMinDamageCreationScalar);
                maxDamageAdjusted = (int)((double)bc_Creature.TamedBaseMaxDamage * bc_Creature.TamedBaseMaxDamageCreationScalar);

                wrestlingAdjusted = bc_Creature.TamedBaseWrestling * bc_Creature.TamedBaseWrestlingCreationScalar;
                evalIntAdjusted = bc_Creature.TamedBaseEvalInt * bc_Creature.TamedBaseEvalIntCreationScalar;
                mageryAdjusted = bc_Creature.TamedBaseMagery * bc_Creature.TamedBaseMageryCreationScalar;
                meditationAdjusted = bc_Creature.TamedBaseMeditation * bc_Creature.TamedBaseMeditationCreationScalar;
                magicResistAdjusted = bc_Creature.TamedBaseMagicResist * bc_Creature.TamedBaseMagicResistCreationScalar;
                poisoningAdjusted = bc_Creature.TamedBasePoisoning * bc_Creature.TamedBasePoisoningCreationScalar;

                virtualArmorAdjusted = (int)((double)bc_Creature.TamedBaseVirtualArmor * bc_Creature.TamedBaseVirtualArmorCreationScalar);
            }

            //---------------

            int labelX = 45;
            int valuesX = 140;
            int tamedScalarsX = 245;

            int startY = 125;

            int rowHeight = 18;
            int rowSpacer = 0;

            bool showTamedScalars = false;

            if (bc_Creature.Tamable)
            {
                showTamedScalars = true;

                if (bc_Creature.IsHenchman)
                    AddLabel(labelX, startY, textHue, "Min Begging:");
                else
                    AddLabel(labelX, startY, textHue, "Min Taming:");           

                AddLabel(valuesX, startY, textHue, bc_Creature.MinTameSkill.ToString());
                startY += rowHeight;

                AddLabel(labelX, startY, textHue, "Control Slots:");
                AddLabel(valuesX, startY, textHue, bc_Creature.ControlSlots.ToString());
                startY += rowHeight;
                startY += rowSpacer;
            }

            AddLabel(labelX, startY, textHue, "Hits:");
            AddLabel(valuesX, startY, textHue, hitsAdjusted + " / " + hitsMaxAdjusted);
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, hitsTamedColor, hitsTamedScalar);

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Stam:");
            AddLabel(valuesX, startY, textHue, stamAdjusted + " / " + stamMaxAdjusted);
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, stamTamedColor, stamTamedScalar);

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Mana:");
            AddLabel(valuesX, startY, textHue, manaAdjusted + " / " + manaMaxAdjusted);
            if (showTamedScalars && manaAdjusted > 0 && manaMaxAdjusted > 0)
                AddLabel(tamedScalarsX, startY, manaTamedColor, manaTamedScalar);

            startY += rowHeight;
            startY += rowSpacer;
            
            AddLabel(labelX, startY, textHue, "Damage:");
            AddLabel(valuesX, startY, textHue, minDamageAdjusted + " - " + maxDamageAdjusted);
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, damageTamedColor, damageTamedScalar);

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Ar:");
            AddLabel(valuesX, startY, textHue, virtualArmorAdjusted.ToString());
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, virtualArmorTamedColor, virtualArmorTamedScalar);

            startY += rowHeight;
            startY += rowSpacer;

            if (bc_Creature.IsHenchman)
                AddLabel(labelX, startY, textHue, "Combat Skill:");
            else
                AddLabel(labelX, startY, textHue, "Wrestling:");
            AddLabel(valuesX, startY, textHue, RoundToTenth(wrestlingAdjusted).ToString());
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, wrestlingTamedColor, wrestlingTamedScalar);

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Magery:");
            if (mageryAdjusted == 0)
                AddLabel(valuesX, startY, textHue, "-");
            else
            {
                AddLabel(valuesX, startY, textHue, RoundToTenth(mageryAdjusted).ToString());
                if (showTamedScalars)
                    AddLabel(tamedScalarsX, startY, mageryTamedColor, mageryTamedScalar);
            }

            startY += rowHeight;
            
            AddLabel(labelX, startY, textHue, "Eval Int:");
            if (evalIntAdjusted == 0)
                AddLabel(valuesX, startY, textHue, "-");
            else
            {
                AddLabel(valuesX, startY, textHue, RoundToTenth(evalIntAdjusted).ToString());
                if (showTamedScalars)
                    AddLabel(tamedScalarsX, startY, evalIntTamedColor, evalIntTamedScalar);
            }

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Meditation:");
            if (meditationAdjusted == 0)
                AddLabel(valuesX, startY, textHue, "-");
            else
            {
                AddLabel(valuesX, startY, textHue, RoundToTenth(meditationAdjusted).ToString());
                if (showTamedScalars)
                    AddLabel(tamedScalarsX, startY, meditationTamedColor, meditationTamedScalar);
            }

            startY += rowHeight;

            AddLabel(labelX, startY, textHue, "Resist:");
            AddLabel(valuesX, startY, textHue, RoundToTenth(magicResistAdjusted).ToString());
            if (showTamedScalars)
                AddLabel(tamedScalarsX, startY, magicResistTamedColor, magicResistTamedScalar);

            startY += rowHeight;
            
            AddLabel(labelX, startY, textHue, "Poison:");
            if (bc_Creature.HitPoison != null)
            {
                AddLabel(labelX + 45, startY, textHue, bc_Creature.HitPoison.Name + " (" + RoundToTenth(poisoningAdjusted).ToString() + "% Chance)");
                if (showTamedScalars)
                    AddLabel(tamedScalarsX, startY, poisoningTamedColor, poisoningTamedScalar);
            }

            else
                AddLabel(valuesX, startY, textHue, "-");           

            startY += rowHeight;

            //Aggro
            if (bc_Creature.AggroBonus > 0 && bc_Creature.AggroBonusExpiration > DateTime.UtcNow)
            {
                AddLabel(45, 370, specialTextHue, "Aggro Bonus: " + bc_Creature.AggroBonus.ToString());
                AddLabel(160, 370, textHue, "Expires in: " + Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Creature.AggroBonusExpiration, true, false, false, true, true));
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