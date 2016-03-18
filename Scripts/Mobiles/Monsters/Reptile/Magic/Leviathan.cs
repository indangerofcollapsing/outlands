using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a leviathan corpse" )]
	public class Leviathan : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Leviathan() : this( null )
		{
		}

		[Constructable]
		public Leviathan( Mobile fisher ) : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;
			
			Name = "a leviathan";
			Body = 77;
			BaseSoundID = 353;

			Hue = 2123;

            SetStr(100);
            SetDex(25);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;            

			Fame = 24000;
			Karma = -24000;

			CanSwim = true;
			CantWalk = true;

			if (Utility.Random(5) == 0)
			{
				Rope rope = new Rope();
				rope.ItemID = 0x14F8;
				PackItem(rope);
			}

            PackItem(new RawFishSteak(8));
		}

        public override int OceanDoubloonValue { get { return 150; } }
        public override bool IsOceanCreature { get { return true; } }
            
        public override int Hides { get { return 25; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public static Type[] Artifacts { get { return m_Artifacts; } }

        private static Type[] m_Artifacts = new Type[]
		{
			// Decorations
			typeof( ShipModelOfTheHMSCape ),
            typeof( ChickenOfTheSea ),
            typeof( EyeOfTrout ),
            typeof( Sardines ),
            typeof( HangingFilets ),
			typeof( AdmiralsHeartyRum ),
            typeof( PirateGlassEye ),
            typeof( FishHeads ),
            typeof( RawFish ),

			// Equipment
			//typeof( AlchemistsBauble ),
			//typeof( ArcticDeathDealer ),
			//typeof( BlazeOfDeath ),
			//typeof( BurglarsBandana ),
			//typeof( CaptainQuacklebushsCutlass ),
			//typeof( CavortingClub ),
			//typeof( DreadPirateHat ),
			//typeof( EnchantedTitanLegBone ),
			//typeof( GwennosHarp ),
			//typeof( IolosLute ),
			//typeof( LunaLance ),
			//typeof( NightsKiss ),
			//typeof( NoxRangersHeavyCrossbow ),
			//typeof( PolarBearMask ),
			//typeof( VioletCourage )
		};

        public static void GiveArtifactTo(Mobile m)
        {
            Item item = Loot.Construct(m_Artifacts);

            if (item == null)
                return;

            // TODO: Confirm messages
            if (m.AddToBackpack(item))
                m.SendMessage("As a reward for slaying the mighty leviathan, an artifact has been placed in your backpack.");
            else
                m.SendMessage("As your backpack is full, your reward for destroying the legendary leviathan has been placed at your feet.");
        }

        public override void OnKilledBy(Mobile mob)
        {
            base.OnKilledBy(mob);

            if (Paragon.CheckArtifactChance(mob, this))
            {
                GiveArtifactTo(mob);

                if (mob == m_Fisher)
                    m_Fisher = null;
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Fisher != null && 25 > Utility.Random(100))
                GiveArtifactTo(m_Fisher);

            m_Fisher = null;
        }

		public Leviathan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}		
	}
}
