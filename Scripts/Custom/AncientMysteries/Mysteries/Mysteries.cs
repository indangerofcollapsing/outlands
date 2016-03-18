using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.Regions;
using Server.ContextMenus;
using Server.Spells;
using Server.Custom;
using Server.Mobiles;

namespace Server.Custom
{
    public class Mysteries
    {
        public static void Daemon(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Daemon);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            Point3D daemonLocation = new Point3D(mysteryPoint.X + 2, mysteryPoint.Y, mysteryPoint.Z);

            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Daemon, from);
            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);

            MysteryDaemon mysteryDaemon = new MysteryDaemon();            
            mysteryDaemon.MoveToWorld(daemonLocation, from.Map);
            mysteryLocation.m_Mobiles.Add(mysteryDaemon);
        }

        public static void Fountain(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Fountain);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            Point3D fountainLocation = new Point3D(mysteryPoint.X + 5, mysteryPoint.Y, mysteryPoint.Z);

            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Fountain, from);
            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);
            
            MysteryFountain mysteryFountain = new MysteryFountain();
            mysteryFountain.MoveToWorld(fountainLocation, from.Map);
            mysteryLocation.m_Items.Add(mysteryFountain);
        }

        public static void Medusa(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Medusa);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            Point3D MedusaLocation = new Point3D(mysteryPoint.X + 2, mysteryPoint.Y, mysteryPoint.Z);

            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Medusa, from);
            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);

            MysteryMedusa mysteryMedusa = new MysteryMedusa();            
            mysteryMedusa.MoveToWorld(MedusaLocation, from.Map);
            mysteryLocation.m_Mobiles.Add(mysteryMedusa);
        }

        public static void Mongbat(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Mongbat);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Mongbat, from);

            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);

            List<Point3D> m_MobileLocations = new List<Point3D>();

            m_MobileLocations.Add(new Point3D(1365, 496, 1));
            m_MobileLocations.Add(new Point3D(1370, 496, 1));
            m_MobileLocations.Add(new Point3D(1375, 496, 1));
            m_MobileLocations.Add(new Point3D(1375, 501, 1));
            m_MobileLocations.Add(new Point3D(1375, 506, 1));
            m_MobileLocations.Add(new Point3D(1370, 506, 1));
            m_MobileLocations.Add(new Point3D(1365, 506, 2));
            m_MobileLocations.Add(new Point3D(1365, 501, 1));
            m_MobileLocations.Add(new Point3D(1367, 498, 1));
            m_MobileLocations.Add(new Point3D(1373, 498, 1));
            //m_MobileLocations.Add(new Point3D(1373, 504, 1));
            //m_MobileLocations.Add(new Point3D(1367, 504, 1));            

            for (int a = 0; a < m_MobileLocations.Count; a++)
            {
                MysteryMongbat mysteryMongbat = new MysteryMongbat();                
                mysteryMongbat.MoveToWorld(m_MobileLocations[a], from.Map);
                mysteryLocation.m_Mobiles.Add(mysteryMongbat);

                //Assign One Mongbat as Master Mongbat for Loot Purposes
                if (a == 0)                
                    mysteryMongbat.IsMasterMongbat = true;                
            } 
        }

        public static void SphinxMystery(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Sphinx);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Sphinx, from);

            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);

            List<Point3D> m_MobileLocations = new List<Point3D>();

            m_MobileLocations.Add(new Point3D(1854, 957, -1));
            m_MobileLocations.Add(new Point3D(1858, 956, -1));
            m_MobileLocations.Add(new Point3D(1861, 958, -1));
            m_MobileLocations.Add(new Point3D(1864, 961, -1));
            m_MobileLocations.Add(new Point3D(1861, 966, -1));
            m_MobileLocations.Add(new Point3D(1857, 968, -1));
            m_MobileLocations.Add(new Point3D(1854, 966, -1));
            m_MobileLocations.Add(new Point3D(1852, 962, -1));

            List<MysterySphinx> m_MysterySphinx = new List<MysterySphinx>();

            for (int a = 0; a < m_MobileLocations.Count; a++)
            {
                MysterySphinx mysterySphinx = new MysterySphinx();                
                mysterySphinx.MoveToWorld(m_MobileLocations[a], from.Map);
                mysteryLocation.m_Mobiles.Add(mysterySphinx);
               
                m_MysterySphinx.Add(mysterySphinx);

                CustomizationAbilities.Vanish(mysterySphinx);
            }

            for (int a = 0; a < m_MysterySphinx.Count; a++)
            {
                MysterySphinx mysterySphinx = m_MysterySphinx[a];

                foreach (MysterySphinx duplicate in m_MysterySphinx)
                {
                    mysterySphinx.m_Duplicates.Add(duplicate);
                }
            }

            MysterySphinx masterSphinx = m_MysterySphinx[Utility.RandomMinMax(0, m_MysterySphinx.Count - 1)];
            masterSphinx.m_MasterSphinx = true;            
        }

        public static void Vampire(Mobile from)
        {
            AncientMystery.MysteryTypeDetail mysteryTypeDetail = AncientMystery.GetMysteryDetails(AncientMystery.MysteryType.Vampire);

            if (mysteryTypeDetail == null || from == null)
                return;

            Point3D mysteryPoint = mysteryTypeDetail.m_MysteryLocation;
            Point3D VampireLocation = new Point3D(mysteryPoint.X + 1, mysteryPoint.Y, mysteryPoint.Z);

            MysteryLocation mysteryLocation = new MysteryLocation(AncientMystery.MysteryType.Vampire, from);
            mysteryLocation.ItemID = 16142;
            mysteryLocation.Name = "an ancient's vampire's coffin";            
            mysteryLocation.MoveToWorld(mysteryPoint, from.Map);
            mysteryLocation.Visible = true;
            

            for (int a = 0; a < 3; a++)
            {
                VampireBat vampireBat = new VampireBat();
                Point3D batLocation = VampireLocation;
                //Point3D batLocation = new Point3D(VampireLocation.X + Utility.RandomList(-1, 1), VampireLocation.Y + Utility.RandomList(-1, 1), VampireLocation.Z);
                
                vampireBat.Murderer = true;
                vampireBat.ResolveAcquireTargetDelay = 1.0;
                vampireBat.RangePerception = 18;
                vampireBat.MoveToWorld(batLocation, from.Map);
                mysteryLocation.m_Mobiles.Add(vampireBat);

                Effects.PlaySound(batLocation, from.Map, 0x657);                
            }

            MysteryVampire mysteryVampire = new MysteryVampire();
            mysteryVampire.Body = 317;
            mysteryVampire.Hue = 2105;
            mysteryVampire.Name = "a vampire bat";
            mysteryVampire.MoveToWorld(VampireLocation, from.Map);
            mysteryLocation.m_Mobiles.Add(mysteryVampire);

            int projectiles = 6;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(VampireLocation.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), VampireLocation.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), VampireLocation.Z);
                SpellHelper.AdjustField(ref newLocation, from.Map, 12, false);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(VampireLocation.X, VampireLocation.Y, VampireLocation.Z + 5), from.Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), from.Map);

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
            } 

            Effects.PlaySound(VampireLocation, from.Map, 0x657);
            Effects.SendLocationParticles(EffectItem.Create(VampireLocation, from.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);
        }
    }    
}
