using System;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class RetweetyBirdLantern : Lantern
    {
        private PetRetweetyBird m_RetweetyBird;
        private string m_RetweetyBirdName;
        private int m_RetweetyBirdHue;

        public override int UnlitItemID
        {
            get
            {
                if (ItemID == 0xA15 || ItemID == 0xA17)
                    return ItemID;

                return 0xA22;
            }
        }

        public override int LitItemID
        {
            get
            {
                if (ItemID == 0xA18)
                    return ItemID;

                return 0xA25;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PetRetweetyBird LinkedPet
        {
            get { return m_RetweetyBird; }
        }

        public override string DefaultName { get { return "lantern of retweety birds"; } }

        [Constructable]
        public RetweetyBirdLantern()
        {
            Hue = 94;
            ItemID = 0xA22;
            m_RetweetyBirdHue = 0;
            LootType = Server.LootType.Blessed;
            SetRetweetyBirdName("a retweety bird");
            SetRetweetyBirdHue(94);
        }

        public RetweetyBirdLantern(Serial serial)
            : base(serial)
        {
        }

        public void SetRetweetyBirdName(string name)
        {
            m_RetweetyBirdName = name;
        }

        public void SetRetweetyBirdHue(int hue)
        {
            m_RetweetyBirdHue = hue;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            //version 3
            writer.Write(m_RetweetyBirdHue);

            //version 2
            writer.Write(m_RetweetyBirdName);

            //version 1

            writer.Write(m_RetweetyBird);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        m_RetweetyBirdHue = reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_RetweetyBirdName = reader.ReadString();
                        goto case 1;
                    }

                case 1:
                    {
                        m_RetweetyBird = (PetRetweetyBird)reader.ReadMobile();
                        break;
                    }

            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (BurntOut)
                return;

            if (Protected && from.AccessLevel == AccessLevel.Player)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            if (Burning)
            {
                if (UnlitItemID != 0)
                {
                    CageRetweetyBird(from);
                    Douse();
                }
            }
            else if (SpellHelper.CheckCombat(from, true))
            {
                from.SendMessage("The retweety bird refuses to come out in the heat of battle.");
            }
            else
            {
                ConjureRetweetyBird(from);
                Ignite();
            }
        }

        public void ConjureRetweetyBird(Mobile from)
        {
            if (m_RetweetyBird != null && !m_RetweetyBird.Deleted)
                return;

            m_RetweetyBird = new PetRetweetyBird();

            if (m_RetweetyBirdName != null && m_RetweetyBirdName.Length > 0)
                m_RetweetyBird.Name = m_RetweetyBirdName;

            m_RetweetyBird.MoveToWorld(from.Location, from.Map);
            m_RetweetyBird.Controlled = true;
            m_RetweetyBird.ControlMaster = from;
            m_RetweetyBird.ControlOrder = OrderType.Follow;
            m_RetweetyBird.ControlTarget = from;
            m_RetweetyBird.Hue = m_RetweetyBirdHue;
            from.PlaySound(0x1d4);
            Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
            {
                CageRetweetyBird(from);
                Douse();
            });
        }

        public void CageRetweetyBird(Mobile from)
        {
            if (m_RetweetyBird != null && !m_RetweetyBird.Deleted)
            {
                m_RetweetyBirdName = m_RetweetyBird.Name;
                m_RetweetyBird.Delete();
                m_RetweetyBird = null;

                from.PlaySound(0x1d6);
            }
        }
    }

    public class PetRetweetyBird : Bird
    {
        public PetRetweetyBird()
        {
            Blessed = true;
            MinTameSkill = 0;
            BardImmune = true;
            Name = "a retweety bird";
            Hue = 94;
        }

        public override int MaxExperience { get { return 0; } }

        public PetRetweetyBird(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
        }
    }
}
