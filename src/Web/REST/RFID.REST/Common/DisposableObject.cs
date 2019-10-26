namespace RFID.REST.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class DisposableObject : IDisposable
    {
        public static readonly IDisposable Empty = new DisposableObject();

        private DisposableObject() { }

        public void Dispose()
        {
            
        }
    }
}
