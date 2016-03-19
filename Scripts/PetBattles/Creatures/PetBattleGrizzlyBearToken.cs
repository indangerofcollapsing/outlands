using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class PetBattleGrizzlyBearToken : PetBattleCreatureToken
	{
        public override Type m_Type { get { return typeof(PetBattleGrizzlyBear); } }
        
        [Constructable]
        public PetBattleGrizzlyBearToken(): base()
		{           
			Weight = 1.0;            
		}

        public PetBattleGrizzlyBearToken(Serial serial): base(serial)
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}