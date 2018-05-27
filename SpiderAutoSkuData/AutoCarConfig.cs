using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderAutoSkuData
{
    public class AutoCarConfig
    {
        public string message { get; set; }
        public ConfigResult result { get; set; }
        public string returncode { get; set; }
    }
    public class ConfigResult
    {
        public string specid { get; set; }

        public List<configtypeitem> configtypeitems { get; set; }
    }

    public class configtypeitem
    {
        public string name { get; set; }
        public List<configitem> configitems { get; set; }
    }
    public class configitem
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
