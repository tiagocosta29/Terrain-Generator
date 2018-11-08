using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour {

    public AudioSource Source;
    public List<AudioClip> AudioClips;
    private int[] clipOrder;
    private System.Random random = new System.Random();
    private int currIndex = 0;

    private void Start()
    {
        Source.loop = false;
        clipOrder = new int[AudioClips.Count];

        for (int i = 0; i < clipOrder.Length; i++)
        {
            clipOrder[i] = i;
        }

        clipOrder = Shuffle<int>(clipOrder);        
    }

    private void Update()
    {
        if (!Source.isPlaying)
        {
            Source.clip = AudioClips[clipOrder[currIndex]];
            Source.Play();
            if (currIndex < AudioClips.Count - 1)
            {
                currIndex++;
            }            
        }
    }

    public T[] Shuffle<T>(T[] array)
    {
        for (int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap.
            int j = random.Next(i); // 0 <= j <= i-1
                                    // Swap.
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;
        }
        return array;
    }
}
