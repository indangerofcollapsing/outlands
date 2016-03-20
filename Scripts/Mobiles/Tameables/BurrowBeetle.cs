using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a burrow beetle corpse" )]
	public class BurrowBeetle : BaseCreature
	{
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(50, 70));
        
        [Constructable]
        public BurrowBeetle(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            Name = "a burrow beetle";
            Body = 0x317;
            Hue = 0;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 150;            

			Fame = 2000;
			Karma = -2000;

			Tameable = true;
			ControlSlots = 1;
			MinTameSkill = 70;
		}
        
        public override bool RevealImmune { get { return !Controlled; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Controlled && ControlMaster is PlayerMobile)
                return;

            double hitsPercent = (double)Hits / (double)HitsMax;

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed && hitsPercent < .50)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    Point3D originalLocation = Location;

                    if (SpecialAbilities.VanishAbility(this, 5.0, false, 0x21D, 5, 10, true, null))
                    {
                        for (int a = 0; a < 3; a++)
                        {
                            if (Utility.RandomDouble() <= .50)
                            {
                                //Rocks
                                Blood rocks = new Blood();
                                rocks.Name = "rocks";
                                rocks.ItemID = Utility.RandomList(4967, 4970, 4973);

                                Point3D rockLocation = new Point3D(originalLocation.X + Utility.RandomMinMax(-2, 2), originalLocation.Y + Utility.RandomMinMax(-2, 2), originalLocation.Z);

                                rocks.MoveToWorld(rockLocation, Map);
                            }

                            else
                            {
                                //Dirt
                                Blood dirt = new Blood();
                                dirt.Name = "dirt";
                                dirt.ItemID = Utility.RandomList(7681, 7682);

                                Point3D dirtLocation = new Point3D(originalLocation.X + Utility.RandomMinMax(-2, 2), originalLocation.Y + Utility.RandomMinMax(-2, 2), Z);

                                dirt.MoveToWorld(dirtLocation, Map);
                            }
                        }

                        PublicOverheadMessage(MessageType.Regular, 0, false, "*burrows*");

                        Combatant = null;

                        Effects.PlaySound(Location, Map, GetIdleSound());
                    }

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public override int TamedItemId { get { return 9743; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return -10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 9; } }
        public override int TamedBaseMaxDamage { get { return 11; } }
        public override double TamedBaseWrestling { get { return 65; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 150; } }

		public override int GetAngerSound(){return 0x4F3;}
		public override int GetIdleSound(){return 0x4F2;}
		public override int GetAttackSound(){ return 0x607;}
		public override int GetHurtSound(){return 0x608;}
		public override int GetDeathSound()	{return 0x4F0;}

		public BurrowBeetle( Serial serial ) : base( serial )
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