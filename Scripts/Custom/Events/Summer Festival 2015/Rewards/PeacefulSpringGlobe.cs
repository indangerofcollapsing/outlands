using System; 
using Server; 
using Server.Network;
using Server.Spells;

namespace Server.Items 
{ 
   public class PeacefulSpringGlobe : Item 
   {
      public override bool AlwaysAllowDoubleClick { get { return true; } }

      public DateTime m_NextUseAllowed;
      public TimeSpan UsageCooldown = TimeSpan.FromMinutes(30);

      public int BranchItemId { get { return Utility.RandomList(3387, 3388, 3889); } }
      public int MushroomItemId { get { return Utility.RandomList(3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353); } }
      public int GrassItemId { get { return Utility.RandomList(3220, 3255, 3256, 3223, 3245, 3246, 3248, 3254, 3257, 3258, 3259, 3260, 3261, 3269, 3270, 3267, 3237, 3267, 3239, 3332); } }
      public int FlowerItemId { get { return Utility.RandomList(3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213, 3214, 3262, 3263, 3264, 3265, 6809, 6810, 6811); } }
       
      [Constructable] 
      public PeacefulSpringGlobe() : base( 0xE2F ) 
      { 
         Name = "a globe depicting a peaceful scene of a forest"; 

         Weight = 5;
         Hue = 2542;
      } 

      public override void OnDoubleClick( Mobile from ) 
      {
          if (from == null)
              return;

          if (!from.Alive)
          {
              from.SendMessage("You must be alive to use that.");
              return;
          }

          if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
          {
              from.SendMessage("You are too far away to use that.");
              return;
          }

          if (m_NextUseAllowed > DateTime.UtcNow && from.AccessLevel == AccessLevel.Player)
          {
              from.SendMessage("You may only use this item once every 30 minutes.");
              return;
          }

         GenerateVision(from);
      }

      public void GenerateVision(Mobile from)
      {
        if (this == null) return;
        if (from == null) return;

        m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;

        from.SendMessage("You see a peaceful scene of a forest nearby a small meadow.");

        Point3D location = from.Location;
        Map map = from.Map;
        Effects.PlaySound(location, map, 0x4);

        int range = 2;

        int minRange = -1 * range;
        int maxRange = range + 1;

        for (int a = minRange; a < maxRange; a++)
        {
            for (int b = minRange; b < maxRange; b++)
            {
                if (Utility.RandomDouble() <= .75)
                {
                    Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);
                    SpellHelper.AdjustField(ref newLocation, map, 12, false);

                    int itemId = Utility.RandomList(3219, 3220, 3255, 3256, 3152, 3153, 3223, 6809, 6811, 3204, 3247, 3248, 3254, 3258, 3259, 3378,
                                3267, 3237, 3267, 9036, 3239, 3208, 3307, 3310, 3311, 3313, 3314, 3332, 3271, 3212, 3213);

                    int duration = 10;

                    TimedStatic meadowItem = new TimedStatic(itemId, duration);
                    meadowItem.Name = "a peaceful vision";
                    meadowItem.MoveToWorld(newLocation, map);
                }

                //Alternate Handling
                /*
                if (Utility.RandomDouble() <= .75)
                {
                    double itemResult = Utility.RandomDouble();

                    if (itemResult <= .2)
                    {
                        meadowItem.Name = "branches";
                        itemId = BranchItemId;
                    }

                    else if (itemResult <= .4)
                    {
                        meadowItem.Name = "mushroom";
                        itemId = MushroomItemId;
                    }

                    else
                    {
                        meadowItem.Name = "mushroom";
                        itemId = FlowerItemId;
                    }
                }
                */                
            }
        }
      }

      public PeacefulSpringGlobe(Serial serial) : base(serial) {}

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 
         writer.Write( (int) 0 ); // version 

         writer.Write(m_NextUseAllowed);
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 
         int version = reader.ReadInt();

         m_NextUseAllowed = reader.ReadDateTime();
      } 
   } 
}