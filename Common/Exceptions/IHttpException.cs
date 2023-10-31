using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    internal interface IHttpException
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        
    }
}
