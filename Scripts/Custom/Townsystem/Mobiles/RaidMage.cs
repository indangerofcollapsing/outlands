using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Custom.Townsystem;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;


namespace Server.Custom.Townsystem
{

    public class RaidMage : Mobile
    {
        private OrderDragon m_Dragon;
        private DateTime m_lastCastHeal;
        private TimeSpan m_CastInterval = TimeSpan.FromSeconds(6);
        private TimeSpan m_LifeTime = TimeSpan.FromSeconds(60);
        private DateTime m_End;
        private Timer m_Timer;
        

        [Constructable]
        public RaidMage(OrderDragon dragon)
        {
            m_Dragon = dragon;

            InitStats(30, 50, 300);

            Name = "an Order Mage";
            Hue = Utility.RandomSkinHue();

            if (!Server.Core.AOS)
                NameHue = 0x35;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
            }
            else
            {
                this.Body = 0x190;
            }

            AddItem(Immovable(Rehued(new Robe(), 1310)));
            Body = Race.Human.GhostBody(this);
            m_Timer = Timer.DelayCall(TimeSpan.FromMilliseconds(Utility.Random(200,1200)), TimeSpan.FromMilliseconds(300), new TimerCallback(Slice));
			m_End = DateTime.Now + m_LifeTime;
            Warmode = true;
            
        }

        public Item Immovable(Item item)
        {
            item.Movable = false;
            return item;
        }
        public Item Rehued(Item item, int hue)
        {
            item.Hue = hue;
            return item;
        }

        public RaidMage(Serial serial)
            : base(serial)
        {
        }

        private void Slice()
        {
			if (m_Dragon == null || m_Dragon.Deleted || !InRange(m_Dragon.Location, 10) || DateTime.Now > m_End || m_Dragon.Hits >= m_Dragon.HitsMax * 0.5)
            {
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }
                Delete();
                return;
            }

			if (DateTime.Now > m_lastCastHeal + m_CastInterval)
            {
                SpellHelper.Turn(this, m_Dragon);
                Animate(204, 7, 1, true, false, 0);
                SpellHelper.Heal(Utility.Random(35,20), m_Dragon, this);
                m_Dragon.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                m_Dragon.PlaySound(0x202);
				m_lastCastHeal = DateTime.Now - TimeSpan.FromSeconds(4 * Utility.RandomDouble() - 2);
            }
        }


        public override bool  OnBeforeDeath()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
            return base.OnBeforeDeath();
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
