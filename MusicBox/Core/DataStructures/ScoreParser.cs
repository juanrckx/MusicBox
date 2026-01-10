using System;
using System.Collections.Generic;
using MusicBox.Models;

namespace MusicBox.DataStructures;

public class ScoreParser
{
    private double _baseDuration; // Duracion de la negra en segundos
    
    public ScoreParser(double baseDuration = 1.0)
    {
        // Validar que la duración esté entre 0.1 y 5.0 segundos
        if (baseDuration < 0.1 || baseDuration > 5.0)
            throw new ArgumentException("La duracion de la base debe estar entre 0.1 y 5.0 segundos");
        
        _baseDuration = baseDuration;
    }

    // Método principal: convierte el texto en lista de notas
    public DoublyLinkedList<Note> Parse(string scoreString)
    {
        var list = new DoublyLinkedList<Note>();

        // Dividir "(Do, negra), (Mi, blanca)" en ["(Do, negra)", "(Mi, blanca)"]
        string[] notePairs = scoreString.Split(new string[] {"), ("}, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string pair in notePairs)
        {
            // Limpiar paréntesis
            string cleanPair = pair.Trim('(', ')', ' ');

            // Separar por coma: "Do, negra" -> ["Do", "negra"]
            string[] parts = cleanPair.Split(',');

            if (parts.Length != 2)  // Validar formato
                throw new FormatException($"Formato inválido: {pair}");

            // Convertir texto a los enums
            NoteName noteName = ParseNoteName(parts[0].Trim());     // "Do" -> NoteName.Do
            FigureType figure = ParseFigureType(parts[1].Trim());   // "negra" -> FigureType.Negra

            // Crear la nota completa
            Note note = new Note()
            {
                Name = noteName,
                Frequency = Note.GetFrequency(noteName),    // Usar el método estático de Note
                Duration = new Duration
                {
                    Figure = figure,
                    BaseSeconds = _baseDuration             // Usar la duración base configurada
                }
            };

            list.AddLast(note);                             // Añade a la lista
        }

        return list;                                        // Devuelve la lista completa
    }

    // Métodos helper para convertir a texto
    private NoteName ParseNoteName(string name)
    {
        return name.ToLower() switch            // Convertir a minúsculas y comparar
        {
            "do" => NoteName.Do,
            "re" => NoteName.Re,
            "mi" => NoteName.Mi,
            "fa" => NoteName.Fa,
            "sol" => NoteName.Sol,
            "la" => NoteName.La,
            "si" => NoteName.Si,
            _ => throw new ArgumentException($"Nombre de nota inválida: {name}")
        };
    }

    private FigureType ParseFigureType(string figure)
    {
        return figure.ToLower() switch
        {
            "redonda" => FigureType.Redonda,
            "blanca" => FigureType.Blanca,
            "negra" => FigureType.Negra,
            "corchea" => FigureType.Corchea,
            "semicorchea" => FigureType.Semicorchea,
            _ => throw new ArgumentException($"Figura inválida: {figure}")

        };
    }
}