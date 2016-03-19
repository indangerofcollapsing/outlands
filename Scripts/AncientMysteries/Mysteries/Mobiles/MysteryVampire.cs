using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a vampiric corpse")]
    public class MysteryVampire : BaseCreature
    {
        public List<Mobile> m_Mobiles = new List<Mobile>();

        public DateTime m_RevertForm = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        public TimeSpan RevertFormDelay = TimeSpan.FromSeconds(2);

        public DateTime m_NextFeedAllowed;
        public TimeSpan NextFeedDelay = TimeSpan.FromSeconds(40);

        public DateTime m_NextFeedTick;
        public TimeSpan NextFeedTickDelay = TimeSpan.FromSeconds(2);

        public DateTime m_FeedExpiration;
        public TimeSpan FeedDuration = TimeSpan.FromSeconds(10);

        public bool IsFeeding = false;
        public int feedTick = 0;        

        [Constructable]
        public MysteryVampire(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "an ancient vampire";

            Body = 400;
            Hue = 2500;            

            SetStr(75);
            SetDex(100);
            SetInt(75);

            SetHits(4000);
            SetStam(4000);
            SetMana(4000);

            SetDamage(20, 30);

            AttackSpeed = 40;

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Fencing, 100);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 75;

            Fame = 1500;
            Karma = -1500;
            
            AddItem(new Cloak() { Movable = false, Hue = 1107 });
            AddItem(new PlateGorget() { Movable = false, Hue = 1107 });
            AddItem(new PlateGloves() { Movable = false, Hue = 1107 });
            AddItem(new ShortPants() { Movable = false, Hue = 2500 });
            AddItem(new Boots() { Movable = false, Hue = 1107 });

            Utility.AssignRandomHair(this, 2498);
                        
            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new WarCleaver() { Movable = false, Speed = 40, Hue = 2498, Name = "a vampiric blade" }); break;
                case 2: AddItem(new RadiantScimitar() { Movable = false, Speed = 40, Hue = 2498, Name = "a vampiric blade" }); break;
                case 3: AddItem(new NoDachi() { Movable = false, Speed = 40, Hue = 2498, Name = "a vampiric blade" }); break;
            }

            //-----

            m_AncientMysteryCreature = true;
        }

        public override bool AlwaysBoss { get { return true; } }
        
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.75;
                        
            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 18;

            DictCombatAction[CombatAction.AttackOnly] = 5;
            DictCombatAction[CombatAction.CombatSpell] = 0;
            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            SpellDelayMin *= 4;
            SpellDelayMax *= 4;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .10 && Body != 317)
                SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 0.5, 20, -1, true, "", "", "*begins to strike with unnatural quickness*");            
            
            if (Utility.RandomDouble() <= .10 && m_NextFeedAllowed <= DateTime.UtcNow && Body != 317)
            {
                IsFeeding = true;

                m_NextFeedAllowed = DateTime.UtcNow + NextFeedDelay;
                m_FeedExpiration = DateTime.UtcNow + FeedDuration;

                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 10, true, 0, false, "", "");                

                FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

                Blood blood = new Blood();
                blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);

                double damage = DamageMax * 6;

                if (defender is BaseCreature)
                    damage *= 1.5;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*begins to feed*");

                Animate(34, 5, 1, true, false, 0);

                SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 10, -1, true, "", "");
                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 10.0, 0x44D, true, "", "The creature sinks its fangs into you, ensnaring and feeding upon your blood!");
            }            
        }

        public override void OnThink()
        {
            base.OnThink();

            if ((Paralyzed || CantWalk || Frozen) && !IsFeeding)
            {
                Effects.PlaySound(Location, Map, 0x657);
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                Name = "a vampire bat";

                Body = 317;
                Hue = 2105;
                BaseSoundID = 0x642;

                Paralyzed = false;
                CantWalk = false;
                Frozen = false;

                DictCombatAction[CombatAction.AttackOnly] = 5;
                DictCombatAction[CombatAction.CombatSpell] = 0;
                DictCombatAction[CombatAction.CombatHealSelf] = 0;

                m_RevertForm = DateTime.UtcNow + RevertFormDelay;

                return;
            }

            if (m_RevertForm < DateTime.UtcNow && Body == 317)
            {
                Effects.PlaySound(Location, Map, 0x657);
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                Name = "an ancient vampire";

                Body = 400;
                Hue = 2500;

                DictCombatAction[CombatAction.AttackOnly] = 5;
                DictCombatAction[CombatAction.CombatSpell] = 15;
                DictCombatAction[CombatAction.CombatHealSelf] = 0;

                return;
            }

            if (IsFeeding)
            {
                if (m_FeedExpiration < DateTime.UtcNow)                
                    IsFeeding = false;                

                else if (m_NextFeedTick < DateTime.UtcNow)
                {
                    m_NextFeedTick = DateTime.UtcNow + NextFeedTickDelay;

                    int healingAmount = (int)((double)HitsMax * .02);

                    Hits += healingAmount;

                    new Blood().MoveToWorld(Location, Map);

                    for (int a = 0; a < 4; a++)
                    {
                        new Blood().MoveToWorld(new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z), Map);
                    }

                    PlaySound(Utility.RandomList(0x5DA));
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*feeds*");

                    Animate(34, 5, 1, true, false, 0);

                    return;
                }
            }           
        }

        public override int GetAngerSound() { return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481); }
        public override int GetIdleSound() { return Utility.RandomList(0x47D, 0x47E, 0x47F, 0x480, 0x481); }
        public override int GetHurtSound() { return 0x5F9; }
        public override int GetDeathSound() { return 0x5F5; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.AddItem(new VampireCoffin());

            int treasurePileLevel = 1;

            if (Utility.RandomMinMax(1, 3) == 1)
                treasurePileLevel = 2;

            if (Utility.RandomMinMax(1, 6) == 1)
                treasurePileLevel = 3;

            if (Utility.RandomMinMax(1, 10) == 1)
                treasurePileLevel = 4;

            switch (treasurePileLevel)
            {
                case 1: c.AddItem(new TreasurePileSmallAddonDeed()); break;
                case 2: c.AddItem(new TreasurePileMediumAddonDeed()); break;
                case 3: c.AddItem(new TreasurePileLargeAddonDeed()); break;
                case 4: c.AddItem(new TreasurePileHugeAddonDeed()); break;
            }

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (m_Mobiles[a].Alive)
                        m_Mobiles[a].Kill();
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }
        }

        public MysteryVampire(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            //Version 0
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

            //Version 0
            if (version >= 0)
            {
                m_Mobiles = new List<Mobile>();

                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();
                    m_Mobiles.Add(creature);
                }
            }
        }
    }
}