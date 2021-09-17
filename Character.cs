using System;
using System.Collections.Generic;
using System.Text;

namespace InitCalcCLI6
{
    public class Character
    {
        public string name;

        public Character(string name)
        {
           this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
