using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avManager.model.data
{
    class Data
    {
        public ObjectId ID { get; set; }

        public string Name { get; set; }

        public bool NeedInsert { get; set; }

        public bool NeedUpdate { get; set; }

        public bool NeedDelete { get; set; }

    }
}
