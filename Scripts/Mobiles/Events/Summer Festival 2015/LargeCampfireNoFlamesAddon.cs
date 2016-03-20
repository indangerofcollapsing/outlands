namespace Server.Items
{
    public class LargeCampfireNoFlamesAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {4966, 0, -1, 2}, {4971, -1, -1, 0}, {4970, 0, 2, 0}// 1	2	3	
			, {4966, -1, 1, 2}, {4969, -1, 0, 2}, {6008, -1, 2, 1}// 4	5	6	
			, {3118, 0, 1, 1}, {3119, 0, 1, 0}, {3117, 0, 0, 0}// 7	8	9	
			, {4972, 2, -1, 0}, {4972, 1, -1, 5}, {4973, 1, -1, 0}// 10	11	12	
			, {4966, 2, 1, 0}, {4966, 1, 2, 0}, {4972, 2, 2, 0}// 13	14	15	
			, {3117, 1, 1, 0}, {3120, 1, 0, 0}, {4965, 2, 0, 0}// 16	17	18	
					};

        [Constructable]
        public LargeCampfireNoFlamesAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public LargeCampfireNoFlamesAddon(Serial serial) : base(serial)
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