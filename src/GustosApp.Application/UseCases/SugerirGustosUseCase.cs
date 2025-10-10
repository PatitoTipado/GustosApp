using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;

namespace GustosApp.Application.UseCases
{
    public class SugerirGustosUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        //private readonly IRestaurantRepository _restaurantRepo;

        // Estos valores deberían ser cargados desde la configuración (API/Infra), no hardcodeados aquí.
        private const double UmbralMinimo = 0.1;
        private const double FactorPenalizacion = 0.1;

        // Constructor que recibe las dependencias (Inversión de Control)
        public SugerirGustosUseCase(
            IEmbeddingService embeddingService
            //IRestaurantRepository restaurantRepo
            )
        {
            _embeddingService = embeddingService;
            //_restaurantRepo = restaurantRepo;
        }

        // Método que ejecuta el caso de uso
        public List<RecomendacionDTO> Handle(int id=0, int maxResults = 10)
            //grupos -> id y cada grupo va a tener 
            //grupos - gustos 
        {
            // 1. Obtención de datos (Usando repositorios) 
            // tmb deberiamos incluir en la query la exclusion de no match con restricciones y gustos
            var restaurantes = getRestaurant();

            //obtenemos al usuario del cual queremos obtener su match
            //consulta por id para obtener los gustos

            var gustosTexto = string.Join(" ", new List<string> { "carne a la parrilla", "asado", "parrilla" });

            // 2. Generar el Embedding del usuario (llamada a la interfaz IEmbeddingService)
            // El Use Case NO sabe que esto usa ONNX.
            var userEmb = _embeddingService.GetEmbedding(gustosTexto);
            var gustosUsuario = gustosTexto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var resultados = new List<(Restaurant restaurant, double score)>();

            foreach (var rest in restaurantes)
            {
                // A. Embeddings del restaurante (llamada al servicio de Infraestructura)
                string especialidadesTexto = string.Join(" ", rest.Especialidad);
                var baseEmb = _embeddingService.GetEmbedding(especialidadesTexto);

                // D. Cálculo de la Penalización (Lógica de Aplicación/Negocio)
                double penalizacion = 0;
                // ... (Tu lógica de penalización usando rest.Especialidad y gustosUsuario) ...
                foreach (var gusto in gustosUsuario)
                {
                    // Si el restaurante no tiene ninguna especialidad que coincida con el gusto del usuario
                    //habria que plantear capaz con lo que menos gustos coincida ahora si
                    if (!rest.Especialidad.Any(especialidad => especialidad.ToLower().Contains(gusto)))
                    {
                        penalizacion += FactorPenalizacion;
                    }
                }

                // E. Score final (Lógica de Aplicación/Negocio)
                double scoreBase = CosineSimilarity.Coseno(userEmb, baseEmb);
                double scoreFinal = ((scoreBase) / 2) * (1 - penalizacion);

                if (scoreFinal >= UmbralMinimo)
                {
                    resultados.Add((rest, scoreFinal));
                }
            }
            // 3. Mapeo y Retorno
            return resultados
                .OrderByDescending(x => x.score)
                .Select(x => new RecomendacionDTO { RestaurantId = x.restaurant.Id, Score = x.score })
                .ToList();
        }
        public static List<Restaurant> getRestaurant()
        {
            return new List<Restaurant>
        {
        new Restaurant(101, new List<string>{"carne","parrilla","asado","pizza","pasta italiana","chimichurri"}),
        new Restaurant(102, new List<string>{"sushi","ramen","nigiri","rolls","comida japonesa","cortes de carne"}),
        new Restaurant(103, new List<string>{"vegetariano","vegano","ensaladas","smoothies","bowls veganos","opciones saludables"}),
        new Restaurant(104, new List<string>{"pizza","pizza vegetariana","masa fina","ingredientes frescos","variedad de pizzas"})
        };
        }
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

    }

    /*
 * esta es una chanchada que hago que luego la tenes que reemplazar para poder pegarle a los repos de verdad
 */
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

}

