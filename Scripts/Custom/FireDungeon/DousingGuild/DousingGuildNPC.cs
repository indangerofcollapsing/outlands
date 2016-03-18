/***************************************************************************
 *                            PaladinVendor.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos ---- DESIGNED BY AZ HE'S REALLY COOL
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Custom.Paladin;
using Server.Custom.Townsystem;
using Scripts.Custom;

namespace Server.Mobiles
{
	public class DousingGuildNPC : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        public override bool HandlesOnSpeech(Mobile from) { return true; }


		[Constructable]
		public DousingGuildNPC() : base( "the Dousing Guild Vendor" )
		{
			SpeechHue = Utility.RandomDyedHue();
            Title = "the dousing guild vendor";
            Hue = Utility.RandomSkinHue();

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);

		}

		public override void InitSBInfo()
		{
			//m_SBInfos.Add( new SBStavesWeapon() );
			//m_SBInfos.Add( new SBPaladinVendor() );
			//m_SBInfos.Add( new SBWoodenShields() );
			
			//if ( IsTokunoVendor )
				//m_SBInfos.Add( new SBSEPaladinVendor() );
		}

        public DousingGuildNPC(Serial serial)
            : base(serial)
		{
		}

        public bool WasNamed( string speech )
		{
			return this.Name != null && Insensitive.StartsWith( speech, this.Name );
		}

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if ( e.Handled || !from.Alive || from.GetDistanceToSqrt( this ) > 3 )
				return;

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171) && WasNamed(e.Speech))) // vendor buy, *buy*
            {
                int points =  FireDungeonPlayerState.GetDousingPoints(from as PlayerMobile);
                PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, String.Format("Welcome to the dousing guild vendor shop! You currently have {0} credit{1} available.", points, points == 1 ? "" : "s"));
                from.SendGump(new DousingGuildVendorGump(1));
            }
        
            base.OnSpeech(e);
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