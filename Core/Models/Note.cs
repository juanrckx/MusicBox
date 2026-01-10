using System.Collections.Generic;

namespace MusicBox.Models;

public class Note
{
    public NoteName Name {get; set;}        // Nombre, (Do, Re, Mi...)
    public double  Frequency {get; set;}    // Frecuencia en Hz
    public Duration Duration {get; set;}    // Duración de la nota

    // Este diccionario relaciona nombres de notas con frecuencias
    private static readonly Dictionary <NoteName, double> Frequencies = new()
    {
        {NoteName.Do, 261.63},
        {NoteName.Re, 293.66},
        {NoteName.Mi, 329.63},
        {NoteName.Fa, 349.23},
        {NoteName.Sol, 392.00},
        {NoteName.La, 440.00},
        {NoteName.Si, 493.88}
    };

    // Metodo estático
    public static double GetFrequency(NoteName noteName)
    {
        return Frequencies[noteName]; // Busca la frecuencia en el diccionario
    }
}