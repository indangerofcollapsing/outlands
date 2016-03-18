using System;

namespace Server.Custom.Townsystem
{
	public class Cove : Town
	{
		public Cove()
		{
            Definition =
                new TownDefinition(
                    1,
                    "Cove",
                    "Cove",
                    "COVE",
                    "The Town Crystal of Cove",
                    "a Cove town control brazier",
                    "a Cove town flag",
                    new Point3D(2233,1195,0),
                    new Point3D[] {new Point3D(2215,1116,20),new Point3D(2215,1115,20)},
                    new Point3D[] {},
                    new Point3D(2213,1112,40),
                    0xE41,
                    new Point3D(2216,1116,20),
                    new Rectangle3D[] {new Rectangle3D(new Point3D(2207,1111,0),new Point3D(2216,1119,65))},
                    50
                    );
		}
	}
}