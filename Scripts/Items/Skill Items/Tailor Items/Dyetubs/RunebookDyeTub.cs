using System;
using Server;
using Server.Targeting;
using Server.Gumps;
using Server.HuePickers;
using System.Collections.Generic;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Items
{
	public class RunebookDyeTub : Item
	{
        public virtual bool Redyable { get { return true; } }
        public virtual int PermanentColor { get { return -1; } }
        
        public virtual CustomHuePicker CustomHuePicker { get { return null; } }

        private int m_UsesRemaining = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; }
        }

        private int m_DyeColor = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DyeColor
        {
            get{ return m_DyeColor; }
            set
            {
                m_DyeColor = value; 
                Hue = value;                
            }
        }

        [Constructable]
        public RunebookDyeTub(): base(0x0FAB)
        {
            Name = "a runebook dye tub";

            if (PermanentColor != -1)
                DyeColor = PermanentColor;
            else
                DyeColor = 1121; // default runebook color

            Weight = 10;
        }

        public RunebookDyeTub(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_UsesRemaining > -1)
                LabelTo(from, String.Format("[Uses Remaining: {0}]", m_UsesRemaining));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Which runebook do you wish to dye?");
                from.Target = new InternalTarget(this);
            }

            else            
                from.SendLocalizedMessage(500446); // That is too far away.            
        }

        private class InternalTarget : Target
        {
            private RunebookDyeTub m_RunebookDyeTub;

            public InternalTarget(RunebookDyeTub runebookDyeTub): base(1, false, TargetFlags.None)
            {
                m_RunebookDyeTub = runebookDyeTub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_RunebookDyeTub == null || m_RunebookDyeTub.Deleted)
                    return;

                if (targeted is Item)
                {
                    Item item = (Item)targeted;

                    if (item is Runebook)
                    {
                        if (!from.InRange(m_RunebookDyeTub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            from.SendMessage("That is too far away.");

                        else if (item.Parent is Mobile)
                            from.SendMessage("You can't dye runebooks that are currently equipped.");

                        else if (!item.Movable)
                            from.SendMessage("You cannot dye runebooks that are locked down.");

                        else
                        {
                            m_RunebookDyeTub.UseCharge(from);
                            item.Hue = m_RunebookDyeTub.m_DyeColor;
                            from.PlaySound(0x23E);
                        }
                    }
                }

                else                
                    from.SendMessage("That is not a runebook.");                
            }
        }

        public void UseCharge(Mobile from)
        {
            if (UsesRemaining == -1)
                return;

            else if (--UsesRemaining <= 0)
            {
                Delete();

                if (from != null)
                    from.SendMessage("You use the last of the remaining dye in the dyetub.");
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_UsesRemaining);
            writer.Write(m_DyeColor);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_UsesRemaining = reader.ReadInt();
                m_DyeColor = reader.ReadInt();
            }
		}
	}
}