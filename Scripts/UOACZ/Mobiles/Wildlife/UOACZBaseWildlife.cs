using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;


namespace Server.Mobiles
{
	public class UOACZBaseWildlife : BaseCreature
	{
        public static List<UOACZBaseWildlife> m_Creatures = new List<UOACZBaseWildlife>();

        public static int CorruptHue = 2210;

        private bool m_Corrupted = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrupted
        {
            get { return m_Corrupted; }
            set { m_Corrupted = value; }
        }

        public UOACZWildlifeSpawner m_Spawner;

        public virtual bool AlwaysFlee { get { return true; } }

        public virtual string CorruptedName { get { return "a corrupted animal"; } }
        public virtual string CorruptedCorpseName { get { return "a corrupted animal corpse"; } }

        public virtual double CorruptedDamageScalar { get { return 1.5; } }
        public virtual double CorruptedAttackSpeedScalar { get { return 1.5; } }
        public virtual double CorruptedWrestlingScalar { get { return .9; } }

        public virtual double CrudeBoneArmorDropChance { get { return .05; } }

        public override bool AlwaysFreelyLootable { get { return true; } }

        public override bool AllowParagon { get { return false; } }
        public override bool CanRummageCorpses { get { return false; } }

        public override Poison HitPoison
        {
            get
            {
                if (m_Corrupted)
                    return Poison.Regular;

                else
                    return null;
            }
        }

        public override Poison PoisonImmune
        {
            get
            {
                if (m_Corrupted)
                    return Poison.Lethal;

                else
                    return null;
            } 
        }
        
        [Constructable]
		public UOACZBaseWildlife() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{
            m_Creatures.Add(this);
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            if (m_Corrupted)
                SetCorruptAI();

            else
            {
                if (AlwaysFlee)
                {
                    DictCombatTargeting[CombatTargeting.UOACZHumanPlayer] = 1;
                    DictCombatTargeting[CombatTargeting.UOACZUndead] = 1;
                    DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] = 1;
                    DictCombatTargeting[CombatTargeting.UOACZEvilWildlife] = 1;

                    DictCombatRange[CombatRange.Withdraw] = 100;
                    CreatureWithdrawRange = 30;

                    RangePerception = 18;
                    DefaultPerceptionRange = 18;
                }

                else
                {
                    DictCombatTargeting[CombatTargeting.UOACZUndead] = 1;
                    DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] = 1;
                    DictCombatTargeting[CombatTargeting.UOACZEvilWildlife] = 1;

                    DictCombatRange[CombatRange.Withdraw] = 0;
                    CreatureWithdrawRange = 8;

                    RangePerception = 12;
                    DefaultPerceptionRange = 12;
                }
            }

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.5;
        }

        public virtual void SetCorruptAI()
        {
            RangePerception = 18;
            DefaultPerceptionRange = 18;

            DictCombatTargeting[CombatTargeting.UOACZUndead] = 0;
            DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] = 0;
            DictCombatTargeting[CombatTargeting.UOACZWildlife] = 1;
            DictCombatTargeting[CombatTargeting.UOACZHuman] = 2;
            DictCombatTargeting[CombatTargeting.UOACZHumanPlayer] = 3;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            DictCombatRange[CombatRange.Withdraw] = 0;
            CreatureWithdrawRange = 0;

            SetSkill(SkillName.Poisoning, 20);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (Corrupted)
                return;

            PlayerMobile pm_From = from as PlayerMobile;
            UOACZBaseUndead bc_Undead = from as UOACZBaseUndead;

            if (pm_From != null)
            {
                if (pm_From.IsUOACZUndead)                
                    damage = (int)(Math.Ceiling((double)damage * UOACZSystem.UndeadPlayerToNonCorruptedWildlifeDamageScalar));                
            }

            if (bc_Undead != null)
                damage = (int)(Math.Ceiling((double)damage * UOACZSystem.UndeadPlayerToNonCorruptedWildlifeDamageScalar));
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Corrupted)
                return;

            if (!UOACZSystem.IsUOACZValidMobile(this)) return;
            if (!UOACZSystem.IsUOACZValidMobile(attacker)) return;

            double conversionChance = .25;
            bool undeadAttacker = false;

            BaseCreature bc_Attacker = attacker as BaseCreature; 
            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            if (pm_Attacker != null)
            {
                if (pm_Attacker.IsUOACZUndead)
                {
                    conversionChance = UOACZSystem.WildlifePlayerCorruptChance;
                    undeadAttacker = true;
                }
            }

            if (bc_Attacker != null)
            {
                if (!bc_Attacker.Controlled)
                    return;

                if (bc_Attacker is UOACZBaseUndead)
                {
                    conversionChance = UOACZSystem.WildlifeCreatureCorruptChance;
                    undeadAttacker = true;

                    if (bc_Attacker.ControlMaster is PlayerMobile)
                    {
                        PlayerMobile controlMaster = bc_Attacker.ControlMaster as PlayerMobile;

                        if (controlMaster.IsUOACZUndead && Utility.GetDistance(Location, controlMaster.Location) <= 36)
                            pm_Attacker = controlMaster;
                    }
                }
            }            

            if (Utility.RandomDouble() <= conversionChance && undeadAttacker)
            {
                if (pm_Attacker != null)
                {
                    pm_Attacker.SendMessage("You corrupt the creature.");

                    UOACZSystem.ChangeStat(pm_Attacker, UOACZSystem.UOACZStatType.UndeadScore, UOACZSystem.UndeadPlayerCorruptWildlifeScore, true);

                    if (Utility.RandomDouble() < UOACZSystem.WildlifeCorruptionStoneChance * UOACZPersistance.UndeadBalanceScalar)
                    {
                        if (pm_Attacker.Backpack != null)
                        {
                            pm_Attacker.Backpack.DropItem(new UOACZCorruptionStone(pm_Attacker));
                            pm_Attacker.SendMessage(UOACZSystem.greenTextHue, "You have received a corruption stone for corrupting wildlife.");
                        }
                    }

                    if (Utility.RandomDouble() < UOACZSystem.WildlifeUpgradeTokenChance * UOACZPersistance.UndeadBalanceScalar)
                    {
                        if (pm_Attacker.Backpack != null)
                        {
                            pm_Attacker.Backpack.DropItem(new UOACZUndeadUpgradeToken(pm_Attacker));
                            pm_Attacker.SendMessage(UOACZSystem.greenTextHue, "You have received an upgrade token for corrupting wildlife.");
                        }
                    }

                    if (Utility.RandomDouble() < UOACZSystem.WildlifeRewardChance * UOACZPersistance.UndeadBalanceScalar)
                    {
                        if (pm_Attacker.Backpack != null)
                        {
                            pm_Attacker.Backpack.DropItem(new UOACZBrains());
                            pm_Attacker.SendMessage(UOACZSystem.greenTextHue, "You have received a reward for corrupting wildlife.");
                        }
                    }
                }

                IPooledEnumerable m_MobilesNearby = Map.GetMobilesInRange(Location, 30);
                
                foreach (Mobile mobile in m_MobilesNearby)
                {
                    if (!UOACZSystem.IsUOACZValidMobile(mobile))
                        continue;

                    if (mobile.Combatant != null)
                    {
                        if (mobile.Combatant == this)
                            mobile.Combatant = null;
                    }

                    mobile.RemoveAggressor(this);
                    mobile.RemoveAggressed(this);
                }

                m_MobilesNearby.Free();

                TurnCorrupted(attacker);
            }
        }

        public virtual void TurnCorrupted(Mobile from)
        {
            if (!UOACZSystem.IsUOACZValidMobile(from)) return;
            if (Corrupted) return;

            if (from is PlayerMobile)
            {
                PlayerMobile player = from as PlayerMobile;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.WildlifeCorrupted++;
            }       

            UOACZEvents.SpreadCorruption();

            Combatant = null;

            Aggressed.Clear();
            Aggressors.Clear();

            Warmode = false;

            m_Corrupted = true;
            Hue = CorruptHue;

            Name = CorruptedName;
            CorpseNameOverride = CorruptedCorpseName;

            PublicOverheadMessage(MessageType.Regular, 2210, false, "*becomes corrupted*");

            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2530, 0, 2023, 0);

            TimedStatic greenGoo = new TimedStatic(0x122D, 5);
            greenGoo.Name = "corruption";
            greenGoo.Hue = 2530;
            greenGoo.MoveToWorld(Location, Map);

            PlaySound(GetAngerSound());
            PlaySound(0x62B);

            for (int a = 0; a < 4; a++)
            {
                greenGoo = new TimedStatic(Utility.RandomList(4651, 4652, 4653, 4654), 5);
                greenGoo.Name = "corruption";
                greenGoo.Hue = 2530;
                
                Point3D targetLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Y);
                SpellHelper.AdjustField(ref targetLocation, Map, 12, false);

                greenGoo.MoveToWorld(targetLocation, Map);
            }

            SetCorruptAI();

            DamageMin = (int)(Math.Round((double)DamageMin * CorruptedDamageScalar));
            DamageMax = (int)(Math.Round((double)DamageMax * CorruptedDamageScalar));

            AttackSpeed = (int)(Math.Round((double)AttackSpeed * CorruptedAttackSpeedScalar));

            Skills.Wrestling.Base *= CorruptedWrestlingScalar;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() <= CrudeBoneArmorDropChance)
            {
                if (Utility.RandomDouble() <= .4)
                {
                    BaseWeapon weapon = UOACZSystem.GetRandomCrudeBoneWeapon();

                    if (weapon != null)
                        c.AddItem(weapon);
                }

                else
                {
                    BaseArmor armor = UOACZSystem.GetRandomCrudeBoneArmor();

                    if (armor != null)
                        c.AddItem(armor);
                }
            }
        }

        public virtual void UOACZCarve(Mobile from, Corpse corpse)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            from.Animate(32, 3, 1, true, false, 0);
            Effects.PlaySound(from.Location, from.Map, 0x3E3);

            new Blood(0x122D).MoveToWorld(corpse.Location, corpse.Map);
            corpse.Carved = true;

            from.SendMessage("You carve the corpse.");

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            player.m_UOACZAccountEntry.WildlifeSkinned++;    
        }

        public override void OnCarve(Mobile from, Corpse corpse)
        {
            UOACZCarve(from, corpse);            
        }

        public override void OnThink()
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Mobiles.Remove(this);
        }

        public UOACZBaseWildlife(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_Corrupted);
            writer.Write(m_Spawner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Corrupted = reader.ReadBool();
                m_Spawner = (UOACZWildlifeSpawner)reader.ReadItem();
            }

            //---------------

            m_Creatures.Add(this);
		}
	}
}
