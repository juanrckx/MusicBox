using System;
using System.Linq;
using MusicBox.DataStructures;
using MusicBox.Models;

namespace MusicBox.Audio;

// IDisposable significa, luego liberará los recursos usados
public class AudioEngine : IDisposable 
{
    // Produce los sonidos y una vez asignado al constructor no podrá cambiar por "readonly"
    private readonly ToneGenerator _toneGenerator;

    // Cuándot es la duración base
    private double _baseDuration = 1.0;
    
    // Constructorr
    public AudioEngine()
    {
        _toneGenerator = new ToneGenerator();       // Crear generador
    }
    
    // Método para cambiar la velocidad base
    public void SetBaseDuration(double seconds)
    {
        // Validar (igual que en ScoreParser)
        _baseDuration = seconds;
    }
    
    // Método princiapl que reproduce toda la partitura
    public void PlayScore(DoublyLinkedList<Note> score, bool forward = true)
    {
        // Verificar si hay notas
        if (score == null || score.Count == 0)
        {
            Console.WriteLine("No hay notas a reproducir.");
            return;
        }
        
        Console.WriteLine($"Reproduciendo partitura con ({score.Count} notas)...");
        
        // Decidir dirección
        var notes = forward 
            ? score.TraverseForward().ToList()
            : score.TraverseBackward().ToList();
        
        // Reproducir cada nota
        foreach (var note in notes)
        {
            PlayNote(note);
        }
        
        Console.WriteLine("Reproducción completa.");
    }
    
    private void PlayNote(Note note)
    {
        if (note == null || note.Duration == null)
            return;
        
        // Calcular duración real
        double durationSeconds = note.Duration.GetSeconds();
        
        Console.WriteLine($"Playing: {note.Name} ({note.Frequency:F2}Hz) for {durationSeconds:F2}s");
        
        try
        {
            // Pedir al ToneGenerator que toque esta nota
            _toneGenerator.PlayTone(note.Frequency, durationSeconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reproduciendo: {note.Name}: {ex.Message}");
        }
    }
    
    public void Dispose()
    {
        _toneGenerator?.Dispose();      // Liberar recursos
    }
}