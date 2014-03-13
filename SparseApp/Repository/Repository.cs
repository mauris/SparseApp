using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;

namespace SparseApp.Repository
{
    [ProtoContract]
    public class Repository
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public List<string> Plugins { get; set; }

        public string Basename
        {
            get
            {
                FileInfo fi = new FileInfo(this.Path);
                return fi.Name;
            }
        }
    }
}
