using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class BagOfSpecialPlate : Bag
	{
		[Constructable] 
		public BagOfSpecialPlate()
		{
			Movable = true;
			Hue = 0x25;
			Name = "a bag of Special Plate Armor";

			SpecialPlateChest chest = new SpecialPlateChest();
			chest.Quality = ArmorQuality.Exceptional;
			chest.LootType = LootType.Blessed;
			DropItem(chest);

			SpecialPlateArms arms = new SpecialPlateArms();
			arms.Quality = ArmorQuality.Exceptional;
			arms.LootType = LootType.Blessed;
			DropItem(arms);

			SpecialPlateGloves gloves = new SpecialPlateGloves();
			gloves.Quality = ArmorQuality.Exceptional;
			gloves.LootType = LootType.Blessed;
			DropItem(gloves);

			SpecialPlateGorget gorget = new SpecialPlateGorget();
			gorget.Quality = ArmorQuality.Exceptional;
			gorget.LootType = LootType.Blessed;
			DropItem(gorget);

			SpecialPlateLegs legs = new SpecialPlateLegs();
			legs.Quality = ArmorQuality.Exceptional;
			legs.LootType = LootType.Blessed;
			DropItem(legs);

			SpecialPlateHelm helm = new SpecialPlateHelm();
			helm.Quality = ArmorQuality.Exceptional;
			helm.LootType = LootType.Blessed;
			DropItem(helm);
		}

		public BagOfSpecialPlate( Serial serial ) : base( serial ) 
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

	public static class SpecialPlate
	{
		public static void OnEquip( PlayerMobile m, Layer layer )
		{
			if ( m == null )
				return;
			
			Item arms       = m.FindItemOnLayer( Layer.Arms );
			Item innerTorso = m.FindItemOnLayer( Layer.InnerTorso );
			Item gloves     = m.FindItemOnLayer( Layer.Gloves );
			Item neck       = m.FindItemOnLayer( Layer.Neck );
			Item helm       = m.FindItemOnLayer( Layer.Helm );
			Item pants      = m.FindItemOnLayer( Layer.Pants );
			
			bool bonusCondition = false;
			switch ( layer )
			{
				case Layer.Arms:
				{
					bonusCondition = innerTorso != null && innerTorso.GetType() == typeof( SpecialPlateChest )
						&& gloves != null && gloves.GetType() == typeof( SpecialPlateGloves )
						&& pants != null && pants.GetType() == typeof( SpecialPlateLegs )
						&& helm != null && helm.GetType() == typeof( SpecialPlateHelm )
						&& neck != null && neck.GetType() == typeof( SpecialPlateGorget );
					break;
				}
				case Layer.InnerTorso:
				{
					bonusCondition = gloves != null && gloves.GetType() == typeof( SpecialPlateGloves )
						&& pants != null && pants.GetType() == typeof( SpecialPlateLegs )
						&& neck != null && neck.GetType() == typeof( SpecialPlateGorget )
						&& helm != null && helm.GetType() == typeof( SpecialPlateHelm )
						&& arms != null && arms.GetType() == typeof( SpecialPlateArms );
					break;
				}
				case Layer.Gloves:
				{
					bonusCondition = innerTorso != null && innerTorso.GetType() == typeof( SpecialPlateChest )
						&& pants != null && pants.GetType() == typeof( SpecialPlateLegs )
						&& neck != null && neck.GetType() == typeof( SpecialPlateGorget )
						&& helm != null && helm.GetType() == typeof( SpecialPlateHelm )
						&& arms != null && arms.GetType() == typeof( SpecialPlateArms );
					break;
				}
				case Layer.Neck:
				{
					bonusCondition = innerTorso != null && innerTorso.GetType() == typeof( SpecialPlateChest )
						&& gloves != null && gloves.GetType() == typeof( SpecialPlateGloves )
						&& pants != null && pants.GetType() == typeof( SpecialPlateLegs )
						&& helm != null && helm.GetType() == typeof( SpecialPlateHelm )
						&& arms != null && arms.GetType() == typeof( SpecialPlateArms );
					break;
				}
				case Layer.Helm:
				{
					bonusCondition = innerTorso != null && innerTorso.GetType() == typeof( SpecialPlateChest )
					&& gloves != null && gloves.GetType() == typeof( SpecialPlateGloves )
					&& pants != null && pants.GetType() == typeof( SpecialPlateLegs )
					&& neck != null && neck.GetType() == typeof( SpecialPlateGorget )
					&& arms != null && arms.GetType() == typeof( SpecialPlateArms );
					break;
				}
				case Layer.Pants:
				{
					bonusCondition = innerTorso != null && innerTorso.GetType() == typeof( SpecialPlateChest )
						&& gloves != null && gloves.GetType() == typeof( SpecialPlateGloves )
						&& neck != null && neck.GetType() == typeof( SpecialPlateGorget )
						&& helm != null && helm.GetType() == typeof( SpecialPlateHelm )
						&& arms != null && arms.GetType() == typeof( SpecialPlateArms );
					break;
				}
			}
			
			if ( bonusCondition )
				OnSetBonusApplied( m );
		}
		
		public static void OnRemoved( PlayerMobile m, Layer layer )
		{
			if ( m == null )
				return;
			
			bool removedCondition = false;
			switch ( layer )
			{
				case Layer.Arms:
				{
					removedCondition = m.FindItemOnLayer( Layer.InnerTorso ) is SpecialPlateChest
						&& m.FindItemOnLayer( Layer.Gloves ) is SpecialPlateGloves
						&& m.FindItemOnLayer( Layer.Neck ) is SpecialPlateGorget
						&& m.FindItemOnLayer( Layer.Pants ) is SpecialPlateLegs
						&& m.FindItemOnLayer( Layer.Helm ) is SpecialPlateHelm;
					break;
				}
				case Layer.InnerTorso:
				{
					removedCondition = m.FindItemOnLayer( Layer.Gloves ) is SpecialPlateGloves
						&& m.FindItemOnLayer( Layer.Pants ) is SpecialPlateLegs
						&& m.FindItemOnLayer( Layer.Arms ) is SpecialPlateArms
						&& m.FindItemOnLayer( Layer.Neck ) is SpecialPlateGorget
						&& m.FindItemOnLayer( Layer.Helm ) is SpecialPlateHelm;
					break;
				}
				case Layer.Gloves:
				{
					removedCondition = m.FindItemOnLayer( Layer.InnerTorso ) is SpecialPlateChest
						&& m.FindItemOnLayer( Layer.Pants ) is SpecialPlateLegs
						&& m.FindItemOnLayer( Layer.Arms ) is SpecialPlateArms
						&& m.FindItemOnLayer( Layer.Neck ) is SpecialPlateGorget
						&& m.FindItemOnLayer( Layer.Helm ) is SpecialPlateHelm;
					break;
				}
				case Layer.Neck:
				{
					removedCondition = m.FindItemOnLayer( Layer.InnerTorso ) is SpecialPlateChest
						&& m.FindItemOnLayer( Layer.Gloves ) is SpecialPlateGloves
						&& m.FindItemOnLayer( Layer.Arms ) is SpecialPlateArms
						&& m.FindItemOnLayer( Layer.Pants ) is SpecialPlateLegs
						&& m.FindItemOnLayer( Layer.Helm ) is SpecialPlateHelm;
					break;
				}
				case Layer.Helm:
				{
					removedCondition = m.FindItemOnLayer( Layer.InnerTorso ) is SpecialPlateChest
						&& m.FindItemOnLayer( Layer.Gloves ) is SpecialPlateGloves
						&& m.FindItemOnLayer( Layer.Arms ) is SpecialPlateArms
						&& m.FindItemOnLayer( Layer.Pants ) is SpecialPlateLegs
						&& m.FindItemOnLayer( Layer.Neck ) is SpecialPlateGorget;
					break;
				}
				case Layer.Pants:
				{
					removedCondition = m.FindItemOnLayer( Layer.InnerTorso ) is SpecialPlateChest
						&& m.FindItemOnLayer( Layer.Gloves ) is SpecialPlateGloves
						&& m.FindItemOnLayer( Layer.Arms ) is SpecialPlateArms
						&& m.FindItemOnLayer( Layer.Neck ) is SpecialPlateGorget
						&& m.FindItemOnLayer( Layer.Helm ) is SpecialPlateHelm;
					break;
				}
			}
			
			if ( removedCondition )
				OnSetBonusRemoved( m );
		}
		
		private static void OnSetBonusApplied( PlayerMobile m )
		{
			m.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
			m.PlaySound( 0x1ED );
			
			string text = String.Format( "* A magical aura surrounds {0}. *", m.Name );
			m.PublicOverheadMessage( MessageType.Emote, 0x0, false, text );

			//m.ActiveSetBonuses ^= SetBonus.PlateMeditation;
		}
		
		private static void OnSetBonusRemoved( PlayerMobile m )
		{
			//m.ActiveSetBonuses ^= SetBonus.PlateMeditation;

			string text = String.Format( "* A magical aura fades from {0}. *", m.Name );
			m.PublicOverheadMessage( MessageType.Emote, 0x0, false, text );
		}
	}
	
	[FlipableAttribute( 0x1410, 0x1417 )]
	public class SpecialPlateArms : BaseArmor
	{
		//Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 80; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -2; } }
        //Changed to IPY values
		public override int ArmorBase{ get{ return 30; } }
		//Added by IPY
        public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateArms() : base( 0x1410 )
		{
			Weight = 5.0;
		}

		public SpecialPlateArms( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.Arms );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.Arms );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x1415, 0x1416 )]
	public class SpecialPlateChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
        //Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 95; } }
		public override int OldStrReq{ get{ return 60; } }
		//Changed to IPY values
		public override int OldDexBonus{ get{ return -4; } }

        //Changed to IPY values
		public override int ArmorBase{ get{ return 30; } }
		//Added by IPY
        public override int RevertArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateChest() : base( 0x1415 )
		{
			Weight = 10.0;
		}

		public SpecialPlateChest( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.InnerTorso );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.InnerTorso );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x1414, 0x1418 )]
	public class SpecialPlateGloves : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

        //Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 70; } }
		public override int OldStrReq{ get{ return 30; } }
 
        //Changed to IPY values
		public override int OldDexBonus{ get{ return -1; } }
        //Changed to IPY values
		public override int ArmorBase{ get{ return 30; } }
		//Added by IPY
        public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateGloves() : base( 0x1414 )
		{
			Weight = 2.0;
		}

		public SpecialPlateGloves( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.Gloves );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.Gloves );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SpecialPlateGorget : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
		//Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 45; } }
		public override int OldStrReq{ get{ return 30; } }

		public override int OldDexBonus{ get{ return -1; } }
        //Changed to IPY values
		public override int ArmorBase{ get{ return 30; } }
		//Added by IPY
        public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateGorget() : base( 0x1413 )
		{
			Weight = 2.0;
		}

		public SpecialPlateGorget( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.Neck );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.Neck );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SpecialPlateHelm : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
		//Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 45; } }
		public override int OldStrReq{ get{ return 30; } }

		public override int OldDexBonus{ get{ return -1; } }
        //Changed to IPY values
		public override int ArmorBase{ get{ return 30; } }
		//Added by IPY
        public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateHelm() : base( 0x1412 )
		{
			Weight = 5.0;
		}

		public SpecialPlateHelm( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.Helm );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.Helm );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x1411, 0x141a )]
	public class SpecialPlateLegs : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

        //Changed to IPY values
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 90; } }

		public override int OldStrReq{ get{ return 60; } }
        //Changed to IPY values
		public override int OldDexBonus{ get{ return -3; } }
        //Changed to IPY values 
		public override int ArmorBase{ get{ return 30; } }
		//Added by iPY
        public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public SpecialPlateLegs() : base( 0x1411 )
		{
			Weight = 7.0;
		}

		public SpecialPlateLegs( Serial serial ) : base( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			SpecialPlate.OnEquip( from as PlayerMobile, Layer.Pants );

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			SpecialPlate.OnRemoved( parent as PlayerMobile, Layer.Pants );

			base.OnRemoved( parent );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}