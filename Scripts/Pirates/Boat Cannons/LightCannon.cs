using System;
using Server;
using Server.Items;
using Server.Multis;

namespace Server.Custom.Pirates
{
    public class LightCannon : BaseCannon
    {
        public override BaseCannonDeed GetDeed { get { return new LightCannonDeed(); } }

        public override int NorthID { get { return 0x2c5; } }
        public override int EastID { get { return 0x2c6; } }
        public override int SouthID { get { return 0x2dc; } }
        public override int WestID { get { return 0x2dd; } }

        public LightCannon(BaseBoat b)
            : base(0x2c5,b)
        {
            Movable = false;
        }

        public LightCannon(Serial serial)
            : base(serial)
        {
        }


        public override void OnSingleClick(Mobile from)
        {
            //LabelTo(from, "Light Cannon");
            LabelTo(from, "[Ammunition: {0}/{1}]", CurrentCharges, MaxCharges);
            
            BaseBoat baseBoat = this.m_Boat as BaseBoat;

            if (baseBoat != null)
            {
                if (baseBoat.CannonCooldown <= DateTime.UtcNow)
                {
                    if (CurrentCharges > 0)
                    {
                        LabelTo(from, "Ready to fire");
                    }

                    else
                    {
                        LabelTo(from, "Needs ammunition");
                    }
                }

                else
                {
                    if (CurrentCharges > 0)
                    {
                        int secondsToFire = (int)(Math.Ceiling((baseBoat.CannonCooldown - DateTime.UtcNow).TotalSeconds));

                        if (secondsToFire == 0)
                        {
                            LabelTo(from, "Ready to fire");
                        }

                        else
                        {
                            LabelTo(from, "Fireable in " + secondsToFire.ToString() + " seconds");
                        }
                    }

                    else
                    {
                        LabelTo(from, "Needs ammunition");  
                    }
                }
            }

            base.OnSingleClick(from);
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
    public class LightCannonDeed : BaseCannonDeed
    {
        public override CannonTypes CannonType { get { return CannonTypes.Light; } }
        
        public override BaseCannon Addon
        {
            get
            {
                return new LightCannon(null);
            }
        }

        [Constructable]
        public LightCannonDeed()
        {
            Name = "Light Cannon Deed";
        }

        public LightCannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    public class LightCannonPlans : BaseCannonPlans
    {
        [Constructable]
        public LightCannonPlans()
        {
            Name = "Light Cannon Plans";
        }

        public LightCannonPlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}