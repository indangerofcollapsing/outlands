using System;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class WispLantern : Lantern
	{
        private PetWisp m_Wisp;
        private string m_WispName;
		private int m_WispHue;

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
        public PetWisp LinkedPet
        {
            get { return m_Wisp; }
        }

        public override string DefaultName {  get { return "lantern of wisps"; } }

		[Constructable]
        public WispLantern()
		{
			Hue = 1154;
            ItemID = 0xA22;
			m_WispHue = 0;
            LootType = Server.LootType.Blessed;
		}

        public WispLantern(Serial serial)
            : base(serial)
		{
		}

		public void SetWispName(string name)
		{
			m_WispName = name;
		}

		public void SetWispHue(int hue)
		{
			m_WispHue = hue;
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 );

			//version 3
			writer.Write(m_WispHue);

            //version 2
            writer.Write(m_WispName);

            //version 1

            writer.Write(m_Wisp);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
				case 3:
					{
						m_WispHue = reader.ReadInt();
						goto case 2;
					}
                case 2:
                    {
                        m_WispName = reader.ReadString();
                        goto case 1;
                    }

                case 1:
                    {
                        m_Wisp = (PetWisp)reader.ReadMobile();
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
                    CageWisp(from);
                    Douse();
                }
            }
            else if (SpellHelper.CheckCombat(from, true))
            {
                from.SendMessage("The wisp refuses to come out in the heat of battle.");
            }
            else
            {
                ConjureWisp(from); 
                Ignite();
            }
        }

        public void ConjureWisp(Mobile from)
        {
            if (m_Wisp != null && !m_Wisp.Deleted)
                return;

            m_Wisp = new PetWisp();

            if (m_WispName != null && m_WispName.Length > 0)
                m_Wisp.Name = m_WispName;

            m_Wisp.MoveToWorld(from.Location, from.Map);
            m_Wisp.Controlled = true;
            m_Wisp.ControlMaster = from;
            m_Wisp.ControlOrder = OrderType.Follow;
            m_Wisp.ControlTarget = from;
			m_Wisp.Hue = m_WispHue;
            from.PlaySound(0x1d4);
            Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
            {
                CageWisp(from);
                Douse();
            });
        }

        public void CageWisp(Mobile from)
        {
            if (m_Wisp != null && !m_Wisp.Deleted)
            {
                m_WispName = m_Wisp.Name;
                m_Wisp.Delete();
                m_Wisp = null;

                from.PlaySound(0x1d6);
            }
        }
	}

    public class PetWisp : Wisp
    {
        public PetWisp()
        {
            Blessed = true;
            MinTameSkill = 0;
            BardImmune = true;
        }

        public override int MaxExperience { get { return 0; } }

        public PetWisp(Serial serial)
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
