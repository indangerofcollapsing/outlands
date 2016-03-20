using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a poacher's corpse")]
    public class Poacher : BaseCreature
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
        public Poacher(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a poacher";
            Hue = Utility.RandomSkinHue(); 
            
            if (Female = Utility.RandomBool())            
                Body = 0x191;            

            else   
                Body = 0x190;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(350);

            SetDamage(8, 16);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new BearMask() { Movable = false, Hue = 0 });
            AddItem(new StuddedChest() { Movable = true, Hue = 0 });
            AddItem(new StuddedLegs() { Movable = true, Hue = 0 });
            AddItem(new LeatherArms() { Movable = true, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = true, Hue = 0 });
            AddItem(new LeatherGorget() { Movable = true, Hue = 0 });
            AddItem(new Boots() { Movable = true, Hue = 0 });            

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1:
                    AddItem(new Bow() { Movable = false, Hue = 0 });
                    PackItem(new Arrow(10));
                break;
                case 2:
                    AddItem(new Crossbow() { Movable = false, Hue = 0 });
                    PackItem(new Bolt(10));
                break;
                case 3:
                    AddItem(new HeavyCrossbow() { Movable = false, Hue = 0 });
                    PackItem(new Bolt(10));
                break;
            }           
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;

            DictCombatTargeting[CombatTargeting.Predator] = 1;
            DictCombatTargeting[CombatTargeting.Prey] = 1;

            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 5;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;

            CombatHealActionMinDelay = 15;
            CombatHealActionMaxDelay = 30;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.EntangleSpecialAbility(0.05, this, defender, 1.0, 5, -1, true, "", "Their arrow pins you in place!");
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

        public Poacher(Serial serial): base(serial)
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