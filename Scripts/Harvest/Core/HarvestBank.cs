using System;

namespace Server.Engines.Harvest
{
	public class HarvestBank
	{		
        private DateTime m_NextRespawn;

        HarvestDefinition m_Definition;
		public HarvestDefinition Definition
		{
			get { return m_Definition; }
		}

        private int m_Current;
        private int m_Maximum;
		public int Current
		{
			get
			{
				CheckRespawn();
				return m_Current;
			}
		}

        private HarvestVein m_Vein;
		public HarvestVein Vein
		{
			get
			{
				CheckRespawn();
				return m_Vein;
			}

			set
			{
				m_Vein = value;
			}
		}

        private HarvestVein m_DefaultVein;
		public HarvestVein DefaultVein
		{
			get
			{
				CheckRespawn();
				return m_DefaultVein;
			}
		}

		public void CheckRespawn()
		{
			if ( m_Current == m_Maximum || m_NextRespawn > DateTime.UtcNow )
				return;

			m_Current = m_Maximum;

			if ( m_Definition.RandomizeVeins )			
				m_DefaultVein = m_Definition.GetVeinFrom( Utility.RandomDouble() );			

			m_Vein = m_DefaultVein;
		}

		public void Consume( int amount, Mobile from )
		{
			CheckRespawn();

			if ( m_Current == m_Maximum )
			{
				double min = m_Definition.MinRespawn.TotalMinutes;
				double max = m_Definition.MaxRespawn.TotalMinutes;
				double rnd = Utility.RandomDouble();

				m_Current = m_Maximum - amount;

				double minutes = min + (rnd * (max - min));

				m_NextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes( minutes );
			}

			else			
				m_Current -= amount;			

			if ( m_Current < 0 )
				m_Current = 0;
		}

		public HarvestBank( HarvestDefinition def, HarvestVein defaultVein )
		{
			m_Maximum = Utility.RandomMinMax( def.MinTotal, def.MaxTotal );
			m_Current = m_Maximum;
			m_DefaultVein = defaultVein;
			m_Vein = m_DefaultVein;

			m_Definition = def;
		}
	}
}