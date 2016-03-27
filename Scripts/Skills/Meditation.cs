using System;
using Server.Items;

namespace Server.SkillHandlers
{
    class Meditation
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Meditation].Callback = new SkillUseCallback(OnUse);
        }

        public static bool CheckOkayHolding(Item item)
        {
            if (item == null)
                return true;

            if (item is Spellbook || item is Runebook)
                return true;

            return false;
        }

        public static TimeSpan OnUse(Mobile from)
        {
            from.RevealingAction();

            if (from.Target != null)
            {
                from.SendLocalizedMessage(501845); // You are busy doing something else and cannot focus.

                return TimeSpan.FromSeconds(SkillCooldown.MeditationInvalidCooldown);
            }

            else if (from.Mana >= from.ManaMax)
            {
                from.SendLocalizedMessage(501846); // You are at peace.

                return TimeSpan.FromSeconds(SkillCooldown.MeditationInvalidCooldown);
            }

            else
            {
                Item oneHanded = from.FindItemOnLayer(Layer.OneHanded);
                Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                if (!CheckOkayHolding(oneHanded) || !CheckOkayHolding(twoHanded))
                {
                    from.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate.

                    return TimeSpan.FromSeconds(2.5);
                }

                if (from.CheckSkill(SkillName.Meditation, 0, 100, 1.0))
                {
                    from.SendLocalizedMessage(501851); // You enter a meditative trance.
                    from.Meditating = true;

                    if (from.Player || from.Body.IsHuman)
                        from.PlaySound(0xF9);
                }

                else                
                    from.SendLocalizedMessage(501850); // You cannot focus your concentration.                

                return TimeSpan.FromSeconds(SkillCooldown.MeditationValidCooldown);
            }
        }
    }
}