using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterfaceSHPPagtoSocio.RootCobranca
{
    public class RootCobranca
    {
        public Cobranca Cobranca { get; set; }
    }
    public class Cobranca
    {
        public string origem { get; set; }
        public string name { get; set; }
        public string titulo { get; set; }
        public string socio { get; set; }
        public string dependente { get; set; }
        public string data { get; set; }
        public string item { get; set; }
        public string valor { get; set; }
        public string localidade { get; set; }
        public string centrocusto { get; set; }
        public string origemCreate { get; set; }
    }
}