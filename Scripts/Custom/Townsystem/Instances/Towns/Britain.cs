using System;

namespace Server.Custom.Townsystem
{
	public class Britain : Town
	{
		public Britain()
		{
            Definition =
                new TownDefinition(
                    0,
                    "Britain",
                    "Britain",
                    "BRITAIN",
                    "The Town Crystal of Britain",
                    "a Britain town control brazier",
                    "a Britain town flag",
                    new Point3D(1425,1693,0),
                    new Point3D[] {},
                    new Point3D[] { new Point3D(1532, 1421, 35), new Point3D(1533, 1421, 35),new Point3D(1534,1421,35)},
                    new Point3D(1533,1412,35),
                    0xE41,
                    new Point3D(1533,1423,35),
                    new Rectangle3D[] {new Rectangle3D(new Point3D(1528,1411,30),new Point3D(1538,1422,55))},
                    300
                    );
		}
	}
}