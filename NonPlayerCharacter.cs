using System;
using System.Collections.Generic;
using System.Text;

namespace InitCalcCLI6
{
    public class NonPlayerCharacter : Character
    {
        public int bonus;

        public NonPlayerCharacter(string name, int bonus) : base(name)
        {
            this.bonus = bonus;
        }

    }
}
