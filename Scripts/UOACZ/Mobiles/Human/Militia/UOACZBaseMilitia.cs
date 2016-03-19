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
    public class UOACZBaseMilitia : UOACZBaseHuman
    {
        public static List<UOACZBaseMilitia> m_Creatures = new List<UOACZBaseMilitia>();

        public UOACZMilitiaSpawner m_Spawner;

        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.None; } }
        public override double StockpileContributionScalar { get { return 0.5; } }

        public override int DifficultyValue { get { return 5; } }

        public virtual int PrimaryHue { get { return 2405; } }
        public virtual int SecondaryHue { get { return 2408; } }

        public override int MaxDistanceAllowedFromHome { get { return 50; } }

        [Constructable]
        public UOACZBaseMilitia(): base()
        {
            Title = "the militia";

            int hairHue = Utility.RandomHairHue();

            if (Female)
                Utility.AssignRandomHair(this, hairHue);
            else
            {
                if (Utility.RandomDouble() <= .90)
                    Utility.AssignRandomHair(this, hairHue);
            }

            if (!Female && Utility.RandomDouble() <= .5)
                Utility.AssignRandomFacialHair(this, hairHue);

            m_Creatures.Add(this);
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ActiveSpeed = 0.35;
            PassiveSpeed = 0.45;

            SpecialAbilities.PhalanxSpecialAbility(1.0, null, this, 1.0, 50000, 0, false, "", "", "-1");

            DictCombatTargeting[CombatTargeting.UOACZEvilHumanPlayer] = 2;

            DictCombatAction[CombatAction.AttackOnly] = 15;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;
           
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;
                     
            DictWanderAction[WanderAction.None] = 2;      
            DictWanderAction[WanderAction.PotionCureSelf] = 1;

            SetSkill(SkillName.Healing, 40);
        }

        public override void OnThink()
        {
            base.OnThink();           

            Point3D location = Location;
            Map map = Map;

            //Outside of Valid Combat Zone
            if (Utility.GetDistance(Home, location) > MaxDistanceAllowedFromHome)
            {
                TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                dirt.Name = "dirt";
                dirt.MoveToWorld(location, map);

                dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*returns to post*");

                Effects.PlaySound(location, map, 0x657);

                int projectiles = 6;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Z);
                    SpellHelper.AdjustField(ref newLocation, map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
                }

                Location = Home;
                Combatant = null;

                return;
            }

            if (Combatant == null && Sentry)
            {
                if (Utility.RandomDouble() <= .02)
                    FaceRandomDirection();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Creatures.Contains(this))
                m_Creatures.Remove(this);

            if (m_Spawner != null)
                m_Spawner.m_Mobiles.Remove(this);
        }

        public UOACZBaseMilitia(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Spawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Spawner = (UOACZMilitiaSpawner)reader.ReadItem();
            }
        }
    }
}
