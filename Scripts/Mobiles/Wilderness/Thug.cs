using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a thug's corpse")]
    public class Thug : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);
                
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
        public Thug(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a thug";
            Hue = Utility.RandomSkinHue(); 
             
            this.Body = 0x190;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(175);

            SetDamage(12, 24);

            SetSkill(SkillName.Macing, 85);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            AddItem(new StuddedGloves() { Movable = true, Hue = 0 });
            AddItem(new StuddedGorget() { Movable = true, Hue = 0 });
            AddItem(new ShortPants() { Movable = true, Hue = Utility.RandomNeutralHue() });           
            
            switch(Utility.RandomMinMax(1, 1))
            {
                case 1: AddItem(new Club() { Movable = true, Hue = 0 }); break;                     
            }            
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

        public Thug(Serial serial): base(serial)
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