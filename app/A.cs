using System;
using System.ComponentModel.DataAnnotations;

namespace app
{
    public class A
    {
        public virtual int? Id { get; set; }

        [Required]
        protected virtual int BId { get; set; }

        public virtual B B { get; set; }
    }
}
