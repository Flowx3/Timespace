using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSpace
{
    public class MapDataDTO
    {
        public short Height { get; set; }
        public short Width { get; set; }
        public byte[] Grid { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
