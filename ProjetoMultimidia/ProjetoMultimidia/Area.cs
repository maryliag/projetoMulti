using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoMultimidia
{
    class Area
    {
        float x1;
        float x2;
        float z1;
        float z2;

        public Area(float x1, float x2, float z1, float z2)
        {
            if (x1 < x2)
            {
                this.x1 = x1;
                this.x2 = x2;
            }
            else
            {
                this.x1 = x2;
                this.x2 = x1;
            }

            if (z1 < z2)
            {
                this.z1 = z1;
                this.z2 = z2;
            }
            else
            {
                this.z1 = z2;
                this.z2 = z1;
            }
        }

        public Boolean isInArea(float x, float z)
        {
            if (x >= this.x1 && x <= this.x2)
            {
                if (z >= this.z1 && z <= this.z2)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
