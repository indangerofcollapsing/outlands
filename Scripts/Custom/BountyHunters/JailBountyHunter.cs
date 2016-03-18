using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class JailBountyHunter : BaseCreature
	{

        public Mobile m_target;
        private BountyHunterTimer m_Timer;


		public JailBountyHunter(Mobile target) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the bounty hunter";
			Hue = Utility.RandomSkinHue();
            m_target = target;
			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			Fame = 250;
			Karma = 250;

            if (Utility.Random(10) <= 2)
            {
                Horse mount = new Horse();
                mount.Rider = this;
            }

            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new FancyShirt(Utility.RandomNeutralHue()));
            AddItem(new Bandana(Utility.RandomNeutralHue()));

            SetStr(100, 140);
            SetDex(75, 100);
            SetInt(100, 140);

            if (Utility.RandomDouble() < .70)
            {
                SetDamage(20, 35);

                SetSkill(SkillName.Fencing, 85, 120);
                SetSkill(SkillName.Macing, 85, 120);
                SetSkill(SkillName.MagicResist, 85, 120);
                SetSkill(SkillName.Swords, 85, 120);
                SetSkill(SkillName.Tactics, 85, 120);
                SetSkill(SkillName.Wrestling, 100, 120);
  
                switch (Utility.Random(4))
                {
                    case 0:
                        {
                            AddItem(new StuddedChest());
                            AddItem(new StuddedArms());
                            AddItem(new StuddedLegs());
                            AddItem(new StuddedGloves());
                            AddItem(new StuddedGorget());
                        } break;
                    case 1:
                        {
                            AddItem(new StuddedChest());
                            AddItem(new StuddedLegs());
                            AddItem(new MetalKiteShield());
                        } break;
                    case 2:
                        {
                            AddItem(new PlateChest());
                            AddItem(new PlateLegs());
                            AddItem(new HeaterShield());
                        } break;
                    case 3:
                        {
                            AddItem(new RingmailChest());
                            AddItem(new RingmailLegs());
                            AddItem(new RingmailArms());
                            AddItem(new WoodenShield());
                        } break;

                }
                switch (Utility.Random(6))
                {
                    case 0: AddItem(new Longsword()); break;
                    case 1: AddItem(new Cutlass()); break;
                    case 2: AddItem(new Broadsword()); break;
                    case 3: AddItem(new Axe()); break;
                    case 4: AddItem(new Club()); break;
                    case 5: AddItem(new Mace()); break;
                    case 6: AddItem(new CompositeBow()); break;
                }
            }
            else
            {
                SetDamage(10, 15);

                SetDamageType(ResistanceType.Physical, 50);

                SetResistance(ResistanceType.Physical, 30, 40);
                SetResistance(ResistanceType.Fire, 20, 30);
                SetResistance(ResistanceType.Cold, 20, 30);
                SetResistance(ResistanceType.Poison, 20, 30);
                SetResistance(ResistanceType.Energy, 40, 50);

                SetSkill(SkillName.EvalInt, 85, 120);
                SetSkill(SkillName.Fencing, 85, 120);
                SetSkill(SkillName.Macing, 85, 120);
                SetSkill(SkillName.Magery, 85, 120);
                SetSkill(SkillName.Meditation, 85, 120);
                SetSkill(SkillName.MagicResist, 85, 120);
                SetSkill(SkillName.Swords, 85, 120);
                SetSkill(SkillName.Tactics, 85, 120);
                SetSkill(SkillName.Wrestling, 85, 120);

                AI = AIType.AI_Mage;

            }
			Utility.AssignRandomHair( this );
            m_Timer = new BountyHunterTimer(this);
            m_Timer.Start();

		}

        public JailBountyHunter( Serial serial ) : base( serial )
		{
		}

        public override bool IsEnemy(Mobile m)
        {
            return m == m_target;
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if (mount != null)
                mount.Rider = null;

            if (mount is Mobile)
                ((Mobile)mount).Delete();


            Server.Custom.JailBountyHunterControl.RemoveHunter(this);

            return base.OnBeforeDeath();
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

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { Delete(); });
		}

        public class BountyHunterTimer : Timer
        {
            private DateTime m_End;
            private Mobile target;
            private JailBountyHunter bHunter;
            private bool speak = false;

            public BountyHunterTimer(JailBountyHunter from)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(150))
            {
				m_End = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                bHunter = from;
                target = from.m_target;
            }

            protected override void OnTick()
            {
				if (DateTime.UtcNow > m_End)
                {
                    bHunter.Delete();
                    this.Stop();
                }
                bHunter.Criminal = false;
                if (speak == false)
                {
                    speak = true;
                    switch (Utility.Random(7))
                    {
                        case 0: { bHunter.Yell("There he is!"); } break;
                        case 1: { bHunter.Yell("He's killed again!"); } break;
                        case 2: { bHunter.Yell("For the Reward!"); } break;
                        case 3: { bHunter.Yell("After him! Don't let him get away!"); } break;
                        case 4: { bHunter.Yell("Look what he's done!"); } break;
                        case 5: { bHunter.Yell("Stop, you murderer!"); } break;
                        case 6: { bHunter.Yell("Let's get that Reward!"); } break;
                    }
                }

                if (bHunter.InRange(target.Location, 1))
                {
                    if (!target.Alive) {
                        Server.Custom.JailBountyHunterControl.RegisterHunterKill(target);
                        bHunter.Delete();
                        Stop();
                    }
                    bHunter.Combatant = target;
                }
                else if (bHunter.InRange(target.Location, 15))
                {
                    bHunter.Move(bHunter.GetDirectionTo(target));
                }
                else
                {
                    bHunter.Delete();
                    this.Stop();
                }
            }
        }
	}


}
