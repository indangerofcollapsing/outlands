using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Achievements;

namespace Server.Items
{
    public class UOACZBrains : Item
    {
        public enum TypeOfBrain
        {
            Healing,
            Mana,    
            CooldownReduction,
            SwarmHeal,            
        }

        private TypeOfBrain m_BrainType = TypeOfBrain.Healing;
        [CommandProperty(AccessLevel.GameMaster)]
        public TypeOfBrain BrainsType
        {
            get { return m_BrainType; }
            set
            { 
                m_BrainType = value;

                switch (m_BrainType)
                {
                    case TypeOfBrain.Healing:
                        Hue = 2117;
                    break;

                    case TypeOfBrain.Mana:
                        Hue = 1366;
                    break;

                    case TypeOfBrain.CooldownReduction:
                        Hue = 2527;
                    break;

                    case TypeOfBrain.SwarmHeal:
                        Hue = 2958;
                    break;                    
                }
            }
        }

        [Constructable]
        public UOACZBrains(): base(7408)
        {
            Name = "brains";
            Weight = 1;

            BrainsType = (TypeOfBrain)Utility.RandomMinMax(0, 3);
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (player.IsUOACZUndead)
                return true;

            return false;
        }

        public UOACZBrains(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            switch (m_BrainType)
            {
                case TypeOfBrain.Healing:
                    LabelTo(from, "(healing)");
                break;

                case TypeOfBrain.Mana:
                    LabelTo(from, "(mana restore)");
                break;

                case TypeOfBrain.SwarmHeal:
                    LabelTo(from, "(swarm heal)");
                break;

                case TypeOfBrain.CooldownReduction:
                    LabelTo(from, "(ability cooldown)");
                break;
            }
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            if (player.IsUOACZHuman)
            {
                player.SendMessage("You decide against consuming this.");
                return;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (DateTime.UtcNow < player.m_UOACZAccountEntry.UndeadProfile.NextUndeadItemAllowed)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.UndeadProfile.NextUndeadItemAllowed, false, false, false, true, true);
                player.SendMessage("You may not use another undead item for " + timeRemaining + ".");

                return;
            }

            player.m_UOACZAccountEntry.UndeadProfile.NextUndeadItemAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(UOACZSystem.UndeadItemCooldownSeconds);

            player.PlaySound(Utility.RandomList(0x5DA, 0x5A9, 0x5AB, 0x03A, 0x03B, 0x03C));

            TypeOfBrain brainType = m_BrainType;

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                switch (brainType)
                {
                    case TypeOfBrain.Healing:
                        int hitsRegained = (int)(Math.Round((double)player.HitsMax * .33));
                        player.Heal(hitsRegained);

                        player.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                        player.PlaySound(0x202);
                    break;

                    case TypeOfBrain.Mana:
                        int manaRegained = (int)(Math.Round((double)player.ManaMax * .66));
                        player.Mana += manaRegained;

                        player.FixedParticles(0x376A, 9, 32, 5030, 1364, 0, EffectLayer.Waist);
                        player.PlaySound(0x1EB);
                    break;

                    case TypeOfBrain.SwarmHeal:
                        Queue m_Queue = new Queue();

                        foreach (BaseCreature follower in player.AllFollowers)
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(follower)) continue;
                            if (Utility.GetDistance(player.Location, follower.Location) > 12) continue;
                            if (follower.Hits == follower.HitsMax) continue;

                            m_Queue.Enqueue(follower);
                        }
                    
                        while (m_Queue.Count > 0)
                        {
                            BaseCreature follower = (BaseCreature)m_Queue.Dequeue();

                            int healingAmount = (int)(Math.Round((double)follower.HitsMax * .25));
                            
                            follower.Heal(healingAmount);

                            follower.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                            follower.PlaySound(0x202);
                        }
                    break;

                    case TypeOfBrain.CooldownReduction:
                        player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                        player.PlaySound(0x3BD);

                        foreach (UOACZUndeadAbilityEntry abilityEntry in player.m_UOACZAccountEntry.UndeadProfile.m_Abilities)
                        {
                            DateTime cooldown = abilityEntry.m_NextUsageAllowed;
                           
                            double cooldownReduction = abilityEntry.m_CooldownMinutes * .20;

                            abilityEntry.m_NextUsageAllowed = abilityEntry.m_NextUsageAllowed.Subtract(TimeSpan.FromMinutes(cooldownReduction));
                            
                        }

                        UOACZSystem.RefreshAllGumps(player);
                    break;
                }
            });

            AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZConsumeBrains, 1);

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version     

            writer.Write((int)m_BrainType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_BrainType = (TypeOfBrain)reader.ReadInt();
        }
    }
}