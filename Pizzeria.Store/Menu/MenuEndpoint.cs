using Pizzeria.Domain;
using Wolverine.Http;

namespace Pizzeria.Store.Menu;

public class MenuEndpoint
{
    [WolverineGet("/menu")]
    public IEnumerable<Pizza> Get() => Domain.Menu.Pizzas;
}
