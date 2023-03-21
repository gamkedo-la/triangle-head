using System;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * A class that collects audio clips together, and allows them to be played in random order or in sequence
 */
[CreateAssetMenu(menuName = "Triangle Head/Multi Audio Clip")]
public class MultiAudioClip : ScriptableObject
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private PlayStyle playStyle;

    private int lastPlayedClip = -1;

    public void PlayOn(AudioSource audioSource)
    {
        var clipIndex = playStyle switch
        {
            PlayStyle.Random => RandomIndex(),
            PlayStyle.RandomButNotLastPlayed => RandomIndex(lastPlayedClip),
            PlayStyle.Sequence => NextIndex(),
            _ => throw new ArgumentException($"Unknown PlayStyle: {playStyle}")
        };
        audioSource.PlayOneShot(audioClips[clipIndex]);
        lastPlayedClip = clipIndex;
    }

    private int NextIndex()
    {
        return (lastPlayedClip + 1) % audioClips.Length;
    }

    private int RandomIndex(int excluding = -1)
    {
        var audioClipsLength = audioClips.Length;
        if (audioClipsLength < 2) return audioClipsLength - 1;
        
        var index = excluding;
        while (index == excluding)
        {
            index = Random.Range(0, audioClipsLength);
        }

        return index;
    }

    [Serializable]
    public enum PlayStyle
    {
        Random,
        RandomButNotLastPlayed,
        Sequence,
    }
}

public static class MultiAudioClipHelper
{
    public static void PlayOneShot(this AudioSource audioSource, MultiAudioClip clip)
    {
        clip.PlayOn(audioSource);
    }
}