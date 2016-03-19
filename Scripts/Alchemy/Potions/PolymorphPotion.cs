using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class PolymorphPotion : BasePotion
	{
        [Constructable]
		public PolymorphPotion() : base( 0xF0D, PotionEffect.Custom )
		{
            Name = "a monsterous polymorph potion";
            Hue = 2600;

            Weight = 1.0;
            Movable = true;
		}

		public PolymorphPotion( Serial serial ) : base( serial )
		{
		}

        public override void Drink(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (this != null && ParentEntity != from.Backpack)
                from.SendMessage("The potion must be in your pack to drink it.");

            else
            {
                if (!from.CanBeginAction(typeof(PolymorphSpell)))
                {
                    from.SendMessage("You are already under the influence of an polymorph effect.");
                    return;
                }

                if (player.LastPlayerCombatTime + player.PlayerCombatExpirationDelay > DateTime.UtcNow)
                {
                    from.SendMessage("You have been in combat with another player too recently to drink this.");
                    return;
                }

                if (DisguiseTimers.IsDisguised(from))
                    from.SendMessage("You cannot can drink that while being disguised.");

                if (KinPaint.IsWearingKinPaint(from))
                {
                    from.SendMessage("You cannot can drink that while wearing kin paint.");
                    return;
                }

                if (!from.CanBeginAction(typeof(IncognitoSpell)) || from.IsBodyMod)
                {
                    from.SendMessage("You cannot can drink that while under the effect of a modification spell, ability, or item.");
                    return;
                }

                from.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                from.PlaySound(0x3BD);
                from.Animate(34, 5, 1, true, false, 0);

                BasePotion.PlayDrinkEffect(from);

                List<int> m_PossibleBodyValues = new List<int>();

                m_PossibleBodyValues.Add(1); //Ogre
                m_PossibleBodyValues.Add(2); //Ettin
                m_PossibleBodyValues.Add(3); //Zombie
                m_PossibleBodyValues.Add(4); //Gargoyle
                m_PossibleBodyValues.Add(5); //Orc Captain
                m_PossibleBodyValues.Add(6); //Corpser
                m_PossibleBodyValues.Add(9); //Daemon
                m_PossibleBodyValues.Add(17); //Orc
                m_PossibleBodyValues.Add(22); //Gazer                
                m_PossibleBodyValues.Add(26); //Ghost
                m_PossibleBodyValues.Add(30); //Harpy
                m_PossibleBodyValues.Add(31); //Headless
                m_PossibleBodyValues.Add(35); //Lizardman
                m_PossibleBodyValues.Add(39); //Mongbat
                m_PossibleBodyValues.Add(45); //Ratman
                m_PossibleBodyValues.Add(57); //Skeleton
                m_PossibleBodyValues.Add(53); //Troll
                m_PossibleBodyValues.Add(70); //Terathan Warrior
                m_PossibleBodyValues.Add(71); //Terathan Drone
                m_PossibleBodyValues.Add(72); //Terathan Queen
                m_PossibleBodyValues.Add(75); //Cyclops
                m_PossibleBodyValues.Add(82); //Lich
                m_PossibleBodyValues.Add(85); //Ophidian Mage
                m_PossibleBodyValues.Add(86); //Ophidian Warrior
                m_PossibleBodyValues.Add(87); //Ophidian Queen
                m_PossibleBodyValues.Add(154); //Mummy
                m_PossibleBodyValues.Add(285); //Treestalk
                m_PossibleBodyValues.Add(301); //Ent
                m_PossibleBodyValues.Add(303); //Devourer
                m_PossibleBodyValues.Add(304); //Flesh Golem
                m_PossibleBodyValues.Add(305); //Ore Golem
                m_PossibleBodyValues.Add(306); //Hook Horror
                m_PossibleBodyValues.Add(309); //Patchwork Skeleton
                m_PossibleBodyValues.Add(312); //Myconid

                from.BodyMod = m_PossibleBodyValues[Utility.RandomMinMax(0, m_PossibleBodyValues.Count - 1)];
                from.HueMod = 0;

                BaseArmor.ValidateMobile(from);

                int duration = 300;

                from.BeginAction(typeof(PolymorphPotion));
                Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                {
                    from.EndAction(typeof(PolymorphPotion));
                });

                from.BeginAction(typeof(PolymorphSpell));
                Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate { ChangeBack(from); });

                Consume();
            }
        }

        public void ChangeBack(Mobile from)
        {
            from.BodyMod = 0;
            from.HueMod = -1;
            from.EndAction(typeof(PolymorphSpell));

            BaseArmor.ValidateMobile(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
	}
}