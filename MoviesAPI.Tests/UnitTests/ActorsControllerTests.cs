using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MoviesAPI.Services;
using System.Text;

namespace MoviesAPI.Tests.UnitTests;
[TestClass]
public class ActorsControllerTests : BasePruebas {
    [TestMethod]
    public async Task ObtenerActoresPaginadas() {
        // Preparar
        var nombreBd = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreBd);
        var mapper = ConfigurarAutoMapper();

        context.Actors.Add(new Actor() { Name = "Actor 1" });
        context.Actors.Add(new Actor() { Name = "Actor 2" });
        context.Actors.Add(new Actor() { Name = "Actor 3" });
        await context.SaveChangesAsync();

        var context2 = ConstruirContext(nombreBd);
        var controller = new ActorsController(context2, mapper, null);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var page1 = await controller.Get(new PaginationDTO() { Page = 1, RecordsPerPage = 2 });
        var actorsPage1 = page1.Value;
        Assert.AreEqual(2, actorsPage1.Count());

        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page2 = await controller.Get(new PaginationDTO() { Page = 2, RecordsPerPage = 2 });
        var actorsPage2 = page2.Value;
        Assert.AreEqual(1, actorsPage2.Count);

        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        var page3 = await controller.Get(new PaginationDTO() { Page = 3, RecordsPerPage = 2 });
        var actorsPage3 = page3.Value;
        Assert.AreEqual(0, actorsPage3.Count);
    }

    [TestMethod]
    public async Task CrearActorSinFoto() {
        // Preparar
        var nombreBd = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreBd);
        var mapper = ConfigurarAutoMapper();

        var actor = new ActorCreationDTO() { Name = "Josue", Birthdate = new DateTime(1986, 11, 8) };
        var mock = new Mock<IFileStorage>();
        mock.Setup(x => x.SaveFile(null, null, null, null)).Returns(Task.FromResult("url"));

        var controller = new ActorsController(context, mapper, mock.Object);
        var respuesta = await controller.Post(actor);
        var result = respuesta as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = ConstruirContext(nombreBd);
        var list = await context2.Actors.ToListAsync();
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("", list[0].Picture);

        Assert.AreEqual(0, mock.Invocations.Count);
    }

    [TestMethod]
    public async Task CrearActorConFoto() {
        // Preparar
        var nombreBd = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreBd);
        var mapper = ConfigurarAutoMapper();

        var content = Encoding.UTF8.GetBytes("Imagen de prueba");
        var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
        file.Headers = new HeaderDictionary();
        file.ContentType = "image/jpg";

        var actor = new ActorCreationDTO() {
            Name = "Josue",
            Birthdate = DateTime.Now,
            Picture = file
        };

        var mock = new Mock<IFileStorage>();
        mock.Setup(x => x.SaveFile(content, ".jpg", "favorites", file.ContentType))
            .Returns(Task.FromResult("url"));

        var controller = new ActorsController(context, mapper, mock.Object);
        var respuesta = await controller.Post(actor);
        var result = respuesta as CreatedAtRouteResult;
        Assert.AreEqual(201, result.StatusCode);

        var context2 = ConstruirContext(nombreBd);
        var list = await context2.Actors.ToListAsync();
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("url", list[0].Picture);
        Assert.AreEqual(1, mock.Invocations.Count);
    }

    [TestMethod]
    public async Task PatchRetorna404SiActorNoExiste() {
        // Preparar
        var nombreBd = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreBd);
        var mapper = ConfigurarAutoMapper();

        var controller = new ActorsController(context, mapper, null);
        var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
        var respuesta = await controller.Patch(1, patchDoc);
        var result = respuesta as StatusCodeResult;
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task PatchActualizarUnSoloCampo() {
        // Preparar
        var nombreBd = Guid.NewGuid().ToString();
        var context = ConstruirContext(nombreBd);
        var mapper = ConfigurarAutoMapper();

        var fechaNacimiento = DateTime.Now;
        var actor = new Actor() { Name = "Josue", Birthdate = fechaNacimiento };
        context.Add(actor);
        await context.SaveChangesAsync();

        var context2 = ConstruirContext(nombreBd);
        var controller = new ActorsController(context2, mapper, null);

        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

        controller.ObjectValidator = objectValidator.Object;

        var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
        patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/name", null, "Josue"));
        var respuesta = await controller.Patch(1, patchDoc);
        var result = respuesta as StatusCodeResult;
        Assert.AreEqual(204, result.StatusCode);

        var context3 = ConstruirContext(nombreBd);
        var actordb = await context3.Actors.FirstAsync();
        Assert.AreEqual("Josue", actordb.Name);
        Assert.AreEqual(fechaNacimiento, actordb.Birthdate);
    }
}