using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetGroupBasedPermissions.Test
{
    public class ModelTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ModelTree> Children { get; set; }
    }
}
