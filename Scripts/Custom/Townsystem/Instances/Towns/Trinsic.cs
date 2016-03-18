using System;

namespace Server.Custom.Townsystem
{
	public class Trinsic : Town
	{
        public Trinsic()
		{
            Definition =
                new TownDefinition(
                    10,
                    "Trinsic",
                    "Trinsic",
                    "TRINSIC",
                    "The Town Crystal of Trinsic",
                    "a Trinsic town control brazier",
                    "a Trinsic town flag",
                    new Point3D(1909,2684,0),
                    new Point3D[] {new Point3D(1902,2729,20)},
                    new Point3D[] {},
                    new Point3D(1900,2735,20),
                    3648,
                    new Point3D(1900,2729,20),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(1899, 2725, 20), new Point3D(1911, 2739, 40)) },
                    150
                    );
		}
	}
}