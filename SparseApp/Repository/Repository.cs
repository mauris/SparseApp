using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace SparseApp.Repository
{
    [ProtoContract]
    class Repository
    {
        [ProtoMember(1)]
        public string Path { get; set; };
    }
}
