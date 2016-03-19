using System;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class BlackCatCollar : GoldNecklace
	{
        private BaseCreature m_Cat;
        private string m_CatName;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature LinkedPet
        {
            get { return m_Cat; }
        }

        public override string DefaultName {  get { return "black cat collar"; } }

		[Constructable]
        public BlackCatCollar()
		{
            Hue = 2106;
            LootType = Server.LootType.Blessed;
		}

        public BlackCatCollar(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

            writer.Write(m_CatName);
            writer.Write(m_Cat);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_CatName = reader.ReadString();
                        m_Cat = reader.ReadMobile() as BaseCreature;
                        break;
                    }

            }                
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack) && !IsChildOf(from))
            {
                from.SendMessage("You can not access that.");
            }
            else if (LinkedPet != null && !LinkedPet.Deleted)
            {
                CageCat(from);
            }
            else if (SpellHelper.CheckCombat(from, true))
            {
                from.SendMessage("The cat refuses to come out in the heat of battle.");
            }
            else
            {
                ConjureCat(from); 
            }
        }

        public void ConjureCat(Mobile from)
        {
            if (m_Cat != null && !m_Cat.Deleted)
                return;

            m_Cat = new BlackCat();

            if (m_CatName != null && m_CatName.Length > 0)
                m_Cat.Name = m_CatName;

            m_Cat.MoveToWorld(from.Location, from.Map);
            m_Cat.Controlled = true;
            m_Cat.ControlMaster = from;
            m_Cat.ControlOrder = OrderType.Follow;
            m_Cat.ControlTarget = from;
            m_Cat.Blessed = true;
	        m_Cat.Hue = 2106;
            from.PlaySound(0x069);
        }

        public void CageCat(Mobile from)
        {
            if (m_Cat != null && !m_Cat.Deleted)
            {
                m_CatName = m_Cat.Name;
                m_Cat.Delete();
                m_Cat = null;

                from.PlaySound(0x06D);
            }
        }
	}

    public class BlackCat : Cat
    {
        public BlackCat()
        {
        }

        public BlackCat(Serial serial)
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
