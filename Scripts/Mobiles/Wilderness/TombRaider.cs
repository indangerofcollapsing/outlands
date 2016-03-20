using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a tomb raider's corpse")]
    public class TombRaider : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Keep searching, there's more gold to be found!",
                    "Once we get these to Bucc's, we'll be rich!",
                    "I can't believe those fools left these relics unguarded!"
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
        public TombRaider(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a tomb raider";
            Hue = Utility.RandomSkinHue(); 
            
            if (Female = Utility.RandomBool())            
                Body = 0x191;            

            else   
                Body = 0x190;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(450);
            SetMana(2000);

            SetDamage(9, 18);

            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            int clothingColor = Utility.RandomDyedHue();

            AddItem(new FancyShirt() { Movable = true, Hue = clothingColor });
            AddItem(new BodySash() { Movable = true, Hue = clothingColor });
            AddItem(new WizardsHat() { Movable = true, Hue = clothingColor });
            AddItem(new LongPants() { Movable = true, Hue = Utility.RandomNeutralHue() });
            AddItem(new LeatherGloves() { Movable = true, Hue = 0 });            
            AddItem(new Boots() { Movable = true, Hue = 0 });

            AddItem(new Lantern());

            switch(Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new Broadsword() { Movable = false, Hue = 0 }); break;
                case 2: AddItem(new Kryss() { Movable = false, Hue = 0 }); break;
                case 3: AddItem(new Mace() { Movable = false, Hue = 0 }); break;                
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

        public TombRaider(Serial serial): base(serial)
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