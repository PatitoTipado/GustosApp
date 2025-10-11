using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class SugerirGustosUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IRestauranteRepository _restaurantRepo;

        // Estos valores deberían ser cargados desde la configuración (API/Infra), no hardcodeados aquí.
        private const double UmbralMinimo = 0.1;
        private const double FactorPenalizacion = 0.1;

        // Constructor que recibe las dependencias (Inversión de Control)
        public SugerirGustosUseCase(IEmbeddingService embeddingService, IRestauranteRepository restaurantRepo)
        {
            _embeddingService = embeddingService;
            _restaurantRepo = restaurantRepo;
        }

        // Método que ejecuta el caso de uso ,agrego el 3 parametro: CancellationToken ct = default
        public async Task<List<RecomendacionDTO>> Handle(List<string> gustosUsuario, int maxResults = 10, CancellationToken ct = default)

        //grupos -> id y cada grupo va a tener 
        //grupos - gustos 
        {
            // 1. Obtención de datos (Usando repositorios) 
            // tmb deberiamos incluir en la query la exclusion de no match con restricciones y gustos
            //var restaurantes = getRestaurant();
            var restaurantes = await _restaurantRepo.GetAllAsync(ct);
            var resultados = new List<(Restaurante restaurante, double score)>();

            //obtenemos al usuario del cual queremos obtener su match
            //consulta por id para obtener los gustos

            // var gustoUsuario = string.Join(" ", new List<string> { "carne a la parrilla", "asado", "parrilla" });

            // 2. Generar el Embedding del usuario (llamada a la interfaz IEmbeddingService)
            // El Use Case NO sabe que esto usa ONNX.
            // var userEmb = _embeddingService.GetEmbedding(string.Join(" ", gustoUsuario));
            //  var gustosUsuario = gustoUsuario.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userEmb = _embeddingService.GetEmbedding(string.Join(" ", gustosUsuario));


            foreach (var rest in restaurantes)
            {
                // A. Embeddings del restaurante (llamada al servicio de Infraestructura)
                //string especialidadesTexto = string.Join(" ", rest.Especialidad);
                //var baseEmb = _embeddingService.GetEmbedding(especialidadesTexto);
                double maxScoreBase = 0; // Inicializamos el score máximo del restaurante
                // Iteramos sobre CADA especialidad para encontrar la MEJOR coincidencia
                foreach (var especialidad in rest.Especialidad)
                {
                    // Nota: Aquí se usa solo el nombre de la especialidad, no la cadena gigante.
                    var baseEmb = _embeddingService.GetEmbedding(especialidad.Nombre);
                    // Calcular la Similitud con el usuario (userEmb se calculó fuera del loop)
                    double scoreSimilitud = CosineSimilarity.Coseno(userEmb, baseEmb);
                    // Usar el score MÁS ALTO encontrado para este restaurante
                    if (scoreSimilitud > maxScoreBase)
                    {
                        maxScoreBase = scoreSimilitud;
                    }
                }
                double scoreBase = maxScoreBase;

                // D. Cálculo de la Penalización (Lógica de Aplicación/Negocio)
                double penalizacion = 0;
                // ... (Tu lógica de penalización usando rest.Especialidad y gustosUsuario) ...
                foreach (var gusto in gustosUsuario)
                {
                    // Si el restaurante no tiene ninguna especialidad que coincida con el gusto del usuario
                    //habria que plantear capaz con lo que menos gustos coincida ahora si
                    if (!rest.Especialidad.Any(e => e.Nombre != null && e.Nombre.ToLower().Contains(gusto.ToLower())))
                    {
                        penalizacion += FactorPenalizacion;
                    }
                }

                // E. Score final (Lógica de Aplicación/Negocio)
               // double scoreBase = CosineSimilarity.Coseno(userEmb, baseEmb);
                // double scoreFinal = ((scoreBase) / 2) * (1 - penalizacion);
                double scoreFinal = scoreBase * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    resultados.Add((rest, scoreFinal));
                }
            }
            // 3. Mapeo y Retorno
            return resultados
                .OrderByDescending(x => x.score)
                .Take(maxResults)
                .Select(x => new RecomendacionDTO { RestaurantId = x.restaurante.Id, Score = x.score })
                .ToList();

        }
    }
}
    

    
        /*public static List<Restaurant> getRestaurant()
        {
            return new List<Restaurant>
        {
        new Restaurant(101, new List<string>{"carne","parrilla","asado","pizza","pasta italiana","chimichurri"}),
        new Restaurant(102, new List<string>{"sushi","ramen","nigiri","rolls","comida japonesa","cortes de carne"}),
        new Restaurant(103, new List<string>{"vegetariano","vegano","ensaladas","smoothies","bowls veganos","opciones saludables"}),
        new Restaurant(104, new List<string>{"pizza","pizza vegetariana","masa fina","ingredientes frescos","variedad de pizzas"})
        };
        }*/
        /*public static List<UserPreference> ObtenerUsuariosEjemplo()
        {
            return new List<UserPreference>
        {
            new UserPreference(1, new List<string>{"carne a la parrilla", "asado", "parrilla"},new List<string>{"celiaco"}),
            new UserPreference(2, new List<string>{"sushi", "platos japoneses", "comida japonesa", "rolls"} ,new List<string>{"gluten"}),
            new UserPreference(3,new List<string>{"vegetariano", "comida saludable", "ensaladas", "smoothies"},
            new List<string>{"carne", "celiaco", "gluten"}),
            new UserPreference(4, new List<string>{"pizza", "pasta italiana", "masa fina"},
            new List<string>{"gluten", "harinas", "carne", "sushi", "vegano"})
        };
        }*/

    

    /*
 * esta es una chanchada que hago que luego la tenes que reemplazar para poder pegarle a los repos de verdad
 */

    /*
    public class Restaurant
    {

        public int Id { get; set; }
        public List<string> Especialidad { get; set; }

        public Restaurant(int id, List<string> especialidad)
        {
            Id = id;
            Especialidad = especialidad ?? new List<string>();
        }
        public override string ToString()
        {
            string resultado = "";
            for (int i = 0; i < Especialidad.Count(); i++)
            {
                resultado += Especialidad[i] + " ,";
            }

            return resultado;
        }
    }

    class UserPreference
    {
        public int Id { get; set; }

        public List<string> Gustos { get; set; } // Ej: "Me gusta la carne a la parrilla"
        public List<string> Restricciones { get; set; } // Ej: ["vegano", "sin gluten"]

        public UserPreference(int id, List<string> gustos, List<string> restricciones)
        {
            Id = id;
            Gustos = gustos ?? new List<string>();
            Restricciones = restricciones ?? new List<string>();
        }
    }

}*/

