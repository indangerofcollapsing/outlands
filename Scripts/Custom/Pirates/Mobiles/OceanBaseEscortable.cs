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
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Custom.Pirates
{
	public class OceanBaseEscortable : BaseCreature
	{   
        public override bool BardImmune { get { return true; } }		
        public override bool CanRummageCorpses { get { return false; } }
        public override bool AllowParagon { get { return false; } }

        public virtual string[] helpSpeech { get{ return new string[0];} }
        public virtual string[] idleSpeech { get { return new string[0]; } }
        public virtual string[] combatSpeech { get { return new string[0]; } }

        public virtual string[] destinations { get { return new string[] { "Minoc", "Britain", "Vesper", "Trinsic" }; } }

        private EDI m_Destination;
        private string m_DestinationString;

        private DateTime m_DeleteTime;
        private Timer m_DeleteTimer;

        protected DateTime m_NextSpeech = DateTime.UtcNow;
        protected DateTime m_NextDestinationCheck;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Destination
        {
            get { return m_Destination == null ? null : m_Destination.Name; }
            set { m_DestinationString = value; m_Destination = EDI.Find(value); }
        }
        
		[Constructable]
		public OceanBaseEscortable() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{
            if (Female = Utility.RandomBool())
            {
                Body = 401;
                Name = NameList.RandomName("female");
            }

            else
            {
                Body = 400;
                Name = NameList.RandomName("male");
            }

            Hue = Utility.RandomSkinHue();

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 500;
            Karma = 500;            
            
            AddItem(new FancyShirt(Utility.RandomNeutralHue()));
            AddItem(new ShortPants(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);
		}

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextDestinationCheck > DateTime.UtcNow)
                return;

            if (!CheckAtDestination())
            {
                if (m_NextSpeech <= DateTime.UtcNow)
                {
                    if (Combatant == null)
                    {
                        Say(idleSpeech[Utility.Random(helpSpeech.Length - 1)]);
                        m_NextSpeech = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));
                    }

                    else
                    {
                        Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);
                        m_NextSpeech = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));
                    }                    
                }

                m_NextDestinationCheck = DateTime.UtcNow + TimeSpan.FromSeconds(2);
            }
        }

        public virtual bool CheckAtDestination()
        {
            EDI dest = GetDestination();

            if (dest == null)
                return false;

            if (dest.Contains(Location))
            {
                Say("Ah! Thank the heavens! I am safe and sound! Here is thy pay as promised!");

                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                {
                    if (this != null)                    
                        Delete();                    
                });                

                return true;
            }

            return false;
        }

        public EDI GetDestination()
        {
            if (m_DestinationString == null && m_DeleteTimer == null)
                m_DestinationString = PickRandomDestination();

            if (m_Destination != null && m_Destination.Name == m_DestinationString)
                return m_Destination;

            if (Map.Felucca.Regions.Count > 0)
                return (m_Destination = EDI.Find(m_DestinationString));

            return (m_Destination = null);
        }

        public virtual string PickRandomDestination()
        {
            if (Map.Felucca.Regions.Count == 0 || Map == null || Map == Map.Internal || Location == Point3D.Zero)
                return null;

            string[] possible = destinations;
            string picked = null;

            while (picked == null)
            {
                picked = possible[Utility.Random(possible.Length)];
                EDI test = EDI.Find(picked);

                if (test != null && test.Contains(Location))
                    picked = null;
            }

            return picked;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 6))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            EDI dest = GetDestination();

            if (dest != null && !e.Handled && e.Mobile.InRange(Location, 6))
            {
                if (e.HasKeyword(0x1D)) // *destination*
                    e.Handled = SayDestinationTo(e.Mobile);

                else if (e.HasKeyword(0x1E)) // *i will take thee*
                    e.Handled = AcceptEscorter(e.Mobile);
            }
        }

        public virtual bool SayDestinationTo(Mobile mobile)
        {
            EDI dest = GetDestination();

            if (dest == null || mobile == null)
                return false;

            if (!mobile.Alive)
                return false;

            Say("If thou could rescue me and bring me to " + dest.Name + " I would be grateful and would reward thee for helping me!");

            return true;           
        }

        public virtual bool AcceptEscorter(Mobile mobile)
        {
            EDI dest = GetDestination();

            if (dest == null || mobile == null)
                return false;

            if (!mobile.Alive)
                return false;

            BaseBoat m_Boat = BaseBoat.FindBoatAt(mobile.Location, mobile.Map);

            if (m_Boat != null)
            {
                if (m_Boat.Deleted || m_Boat.m_SinkTimer != null)
                {
                    mobile.SendMessage("Your boat is in no condition right now to be taking on passengers!");

                    return false;
                }

                if (!(m_Boat.IsOwner(mobile) || m_Boat.IsCoOwner(mobile)))
                {
                    mobile.SendMessage("Only owners and co-owners of this ship have permission to take on passengers.");

                    return false;
                }

                Say("Thank you! Let us return to " + dest.Name);

                if (Body.IsHuman && !Mounted)
                    Animate(32, 3, 1, true, false, 0);

                if (m_DeleteTimer != null)
                    m_DeleteTimer.Stop();

                m_DeleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(120);

                m_DeleteTimer = new DeleteTimer(this, m_DeleteTime - DateTime.UtcNow);
                m_DeleteTimer.Start();

                Location = m_Boat.GetRandomEmbarkLocation(true);
                m_Boat.AddEmbarkedMobile(this);
            }

            return true;
        }

        private class DeleteTimer : Timer
        {
            private Mobile m_Mobile;

            public DeleteTimer(Mobile mobile, TimeSpan delay): base(delay)
            {
                m_Mobile = mobile;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Mobile.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_DeleteTimer != null)
                m_DeleteTimer.Stop();

            m_DeleteTimer = null;

            base.OnAfterDelete();
        }

        public OceanBaseEscortable(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            EDI dest = GetDestination();

            writer.Write(dest != null);

            if (dest != null)
                writer.Write(dest.Name);

            writer.Write(m_DeleteTimer != null);

            if (m_DeleteTimer != null)
                writer.WriteDeltaTime(m_DeleteTime);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (reader.ReadBool())
                m_DestinationString = reader.ReadString();

            if (reader.ReadBool())
            {
                m_DeleteTime = reader.ReadDeltaTime();
                m_DeleteTimer = new DeleteTimer(this, m_DeleteTime - DateTime.UtcNow);
                m_DeleteTimer.Start();
            }

            m_NextDestinationCheck = DateTime.UtcNow;
		}
	}
}
