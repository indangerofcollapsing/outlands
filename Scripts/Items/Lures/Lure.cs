using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Custom
{
    public class Lure : Item
    {
        public virtual int AggroBonus { get { return 3; } }
        public virtual TimeSpan AggroExpiration { get { return TimeSpan.FromMinutes(20); } }
        public virtual int LureHue { get { return 2635; } }

        public int MaxCharges = 10;

        private int m_Charges = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        [Constructable]
        public Lure(): base(3619)
        {
            Name = "a lure";
            Hue = LureHue;

            Weight = 1;
        }

        public Lure(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "[uses remaining: " + m_Charges.ToString() + "]");
        }        

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (m_Charges < 0)
                return;

            if (!IsChildOf(from.Backpack))
                from.SendMessage("This must be in your pack in order to use it.");
            else
            {
                from.SendMessage("Target whom you wish to apply to lure to.");
                from.Target = new LureTarget(this);
            }
        }

        public class LureTarget : Target
        {
            private Lure m_Lure;

            public LureTarget(Lure lure): base(4, false, TargetFlags.None)
            {
                m_Lure = lure;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Lure.Deleted || m_Lure.RootParent != from)
                    return;

                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From == null)
                    return;

                Mobile mobile = target as Mobile;

                if (mobile != null)
                {
                    bool validControlledCreature = false;

                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster == pm_From)
                            validControlledCreature = true;
                    }

                    if (pm_From == mobile || validControlledCreature)
                    {
                        mobile.AggroBonus = m_Lure.AggroBonus;
                        mobile.AggroBonusExpiration = DateTime.UtcNow + m_Lure.AggroExpiration;
                        
                        Blood lureExtract = new Blood();
                        lureExtract.Name = "residue";
                        lureExtract.Hue = m_Lure.LureHue;
                        lureExtract.MoveToWorld(new Point3D(mobile.X + Utility.RandomMinMax(-1, 1), mobile.Y + Utility.RandomMinMax(-1, 1), mobile.Z + 1), mobile.Map);
                        
                        for (int a = 0; a < 3; a++)
                        {
                            Blood extraLureExtract = new Blood();
                            extraLureExtract.Name = "residue";
                            extraLureExtract.Hue = m_Lure.LureHue;
                            extraLureExtract.MoveToWorld(new Point3D(mobile.X + Utility.RandomMinMax(-1, 1), mobile.Y + Utility.RandomMinMax(-1, 1), mobile.Z + 1), mobile.Map);
                        }

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x4F1);
                        mobile.FixedParticles(0x374A, 10, 20, m_Lure.LureHue, 1107, 0, EffectLayer.Head);

                        if (mobile == pm_From)
                        {
                            pm_From.SendMessage("You apply the lure to yourself, hoping to focus the attention of nearby foes onto yourself.");
                            pm_From.RevealingAction();
                        }

                        else
                        {
                            pm_From.SendMessage("You apply the lure, hoping to focus the attention of nearby foes onto them.");
                            pm_From.RevealingAction();
                            mobile.RevealingAction();
                        }

                        pm_From.PublicOverheadMessage(MessageType.Regular, 0, false, "*applies lure*");

                        m_Lure.m_Charges--;

                        if (m_Lure.m_Charges <= 0)
                            m_Lure.Delete();
                    }

                    else
                        pm_From.SendMessage("You may only apply a lure to yourself or to a creature that you control.");
                }

                else
                {
                    pm_From.SendMessage("You may only apply a lure to yourself or to a creature that you control.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
    }
}
