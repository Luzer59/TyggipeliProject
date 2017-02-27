using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Player
{
    public Player(uint id)
    {
        this.id = id;
    }

    public uint id = 0;
    public bool ready = false;
    public float pos = 0;
}
