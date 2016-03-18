using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public interface ISiegeable
    {
        void RepairOrDelete();

        bool Deleted { get; }
    }
}
