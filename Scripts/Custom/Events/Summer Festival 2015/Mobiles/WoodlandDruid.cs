using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a druid corpse")]
    public class WoodlandDruid : BaseCreature
    {
        public enum DruidForm
        {
            Human,
            Bear,
            Panther,
            GiantSpider
        }

        public DateTime m_NextTransformationAllowed;
        public DateTime m_NextHumanFormReversionAllowed;

        public static TimeSpan TransformationDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));
        public static TimeSpan HumanFormReversionDelay = TimeSpan.FromSeconds(5);

        public DruidForm m_DruidForm = DruidForm.Human;

        public bool IsRevertingToHumanForm = false;

        List<Mobile> m_Mobiles = new List<Mobile>();

        [Constructable]
        public WoodlandDruid() : base(AIType.AI_Healer, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a woodland druid";            

            if (Female = Utility.RandomBool())            
                Body = 0x191;

            else            
                Body = 0x190;

            SpeechHue = 2538;
            Hue = 2538;

            SetStr(100);
            SetDex(50);
            SetInt(100);
            
            SetHits(1500);
            SetStam(250);
            SetMana(3000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Macing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);            

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);
           
            SetSkill(SkillName.Poisoning, 50);

            VirtualArmor = 25;

            SpellHue = 2210;

            ResolveAcquireTargetDelay = 0.5;

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.4;
            CurrentSpeed = 0.4;

            Fame = 8000;
            Karma = -10000;

            HairItemID = 8252;
            HairHue = 2210;

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new DeerMask() { Movable = false });  break;
                case 2: AddItem(new BearMask() { Movable = false });  break;
            }            

            if (Female)
                AddItem(new FancyDress() { Hue = 2210, Movable = false });
            else
                AddItem(new Robe() { Hue = 2210, Movable = false});
            
            AddItem(new GnarledStaff() { Movable = false });        
        }

        public override void SetUniqueAI()
        {
            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            UniqueCreatureDifficultyScalar = 1.5;
        }

        public override bool AllowParagon { get { return false; } }
        public override Poison HitPoison { get { if (m_DruidForm == DruidForm.GiantSpider)  return Poison.Deadly; return null; } }
        public override Poison PoisonImmune { get { if (m_DruidForm == DruidForm.GiantSpider)  return Poison.Deadly; return null; } }

        public override bool ShowFameTitle { get { return false; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            switch (m_DruidForm)
            {
                case DruidForm.Bear:
                    SpecialAbilities.EnrageSpecialAbility(.25, attacker, this, .25, 10, -1, true, "", "Your attack enrages the target.", "*becomes enraged*");
                break;
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            switch (m_DruidForm)
            {
                case DruidForm.Panther:
                    SpecialAbilities.BleedSpecialAbility(.25, this, defender, DamageMax, 8.0, -1, true, "", "Their bite causes you to bleed!");
                    SpecialAbilities.FrenzySpecialAbility(.25, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied*");
                break;

                case DruidForm.GiantSpider:
                    if (Utility.RandomDouble() <= .25)
                    {
                        Effects.PlaySound(defender.Location, defender.Map, 0x580);

                        Point3D targetLocation = defender.Location;
                        targetLocation.Z += 1;

                        Effects.SendLocationParticles(EffectItem.Create(targetLocation, defender.Map, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308), 0, 125, 0, 0, 5029, 0);
                        targetLocation.Z += 1;

                        Effects.SendLocationParticles(EffectItem.Create(targetLocation, defender.Map, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308), 0, 125, 0, 0, 5029, 0);
                        
                        SpecialAbilities.HinderSpecialAbility(1.0, this, defender, 1.0, Utility.RandomMinMax(4, 6), false, -1, false, "", "You have been wrapped in a web!");
                    }
                break;
            }
        }

        public override void AlterMeleeDamageTo(Mobile target, ref int damage)
        {
            BaseCreature bc_Target = target as BaseCreature;

            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    damage *= 2;
            }                
        }

        public override void OnThink()
        {
            //Currently In the Process of Reverting to Druid Form
            if (IsRevertingToHumanForm)
            {
                if (m_NextHumanFormReversionAllowed <= DateTime.UtcNow)
                {
                    IsRevertingToHumanForm = false;

                    ChangeForm(DruidForm.Human, false);
                    return;
                }
            }

            //Actively in Combat
            if (Combatant != null)
            {
                //Can Transform into Other Forms
                if (m_NextTransformationAllowed <= DateTime.UtcNow)
                {
                    DruidForm newForm = (DruidForm)Utility.RandomMinMax(0, 3);
                    ChangeForm(newForm, false);
                }
            }

            else
            {
                //Begin Reverting to Human Form
                if (m_DruidForm != DruidForm.Human && !IsRevertingToHumanForm && m_NextTransformationAllowed <= DateTime.UtcNow)
                {
                    IsRevertingToHumanForm = true;
                    m_NextHumanFormReversionAllowed = DateTime.UtcNow + HumanFormReversionDelay;                                     
                }
            }         
        }

        public void ChangeForm(DruidForm newForm, bool ignoreCurrentForm)
        {
            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }

            if (newForm == m_DruidForm && !ignoreCurrentForm)
                return;

            PopulateDefaultAI();
            SetGroup(AIGroup.EvilHuman);

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.4;
            CurrentSpeed = 0.4;

            if (Spell != null)
                Spell = null;

            switch (newForm)
            {
                case DruidForm.Human:
                    m_DruidForm = DruidForm.Human;

                    BaseSoundID = 0xA3;

                    if (Female)                    
                        Body = 401;                    
                    else                    
                        Body = 400;

                    Hue = 2538;

                    SetDex(50);

                    SetDamage(15, 25);

                    SetSkill(SkillName.Wrestling, 85);

                    VirtualArmor = 25;

                    SetSubGroup(AISubgroup.GroupHealerMeleeMage4);
                    UpdateAI(false);

                    Effects.PlaySound(Location, Map, 0x4);
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Earthmother, grant me mortal flesh...");
                break;

                case DruidForm.GiantSpider:
                    m_DruidForm = DruidForm.GiantSpider;

                    Body = 28;
                    BaseSoundID = 0x388;

                    Hue = 2515;

                    SetDex(75);

                    SetDamage(15, 25);

                    SetSkill(SkillName.Wrestling, 90);

                    VirtualArmor = 25;
                    
                    SetSubGroup(AISubgroup.None);
                    UpdateAI(false);
                    
                    Effects.PlaySound(Location, Map, 0x4);
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Earthmother, grant me the spider's cunning...");
                break;

                case DruidForm.Panther:
                    m_DruidForm = DruidForm.Panther;

                    Body = 214;
                    BaseSoundID = 0x462;

                    Hue = 1107;

                    SetDex(100);

                    SetDamage(15, 25);

                    SetSkill(SkillName.Wrestling, 100);

                    VirtualArmor = 25;                    

                    SetSubGroup(AISubgroup.Duelist);
                    UpdateAI(false);

                    ActiveSpeed = 0.2;
                    PassiveSpeed = 0.2;
                    CurrentSpeed = 0.2;

                    Effects.PlaySound(Location, Map, 0x4);
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Earthmother, grant me the panther's quickness...");
                break;

                case DruidForm.Bear:
                    m_DruidForm = DruidForm.Bear;

                    Body = 212;
                    BaseSoundID = 0xA3;

                    Hue = 1103;

                    SetDex(50);

                    SetDamage(20, 30);

                    SetSkill(SkillName.Wrestling, 90);

                    VirtualArmor = 150;

                    SetSubGroup(AISubgroup.AntiArmor);
                    UpdateAI(false);

                    ActiveSpeed = 0.3;
                    PassiveSpeed = 0.3;
                    CurrentSpeed = 0.3;

                    Effects.PlaySound(Location, Map, 0x4);
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Earthmother, grant me the bear's strength...");
                break;
            }

            int spellHue = 2538;

            FixedParticles(0x3709, 10, 30, 5052, spellHue, 0, EffectLayer.LeftFoot);

            for (int a = -2; a < 3; a++)
            {
                for (int b = -2; b < 3; b++)
                {
                    double chance = .5;                   
                    
                    if (a == -2 || b == 2)
                        chance = .33;

                    if (a == 0 && b == 0)
                        chance = 0;

                    Point3D greeneryLocation = new Point3D(Location.X + a, Location.Y + b, Z);
                    SpellHelper.AdjustField(ref greeneryLocation, Map, 16, false);

                    if (Utility.RandomDouble() <= chance)
                    {
                        int itemId = Utility.RandomList(3219, 3220, 3255, 3256, 3152, 3153, 3223, 6809, 6811, 3204, 3247, 3248, 3254, 3258, 3259, 3378,
                            3267, 3237, 3267, 9036, 3239, 3208, 3307, 3310, 3311, 3313, 3314, 3332, 3271, 3212, 3213);

                        TimedStatic timedStatic = new TimedStatic(itemId, 29);                           
                        timedStatic.Name = "greenery";
                        timedStatic.MoveToWorld(greeneryLocation, Map);                        
                    }

                    if (Utility.RandomDouble() <= .1)
                    {
                        BaseCreature bc_Creature = null;

                        switch (Utility.RandomMinMax(1, 6))
                        {
                            case 1: bc_Creature = new Rabbit(); break;
                            case 2: bc_Creature = new Bird(); break;
                            case 3: bc_Creature = new BullFrog(); break;
                            case 4: bc_Creature = new Hind(); break;
                            case 5: bc_Creature = new Fox(); break;
                            case 6: bc_Creature = new Ferret(); break;
                        }

                        if (bc_Creature != null)
                        {
                            bc_Creature.MoveToWorld(greeneryLocation, Map);                            
                            m_Mobiles.Add(bc_Creature);
                        }
                    }
                }
            }

            m_NextTransformationAllowed = DateTime.UtcNow + TransformationDelay;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (m_Mobiles[a].Alive)
                        m_Mobiles[a].Kill();
                }
            }
        }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 150) == 1)
            {
                Sandals footwear = new Sandals();
                footwear.Name = "Woodland Sprite Sandals";
                footwear.Hue = 2003;

                PackItem(footwear);
            }

            if (Utility.RandomMinMax(1, 15) == 1)
            {
                PouchOfGypsyGoods rewardPouch = new PouchOfGypsyGoods();
                rewardPouch.Name = "a stolen pouch of gyspy goods";

                PackItem(rewardPouch);
            }

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound()
        {
            switch (m_DruidForm)
            {
                case DruidForm.Human: return -1;  break;
                case DruidForm.GiantSpider: return 0x388; break;
                case DruidForm.Panther: return 0x462; break;
                case DruidForm.Bear: return 0xA3; break;
            }

            return -1;
        }

        public override int GetIdleSound()
        {
            switch (m_DruidForm)
            {
                case DruidForm.Human: return -1; break;
                case DruidForm.GiantSpider: return 0x389; break;
                case DruidForm.Panther: return 0x463; break;
                case DruidForm.Bear: return 0xA4; break;
            }

            return -1;  
        }

        public override int GetAttackSound()
        {
            switch (m_DruidForm)
            {
                case DruidForm.Human: return -1; break;
                case DruidForm.GiantSpider: return 0x38A; break;
                case DruidForm.Panther: return 0x464; break;
                case DruidForm.Bear: return 0xA5; break;
            }

            return -1;
        }

        public override int GetHurtSound()
        {
            switch (m_DruidForm)
            {
                case DruidForm.Human:
                    if (Female)
                        return (Utility.RandomList(0x14B, 0x14C, 0x14D, 0x14E, 0x14F, 0x57E, 0x57B));
                    else
                        return (Utility.RandomList(0x154, 0x155, 0x156, 0x159, 0x589, 0x436, 0x437, 0x43B, 0x43C));
                break;

                case DruidForm.GiantSpider: return 0x38B; break;
                case DruidForm.Panther: return 0x465; break;
                case DruidForm.Bear: return 0xA6; break;
            }

            return -1;
        }

        public override int GetDeathSound()
        {
            switch (m_DruidForm)
            {
                case DruidForm.Human:
                    if (Female)
                        return (Utility.RandomList(0x150, 0x151f, 0x152f, 0x153f, 0x57A, 0x54C, 0x314f, 0x315f, 0x316f, 0x317f));
                    else
                        return (Utility.RandomList(0x15A, 0x15B, 0x15C, 0x15D, 0x5F5, 0x54D, 0x53F, 0x423, 0x424, 0x425, 0x426, 0x427, 0x438));
                break;
                case DruidForm.GiantSpider: return 0x38C; break;
                case DruidForm.Panther: return 0x466; break;
                case DruidForm.Bear: return 0xA7; break;
            }

            return -1;
        }

        public WoodlandDruid(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_DruidForm);
            writer.Write(IsRevertingToHumanForm);

            writer.Write(m_Mobiles.Count);
            for (int a = 0; a < m_Mobiles.Count; a++)
            {
                writer.Write(m_Mobiles[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_DruidForm = (DruidForm)reader.ReadInt();
            IsRevertingToHumanForm = reader.ReadBool();            

            int creatureCount = reader.ReadInt();
            for (int a = 0; a < creatureCount; a++)
            {
                Mobile mobile = reader.ReadMobile();

                if (mobile != null)                
                    m_Mobiles.Add(mobile);                
            }

            ChangeForm(m_DruidForm, true);
        }
    }
}
