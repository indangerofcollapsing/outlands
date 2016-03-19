using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Commands;

namespace Server.Custom
{
    public class LuniteOreFormation : BreakableStatic
    {
        public LuniteOreFormationSpawner m_LuniteOreFormationSpawner;

        [Constructable]
        public LuniteOreFormation(): base()
        {
            Name = "a lunite ore formation";

            Visible = true;

            InteractionRange = 1;
            InteractionDelay = 5;
            MinInteractDamage = 5;
            MaxInteractDamage = 10;

            InteractDamageScalar = 0.20;
            WeaponDamageScalar = 0.20;
            LockpickDamageScalar = 0;
            ObjectBreakingDeviceDamageScalar = 0.20;
            MiningDamageScalar = 1.0;
            LumberjackingDamageScalar = 0.01;

            MaxHitPoints = 2000;
            HitPoints = 2000;            

            AddLOSBlocker = false;            
            
            HitSound = 0x125;

            ItemID = 6002;
            Hue = 2603;

            NormalItemId = 6002;
            NormalHue = 2603;

            LightlyDamagedPercent = .666;
            LightlyDamagedSound = 0x3B4;
            LightlyDamagedItemId = 6001;
            LightlyDamagedHue = 2603;

            HeavilyDamagedPercent = .333;
            HeavilyDamagedSound = 0x3B4;
            HeavilyDamagedItemId = 6004;
            HeavilyDamagedHue = 2603;

            BrokenSound = 0x221;
            BrokenItemId = 6001;
            BrokenHue = 2603;        

            DeleteOnBreak = true;
            CreateTimedStaticAfterBreak = false;

            RevealNearbyHiddenItemsOnBreak = false;
            RevealNearbyHiddenItemsOnBreakRadius = 0;
            RefreshNearbyMovables = false;
            RefreshNearbyMovablesRadius = 0;
        }

        public override void Interact(Mobile from, BreakableStatic.InteractionType interactionType)
        {
            if (from.Skills.Mining.Value < 100)
            {
                from.SendMessage("You are not skilled enough in mining in order to harvest this.");
                return;
            }

            base.Interact(from, interactionType);
        }

        public override void AfterInteract(Mobile from, InteractionType interactionType)
        {
            TimedStatic ore = new TimedStatic(6583, 5);
            ore.Name = "lunite fragments";
            ore.Hue = 2603;

            Point3D oreLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
            SpellHelper.AdjustField(ref oreLocation, Map, 12, false);
            ore.MoveToWorld(oreLocation, Map);
        }

        public override void BeforeBreak(Mobile from, InteractionType interactionType)
        {
            int fragments = Utility.RandomMinMax(2, 3);
            for (int a = 0; a < fragments; a++)
            {
                TimedStatic ore = new TimedStatic(6583, 5);
                ore.Name = "lunite fragments";
                ore.Hue = 2603;

                Point3D oreLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref oreLocation, Map, 12, false);
                ore.MoveToWorld(oreLocation, Map);
            }

            if (m_LuniteOreFormationSpawner != null)            
                m_LuniteOreFormationSpawner.ResetTimer();            

            Point3D location = Location;
            Map map = Map;

            Effects.PlaySound(location, map, 0x5CA);
            Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2602, 0, 5029, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                int amount = Utility.RandomMinMax(3, 5);

                LuniteOre ore = new LuniteOre(amount);
                ore.MoveToWorld(location, map);
            });

            base.BeforeBreak(from, interactionType);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            
            if (m_LuniteOreFormationSpawner != null)
            {
                if (m_LuniteOreFormationSpawner.m_LuniteOreFormations.Contains(this))
                    m_LuniteOreFormationSpawner.m_LuniteOreFormations.Remove(this);                
            }

            Delete();
        }

        public LuniteOreFormation(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            //Version 1
            writer.Write(m_LuniteOreFormationSpawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version1
            if (version >= 1)
            {
                m_LuniteOreFormationSpawner = (LuniteOreFormationSpawner)reader.ReadItem();
            }

            //---------
        }
    }
}
