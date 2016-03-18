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

namespace Server.Custom
{
	public class BaseHenchman : BaseCreature
	{
        public enum HenchmanGroupType
        {
            Honorable,
            Villain,
            Navy,
            Pirate,
            Undead
        }

        public override bool IsHenchman { get { return true; } }        

        public virtual HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Honorable; } }
        public virtual bool HenchmanHumanoid { get { return true; } }
        public virtual bool Recruitable { get { return true; } }
        
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        public override bool AllowParagon { get { return false; } }

        public override bool CanRummageCorpses { get { return false; } }

        public virtual string[] recruitSpeech { get { return new string[0]; } }
        public virtual string[] idleSpeech { get{ return new string[0];} }
        public virtual string[] combatSpeech { get { return new string[0]; } }

        public override bool IsRangedPrimary { get { return false; } }

        public DateTime DecayExpiration = DateTime.MaxValue;
      
        public static TimeSpan DecayDuration = TimeSpan.FromHours(12);
        public static TimeSpan DecayCheckInterval = TimeSpan.FromHours(1);

        public List<Item> m_Items = new List<Item>();

        private Timer m_Timer;     
        
        [Constructable]
		public BaseHenchman() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{
            SpeechHue = 0;

            if (Recruitable)
                Blessed = true;            

            if (HenchmanHumanoid)
            {
                Hue = Utility.RandomSkinHue();

                if (this.Female = Utility.RandomBool())
                {
                    Body = 0x191;
                    Name = NameList.RandomName("female");
                }

                else
                {
                    Body = 0x190;
                    Name = NameList.RandomName("male");
                }

                Backpack backpack = new Backpack();
                backpack.Movable = false;
                AddItem(backpack);

                GenerateItems();
                EquipItems();
            }
		}

        public virtual void GenerateItems()
        {
            if (!HenchmanHumanoid)
                return;
        }

        public virtual void EquipItems()
        {
            if (!HenchmanHumanoid)
                return;

            foreach (Item item in m_Items)
            {
                if (item != null)
                {
                    if (!item.Deleted)
                    {
                        if ((item is BaseWeapon && !(item is BaseRanged)) || item is BaseShield)
                        {
                            if (!IsRangedPrimary)
                                AddItem(item);
                            else
                                PackItem(item);
                        }

                        else if (item is BaseRanged)
                        {
                            if (IsRangedPrimary)
                                AddItem(item);
                            else
                                PackItem(item);
                        }

                        else
                            AddItem(item);
                    }
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!Controlled)
                return;

            PlayerMobile playerController = ControlMaster as PlayerMobile;

            if (playerController == null)
                return;

            double frequency = .001;           
            double infrequentValue = .0001;

            if (!Hidden && !IsDeadBondedPet && Alive)
            {
                if (Combatant != null)
                {
                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.IdleOnly)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.IdleOnlyInfrequent)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.None)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.Infrequent)
                        frequency = infrequentValue;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.CombatOnlyInfrequent)
                        frequency = infrequentValue;

                    if (Utility.RandomDouble() <= frequency)
                        Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);
                }

                else
                {
                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.CombatOnly)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.CombatOnlyInfrequent)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.None)
                        frequency = 0;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.Infrequent)
                        frequency = infrequentValue;

                    if (playerController.m_HenchmenSpeechDisplayMode == HenchmenSpeechDisplayMode.IdleOnlyInfrequent)
                        frequency = infrequentValue;

                    if (Utility.RandomDouble() <= frequency)
                        Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                }
            }
        }

        public override void OnAfterResurrect()
        {
            base.OnAfterResurrect();

            if (HenchmanHumanoid)                
                EquipItems();
        }

        public override int GetHurtSound()
        {
            if (HenchmanHumanoid)
            {
                int sound;

                if (Female)
                    sound = Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B);
                else
                    sound = Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x5F6, 0x436, 0x437, 0x43B, 0x43C);

                return sound;
            }

            else
                return base.GetHurtSound();
        }

        public override int GetDeathSound()
        {
            if (HenchmanHumanoid)
            {
                int sound;

                if (Female)
                    sound = Utility.RandomList(0x150, 0x151f, 0x152f, 0x153f, 0x57A, 0x54C, 0x314f, 0x315f, 0x316f, 0x317f);
                else
                    sound = Utility.RandomList(0x15A, 0x15B, 0x15C, 0x15D, 0x5F5, 0x54D, 0x53F, 0x423, 0x424, 0x425, 0x426, 0x427, 0x438);

                return sound;
            }

            else
                return base.GetDeathSound();
        }

        public override void OnAfterSpawn()
        {
            base.OnAfterSpawn();

            if (Recruitable)
            {
                DecayExpiration = DateTime.UtcNow + DecayDuration;

                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        private class InternalTimer : Timer
        {
            private BaseHenchman m_BaseHenchman;

            public InternalTimer(BaseHenchman baseHenchman): base(TimeSpan.Zero, DecayCheckInterval)
            {
                Priority = TimerPriority.OneMinute;

                m_BaseHenchman = baseHenchman;
            }

            protected override void OnTick()
            {
                if (m_BaseHenchman == null)
                {
                    Stop();
                    return;
                }

                if (m_BaseHenchman.Deleted || !m_BaseHenchman.Alive)
                {
                    Stop();
                    return;
                }

                if (!m_BaseHenchman.Recruitable)
                {
                    Stop();
                    return;
                }

                if (m_BaseHenchman.DecayExpiration <= DateTime.UtcNow && !m_BaseHenchman.Controlled && m_BaseHenchman.TimesTamed == 0)
                {
                    Stop();
                    m_BaseHenchman.Delete();

                    return;
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Delete();
        }

        public BaseHenchman(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int) 1); // version

            writer.Write(m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            //Version 1
            writer.Write(DecayExpiration);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_Items = new List<Item>();

            //Version 0
            if (version >= 0)
            {
                int items = reader.ReadInt();
                for (int a = 0; a < items; a++)
                {
                    Item item = reader.ReadItem();

                    if (item != null)
                        m_Items.Add(item);
                }
            }

            //Version 1
            if (version >= 1)
            {
                DecayExpiration = reader.ReadDateTime();
            }

            if (Recruitable)
            {
                if (DecayExpiration != DateTime.MaxValue && !Controlled && TimesTamed == 0)
                {
                    m_Timer = new InternalTimer(this);
                    m_Timer.Start();
                }
            }
		}
	}
}
