using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class UOACZDeadTree1RewardAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{3293, 0, 0, 0, 2405},
		};
            
		public override BaseAddonDeed Deed { get { return new UOACZDeadTree1RewardAddonDeed(); }}     

		[ Constructable ]
		public UOACZDeadTree1RewardAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 5; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0], m_AddOnSimpleComponents[i, 4]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public UOACZDeadTree1RewardAddon(Serial serial): base(serial)
		{
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            base.OnComponentUsed(c, from);
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            base.OnLocationChange(oldLoc);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
        }      

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class UOACZDeadTree1RewardAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new UOACZDeadTree1RewardAddon(); } }

		[Constructable]
		public UOACZDeadTree1RewardAddonDeed()
		{
            Name = "a dead oak tree deed";
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(UOACZ Reward)");
        }

		public UOACZDeadTree1RewardAddonDeed( Serial serial ) : base( serial )
        {
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}