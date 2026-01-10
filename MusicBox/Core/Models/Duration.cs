using System.Collections.Generic;

namespace MusicBox.Models;

// Reloj de cada nota
public class Duration
{
    public FigureType Figure {get; set;} // Qué figura es
    public double BaseSeconds {get; set;} // Cuánto dura una negra

    // De esta manera, el diccionario dice "Si es negra, multiplícalo por 1"
    // "Si es blanca, multiplica por 2", etc.
    private static readonly Dictionary<FigureType, double> Multipliers = new()
    {
        {FigureType.Redonda, 4.0},     // 4 veces una negra
        {FigureType.Blanca, 2.0},      // 2 veces una negra
        {FigureType.Negra, 1.0},       // 1 vez ella misma
        {FigureType.Corchea, 0.5},     // Mitad de una negra
        {FigureType.Semicorchea, 0.25} // Cuarto de una negra
    };

    // Calcula el tiempo real en segundos    
    public double GetSeconds() => BaseSeconds * Multipliers[Figure];

}
