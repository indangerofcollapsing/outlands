using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a drow corpse")]
    public class BaseDrow : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public int itemHue = 1105;
        public int weaponHue = 2051;
        public int hairHue = 1150;
        
        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Jal khaless zhah waela",
                    "Oloth zhah tuth abbil lueth ogglin",
                    "Lloth tlu malla, jal ultrinnan zhah xundus",
                    "Ilharessen zhaunil alurl",
                    "Khaless nau uss mzild taga dosstan",
                    "Ssinssrigg lueth",
                    "Lloth kyorl dos d'jal",
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "Ssussun pholor dos!",
                    "Ssussun!",
                    "Oloth plynn dos!",
                    "Olot dos!",
                    "Jiv'ellg lueth jiv'undus phuul jivvin!",
                    "Dal utrinnan ulu el'inssrigg", 
                };
            }
        }
        
        [Constructable]
        public BaseDrow(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 0x075;
            Name = "a drow";
            Hue = 1105;
            
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

        public override int GetAngerSound() {return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481);}
        public override int GetIdleSound() {return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481);}
        public override int GetHurtSound() {return 0x5F9;}
        public override int GetDeathSound() {return 0x5F5;}

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override bool OnBeforeDeath()
        {
            AddItem(new Kilt());

            return base.OnBeforeDeath();
        }

        public BaseDrow(Serial serial): base(serial)
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