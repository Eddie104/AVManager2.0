using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avManager.model.data
{
    class Actress : Data
    {

        public Actress(ObjectId id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }
}
