using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;

namespace Server.Custom.Mobiles.Evolvables
{
    public abstract class BaseEvolvable : BaseCreature
    {
        private List<CreatureStage> _creatureStages;
        private int _currentStage;

        public List<CreatureStage> CreateStages
        {
            get { return _creatureStages; }
            set { _creatureStages = value; }
        }

        public int CurrentStage
        {
            get { return _currentStage; }
            set { _currentStage = value; }
        }

        [Constructable]
        protected BaseEvolvable(AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
            : base(ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
        {

        }

        public BaseEvolvable(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CreatureStage
    {
        public int StageId { get; set; }
        public string Name { get; set; }
        public int Hue { get; set; }
        public Body Body { get; set; }
        public int BaseSoundID { get; set; }
    }
}
