using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderAutoSkuData
{
    public class AutoCarParam
    {
        public string message { get; set; }
        public ParamResult result { get; set; }
        public string returncode { get; set; }
    }

    public class ParamResult
    {
        public string specid { get; set; }

        public List<paramtypeitem> paramtypeitems { get; set; }
    }

    public class paramtypeitem
    {
        public string name { get; set; }
        public List<paramitem> paramitems { get;set;}
    }
    public class paramitem
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
