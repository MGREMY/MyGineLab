using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Collections;

namespace MyGine.Audio;

/// <summary>
/// Wrapper around <see cref="MediaPlayer"/> and <see cref="SoundEffect"/>
/// with a centralized mute/unmute functionality.
/// </summary>
public sealed class AudioController
{
    private readonly Bag<SoundEffectInstance> _activeSoundEffects = [];
    private float _previousSongVolume;
    private float _previousSoundEffectVolume;
    public bool IsMuted { get; set; }

    public float SongVolume
    {
        get
        {
            if (IsMuted) return 0.0f;
            return MediaPlayer.Volume;
        }
        set
        {
            if (IsMuted) return;
            MediaPlayer.Volume = value;
        }
    }

    public float EffectVolume
    {
        get
        {
            if (IsMuted) return 0.0f;
            return SoundEffect.MasterVolume;
        }
        set
        {
            if (IsMuted) return;
            SoundEffect.MasterVolume = value;
        }
    }

    public void Update()
    {
        // Reverse counting so that we can remove items while iterating the collection
        for (var i = _activeSoundEffects.Count - 1; i >= 0; i--)
        {
            var instance = _activeSoundEffects[i];

            if (instance.State is not SoundState.Stopped) continue;

            if (!instance.IsDisposed) instance.Dispose();
            _activeSoundEffects.RemoveAt(i);
        }
    }

    public SoundEffectInstance Play(
        SoundEffect source,
        bool isLooped = false,
        float volume = 1.0f,
        float pitch = 0.0f,
        float pan = 0.0f)
    {
        var instance = source.CreateInstance();

        instance.IsLooped = isLooped;
        instance.Volume = volume;
        instance.Pitch = pitch;
        instance.Pan = pan;

        instance.Play();
        _activeSoundEffects.Add(instance);

        return instance;
    }

    public void Play(
        Song source,
        bool isLooped = true)
    {
        if (MediaPlayer.State is MediaState.Playing) MediaPlayer.Stop();

        MediaPlayer.Play(source);
        MediaPlayer.IsRepeating = isLooped;
    }

    public void Pause()
    {
        MediaPlayer.Pause();

        foreach (var instance in _activeSoundEffects)
        {
            instance.Pause();
        }
    }

    public void Resume()
    {
        MediaPlayer.Resume();

        foreach (var instance in _activeSoundEffects)
        {
            instance.Resume();
        }
    }

    public void Mute()
    {
        _previousSongVolume = MediaPlayer.Volume;
        _previousSoundEffectVolume = SoundEffect.MasterVolume;

        MediaPlayer.Volume = 0.0f;
        SoundEffect.MasterVolume = 0.0f;

        IsMuted = true;
    }

    public void Unmute()
    {
        MediaPlayer.Volume = _previousSongVolume;
        SoundEffect.MasterVolume = _previousSoundEffectVolume;

        IsMuted = false;
    }

    public void ToggleMute()
    {
        if (IsMuted) Unmute();
        else Mute();
    }
}