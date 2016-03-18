using System;

namespace Server.Custom.Townsystem
{
	public class SkaraBrae : Town
	{
        public SkaraBrae()
		{
            Definition =
                new TownDefinition(
                    9,
                    "Skara Brae",
                    "Skara Brae",
                    "SKARA BRAE",
                    "The Town Crystal of Skara Brae",
                    "a Skara Brae town control brazier",
                    "a Skara Brae town flag",
                    new Point3D(589,2152,0),
                    new Point3D[] {new Point3D(610,2125,0),new Point3D(610,2126,0),new Point3D(610,2127,0),new Point3D(610,2128,0)},
                    new Point3D[] {new Point3D(608,2128,0),new Point3D(609,2128,0),new Point3D(610,2128,0),new Point3D(608,2124,0),new Point3D(609,2124,0),new Point3D(610,2124,0)},
                    new Point3D(608,2126,0),
                    3648,
                    new Point3D(613,2126,0),
					new Rectangle3D[] { new Rectangle3D(new Point3D(607, 2119, 0), new Point3D(632, 2136, 20)), new Rectangle3D(new Point3D(615, 2111, 0), new Point3D(624, 2144, 20)) },
                    100
                    );
		}
	}
}