using System;

namespace Server.Custom.Townsystem
{
    public class SerpentsHold : Town
    {
        public SerpentsHold()
        {
            Definition =
                new TownDefinition(
                    8,
                    "Serpent's Hold",
                    "Serpent's Hold",
                    "SERPENT'S HOLD",
                    "The Town Crystal of Serpent's Hold",
                    "a Serpent's Hold town control brazier",
                    "a Serpent's Hold town flag",
                    new Point3D(2878, 3464, 15),
                    new Point3D[] { },
                    new Point3D[] { new Point3D(2949, 3346, 15), new Point3D(2950, 3346, 15) },
                    new Point3D(2949, 3344, 15),
                    3649,
                    new Point3D(2949, 3350, 15),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(2933, 3341, 10), new Point3D(2968, 3367, 65)) },
                    75
                    );
        }
    }
}
