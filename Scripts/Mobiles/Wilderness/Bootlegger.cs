using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a bootlegger's corpse")]
    public class Bootlegger : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextDrinkAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));
        public TimeSpan NextDrinkDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));

        public DateTime m_NextThrowingBottleAllowed;
        public TimeSpan NextThrowingBottleDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "*hic*"
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
        public Bootlegger(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a bootlegger";
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
                this.Body = 0x191;

            else
                this.Body = 0x190;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(225);

            SetDamage(15, 25);

            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 50);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new LeatherArms() { Movable = true, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = true, Hue = 0 });
            AddItem(new Shirt() { Movable = false, Hue = 2051});
            AddItem(new LongPants() { Movable = true, });
            AddItem(new HalfApron() { Movable = true, Hue = Utility.RandomNeutralHue()});
            AddItem(new Boots() { Movable = true,});
            
            switch(Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new Dagger() { Movable = true, Hue = 0 }); break;  
                case 2: AddItem(new Kryss() { Movable = true, Hue = 0 }); break;  
                case 3: AddItem(new WarFork() { Movable = true, Hue = 0 }); break;    
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
            
            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextDrinkAllowed && Combatant == null)
            {
                Effects.PlaySound(this.Location, this.Map, Utility.RandomList(0x030, 0x031, 0x050));
                Animate(34, 5, 1, true, false, 0);

                AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(2);

                m_NextDrinkAllowed = DateTime.UtcNow + NextDrinkDelay;
            }

            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextThrowingBottleAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                    {
                        int minDamage = 1;
                        int maxDamage = 2;

                        int itemId = 0;
                        int itemIdA = 0;
                        int itemIdB = 0;
                        int itemHitSound = 0;

                        minDamage = 6;
                        maxDamage = 12;

                        itemId = Utility.RandomList(2459, 2463, 2503);

                        itemIdA = itemId;
                        itemIdB = itemId;

                        itemHitSound = Utility.RandomList(0x38D, 0x38E, 0x38F, 0x390);

                        SpecialAbilities.ThrowObjectAbility(this, combatant, 1.5, 5, .5, minDamage, maxDamage, itemIdA, itemIdB, 0, -1, itemHitSound, .66);

                        m_NextThrowingBottleAllowed = DateTime.UtcNow + NextThrowingBottleDelay;
                    }
                }
            }            
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public Bootlegger(Serial serial): base(serial)
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