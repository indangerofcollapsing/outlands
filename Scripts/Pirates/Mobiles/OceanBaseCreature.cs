using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom.Pirates
{
	public class OceanBaseCreature : BaseCreature
	{
        public override bool AllowParagon { get { return false; } }

        public override bool CanRummageCorpses { get { return false; } }       

        public virtual string[] idleSpeech { get{ return new string[0];} }
        public virtual string[] combatSpeech { get { return new string[0]; } }
        
        [Constructable]
		public OceanBaseCreature() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{			
		}

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.001)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                else
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);
            }
        }

        public OceanBaseCreature(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
