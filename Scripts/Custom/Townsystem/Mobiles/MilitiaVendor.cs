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

namespace Server.Mobiles
{
	public class MilitiaVendor : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        public override bool HandlesOnSpeech(Mobile from) { return true; }

		[Constructable]
		public MilitiaVendor() : base( "the Militia Vendor" )
		{
            Female = true;

			SpeechHue = Utility.RandomDyedHue();
            Title = "the militia dealer";
            Hue = Utility.RandomSkinHue();

            EquipItem(new Doublet() { Hue = 1109, Movable = false });
            EquipItem(new HalfApron() { Hue = 1109, Movable = false });
            EquipItem(new Spear() { Hue = 1109, Movable = false });
            EquipItem(new Skirt() { Hue = 1109, Movable = false });
            EquipItem(new Sandals() { Hue = 1109, Movable = false });
            EquipItem(new LeatherGorget() { Hue = 1109, Movable = false });
            EquipItem(new LeatherChest() { Hue = 1109, Movable = false });
            EquipItem(new LeatherArms() { Hue = 1109, Movable = false });
            EquipItem(new LeatherLegs() { Hue = 1109, Movable = false });
            EquipItem(new LeatherGloves() { Hue = 1109, Movable = false });
            EquipItem(new LeatherCap() { Hue = 1109, Movable = false });

            Utility.AssignRandomHair(this, GetHairHue());

		}

		public override void InitSBInfo()
		{

		}

        public MilitiaVendor(Serial serial)
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
                var ps = PlayerState.Find(from);
                var player = from as PlayerMobile;

				if (ps == null || player == null)
                {
                    PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, "You must be in a militia to purchase these goods.");
                }
                else
                {
                    PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, String.Format("Welcome to the militia vendor shop! You currently have {0} key{1} available.", player.TreasuryKeys, player.TreasuryKeys == 1 ? "" : "s"));
                    from.SendGump(new MilitiaVendorGump(0));
                }
            }
        
            base.OnSpeech(e);
        }

        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}