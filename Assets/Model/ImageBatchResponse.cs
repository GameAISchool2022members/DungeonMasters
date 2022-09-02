using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Model
{
    public class ImageBatchResponse
    {
        public string Format { get; set; }
        public List<string> Imgs { get; set; }
        public string Msg { get; set; }
        public List<int> Size { get; set; }

    }
}