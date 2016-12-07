using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
/// <summary>
/// Buffer that holds all player inputs,
/// </summary>
public class Buffer : MonoBehaviour
{
    //number of frames to keep an input
    [SerializeField]
    private int bufferSize = 6;

    /// <summary>
    /// list of inputs, contexted with a number of frames it has been in the buffer
    /// </summary>
    private List<InputPair> buffer;

    public void Awake()
    {
        buffer = new List<InputPair>();
        StartCoroutine("CleanBuffer");
    }

    private IEnumerator CleanBuffer()
    {
        while (true)
        {
            //delete inputs that are too stale
            while (buffer.Count > 0 && buffer[0].frames > bufferSize) buffer.RemoveAt(0);
            //and increment all surviving frames
            foreach (InputPair ip in buffer) ++ip.frames;
            //cya next frame nerds
            yield return null;
        }
    }

    public void ClearBuffer()
    {
        buffer.Clear();
    }

    public void RegisterInput(GameInput input)
    {
        buffer.Add(new InputPair(input));
    }

    public bool Contains(GameInput input)
    {
        for(int ii = 0; ii < buffer.Count; ++ii)
        {
            if (buffer[ii].GetInput == input) return true;
        }
        return false;
    }

    public GameInput this[int key]
    {
        //just return the input, as it should not be modified outside this class
        get
        {
            return buffer[key].GetInput;   
        }
    }
    public int Count { get { return buffer.Count; } }

}


/// <summary>
/// Basically a pair struct, holding an input, and the number of frames it has been in the buffer
/// </summary>
public class InputPair
{
    private GameInput input;
    public int frames;

    public GameInput GetInput { get { return input; } }
    public InputPair(InputPair ip)
    {
        this.input = ip.input;
        this.frames = ip.frames;
    }

    public InputPair(GameInput input)
    {
        this.input = input;
        frames = 0;
    }
}