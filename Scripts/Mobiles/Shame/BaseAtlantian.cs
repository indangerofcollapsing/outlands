using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an atlantian corpse")]
    public class BaseAtlantian : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public int itemHue = 1175;
        public int alternateHue = 2118;
        public int hairHue = 2411;
        
        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "",
                    "",
                    "",
                    "",
                    "",
                    "", 
                };
            }
        }
        
        [Constructable]
        public BaseAtlantian(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 2222;
            Name = "an atlantian";
            Hue = 2222;
            
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

        public override int GetAngerSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetIdleSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetHurtSound() { return Utility.RandomList(0x302, 0x303); }
        public override int GetDeathSound() { return 0x304; }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override bool OnBeforeDeath()
        { 
            if (this is AtlanteanBattleMage)
                AddItem(new Skirt(438));
            else
                AddItem(new Skirt(902));

            return base.OnBeforeDeath();
        }

        public BaseAtlantian(Serial serial): base(serial)
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