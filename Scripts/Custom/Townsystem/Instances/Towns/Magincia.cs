using System;

namespace Server.Custom.Townsystem
{
	public class Magincia : Town
	{
		public Magincia()
		{
            Definition =
                new TownDefinition(
                    3,
                    "Magincia",
                    "Magincia",
                    "MAGINCIA",
                    "The Town Crystal of Magincia",
                    "a Magincia town control brazier",
                    "a Magincia town flag",
                    new Point3D(3736,2163,20),
                    new Point3D[] { new Point3D(3779, 2256, 20), new Point3D(3779, 2257, 20), new Point3D(3779, 2258, 20), new Point3D(3779, 2259, 20), new Point3D(3779, 2260, 20), new Point3D(3779, 2261, 20), new Point3D(3779, 2262, 20) },
                    new Point3D[] { },
                    new Point3D(3778,2259,20),
                    3648,
                    new Point3D(3782,2259,20),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(3777,2239,15), new Point3D(3808,2272,25)) },
                    75
                    );
		}
	}
}