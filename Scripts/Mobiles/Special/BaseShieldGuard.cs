using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;
using Server.Custom.Townsystem;

namespace Server.Mobiles
{
	public abstract class BaseShieldGuard : BaseCreature
	{
        private Faction m_Faction;
        
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Faction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; }
        }
        
        public BaseShieldGuard() : base( AIType.AI_Melee, FightMode.Aggressor, 14, 1, 0.8, 1.6 )
		{
			InitStats( 1000, 1000, 1000 );
			Title = "the guard";

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );

				AddItem( new FemalePlateChest() );
				AddItem( new PlateArms() );
				AddItem( new PlateLegs() );

				switch( Utility.Random( 2 ) )
				{
					case 0: AddItem( new Doublet( Utility.RandomNondyedHue() ) ); break;
					case 1: AddItem( new BodySash( Utility.RandomNondyedHue() ) ); break;
				}

				switch( Utility.Random( 2 ) )
				{
					case 0: AddItem( new Skirt( Utility.RandomNondyedHue() ) ); break;
					case 1: AddItem( new Kilt( Utility.RandomNondyedHue() ) ); break;
				}
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );

				AddItem( new PlateChest() );
				AddItem( new PlateArms() );
				AddItem( new PlateLegs() );

				switch( Utility.Random( 3 ) )
				{
					case 0: AddItem( new Doublet( Utility.RandomNondyedHue() ) ); break;
					case 1: AddItem( new Tunic( Utility.RandomNondyedHue() ) ); break;
					case 2: AddItem( new BodySash( Utility.RandomNondyedHue() ) ); break;
				}
			}

			Utility.AssignRandomHair( this );
			if( Utility.RandomBool() )
				Utility.AssignRandomFacialHair( this, HairHue );

			VikingSword weapon = new VikingSword();
			weapon.Movable = false;
			AddItem( weapon );

			BaseShield shield = Shield;
			shield.Movable = false;
			AddItem( shield );

			PackGold( 250, 500 );

			Skills[SkillName.Anatomy].Base = 120.0;
			Skills[SkillName.Tactics].Base = 120.0;
			Skills[SkillName.Swords].Base = 120.0;
			Skills[SkillName.MagicResist].Base = 120.0;
			Skills[SkillName.DetectHidden].Base = 100.0;
		}

		public abstract int Keyword{ get; }
		public abstract BaseShield Shield{ get; }
		public abstract int SignupNumber{ get; }
		public abstract GuildType Type{ get; }

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 2 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.Mobile.InRange( this.Location, 2 ) )
			{
                string text = e.Speech.Trim().ToLower();

                if (text.IndexOf("chaos shield") != -1)
                {
                    e.Handled = true;
                    Mobile from = e.Mobile;

                    var faction = Faction.Find(from);

                    if (faction == null || faction.Alliance == null || faction.Alliance.Name != "Chaos")
                    {
                        SayTo(from, "Good sire, thou art not in the proper faction to receive such a shield.");
                        return;
                    }

                    Container pack = from.Backpack;
                    BaseShield shield = new ChaosShield();
                    Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                    if ((pack != null && pack.FindItemByType(shield.GetType()) != null) || (twoHanded != null && shield.GetType().IsAssignableFrom(twoHanded.GetType())))
                    {
                        Say(1007110); // Why dost thou ask about virtue guards when thou art one?
                        shield.Delete();
                    }
                    else if (from.PlaceInBackpack(shield))
                    {
                        Say(Utility.Random(1007101, 5));
                        Say(1007139); // I see you are in need of our shield, Here you go.
                        from.AddToBackpack(shield);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502868); // Your backpack is too full.
                        shield.Delete();
                    }
                }
                else if (text.IndexOf("order shield") != -1)
                {
                    e.Handled = true;
                    Mobile from = e.Mobile;

                    var faction = Faction.Find(from);

                    if (faction == null || faction.Alliance == null || faction.Alliance.Name != "Order")
                    {
                        SayTo(from, "Good sire, thou art not in the proper faction to receive such a shield.");
                        return;
                    }

                    Container pack = from.Backpack;
                    BaseShield shield = new OrderShield();
                    Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                    if ((pack != null && pack.FindItemByType(shield.GetType()) != null) || (twoHanded != null && shield.GetType().IsAssignableFrom(twoHanded.GetType())))
                    {
                        Say(1007110); // Why dost thou ask about virtue guards when thou art one?
                        shield.Delete();
                    }
                    else if (from.PlaceInBackpack(shield))
                    {
                        Say(Utility.Random(1007101, 5));
                        Say(1007139); // I see you are in need of our shield, Here you go.
                        from.AddToBackpack(shield);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502868); // Your backpack is too full.
                        shield.Delete();
                    }
                }
                else if (text.IndexOf("balance shield") != -1)
                {
                    e.Handled = true;
                    Mobile from = e.Mobile;

                    var faction = Faction.Find(from);

                    if (faction == null || faction.Alliance == null || faction.Alliance.Name != "Balance")
                    {
                        SayTo(from, "Good sire, thou art not in the proper faction to receive such a shield.");
                        return;
                    }
                    
                    Container pack = from.Backpack;
                    BaseShield shield = new BalanceShield();
                    Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                    if ((pack != null && pack.FindItemByType(shield.GetType()) != null) || (twoHanded != null && shield.GetType().IsAssignableFrom(twoHanded.GetType())))
                    {
                        Say(1007110); // Why dost thou ask about virtue guards when thou art one?
                        shield.Delete();
                    }
                    else if (from.PlaceInBackpack(shield))
                    {
                        Say(Utility.Random(1007101, 5));
                        Say(1007139); // I see you are in need of our shield, Here you go.
                        from.AddToBackpack(shield);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502868); // Your backpack is too full.
                        shield.Delete();
                    }
                }

			}

			base.OnSpeech( e );
		}

		public BaseShieldGuard( Serial serial ) : base( serial )
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