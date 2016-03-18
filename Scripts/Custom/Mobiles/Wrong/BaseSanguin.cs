using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguin corpse")]
    public class BaseSanguin : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public int itemHue = 1775;
        public int weaponHue = 2051;
        public int hairHue = Utility.RandomHairHue();
        
        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "*prays*",
                    "I only hope I shall be found worthy",
                    "I will give my blood freely...",
                    "The time is nearly upon us...",
                    "*stares off into the distance*",
                    "My life is yours, master...",
                    "We live to serve...",
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "Die interloper!",
                    "You shall make an adequate sacrifice",
                    "Behold the power of our blood!",
                    "Your end is nigh!",
                    "Give your soul unto us!",
                    "Death is but the beginning!", 
                };
            }
        }
        
        [Constructable]
        public BaseSanguin(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 0x22;
            Name = "a sanguin";
            Hue = Utility.RandomSkinHue(); 
            
            if (this.Female = Utility.RandomBool())            
                this.Body = 0x191;            

            else   
                this.Body = 0x190;   
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                else
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }
        }        

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override bool OnBeforeDeath()
        {
            AddItem(new Kilt(907));

            return base.OnBeforeDeath();
        }

        public BaseSanguin(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}