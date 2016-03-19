namespace Server.Items
{
	public class LargeGreenCampFireNoKindle : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {4966, 0, -1, 2}, {4971, -1, -1, 0}, {4970, 0, 2, 0}// 1	2	3	
			, {4966, -1, 1, 2}, {4969, -1, 0, 2}, {6008, -1, 2, 1}// 4	5	6	
			, {3118, 0, 1, 1}, {3119, 0, 1, 0}, {3117, 0, 0, 0}// 11	12	13	
			, {4972, 2, -1, 0}, {4973, 1, -1, 0}, {4972, 1, -1, 5}// 15	16	17	
			, {4965, 2, 0, 0}, {4966, 2, 1, 0}, {4966, 1, 2, 0}// 18	19	20	
			, {4972, 2, 2, 0}, {3117, 1, 1, 0}, {3120, 1, 0, 0}// 21	24	25	
		};

		[ Constructable ]
		public LargeGreenCampFireNoKindle()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );

			AddComplexComponent( (BaseAddon) this, 3561, 0, 1, 0, 2005, 0, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 3561, 0, 0, 0, 0, 0, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 3561, 0, 0, 6, 2005, 0, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 3561, 0, 1, 9, 2005, 0, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 3555, 0, 0, 1, 2003, 0, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 3561, 1, 1, 0, 2005, 0, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 3561, 1, 0, 7, 2005, 0, "", 1);// 23
			//AddComplexComponent( (BaseAddon) this, 6571, 1, 1, 8, 2003, 0, "", 1);// 26
		}

        public LargeGreenCampFireNoKindle(Serial serial) : base(serial) {}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac = new AddonComponent(item);

            if (!string.IsNullOrEmpty(name))
                ac.Name = name;

            if (hue != 0)
                ac.Hue = hue;

            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }

            if (lightsource != -1)
                ac.Light = (LightType) lightsource;

            addon.AddComponent(ac, xoffset, yoffset, zoffset);
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
}