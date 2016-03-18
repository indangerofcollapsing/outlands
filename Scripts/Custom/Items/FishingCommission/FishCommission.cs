using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{    
    public class FishCommissionDeed : Item
    {
        private int m_FishAdded;
        [CommandProperty(AccessLevel.GameMaster)]
        public int FishAdded
        {
            get { return m_FishAdded; }
            set { m_FishAdded = value; }
        }

        public virtual int FishRequired { get { return 1000; } }

        [Constructable]
        public FishCommissionDeed(): base(0x14F0)
		{
            Name = "a fish commission deed";
            Hue = 2656;

            Weight = 0.1;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "[" + m_FishAdded.ToString() + " fish out of " + FishRequired.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
                from.SendMessage("That item must be in your pack in order to use it.");
            else
            {
                from.SendMessage("Select which fish to add to the fish commission deed.");
                from.Target = new AddFishTarget(this);
            }

            return;
        }

        public class AddFishTarget : Target
        {
            private FishCommissionDeed m_FishCommissionDeed;

            public AddFishTarget(FishCommissionDeed fishCommissionDeed): base(2, false, TargetFlags.None)
            {
                m_FishCommissionDeed = fishCommissionDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null) return;
                if (from.Deleted) return;
                if (m_FishCommissionDeed == null) return;
                if (m_FishCommissionDeed.Deleted) return;

                if (!m_FishCommissionDeed.IsChildOf(from))
                {
                    from.SendMessage("The commission deed is no longer in your backpack.");
                    return;
                }               

                if (target is RawFishSteak || target is RawFish || target is BaseFish || target is BaseMagicFish)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("The fish you wish to add must be in your backpack.");
                        return;
                    }

                    int amountNeeded = m_FishCommissionDeed.FishRequired - m_FishCommissionDeed.m_FishAdded;

                    if (amountNeeded >= item.Amount)
                    {
                        m_FishCommissionDeed.m_FishAdded += item.Amount;
                       
                        item.Delete();

                        from.SendSound(0x5AB);
                        from.SendMessage("You add the fish to the commission deed.");

                        return;
                    }

                    else
                    {
                        item.Amount -= amountNeeded;

                        m_FishCommissionDeed.Delete();
                        m_FishCommissionDeed.CompleteCommission(from);
                    }
                }

                else
                {
                    from.SendMessage("Only fish may be added to this commission deed.");
                    return;
                }
            }
        } 

        public void CompleteCommission(Mobile from)
        {
            if (from == null) return;

            from.SendSound(0x5AE);
            from.SendMessage("You fill the fishing commission deed.");

            from.AddToBackpack(new FishCommissionCompletedDeed());     
        }

        public FishCommissionDeed(Serial serial): base(serial)
		{
		}
        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            writer.Write(m_FishAdded);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_FishAdded = reader.ReadInt();
            }
		}
    }

    public class FishCommissionCompletedDeed : Item
    {
        public virtual int FishRequired { get { return 1000; } }

        [Constructable]
        public FishCommissionCompletedDeed(): base(0x14F0)
        {
            Name = "a fish commission deed";
            Hue = 2615;

            Weight = 0.1;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(Completed: " + FishRequired.ToString() + " fish)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Completed commissions may be sold to any fisherman.");
            return;
        }

        public FishCommissionCompletedDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}