using System;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class CatLantern : Lantern
    {
        private PetCat m_Cat;
        private string m_CatName;
        private int m_CatHue;

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
        public PetCat LinkedPet
        {
            get { return m_Cat; }
        }

        public override string DefaultName { get { return "lantern of cats"; } }

        [Constructable]
        public CatLantern() : this(1154, 0)
        {

        }

        [Constructable]
        public CatLantern(int lanternHue, int catHue)
        {
            Hue = lanternHue;
            ItemID = 0xA22;
            m_CatHue = catHue;
            LootType = Server.LootType.Blessed;
        }

        public CatLantern(Serial serial)
            : base(serial)
        {
        }

        public void SetCatName(string name)
        {
            m_CatName = name;
        }

        public void SetCatHue(int hue)
        {
            m_CatHue = hue;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            //version 3
            writer.Write(m_CatHue);

            //version 2
            writer.Write(m_CatName);

            //version 1

            writer.Write(m_Cat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        m_CatHue = reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_CatName = reader.ReadString();
                        goto case 1;
                    }

                case 1:
                    {
                        m_Cat = (PetCat)reader.ReadMobile();
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
                    CageCat(from);
                    Douse();
                }
            }
            else if (SpellHelper.CheckCombat(from, true))
            {
                from.SendMessage("The cat refuses to come out in the heat of battle.");
            }
            else
            {
                ConjureCat(from);
                Ignite();
            }
        }

        public void ConjureCat(Mobile from)
        {
            if (m_Cat != null && !m_Cat.Deleted)
                return;

            m_Cat = new PetCat();

            if (m_CatName != null && m_CatName.Length > 0)
                m_Cat.Name = m_CatName;

            m_Cat.MoveToWorld(from.Location, from.Map);
            m_Cat.Controlled = true;
            m_Cat.ControlMaster = from;
            m_Cat.ControlOrder = OrderType.Follow;
            m_Cat.ControlTarget = from;
            m_Cat.Hue = m_CatHue;
            from.PlaySound(0x1d4);
            Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
            {
                CageCat(from);
                Douse();
            });
        }

        public void CageCat(Mobile from)
        {
            if (m_Cat != null && !m_Cat.Deleted)
            {
                m_CatName = m_Cat.Name;
                m_Cat.Delete();
                m_Cat = null;

                from.PlaySound(0x1d6);
            }
        }
    }

    public class PetCat : Cat
    {
        public PetCat()
        {
            Blessed = true;
            MinTameSkill = 0;
            BardImmune = true;
        }
        
        public PetCat(Serial serial)
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
