using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class BaseMasteryOrb : Item
    {
        public enum MasteryOrbType
        {
            Bronze,
            Silver,
            Gold
        }

        public int BronzeSkillCapIncrease = 10;
        public int SilverSkillCapIncrease = 20;
        public int GoldSkillCapIncrease = 30;

        private MasteryOrbType m_OrbType = MasteryOrbType.Bronze;
        [CommandProperty(AccessLevel.GameMaster)]
        public MasteryOrbType OrbType
        {
            get { return m_OrbType; }
            set
            {
                m_OrbType = value;

                switch (m_OrbType)
                {
                    case MasteryOrbType.Bronze: Hue = 2428; break;
                    case MasteryOrbType.Silver: Hue = 2409; break;
                    case MasteryOrbType.Gold: Hue = 2213; break;
                }
            }
        }

        [Constructable]
        public BaseMasteryOrb(): base(0x26BC)
        {
            Name = "a mastery orb";            
        }

        public BaseMasteryOrb(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            string label = "a mastery orb";

            switch (m_OrbType)
            {
                case MasteryOrbType.Bronze: label = "a bronze mastery orb"; break;
                case MasteryOrbType.Silver: label = "a silver mastery orb"; break;
                case MasteryOrbType.Gold: label = "a gold mastery orb"; break;
            }

            LabelTo(from, label);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (player.BonusSkillCap == PlayerMobile.MaxBonusSkillCap)
            {
                from.SendMessage("You are currently at the maximum skill cap and cannot use this.");
                return;
            }

            int skillCapIncrease = 0;

            switch (m_OrbType)
            {
                case MasteryOrbType.Bronze: skillCapIncrease = BronzeSkillCapIncrease; break;
                case MasteryOrbType.Silver: skillCapIncrease = SilverSkillCapIncrease; break;
                case MasteryOrbType.Gold: skillCapIncrease = GoldSkillCapIncrease; break;
            }

            int amountIncreased = 0;

            if (player.BonusSkillCap + skillCapIncrease > PlayerMobile.MaxBonusSkillCap)
                amountIncreased = PlayerMobile.MaxBonusSkillCap - player.BonusSkillCap;

            player.BonusSkillCap += amountIncreased;

            string strAmountIncreased = ((double)amountIncreased / 10).ToString();
            string currentSkillCap = ((double)player.SkillsCap / 10).ToString();

            player.SendMessage("You increase your skillcap by " + strAmountIncreased + ". It is now " + currentSkillCap + ".");

            player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
            player.PlaySound(0x3BD);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write((int)m_OrbType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_OrbType = (MasteryOrbType)reader.ReadInt();
            }
        }
    }
}