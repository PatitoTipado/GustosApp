using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
        public class ReviewRestaurante
    {

 
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Autor { get; set; } = string.Empty;
            public double Rating { get; set; }
            public string Texto { get; set; } = string.Empty;

            public Guid RestauranteId { get; set; }
        }

    }

