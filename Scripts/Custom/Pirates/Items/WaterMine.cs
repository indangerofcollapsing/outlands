using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Guilds;
using Server.Custom.Pirates.Battleground;
using System.Collections.Generic;

namespace Server.Custom.Pirates
{
    public class DepthCharge : Item, IHideable
    {
        #region IHideable Interface Members
        //IHideable Interface Members
        public int HideLevel { get { return 80; } set { } }
        public bool Findable { get { return true; } set { } }
        public Mobile Finder { get { return null; } set { } }
        public bool IsVisible { get { return false; } set { } }
        public void Reveal(Mobile from)
        {
            if (CanBeSeenBy(from))
                return;

            VisiblityList.Add(from);

            if (from.NetState != null)
                SendInfoTo(from.NetState);
        }
        #endregion


        private bool m_Exploding;
        private static int[] m_RandomEffectIDs = new int[] { 0xC2D, 0xC2E, 0xC2F, 0xC30 };
        private BaseGuild m_Guild;
        private List<Mobile> VisiblityList = new List<Mobile>();

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public BaseGuild ActiveGuild { get { return m_Guild; } set { m_Guild = value; } }

        public override string DefaultName { get { return "depth charge"; } }

        [Constructable]
        public DepthCharge()
            : base(0x4228)
        {
            Movable = false;
        }

        public DepthCharge(Guild guild)
            : base(0x4228)
        {
            m_Guild = guild;
            Movable = false;
        }

        public void Explode(BaseBoat b)
        {
            if (m_Exploding == true)
                return;

            m_Exploding = true;

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (b != null)
                    b.ReceiveDamage(null, null, 250, DamageType.Hull);

                Effects.PlaySound(Location, Map, 0x207);
                Effects.PlaySound(Location, Map, 0x11E);
                Effects.SendLocationEffect(new Point3D(Location, Location.Z + 5), Map, 0x36BD, 20);

                for (int i = 0; i < 2; i++)
                    Effects.SendMovingEffect(this, new Entity(Serial.Zero, new Point3D(X + Utility.RandomMinMax(-6, 6), Y + Utility.RandomMinMax(-6, 6), Z + Utility.RandomMinMax(10, 20)), Map), Utility.RandomList(m_RandomEffectIDs), 1, 0, false, true);

                Delete();
            });
        }

        public override void OnAfterDelete()
        {
            BattlegroundDefense.OnDepthChargeExplode(this);
            base.OnAfterDelete();
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            return ActiveGuild == null || (from.Guild != null && from.Guild == ActiveGuild) || VisiblityList.Contains(from);
        }

        public DepthCharge(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); //version

            writer.Write(m_Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version > 0)
                m_Guild = reader.ReadGuild();
        }
    }
}
