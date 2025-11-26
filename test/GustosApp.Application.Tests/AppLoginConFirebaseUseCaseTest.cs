using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace GustosApp.Application.Tests
    {
        public class AppLoginConFirebaseUseCaseTest
        {
            private const string TestUid = "TEST_FIREBASE_UID";
            private const string TestToken = "valid_fake_token";
            private const string TestEmail = "test@user.com";
            private const string TestNombre = "TestName";
            private const string TestApellido = "TestLast";

            // --- Helper para simular el Token ---
            private DecodedToken CreateFakeDecodedToken(Dictionary<string, object> claims = null)
            {
                var mockToken = new Mock<DecodedToken>();

                mockToken.SetupGet(t => t.Uid).Returns(TestUid);

                mockToken.SetupGet(t => t.Claims).Returns(claims ?? new Dictionary<string, object>
            {
                { "email", TestEmail },
                { "nombre", TestNombre },
                { "apellido", TestApellido },
                { "direccion", "TestAddress" }
            });
                return mockToken.Object;
            }

            [Fact]
            public async Task HandleAsyncCrearRestaurante_DebeRetornarRestauranteExistente()
            {
                var mockAuthService = new Mock<IFirebaseAuthService>(); 
                var mockUsuarioRepo = new Mock<IUsuarioRepository>();
                var mockRestauranteRepo = new Mock<IRestauranteRepository>();
                var restauranteExistente = new Restaurante { PropietarioUid = TestUid, Nombre = "Existente" };

                var fakeDecodedToken = CreateFakeDecodedToken();
                mockAuthService.Setup(s => s.VerifyIdTokenAsync(TestToken)).ReturnsAsync(fakeDecodedToken);

                mockRestauranteRepo.Setup(r => r.GetByFirebaseUidAsync(
                    TestUid,
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(restauranteExistente);

                var casoDeUso = new AppLoginConFirebaseUseCase(
                    mockUsuarioRepo.Object,
                    mockRestauranteRepo.Object,
                    mockAuthService.Object 
                );

                var resultado = await casoDeUso.HandleAsyncCrearRestaurante(TestToken);

                Assert.Equal(TestUid, resultado.firebaseUid);
                Assert.Equal("Restaurante", resultado.tipoUsuario);

                mockRestauranteRepo.Verify(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()), Times.Never);
                mockRestauranteRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task HandleAsyncCrearRestaurante_DebeCrearRestauranteNuevoSiNoExiste()
            {
                var mockAuthService = new Mock<IFirebaseAuthService>(); 
                var mockUsuarioRepo = new Mock<IUsuarioRepository>();
                var mockRestauranteRepo = new Mock<IRestauranteRepository>();

                var fakeDecodedToken = CreateFakeDecodedToken();
                mockAuthService.Setup(s => s.VerifyIdTokenAsync(TestToken)).ReturnsAsync(fakeDecodedToken);

                mockRestauranteRepo.Setup(r => r.GetByFirebaseUidAsync(
                    TestUid,
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync((Restaurante)null!);

                Restaurante restauranteCapturado = null;
                mockRestauranteRepo.Setup(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()))
                                   .Callback<Restaurante, CancellationToken>((r, ct) => restauranteCapturado = r)
                                   .Returns(Task.CompletedTask);

                mockRestauranteRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                                   .Returns(Task.CompletedTask);

                var casoDeUso = new AppLoginConFirebaseUseCase(
                    mockUsuarioRepo.Object,
                    mockRestauranteRepo.Object,
                    mockAuthService.Object 
                );

                var resultado = await casoDeUso.HandleAsyncCrearRestaurante(TestToken);

                Assert.Equal(TestUid, resultado.firebaseUid);
                Assert.Equal("Restaurante", resultado.tipoUsuario);

                mockRestauranteRepo.Verify(r => r.AddAsync(It.IsAny<Restaurante>(), It.IsAny<CancellationToken>()), Times.Once);
                mockRestauranteRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

                Assert.NotNull(restauranteCapturado);
                Assert.Equal(TestUid, restauranteCapturado.PropietarioUid);
                Assert.Equal(TestNombre, restauranteCapturado.Nombre);
            }

            [Fact]
            public async Task HandleAsyncCrearUsuario_DebeRetornarUsuarioExistente()
            {
                var mockAuthService = new Mock<IFirebaseAuthService>(); 
                var mockUsuarioRepo = new Mock<IUsuarioRepository>();
                var mockRestauranteRepo = new Mock<IRestauranteRepository>();
                var usuarioExistente = new Usuario { FirebaseUid = TestUid, Nombre = "Existente" };

                var fakeDecodedToken = CreateFakeDecodedToken();
                mockAuthService.Setup(s => s.VerifyIdTokenAsync(TestToken)).ReturnsAsync(fakeDecodedToken);

                mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(
                    TestUid,
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(usuarioExistente);

                var casoDeUso = new AppLoginConFirebaseUseCase(
                    mockUsuarioRepo.Object,
                    mockRestauranteRepo.Object,
                    mockAuthService.Object 
                );

                var resultado = await casoDeUso.HandleAsyncCrearUsuario(TestToken);

                Assert.Equal(TestUid, resultado.firebaseUid);
                Assert.Equal("Usuario", resultado.tipoUsuario);

                mockUsuarioRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
                mockUsuarioRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            }

        [Fact]
            public async Task HandleAsyncCrearUsuario_DebeCrearUsuarioNuevoSiNoExiste()
            {
                var mockAuthService = new Mock<IFirebaseAuthService>(); 
                var mockUsuarioRepo = new Mock<IUsuarioRepository>();
                var mockRestauranteRepo = new Mock<IRestauranteRepository>();

                var fakeDecodedToken = CreateFakeDecodedToken();
                mockAuthService.Setup(s => s.VerifyIdTokenAsync(TestToken)).ReturnsAsync(fakeDecodedToken);

                mockUsuarioRepo.Setup(r => r.GetByFirebaseUidAsync(
                    TestUid,
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync((Usuario)null!);

                Usuario usuarioCapturado = null;
                mockUsuarioRepo.Setup(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                               .Callback<Usuario, CancellationToken>((u, ct) => usuarioCapturado = u)
                               .Returns(Task.CompletedTask);

                mockUsuarioRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                               .Returns(Task.CompletedTask);

                var casoDeUso = new AppLoginConFirebaseUseCase(
                    mockUsuarioRepo.Object,
                    mockRestauranteRepo.Object,
                    mockAuthService.Object 
                );

                var resultado = await casoDeUso.HandleAsyncCrearUsuario(TestToken);

                Assert.Equal(TestUid, resultado.firebaseUid);
                Assert.Equal("Usuario", resultado.tipoUsuario);

                mockUsuarioRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
                mockUsuarioRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

                Assert.NotNull(usuarioCapturado);
                Assert.Equal(TestUid, usuarioCapturado.FirebaseUid);
                Assert.Equal(TestEmail, usuarioCapturado.Email);
                Assert.Equal(PlanUsuario.Free, usuarioCapturado.Plan);
            }
        }
    }





