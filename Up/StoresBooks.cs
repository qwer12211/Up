//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Up
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoresBooks
    {
        public int ID_StoreBook { get; set; }
        public int Store_ID { get; set; }
        public int Book_ID { get; set; }
        public int StoreBookAmount { get; set; }
    
        public virtual Books Books { get; set; }
        public virtual Stores Stores { get; set; }
    }
}
