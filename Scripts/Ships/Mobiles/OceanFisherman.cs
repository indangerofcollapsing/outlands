using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;

namespace Server.Custom.Pirates
{
	public class OceanFisherman : OceanBaseCreature
	{
        public override string[] idleSpeech { get { return new string[] {       "Looks like rain...",
                                                                                "Storm's a brewin'",
                                                                                "*spits*",
                                                                                "*whistles*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Mercy!",
                                                                                "I yield!",
                                                                                "*panicks*",
                                                                                "*Leave me be!*"
                                                                                };}}

        public override int DoubloonValue { get { return 4; } }

        public DateTime m_NextCastAllowed;
        public TimeSpan NextCastDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextThrowingBottleAllowed;
        public TimeSpan NextThrowingBottleDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(4,8));
        
        [Constructable]
		public OceanFisherman(): base()
		{
            SpeechHue = 0x3F;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");

                AddItem(new Shirt(Utility.RandomDyedHue()));
                AddItem(new Skirt(Utility.RandomDyedHue()));

                Title = "the fisherwoman";
            }

            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");

                AddItem(new ShortPants(Utility.RandomNeutralHue()));

                Title = "the fisherman";
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(4, 8);

            SetSkill(SkillName.Fencing, 60);
            SetSkill(SkillName.Swords, 60);
            SetSkill(SkillName.Macing, 60);
            SetSkill(SkillName.Archery, 60);
            SetSkill(SkillName.Wrestling, 60);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Fishing, 80);

            VirtualArmor = 25;

            Fame = 500;
            Karma = 2000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
			
			AddItem( new FishingPole() );

			Container pack = new Backpack();

			pack.Movable = false;
			AddItem( pack );
		}

        public override void SetUniqueAI()
        {
        }

		public override bool CanTeach { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextThrowingBottleAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee && !BardPacified)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                    {
                        int minDamage = 1;
                        int maxDamage = 2;

                        int itemId = 0;
                        int itemIdA = 0;
                        int itemIdB = 0;
                        int itemHitSound = 0;

                        int thrownItem = Utility.RandomMinMax(1, 2);

                        switch (thrownItem)
                        {
                            case 1:
                                minDamage = 6;
                                maxDamage = 12;

                                itemId = Utility.RandomList(2459, 2463, 2503); //Bottles

                                itemIdA = itemId;
                                itemIdB = itemId;

                                itemHitSound = Utility.RandomList(0x38D, 0x38E, 0x38F, 0x390);

                                SpecialAbilities.ThrowObjectAbility(this, combatant, 1.5, 5, .5, minDamage, maxDamage, itemIdA, itemIdB, 0, -1, itemHitSound, .66);
                            break;

                            case 2:
                                minDamage = 5;
                                maxDamage = 10;

                                itemId = Utility.RandomList(2508, 2509, 2510, 2511, 7701, 7702, 7703, 7704); //Fish + Fishparts

                                itemIdA = itemId;
                                itemIdB = itemId;

                                itemHitSound = Utility.RandomList(0x5A2, 0x5D8);

                                SpecialAbilities.ThrowObjectAbility(this, combatant, 1.5, 5, .66, minDamage, maxDamage, itemIdA, itemIdB, 0, -1, itemHitSound, .33);
                            break;
                        }

                        m_NextThrowingBottleAllowed = DateTime.UtcNow + NextThrowingBottleDelay;
                    }
                }
            }

            if (Utility.RandomDouble() < 0.2 && Combatant == null && DateTime.UtcNow > m_NextCastAllowed && !BardPacified)
            {
                if (Body.IsHuman)
                {
                    int waterLocationChecks = 5;
                    int minFishingRadius = 2;
                    int maxFishingRadius = 6;

                    bool foundWaterSpot = false;

                    Point3D newLocation = new Point3D();

                    for (int a = 0; a < waterLocationChecks; a++)
                    {
                        int x = X;

                        int xOffset = Utility.RandomMinMax(minFishingRadius, maxFishingRadius);
                        if (Utility.RandomDouble() >= .5)
                            xOffset *= -1;

                        x += xOffset;

                        int y = Y;

                        int yOffset = Utility.RandomMinMax(minFishingRadius, maxFishingRadius);
                        if (Utility.RandomDouble() >= .5)
                            yOffset *= -1;

                        y += yOffset;

                        newLocation.X = x;
                        newLocation.Y = y;
                        newLocation.Z = -5;

                        LandTile landTile = Map.Tiles.GetLandTile(newLocation.X, newLocation.Y);

                        if ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311))
                        {
                            if (BaseBoat.FindBoatAt(newLocation, Map) != null)
                                continue;

                            foundWaterSpot = true;

                            break;
                        }

                        else
                            continue;                            
                    }

                    if (foundWaterSpot)
                    {
                        m_NextCastAllowed = DateTime.UtcNow + NextCastDelay;

                        Direction = GetDirectionTo(newLocation);

                        if (AIObject != null)
                            AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(5);

                        Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                        {
                            if (this == null) return;
                            if (!this.Alive || this.Deleted) return;
                            if (this.Combatant != null) return;

                            Animate(11, 5, 1, true, false, 0);
                        });

                        Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
                        {
                            if (this == null) return;
                            if (!this.Alive || this.Deleted) return;
                            if (this.Combatant != null) return;

                            Effects.SendLocationEffect(newLocation, Map, 0x352D, 7);
                            Effects.PlaySound(newLocation, Map, 0x027);
                        });
                    }
                }
                
            }
        }

        public OceanFisherman(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
