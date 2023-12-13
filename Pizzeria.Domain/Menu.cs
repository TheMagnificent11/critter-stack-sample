namespace Pizzeria.Domain;

public static class Menu
{
    public static readonly Pizza[] Pizzas = new Pizza[]
    {
        Margherita!,
        Marinara!,
        QuattroStagioni!,
        Carbonara!,
        FruttiDiMare!,
        QuattroFormaggi!,
        Crudo!,
        Napoletana!,
        Pugliese!,
        Montanara!
    };

    private static readonly Pizza Margherita = new(1, "Margherita", "Tomato sauce, mozzarella, and oregano", 5.00m);
    private static readonly Pizza Marinara = new(2, "Marinara", "Tomato sauce, garlic and basil", 5.50m);
    private static readonly Pizza QuattroStagioni = new(
        3,
        "Quattro Stagioni",
        "Tomato sauce, mozzarella, mushrooms, ham, artichokes, olives, and oregano",
        8.00m);
    private static readonly Pizza Carbonara = new(
        4,
        "Carbonara",
        "Tomato sauce, mozzarella, parmesan, eggs, and bacon",
        8.50m);
    private static readonly Pizza FruttiDiMare = new(5, "Frutti di Mare", "Tomato sauce and seafood", 8.50m);
    private static readonly Pizza QuattroFormaggi = new(
        6,
        "Quattro Formaggi",
        "Tomato sauce, mozzarella, parmesan, gorgonzola cheese, artichokes, and oregano",
        8.50m);
    private static readonly Pizza Crudo = new(
        7,
        "Crudo",
        "Tomato sauce, mozzarella, Parma ham, parmesan, mushrooms, and oregano",
        9.00m);
    private static readonly Pizza Napoletana = new(
        8,
        "Napoletana",
        "Tomato sauce, mozzarella, oregano, anchovies",
        9.00m);
    private static readonly Pizza Pugliese = new(
        9,
        "Pugliese",
        "Tomato sauce, mozzarella, oregano, and onions",
        9.00m);
    private static readonly Pizza Montanara = new(
        10,
        "Montanara",
        "Tomato sauce, mozzarella, mushrooms, pepperoni, and oregano",
        9.00m);
}
