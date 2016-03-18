using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a smuggler's corpse")]
    public class Smuggler : BaseCreature
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
        public Smuggler(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a smuggler";
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
                this.Body = 0x191;

            else
                this.Body = 0x190;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(350);

            SetDamage(12, 24);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Parry, 30);

            SetSkill(SkillName.Poisoning, 50);

            VirtualArmor = 50;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            int clothingColor = Utility.RandomDyedHue();

            AddItem(new StuddedGloves() { Movable = true, Hue = 0 });
            AddItem(new StuddedArms() { Movable = true, Hue = 0 });
            AddItem(new BodySash() { Movable = true, Hue = clothingColor });
            AddItem(new Cloak() { Movable = true, Hue = clothingColor });
            AddItem(new LongPants() { Movable = true, Hue = clothingColor });          
            AddItem(new ThighBoots() { Movable = true, Hue = 0 });
            
            switch(Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new Broadsword() { Movable = true, Hue = 0 }); break;  
                case 2: AddItem(new Cutlass() { Movable = true, Hue = 0 }); break;  
                case 3: AddItem(new Katana() { Movable = true, Hue = 0 }); break;    
            }

            AddItem(new Buckler() { Movable = true, Hue = 0 });
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.AttackOnly] = 8;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 5;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;

            CombatHealActionMinDelay = 15;
            CombatHealActionMaxDelay = 30;
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

        public Smuggler(Serial serial): base(serial)
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