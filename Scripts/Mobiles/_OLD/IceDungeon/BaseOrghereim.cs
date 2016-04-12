using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orghereim corpse")]
    public class BaseOrghereim : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public int itemHue = 1326;
        public int weaponHue = 0x482;
        public int hairHue = Utility.RandomHairHue();
        
        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Svejln",
                    "Johl son mot nost",
                    "Nolseh holl",
                    "Mjoln nemn shet",
                    "Olan hurs mehnet",
                    "Feldra fhen falot",
                    "Melst henno",
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "Svejln fuurst!",
                    "Mjoln hehm tosk!",
                    "Nehn mols sevn!",
                    "Eften ma senn djol!",
                    "Lelh gran brenh!",
                    "Gans thos kjelm mos!", 
                };
            }
        }
        
        [Constructable]
        public BaseOrghereim(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 1326;
            Name = "an orghereim";
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
            return base.OnBeforeDeath();
        }

        public BaseOrghereim(Serial serial): base(serial)
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