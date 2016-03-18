namespace Server.Items
{
    public interface IHideable : IPoint2D
    {
        int HideLevel { get; set; }
        bool Findable { get; set; }
        Mobile Finder { get; set;  }
        bool IsVisible { get; set; }
        void Reveal(Mobile from);
    }
}

/*
 * 
 *      private int m_HideLevel;
        private bool m_Findable;
        private Mobile m_Finder;

        [CommandProperty(AccessLevel.GameMaster)]
        public int HideLevel 
        { 
            get { return m_HideLevel; }
            set { m_HideLevel = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Findable
        {
            get { return m_Findable; }
            set { m_Findable = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Finder { 
            get { return m_Finder; }
            set { m_Finder = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Visible
        {
            get { return Visible; }
            set { Visible = value; }
        }

        public void Reveal(Mobile from)
        {
            this.Visible = true;
        }
 * 
 */
