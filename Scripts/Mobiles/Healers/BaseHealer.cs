using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Regions;

namespace Server.Mobiles
{
    public abstract class BaseHealer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override bool IsActiveVendor { get { return false; } }
        public override bool IsInvulnerable { get { return false; } }

        public override void InitSBInfo()
        {
        }

        public BaseHealer(): base(null)
        {
            if (!IsInvulnerable)
            {
                AI = AIType.AI_Mage;
                ActiveSpeed = 0.2;
                PassiveSpeed = 0.8;
                RangePerception = BaseCreature.DefaultRangePerception;
                FightMode = FightMode.Aggressor;
            }

            SpeechHue = 0;

            SetStr(75);
            SetDex(75);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.Wrestling, 100);            
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = 10000;

            PackItem(new Bandage(Utility.RandomMinMax(5, 10)));
            PackItem(new HealPotion());
            PackItem(new CurePotion());
        }

        public override VendorShoeType ShoeType { get { return VendorShoeType.None; } }

        public virtual int GetRobeColor()
        {
            return Utility.RandomYellowHue();
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Robe(GetRobeColor()));
        }

        public virtual bool HealsYoungPlayers { get { return true; } }

        public virtual bool CheckResurrect(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;

            if (player == null)
                return false;

            if (player.Criminal)
            {
                Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }

            else if (player.ShortTermMurders >= 5)
            {
                Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
                return false;
            }

            else if (player.RestitutionFee > 0 || player.MurdererDeathGumpNeeded)
            {
                Say("Thou has not paid sufficiently for your crimes and I shall not ressurect thee.");
                return false;
            }

            else if (player.Karma < 0)
            {
                Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.
                return true;
            }

            return true;
        }

        private DateTime m_NextResurrect;
        private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);

        public virtual void OfferResurrection(Mobile m)
        {
            Direction = GetDirectionTo(m);

            m.PlaySound(0x1F2);
            m.FixedEffect(0x376A, 10, 16);

            m.CloseGump(typeof(ResurrectGump));
            m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
        }

        public virtual void OfferHeal(PlayerMobile m)
        {
            Direction = GetDirectionTo(m);

            if (m.CheckYoungHealTime())
            {
                Say(501229); // You look like you need some healing my child.

                m.PlaySound(0x1F2);
                m.FixedEffect(0x376A, 9, 32);

                m.Hits = m.HitsMax;
            }

            else
            {
                Say(501228); // I can do no more for you at this time.
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Frozen && DateTime.UtcNow >= m_NextResurrect && InRange(m, 4) && !InRange(oldLocation, 4) && InLOS(m))
            {
                if (!m.Frozen && !m.Alive && DateTime.UtcNow >= m_NextResurrect && InRange(m, 4) && !InRange(oldLocation, 4) && InLOS(m) && !(m.Region is HouseRegion) && !(m.Z <= this.Z - 5 || m.Z >= this.Z + 5)) // HouseRegion and Z level fix (with +/- 5 grace area.
                {
                    m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

                    if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                    {
                        m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    }

                    else if (CheckResurrect(m))
                    {
                        OfferResurrection(m);
                    }
                }

                else if (this.HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young)
                {
                    //OfferHeal((PlayerMobile)m);
                }
            }
        }

        public BaseHealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (!IsInvulnerable)
            {
                AI = AIType.AI_Mage;
                ActiveSpeed = 0.2;
                PassiveSpeed = 0.8;
                RangePerception = BaseCreature.DefaultRangePerception;
                FightMode = FightMode.Aggressor;
            }
        }
    }
}