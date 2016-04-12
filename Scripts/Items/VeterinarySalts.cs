using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Custom.Items
{
	public class VeterinarySalts : Item
	{
        private int m_Charges = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; } }

		[Constructable]
		public VeterinarySalts(): base(0x0E26)
		{
            Name = "veterinary salts";
			Hue = 2101;
            Weight = 1.0;
		}        

		public VeterinarySalts(Serial serial): base(serial)
		{
		}		

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "veterinary salts");
            LabelTo(from, "charges: " + Charges.ToString());           
        }        

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))	
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.			
			else
			{
				from.SendMessage("Target the deceased creature you wish resurrect or the bottle of veterinary salts you wish to combine this with.");
				from.Target = new PetResurrectTarget(this);
			}
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();

            //--------

            if (Weight < 1.0)
                Weight = 1.0;
        }
	}    

	public class PetResurrectTarget : Target
	{
		private VeterinarySalts m_Salts;

		public PetResurrectTarget(VeterinarySalts salts): base(5, false, TargetFlags.None)
		{
            m_Salts = salts;
		}

		protected override void OnTarget(Mobile from, object target)
		{
			if (m_Salts.Deleted || m_Salts.RootParent != from)
				return;

			PlayerMobile pm = from as PlayerMobile;

			if (pm == null)
				return;

            VeterinarySalts targetSalts = target as VeterinarySalts;
            
            if (targetSalts != null)
            {
                if (targetSalts.Charges >= 5)
                {
                    from.SendMessage("That bottle of veterinary salts is full.");

                    return;
                }

                int maxChargesToAdd = 5 - targetSalts.Charges;
                int chargesAdded = m_Salts.Charges;

                if (chargesAdded > maxChargesToAdd)
                    chargesAdded = maxChargesToAdd;

                targetSalts.Charges += chargesAdded;

                if (m_Salts.Charges - chargesAdded <= 0)
                    m_Salts.Delete();

                else
                    m_Salts.Charges -= chargesAdded;

                from.SendMessage("You combine the veterinary salts.");
                from.SendSound(0x5AF);

                return;
            }

			BaseCreature deadFollower = target as BaseCreature;

			if (deadFollower == null || !deadFollower.IsDeadPet)
			{
				from.SendMessage("That is not a deceased creature.");
				return;
			}

                /*
			else if (!deadFollower.CanBeResurrectedThroughVeterinary && deadFollower.Controlled && deadFollower.ControlMaster is PlayerMobile)
			{
                if (m_Salts.Charges >= deadFollower.ControlSlots)
                {
                    //Creature That Has Limited Resurrections And Somehow Hasn't Been Deleted
                    if (deadFollower.ResurrectionsRemaining == 0)
                    {
                        from.SendMessage("That follower is no longer resurrectable.");
                        return;
                    }

                    from.SendMessage("The creature springs to life.");

                    from.CloseGump(typeof(PetResurrectGump));
                    from.SendGump(new PetResurrectGump(from, deadFollower, m_Salts));
                }

                else                
                    from.SendMessage("There are insufficent charges remaining to resurrect that.");                
			}
                 * */

			//else			
				from.SendMessage("That creature cannot be resurrected with this item.");			
		}
	}

	public class RareBerries : Item
	{
		[Constructable]
		public RareBerries() : base(0x0D1A) { Weight = 1.0; Hue = 0x22;  }
		public RareBerries(Serial serial) : base(serial) { }

		public override string DefaultName { get { return "rare berries"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); //version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}
}
