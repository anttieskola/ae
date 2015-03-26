using System;
using AE.EF.Dal;
using System.Threading.Tasks;

namespace AE.Funny.Dal
{
    public class FunnyRepository : BasicRepository
    {
        public FunnyRepository()
            : base(new FunnyContext())
        {
        }
    }
}
