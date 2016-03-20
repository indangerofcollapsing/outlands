using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a drow blackguard corpse")]
    public class DrowBlackguard : BaseDrow
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);     

        [Constructable]
        public DrowBlackguard(): base()
        {            
            Name = "a drow blackguard";  

            SetStr(75);
            SetDex(100);
            SetInt(75);

            SetHits(600);
            SetMana(1000);

            SetDamage(12, 24);

            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Swords, 100);

            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 50);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;

            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new ChainChest() { Movable = false, Hue = itemHue });
            AddItem(new RingmailArms() { Movable = false, Hue = itemHue });
            AddItem(new RingmailGloves() { Movable = false, Hue = itemHue });
            AddItem(new ChainLegs() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });

            Utility.AssignRandomHair(this, hairHue);

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new Kryss() { Movable = false, Hue = weaponHue,Layer = Layer.FirstValid, Name = "a drow nightblade" }); break;
                case 2: AddItem(new Katana() { Movable = false, Hue = weaponHue, Layer = Layer.FirstValid, Name = "drow runeblades" }); break;
            }
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.20;

            DictCombatAction[CombatAction.CombatHealSelf] = 2;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;
            
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] = 1;
            
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;

            CombatHealActionMinDelay = 10;
            CombatHealActionMaxDelay = 20;
        }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))                    
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }        

        public DrowBlackguard(Serial serial): base(serial)
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