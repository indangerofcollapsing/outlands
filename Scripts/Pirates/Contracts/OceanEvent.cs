using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class OceanEvent
    {
        public enum OceanEventType
        {
            Stowaway,
            CastAway,
            Mutiny,
            Rats,
            Livestock,
            SpawningGrounds,
            DolphinSchool,
            DeepOnesReach,
            Shipwrecks,
            DerelictCargo,
            Sargasso,
            Storm,
            Pirates,
            Blockade,
            RunAground,
            SailTear,
            GunpowderExplosion,
            FireBreakout
        }

        public static void StartOceanEvent(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
            int eventTypes = Enum.GetNames(typeof(OceanEventType)).Length;

            OceanEventType oceanEventType = (OceanEventType)Utility.RandomMinMax(0, eventTypes - 1);

            switch (oceanEventType)
            {
                case OceanEventType.Stowaway: Stowaway(location, map, difficulty, boat); break;
                case OceanEventType.CastAway: CastAway(location, map, difficulty, boat); break;
                case OceanEventType.Mutiny: Mutiny(location, map, difficulty, boat); break;
                case OceanEventType.Rats: Rats(location, map, difficulty, boat); break;
                case OceanEventType.Livestock: Livestock(location, map, difficulty, boat); break;
                case OceanEventType.SpawningGrounds: SpawningGrounds(location, map, difficulty, boat); break;
                case OceanEventType.DolphinSchool: DolphinSchool(location, map, difficulty, boat); break;
                case OceanEventType.DeepOnesReach: DeepOnesReach(location, map, difficulty, boat); break;
                case OceanEventType.Shipwrecks: ShipWrecks(location, map, difficulty, boat); break;
                case OceanEventType.DerelictCargo: DerelictCargo(location, map, difficulty, boat); break;
                case OceanEventType.Sargasso: Sargasso(location, map, difficulty, boat); break;
                case OceanEventType.Storm: Storm(location, map, difficulty, boat); break;
                case OceanEventType.Pirates: Pirates(location, map, difficulty, boat); break;
                case OceanEventType.Blockade: Blockade(location, map, difficulty, boat); break;
                case OceanEventType.RunAground: RunAground(location, map, difficulty, boat); break;
                case OceanEventType.SailTear: SailTear(location, map, difficulty, boat); break;
                case OceanEventType.GunpowderExplosion: GunpowderExplosion(location, map, difficulty, boat); break;
                case OceanEventType.FireBreakout: FireBreakout(location, map, difficulty, boat); break;
            }
        }

        public static void Stowaway(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void CastAway(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Mutiny(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Rats(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Livestock(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void SpawningGrounds(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void DolphinSchool(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void DeepOnesReach(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void ShipWrecks(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void DerelictCargo(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Sargasso(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Storm(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Pirates(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void Blockade(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void RunAground(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void SailTear(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void GunpowderExplosion(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }

        public static void FireBreakout(Point3D location, Map map, int difficulty, BaseBoat boat)
        {
        }
    }
}