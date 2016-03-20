using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using System.Linq;
using Server.Misc;
using System.Text;

namespace Server.Mobiles
{   
    [CorpseName("an orcish corpse")]
    public class BaseOrc : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);
        
        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Ug",
                    "Skah, agral...",
                    "Bubhosh uruk, bubhosh hungur.",
                    "Srinkh rabu",
                    "Gah, nub jul ologz",
                    "Narkuu a magru",
                    "Shineees...",
                    "Ug der stinki",
                    "Gah! Lat Dumhed!",
                    "Toopid humies..."
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "Hoowah!",
                    "Lat gu'n git clomped!",
                    "Skah! Gib tribuut!",
                    "Pushdug glob!",
                    "Kihagh, ogba, faugh!",
                    "Duguurz snaga!", 
                    "HOOWAH!",
                    "Lat guin flat!",
                    "Pushdung humie! Me makr lat ded"
                };
            }
        }

        [Constructable]
        public BaseOrc(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 2310;
            Body = Utility.RandomMinMax(400, 401);
            Hue = 2417;

            Name = "an orc";
            BaseSoundID = 0x45A;
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

        public BaseOrc(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
