using System;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.SkillHandlers
{
    public class Stealth
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Stealth].Callback = new SkillUseCallback(OnUse);
        }

        public static double HidingRequirement { get { return 80; } }

        public static TimeSpan OnUse(Mobile mobile)
        {
            PlayerMobile pm = mobile as PlayerMobile;

            if (pm != null)
            {
                if (pm.TrueHidden == false)
                {
                    pm.SendMessage("You must hide first using the Hiding skill.");
                    pm.RevealingAction();

                    return TimeSpan.FromSeconds(SkillCooldown.StealthCooldown);
                }
            }

            if (!mobile.Hidden)
                mobile.SendLocalizedMessage(502725); // You must hide first   

            else if (mobile.Skills[SkillName.Hiding].Base < HidingRequirement)
            {
                mobile.SendLocalizedMessage(502726); // You are not hidden well enough.  Become better at hiding.
                mobile.RevealingAction();
            }

            else if (!mobile.CanBeginAction(typeof(Stealth)))
                mobile.RevealingAction();

            else
            {
                if (mobile.CheckSkill(SkillName.Stealth, 0, 100, 1.0))
                {
                    int steps = (int)(mobile.Skills[SkillName.Stealth].Value / 10);

                    BaseArmor gorget = mobile.NeckArmor as BaseArmor;
                    BaseArmor gloves = mobile.HandArmor as BaseArmor;
                    BaseArmor arms = mobile.ArmsArmor as BaseArmor;
                    BaseArmor head = mobile.HeadArmor as BaseArmor;
                    BaseArmor legs = mobile.LegsArmor as BaseArmor;
                    BaseArmor chest = mobile.ChestArmor as BaseArmor;

                    List<BaseArmor> equipment = new List<BaseArmor>();

                    equipment.Add(gorget);
                    equipment.Add(gloves);
                    equipment.Add(arms);
                    equipment.Add(head);
                    equipment.Add(legs);
                    equipment.Add(chest);

                    //Each Piece of Armor that Has a Meditation Penalty Reduces Total Stealth Steps By 1
                    foreach (BaseArmor armor in equipment)
                    {
                        if (armor != null)
                        {
                            if (armor.OldMedAllowance == ArmorMeditationAllowance.None)
                                steps--;
                        }
                    }

                    DungeonArmor.PlayerDungeonArmorProfile stealtherDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(mobile, null);

                    if (stealtherDungeonArmor.MatchingSet && !stealtherDungeonArmor.InPlayerCombat)                    
                        steps += 6 + stealtherDungeonArmor.DungeonArmorDetail.BonusStealthSteps;                    

                    if (steps < 1)
                        steps = 1;

                    mobile.AllowedStealthSteps = steps;

                    if (pm != null)
                    {
                        pm.IsStealthing = true;

                        if (pm.Skills[SkillName.Hiding].Value >= 80 && pm.Skills[SkillName.Stealth].Value >= 80)
                            mobile.StealthAttackReady = true;
                    }

                    mobile.SendMessage("You begin to move quietly.");                    

                    mobile.BeginAction((typeof(Hiding)));
                    Timer.DelayCall(TimeSpan.FromSeconds(SkillCooldown.StealthCooldown - .1), delegate { mobile.EndAction(typeof(Hiding)); });

                    mobile.BeginAction((typeof(Stealth)));
                    Timer.DelayCall(TimeSpan.FromSeconds(SkillCooldown.StealthCooldown - .1), delegate { mobile.EndAction(typeof(Stealth)); });

                    mobile.m_StealthMovementTimer = null;
                    mobile.m_StealthMovementTimer = new Mobile.StealthMovementTimer(mobile);
                    mobile.m_StealthMovementTimer.Start();

                    mobile.m_HidingTimer = null;
                    mobile.m_HidingTimer = new Mobile.HidingTimer(mobile, DateTime.UtcNow, true);
                    mobile.m_HidingTimer.Start();

                    return TimeSpan.FromSeconds(SkillCooldown.StealthCooldown);
                }

                else
                {
                    mobile.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
                    mobile.RevealingAction();
                }
            }

            return TimeSpan.FromSeconds(SkillCooldown.StealthCooldown);
        }
    }
}