using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Misc;
using Server.Network;

namespace Server.Gumps
{
    public class RestedStateGump : Gump
    {
        public static string TimeSince(DateTime time, DateTime now)
        {
            var diff = now - time;

            int years = (int)(diff.TotalDays / 365);
            int months = (int)(now.Month - time.Month);
            if (time.Day > now.Day)
                months--;
            if (months < 0)
                months += 12;
            int days = (int)(now - time.AddYears(years).AddMonths(months)).TotalDays;
            int hours = (int)(diff - TimeSpan.FromDays(days)).TotalHours;
            int minutes = (int)(diff - TimeSpan.FromDays(days) - TimeSpan.FromHours(hours)).TotalMinutes;
            int seconds = (int)(diff - TimeSpan.FromMinutes(days * 60 * 24 + hours * 60 + minutes)).TotalSeconds;
            int count = 0;
            string ret = "";

            if (years > 0)
            {
                ret = String.Concat(ret, " ", years, " year", years == 1 ? " " : "s ");
                count++;
            }

            if (months > 0)
            {
                ret = String.Concat(ret, " ", months, " month", months == 1 ? " " : "s ");
                count++;
            }

            if (count < 2 && days > 0)
            {
                ret = String.Concat(ret, " ", days, " day", days == 1 ? " " : "s ");
                if (count++ == 2)
                    return ret;
            }

            if (count < 2 && hours > 0)
            {
                ret = String.Concat(ret, " ", hours, " hour", hours == 1 ? " " : "s ");
                if (count++ == 2)
                    return ret;
            }

            if (count < 2 && minutes > 0)
            {
                ret = String.Concat(ret, " ", minutes, " minute", minutes == 1 ? " " : "s ");
                if (count++ == 2)
                    return ret;
            }

            if (count < 1 && seconds > 0)
            {
                ret = String.Concat(ret, " ", seconds, " second", seconds == 1 ? " " : "s ");
                if (count++ == 2)
                    return ret;
            }

            return ret;
        }

        public RestedStateGump(PlayerMobile pm)
            : base(250, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            this.AddBackground(0, 31, 250, 200, 0x2436);
            this.AddImage(220, 76, 10410); //Dragon

            //AddImage(100, 100, 2080);
            //AddImage(118, 137, 2081);
            //AddImage(118, 207, 2081);
            //AddImage(118, 277, 2081);
            //AddImage(118, 347, 2083);
            //AddHtml(147, 108, 210, 18, String.Format("<center><i>{0}</i></center>", pm.RawName.ToString()), false, false);
            //AddImage(140, 138, 2091);
            // AddImage(140, 335, 2091);

            //Boosted
            string stateString;

            const int buttonX = 110;
            const int buttonY = 182;
            if (SkillCheck.InPowerHour(pm))
            {
                AddButton(buttonX, buttonY, 4020, 4022, 0, GumpButtonType.Reply, 0);
                var left = pm.PowerHourReset.Add(pm.PowerHourDuration).Subtract(DateTime.UtcNow);
                stateString = String.Format("You will be boosted in your skills training for {0:0} more minutes.", left.TotalMinutes);
            }
            else
            {
                if (DateTime.UtcNow > pm.PowerHourReset.Add(SkillCheck.PowerHourResetTime))
                {
                    AddButton(buttonX - 30, buttonY, 4023, 4025, 1, GumpButtonType.Reply, 0);
                    AddButton(buttonX + 30, buttonY, 4020, 4022, 0, GumpButtonType.Reply, 0);
                    stateString = "You can now trigger your powerhour to boost your skills training.";
                }
                else
                {
                    AddButton(buttonX, buttonY, 4020, 4022, 0, GumpButtonType.Reply, 0);
                    stateString = "You will be able to benefit from boosted skills training in " + TimeSince(DateTime.UtcNow, pm.PowerHourReset.Add(SkillCheck.PowerHourResetTime));
                }
            }

            AddHtml(50, 68, 160, 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 0xFFFF00, stateString), false, false); // Skill Name
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile as PlayerMobile;

            switch (info.ButtonID)
            {
                case 0:
                    {

                        break;
                    }

                case 1:
                    {
                        if (DateTime.UtcNow > from.PowerHourReset.Add(SkillCheck.PowerHourResetTime))
                        {
                            from.PowerHourReset = DateTime.UtcNow;
                            from.BoostPowerHourDuration();
                            from.SendMessage("You sense a sudden boost of energy begins enhancing your training.");
                        }
                        break;
                    }

            }
        }
    }
}