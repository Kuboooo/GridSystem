using System;
using System.Collections.Generic;

public class ToppingsOrganizer {
    public static readonly string Mushrooms = nameof(Mushrooms);
    public static readonly string Corn = nameof(Corn);
    public static readonly string Olives = nameof(Olives);
    public static readonly string Salami = nameof(Salami);
    public static readonly string Prosciutto = nameof(Prosciutto);
    public static readonly string Artichokes = nameof(Artichokes);
    public static readonly string Tuna = nameof(Tuna);
    public static readonly string Bacon = nameof(Bacon);
    public static readonly string Pineapple = nameof(Pineapple);

    private static readonly Dictionary<string, Dictionary<string, int>> ToppingCombinations =
        new() {
            {
                Mushrooms, new Dictionary<string, int> {
                    { Mushrooms, 0 },
                    { Corn, 33 },
                    { Olives, 28 },
                    { Salami, 83 },
                    { Prosciutto, 88 },
                    { Artichokes, 28 },
                    { Tuna, 10 },
                    { Bacon, 70 },
                    { Pineapple, 5 }
                }
            }, {
                Corn, new Dictionary<string, int> {
                    { Mushrooms, 33 },
                    { Corn, 0 },
                    { Olives, 53 },
                    { Salami, 80 },
                    { Prosciutto, 78 },
                    { Artichokes, 40 },
                    { Tuna, 68 },
                    { Bacon, 73 },
                    { Pineapple, 28 }
                }
            }, {
                Olives, new Dictionary<string, int> {
                    { Mushrooms, 28 },
                    { Corn, 53 },
                    { Olives, 0 },
                    { Salami, 95 },
                    { Prosciutto, 93 },
                    { Artichokes, 60 },
                    { Tuna, 68 },
                    { Bacon, 80 },
                    { Pineapple, 20 }
                }
            }, {
                Salami, new Dictionary<string, int> {
                    { Mushrooms, 83 },
                    { Corn, 80 },
                    { Olives, 95 },
                    { Salami, 0 },
                    { Prosciutto, 58 },
                    { Artichokes, 75 },
                    { Tuna, 23 },
                    { Bacon, 38 },
                    { Pineapple, 68 }
                }
            }, {
                Prosciutto, new Dictionary<string, int> {
                    { Mushrooms, 88 },
                    { Corn, 78 },
                    { Olives, 93 },
                    { Salami, 58 },
                    { Prosciutto, 0 },
                    { Artichokes, 73 },
                    { Tuna, 8 },
                    { Bacon, 50 },
                    { Pineapple, 75 }
                }
            }, {
                Artichokes, new Dictionary<string, int> {
                    { Mushrooms, 28 },
                    { Corn, 40 },
                    { Olives, 60 },
                    { Salami, 75 },
                    { Prosciutto, 73 },
                    { Artichokes, 0 },
                    { Tuna, 30 },
                    { Bacon, 68 },
                    { Pineapple, 15 }
                }
            }, {
                Tuna, new Dictionary<string, int> {
                    { Mushrooms, 10 },
                    { Corn, 68 },
                    { Olives, 68 },
                    { Salami, 23 },
                    { Prosciutto, 8 },
                    { Artichokes, 30 },
                    { Tuna, 0 },
                    { Bacon, 40 },
                    { Pineapple, 23 }
                }
            }, {
                Bacon, new Dictionary<string, int> {
                    { Mushrooms, 70 },
                    { Corn, 73 },
                    { Olives, 80 },
                    { Salami, 38 },
                    { Prosciutto, 50 },
                    { Artichokes, 68 },
                    { Tuna, 40 },
                    { Bacon, 0 },
                    { Pineapple, 70 }
                }
            }, {
                Pineapple, new Dictionary<string, int> {
                    { Mushrooms, 5 },
                    { Corn, 28 },
                    { Olives, 20 },
                    { Salami, 68 },
                    { Prosciutto, 75 },
                    { Artichokes, 15 },
                    { Tuna, 23 },
                    { Bacon, 70 },
                    { Pineapple, 0 }
                }
            }
        };

    public static int GetCombinationValue(string topping1, string topping2) {
        if (ToppingCombinations.ContainsKey(topping1) && ToppingCombinations[topping1].ContainsKey(topping2)) {
            return ToppingCombinations[topping1][topping2];
        }

        return -1; // Return -1 if the combination does not exist
    }
}