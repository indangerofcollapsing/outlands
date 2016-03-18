using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Server;
using Server.Items;
using Server.Spells;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    public abstract class LoHEvent
    {
        public virtual Type CreatureType { get { return typeof(LoHSandWyvern); } }
        public virtual string DisplayName { get { return "Sand Wyvern"; } }

        public virtual string AnnouncementText { get { return "The Sand Wyvern has risen from the shifting sands of the northeast Britain desert!"; } }

        public virtual int RewardItemId { get { return 0; } }
        public virtual int RewardHue { get { return 0; } }        

        public virtual Point3D MonsterLocation { get { return new Point3D(0, 0, 0); } }
        public virtual Point3D PortalLocation { get { return new Point3D(0, 0, 0); } }

        //-----

        public LoHMonster m_LoHCreature;
        public int m_PlayersAtStartOfEvent;
        
        public void Despawn()
        {
            if (m_LoHCreature != null)
                m_LoHCreature.Delete();

            m_LoHCreature = null;
        }

        public void Spawn(BaseCreature.OnBeforeDeathCB onkilled)
        {
            LoHMonster bc_Creature = (LoHMonster)Activator.CreateInstance(CreatureType);

            if (bc_Creature != null)
            {
                m_LoHCreature = bc_Creature;
                m_LoHCreature.m_OnBeforeDeathCallback = onkilled;                

                Region region = Region.Find(MonsterLocation, Map.Felucca);
                Debug.Assert(region != Map.Felucca.DefaultRegion);

                m_PlayersAtStartOfEvent = Math.Max(1, region.GetPlayerCount(true));

                m_LoHCreature.m_PlayersAtStartOfLoHEvent = m_PlayersAtStartOfEvent;
                m_LoHCreature.ConfigureCreature();

                m_LoHCreature.MoveToWorld(MonsterLocation, Map.Felucca);
                m_LoHCreature.Tamable = false;

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!SpecialAbilities.Exists(m_LoHCreature))
                        return;

                    m_LoHCreature.SpawnComplete();
                });
            }
        }
        
        #region Generate Loot

        public virtual void GenerateLoot()
        {
            for (int i = 0; i < 5; ++i)
            {
                m_LoHCreature.PackGem(4, 8);
            }

            int packs = m_PlayersAtStartOfEvent / 5;

            if (packs > 3) packs = 3;
            if (packs <= 0) packs = 1;

            m_LoHCreature.AddLoot(LootPack.SuperBoss, packs);

            m_LoHCreature.PackCraftingComponent(10, 0.5);
            m_LoHCreature.PackPrestigeScroll(5, 0.5);
            m_LoHCreature.PackResearchMaterials(2, 0.5);
            m_LoHCreature.PackSpellHueDeed(1, .25);

            foreach (DamageEntry de in m_LoHCreature.DamageEntries)
            {
                BaseCreature bc = de.Damager as BaseCreature;
                PlayerMobile pm = de.Damager as PlayerMobile;

                if (bc != null && pm == null)
                {
                    if (bc.Summoned)
                        pm = bc.SummonMaster as PlayerMobile;

                    if (bc.Controlled)
                        pm = bc.ControlMaster as PlayerMobile;
                }

                if (pm != null)
                {
                    Item ss = SkillScroll.Generate(pm, 120.0, 2);

                    if (ss != null)
                    {
                        pm.Backpack.AddItem(ss);
                        pm.PrivateOverheadMessage(MessageType.Emote, 0, true, "* a skill scroll magically appears in your backpack *", pm.NetState);
                    }

                    if (Utility.RandomDouble() < 0.10)
                    {
                        Item reward = new Gold(500);

                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1:
                                BaseWeapon weapon = Loot.RandomWeapon();

                                if (weapon == null)
                                    break;

                                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(2, 5);
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(2, 5);
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(2, 5);
                                reward = weapon;
                                break;

                            case 2:
                                BaseArmor armor = Loot.RandomArmorOrShield();

                                if (armor == null)
                                    break;

                                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(1, 5);
                                armor.Durability = (ArmorDurabilityLevel)Utility.RandomMinMax(1, 5);
                                reward = armor;
                                break;

                            case 3:
                                reward = new RareTitleDye();
                                break;

                            case 4:
                                reward = new RareCloth();
                                break;
                        }

                        if (reward != null)
                        {
                            pm.Backpack.AddItem(reward);
                            pm.PrivateOverheadMessage(MessageType.Emote, 0, true, "* you receive special loot from slaying the beast *", pm.NetState);
                        }
                    }
                }
            }

            Region r = Region.Find(MonsterLocation, Map.Felucca);

            int playercount = 3;

            if (r != Map.Felucca.DefaultRegion)
                playercount = Math.Max(1, r.GetPlayerCount(true));

            Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerStateCallback<int[]>(GenerateGoldPiles), new int[3] { m_LoHCreature.X, m_LoHCreature.Y, playercount });
        }

        #endregion
        
        #region Gold Piles

        private static void GenerateGoldPiles(int[] para)
        {
            int base_x = para[0];
            int base_y = para[1];
            int playercount = para[2];

            for (int x = -5; x <= 5; ++x)
            {
                for (int y = -5; y <= 5; ++y)
                {
                    double sqdist = x * x + y * y;

                    if (sqdist <= 5 * 5) // 
                        GenerateGoldpile(base_x + x, base_y + y, playercount);
                }
            }
        }

        private static void GenerateGoldpile(int x, int y, int playercount)
        {
            Map map = Map.Felucca;

            int z = map.GetAverageZ(x, y);
            bool canFit = map.CanFit(x, y, z, 6, false, false);

            for (int i = -3; !canFit && i <= 3; ++i)
            {
                canFit = map.CanFit(x, y, z + i, 6, false, false);

                if (canFit)
                    z += i;
            }

            if (!canFit)
                return;

            Gold g = new Gold(350, 750);

            g.MoveToWorld(new Point3D(x, y, z), map);

            if (0.5 >= Utility.RandomDouble())
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: // Fire column
                    {
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                        Effects.PlaySound(g, g.Map, 0x208);
                        break;
                    }

                    case 2: // Explosion
                    {
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                        Effects.PlaySound(g, g.Map, 0x307);
                        break;
                    }

                    case 3: // Ball of fire
                    {
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
